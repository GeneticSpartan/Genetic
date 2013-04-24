using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Genetic.Geometry;

namespace Genetic
{
    public class GenQuadtree
    {
        /// <summary>
        /// The maximum amount of objects allowed in a node.
        /// </summary>
        internal const int MAX_OBJECTS = 5;

        /// <summary>
        /// The maximum amount of splits that can occur through a series of nodes.
        /// </summary>
        internal const int MAX_LEVELS = 6;

        /// <summary>
        /// The highest level node of the quadtree in which all other nodes will be contained.
        /// </summary>
        protected GenQuadtreeNode _rootNode;

        /// <summary>
        /// A list of reusable quadtree nodes.
        /// Used to avoid massive garbage collection when quadtree nodes need to be created each update.
        /// </summary>
        protected List<GenQuadtreeNode> _quadtreeNodes;

        /// <summary>
        /// The current index for the quadtree nodes list.
        /// Used to determine the next available node in the list.
        /// </summary>
        internal int NodeIndex = 0;

        /// <summary>
        /// A manager for a quadtree data structure.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the root node bounding box.</param>
        /// <param name="y">The y position of the top-left corner of the root node bounding box.</param>
        /// <param name="width">The width of the root node bounding box.</param>
        /// <param name="height">The height of the root node bounding box.</param>
        public GenQuadtree(float x, float y, float width, float height)
        {
            _rootNode = new GenQuadtreeNode(x, y, width, height, this, 0);
            _quadtreeNodes = new List<GenQuadtreeNode>();
        }

        /// <summary>
        /// Gets an available node from the quadtree nodes list.
        /// If no nodes are available, a new node is created and added to the quadtree nodes list.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the node bounding box.</param>
        /// <param name="y">The y position of the top-left corner of the node bounding box.</param>
        /// <param name="width">The width of the node bounding box.</param>
        /// <param name="height">The height of the node bounding box.</param>
        /// <param name="level">The split level of the node.</param>
        /// <returns></returns>
        internal GenQuadtreeNode GetNode(float x, float y, float width, float height, int level)
        {
            // Create a new quadtree node object if the node index is out of range, and add the new node to the quadtree nodes list.
            if (NodeIndex >= _quadtreeNodes.Count)
            {
                _quadtreeNodes.Add(new GenQuadtreeNode(x, y, width, height, this, level));

                return _quadtreeNodes[NodeIndex++];
            }

            // Reuse a node in the quadtree nodes list by initializing it with new values.
            return _quadtreeNodes[NodeIndex++].Initialize(x, y, width, height, level);
        }

        /// <summary>
        /// Calls Clear on the root quadtree node, and resets the node index value to 0.
        /// </summary>
        public void Clear()
        {
            _rootNode.Clear();
            NodeIndex = 0;
        }

        /// <summary>
        /// Iterates through each node that entirely contains the given object or group of objects, and adds the object to the lowest level node possible.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to insert into the quadtree.</param>
        public void Insert(GenBasic objectOrGroup)
        {
            _rootNode.Insert(objectOrGroup);
        }

        /// <summary>
        /// Iterates through each node that entirely contains the given bounding box, and retrieves a list of objects from each node.
        /// </summary>
        /// <param name="returnObjects">A list that will be populated with accessible game objects.</param>
        /// <param name="box">The bounding box used to search for nodes.</param>
        public void Retrieve(List<GenBasic> returnObjects, GenAABB box)
        {
            _rootNode.Retrieve(returnObjects, box);
        }

        /// <summary>
        /// Draws debug lines that visually represent each node.
        /// </summary>
        public void Draw()
        {
            _rootNode.Draw();
        }
    }

    public class GenQuadtreeNode : GenAABB
    {
        /// <summary>
        /// A reference to the quadtree manager of this node.
        /// </summary>
        protected GenQuadtree _quadtree;

        /// <summary>
        /// The current split level of the node.
        /// </summary>
        protected int _level;

        /// <summary>
        /// A list of game objects associated with the node.
        /// </summary>
        protected List<GenBasic> _objects;

