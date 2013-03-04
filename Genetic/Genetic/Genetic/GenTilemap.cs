using System;
using System.Collections.Generic;

namespace Genetic
{
    public class GenTilemap : GenBasic
    {
        /// <summary>
        /// A list of the various tile types in this tilemap, each with a unique string identifier.
        /// </summary>
        public Dictionary<string, GenTile> tileTypes;

        /// <summary>
        /// The width of each tile in the tilemap.
        /// </summary>
        public int tileWidth;

        /// <summary>
        /// The height of each tile in the tilemap.
        /// </summary>
        public int tileHeight;

        /// <summary>
        /// A 2-dimensional array of tiles that construct the tilemap.
        /// </summary>
        public GenTile[,] tiles;

        public GenTilemap()
        {
            tileTypes = new Dictionary<string, GenTile>();
        }

        public GenTile LoadTile(string id, GenTile tile)
        {
            return tileTypes[id] = tile;
        }

        public void LoadMap(string mapData, int tileWidth, int tileHeight)
        {
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            string[] rows = mapData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] row;

            int width = rows[0].Split(new char[] { ',' }).Length;

            tiles = new GenTile[width, rows.Length];

            for (int y = 0; y < rows.Length; y++)
            {
                row = rows[y].Split(new char[] { ',' });

                if (row.Length != width)
                    throw new Exception(String.Format("The length of row {0} is different from all preceeding rows.", row.Length));

                for (int x = 0; x < width; x++)
                {
                    if (row[x] != "0")
                    {
                        tiles[x, y] = new GenTile() { X = x * tileWidth, Y = y * tileHeight, Width = tileWidth, Height = tileHeight };
                        tiles[x, y].LoadTexture(tileTypes[row[x]].Texture);
                    }
                    else
                        tiles[x, y] = null;
                }
            }
        }

        public override void Update()
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    if (tiles[x, y] != null)
                    {
                        tiles[x, y].Update();
                    }
                }
            }
        }

        public override void Draw()
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    if (tiles[x, y] != null)
                    {
                        tiles[x, y].Draw();
                    }
                }
            }
        }
    }
}