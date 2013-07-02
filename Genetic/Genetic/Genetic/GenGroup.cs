using System;
using System.Collections.Generic;

namespace Genetic
{
    /// <summary>
    /// A container that manages a group of game objects derived from <c>GenBasic</c>, including any updating and drawing.
    /// Each group may also contain a quadtree data structure to handle overlap and collision checks against its members.
    /// 
    /// Author: Tyler Gregory (GeneticSpartan)
    /// </summary>
    public class GenGroup : GenBasic
    {
        /// <summary>
        /// The GenBasic objects that have been added to the group.
        /// </summary>
        public List<GenBasic> Members;

        /// <summary>
        /// The GenBasic objects that have been added to the group, and are available for calling updates.
        /// </summary>
        protected List<GenBasic> _activeMembers;

        /// <summary>
        /// A flag used to determine if any objects are waiting to be added to the active members list.
        /// Prevents new objects from being directly added or removed from the active members list during update loops.
        /// </summary>
        protected bool _updateMembers;

        /// <summary>
        /// A quadtree data structure for group member objects.
        /// Useful for faster collision detection against objects in the quadtree.
        /// </summary>
        public GenQuadtree Quadtree;

        /// <summary>
        /// A flag used to determine if the group should be cleared.
        /// </summary>
        protected bool _clear;

        /// <summary>
        /// A group used to manage a list of objects inherited from GenBasic.
        /// </summary>
        public GenGroup()
        {
            Members = new List<GenBasic>();
            _activeMembers = new List<GenBasic>();
            _updateMembers = false;

            Quadtree = null;
            _clear = false;
        }

        /// <summary>
        /// Calls PreUpdate on each of the objects in the active members list.
        /// Override this method to add additional pre-update logic.
        /// </summary>
        public override void PreUpdate()
        {
            // Clear the active members list if the group has been cleared.
            if (_clear)
            {
                _activeMembers.Clear();
                _clear = false;
            }

            // if any objects have been recently added or removed, update the active members list.
            if (_updateMembers)
            {
                _activeMembers.Clear();
                _activeMembers.AddRange(Members);
                _updateMembers = false;
            }

            if (Exists && Active)
            {
                foreach (GenBasic member in _activeMembers)
                {
                    if (member.Exists && member.Active)
                        member.PreUpdate();
                }
            }
        }

        /// <summary>
        /// Calls Update on each of the objects in the active members list.
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (!Exists || !Active)
                return;
            
            foreach (GenBasic member in _activeMembers)
            {
                if (member.Exists && member.Active)
                    member.Update();
            }

            // Update the quadtree.
            if (Quadtree != null)
                Quadtree.Update();
        }

        /// <summary>
        /// Calls PostUpdate on each of the objects in the active members list.
        /// Override this method to add additional post-update logic.
        /// </summary>
        public override void PostUpdate()
        {
            if (!Exists || !Active)
                return;
            
            foreach (GenBasic member in _activeMembers)
            {
                if (member.Exists && member.Active)
                    member.PostUpdate();
            }
        }

        /// <summary>
        /// Calls Draw on each of the objects in the active members list.
        /// </summary>
        /// <param name="camera">The camera used to draw.</param>
        public override void Draw(GenCamera camera)
        {
            if (!Exists || !Visible)
                return;

            foreach (GenBasic member in _activeMembers)
            {
                if (!member.Exists)
                    continue;

                if (member.Visible)
                    member.Draw(camera);

                if (GenG.AllowDebug && GenG.IsDebug)
                    member.DrawDebug(camera);
            }

            if (GenG.AllowDebug && GenG.IsDebug && (Quadtree != null))
                Quadtree.DrawDebug();
        }

        /// <summary>
        /// Adds an object to the group.
        /// If the group already contains the object, the object will not be added again.
        /// </summary>
        /// <param name="basic">The object to add.</param>
        /// <returns>The object that was added to the group.</returns>
        public GenBasic Add(GenBasic basic)
        {
            // Do not add the same object twice.
            if (Members.Contains(basic))
                return basic;

            Members.Add(basic);
            _updateMembers = true;

            if (Quadtree != null)
            {
                Quadtree.Add(basic);
            }

            return basic;
        }

        /// <summary>
        /// Removes a specified object from the members list.
        /// </summary>
        /// <param name="basic">The object to remove.</param>
        /// <returns>The object removed from the members list. Null if the object was not found in the members list.</returns>
        public GenBasic Remove(GenBasic basic)
        {
            // Attempt to remove the object from the members list.
            if (Members.Contains(basic))
            {
                Members.Remove(basic);
                _updateMembers = true;

                // TODO: Remove from the quadtree.

                return basic;
            }

            return null;
        }

        /// <summary>
        /// Replaces an existing object in the members list with a new object.
        /// </summary>
        /// <param name="oldMember">The existing object to replace.</param>
        /// <param name="newMember">The new object that will replace the existing object.</param>
        /// <returns>The new object that replaced the existing object. Null if the object was not found in the members list.</returns>
        public GenBasic Replace(GenBasic oldMember, GenBasic newMember)
        {
            // Attempt to replace the existing object in the members list.
            int index = Members.IndexOf(oldMember);

            if (index == -1)
                return null;

            Members[index] = newMember;
            _updateMembers = true;

            return newMember;
        }

