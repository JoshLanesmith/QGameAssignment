/* 
 * DesignForm.cs
 * Assignment 3
 * Revision History
 *      Josh Lanesmith 2023-10-30: Created
 *      Josh Lanesmith 2023-11-27: Updated for assignment 3
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using static JLanesmithQGame.GameBoard;

namespace JLanesmithQGame
{
	/// <summary>
	/// Design form to allow user to design a new level for QGame
	/// </summary>
	public partial class DesignForm : Form
    {
        // Declare global variable
        private GameBoardDesigner gameBoardDesigner;
        private Button selectedTool;
        private TileType selectedTileType;

        public ImageList ImageListTools { get => imageListTools; }
        public Button SelectedTool { get => selectedTool; set => selectedTool = value; }
        public TileType SelectedTileType { get => selectedTileType; set => selectedTileType = value; }

        public DesignForm()
        {
            InitializeComponent();
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void toolButton_Click(object sender, EventArgs e)
        {
            // If there is a currently selected panel change its colour back to default
            if (SelectedTool != null)
            {
                SelectedTool.BackColor = SystemColors.ControlLightLight;
            }

            // Updated the selected panel and selected TileType
            SelectedTool = sender as Button;
            SelectedTileType = (TileType)SelectedTool.ImageIndex;

            // Change background colour of newly selected panel
            SelectedTool.BackColor = Color.DeepSkyBlue;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            int rows;
            int columns;

            // Check if a GameBoard already exists and display warning message to confirm if user wants to overwrite current GameBoard
            if (gameBoardDesigner != null)
            {
                DialogResult overrideBoard = MessageBox.Show("Do you want to create a new level?\n If you do, the current level will be lost",
                    "QGame", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (overrideBoard.ToString() == "No")
                {
                    return;
                }
            }

            // Isolate user inputs for Rows and Columns and display error message if incorrect values are entered
            try
            {
                rows = int.Parse(txtRows.Text);
                columns = int.Parse(txtColumns.Text);

                if (rows < 1 || columns < 1)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please provide valid data for rows and columns (both must be integers) Number of rows and columns must be positve",
                    "QGame", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Gernerate new game board or overwrite current game board
            try
            {
                if (gameBoardDesigner == null)
                {
                    gameBoardDesigner = new GameBoardDesigner(this, rows, columns);
                }
                else
                {
                    gameBoardDesigner.ResetBoard(rows, columns);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "QGame", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            // Display error message if a user attempts to save before generating a game board
            if (gameBoardDesigner == null)
            {
                MessageBox.Show("Error: need to create a board before saving", "QGame", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set save dialog defaults
            dlgSave.Filter = "QGame extension (*.qgame)|*.qgame";
            dlgSave.DefaultExt = "qgame";
            dlgSave.AddExtension = true;

            DialogResult r = dlgSave.ShowDialog();
            switch (r)
            {
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    //Save file as per user's inputs in the save dialog
                    try
                    {
                        string fName = dlgSave.FileName;
                        string result = gameBoardDesigner.SaveBoard(fName);
                        MessageBox.Show(result, "QGame", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in file save: {ex.Message}", "QGame", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }
    }
}