        /// <summary>
        /// An array of four leaf nodes that are created when a split occurs.
        /// </summary>
        protected GenQuadtreeNode[] _nodes;

        /// <summary>
        /// A single node within a quadtree data structure.
        /// Used as a container for objects or other nodes.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the node bounding box.</param>
        /// <param name="y">The y position of the top-left corner of the node bounding box.</param>
        /// <param name="width">The width of the node bounding box.</param>
        /// <param name="height">The height of the node bounding box.</param>
        /// <param name="quadtree">A reference to the quadtree manager of this node.</param>
        /// <param name="level">The split level of the node.</param>
        public GenQuadtreeNode(float x, float y, float width, float height, GenQuadtree quadtree, int level)
            : base(x, y, width, height)
        {
            _quadtree = quadtree;
            _level = level;
            _objects = new List<GenBasic>();
            _nodes = new GenQuadtreeNode[4];
        }

        /// <summary>
        /// Initializes the quadtree node.
        /// Useful for setting up a new node without creating a new node object.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the node bounding box.</param>
        /// <param name="y">The y position of the top-left corner of the node bounding box.</param>
        /// <param name="width">The width of the node bounding box.</param>
        /// <param name="height">The height of the node bounding box.</param>
        /// <param name="level">The split level of the node.</param>
        /// <returns>This quadtree node itself.</returns>
        public GenQuadtreeNode Initialize(float x, float y, float width, float height, int level)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            _level = level;

            Clear();

            return this;
        }

        /// <summary>
        /// Clears the quadtree node objects list and sets the leaf nodes to null.
        /// </summary>
        public void Clear()
        {
            _objects.Clear();

            for (int i = 0; i < _nodes.Length; i++)
                _nodes[i] = null;
        }

        /// <summary>
        /// Gets the index of the node that entirely contains the given bounding box.
        /// </summary>
        /// <param name="box">The bounding box used to search the nodes.</param>
        /// <returns>The index of the node that entirely contains the given bounding box.</returns>
        private int GetIndex(GenAABB box)
        {
            if ((box.Left > _left) && (box.Right < _right) && (box.Top > _top) && (box.Bottom < _bottom))
            {
                if (box.Right < _midpointY)
                {
                    if (box.Bottom < _midpointX)
                        return 1;
                    else if (box.Top > _midpointX)
                        return 2;
                }
                else if (box.Left > _midpointY)
                {
                    if (box.Bottom < _midpointX)
                        return 0;
                    else if (box.Top > _midpointX)
                        return 3;
                }
            }

            return -1;
        }

