using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class ProjectTreeViewForm : Form
	{
		private class NodeInfo
		{
			public enum Class
			{
				Title,
				Group,
				Data,
			};
			public enum Type
			{
				Unknown,
				Spriteset,
				Palette,
				Backgrounds,	// Only used for Class = Title
				BGTileset,
				BGPalette,
				BGMap,
				BGImage,
				Sound
			};

			public Class node_class;
			public Type node_type;

			public NodeInfo(Class c, Type t)
			{
				node_class = c;
				node_type = t;
			}
		}

		private ProjectMainForm m_parent;
		private Document m_doc;

		public ProjectTreeViewForm(ProjectMainForm parent, Document doc)
		{
			m_parent = parent;
			m_doc = doc;

			InitializeComponent();

			// Set up context menus for tree view
			treeView.MouseUp += new MouseEventHandler(treeView_MouseUp);

			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			MdiParent = parent;
			Left = 0;
			Top = 0;
			Width = 185;
			Height = parent.ClientSize.Height - parent.MenuBarHeight - 6;
			Text = "Project 'Untitled'";
		}

		private Font m_fontBold;

		private TreeNode AddTitleNode(TreeNodeCollection tnc, String strName, NodeInfo.Type type)
		{
			TreeNode tn = new TreeNode(strName);
			tnc.Add(tn);
			tn.NodeFont = m_fontBold;
			tn.Tag = new NodeInfo(NodeInfo.Class.Title, type);
			return tn;
		}

		private TreeNode AddDataNode(TreeNodeCollection tnc, String strName, NodeInfo.Type type)
		{
			TreeNode tn = new TreeNode(strName);
			tnc.Add(tn);
			tn.Tag = new NodeInfo(NodeInfo.Class.Data, type);
			return tn;
		}

		private void ProjectTreeViewForm_Load(object sender, EventArgs e)
		{
			// Begin bulk update of treeview - disable repainting.
			treeView.BeginUpdate();

			m_fontBold = new Font(treeView.Font, FontStyle.Bold);

			TreeNode nodeSprites = AddTitleNode(treeView.Nodes, "Spritesets", NodeInfo.Type.Spriteset);
			AddDataNode(nodeSprites.Nodes, m_doc.Spritesets.Current.Name, NodeInfo.Type.Spriteset);

			TreeNode nodePalettes = AddTitleNode(treeView.Nodes, "Palettes", NodeInfo.Type.Palette);
			AddDataNode(nodePalettes.Nodes, m_doc.GetSpritePalette(Options.DefaultPaletteId).NameDesc, NodeInfo.Type.Palette);
			//AddDataNode(nodePalettes.Nodes, "default256 [256 color]", NodeInfo.Type.Palette);

			TreeNode nodeBackgrounds = AddTitleNode(treeView.Nodes, "Backgrounds", NodeInfo.Type.Backgrounds);
			{
				TreeNode nodeBGTiles = AddTitleNode(nodeBackgrounds.Nodes, "Background Tilesets", NodeInfo.Type.BGTileset);
				AddDataNode(nodeBGTiles.Nodes, "bg_tiles", NodeInfo.Type.BGTileset);

				TreeNode nodeBGPalettes = AddTitleNode(nodeBackgrounds.Nodes, "Background Palettes", NodeInfo.Type.BGPalette);
				AddDataNode(nodeBGPalettes.Nodes, m_doc.GetBackgroundPalette(Options.DefaultBgPaletteId).NameDesc, NodeInfo.Type.BGPalette);
				//AddDataNode(nodeBGPalettes.Nodes, "default256 [256 color]", NodeInfo.Type.BGPalette);

				TreeNode nodeBGMaps = AddTitleNode(nodeBackgrounds.Nodes, "Background Tile Maps", NodeInfo.Type.BGMap);
				AddDataNode(nodeBGMaps.Nodes, "bg_map", NodeInfo.Type.BGMap);

				//TreeNode nodeBGImages = AddTitleNode(nodeBackgrounds.Nodes, "Background Images", NodeInfo.Type.BGImage);
			}

			//TreeNode nodeSounds = AddTitleNode(treeView.Nodes, "Sounds", NodeInfo.Type.Sound);

			foreach (TreeNode n in treeView.Nodes)
				n.ExpandAll();
			Controls.Add(treeView);

			// Treeview update complete, re-enable painting.
			treeView.EndUpdate();
		}

		private void ProjectTreeViewForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Hide();
			m_parent.CheckProjectList(false);
			e.Cancel = true;
		}

		/// <summary>
		/// Used to keep track of the node clicked on for the context menu.
		/// </summary>
		private TreeNode m_contextNode = null;

		private void cmenuSpriteset_Open_Click(object sender, EventArgs e)
		{
			if (m_contextNode == null || m_contextNode.Tag == null)
				return;
			m_parent.OpenSpritesetWindow();
		}

		private void treeView_MouseUp(object sender, MouseEventArgs e)
		{
			m_contextNode = null;
			if (e.Button == MouseButtons.Right)
			{
				Point pt = new Point(e.X, e.Y);
				TreeNode node = treeView.GetNodeAt(pt);
				if (node != null)
				{
					// Record the context menu node.
					m_contextNode = node;

					NodeInfo ninfo = node.Tag as NodeInfo;
					if (ninfo.node_class == NodeInfo.Class.Title)
					{
						if (ninfo.node_type == NodeInfo.Type.Spriteset)
							cmenuSpritesetTitle.Show(treeView, pt);
					}
					else if (ninfo.node_class == NodeInfo.Class.Data)
					{
						switch (ninfo.node_type)
						{
							case NodeInfo.Type.Spriteset:
								cmenuSpriteset.Show(treeView, pt);
								break;
						}
					}
				}
			}
		}

		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			TreeView tv = sender as TreeView;
			NodeInfo ninfo = e.Node.Tag as NodeInfo;
			if (ninfo != null && ninfo.node_class != NodeInfo.Class.Title)
			{
				switch (ninfo.node_type)
				{
					case NodeInfo.Type.Spriteset:
						m_parent.OpenSpritesetWindow();
						break;
					case NodeInfo.Type.Palette:
						m_parent.OpenPalette16Window();
						break;
					default:
						MessageBox.Show("Ouch!");
						break;
				}
			}
		}

	}
}
