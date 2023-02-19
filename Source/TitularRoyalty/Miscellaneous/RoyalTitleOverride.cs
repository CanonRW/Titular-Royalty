﻿using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace TitularRoyalty
{
    //Custom Data Structures
    public class RoyalTitleOverride : IExposable
    {
        public PlayerTitleDef titleDef;

        public string label = "None";
        public string labelFemale = "None";

        public bool inheritable = false;
        public bool useTierOverride = false; // Realmtype only
        public TitleTiers titleTier = TitleTiers.Lowborn;

        public ExpectationDef minExpectation;

        public RoyalTitleOverride() { }

        public RoyalTitleOverride(PlayerTitleDef playerTitle) 
        {
            this.titleDef = playerTitle;

            this.label = playerTitle.label;
            this.labelFemale = playerTitle.labelFemale;
            this.inheritable = playerTitle.canBeInherited;
            this.titleTier = playerTitle.titleTier;
            this.minExpectation = playerTitle.minExpectation ?? ExpectationDefOf.ExtremelyLow;
        }

        public bool HasFemaleTitle()
        {
            if (labelFemale != "None" && labelFemale != null && labelFemale != string.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref titleDef, nameof(titleDef));

            Scribe_Values.Look(ref label, nameof(label), "None");
            Scribe_Values.Look(ref labelFemale, nameof(labelFemale), "None");

            Scribe_Values.Look(ref inheritable, nameof(inheritable), false);
            Scribe_Values.Look(ref titleTier, nameof(titleTier), TitleTiers.Lowborn);
            Scribe_Defs.Look(ref minExpectation, nameof(minExpectation));
        }
    }
}
