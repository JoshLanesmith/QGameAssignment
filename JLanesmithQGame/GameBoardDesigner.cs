/* 
 * GameBoardDesigner.cs
 * Assignment 3
 * Revision History
 *      Josh Lanesmith 2023-10-30: Created
 *      Josh Lanesmith 2023-11-27: Updated for assignment 3
 */
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JLanesmithQGame
{
	/// <summary>
	/// GameBoardDesiner class for creating a board for QGame
	/// </summary>
	public class GameBoardDesigner : GameBoard
    {
        // Declare constants that set the parameters for the size of the board
        protected const int BOARD_TOP_LEFT_CORNER_X = 150;
        protected const int BOARD_TOP_LEFT_CORNER_Y = 100;


        // Declare class fields
        private DesignForm formDesigner;


        //Declare class properties
        public DesignForm FormDesigner { get { return formDesigner; } set { formDesigner = value; } }

        /// <summary>
        /// GameBoard constructor defining the interaction with the game board
        /// </summary>
        /// <param name="formDesigner">Form environment that the GameBoard is being displayed in</param>
        /// <param name="rows">Number of rows for the game board</param>
        /// <param name="columns">Number of columns for the game board</param>
        public GameBoardDesigner(DesignForm formDesigner, int rows, int columns)
        {
            FormDesigner = formDesigner;
            Rows = rows;
            Columns = columns;
            Board = SetNewBoard(rows, columns);
        }

        /// <summary>
        /// Set the grid of picture boxes for the game board
        /// </summary>
        /// <param name="rows">Number of rows for the game board grid</param>
        /// <param name="columns">Number of columns for the game board grid</param>
        /// <returns>Return 2D array of BoardTiles</returns>
        private BoardTile[,] SetNewBoard(int rows, int columns)
        {
            BoardTile[,] newBoard = new BoardTile[rows, columns];

            // Set the tile size of the board
            TileSize = DEFAULT_TILE_SIZE;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    // Instansiate each board tile setting all of its relevant properties
                    newBoard[i, j] = new BoardTile() { tile = new PictureBox(), tileType = TileType.Empty };
                    newBoard[i, j].tile.Image = FormDesigner.ImageListTools.Images[TileType.Empty.GetHashCode()];
                    newBoard[i, j].tile.Location = new Point(BOARD_TOP_LEFT_CORNER_X + (TileSize) * j,
                        BOARD_TOP_LEFT_CORNER_Y + (TileSize) * i);
                    newBoard[i, j].tile.Width = TileSize;
                    newBoard[i, j].tile.Height = TileSize;
                    newBoard[i, j].tile.Name = $"pbBoardTile{i}|{j}";
                    newBoard[i, j].tile.BorderStyle = BorderStyle.FixedSingle;
                    newBoard[i, j].tile.SizeMode = PictureBoxSizeMode.StretchImage;
                    newBoard[i, j].tile.MouseClick += new MouseEventHandler(BoardTile_Click);

                    // Add the picture box to the Form Controls and make it visible
                    FormDesigner.Controls.Add(newBoard[i, j].tile);
                    newBoard[i, j].tile.Visible = true;
                }
            }

            return newBoard;
        }

        /// <summary>
        /// Overwrite and reset the game board
        /// </summary>
        /// <param name="rows">Number of rows for the game board grid</param>
        /// <param name="columns">Number of columns for the game board grid</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ResetBoard(int rows, int columns)
        {
            // Test the code for reseting the board and throw error if it doesn't work
            try
            {
                Rows = rows;
                Columns = columns;

                // Remove all current board tile picture boxes from the Form Controls
                foreach (BoardTile boardTile in Board)
                {
                    FormDesigner.Controls.Remove(boardTile.tile);
                }

                // Instantiate new board
                Board = SetNewBoard(rows, columns);
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException("", ex.Message);
            }
        }

        /// <summary>
        /// Save the game board state to a file
        /// </summary>
        /// <param name="fileName">File name</param>
        public string SaveBoard(string fileName)
        {
            int wallCount = 0;
            int doorCount = 0;
            int boxCount = 0;

            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine($"{Rows}\n{Columns}");
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int tileTypeCode = Board[i, j].tileType.GetHashCode();
                    writer.WriteLine($"{i}\n{j}\n{tileTypeCode}");
                    if (Board[i, j].tileType == TileType.Wall)
                    {
                        wallCount++;
                    }
                    else if (Board[i, j].tileType == TileType.RedDoor || Board[i, j].tileType == TileType.GreenDoor)
                    {
                        doorCount++;
                    }
                    else if (Board[i, j].tileType == TileType.RedBox || Board[i, j].tileType == TileType.GreenBox)
                    {
                        boxCount++;
                    }
                }
            }
            writer.Close();

            return $"File saved successfully" +
                $"\nTotal number of walls: {wallCount}" +
                $"\nTotal number of doors: {doorCount}" +
                $"\nTotal number of boxes: {boxCount}";

        }

        private void BoardTile_Click(object sender, EventArgs e)
        {
            PictureBox selectedTile = sender as PictureBox;

            if (FormDesigner.SelectedTileType != null)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        // Change the selected board tile type
                        if (Board[i, j].tile == selectedTile)
                        {
                            Board[i, j].tileType = FormDesigner.SelectedTileType;
                            Board[i, j].tile.Image = FormDesigner.SelectedTool.ImageList.Images[(int)FormDesigner.SelectedTileType];
                            return;
                        }
                    }
                }
            }
        }
    }
}
