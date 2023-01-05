﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using RimWorld;
using UnityEngine;
using Verse;


namespace TitularRoyalty { 

	public static class DebugActionsTitularRoyalty
	{

		[DebugAction("Mods", "TR: Rename Title", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TryChangeCustomTitle()
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (PlayerTitleDef title in Faction.OfPlayer.def.RoyalTitlesAllInSeniorityOrderForReading)
			{
				list.Add(new DebugMenuOption($"{title.GetLabelForBothGenders()}", DebugMenuOptionMode.Action, delegate
				{
					//Current.Game.GetComponent<GameComponent_TitularRoyalty>();
					Find.WindowStack.Add(new Dialog_TitleRenamer(title));
				}
				));

			}
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
		}

		[DebugAction("Mods", "TR: Reset Custom Titles", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TryResetCustomTitles()
		{
			Current.Game.GetComponent<GameComponent_TitularRoyalty>().ResetTitles();
		}

        [DebugAction("Mods", "TR: Refresh Titles", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void UpdateTitles()
		{
			Current.Game.GetComponent<GameComponent_TitularRoyalty>().SetupTitles();
		}

        [DebugAction("Mods", "TR: Try Apply ModSettings", false, false, allowedGameStates = AllowedGameStates.Playing)]
        public static void ReloadSettings()
        {
			ModSettingsApplier.ApplySettings();
        }

        [DebugAction("Mods", "TR: Export Titlelist to Doc", false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void ExportTitlesToDoc()
		{
			string doc = "";
			
			foreach (var title in DefDatabase<PlayerTitleDef>.AllDefsListForReading)
			{
				doc += $"  <li> <!--{title.originalLabels.label.CapitalizeFirst()}-->\n";
				doc += $"    <titleDef>{title.defName}</titleDef>\n";
				doc += $"    \n";
                doc += $"    <label>{title.label}</label>\n";
                doc += $"    <labelFemale>{title.labelFemale ?? "None"}</labelFemale>\n";
                doc += $"  </li>\n\n";
			}

			using (FileStream fs = File.Create($"RealmTypeList{Rand.Int}.xml"))
			{
				byte[] info = new UTF8Encoding(true).GetBytes(doc);
				fs.Write(info, 0, info.Length);
            }

			Log.Message($"Saved to your rimworld folder.");
		}
		
    }

}