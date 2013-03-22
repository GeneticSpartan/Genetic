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

        public const string ImageAuto = "auto_tiles";

        public Texture2D tileSheet = null;

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
        /// <param name="tileSheet">The location of the tilesheet image used for automatic tile texturing. A value of null will not use a tilesheet image.</param>
        public void LoadMap(string mapData, int tileWidth, int tileHeight, string tileSheet = null)
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
                            Tiles[x, y].OpenEdges[0] = false;
                            Tiles[x - 1, y].OpenEdges[1] = false;
                        }

                        // Check for a tile on top of the current tile, and flag internal edges as closed.
                        if ((y > 0) && (Tiles[x, y - 1] != null))
                        {
                            Tiles[x, y].OpenEdges[2] = false;
                            Tiles[x, y - 1].OpenEdges[3] = false;
                        }
                    }
                    else
                        Tiles[x, y] = null;
                }
            }

            // Use a tile sheet for the tile images if one is provided.
            if (tileSheet != null)
            {
                this.tileSheet = GenG.Content.Load<Texture2D>(tileSheet);

                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    for (int x = 0; x < Tiles.GetLength(0); x++)
                    {
                        if (Tiles[x, y] != null)
                        {
                            ((GenSprite)Tiles[x, y]).LoadTexture(this.tileSheet);

                            // Set the source rect of each tile to set the tile image relative to its open edges.
                            if (Tiles[x, y].OpenEdges[0])
                            {
                                if (Tiles[x, y].OpenEdges[1])
                                {
                                    if (Tiles[x, y].OpenEdges[2])
                                    {
                                        if (Tiles[x, y].OpenEdges[3])
                                            ((GenSprite)Tiles[x, y]).SetSourceRect(0, 0, tileWidth, tileHeight); // Open edges: Left, Right, Top, Bottom
                                        else
                                            ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth, 0, tileWidth, tileHeight); // Open edges: Left, Right, Top
                                    }
                                    else if (Tiles[x, y].OpenEdges[3])
                                        ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 3, 0, tileWidth, tileHeight); // Open edges: Left, Right, Bottom
                                    else
                                        ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 6, 0, tileWidth, tileHeight); // Open edges: Left, Right
                                }
                                else if (Tiles[x, y].OpenEdges[2])
                                {
                                    if (Tiles[x, y].OpenEdges[3])
                                        ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 4, 0, tileWidth, tileHeight); // Open edges: Left, Top, Bottom
                                    else
                                        ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 14, 0, tileWidth, tileHeight); // Open edges: Left, Top
                                }
                                else if (Tiles[x, y].OpenEdges[3])
                                    ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 12, 0, tileWidth, tileHeight); // Open edges: Left, Bottom
                                else
                                    ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 13, 0, tileWidth, tileHeight); // Open edges: Left
                            }
                            else if (Tiles[x, y].OpenEdges[1])
                            {
                                if (Tiles[x, y].OpenEdges[2])
                                {
                                    if (Tiles[x, y].OpenEdges[3])
                                        ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 2, 0, tileWidth, tileHeight); // Open edges: Right, Top, Bottom
                                    else
                                        ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 8, 0, tileWidth, tileHeight); // Open edges: Right, Top
                                }
                                else if (Tiles[x, y].OpenEdges[3])
                                    ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 10, 0, tileWidth, tileHeight); // Open edges: Right, Bottom
                                else
                                    ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 9, 0, tileWidth, tileHeight); // Open edges: Right
                            }
                            else if (Tiles[x, y].OpenEdges[2])
                            {
                                if (Tiles[x, y].OpenEdges[3])
                                    ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 5, 0, tileWidth, tileHeight); // Open edges: Top/Bottom
                                else
                                    ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 7, 0, tileWidth, tileHeight); // Open edges: Top
                            }
                            else if (Tiles[x, y].OpenEdges[3])
                                ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 11, 0, tileWidth, tileHeight); // Open edges: Bottom
                            else
                                ((GenSprite)Tiles[x, y]).SetSourceRect(tileWidth * 15, 0, tileWidth, tileHeight); // Open edges: None
                        }
                    }
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
                            if (GenG.CollideObjects(gameObject, Tiles[x, y], false, Tiles[x, y].OpenEdges) && !collided)
                                collided = true;
                        }
                    }
                }
            }

            return collided;
        }
    }
}