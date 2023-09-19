﻿using JetBrains.Annotations;
using Verse;

namespace TitularRoyalty.Titles
{
    
    /// <summary>
    /// Data for a type of player title
    /// </summary>
    public class PlayerTitleData : IExposable, ILoadReferenceable
    {
        protected int id;
        
        public string label;
        public string labelFemale;
        
        public string description;

        public PlayerTitleData()
        {
            id = Find.UniqueIDsManager.GetNextIdeoID();
        }

        /// <summary>
        /// Get Previous Title in the Hierarchy
        /// </summary>
        /// <returns>The Title Below this one in the Hierarchy, Null if there is none</returns>
        [CanBeNull]
        public PlayerTitleData GetPrevTitle()
        {
            var titles = GameComponent_PlayerTitlesManager.Current.Titles;
            var curIndex = titles.IndexOf(this);
            
            if (curIndex == -1)
            {
                Log.Error($"Couldn't find title {this} in the list of titles");
                return null;
            }
            if (curIndex == 0)
            {
                Log.Error($"There is no title before {this}");
                return null;
            }

            // for (int i = curIndex - 1; i < titles.Count; i--)
            // {
            //     // eligibility checks probably here
            //     return titles[i];
            // }
            return titles[curIndex - 1];
        }
        
        /// <summary>
        /// Get Next Title in the Hierarchy
        /// </summary>
        /// <returns>The Title Above this one in the Hierarchy, Null if there is none</returns>
        [CanBeNull]
        public PlayerTitleData GetNextTitle()
        {
            var titles = GameComponent_PlayerTitlesManager.Current.Titles;
            var curIndex = titles.IndexOf(this);
            
            if (curIndex == -1)
            {
                Log.Error($"Couldn't find title {this} in the list of titles");
                return null;
            }

            for (int i = curIndex + 1; i < titles.Count;) //i++)
            {
                // eligibility checks probably here
                return titles[i];
            }
            
            Log.Error($"Couldn't find a title after {this}");
            return null;
        }
        
        public void ExposeData()
        {
            Scribe_Values.Look(ref id, "id");
            Scribe_Values.Look(ref label, "label");
            Scribe_Values.Look(ref labelFemale, "labelFemale");
        }
        
        public override string ToString()
        {
            return label ?? GetUniqueLoadID();
        }

        public string GetUniqueLoadID()
        {
            return "titleData_" + id;
        }
    }
}