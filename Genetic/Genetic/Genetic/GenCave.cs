using System;

namespace Genetic
{
    public class GenCave : GenTilemap
    {
        protected int _width;

        protected int _height;

        protected int[,] _cells;

        protected int _cellSize;

        public GenCave()
        {

        }

        public void MakeCave(int width, int height, int startPercentage = 50, int tileWidth = 8, int tileHeight = 8, int cellSize = 4, int[] born = null, int[] survive = null)
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
                        alive = (GenU.Random.Next(0, 100) <= startPercentage) ? true : false;

                    for (int i = 0; i < cellSize; i++)
                    {
                        for (int j = 0; j < cellSize; j++)
                            _cells[x + j, y + i] = alive ? 1 : 0;
                    }
                }
            }

            ApplyAutomaton(2, born, survive, false);

            ApplyAutomaton(1, new int[] { 5, 6, 7, 8 }, new int[] { 5, 6, 7, 8 }, true);
            ApplyAutomaton(1, new int[] { 5, 6, 7, 8 }, new int[] { 5, 6, 7, 8 }, true);

            TileWidth = tileWidth;
            TileHeight = tileHeight;

            Tiles = new GenTile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (_cells[x, y] != 0)
                    {
                        Tiles[x, y] = new GenTile() { X = x * TileWidth, Y = y * TileHeight, Width = TileWidth, Height = TileHeight };

                        // Check for a tile to the left of the current tile, and flag internal edges as closed.
                        if ((x > 0) && (Tiles[x - 1, y] != null))
                        {
                            Tiles[x, y].OpenEdges &= ~GenObject.Direction.Left;
                            Tiles[x - 1, y].OpenEdges &= ~GenObject.Direction.Right;
                        }

                        // Check for a tile on top of the current tile, and flag internal edges as closed.
                        if ((y > 0) && (Tiles[x, y - 1] != null))
                        {
                            Tiles[x, y].OpenEdges &= ~GenObject.Direction.Up;
                            Tiles[x, y - 1].OpenEdges &= ~GenObject.Direction.Down;
                        }
                    }
                    else
                        Tiles[x, y] = null;
                }
            }

            LoadTileSheet(ImageAuto);
        }

        protected void ApplyAutomaton(int iterations, int[] born, int[] survive, bool increaseResolution)
        {
            int[,] newCells = new int[_width, _height];

            while (iterations-- > 0)
            {
                if (increaseResolution)
                    _cellSize /= 2;

                for (int y = 0; y < _height; y += _cellSize)
                {
                    for (int x = 0; x < _width; x += _cellSize)
                    {
                        bool alive = true;

                        if ((x != 0) && (x != (_width - _cellSize)) && (y != 0) && (y != (_height - _cellSize)))
                        {
                            int neighborCount = CountNeighbors(x, y);

                            alive = (((_cells[x + _cellSize, y + _cellSize] == 0) && (Array.IndexOf(born, neighborCount) > -1)) || (_cells[x + _cellSize, y + _cellSize] == 1) && (Array.IndexOf(survive, neighborCount) > -1));
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