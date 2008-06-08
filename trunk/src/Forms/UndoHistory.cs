using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class UndoHistory : Form
	{
		public UndoHistory()
		{
			InitializeComponent();
		}

		public void Reset()
		{
			lbUndo.Items.Clear();
		}

		public void Add(UndoAction action)
		{
			lbUndo.Items.Add(action.Description);
		}

		public void Remove(int nIndex)
		{
			lbUndo.Items.RemoveAt(nIndex);
		}

		public void RemoveRange(int nStart, int nCount)
		{
			for (int i = 0; i < nCount; i++)
				lbUndo.Items.RemoveAt(nStart);
		}

		public void SetCurrent(int nIndex)
		{
			// TODO: fix out of range index when switching tabs and continue editing
			if (nIndex >= lbUndo.Items.Count)
				return;
			lbUndo.SelectedIndex = nIndex;
		}

		private void bOK_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void UndoHistory_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.Hide();
		}
	}
}