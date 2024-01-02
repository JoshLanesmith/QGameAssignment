/* 
 * ControlPanelForm.cs
 * Assignment 3
 * Revision History
 *      Josh Lanesmith 2023-10-30: Created
 *      Josh Lanesmith 2023-11-27: Updated for assignment 3
 */
using System;
using System.Windows.Forms;

namespace JLanesmithQGame
{
    /// <summary>
    /// Control panel form to provide user with options to design a level or play a level
    /// </summary>
    public partial class ControlPanelForm : Form
    {
        public ControlPanelForm()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnDesign_Click(object sender, EventArgs e)
        {
            DesignForm designForm = new DesignForm();

            designForm.Show();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayForm playForm = new PlayForm();

            playForm.Show();
        }
    }
}
