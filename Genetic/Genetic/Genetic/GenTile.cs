namespace Genetic
{
    public class GenTile : GenSprite
    {
        /// <summary>
        /// An array of flags determining which edges of the tile have no neighboring tiles.
        /// [0] = left, [1] = right, [2] = top, and [3] = bottom.
        /// Useful for avoiding collisions with internal edges.
        /// </summary>
        public bool[] OpenEdges;

        public GenTile()
        {
            Immovable = true;

            // Set all tile edges as open by default.
            OpenEdges = new bool[] { true, true, true, true };
        }
    }
}