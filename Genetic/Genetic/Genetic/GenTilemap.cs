using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;
using Genetic.Physics;

namespace Genetic
{
    /// <summary>
    /// A manager for a grid of tilemap tiles.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenTilemap : GenBasic
    {
        /// <summary>
        /// A dictionary of the various tile types in this tilemap, each with a unique string identifier.
        /// </summary>
        public Dictionary<string, GenTile> TileTypes;

        /// <summary>
        /// The width of each tile in the tilemap.
        /// </summary>
        protected int _tileWidth;

        /// <summary>
        /// The height of each tile in the tilemap.
        /// </summary>
        protected int _tileHeight;

        /// <summary>
        /// The inverse of the width of each tile in the tilemap.
        /// Useful for optimizing tile bounds calculations.
        /// </summary>
        protected float _inverseTileWidth;

        /// <summary>
        /// The inverse of the height of each tile in the tilemap.
        /// Useful for optimizing tile bounds calculations.
        /// </summary>
        protected float _inverseTileHeight;

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
        /// A premade automatic tile sheet texture.
        /// </summary>
        public static Texture2D ImageAuto;

        /// <summary>
        /// An array used to contain the positions of the left, right, top, and bottom tile positions in the Tiles array during optimized tile checking.
        /// [0] = left, [1] = right, [2] = top, and [3] = bottom.
        /// </summary>
        protected int[] _tileBounds;

        /// <summary>
        /// Gets or sets the width of each tile in the tilemap.
        /// </summary>
        public int TileWidth
        {
            get { return _tileWidth; }

            set
            {
                _tileWidth = value;
                _inverseTileWidth = 1f / _tileWidth;
            }
        }

        /// <summary>
        /// Gets or sets the height of each tile in the tilemap.
        /// </summary>
        public int TileHeight
        {
            get { return _tileHeight; }

            set
            {
                _tileHeight = value;
                _inverseTileHeight = 1f / _tileHeight;
            }
        }

        /// <summary>
        /// Initializes the manager for a grid of tilemap tiles.
        /// </summary>
        public GenTilemap()
        {
            TileTypes = new Dictionary<string, GenTile>();
            ImageAuto = GenG.LoadContent<Texture2D>("auto_tiles");
            _tileBounds = new int[4];
        }

        /// <summary>
        /// Calls Draw on each of the tiles in the tilemap which overlap the current camera's view area.
        /// </summary>
        /// <param name="camera">The camera used to draw.</param>
        public override void Draw(GenCamera camera)
        {
            if ((camera == null) || !CanDraw(camera))
                return;

            // Prevent drawing tiles outside of the camera view to increase performance.
            // If the camera is rotated, extend the drawing bounds to prevent culling tiles rotated into the camera view.
            float extendBounds = 0f;

            if (camera.Rotation != 0)
            {
                extendBounds =
                    (float)Math.Sqrt((camera.CameraView.HalfWidth * camera.CameraView.HalfWidth) + (camera.CameraView.HalfHeight * camera.CameraView.HalfHeight)) -
                    Math.Min(camera.CameraView.HalfWidth, camera.CameraView.HalfHeight);
            }

            GetTileBounds(
                camera.CameraView.Left - extendBounds,
                camera.CameraView.Right + extendBounds,
                camera.CameraView.Top - extendBounds,
                camera.CameraView.Bottom + extendBounds);

            for (int y = _tileBounds[2]; y <= _tileBounds[3]; y++)
            {
                for (int x = _tileBounds[0]; x <= _tileBounds[1]; x++)
                {
                    if ((Tiles[y][x] != null) && Tiles[y][x].Exists && Tiles[y][x].Visible)
                    {
                        Tiles[y][x].Draw();

                        if (GenG.AllowDebug && GenG.IsDebug)
                            Tiles[y][x].DrawDebug(null);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a tile associated with a specific string id used when loading a tilemap.
        /// The tile is stored in a list of tile types used to create tiles for a tilemap.
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
        /// <param name="mapData">The CSV (Comma Separated Values) tilemap to load.</param>
        /// <param name="tileWidth">The width of each tile.</param>
        /// <param name="tileHeight">The height of each tile.</param>
        /// <param name="tileSheet">The tilesheet texture used for automatic tilemap texturing. A value of null will not use a tilesheet texture.</param>
        public void LoadMap(string mapData, int tileWidth, int tileHeight, Texture2D tileSheet = null)
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
                        Tiles[y][x] = new GenTile(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);

                        if (tileSheet == null)
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
        /// Loads a tile sheet texture, using it to automatically texture each tilemap tile based on open edges.
        /// </summary>
        /// <param name="tileSheet">The tilesheet texture used for automatic tilemap texturing.</param>
        public void LoadTileSheet(Texture2D tileSheet)
        {
            TileSheetTexture = tileSheet;

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    if (Tiles[y][x] != null)
                    {
                        (Tiles[y][x]).LoadTexture(TileSheetTexture, false);
                        GetAutoTile(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Automatically sets the source rectangle of the tile at the given position in the tiles array.
        /// Sets the tile image relative to its open edges using the automatic tile sheet.
        /// </summary>
        /// <param name="x">The column position of the tile, starting from 0.</param>
        /// <param name="y">The row position of the tile, starting from 0.</param>
        public void GetAutoTile(int x, int y)
        {
            int xOffset = 0;

            switch ((int)Tiles[y][x].OpenEdges)
            {
                case 0x1111: // Open edges: Left, Right, Up, Down
                    xOffset = 0; 
                    break;

                case 0x0111: // Open edges: Left, Right, Up
                    xOffset = _tileWidth;
                    break;

                case 0x1110: // Open edges: Right, Up, Down
                    xOffset = _tileWidth * 2;
                    break;

                case 0x1011: // Open edges: Left, Right, Down
                    xOffset = _tileWidth * 3;
                    break;

                case 0x1101: // Open edges: Left, Up, Down
                    xOffset = _tileWidth * 4;
                    break;

                case 0x1100: // Open edges: Up, Down
                    xOffset = _tileWidth * 5;
                    break;

                case 0x0011: // Open edges: Left, Right
                    xOffset = _tileWidth * 6;
                    break;

                case 0x0100: // Open edges: Up
                    xOffset = _tileWidth * 7;
                    break;

                case 0x0110: // Open edges: Right, Up
                    xOffset = _tileWidth * 8;
                    break;

                case 0x0010: // Open edges: Right
                    xOffset = _tileWidth * 9;
                    break;

                case 0x1010: // Open edges: Right, Down
                    xOffset = _tileWidth * 10;
                    break;

                case 0x1000: // Open edges: Down
                    xOffset = _tileWidth * 11;
                    break;

                case 0x1001: // Open edges: Left, Down
                    xOffset = _tileWidth * 12;
                    break;

                case 0x0001: // Open edges: Left
                    xOffset = _tileWidth * 13;
                    break;

                case 0x0101: // Open edges: Left, Up
                    xOffset = _tileWidth * 14;
                    break;

                case 0x0000: // Open edges: None
                    xOffset = _tileWidth * 15;
                    break;
            }

            Tiles[y][x].SetSourceRect(xOffset, 0, _tileWidth, _tileHeight, true);
        }

        /// <summary>
        /// Flags the given edge of a specific tile as open.
        /// </summary>
        /// <param name="x">The column position of the tile, starting from 0.</param>
        /// <param name="y">The row position of the tile, starting from 0.</param>
        /// <param name="edge">The direction of the edge to flag as closed.</param>
        /// <returns>True if the tile edge was flagged as open, false if not.</returns>
        public bool OpenTileEdge(int x, int y, GenObject.Direction edge)
        {
            if (Tiles[y][x] == null)
                return false;

            Tiles[y][x].OpenEdges |= edge;

            if (TileSheetTexture != null)
                GetAutoTile(x, y);

            return true;
        }

        /// <summary>
        /// Flags the given edge of a specific tile as closed.
        /// </summary>
        /// <param name="x">The column position of the tile, starting from 0.</param>
        /// <param name="y">The row position of the tile, starting from 0.</param>
        /// <param name="edge">The direction of the edge to flag as closed.</param>
        /// <returns>True if the tile edge was flagged as closed, false if not.</returns>
        public bool CloseTileEdge(int x, int y, GenObject.Direction edge)
        {
            if (Tiles[y][x] == null)
                return false;

            Tiles[y][x].OpenEdges &= ~edge;

            if (TileSheetTexture != null)
                GetAutoTile(x, y);

            return true;
        }

        /// <summary>
        /// Gets the boundary of tiles within the given boundary positions.
        /// Translates the boundary positions to row and column positions used by the tiles array.
        /// </summary>
        /// <param name="left">The horizontal position of the left edge of the bounding box.</param>
        /// <param name="right">The horizontal position of the right edge of the bounding box.</param>
        /// <param name="top">The vertical position of the top edge of the bounding box.</param>
        /// <param name="bottom">The vertical position of the bottom edge of the bounding box.</param>
        public void GetTileBounds(float left, float right, float top, float bottom)
        {
            // Left bound.
            _tileBounds[0] = (int)Math.Max(left * _inverseTileWidth, 0);

            // Right bound.
            _tileBounds[1] = (int)Math.Min(right * _inverseTileWidth, _columns - 1);

            // Top bound.
            _tileBounds[2] = (int)Math.Max(top * _inverseTileHeight, 0);

            // Bottom bound.
            _tileBounds[3] = (int)Math.Min(bottom * _inverseTileHeight, _rows - 1);
        }

        /// <summary>
        /// Adds a tile to the tilemap at the specified position.
        /// </summary>
        /// <param name="x">The column position of the tile, starting from 0.</param>
        /// <param name="y">The row position of the tile, starting from 0.</param>
        /// <param name="replace">A flag used to determine if any existing tile will be replaced.</param>
        /// <returns>The tile that was added to the tilemap. Null if no tile was added.</returns>
        public GenTile AddTile(int x, int y, bool replace = false)
        {
            if ((x < 0) || (y < 0))
                return null;

            if ((x >= Tiles[0].Length) || (y >= Tiles.Length))
                return null;

            if ((Tiles[y][x] != null) && !replace)
                return null;

            Tiles[y][x] = new GenTile(x * _tileWidth, y * _tileHeight, _tileWidth, _tileHeight);

            if (TileSheetTexture != null)
            {
                Tiles[y][x].LoadTexture(TileSheetTexture, false);
                GetAutoTile(x, y);
            }

            // Check for a tile to the left of the current tile, and flag internal edges as closed.
            if (x > 0)
            {
                if (CloseTileEdge(x - 1, y, GenObject.Direction.Right))
                    CloseTileEdge(x, y, GenObject.Direction.Left);             
            }

            // Check for a tile to the right of the current tile, and flag internal edges as closed.
            if (x < _columns - 1)
            {
                if (CloseTileEdge(x + 1, y, GenObject.Direction.Left))
                    CloseTileEdge(x, y, GenObject.Direction.Right);
            }

            // Check for a tile above the current tile, and flag internal edges as closed.
            if (y > 0)
            {
                if (CloseTileEdge(x, y - 1, GenObject.Direction.Down))
                    CloseTileEdge(x, y, GenObject.Direction.Up);
            }

            // Check for a tile below the current tile, and flag internal edges as closed.
            if (y < _rows - 1)
            {
                if (CloseTileEdge(x, y + 1, GenObject.Direction.Up))
                    CloseTileEdge(x, y, GenObject.Direction.Down);
            }

            return Tiles[y][x];
        }

        /// <summary>
        /// Removes a tile from the tilemap at the specified position.
        /// </summary>
        /// <param name="x">The column position of the tile, starting from 0.</param>
        /// <param name="y">The row position of the tile, starting from 0.</param>
        public void RemoveTile(int x, int y)
        {
            if ((x < 0) || (y < 0))
                return;

            if ((x >= Tiles[0].Length) || (y >= Tiles.Length))
                return;

            Tiles[y][x] = null;

            // Check for a tile to the left of the current tile, and flag its right edge as open.
            if (x > 0)
                OpenTileEdge(x - 1, y, GenObject.Direction.Right);

            // Check for a tile to the right of the current tile, and flag its left edge as open.
            if (x < _columns - 1)
                OpenTileEdge(x + 1, y, GenObject.Direction.Left);

            // Check for a tile above the current tile, and flag its bottom edge as open.
            if (y > 0)
                OpenTileEdge(x, y - 1, GenObject.Direction.Down);

            // Check for a tile below the current tile, and flag its top edge as open.
            if (y < _rows - 1)
                OpenTileEdge(x, y + 1, GenObject.Direction.Up);
        }

        /// <summary>
        /// Applies collision detection and response between an object or group of objects and the tiles of the tilemap.
        /// Uses an object's position to efficiently find neighboring tiles to check for collision in the tiles two-dimensional array.
        /// </summary>
        /// <param name="objectOrGroup">The object or group to check for collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public bool Collide(GenBasic objectOrGroup, CollideEvent callback)
        {            
            if (objectOrGroup is GenObject)
                return CollideObject(objectOrGroup as GenObject, callback);
            
            if (objectOrGroup is GenGroup)
            {
                bool collided = false;

                foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
                {
                    if (CollideObject(basic as GenObject, callback))
                        collided = true;
                }

                return collided;
            }

            return false;
        }

        /// <summary>
        /// Applies collision detection and response between an object and the tiles of the tilemap.
        /// Uses an object's position to efficiently find neighboring tiles to check for collision in the tiles two-dimensional array.
        /// </summary>
        /// <param name="gameObject">The object to check for collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <returns>True is a collision occurs, false if not.</returns>
        public bool CollideObject(GenObject gameObject, CollideEvent callback)
        {
            GetTileBounds(
                gameObject.MoveBounds.Left,
                gameObject.MoveBounds.Right,
                gameObject.MoveBounds.Top,
                gameObject.MoveBounds.Bottom);

            bool collided = false;

            for (int y = _tileBounds[2]; y <= _tileBounds[3]; y++)
            {
                for (int x = _tileBounds[0]; x <= _tileBounds[1]; x++)
                {
                    // If the tile is empty, do not check for a collision.
                     if (Tiles[y][x] != null)
                    {
                        if (GenCollide.Collide(gameObject, Tiles[y][x], callback, Tiles[y][x].OpenEdges))
                            collided = true;
                    }
                }
            }

            return collided;
        }
    }
}