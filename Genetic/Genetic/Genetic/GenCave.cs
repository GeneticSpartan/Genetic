using System;

namespace Genetic
{
    /// <summary>
    /// Uses cellular automata to generate cave-like tilemaps.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenCave : GenTilemap
    {
        /// <summary>
        /// The number of cells along the x-axis.
        /// </summary>
        protected int _width;

        /// <summary>
        /// The number of cells along the y-axis.
        /// </summary>
        protected int _height;

        /// <summary>
        /// A two-dimensional array containing each individual cell.
        /// </summary>
        protected int[,] _cells;

        /// <summary>
        /// The current number of individual cells, horizontally and vertically, that make one cell.
        /// </summary>
        protected int _cellSize;

        /// <summary>
        /// Generates a cave using cellular automaton, given the specified settings.
        /// </summary>
        /// <param name="width">The number of cells along the x-axis, a value evenly divisible by the cell size.</param>
        /// <param name="height">The number of cells along the y-axis, a value evenly divisible by the cell size.</param>
        /// <param name="startPercentage">The initial percentage of alive cells.</param>
        /// <param name="tileWidth">The width of the tile in each cell.</param>
        /// <param name="tileHeight">The height of the tile in each cell.</param>
        /// <param name="cellSize">The initial number of individual cells, horizontally and vertically, that make one cell.</param>
        /// <param name="born">An array of numbers of neighboring cells needed for a cell to be born. Null will use an array of default values.</param>
        /// <param name="survive">An array of numbers of neighboring cells needed for a cell to survive. Null will use an array of default values.</param>
        public void MakeCave(int width, int height, int startPercentage = 45, int tileWidth = 8, int tileHeight = 8, int cellSize = 2, int[] born = null, int[] survive = null)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;

            if (born == null)
                born = new int[] { 6, 7, 8 };

            if (survive == null)
                survive = new int[] { 3, 4, 5, 6, 7, 8 };

            _cells = new int[width, height];

            for (int y = 0; y < height; y += cellSize)
            {
                for (int x = 0; x < width; x += cellSize)
                {
                    bool alive = false;

                    if ((x == 0) || (x == (_width - cellSize)) || (y == 0) || (y == (_height - cellSize)))
                        alive = true;
                    else
                        alive = (GenU.Random(0, 101) <= startPercentage) ? true : false;

                    for (int i = 0; i < cellSize; i++)
                    {
                        for (int j = 0; j < cellSize; j++)
                            _cells[x + j, y + i] = alive ? 1 : 0;
                    }
                }
            }

            ApplyAutomaton(2, born, survive, false);

            // Apply smoothing automatons.
            while (_cellSize >= 1)
                ApplyAutomaton(1, new int[] { 5, 6, 7, 8 }, new int[] { 5, 6, 7, 8 }, true);

            TileWidth = tileWidth;
            TileHeight = tileHeight;

            // Create the rows.
            Tiles = new GenTile[height][];

            // Create the columns.
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new GenTile[width];

            // Get the number of rows and columns of the tilemap.
            _rows = height;
            _columns = width;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (_cells[x, y] != 0)
                    {
                        Tiles[y][x] = new GenTile(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);

                        // Check for a tile to the left of the current tile, and flag internal edges as closed.
                        if ((x > 0) && (Tiles[y][x - 1] != null))
                        {
                            Tiles[y][x].OpenEdges &= ~GenObject.Direction.Left;
                            Tiles[y][x - 1].OpenEdges &= ~GenObject.Direction.Right;
                        }

                        // Check for a tile on top of the current tile, and flag internal edges as closed.
                        if ((y > 0) && (Tiles[y - 1][x] != null))
                        {
                            Tiles[y][x].OpenEdges &= ~GenObject.Direction.Up;
                            Tiles[y - 1][x].OpenEdges &= ~GenObject.Direction.Down;
                        }
                    }
                    else
                        Tiles[y][x] = null;
                }
            }

            LoadTileSheet(ImageAuto);
        }

        /// <summary>
        /// Applies a cellular automaton to the current grid of cells.
        /// </summary>
        /// <param name="iterations">The number of iterations to apply the cellular automaton.</param>
        /// <param name="born">An array of numbers of neighboring cells needed for a cell to be born.</param>
        /// <param name="survive">An array of numbers of neighboring cells needed for a cell to survive.</param>
        /// <param name="increaseResolution">A flag used to determine if the cell size should be divided in half during each iteration.</param>
        protected void ApplyAutomaton(int iterations, int[] born, int[] survive, bool increaseResolution)
        {
            int[,] newCells = new int[_width, _height];

            while (iterations-- > 0)
            {
                if (increaseResolution)
                    _cellSize /= 2;

                if (_cellSize >= 1)
                {
                    for (int y = 0; y < _height; y += _cellSize)
                    {
                        for (int x = 0; x < _width; x += _cellSize)
                        {
                            bool alive = true;

                            if ((x != 0) && (x != (_width - _cellSize)) && (y != 0) && (y != (_height - _cellSize)))
                            {
                                int neighborCount = CountNeighbors(x, y);

                                if ((_cells[x + _cellSize, y + _cellSize] == 0) && (Array.IndexOf(born, neighborCount) > -1))
                                    alive = true;
                                else if ((_cells[x + _cellSize, y + _cellSize] == 1) && (Array.IndexOf(survive, neighborCount) > -1))
                                    alive = true;
                                else
                                    alive = false;
                            }

                            for (int i = 0; i < _cellSize; i++)
                            {
                                for (int j = 0; j < _cellSize; j++)
                                    newCells[x + j, y + i] = alive ? 1 : 0;
                            }
                        }
                    }

                    _cells = newCells;
                }
            }
        }

        /// <summary>
        /// Counts the number of alive cells immediately surrounding a given cell's position.
        /// </summary>
        /// <param name="x">The x position of the cell in the two-dimensional array.</param>
        /// <param name="y">The y position of the cell in the two-dimensional array.</param>
        /// <returns>The number of alive cells immediately surrounding a given cell's position.</returns>
        protected int CountNeighbors(int x, int y)
        {
            int count = 0;

            if (_cells[x - _cellSize, y] == 1) // Left cell
                count++;

            if (_cells[x + _cellSize, y] == 1) // Right cell
                count++;

            if (_cells[x, y - _cellSize] == 1) // Upper cell
                count++;

            if (_cells[x, y + _cellSize] == 1) // Lower cell
                count++;

            if (_cells[x - 1, y - _cellSize] == 1) // Upper-left cell
                count++;

            if (_cells[x + 1, y - _cellSize] == 1) // Upper-right cell
                count++;

            if (_cells[x - 1, y + _cellSize] == 1) // Lower-left cell
                count++;

            if (_cells[x + 1, y + _cellSize] == 1) // Lower-right cell
                count++;

            return count;
        }
    }
}