/* 
 * PlayForm.cs
 * Assignment 3
 * Revision History
 *      Josh Lanesmith 2023-11-27: Created
 */

using System;
using System.Windows.Forms;
using static JLanesmithQGame.GameBoardPlayer;

namespace JLanesmithQGame
{
	/// <summary>
	/// Play form to allow the user to load and play a game
	/// </summary>
	public partial class PlayForm : Form
    {
        private GameBoardPlayer gameBoardPlayer;

        public ImageList ImageListTools { get => imageListTools; }
        public Panel PnlBoardArea { get => pnlBoardArea; }

        public PlayForm()
        {
            InitializeComponent();

            gameBoardPlayer = new GameBoardPlayer(this);
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmiLoadGame_Click(object sender, EventArgs e)
        {
            // Check if the board is currently displaying something and warn the user before overwriting it with the new board
            if (gameBoardPlayer.Board != null)
            {
                DialogResult result = MessageBox.Show("Do you want to load a new game?\n" +
                    "Your current game will be lost.", "QGame", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
                gameBoardPlayer.ResetBoardArea();
            }

            dlgLoadGame.Filter = "QGame files|*.qgame";
            
            DialogResult r = dlgLoadGame.ShowDialog();
            switch (r)
            {
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    try
                    {
                        gameBoardPlayer.LoadGame(dlgLoadGame.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in file load: " + ex.Message);
                    }
                    break;
                case DialogResult.Cancel:
                    break;
                case DialogResult.Abort:
                    break;
                case DialogResult.Retry:
                    break;
                case DialogResult.Ignore:
                    break;
                case DialogResult.Yes:
                    break;
                case DialogResult.No:
                    break;
                default:
                    break;
            }

            txtNumberOfMoves.Text = "0";
            txtRemainingBoxes.Text = gameBoardPlayer.NumberOfBoxes.ToString();
        }

        private void ControlPad_Click(object sender, EventArgs e)
        {
            // Prompt the user to load a game before selecting the movement buttons
            if (gameBoardPlayer.SelectedBox.tile == null)
            {
                MessageBox.Show("Select a box to move", "QGame", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Button controlPadBtn = (Button)sender;

            // Isolate control name without 'btn' prefix
            string directionString = controlPadBtn.Name.Substring(3);

            Direction dir = (Direction)Enum.Parse(typeof(Direction), directionString);

            gameBoardPlayer.MoveTile(dir);

            txtNumberOfMoves.Text = gameBoardPlayer.NumberOfMoves.ToString();
            txtRemainingBoxes.Text = gameBoardPlayer.NumberOfBoxes.ToString();

            // Check if the player won the game after each movement
            if (gameBoardPlayer.CheckForWin())
            {
                MessageBox.Show($"You have completed the level in {gameBoardPlayer.NumberOfMoves} moves", "QGame",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                gameBoardPlayer.ResetBoardArea();
                txtNumberOfMoves.Text = "";
                txtRemainingBoxes.Text = "";

                gameBoardPlayer.Board = null;
            }
        }
    }
}
