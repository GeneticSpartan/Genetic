using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic
{
    public class GenQuadTree
    {
        /// <summary>
        /// The maximum amount of objects allowed in a node.
        /// </summary>
        protected const int MAX_OBJECTS = 5;

        /// <summary>
        /// The maximum amount of splits that can occur through a series of nodes.
        /// </summary>
        protected const int MAX_LEVELS = 4;

        /// <summary>
        /// The current split level of the node.
        /// </summary>
        protected int _level;

        /// <summary>
        /// A list of game objects associated with the node.
        /// </summary>
        protected List<GenBasic> _objects;

        /// <summary>
        /// The left bounding edge of the node.
        /// </summary>
        protected int _left;

        /// <summary>
        /// The right bounding edge of the node.
        /// </summary>
        protected int _right;

        /// <summary>
        /// The top bounding edge of the node.
        /// </summary>
        protected int _top;

        /// <summary>
        /// The bottom bounding edge of the node.
        /// </summary>
        protected int _bottom;

        protected int _halfWidth;

        protected int _halfHeight;

        /// <summary>
        /// Represents the position of a vertical center line separating the left and right areas of the node.
        /// </summary>
        protected int _verticalMidpoint;

        /// <summary>
        /// Represents the position of a horizontal center line separating the top and bottom areas of the node.
        /// </summary>
        protected int _horizontalMidpoint;

        /// <summary>
        /// An array of four leaf nodes that are created when a split occurs.
        /// </summary>
        protected GenQuadTree[] _nodes;

        public GenQuadTree(int x, int y, int width, int height, int level = 0)
        {
            _level = level;
            _objects = new List<GenBasic>();
            _left = x;
            _right = x + width;
            _top = y;
            _bottom = y + height;
            _halfWidth = width / 2;
            _halfHeight = height / 2;
            _verticalMidpoint = x + _halfWidth;
            _horizontalMidpoint = y + _halfHeight;
            _nodes = new GenQuadTree[4];
        }

        /// <summary>
        /// Clears the quadtree and each of its leaf nodes.
        /// </summary>
        public void Clear()
        {
            _objects.Clear();

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (_nodes[i] != null)
                {
                    _nodes[i].Clear();
                    _nodes[i] = null;
                }
            }
        }

        /// <summary>
        /// Gets the index of the node that entirely contains the given bounding rectangle.
        /// </summary>
        /// <param name="rect">The bounding rectangle used to search the nodes.</param>
        /// <returns>The index of the node that entirely contains the given bounding rectangle.</returns>
        private int GetIndex(Rectangle rect)
        {
            if ((rect.Left > _left) && (rect.Right < _right) && (rect.Top > _top) && (rect.Bottom < _bottom))
            {
                if (rect.Right < _verticalMidpoint)
                {
                    if (rect.Bottom < _horizontalMidpoint)
                        return 1;
                    else if (rect.Top > _horizontalMidpoint)
                        return 2;
                }
                else if (rect.X > _verticalMidpoint)
                {
                    if (rect.Bottom < _horizontalMidpoint)
                        return 0;
                    else if (rect.Top > _horizontalMidpoint)
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
                foreach (GenBasic member in ((GenGroup)objectOrGroup).members)
                    Insert(member);
            }
            else
            {
                if (_level != 0 || (_level == 0 && (((GenObject)objectOrGroup).PositionRect.Left < _right && ((GenObject)objectOrGroup).PositionRect.Right > _left && ((GenObject)objectOrGroup).PositionRect.Top < _bottom && ((GenObject)objectOrGroup).PositionRect.Bottom > _top)))
                {
                    if (_nodes[0] != null)
                    {
                        int index = GetIndex(((GenObject)objectOrGroup).PositionRect);

                        if (index != -1)
                        {
                            _nodes[index].Insert(objectOrGroup);

                            return;
                        }
                    }

                    _objects.Add(objectOrGroup);

                    if ((_objects.Count > MAX_OBJECTS) && (_level < MAX_LEVELS))
                    {
                        if (_nodes[0] == null)
                        {
                            _nodes[0] = new GenQuadTree(_left + _halfWidth, _top, _halfWidth, _halfHeight, _level + 1);
                            _nodes[1] = new GenQuadTree(_left, _top, _halfWidth, _halfHeight, _level + 1);
                            _nodes[2] = new GenQuadTree(_left, _top + _halfHeight, _halfWidth, _halfHeight, _level + 1);
                            _nodes[3] = new GenQuadTree(_left + _halfWidth, _top + _halfHeight, _halfWidth, _halfHeight, _level + 1);
                        }

                        int i = 0;

                        while (i < _objects.Count)
                        {
                            int index = GetIndex(((GenObject)_objects[i]).PositionRect);

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
        /// Iterates through each node that entirely contains the given rectangle, and retrieves a list of objects from each node.
        /// </summary>
        /// <param name="returnObjects">A list that will be populated with accessible game objects.</param>
        /// <param name="rect">The bounding rectangle used to search for nodes.</param>
        /// <returns>A list populated with accessible game objects.</returns>
        public void Retrieve(List<GenBasic> returnObjects, Rectangle rect)
        {
            if (_nodes[0] != null)
            {
                int index = GetIndex(rect);

                if (index != -1)
                    _nodes[index].Retrieve(returnObjects, rect);
                else
                    RetrieveNodes(returnObjects, rect);
            }

            returnObjects.AddRange(_objects);
        }

        /// <summary>
        /// Iterates through remaining leaf nodes that intersect the given rectangle, and retrieves a list of objects from those nodes.
        /// </summary>
        /// <param name="returnObjects">A list that will be populated with accessible game objects.</param>
        /// <param name="rect">The bounding rectangle used to search for nodes.</param>
        /// <returns>A list populated with accessible game objects.</returns>
        public void RetrieveNodes(List<GenBasic> returnObjects, Rectangle rect)
        {
            if (_nodes[0] != null)
            {
                if (rect.Left <= _verticalMidpoint)
                {
                    if (rect.Top <= _horizontalMidpoint)
                    {
                        _nodes[1].RetrieveNodes(returnObjects, rect);
                        returnObjects.AddRange(_nodes[1]._objects);
                    }

                    if (rect.Bottom >= _horizontalMidpoint)
                    {
                        _nodes[2].RetrieveNodes(returnObjects, rect);
                        returnObjects.AddRange(_nodes[2]._objects);
                    }
                }

                if (rect.Right >= _verticalMidpoint)
                {
                    if (rect.Top <= _horizontalMidpoint)
                    {
                        _nodes[0].RetrieveNodes(returnObjects, rect);
                        returnObjects.AddRange(_nodes[0]._objects);
                    }

                    if (rect.Bottom >= _horizontalMidpoint)
                    {
                        _nodes[3].RetrieveNodes(returnObjects, rect);
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
        protected void Draw(GenQuadTree[] _nodes)
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