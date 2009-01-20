using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Spritely
{
	public partial class ProjectMainForm : Form
	{
		public void CheckProjectList(bool check)
		{
			//menuWindow_ProjectList.CheckState = (check ? CheckState.Checked : CheckState.Unchecked);
		}

		/// <summary>
		/// Show/hide the project list window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuWindow_ProjectList_Click(object sender, EventArgs e)
		{
			//if (menuWindow_ProjectList.CheckState == CheckState.Checked)
			{
			//	m_ProjectTreeView.Hide();
			//	CheckProjectList(false);
			}
			//else
			{
			//	m_ProjectTreeView.Show();
			//	CheckProjectList(true);
			}
		}
	}
}