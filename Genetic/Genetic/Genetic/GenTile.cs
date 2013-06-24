using Microsoft.Xna.Framework;

namespace Genetic
{
    /// <summary>
    /// A game object that makes up a single tile in a tilemap.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenTile : GenSprite
    {
        /// <summary>
        /// A bit field of flags determining which edges of the tile have no neighboring tiles.
        /// Useful for avoiding collisions with internal edges.
        /// </summary>
        public Direction OpenEdges;

        /// <summary>
        /// A physical tile object used in a tilemap.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the tile.</param>
        /// <param name="y">The y position of the top-left corner of the tile.</param>
        /// <param name="width">The width of the tile.</param>
        /// <param name="height">The height of the tile.</param>
        public GenTile(float x = 0, float y = 0, int width = 1, int height = 1)
            : base(x, y, null, width, height)
        {
            Immovable = true;

            // Set all tile edges as open by default.
            OpenEdges |= Direction.Any;
        }

        /// <summary>
        /// Draws the tile using a simple draw method.
        /// </summary>
        public override void Draw()
        {
            if (_texture != null)
                GenG.SpriteBatch.Draw(_texture, _position, _sourceRect, _color);
        }
    }
}