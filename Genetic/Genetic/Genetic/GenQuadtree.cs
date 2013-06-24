using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Genetic.Geometry;
using Genetic.Physics;

namespace Genetic
{
    /// <summary>
    /// A quadtree data structure manager.
    /// Useful for optimizing overlap and collision checks among multiple game objects.
    /// Handles moving objects by inserting them back into the quadtree, and removes only the quadtree nodes which no longer contain any objects.
    /// Game objects that do not intersect the quadtree bounds will not be inserted into any nodes, avoiding too many overlap and collision checks.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenQuadtree
    {
        /// <summary>
        /// The maximum amount of objects allowed in a single node.
        /// </summary>
        public int MaxObjects;

        /// <summary>
        /// The maximum amount of splits that can occur through a series of nodes.
        /// </summary>
        public int MaxLevels;

        /// <summary>
        /// The highest level node of the quadtree in which all other nodes will be contained.
        /// </summary>
        protected GenQuadtreeNode _rootNode;

        /// <summary>
        /// A list of each <c>GenObject</c> added to the quadtree manager.
        /// Modifying this list directly may produce undesired results.
        /// </summary>
        public List<GenObject> Objects;

        /// <summary>
        /// A list of each <c>GenObject</c> added to the quadtree and active within its nodes.
        /// </summary>
        protected List<GenObject> _activeObjects;

        /// <summary>
        /// A flag used to determine if any objects are waiting to be added or removed from the active objects list.
        /// Prevents new objects from being directly added or removed from the active members list during update loops.
        /// </summary>
        protected bool _updateObjects;

        /// <summary>
        /// The node location of each active object in the quadtree.
        /// </summary>
        internal Dictionary<GenObject, GenQuadtreeNode> _objectLocations;

        /// <summary>
        /// A manager for a quadtree data structure.
        /// </summary>
        /// <param name="x">The x position of the top-left corner of the root node bounding box.</param>
        /// <param name="y">The y position of the top-left corner of the root node bounding box.</param>
        /// <param name="width">The width of the root node bounding box.</param>
        /// <param name="height">The height of the root node bounding box.</param>
        public GenQuadtree(float x, float y, float width, float height)
        {
            MaxObjects = 5;
            MaxLevels = 5;
            _rootNode = new GenQuadtreeNode(x, y, width, height, this, null, 0);
            Objects = new List<GenObject>();
            _activeObjects = new List<GenObject>();
            _updateObjects = false;
            _objectLocations = new Dictionary<GenObject, GenQuadtreeNode>();
        }

        /// <summary>
        /// Updates the quadtree by inserting recently added game objects into its nodes as active objects.
        /// Game objects that have moved since the previous update will be inserted back into the quadtree.
        /// </summary>
        public void Update()
        {
            // If any objects have been added or removed since the last update, repopulate the quadtree.
            if (_updateObjects)
            {
                // Clear the quadtree nodes.
                _rootNode.Clear();

                // Repopulate the active objects list with the current objects that have been added to the quadtree manager.
                _activeObjects.Clear();
                _activeObjects.AddRange(Objects);

                // Clear the current object locations, since the active objects list has been repopulated.
                // Allows for new object locations to be created as the current active objects are being inserted back into the quadtree.
                _objectLocations.Clear();

                // Insert each active object into the quadtree.
                foreach (GenObject activeObject in _activeObjects)
                    _rootNode.Add(activeObject);

                _updateObjects = false;
            }

            foreach (GenObject activeObject in _activeObjects)
            {
                if (activeObject.HasMoved)
                {
                    if (_objectLocations[activeObject]._objects.Remove(activeObject))
                        CheckObjects(_objectLocations[activeObject]);

                    // Check if the game object intersects the root node before inserting it into the quadtree.
                    // Prevents objects that are outside of the quadtree bounds from being checked for overlaps or collisions, possibly causing slowdown.
                    if (activeObject.MoveBounds.Intersects(_rootNode))
                        _rootNode.Add(activeObject);
                }
            }
        }

        /// <summary>
        /// Decrements the object cound in a given node.
        /// Clears each leaf node within the given node if the object count is 0.
        /// Walks up the tree from the first given node, repeating the process for each parent node.
        /// </summary>
        /// <param name="node">The node to check for its object count.</param>
        protected void CheckObjects(GenQuadtreeNode node)
        {
            if (node != null)
            {
                if (--node._objectCount <= 0)
                    node.ClearLeaves();

                CheckObjects(node._parentNode);
            }
        }

        /// <summary>
        /// Adds an object or group of objects to the quadtree.
        /// Added objects will be inserted into the quadtree nodes during the next update, and added to the active objects list.
        /// </summary>
        /// <param name="objectOrGroup">The object or group of objects to add.</param>
        public void Add(GenBasic objectOrGroup)
        {
            if (objectOrGroup is GenObject)
            {
                // If the object has already been added to the quadtree, do not add the same object twice.
                if (!Objects.Contains(objectOrGroup as GenObject))
                {
                    Objects.Add(objectOrGroup as GenObject);
                    _updateObjects = true;
                }
            }
            else if (objectOrGroup is GenGroup)
            {
                foreach (GenBasic basic in (objectOrGroup as GenGroup).Members)
                    Add(basic);
            }
        }

        /// <summary>
        /// Checks for overlaps or collisions between the given game object and objects within the quadtree.
        /// A broad phase check is done by only checking for overlaps or collisions against objects within the nodes that intersect with the given game object.
        /// </summary>
        /// <param name="gameObject">The game object to check for overlaps or collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <param name="separate">Determines if objects should collide with each other.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the quadtree objects are collidable.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        public bool Overlap(GenObject gameObject, CollideEvent callback, bool separate, GenObject.Direction collidableEdges)
        {
            return _rootNode.Overlap(gameObject, callback, separate, collidableEdges);
        }

        /// <summary>
        /// Calls <c>Clear</c> on the root quadtree node to recursively clear all leaf nodes in the quadtree.
        /// All leaf nodes will be set to null after their objects lists have been cleared.
        /// </summary>
        public void Clear()
        {
            _rootNode.Clear();
            Objects.Clear();
        }

        /// <summary>
        /// Draws debug lines that visually represent each node.
        /// </summary>
        public void DrawDebug()
        {
            _rootNode.DrawDebug();
        }
    }

