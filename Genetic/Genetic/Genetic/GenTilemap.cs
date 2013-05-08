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
        /// A jagged array of tiles that construct the tilemap.
        /// </summary>
        public GenTile[][] Tiles;

        /// <summary>
        /// The number of horizontal rows of tiles in the tile map.
        /// </summary>
        protected int _rows;

        /// <summary>
        /// The number of vertical columns of tiles in the tile map.
        /// </summary>
        protected int _columns;

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

            int x;
            int y;

            for (y = _tileBounds[2]; y <= _tileBounds[3]; ++y)
            {
                for (x = _tileBounds[0]; x <= _tileBounds[1]; ++x)
                {
                    if ((x >= 0) && (x < _columns) && (y >= 0) && (y < _rows))
                    {
                        if ((Tiles[y][x] != null) && Tiles[y][x].Exists && Tiles[y][x].Visible)
                        {
                            Tiles[y][x].Draw();

                            if (GenG.IsDebug)
                                Tiles[y][x].DrawDebug();
                        }
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

            // Create the rows.
            Tiles = new GenTile[rows.Length][];

            // Create the columns.
            for (int i = 0; i < Tiles.Length; i++)
                Tiles[i] = new GenTile[width];

            // Get the number of rows and columns of the tilemap.
            _rows = Tiles.Length;
            _columns = Tiles[0].Length;

            int x;
            int y;

            for (y = 0; y < rows.Length; y++)
            {
                row = rows[y].Split(new char[] { ',' });

                if (row.Length != width)
                    throw new Exception(String.Format("The length of row {0} is different from all preceeding rows.", row.Length));

                for (x = 0; x < width; x++)
                {
                    if (row[x] != "0")
                    {
                        Tiles[y][x] = new GenTile(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
                        Tiles[y][x].LoadTexture(TileTypes[row[x]].Texture);

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

            int x;
            int y;

            for (y = 0; y < _rows; y++)
            {
                for (x = 0; x < _columns; x++)
                {
                    if (Tiles[y][x] != null)
                    {
                        ((GenSprite)Tiles[y][x]).LoadTexture(TileSheetTexture);

                        // Set the source rect of each tile, setting the tile image relative to its open edges using the automatic tile sheet.
                        if (Tiles[y][x].OpenEdges == GenObject.Direction.Any)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(1, 1, TileWidth, TileWidth); // Open edges: Left, Right, Up, Down
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x0111)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth + 2, 1, TileWidth, TileWidth); // Open edges: Left, Right, Up
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x1110)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 2 + 3, 1, TileWidth, TileWidth); // Open edges: Right, Up, Down
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x1011)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 3 + 4, 1, TileWidth, TileWidth); // Open edges: Left, Right, Down
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x1101)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 4 + 5, 1, TileWidth, TileWidth); // Open edges: Left, Up, Down
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x1100)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 5 + 6, 1, TileWidth, TileWidth); // Open edges: Up, Down
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x0011)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 6 + 7, 1, TileWidth, TileWidth); // Open edges: Left, Right
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x0100)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 7 + 8, 1, TileWidth, TileWidth); // Open edges: Up
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x0110)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 8 + 9, 1, TileWidth, TileWidth); // Open edges: Right, Up
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x0010)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 9 + 10, 1, TileWidth, TileWidth); // Open edges: Right
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x1010)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 10 + 11, 1, TileWidth, TileWidth); // Open edges: Right, Down
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x1000)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 11 + 12, 1, TileWidth, TileWidth); // Open edges: Down
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x1001)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 12 + 13, 1, TileWidth, TileWidth); // Open edges: Left, Down
                        else if (Tiles[y][x].OpenEdges == GenObject.Direction.Left)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 13 + 14, 1, TileWidth, TileWidth); // Open edges: Left
                        else if (Tiles[y][x].OpenEdges == (GenObject.Direction)0x0101)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 14 + 15, 1, TileWidth, TileWidth); // Open edges: Left, Up
                        else if (Tiles[y][x].OpenEdges == GenObject.Direction.None)
                            ((GenSprite)Tiles[y][x]).SetSourceRect(TileWidth * 15 + 16, 1, TileWidth, TileWidth); // Open edges: None
                    }
                }
            }
        }

        /// <summary>
        /// Applys collision detection and response between an object or group of objects and the tiles of the tilemap.
        /// Uses an object's position to efficiently find neighboring tiles to check for collision in the tiles two-dimensional array.
        /// </summary>
        /// <param name="objectOrGroup">The object or group to check for collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public bool Collide(GenBasic objectOrGroup, CollideEvent callback = null)
        {            
            if (objectOrGroup is GenObject)
            {
                return CollideObject((GenObject)objectOrGroup, callback);
            }
            else if (objectOrGroup is GenGroup)
            {
                bool collided = false;

                foreach (GenBasic basic in ((GenGroup)objectOrGroup).Members)
                {
                    if (CollideObject((GenObject)basic, callback) && !collided)
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
        /// <param name="gameObject">The object to check for collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public bool CollideObject(GenObject gameObject, CollideEvent callback = null)
        {
            // Refresh the movement bounding box of the object.
            gameObject.GetMoveBounds();

            _tileBounds[0] = (int)(gameObject.MoveBounds.Left / TileWidth); // Left bound.
            _tileBounds[1] = (int)(gameObject.MoveBounds.Right / TileWidth); // Right bound.
            _tileBounds[2] = (int)(gameObject.MoveBounds.Top / TileHeight); // Top bound.
            _tileBounds[3] = (int)(gameObject.MoveBounds.Bottom / TileHeight); // Bottom bound.

            bool collided = false;

            int x;
            int y;

            for (y = _tileBounds[2]; y <= _tileBounds[3]; ++y)
            {
                for (x = _tileBounds[0]; x <= _tileBounds[1]; ++x)
                {
                    if ((x >= 0) && (x < _columns) && (y >= 0) && (y < _rows))
                    {
                        // Do not check for a collision if the tile is empty.
                        if ((Tiles[y][x] != null))
                        {
                            if (gameObject.Collide(Tiles[y][x], callback, false, Tiles[y][x].OpenEdges) && !collided)
                                collided = true;
                        }
                    }
                }
            }

            return collided;
        }
    }
}