﻿using System;
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

        /// <summary>
        /// Loads a tile associated with a specific string id used when loading a tilemap.
        /// The tile is stored in a list of tyle types used to create tiles for a tilemap.
        /// </summary>
        /// <param name="id">The unique string id associated with this tile.</param>
        /// <param name="tile">The tile object to store in the list of tile types.</param>
        /// <returns>The tile object that was loaded.</returns>
        public GenTile LoadTile(string id, GenTile tile)
        {
            return tileTypes[id] = tile;
        }

        /// <summary>
        /// Loads a CSV tilemap containing comma separated string values, each associated with a predefined tile used by the LoadTile method.
        /// </summary>
        /// <param name="mapData">The CSV (Comma Separated Values) tilmap to load.</param>
        /// <param name="tileWidth">The width of each tile.</param>
        /// <param name="tileHeight">The height of each tile.</param>
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

        /// <summary>
        /// Applys collision detection and response between an objects or group of objects and the tiles of the tilemap.
        /// Uses an object's position to efficiently find neighboring tiles to check for collision in the tiles two-dimensional array.
        /// </summary>
        /// <param name="objectOrGroup1">The object or group to check for collisions.</param>
        public void Collide(GenBasic objectOrGroup)
        {
            int leftTile;
            int rightTile;
            int topTile;
            int bottomTile;

            if (objectOrGroup is GenObject)
            {
                leftTile = (int)Math.Floor((float)((GenObject)objectOrGroup).X / tileWidth);
                rightTile = (int)Math.Ceiling(((float)(((GenObject)objectOrGroup).X + ((GenObject)objectOrGroup).Width) / tileWidth)) - 1;
                topTile = (int)Math.Floor((float)((GenObject)objectOrGroup).Y / tileHeight);
                bottomTile = (int)Math.Ceiling(((float)(((GenObject)objectOrGroup).Y + ((GenObject)objectOrGroup).Height) / tileHeight)) - 1;

                for (int y = topTile; y <= bottomTile; ++y)
                {
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
                            GenG.Collide(objectOrGroup, tiles[x, y]);
                    }
                }
            }
            else if (objectOrGroup is GenGroup)
            {
                for (int i = 0; i < ((GenGroup)objectOrGroup).members.Count; i++)
                {
                    leftTile = (int)Math.Floor((float)((GenObject)((GenGroup)objectOrGroup).members[i]).X / tileWidth);
                    rightTile = (int)Math.Ceiling(((float)(((GenObject)((GenGroup)objectOrGroup).members[i]).X + ((GenObject)((GenGroup)objectOrGroup).members[i]).Width) / tileWidth)) - 1;
                    topTile = (int)Math.Floor((float)((GenObject)((GenGroup)objectOrGroup).members[i]).Y / tileHeight);
                    bottomTile = (int)Math.Ceiling(((float)(((GenObject)((GenGroup)objectOrGroup).members[i]).Y + ((GenObject)((GenGroup)objectOrGroup).members[i]).Height) / tileHeight)) - 1;

                    for (int y = topTile; y <= bottomTile; ++y)
                    {
                        for (int x = leftTile; x <= rightTile; ++x)
                        {
                            if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
                                GenG.Collide(((GenGroup)objectOrGroup).members[i], tiles[x, y]);
                        }
                    }
                }
            }
        }
    }
}