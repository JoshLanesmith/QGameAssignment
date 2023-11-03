/* 
 * GameBoard.cs
 * Assignment 2
 * Revision History
 *      Josh Lanesmith 2023-10-30: Created
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JLanesmithQGame
{
    /// <summary>
    /// GameBoard class for creating a board for QGame
    /// </summary>
    public class GameBoard
    {
        // Declare constants that set the parameters for the size of the board
        private const int BOARD_TOP_LEFT_CORNER_X = 150;
        private const int BOARD_TOP_LEFT_CORNER_Y = 100;
        private const int MAX_BOARD_WIDTH = 890;
        private const int MAX_BOARD_HEIGHT = 660;
        private const int TILE_GAP = 3;
        private const int MIN_TILE_SIZE = 30;
        private const int MAX_ROWS = MAX_BOARD_HEIGHT / (MIN_TILE_SIZE + TILE_GAP);
        private const int MAX_COLUMNS = MAX_BOARD_WIDTH / (MIN_TILE_SIZE + TILE_GAP);
        private const int DEFAULT_TILE_SIZE = 65;

        // Declare enum to track the TileTypes
        public enum TileType
        {
            Empty,
            Wall,
            RedDoor,
            GreenDoor,
            RedBox,
            GreenBox
        }

        // Delcare struct used for the individual board tiles
        public struct BoardTile
        {
            public PictureBox tile;
            public TileType tileType;
        }

        // Declare class fields
        private DesignForm formEnvironment;
        private int rows;
        private int columns;
        private int tileSize;
        private BoardTile[,] board;

        //Declare class properties
        #region Properties

        public DesignForm FormEnvironment { get { return formEnvironment; } set { formEnvironment = value; } }
        public int Rows
        {
            get { return rows; }
            set
            {
                if (value <= MAX_ROWS)
                {
                    rows = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Rows", "Board is too big\n" +
                    $"Max Rows: {MAX_ROWS}\n" +
                    $"Max Columns: {MAX_COLUMNS}");
                }
            }
        }
        public int Columns
        {
            get { return columns; }
            set
            {
                if (value <= MAX_COLUMNS)
                {
                    columns = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Columns", "Board is too big\n" +
                    $"Max Rows: {MAX_ROWS}\n" +
                    $"Max Columns: {MAX_COLUMNS}");
                }
            }
        }
        public int TileSize
        {
            get { return tileSize; }
            set
            {
                if (Columns * (value + TILE_GAP) > MAX_BOARD_WIDTH)
                {
                    value = MAX_BOARD_WIDTH / Columns - TILE_GAP;
                }

                if (Rows * (value + TILE_GAP) > MAX_BOARD_HEIGHT)
                {
                    value = MAX_BOARD_HEIGHT / Rows - TILE_GAP;
                }
                tileSize = value;
            }
        }
        public BoardTile[,] Board { get { return board; } set { board = value; } }

        #endregion

        /// <summary>
        /// GameBoard constructor defining the interaction with the game board
        /// </summary>
        /// <param name="formEnvironment">Form environment that the GameBoard is being displayed in</param>
        /// <param name="rows">Number of rows for the game board</param>
        /// <param name="columns">Number of columns for the game board</param>
        public GameBoard(DesignForm formEnvironment, int rows, int columns)
        {
            FormEnvironment = formEnvironment;
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
                    newBoard[i, j].tile.Image = FormEnvironment.ImageListTools.Images[TileType.Empty.GetHashCode()];
                    newBoard[i, j].tile.Location = new Point(BOARD_TOP_LEFT_CORNER_X + (TileSize + TILE_GAP) * j,
                        BOARD_TOP_LEFT_CORNER_Y + (TileSize + TILE_GAP) * i);
                    newBoard[i, j].tile.Width = TileSize;
                    newBoard[i, j].tile.Height = TileSize;
                    newBoard[i, j].tile.Name = $"pbBoardTile{i}|{j}";
                    newBoard[i, j].tile.BorderStyle = BorderStyle.FixedSingle;
                    newBoard[i, j].tile.SizeMode = PictureBoxSizeMode.StretchImage;
                    newBoard[i, j].tile.MouseClick += new MouseEventHandler(BoardTile_Click);

                    // Add the picture box to the Form Controls and make it visible
                    FormEnvironment.Controls.Add(newBoard[i, j].tile);
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
                    FormEnvironment.Controls.Remove(boardTile.tile);
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
                    if (tileTypeCode == 1)
                    {
                        wallCount++;
                    }
                    else if (tileTypeCode == 2 || tileTypeCode == 3)
                    {
                        doorCount++;
                    }
                    else if (tileTypeCode == 4 || tileTypeCode == 5)
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

            if (FormEnvironment.SelectedTileType != null)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        // Change the selected board tile type
                        if (Board[i, j].tile == selectedTile)
                        {
                            Board[i, j].tileType = FormEnvironment.SelectedTileType;
                            Board[i, j].tile.Image = FormEnvironment.SelectedTool.ImageList.Images[(int)FormEnvironment.SelectedTileType];
                            return;
                        }
                    }
                }
            }
        }
    }
}
