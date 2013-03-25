namespace Genetic
{
    public class GenTile : GenSprite
    {
        /// <summary>
        /// A bit field of flags determining which edges of the tile have no neighboring tiles.
        /// Useful for avoiding collisions with internal edges.
        /// </summary>
        public Direction OpenEdges;

        public GenTile()
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
            GenG.SpriteBatch.Draw(_texture, _position, _sourceRect, _color);
        }
    }
}