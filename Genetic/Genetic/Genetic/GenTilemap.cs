using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic
{
    public class GenTilemap : GenBasic
    {
        /// <summary>
        /// A list of the various tile types in this tilemap, each with a unique string identifier.
        /// </summary>
        public Dictionary<string, GenTile> TileTypes;

        /// <summary>
        /// The width of each tile in the tilemap.
        /// </summary>
        public int TileWidth;

        /// <summary>
        /// The height of each tile in the tilemap.
        /// </summary>
        public int TileHeight;

        /// <summary>
        /// A 2-dimensional array of tiles that construct the tilemap.
        /// </summary>
        public GenTile[,] Tiles;

        public const string ImageAuto = "Content/auto_tiles";

        public Texture2D tilesheet = null;

        public GenTilemap()
        {
            TileTypes = new Dictionary<string, GenTile>();
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
            return TileTypes[id] = tile;
        }

        /// <summary>
        /// Loads a CSV tilemap containing comma separated string values, each associated with a predefined tile used by the LoadTile method.
        /// </summary>
        /// <param name="mapData">The CSV (Comma Separated Values) tilmap to load.</param>
        /// <param name="tileWidth">The width of each tile.</param>
        /// <param name="tileHeight">The height of each tile.</param>
        /// <param name="tilesheet">The location of the tilesheet image. A value of null will not use a tilesheet image.</param>
        public void LoadMap(string mapData, int tileWidth, int tileHeight, string tilesheet = null)
        {
            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;

            string[] rows = mapData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] row;

            int width = rows[0].Split(new char[] { ',' }).Length;

            Tiles = new GenTile[width, rows.Length];

            for (int y = 0; y < rows.Length; y++)
            {
                row = rows[y].Split(new char[] { ',' });

                if (row.Length != width)
                    throw new Exception(String.Format("The length of row {0} is different from all preceeding rows.", row.Length));

                for (int x = 0; x < width; x++)
                {
                    if (row[x] != "0")
                    {
                        Tiles[x, y] = new GenTile() { X = x * tileWidth, Y = y * tileHeight, Width = tileWidth, Height = tileHeight };
                        Tiles[x, y].LoadTexture(TileTypes[row[x]].Texture);

                        // Check for a tile to the left of the current tile, and flag internal edges as closed.
                        if ((x > 0) && (Tiles[x - 1, y] != null))
                        {
                            Tiles[x, y].openEdges[0] = false;
                            Tiles[x - 1, y].openEdges[1] = false;
                        }

                        // Check for a tile on top of the current tile, and flag internal edges as closed.
                        if ((y > 0) && (Tiles[x, y - 1] != null))
                        {
                            Tiles[x, y].openEdges[2] = false;
                            Tiles[x, y - 1].openEdges[3] = false;
                        }
                    }
                    else
                        Tiles[x, y] = null;
                }
            }
        }

        public override void Update()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if (Tiles[x, y] != null)
                    {
                        Tiles[x, y].Update();
                    }
                }
            }
        }

        public override void Draw()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if (Tiles[x, y] != null)
                        Tiles[x, y].Draw();
                }
            }
        }

        /// <summary>
        /// Applys collision detection and response between an objects or group of objects and the tiles of the tilemap.
        /// Uses an object's position to efficiently find neighboring tiles to check for collision in the tiles two-dimensional array.
        /// </summary>
        /// <param name="objectOrGroup1">The object or group to check for collisions.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public bool Collide(GenBasic objectOrGroup)
        {
            if (objectOrGroup is GenObject)
            {
                return CollideObject((GenObject)objectOrGroup);
            }
            else if (objectOrGroup is GenGroup)
            {
                bool collided = false;

                for (int i = 0; i < ((GenGroup)objectOrGroup).Members.Count; i++)
                {
                    if (CollideObject((GenObject)((GenGroup)objectOrGroup).Members[i]) && !collided)
                        collided = true;
                }

                return collided;
            }

            return false;
        }

        /// <summary>
        /// Applys collision detection and response between an object and the tiles of the tilemap.
        /// Uses an object's position to efficiently find neighboring tiles to check for collision in the tiles two-dimensional array.
        /// </summary>
        /// <param name="objectOrGroup1">The object to check for collisions.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public bool CollideObject(GenObject gameObject)
        {
            GenAABB moveBounds = GenU.GetMoveBounds(gameObject);

            int leftTile = (int)Math.Floor(moveBounds.X / TileWidth);
            int rightTile = (int)Math.Ceiling(((moveBounds.X + moveBounds.Width) / TileWidth) - 1);
            int topTile = (int)Math.Floor(moveBounds.Y / TileHeight);
            int bottomTile = (int)Math.Ceiling(((moveBounds.Y + moveBounds.Height) / TileHeight) - 1);

            bool collided = false;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    if ((x >= 0) && (x < Tiles.GetLength(0)) && (y >= 0) && (y < Tiles.GetLength(1)))
                    {
                        if ((Tiles[x, y] != null))
                        {
                            if (GenG.CollideObjects(gameObject, Tiles[x, y], false, Tiles[x, y].openEdges) && !collided)
                                collided = true;
                        }
                    }
                }
            }

            return collided;
        }
    }
}