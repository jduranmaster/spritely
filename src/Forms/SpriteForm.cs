using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class SpriteForm : Form
	{
		private ProjectMainForm m_parent;
		private Sprite m_sprite;

		private Toolbox_Sprite m_toolbox;

		public bool HasLocation = false;

		public SpriteForm(ProjectMainForm parent, Sprite s)
		{
			m_parent = parent;
			m_sprite = s;

			InitializeComponent();

			m_toolbox = new Toolbox_Sprite();

			MdiParent = parent;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			Text = "Sprite '" + s.Name + "'";
		}

		private void pbTools_Paint(object sender, PaintEventArgs e)
		{
			m_toolbox.Draw(e.Graphics);
		}

		private void pbTools_MouseDown(object sender, MouseEventArgs e)
		{
		
		}

		private void pbTools_MouseLeave(object sender, EventArgs e)
		{
		
		}

		private void pbTools_MouseMove(object sender, MouseEventArgs e)
		{
		
		}

		private void pbTools_MouseUp(object sender, MouseEventArgs e)
		{
		
		}

		private void pbSprite_Paint(object sender, PaintEventArgs e)
		{
			m_sprite.DrawEditSprite(e.Graphics);
		}

		private void pbSprite_MouseDown(object sender, MouseEventArgs e)
		{

		}

		private void pbSprite_MouseMove(object sender, MouseEventArgs e)
		{

		}

		private void pbSprite_MouseUp(object sender, MouseEventArgs e)
		{

		}

		private void cbZoom_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

	}
}