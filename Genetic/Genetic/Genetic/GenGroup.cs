﻿using System;
using System.Collections.Generic;

namespace Genetic
{
    public class GenGroup : GenBasic
    {
        /// <summary>
        /// The GenBasic objects that have been added to the group, and are available for calling updates.
        /// </summary>
        protected List<GenBasic> _activeMembers;

        /// <summary>
        /// The GenBasic objects that have been added to the group.
        /// </summary>
        public List<GenBasic> Members;

        /// <summary>
        /// A flag used to determine if any objects are waiting to be added to the active members list.
        /// Prevents new objects from being directly added to the active members list during update loops.
        /// </summary>
        protected bool _updateMembers;

        /// <summary>
        /// The GenBasic overlay objects that have been added to the group, and are available for calling updates.
        /// </summary>
        protected List<GenBasic> _activeMembersOverlay;

        /// <summary>
        /// The GenBasic overlay objects that have been added to the group.
        /// </summary>
        public List<GenBasic> MembersOverlay;

        /// <summary>
        /// A flag used to determine if any overlay objects are waiting to be added to the active members overlay list.
        /// Prevents new overlay objects from being directly added to the active members overlay list during update loops.
        /// </summary>
        protected bool _updateMembersOverlay;

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
        /// <param name="useQuadtree">A flag used to determine if the group will use a quadtree for overlap/collision detection with other objects or groups. Use false when overlapping/colliding with tilemaps.</param>
        public GenGroup(bool useQuadtree = false)
        {
            Members = new List<GenBasic>();
            _activeMembers = new List<GenBasic>();
            _updateMembers = false;

            MembersOverlay = new List<GenBasic>();
            _activeMembersOverlay = new List<GenBasic>();
            _updateMembersOverlay = false;

            // If the group uses a quadtree for collision detection, set its position and size using the world bounding box.
            if (useQuadtree)
                Quadtree = new GenQuadtree(GenG.WorldBounds.X, GenG.WorldBounds.Y, GenG.WorldBounds.Width, GenG.WorldBounds.Height);
            else
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
                _activeMembersOverlay.Clear();
                _clear = false;
            }

            // if any objects have been recently added or removed, update the active members list.
            if (_updateMembers)
            {
                _activeMembers.Clear();
                _activeMembers.AddRange(Members);
                _updateMembers = false;
            }

            // if any overlay objects have been recently added or removed, update the active members overlay list.
            if (_updateMembersOverlay)
            {
                _activeMembersOverlay.Clear();
                _activeMembersOverlay.AddRange(MembersOverlay);
                _updateMembersOverlay = false;
            }

            if (Exists && Active)
            {
                foreach (GenBasic member in _activeMembers)
                {
                    if (member.Exists && member.Active)
                        member.PreUpdate();
                }

                foreach (GenBasic member in _activeMembersOverlay)
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
            if (Exists && Active)
            {
                foreach (GenBasic member in _activeMembers)
                {
                    if (member.Exists && member.Active)
                        member.Update();
                }

                foreach (GenBasic member in _activeMembersOverlay)
                {
                    if (member.Exists && member.Active)
                        member.Update();
                }

                if (Quadtree != null)
                    Quadtree.Update();
            }
        }

        /// <summary>
        /// Calls PostUpdate on each of the objects in the active members list.
        /// Override this method to add additional post-update logic.
        /// </summary>
        public override void PostUpdate()
        {
            if (Exists && Active)
            {
                foreach (GenBasic member in _activeMembers)
                {
                    if (member.Exists && member.Active)
                        member.PostUpdate();
                }

                foreach (GenBasic member in _activeMembersOverlay)
                {
                    if (member.Exists && member.Active)
                        member.PostUpdate();
                }
            }
        }

        /// <summary>
        /// Calls Draw on each of the objects in the active members list.
        /// </summary>
        public override void Draw()
        {
            if (Exists && Visible)
            {
                foreach (GenBasic member in _activeMembers)
                {
                    if (member.Exists)
                    {
                        if (member.Visible)
                            member.Draw();

                        if (GenG.IsDebug)
                            member.DrawDebug();
                    }
                }

                if (GenG.IsDebug && (Quadtree != null))
                    Quadtree.Draw();
            }
        }