    /// <summary>
    /// Makes up a single node within a quadtree data structure.
    /// Manages game objects within the node, and assigns overlap and collision checks against those objects.
    /// Manages references to its four leaf nodes.
    /// </summary>
    public class GenQuadtreeNode : GenAABB
    {
        /// <summary>
        /// A reference to the quadtree manager of this node.
        /// </summary>
        protected GenQuadtree _quadtree;

        /// <summary>
        /// The quadtree node that this node is a leaf of.
        /// The root node will have a parent node of null.
        /// </summary>
        internal GenQuadtreeNode _parentNode;

        /// <summary>
        /// The current split level of the node.
        /// </summary>
        protected int _level;

        /// <summary>
        /// A list of game objects associated with the node.
        /// </summary>
        internal List<GenObject> _objects;

        /// <summary>
        /// An internal tracker for the amount of objects currently within this node and its leaf nodes.
        /// </summary>
        internal int _objectCount;

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
        /// <param name="parentNode">The quadtree node that this node is a leaf of. Use null if this is a root node.</param>
        /// <param name="level">The split level of the node.</param>
        public GenQuadtreeNode(float x, float y, float width, float height, GenQuadtree quadtree, GenQuadtreeNode parentNode, int level)
            : base(x, y, width, height)
        {
            _quadtree = quadtree;
            _parentNode = parentNode;
            _level = level;
            _objects = new List<GenObject>();
            _objectCount = 0;
            _nodes = new GenQuadtreeNode[4];
        }

        internal void Add(GenObject gameObject)
        {
            if (_nodes[0] == null)
            {
                Insert(gameObject);

                // If the node is full, attempt to add each of its objects into a leaf node.
                if ((_objects.Count >= _quadtree.MaxObjects) && (_level < _quadtree.MaxLevels))
                {
                    int nextLevel = _level + 1;

                    // Create the leaf nodes.
                    _nodes[0] = new GenQuadtreeNode(Left, Top, _halfWidth, _halfHeight, _quadtree, this, nextLevel);
                    _nodes[1] = new GenQuadtreeNode(_midpointX, Top, _halfWidth, _halfHeight, _quadtree, this, nextLevel);
                    _nodes[2] = new GenQuadtreeNode(Left, _midpointY, _halfWidth, _halfHeight, _quadtree, this, nextLevel);
                    _nodes[3] = new GenQuadtreeNode(_midpointX, _midpointY, _halfWidth, _halfHeight, _quadtree, this, nextLevel);

                    // Iterate through the objects list in reverse, attempting to add each object into a leaf node.
                    // Otherwise, leave the object in this node.
                    for (int i = _objects.Count - 1; i >= 0; i--)
                    {
                        int index = GetIndex(_objects[i].MoveBounds);

                        if (index != -1)
                        {
                            _nodes[index].Add(_objects[i]);
                            _objects.Remove(_objects[i]);
                        }
                    }
                }
            }
            else
            {
                // Since leaf nodes have already been created, attempt to add the object into a leaf node.
                // Otherwise, add the object to this node.
                int index = GetIndex(gameObject.MoveBounds);

                if (index != -1)
                    _nodes[index].Add(gameObject);
                else
                    Insert(gameObject);
            }

            _objectCount++;
        }

        /// <summary>
        /// Inserts a given object by adding it to this quadtree node's objects list, and sets this node as its current location.
        /// </summary>
        /// <param name="gameObject">The <c>GenObject</c> to insert into this node.</param>
        protected void Insert(GenObject gameObject)
        {
            // If the game object is already contained within the node, do not insert the same object twice.
            _objects.Add(gameObject);

            // Set the object's location to this node.
            if (_quadtree._objectLocations.ContainsKey(gameObject))
                _quadtree._objectLocations[gameObject] = this;
            else
                _quadtree._objectLocations.Add(gameObject, this);
        }

