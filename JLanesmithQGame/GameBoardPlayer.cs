/* 
 * GameBoardPlayer.cs
 * Assignment 3
 * Revision History
 *      Josh Lanesmith 2023-11-27: Created
 */

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JLanesmithQGame
{
	/// <summary>
	/// GameBoardPlayer class for loading and playing QGame
	/// </summary>
	internal class GameBoardPlayer : GameBoard
    {
        // Declare constants that set the parameters for the size of the board
        private const int BOARD_TOP_LEFT_CORNER_X = 25;
        private const int BOARD_TOP_LEFT_CORNER_Y = 25;
        private Color defaultBorderColor = Color.Gray;
        private Color selectedBorderColor = Color.Blue;

        // Declare enum to track the direction of movement
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        // Declare class fields
        private PlayForm playForm;
        private int numberOfBoxes;
        private int numberOfMoves;
        private BoardTile selectedBox;

        /// <summary>
        /// Create a game board player that will load a board and manage the player controls
        /// </summary>
        /// <param name="playForm">The Form that the Player will operate within</param>
        public GameBoardPlayer(PlayForm playForm)
        {
            PlayForm = playForm;
        }

        //Declare class properties
        public PlayForm PlayForm { get { return playForm; } set { playForm = value; } }

        public int NumberOfBoxes { get => numberOfBoxes; set => numberOfBoxes = value; }
        public int NumberOfMoves { get => numberOfMoves; set => numberOfMoves = value; }
        public BoardTile SelectedBox { get => selectedBox; set => selectedBox = value; }

        /// <summary>
        /// Load a game from a .qgame file
        /// </summary>
        /// <param name="fileName">Name of the .qgame file</param>
        public void LoadGame(string fileName)
        {

            int boxCount = 0;

            // Access the file through the stream reader
            StreamReader reader = new StreamReader(fileName);

            // Read the first two lines as the rows and columns of the board
            Rows = int.Parse(reader.ReadLine());
            Columns = int.Parse(reader.ReadLine());

            // Set the tile size of the board
            TileSize = DEFAULT_TILE_SIZE;

            Board = new BoardTile[Rows, Columns];

            // Read through the rest of the file and set the properties for each square of the board
            while (reader.Peek() != -1)
            {
                // Identify the location and type for each tile
                int row = int.Parse(reader.ReadLine());
                int column = int.Parse(reader.ReadLine());
                int tileTypeIndex = int.Parse(reader.ReadLine());

                Board[row, column].row = row;
                Board[row, column].column = column;
                Board[row, column].tileType = (TileType)tileTypeIndex;
                Board[row, column].tile = new PictureBox();

                Board[row, column].tile.Image = playForm.ImageListTools.Images[tileTypeIndex];
                Board[row, column].tile.Location = new Point(BOARD_TOP_LEFT_CORNER_X + TileSize * column,
                    BOARD_TOP_LEFT_CORNER_Y + TileSize * row);
                Board[row, column].tile.Width = TileSize;
                Board[row, column].tile.Height = TileSize;
                Board[row, column].tile.Name = $"pbBoardTile{row}|{column}";
                Board[row, column].tile.BorderStyle = BorderStyle.FixedSingle;
                Board[row, column].tile.SizeMode = PictureBoxSizeMode.StretchImage;
                Board[row, column].tile.Tag = defaultBorderColor;
                Board[row, column].tile.MouseClick += new MouseEventHandler(BoardTile_Click);
                Board[row, column].tile.Paint += new PaintEventHandler(SelectedTile_Paint);
                Board[row, column].tile.Visible = true;

                playForm.PnlBoardArea.Controls.Add(Board[row, column].tile);

                // Count the number of boxes on the board
                if (Board[row, column].tileType == TileType.RedBox || Board[row, column].tileType == TileType.GreenBox)
                {
                    boxCount++;
                }
            }
            reader.Close();

            NumberOfBoxes = boxCount;
        }

        /// <summary>
        /// Reset the board area to a blank play area
        /// </summary>
        public void ResetBoardArea()
        {
            foreach (BoardTile boardTile in Board)
            {
                playForm.PnlBoardArea.Controls.Remove(boardTile.tile);
            }

            NumberOfMoves = 0;
        }

        /// <summary>
        /// Move the selected tile in the direction specified until it colides
        /// </summary>
        /// <param name="dir">Direction of movement</param>
        public void MoveTile(Direction dir)
        {
            // Declare booleans to track if the selected box has moved or been removed from the board
            bool moved = false;
            bool boxRemoved = false;

            int verticalMovement = 0;
            int horizontalMovement = 0;

            // Determine the numeric value for the movement direction
            if (dir == Direction.Up)
            {
                verticalMovement = -1;
            }
            else if (dir == Direction.Down)
            {
                verticalMovement = 1;
            }
            else if (dir == Direction.Left)
            {
                horizontalMovement = -1;
            }
            else if (dir == Direction.Right)
            {
                horizontalMovement = 1;
            }

            // Reset the border of the moving tile to the default border colour
            SelectedBox.tile.Tag = defaultBorderColor;
            SelectedBox.tile.Refresh();

            // Continue moving the box one tile at a time until it collides with a wall, box, or door
            while (Board[SelectedBox.row + verticalMovement, SelectedBox.column + horizontalMovement].tileType == TileType.Empty)
            {
                Board[SelectedBox.row + verticalMovement, SelectedBox.column + horizontalMovement].tileType = SelectedBox.tileType;
                Board[SelectedBox.row + verticalMovement, SelectedBox.column + horizontalMovement].tile.Image = SelectedBox.tile.Image;


                Board[SelectedBox.row, SelectedBox.column].tile.Image = playForm.ImageListTools.Images[TileType.Empty.GetHashCode()];
                Board[SelectedBox.row, SelectedBox.column].tileType = TileType.Empty;

                SelectedBox = Board[SelectedBox.row + verticalMovement, SelectedBox.column + horizontalMovement];
                moved = true;
            }

            // Check if the box collided with the door of the same colour and remove it from the board
            if ((SelectedBox.tileType == TileType.RedBox && 
                Board[SelectedBox.row + verticalMovement, SelectedBox.column + horizontalMovement].tileType == TileType.RedDoor) ||
                (SelectedBox.tileType == TileType.GreenBox &&
                Board[SelectedBox.row + verticalMovement, SelectedBox.column + horizontalMovement].tileType == TileType.GreenDoor))
            {
                NumberOfBoxes--;
                Board[SelectedBox.row, SelectedBox.column].tile.Image = playForm.ImageListTools.Images[TileType.Empty.GetHashCode()];
                Board[SelectedBox.row, SelectedBox.column].tileType = TileType.Empty;
                Board[SelectedBox.row, SelectedBox.column].tile.Refresh();

                selectedBox.tile = null;

                moved = true;
                boxRemoved = true;
            }

            // If the box was not removed then change the boarder colour of the new box location to indicate it is selected
            if (!boxRemoved)
            {
                SelectedBox.tile.Tag = selectedBorderColor;
                SelectedBox.tile.Refresh();
            }

            // Increment the movement counter if the box moved
            if (moved)
            {
                NumberOfMoves++;
            }
        }

        /// <summary>
        /// Check if the player won the current board
        /// </summary>
        /// <returns>Return true if the number of boxes is 0, or false if there are 1 or more boxes on the board</returns>
        public bool CheckForWin()
        {
            return NumberOfBoxes == 0;
        }

        private void BoardTile_Click(object sender, EventArgs e)
        {
            PictureBox pbSender = sender as PictureBox;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (pbSender == Board[i, j].tile)
                    {
                        if (Board[i, j].tileType == TileType.RedBox || Board[i, j].tileType == TileType.GreenBox)
                        {
                            if (SelectedBox.tile != null)
                            {
                                // Visualy deselect previously selected box by changing border 
                                SelectedBox.tile.Tag = defaultBorderColor;
                                SelectedBox.tile.Refresh();

                            }

                            // Update selected box and change border color
                            SelectedBox = Board[i, j];
                            SelectedBox.tile.Tag = selectedBorderColor;
                            SelectedBox.tile.Refresh();
                        }
                        return;
                    }
                }
            }
        }

        private void SelectedTile_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pbSender = sender as PictureBox;

            // Draw the boarder based on the colour identified in the tile's tag property
            ControlPaint.DrawBorder(e.Graphics, pbSender.ClientRectangle, (Color)pbSender.Tag, ButtonBorderStyle.Solid);
        }
    }
}
