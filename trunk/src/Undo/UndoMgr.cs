using System;
using System.Collections.Generic;
using System.Text;

namespace Spritely
{
	/// <summary>
	/// The UndoMgr manages the stack of UndoActions.
	/// Each UndoAction contains a before/after snapshot.
	/// The current entry into the stack points to the current (after) snapshot.
	/// 
	/// When Undo is selected:
	///   The before snapshot of the current UndoAction is applied
	///   The current UndoAction index is decremented
	/// 
	/// When Redo is selected:
	///   The current UndoAction index is incremented
	///   The after snapshot of the current UndoAction is applied
	/// 
	/// If all UndoActions on the stack are the same type and for the same sprite,
	/// then the nth item's after snapshot is the same as the (n+1)th item's before
	/// snapshot.
	/// However, when the UndoActions are of different types or are for different
	/// sprites, then the duplicate info makes is easier to apply the undo/redo actions.
	/// </summary>
	public class UndoMgr
	{
		/// <summary>
		/// A stack of the editing history.
		/// </summary>
		List<UndoAction> m_history;

		/// <summary>
		/// Debug window to display the current contents of the Undo stack.
		/// </summary>
		static UndoHistory s_formView = new UndoHistory();

		static bool s_fUndoFormVisible = false;

		/// <summary>
		/// Maximum depth of undo events stack.
		/// </summary>
		const int k_nMaxUndoHistory = 100;

		/// <summary>
		/// The current entry in the undo stack.
		/// This points to the snapshot that corresponds to the current state being
		/// shown to the user.
		/// </summary>
		int m_nCurrent;

		int m_nID;
		static int s_nNextID = 1;

		public UndoMgr()
		{
			m_history = new List<UndoAction>();
			m_nCurrent = -1;

			m_nID = s_nNextID++;
		}

		public void Reset()
		{
			m_history.Clear();
			m_nCurrent = -1;
			s_formView.Reset();
		}

		static public bool ShowDebugWindow
		{
			get { return s_fUndoFormVisible; }
			set { s_fUndoFormVisible = value; if (s_fUndoFormVisible) s_formView.Show(); else s_formView.Hide(); }
		}

		static public void LoadDebugWindow(UndoMgr undo)
		{
			s_formView.Reset();
			if (undo == null)
				return;

			foreach (UndoAction action in undo.m_history)
				s_formView.Add(action);
			s_formView.SetCurrent(undo.m_nCurrent);
		}

		public bool CanUndo()
		{
			return m_nCurrent >= 0;
		}

		public bool CanRedo()
		{
			return m_history.Count > m_nCurrent + 1;
		}

		public UndoAction GetCurrent()
		{
			if (m_nCurrent < 0 || m_nCurrent >= m_history.Count)
				return null;
			return m_history[m_nCurrent];
		}

		public void Push(UndoAction action)
		{
			// Remove any "redo" actions from the history.
			//   +---+   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | 3 | > | 4 |
			//   +---+   +---+   +---+   +---+   +---+
			//                     ^current
			//   count = 5; current = 2;
			//   Remove the last 2 items.
			int nCount = m_history.Count;
			if (nCount > m_nCurrent + 1)
			{
				m_history.RemoveRange(m_nCurrent + 1, nCount - (m_nCurrent + 1));
				s_formView.RemoveRange(m_nCurrent + 1, nCount - (m_nCurrent + 1));
			}

			// Add the new item at the end.
			//   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | N |
			//   +---+   +---+   +---+   +---+
			//                     ^old    ^current
			m_history.Add(action);
			s_formView.Add(action);
			m_nCurrent++;
			s_formView.SetCurrent(m_nCurrent);

			// Remove the oldest action if the stack is too large.
			//   +---+   +---+   +---+
			//   | 1 | > | 2 | > | N |
			//   +---+   +---+   +---+
			//                     ^current
			if (m_history.Count > k_nMaxUndoHistory)
			{
				m_history.RemoveAt(0);
				s_formView.Remove(0);
				m_nCurrent--;
				s_formView.SetCurrent(m_nCurrent);
			}
		}

		/// <summary>
		/// Remove the current entry (and everything after it) from the UndoAction list.
		/// This is called instead of Push() when 2 consecutive UndoActions would cancel
		/// each other out.
		/// </summary>
		public void DeleteCurrent()
		{
			int nCount = m_history.Count;
			m_history.RemoveRange(m_nCurrent, nCount - m_nCurrent);
			s_formView.RemoveRange(m_nCurrent, nCount - m_nCurrent);
			m_nCurrent--;
			s_formView.SetCurrent(m_nCurrent);
		}

		public void ApplyUndo()
		{
			if (m_nCurrent < 0 || m_nCurrent >= m_history.Count)
				return;

			// Apply the current undo action
			m_history[m_nCurrent].ApplyUndo();

			// Decrement the current undo action index.
			//   +---+   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | 3 | > | 4 |
			//   +---+   +---+   +---+   +---+   +---+
			//             ^new    ^old
			//   Decrement current from old to new
			if (m_nCurrent >= 0)
			{
				m_nCurrent--;
				s_formView.SetCurrent(m_nCurrent);
			}
		}

		public void ApplyRedo()
		{
			if (m_nCurrent < 0 || m_nCurrent >= m_history.Count)
				return;

			// Increment the current undo action index.
			//   +---+   +---+   +---+   +---+   +---+
			//   | 0 | > | 1 | > | 2 | > | 3 | > | 4 |
			//   +---+   +---+   +---+   +---+   +---+
			//                     ^old    ^new
			//   Increment current from old to new and return that UndoAction (for Redo)
			if (m_history.Count > m_nCurrent + 1)
			{
				m_nCurrent++;
				s_formView.SetCurrent(m_nCurrent);
			}

			// Apply the current redo action
			m_history[m_nCurrent].ApplyRedo();
		}

		/// <summary>
		/// Scan backwards in the undo stack for the most recently edited sprite.
		/// This is used to select a sprite when the current one is deleted.
		/// </summary>
		/// <returns>The most recent Sprite, or null if a suitable Sprite cannot be found.</returns>
		public Sprite FindMostRecentSprite()
		{
			for (int i = m_nCurrent-1; i >= 0; i--)
			{
				// Use sprite from most recent edit.
				UndoAction_SpriteEdit action_edit = m_history[i] as UndoAction_SpriteEdit;
				if (action_edit != null)
					return action_edit.GetSprite;

				// Use sprite from most recent Add (not Delete, since the sprite doesn't exist anymore).
				UndoAction_AddSprite action_add = m_history[i] as UndoAction_AddSprite;
				if (action_add != null && action_add.Add)
					return action_add.GetSprite;
			}
			return null;
		}
	}
}
