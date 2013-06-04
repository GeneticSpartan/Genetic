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
        internal const int MAX_OBJECTS = 10;

        /// <summary>
        /// The maximum amount of splits that can occur through a series of nodes.
        /// </summary>
        internal const int MAX_LEVELS = 6;

        /// <summary>
        /// The highest level node of the quadtree in which all other nodes will be contained.
        /// </summary>
        protected GenQuadtreeNode _rootNode;

        /// <summary>
        /// A dictionary holding the quadtree node location of each object inserted into the quadtree.
        /// </summary>
        internal Dictionary<GenObject, GenQuadtreeNode> _objectLocations;

        internal List<GenObject> _objectLocationKeys;

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
            _objectLocations = new Dictionary<GenObject, GenQuadtreeNode>();
            _objectLocationKeys = new List<GenObject>();
        }

        /// <summary>
        /// Checks each object in the quadtree, and places any object that has moved since the last update into the correct node.
        /// </summary>
        public void Update()
        {
            foreach (GenObject objectKey in _objectLocationKeys)
            {
                // If the object has not moved since the last update, move on to the next object.
                if (objectKey.Position == objectKey.OldPosition)
                    continue;

                // Remove the object from its current node location, and re-insert the object into the quadtree.
                _objectLocations[objectKey].Remove(objectKey);
                _rootNode.Insert(objectKey);
            }
        }

        /// <summary>
        /// Calls Clear on the root quadtree node, and resets the node index value to 0.
        /// </summary>
        public void Clear()
        {
            _rootNode.Clear();
            _objectLocations.Clear();
            _objectLocationKeys.Clear();
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
        /// Removes a given object from the quadtree by removing the object from its containing node.
        /// </summary>
        /// <param name="gameObject">The game object to remove from the quadtree.</param>
        public void Remove(GenObject gameObject)
        {
            // Remove the object from its containing node.
            _objectLocations[gameObject].Remove(gameObject);

            // Remove node location references to the object.
            _objectLocations.Remove(gameObject);
            _objectLocationKeys.Remove(gameObject);
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
        /// Iterates through each node that entirely contains the given object's bounding box, and checks for a collision with every object in each node.
        /// </summary>
        /// <param name="gameObject">The object to check for a collision.</param>
        /// <param name="callback">The delegate method that will be invoked if a collision occurs.</param>
        /// <param name="separate">Determines if objects should collide with each other.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for elastic collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the given object are collidable.</param>
        /// <returns>True if a collision occurs, false if not.</returns>
        public bool Overlap(GenObject gameObject, CollideEvent callback = null, bool separate = false, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            return _rootNode.Overlap(gameObject, callback, separate, penetrate, collidableEdges);
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

            // Return -1 if the given bounding box does not fit within any leaf node.
            return -1;
        }

        /// <summary>
        /// Iterates through each node that entirely contains the given object or group of objects, and adds the object to the lowest level node possible.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to insert into the quadtree.</param>
        public void Insert(GenBasic objectOrGroup)
        {
            if (objectOrGroup is GenObject)
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

                // If the object is not already in the quadtree, add the object to the object locations list in the quadtree manager using this node as its location.
                // Otherwise, update the object's node location.
                if (_quadtree._objectLocationKeys.Contains((GenObject)objectOrGroup))
                    _quadtree._objectLocations[(GenObject)objectOrGroup] = this;
                else
                {
                    _quadtree._objectLocations.Add((GenObject)objectOrGroup, this);
                    _quadtree._objectLocationKeys.Add((GenObject)objectOrGroup);
                }

                if ((_objects.Count > GenQuadtree.MAX_OBJECTS) && (_level < GenQuadtree.MAX_LEVELS))
                {
                    if (_nodes[0] == null)
                    {
                        _nodes[0] = new GenQuadtreeNode(_midpointX, _top, _halfWidth, _halfHeight, _quadtree, _level + 1);
                        _nodes[1] = new GenQuadtreeNode(_left, _top, _halfWidth, _halfHeight, _quadtree, _level + 1);
                        _nodes[2] = new GenQuadtreeNode(_left, _midpointY, _halfWidth, _halfHeight, _quadtree, _level + 1);
                        _nodes[3] = new GenQuadtreeNode(_midpointX, _midpointY, _halfWidth, _halfHeight, _quadtree, _level + 1);
                    }

                    // Attempt to move each object in this node to a leaf node.
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
                            i++;
                    }
                }
            }
            else if (objectOrGroup is GenGroup)
            {
                foreach (GenBasic member in ((GenGroup)objectOrGroup).Members)
                    Insert(member);
            } 
        }

        /// <summary>
        /// Removes a given object from this node.
        /// </summary>
        /// <param name="gameObject">The game object to remove.</param>
        public void Remove(GenObject gameObject)
        {
            _objects.Remove(gameObject);
        }

        /// <summary>
        /// Iterates through each node that entirely contains the given bounding box, and retrieves a list of objects from each of those nodes.
        /// </summary>
        /// <param name="returnObjects">A list that will be populated with accessible game objects.</param>
        /// <param name="box">The bounding box used to search for nodes.</param>
        public void Retrieve(List<GenBasic> returnObjects, GenAABB box)
        {
            if (_nodes[0] != null)
            {
                // Get the index of the leaf node that entirely contains the bounding box.
                int index = GetIndex(box);

                // If the bounding box fits within a leaf node, iterate through that node.
                // Otherwise, iterate through each leaf node.
                if (index != -1)
                    _nodes[index].Retrieve(returnObjects, box);
                else
                    RetrieveNodes(returnObjects, box);
            }

            // Retrieve the objects within this node.
            returnObjects.AddRange(_objects);
        }

        /// <summary>
        /// Iterates through remaining leaf nodes that intersect the given bounding box.
        /// </summary>
        /// <param name="returnObjects">A list that will be populated with accessible game objects.</param>
        /// <param name="box">The bounding box used to search for nodes.</param>
        /// <returns>A list populated with accessible game objects.</returns>
        protected void RetrieveNodes(List<GenBasic> returnObjects, GenAABB box)
        {
            // If the bounding box intersects a leaf node, iterate through that node.
            if (box.Left <= _midpointY)
            {
                if (box.Top <= _midpointX)
                    _nodes[1].Retrieve(returnObjects, box);

                if (box.Bottom >= _midpointX)
                    _nodes[2].Retrieve(returnObjects, box);
            }

            if (box.Right >= _midpointY)
            {
                if (box.Top <= _midpointX)
                    _nodes[0].Retrieve(returnObjects, box);

                if (box.Bottom >= _midpointX)
                    _nodes[3].Retrieve(returnObjects, box);
            }
        }

        /// <summary>
        /// Iterates through each node that entirely contains the given object's bounding box, and checks for overlaps or collisions against every object in each node.
        /// </summary>
        /// <param name="gameObject">The object to check for an overlap.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <param name="separate">Determines if objects should collide with each other.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for elastic collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the given object are collidable.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        public bool Overlap(GenObject gameObject, CollideEvent callback = null, bool separate = false, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            bool overlap = false;

            if (_nodes[0] != null)
            {
                // Get the index of the leaf node that entirely contains the object's bounding box.
                int index = GetIndex(gameObject.BoundingBox);

                // If the object's bounding box fits within a leaf node, check for overlaps or collisions against objects within that node.
                // Otherwise, check for overlaps or collisions against objects within each leaf node.
                if (index != -1)
                {
                    if (_nodes[index].Overlap(gameObject, callback, separate, penetrate, collidableEdges) && !overlap)
                        overlap = true;
                }
                else
                    if (OverlapNodes(gameObject, callback, separate, penetrate, collidableEdges) && !overlap)
                        overlap = true;
            }

            // Check for overlaps or collisions against objects within this node.
            foreach (GenBasic basic in _objects)
            {
                if (basic is GenObject)
                {
                    if (separate)
                    {
                        if (gameObject.Collide((GenObject)basic, callback, penetrate, collidableEdges) && !overlap)
                            overlap = true;
                    }
                    else
                    {
                        if (gameObject.Overlap((GenObject)basic, callback) && !overlap)
                            overlap = true;
                    }
                }
            }

            return overlap;
        }

        /// <summary>
        /// Iterates through remaining leaf nodes that intersect the given object's bounding box, and checks for overlaps or collisions against every object in the leaf node.
        /// </summary>
        /// <param name="gameObject">The object to check for an overlap.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <param name="separate">Determines if objects should collide with each other.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for elastic collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the given object are collidable.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        protected bool OverlapNodes(GenObject gameObject, CollideEvent callback = null, bool separate = false, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            bool overlap = false;

            // If the object's bounding box intersects a leaf node, check for overlaps or collisions within that node.
            if (gameObject.BoundingBox.Left <= _midpointY)
            {
                if (gameObject.BoundingBox.Top <= _midpointX)
                {
                    if (OverlapLeafNode(1, gameObject, callback, separate, penetrate, collidableEdges))
                        overlap = true;
                }

                if (gameObject.BoundingBox.Bottom >= _midpointX)
                {
                    if (OverlapLeafNode(2, gameObject, callback, separate, penetrate, collidableEdges))
                        overlap = true;
                }
            }

            if (gameObject.BoundingBox.Right >= _midpointY)
            {
                if (gameObject.BoundingBox.Top <= _midpointX)
                {
                    if (OverlapLeafNode(0, gameObject, callback, separate, penetrate, collidableEdges))
                        overlap = true;
                }

                if (gameObject.BoundingBox.Bottom >= _midpointX)
                {
                    if (OverlapLeafNode(3, gameObject, callback, separate, penetrate, collidableEdges))
                        overlap = true;
                }
            }

            return overlap;
        }

        /// <summary>
        /// Iterates through each object in a leaf node, and checks for overlaps or collisions against a given object.
        /// </summary>
        /// <param name="nodeIndex">The index number of the leaf node to check.</param>
        /// <param name="gameObject">The object to check for an overlap.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <param name="separate">Determines if objects should collide with each other.</param>
        /// <param name="penetrate">Determines if the objects are able to penetrate each other for elastic collision response.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the given object are collidable.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        protected bool OverlapLeafNode(int nodeIndex, GenObject gameObject, CollideEvent callback = null, bool separate = false, bool penetrate = true, GenObject.Direction collidableEdges = GenObject.Direction.Any)
        {
            bool overlap = false;

            // Check for overlaps or collisions against objects within a leaf node.
            foreach (GenBasic basic in _nodes[nodeIndex]._objects)
            {
                if (basic is GenObject)
                {
                    if (separate)
                    {
                        if (gameObject.Collide((GenObject)basic, callback, penetrate, collidableEdges) && !overlap)
                            overlap = true;
                    }
                    else
                    {
                        if (gameObject.Overlap((GenObject)basic, callback) && !overlap)
                            overlap = true;
                    }
                }
            }

            return overlap;
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
            if (_nodes[0] != null)
            {
                foreach (GenQuadtreeNode node in _nodes)
                {
                    // Draw the top bounding box line.
                    GenG.DrawLine(node._left, node._top, node._right, node._top, Color.White);

                    // Draw the right bounding box line.
                    GenG.DrawLine(node._right, node._top, node._right, node._bottom, Color.White);

                    // Draw the bottom bounding box line.
                    GenG.DrawLine(node._left, node._bottom, node._right, node._bottom, Color.White);

                    // Draw the left bounding box line.
                    GenG.DrawLine(node._left, node._top, node._left, node._bottom, Color.White);

                    if (node._nodes != null)
                    {
                        //GenG.SpriteBatch.DrawString(GenG.Font, node._objects.Count.ToString(), new Vector2(node._left, node._top), Color.White);
                        Draw(node._nodes);
                    }
                }
            }
        }
    }
}