        /// <summary>
        /// Calls Draw on each of the overlay objects in the active members overlay list.
        /// </summary>
        public override void DrawOverlay()
        {
            if (Exists && Visible)
            {
                foreach (GenBasic member in _activeMembersOverlay)
                {
                    if (member.Exists)
                    {
                        if (member.Visible)
                        {
                            if (member is GenGroup)
                                member.DrawOverlay();
                            else
                                member.Draw();
                        }

                        if (GenG.IsDebug)
                            member.DrawDebug();
                    }
                }

                if (GenG.IsDebug && (Quadtree != null))
                    Quadtree.Draw();
            }
        }

        /// <summary>
        /// Adds an object to the group.
        /// </summary>
        /// <param name="basic">The object to add.</param>
        /// <param name="overlay">A flag used to determine if the object should be drawn to the screen directly, ignoring cameras.</param>
        /// <returns>The object added to the group.</returns>
        public GenBasic Add(GenBasic basic, bool overlay = false)
        {
            if (overlay)
            {
                // Do not add the same overlay object twice.
                if (MembersOverlay.Contains(basic))
                    return basic;

                MembersOverlay.Add(basic);
                _updateMembersOverlay = true;

                if (Quadtree != null)
                {
                    if ((basic is GenObject) || (basic is GenGroup))
                        Quadtree.Insert(basic);
                }
            }
            else
            {
                // Do not add the same object twice.
                if (Members.Contains(basic))
                    return basic;

                Members.Add(basic);
                _updateMembers = true;

                if (Quadtree != null)
                {
                    if ((basic is GenObject) || (basic is GenGroup))
                        Quadtree.Insert(basic);
                }
            }

            return basic;
        }

        /// <summary>
        /// Removes a specified object from the members list.
        /// </summary>
        /// <param name="basic">The object to remove.</param>
        /// <param name="overlay">A flag used to determine if the object being removed is an overlay object.</param>
        /// <returns>The object removed from the members list. Null if the object was not found in the members list.</returns>
        public GenBasic Remove(GenBasic basic, bool overlay = false)
        {
            if (overlay)
            {
                int index = MembersOverlay.IndexOf(basic);

                if (index > -1)
                {
                    MembersOverlay.Remove(basic);
                    _updateMembersOverlay = true;
                }
                else
                    return null;
            }
            else
            {
                int index = Members.IndexOf(basic);

                if (index > -1)
                {
                    Members.Remove(basic);
                    _updateMembers = true;
                }
                else
                    return null;
            }

            return basic;
        }

        /// <summary>
        /// Replaces an existing object in the members list with a new object.
        /// </summary>
        /// <param name="oldMember">The existing object to replace.</param>
        /// <param name="newMember">The new object that will replace the existing object.</param>
        /// <param name="overlay">A flag used to determine if the object being replaced is an overlay object.</param>
        /// <returns>The new object that replaced the existing object. Null if the object was not found in the members list.</returns>
        public GenBasic Replace(GenBasic oldMember, GenBasic newMember, bool overlay = false)
        {
            if (overlay)
            {
                int index = MembersOverlay.IndexOf(oldMember);

                if (index > -1)
                {
                    MembersOverlay[index] = newMember;
                    _updateMembersOverlay = true;
                }
                else
                    return null;
            }
            else
            {
                int index = Members.IndexOf(oldMember);

                if (index > -1)
                {
                    Members[index] = newMember;
                    _updateMembers = true;
                }
                else
                    return null;
            }

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
            if (length == 0)
                length = _activeMembers.Count;

            int index = GenU.Random(startIndex, startIndex + length);

            // Check if the index is outside of the list.
            if (_activeMembers.Count < (index - 1))
                return null;

            return _activeMembers[index];
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
        /// Clears the members list of all current objects.
        /// </summary>
        public void Clear()
        {
            Members.Clear();
            MembersOverlay.Clear();

            if (Quadtree != null)
                Quadtree.Clear();

            _clear = true;
        }
    }
}