        /// <summary>
        /// Gets the first available object in the members list by checking if Exists equals false.
        /// </summary>
        /// <returns>The first available object in the members list. Null if no object is available.</returns>
        public GenBasic GetFirstAvailable()
        {
            foreach (GenBasic member in _activeMembers)
            {
                if (!member.Exists)
                    return member;
            }

            return null;
        }

        /// <summary>
        /// Gets the first existing object in the members list by checking if Exists equals true.
        /// Useful for determining if any objects in the members list are existing.
        /// </summary>
        /// <returns>The first existing object in the members list. Null if no object is existing.</returns>
        public GenBasic GetFirstExisting()
        {
            foreach (GenBasic member in _activeMembers)
            {
                if (member.Exists)
                    return member;
            }

            return null;
        }

        /// <summary>
        /// Gets the first alive object in the members list by checking if Exists and Active both equal true.
        /// Useful for determining if any objects in the members list are alive.
        /// </summary>
        /// <returns>The first alive object in the members list. Null if no object is alive.</returns>
        public GenBasic GetFirstAlive()
        {
            foreach (GenBasic member in _activeMembers)
            {
                if (member.Exists && member.Active)
                    return member;
            }

            return null;
        }

        /// <summary>
        /// Gets the first dead object in the members list by checking if either Exists or Active equals false.
        /// </summary>
        /// <returns>The first dead object in the members list. Null if no object is dead.</returns>
        public GenBasic GetFirstDead()
        {
            foreach (GenBasic member in _activeMembers)
            {
                if (!member.Exists || !member.Active)
                    return member;
            }

            return null;
        }

        /// <summary>
        /// Gets the first visible object in the members list by checking if Exists and Visible both equal true.
        /// Useful for determining if any objects in the members list are visible.
        /// </summary>
        /// <returns>The first visible object in the members list. Null if no object is visible.</returns>
        public GenBasic GetFirstVisible()
        {
            foreach (GenBasic member in _activeMembers)
            {
                if (member.Exists && member.Visible)
                    return member;
            }

            return null;
        }

        /// <summary>
        /// Gets the first hidden object in the members list by checking if either Exists or Visible equals false.
        /// </summary>
        /// <returns>The first hidden object in the members list. Null if no object is hidden.</returns>
        public GenBasic GetFirstHidden()
        {
            foreach (GenBasic member in _activeMembers)
            {
                if (!member.Exists || !member.Visible)
                    return member;
            }

            return null;
        }

        /// <summary>
        /// Gets a random object from the members list.
        /// </summary>
        /// <param name="startIndex">The index location in the list from where to start the search.</param>
        /// <param name="length">The number of objects in the list to include in the search from the start index. A value of 0 will use the entire list.</param>
        /// <returns>A random object from the list. Null if the index was outside of the list.</returns>
        public GenBasic GetRandom(int startIndex = 0, int length = 0)
        {
            // Get a random object from the active members list if the list contains any members.
            if (_activeMembers.Count > 0)
            {
                if (length == 0)
                    length = _activeMembers.Count;

                int index = GenU.Random(startIndex, startIndex + length);

                // Check if the index is outside of the list.
                if (_activeMembers.Count < (index - 1))
                    return null;

                return _activeMembers[index];
            }

            return null;
        }

        /// <summary>
        /// Counts the number of alive objects in the members list by checking if Exists and Active both equal true on each.
        /// </summary>
        /// <returns>The number of alive objects in the members list.</returns>
        public int CountAlive()
        {
            int count = 0;

            foreach (GenBasic member in _activeMembers)
            {
                if (member.Exists && member.Active)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Counts the number of dead objects in the members list by checking if either Exists or Active equals false on each.
        /// </summary>
        /// <returns>The number of dead objects in the members list.</returns>
        public int CountDead()
        {
            int count = 0;

            foreach (GenBasic member in _activeMembers)
            {
                if (!member.Exists || !member.Active)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Calls Kill on each object in the members list, and then on the group itself.
        /// Kill sets Exists and Active to false to "kill" an object.
        /// </summary>
        public override void Kill()
        {
            foreach (GenBasic member in _activeMembers)
            {
                member.Kill();
            }

            base.Kill();
        }

        /// <summary>
        /// Calls Revive on each object in the members list, and then on the group itself.
        /// Revive sets Exists and Active to true to "revive" an object.
        /// </summary>
        public override void Revive()
        {
            foreach (GenBasic member in _activeMembers)
                member.Revive();

            base.Revive();
        }

        /// <summary>
        /// Creates a quadtree for the group based on the current world bounds, and inserts the group's current members.
        /// </summary>
        public void MakeQuadtree()
        {
            Quadtree = new GenQuadtree(GenG.WorldBounds.X, GenG.WorldBounds.Y, GenG.WorldBounds.Width, GenG.WorldBounds.Height);

            // Add each current member of this group to the quadtree by adding the group itself.
            Quadtree.Add(this);
        }

        /// <summary>
        /// Clears the members list of all current objects.
        /// </summary>
        public void Clear()
        {
            Members.Clear();

            if (Quadtree != null)
                Quadtree.Clear();

            _clear = true;
        }
    }
}