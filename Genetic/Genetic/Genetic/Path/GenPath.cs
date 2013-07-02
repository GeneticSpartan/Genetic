using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Genetic.Path
{
    /// <summary>
    /// Manages a series of connected node points, creating a path for a game object to follow along.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenPath : GenBasic
    {
        /// <summary>
        /// The path type determines the order to move along the path nodes.
        /// </summary>
        public enum Type
        { 
            /// <summary>
            /// The object moves through each path node from the first node to the last node. 
            /// </summary>
            Clockwise,

            /// <summary>
            /// The object moves through each path node from the last node to the first node. 
            /// </summary>
            Counterclockwise, 
            
            /// <summary>
            /// The object moves through each path node, and reverses direction.
            /// </summary>
            Yoyo, 
            
            /// <summary>
            /// The object moves from the current path node to a randomly selected node.
            /// </summary>
            Random
        }

        /// <summary>
        /// Determines whether to set the object's velocity or acceleration to move along the path.
        /// </summary>
        public enum Movement
        {
            /// <summary>
            /// Set an object's velocity to move along the path.
            /// </summary>
            Instant,

            /// <summary>
            /// Set an object's acceleration to move along the path.
            /// </summary>
            Accelerates
        }

        /// <summary>
        /// A list of nodes that make up the path.
        /// </summary>
        public List<GenPathNode> Nodes;

        /// <summary>
        /// Initializes the path.
        /// </summary>
        public GenPath()
        {
            Nodes = new List<GenPathNode>();
        }

        /// <summary>
        /// Draws a line between each node along the path.
        /// </summary>
        /// <param name="camera">The camera used to draw.</param>
        public override void DrawDebug(GenCamera camera)
        {
            if ((camera != null) && !CanDraw(camera))
                return;

            for (int i = 0; i < Nodes.Count - 1; i++)
                GenG.DrawLine(Nodes[i].Position, Nodes[i + 1].Position, Color.Lime * 0.5f, 1);
        }

        /// <summary>
        /// Adds a movement node to the nodes list for use in the path.
        /// </summary>
        /// <param name="node">The movement node to add.</param>
        /// <returns>The node that was added.</returns>
        public GenPathNode AddNode(GenPathNode node)
        {
            Nodes.Add(node);

            return node;
        }

        /// <summary>
        /// Adds a movement node at the specified index in the nodes list for use in the path.
        /// </summary>
        /// <param name="node">The movement node to add.</param>
        /// <param name="index">The index number in the nodes list.</param>
        /// <returns>The node that was added. Null if the index was outside of the bounds of the nodes list.</returns>
        public GenPathNode AddNodeAt(GenPathNode node, int index)
        {
            if ((index < 0) || (index > Nodes.Count))
                return null;

            Nodes.Insert(index, node);

            return node;
        }

        /// <summary>
        /// Removes a movement node from the nodes list.
        /// </summary>
        /// <param name="node">The movement node to remove.</param>
        /// <returns>The node that was removed. Null if no node was removed.</returns>
        public GenPathNode RemoveNode(GenPathNode node)
        {
            if (Nodes.Remove(node))
                return node;

            return null;
        }

        /// <summary>
        /// Removes a movement node from the nodes list at the specified index.
        /// </summary>
        /// <param name="index">The index number of the movement node to remove in the nodes list.</param>
        /// <returns>The node that was removed. Null if no node was removed.</returns>
        public GenPathNode RemoveNodeAt(int index)
        {
            if ((index < 0) || (index >= Nodes.Count))
                return null;

            GenPathNode node = Nodes[index];
            Nodes.RemoveAt(index);

            return node;
        }

        /// <summary>
        /// Gets a node at the specified index location in the nodes list.
        /// </summary>
        /// <param name="index">The index number of the movement node to get in the nodes list.</param>
        /// <returns>The movement node at the specified index location in the nodes list. Null if the index was outside of the bounds of the nodes list.</returns>
        public GenPathNode GetAt(int index)
        {
            if ((index < 0) || (index >= Nodes.Count))
                return null;
            
            return Nodes[index];
        }

        /// <summary>
        /// Gets the first movement node in the nodes list.
        /// </summary>
        /// <returns>The first movement node in the nodes list. Null if no nodes exist.</returns>
        public GenPathNode GetHead()
        {
            if (Nodes.Count > 0)
                return Nodes[0];

            return null;
        }

        /// <summary>
        /// Gets the last movement node in the nodes list.
        /// </summary>
        /// <returns>The last movement node in the nodes list. Null if no nodes exist.</returns>
        public GenPathNode GetTail()
        {
            if (Nodes.Count > 0)
                return Nodes[Nodes.Count - 1];

            return null;
        }

        /// <summary>
        /// Gets a randomly selected node in the nodes list.
        /// </summary>
        /// <returns>The randomly selected node from the nodes list. Null if no nodes exist.</returns>
        public GenPathNode GetRandom()
        {
            if (Nodes.Count > 0)
                return Nodes[GenU.Random(0, Nodes.Count)];

            return null;
        }
    }
}