        /// <summary>
        /// Iterates through each node that entirely contains the given object or group of objects, and adds the object to the lowest level node possible.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to insert into the quadtree.</param>
        public void Insert(GenBasic objectOrGroup)
        {
            if (objectOrGroup is GenGroup)
            {
                foreach (GenBasic member in ((GenGroup)objectOrGroup).Members)
                    Insert(member);
            }
            else
            {
                if (_level != 0 || (_level == 0 && (((GenObject)objectOrGroup).BoundingBox.Left < _right && ((GenObject)objectOrGroup).BoundingBox.Right > _left && ((GenObject)objectOrGroup).BoundingBox.Top < _bottom && ((GenObject)objectOrGroup).BoundingBox.Bottom > _top)))
                {
                    if (_nodes[0] != null)
                    {
                        int index = GetIndex(((GenObject)objectOrGroup).BoundingBox);

                        if (index != -1)
                        {
                            _nodes[index].Insert(objectOrGroup);

                            return;
                        }
                    }

                    _objects.Add(objectOrGroup);

                    if ((_objects.Count > GenQuadtree.MAX_OBJECTS) && (_level < GenQuadtree.MAX_LEVELS))
                    {
                        if (_nodes[0] == null)
                        {
                            _nodes[0] = _quadtree.GetNode(_midpointX, _top, _halfWidth, _halfHeight, _level + 1);
                            _nodes[1] = _quadtree.GetNode(_left, _top, _halfWidth, _halfHeight, _level + 1);
                            _nodes[2] = _quadtree.GetNode(_left, _midpointY, _halfWidth, _halfHeight, _level + 1);
                            _nodes[3] = _quadtree.GetNode(_midpointX, _midpointY, _halfWidth, _halfHeight, _level + 1);
                        }

                        int i = 0;

                        while (i < _objects.Count)
                        {
                            int index = GetIndex(((GenObject)_objects[i]).BoundingBox);

                            if (index != -1)
                            {
                                _nodes[index].Insert(_objects[i]);
                                _objects.RemoveAt(i);
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Iterates through each node that entirely contains the given bounding box, and retrieves a list of objects from each node.
        /// </summary>
        /// <param name="returnObjects">A list that will be populated with accessible game objects.</param>
        /// <param name="box">The bounding box used to search for nodes.</param>
        public void Retrieve(List<GenBasic> returnObjects, GenAABB box)
        {
            if (_nodes[0] != null)
            {
                int index = GetIndex(box);

                if (index != -1)
                    _nodes[index].Retrieve(returnObjects, box);
                else
                    RetrieveNodes(returnObjects, box);
            }

            returnObjects.AddRange(_objects);
        }

        /// <summary>
        /// Iterates through remaining leaf nodes that intersect the given bounding box, and retrieves a list of objects from those nodes.
        /// </summary>
        /// <param name="returnObjects">A list that will be populated with accessible game objects.</param>
        /// <param name="box">The bounding box used to search for nodes.</param>
        /// <returns>A list populated with accessible game objects.</returns>
        public void RetrieveNodes(List<GenBasic> returnObjects, GenAABB box)
        {
            if (_nodes[0] != null)
            {
                if (box.Left <= _midpointY)
                {
                    if (box.Top <= _midpointX)
                    {
                        _nodes[1].RetrieveNodes(returnObjects, box);
                        returnObjects.AddRange(_nodes[1]._objects);
                    }

                    if (box.Bottom >= _midpointX)
                    {
                        _nodes[2].RetrieveNodes(returnObjects, box);
                        returnObjects.AddRange(_nodes[2]._objects);
                    }
                }

                if (box.Right >= _midpointY)
                {
                    if (box.Top <= _midpointX)
                    {
                        _nodes[0].RetrieveNodes(returnObjects, box);
                        returnObjects.AddRange(_nodes[0]._objects);
                    }

                    if (box.Bottom >= _midpointX)
                    {
                        _nodes[3].RetrieveNodes(returnObjects, box);
                        returnObjects.AddRange(_nodes[3]._objects);
                    }
                }
            }
        }

        /// <summary>
        /// Draws debug lines that visually represent each node.
        /// </summary>
        public void Draw()
        {
            Draw(_nodes);
        }

        /// <summary>
        /// Draws debug lines that visually represent a node.
        /// </summary>
        /// <param name="_nodes">The array of leaf nodes to draw.</param>
        protected void Draw(GenQuadtreeNode[] _nodes)
        {
            for (int i = 0; i < 4; i++)
            {
                if (_nodes[i] != null)
                {
                    // Draw the top bounding box line.
                    GenG.DrawLine(_nodes[i]._left, _nodes[i]._top, _nodes[i]._right, _nodes[i]._top, Color.White);

                    // Draw the right bounding box line.
                    GenG.DrawLine(_nodes[i]._right, _nodes[i]._top, _nodes[i]._right, _nodes[i]._bottom, Color.White);

                    // Draw the bottom bounding box line.
                    GenG.DrawLine(_nodes[i]._left, _nodes[i]._bottom, _nodes[i]._right, _nodes[i]._bottom, Color.White);

                    // Draw the left bounding box line.
                    GenG.DrawLine(_nodes[i]._left, _nodes[i]._top, _nodes[i]._left, _nodes[i]._bottom, Color.White);

                    if (_nodes[i]._nodes != null)
                    {
                        //GenG.SpriteBatch.DrawString(GenG.Game.font, _nodes[i]._objects.Count.ToString(), new Vector2(_nodes[i]._bounds.X, _nodes[i]._bounds.Y), Color.White);
                        Draw(_nodes[i]._nodes);
                    }
                }
            }
        }
    }
}