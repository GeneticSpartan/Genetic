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

        /// <summary>
        /// The current tile sheet texture used for automatic tilemap texturing.
        /// </summary>
        public Texture2D TileSheetTexture;

        /// <summary>
        /// The location of a premade automatic tilemap sheet.
        /// </summary>
        public const string ImageAuto = "auto_tiles";

        /// <summary>
        /// An array used to contain the positons of the left, right, top, and bottom tile positons in the Tiles array during optimized tile checking.
        /// [0] = left, [1] = right, [2] = top, and [3] = bottom.
        /// </summary>
        protected int[] _tileBounds = new int[4];

        protected int _x;
        protected int _y;

        public GenTilemap()
        {
            TileTypes = new Dictionary<string, GenTile>();
        }

        /// <summary>
        /// Calls Draw on each of the tiles in the tilemap which overlap the current camera's view area.
        /// </summary>
        public override void Draw()
        {
            _tileBounds[0] = (int)(GenG.CurrentCamera.CameraView.Left / TileWidth);
            _tileBounds[1] = (int)(GenG.CurrentCamera.CameraView.Right / TileWidth);
            _tileBounds[2] = (int)(GenG.CurrentCamera.CameraView.Top / TileHeight);
            _tileBounds[3] = (int)(GenG.CurrentCamera.CameraView.Bottom / TileHeight);

            for (_y = _tileBounds[2]; _y <= _tileBounds[3]; ++_y)
            {
                for (_x = _tileBounds[0]; _x <= _tileBounds[1]; ++_x)
                {
                    if ((_x >= 0) && (_x < Tiles.GetLength(0)) && (_y >= 0) && (_y < Tiles.GetLength(1)))
                    {
                        if ((Tiles[_x, _y] != null))
                            Tiles[_x, _y].Draw();
                    }
                }
            }
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
        /// <param name="tileSheet">The location of the tilesheet image used for automatic tilemap texturing. A value of null will not use a tilesheet image.</param>
        public void LoadMap(string mapData, int tileWidth, int tileHeight, string tileSheet = null)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            string[] rows = mapData.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] row;

            int width = rows[0].Split(new char[] { ',' }).Length;

            Tiles = new GenTile[width, rows.Length];

            for (_y = 0; _y < rows.Length; _y++)
            {
                row = rows[_y].Split(new char[] { ',' });

                if (row.Length != width)
                    throw new Exception(String.Format("The length of row {0} is different from all preceeding rows.", row.Length));

                for (_x = 0; _x < width; _x++)
                {
                    if (row[_x] != "0")
                    {
                        Tiles[_x, _y] = new GenTile() { X = _x * TileWidth, Y = _y * TileHeight, Width = TileWidth, Height = TileHeight };
                        Tiles[_x, _y].LoadTexture(TileTypes[row[_x]].Texture);

                        // Check for a tile to the left of the current tile, and flag internal edges as closed.
                        if ((_x > 0) && (Tiles[_x - 1, _y] != null))
                        {
                            Tiles[_x, _y].OpenEdges &= ~GenObject.Direction.Left;
                            Tiles[_x - 1, _y].OpenEdges &= ~GenObject.Direction.Right;
                        }

                        // Check for a tile on top of the current tile, and flag internal edges as closed.
                        if ((_y > 0) && (Tiles[_x, _y - 1] != null))
                        {
                            Tiles[_x, _y].OpenEdges &= ~GenObject.Direction.Up;
                            Tiles[_x, _y - 1].OpenEdges &= ~GenObject.Direction.Down;
                        }
                    }
                    else
                        Tiles[_x, _y] = null;
                }
            }

            // Use a tile sheet for the tile images if one is provided.
            if (tileSheet != null)
            {
                LoadTileSheet(tileSheet);
            }
        }

        /// <summary>
        /// Loads a tile sheet image, using it to automatically texture each tilemap tile based on open edges.
        /// </summary>
        /// <param name="tileSheet">The location of the tilesheet image used for automatic tilemap texturing.</param>
        public void LoadTileSheet(string tileSheet)
        {
            TileSheetTexture = GenG.Content.Load<Texture2D>(tileSheet);

            for (_y = 0; _y < Tiles.GetLength(1); _y++)
            {
                for (_x = 0; _x < Tiles.GetLength(0); _x++)
                {
                    if (Tiles[_x, _y] != null)
                    {
                        ((GenSprite)Tiles[_x, _y]).LoadTexture(TileSheetTexture);

                        // Set the source rect of each tile, setting the tile image relative to its open edges using the automatic tile sheet.
                        if (Tiles[_x, _y].OpenEdges == GenObject.Direction.Any)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(0, 0, TileWidth, TileWidth); // Open edges: Left, Right, Up, Down
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x0111)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth, 0, TileWidth, TileWidth); // Open edges: Left, Right, Up
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x1110)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 2, 0, TileWidth, TileWidth); // Open edges: Right, Up, Down
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x1011)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 3, 0, TileWidth, TileWidth); // Open edges: Left, Right, Down
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x1101)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 4, 0, TileWidth, TileWidth); // Open edges: Left, Up, Down
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x1100)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 5, 0, TileWidth, TileWidth); // Open edges: Up, Down
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x0011)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 6, 0, TileWidth, TileWidth); // Open edges: Left, Right
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x0100)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 7, 0, TileWidth, TileWidth); // Open edges: Up
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x0110)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 8, 0, TileWidth, TileWidth); // Open edges: Right, Up
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x0010)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 9, 0, TileWidth, TileWidth); // Open edges: Right
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x1010)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 10, 0, TileWidth, TileWidth); // Open edges: Right, Down
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x1000)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 11, 0, TileWidth, TileWidth); // Open edges: Down
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x1001)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 12, 0, TileWidth, TileWidth); // Open edges: Left, Down
                        else if (Tiles[_x, _y].OpenEdges == GenObject.Direction.Left)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 13, 0, TileWidth, TileWidth); // Open edges: Left
                        else if (Tiles[_x, _y].OpenEdges == (GenObject.Direction)0x0101)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 14, 0, TileWidth, TileWidth); // Open edges: Left, Up
                        else if (Tiles[_x, _y].OpenEdges == GenObject.Direction.None)
                            ((GenSprite)Tiles[_x, _y]).SetSourceRect(TileWidth * 15, 0, TileWidth, TileWidth); // Open edges: None
                    }
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

            _tileBounds[0] = (int)(moveBounds.Left / TileWidth);
            _tileBounds[1] = (int)(moveBounds.Right / TileWidth);
            _tileBounds[2] = (int)(moveBounds.Top / TileHeight);
            _tileBounds[3] = (int)(moveBounds.Bottom / TileHeight);

            bool collided = false;

            for (_y = _tileBounds[2]; _y <= _tileBounds[3]; ++_y)
            {
                for (_x = _tileBounds[0]; _x <= _tileBounds[1]; ++_x)
                {
                    if ((_x >= 0) && (_x < Tiles.GetLength(0)) && (_y >= 0) && (_y < Tiles.GetLength(1)))
                    {
                        if ((Tiles[_x, _y] != null))
                        {
                            if (CollideTile(gameObject, Tiles[_x, _y], Tiles[_x, _y].OpenEdges) && !collided)
                                collided = true;
                        }
                    }
                }
            }

            return collided;
        }

        /// <summary>
        /// Applys collision detection and response to an object that may overlap a given tile.
        /// </summary>
        /// <param name="gameObject">The object to check for a collision.</param>
        /// <param name="tile">The tile to check for a collision.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the tile are collidable.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public static bool CollideTile(GenObject gameObject, GenTile tile, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            if (!gameObject.Equals(tile))
            {
                if (gameObject.Immovable)
                    return false;

                if (gameObject.GetMoveBounds().Intersects(tile.BoundingBox))
                {
                    Vector2 distances = GenU.GetDistanceAABB(gameObject.BoundingBox, tile.BoundingBox);
                    Vector2 collisionNormal;

                    if (distances.X > distances.Y)
                        collisionNormal = (gameObject.BoundingBox.MidpointX > tile.BoundingBox.MidpointX) ? new Vector2(-1, 0) : new Vector2(1, 0);
                    else
                        collisionNormal = (gameObject.BoundingBox.MidpointY > tile.BoundingBox.MidpointY) ? new Vector2(0, -1) : new Vector2(0, 1);

                    if (((collisionNormal.X == 1) && ((collidableEdges & GenObject.Direction.Left) == GenObject.Direction.Left)) ||
                        ((collisionNormal.X == -1) && ((collidableEdges & GenObject.Direction.Right) == GenObject.Direction.Right)) ||
                        ((collisionNormal.Y == 1) && ((collidableEdges & GenObject.Direction.Up) == GenObject.Direction.Up)) ||
                        ((collisionNormal.Y == -1) && ((collidableEdges & GenObject.Direction.Down) == GenObject.Direction.Down)))
                    {
                        float distance = Math.Max(distances.X, distances.Y);
                        float remove = Vector2.Dot(tile.Velocity - gameObject.Velocity, collisionNormal) + Math.Max(distance, 0) / GenG.TimeStep;

                        if (remove < 0)
                        {
                            float impulse = remove / (gameObject.Mass + tile.Mass);

                            if (!gameObject.Immovable)
                            {
                                if (collisionNormal.X != 0)
                                    gameObject.Velocity.X = 0;
                                else
                                    gameObject.Velocity.Y = 0;

                                float penetration = Math.Min(distance, 0);

                                gameObject.X += distance * collisionNormal.X;
                                gameObject.Y += distance * collisionNormal.Y;
                            }

                            if (collisionNormal.X != 0)
                            {
                                if (collisionNormal.X == 1)
                                {
                                    gameObject.Touching |= GenObject.Direction.Right;
                                    tile.Touching |= GenObject.Direction.Left;
                                }
                                else
                                {
                                    gameObject.Touching |= GenObject.Direction.Left;
                                    tile.Touching |= GenObject.Direction.Right;
                                }
                            }
                            else
                            {
                                if (collisionNormal.Y == 1)
                                {
                                    gameObject.Touching |= GenObject.Direction.Down;
                                    tile.Touching |= GenObject.Direction.Up;
                                }
                                else
                                {
                                    gameObject.Touching |= GenObject.Direction.Up;
                                    tile.Touching |= GenObject.Direction.Down;
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}