        /// <summary>
        /// Gets the index of the leaf node that entirely contains the given bounding box.
        /// </summary>
        /// <param name="box">The bounding box used to search the leaf nodes.</param>
        /// <returns>The index of the leaf node that entirely contains the given bounding box. Otherwise, -1 is returned.</returns>
        internal int GetIndex(GenAABBBasic box)
        {
            if (Contains(box))
            {
                if (box.Bottom < _midpointY)
                {
                    if (box.Right < _midpointX) // Top-left leaf node.
                        return 0;
                    else if (box.Left > _midpointX) // Top-right leaf node.
                        return 1;
                }
                else if (box.Top > _midpointY)
                {
                    if (box.Right < _midpointX) // Bottom-left leaf node.
                        return 2;
                    else if (box.Left > _midpointX) // Bottom-right leaf node.
                        return 3;
                }
            }

            // Return -1 if the given bounding box does not fit within any leaf node.
            return -1;
        }

        /// <summary>
        /// Checks for overlaps or collisions between the given game object and objects within the node and its leaf nodes.
        /// A broad phase check is done by only checking for overlaps or collisions against objects within the leaf nodes that intersect with the given game object.
        /// </summary>
        /// <param name="gameObject">The game object to check for overlaps or collisions.</param>
        /// <param name="callback">The delegate method that will be invoked if an overlap occurs.</param>
        /// <param name="separate">Determines if objects should collide with each other.</param>
        /// <param name="collidableEdges">A bit field of flags determining which edges of the quadtree objects are collidable.</param>
        /// <returns>True if an overlap occurs, false if not.</returns>
        internal bool Overlap(GenObject gameObject, CollideEvent callback, bool separate, GenObject.Direction collidableEdges)
        {
            bool overlap = false;

            if (_nodes[0] != null)
            {
                // If the game object's movement bounds intersects with any leaf node, do overlap and collision checks against any objects in that node.
                if (gameObject.MoveBounds.Intersects(this))
                {
                    if (gameObject.MoveBounds.Top <= _midpointY)
                    {
                        if (gameObject.MoveBounds.Left <= _midpointX) // Top-left leaf node.
                        {
                            if (_nodes[0].Overlap(gameObject, callback, separate, collidableEdges))
                                overlap = true;
                        }

                        if (gameObject.MoveBounds.Right >= _midpointX) // Top-right leaf node.
                        {
                            if (_nodes[1].Overlap(gameObject, callback, separate, collidableEdges))
                                overlap = true;
                        }
                    }

                    if (gameObject.MoveBounds.Bottom >= _midpointY)
                    {
                        if (gameObject.MoveBounds.Left <= _midpointX) // Bottom-left leaf node.
                        {
                            if (_nodes[2].Overlap(gameObject, callback, separate, collidableEdges))
                                overlap = true;
                        }

                        if (gameObject.MoveBounds.Right >= _midpointX) // Bottom-right leaf node.
                        {
                            if (_nodes[3].Overlap(gameObject, callback, separate, collidableEdges))
                                overlap = true;
                        }
                    }
                }
            }

            // Check for overlaps and collisions against this node's objects.
            foreach (GenObject nodeObject in _objects)
            {
                if (separate)
                {
                    if (GenCollide.Collide(gameObject, nodeObject, callback, collidableEdges))
                        overlap = true;
                }
                else
                {
                    if (GenCollide.Overlap(gameObject, nodeObject, callback))
                        overlap = true;
                }
            }

            return overlap;
        }

        /// <summary>
        /// Clears the quadtree node objects list.
        /// Calls <c>Clear</c> on each leaf node, which are then set to null.
        /// </summary>
        public void Clear()
        {
            _objects.Clear();

            if (_nodes[0] != null)
            {
                for (int i = 0; i < _nodes.Length; i++)
                {
                    // Call Clear on the leaf node before setting it to null.
                    _nodes[i].Clear();
                    _nodes[i] = null;
                }
            }
        }

        /// <summary>
        /// Sets each leaf node to null.
        /// </summary>
        public void ClearLeaves()
        {
            if (_nodes[0] != null)
            {
                for (int i = 0; i < _nodes.Length; i++)
                    _nodes[i] = null;
            }
        }

        /// <summary>
        /// Draws debug lines that visually represent each leaf node.
        /// </summary>
        internal void DrawDebug()
        {
            if (_nodes[0] != null)
            {
                foreach (GenQuadtreeNode node in _nodes)
                {
                    Vector2 topRight = new Vector2(node._max.X, node._min.Y);
                    Vector2 bottomLeft = new Vector2(node._min.X, node._max.Y);

                    // Draw the top bounding box line.
                    GenG.DrawLine(node._min, topRight, Color.White);

                    // Draw the right bounding box line.
                    GenG.DrawLine(topRight, node._max, Color.White);

                    // Draw the bottom bounding box line.
                    GenG.DrawLine(bottomLeft, node._max, Color.White);

                    // Draw the left bounding box line.
                    GenG.DrawLine(node._min, bottomLeft, Color.White);

                    // Draw the leaf nodes contained within this node.
                    node.DrawDebug();
                }
            }
        }
    }
}