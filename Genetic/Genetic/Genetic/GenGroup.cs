﻿using System.Collections.Generic;

namespace Genetic
{
    public class GenGroup : GenBasic
    {
        /// <summary>
        /// The GenBasic objects that have been added to the group.
        /// </summary>
        public List<GenBasic> members;

        public GenGroup()
        {
            members = new List<GenBasic>();
        }

        /// <summary>
        /// Calls Update on each of the group's members.
        /// Override this method to add additional update logic.
        /// </summary>
        public override void Update()
        {
            foreach (GenBasic member in members)
                member.Update();
        }

        /// <summary>
        /// Calls Draw on each of the group's members.
        /// </summary>
        public override void Draw()
        {
            foreach (GenBasic member in members)
                member.Draw();
        }

        /// <summary>
        /// Adds any GenBasic object to the group's members list.
        /// </summary>
        /// <param name="basic"></param>
        public void Add(GenBasic basic)
        {
            members.Add(basic);
        }
    }
}