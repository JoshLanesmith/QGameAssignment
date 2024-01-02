/* 
 * GameBoard.cs
 * Assignment 3
 * Revision History
 *      Josh Lanesmith 2023-11-27: Created
 */

using System;
using System.Windows.Forms;

namespace JLanesmithQGame
{
    /// <summary>
    /// GameBoard class managing details common to the designer and the player
    /// </summary>
    public class GameBoard
    {
        // Declare constants that set the parameters for the size of the board
        protected const int MAX_BOARD_WIDTH = 890;
        protected const int MAX_BOARD_HEIGHT = 660;
        protected const int MIN_TILE_SIZE = 33;
        protected const int MAX_ROWS = MAX_BOARD_HEIGHT / MIN_TILE_SIZE;
        protected const int MAX_COLUMNS = MAX_BOARD_WIDTH / MIN_TILE_SIZE;
        protected const int DEFAULT_TILE_SIZE = 65;

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
            public int row;
            public int column;
        }


        protected int rows;
        protected int columns;
        protected int tileSize;
        private BoardTile[,] board;

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
                if (Columns * value > MAX_BOARD_WIDTH)
                {
                    value = MAX_BOARD_WIDTH / Columns;
                }

                if (Rows * value > MAX_BOARD_HEIGHT)
                {
                    value = MAX_BOARD_HEIGHT / Rows;
                }
                tileSize = value;
            }
        }

        public BoardTile[,] Board { get { return board; } set { board = value; } }

    }
}
