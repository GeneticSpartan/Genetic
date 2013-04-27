using System;
using System.Collections.Generic;

namespace Genetic
{
    public class GenGroup : GenBasic
    {
        /// <summary>
        /// The GenBasic objects that have been added to the group.
        /// </summary>
        public List<GenBasic> Members;

        public GenGroup()
        {
            Members = new List<GenBasic>();
        }

        /// <summary>
        /// Calls Update on each of the objects in the members list.
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            if (Exists && Active)
            {
                foreach (GenBasic member in Members)
                {
                    if (member.Exists && member.Active)
                        member.Update();
                }

                foreach (GenBasic member in Members)
                {
                    if (member.Exists && member.Active)
                        member.PostUpdate();
                }
            }
        }

        /// <summary>
        /// Calls Draw on each of the objects in the members list.
        /// </summary>
        public override void Draw()
        {
            if (Exists && Visible)
            {
                foreach (GenBasic member in Members)
                {
                    if (member.Exists)
                    {
                        if (member.Visible)
                            member.Draw();

                        if (GenG.IsDebug)
                            member.DrawDebug();
                    }
                }
            }
        }

        /// <summary>
        /// Adds an object to the members list.
        /// </summary>
        /// <param name="basic">The object to add.</param>
        /// <returns>The object added to the members list.</returns>
        public GenBasic Add(GenBasic basic)
        {
            // Do not add the same object twice.
            if (Members.Contains(basic))
                return basic;

            Members.Add(basic);

            return basic;
        }

        /// <summary>
        /// Removes a specified object from the members list.
        /// </summary>
        /// <param name="basic">The object to remove.</param>
        /// <returns>The object removed from the members list.</returns>
        public GenBasic Remove(GenBasic basic)
        {
            Members.Remove(basic);

            return basic;
        }

        /// <summary>
        /// Replaces an existing object in the members list with a new object.
        /// </summary>
        /// <param name="oldBasic">The existing object to replace.</param>
        /// <param name="newBasic">The new object that will replace the existing object.</param>
        /// <returns>The new object that replaced the existing object. Null if the object was not found in the members list.</returns>
        public GenBasic Replace(GenBasic oldBasic, GenBasic newBasic)
        {
            int index = Members.IndexOf(oldBasic);

            if (index > -1)
                Members[index] = newBasic;
            else
                return null;

            return newBasic;
        }

        /// <summary>
        /// Gets the first available object in the members list by checking if Exists equals false.
        /// </summary>
        /// <returns>The first available object in the members list. Null if no object is available.</returns>
        public GenBasic GetFirstAvailable()
        {
            foreach (GenBasic basic in Members)
            {
                if (!basic.Exists)
                    return basic;
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
            foreach (GenBasic basic in Members)
            {
                if (basic.Exists)
                    return basic;
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
            foreach (GenBasic basic in Members)
            {
                if (basic.Exists && basic.Active)
                    return basic;
            }

            return null;
        }

        /// <summary>
        /// Gets the first dead object in the members list by checking if either Exists or Active equals false.
        /// </summary>
        /// <returns>The first dead object in the members list. Null if no object is dead.</returns>
        public GenBasic GetFirstDead()
        {
            foreach (GenBasic basic in Members)
            {
                if (!basic.Exists || !basic.Active)
                    return basic;
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
            foreach (GenBasic basic in Members)
            {
                if (basic.Exists && basic.Visible)
                    return basic;
            }

            return null;
        }

        /// <summary>
        /// Gets the first hidden object in the members list by checking if either Exists or Visible equals false.
        /// </summary>
        /// <returns>The first hidden object in the members list. Null if no object is hidden.</returns>
        public GenBasic GetFirstHidden()
        {
            foreach (GenBasic basic in Members)
            {
                if (!basic.Exists || !basic.Visible)
                    return basic;
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
                length = Members.Count;

            int index = GenU.Random(startIndex, startIndex + length);

            // Check if the index is outside of the list.
            if (Members.Count < (index - 1))
                return null;

            return Members[index];
        }

        /// <summary>
        /// Counts the number of alive objects in the members list by checking if Exists and Active both equal true on each.
        /// </summary>
        /// <returns>The number of alive objects in the members list.</returns>
        public int CountAlive()
        {
            int count = 0;

            foreach (GenBasic basic in Members)
            {
                if (basic.Exists && basic.Active)
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

            foreach (GenBasic basic in Members)
            {
                if (!basic.Exists || !basic.Active)
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
            foreach (GenBasic basic in Members)
            {
                basic.Kill();
            }

            base.Kill();
        }

        /// <summary>
        /// Calls Revive on each object in the members list, and then on the group itself.
        /// Revive sets Exists and Active to true to "revive" an object.
        /// </summary>
        public override void Revive()
        {
            foreach (GenBasic basic in Members)
            {
                basic.Revive();
            }

            base.Revive();
        }

        /// <summary>
        /// Clears the members list of all current objects.
        /// </summary>
        public void Clear()
        {
            Members.Clear();
        }
    }
}