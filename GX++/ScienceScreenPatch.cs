
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
namespace CompSearch
{
	[HarmonyPatch(typeof(PLScientistComputerScreen), "TypeSearchNumber")]
	internal class ScienceScreenPatch
	{
		public static bool Prefix(PLScientistComputerScreen __instance, int num)
		{

			var currentSearch = AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchString");
			var status = (int)AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchStatus").GetValue(__instance);
			var IsActiveTabIndex = (bool)AccessTools.Method(typeof(PLScientistComputerScreen), "IsActiveTabIndex", new Type[] { typeof(int), typeof(bool) }).Invoke(__instance, new object[] { 1, false });
			if (IsActiveTabIndex && (ESearchStatus)status == ESearchStatus.E_START_SEARCH && currentSearch.GetValue(__instance).ToString().Length < 6)
			{
				currentSearch.SetValue(__instance, currentSearch.GetValue(__instance) + num.ToString());
			}
			__instance.PlaySoundEventOnAllClonedScreens("play_ship_generic_internal_computer_ui_keypad");
			return false;
		}
	}


	[HarmonyPatch(typeof(PLScientistComputerScreen), "SetupSearchPanel")]
	internal class SciencebtnPatch
	{
		public static void Postfix(PLScientistComputerScreen __instance)
		{
			
			var UIElements = AccessTools.Field(typeof(PLScientistComputerScreen), "UIElements").GetValue(__instance) as Dictionary<string,UIWidget>;
			var SearchStartPanel = AccessTools.Field(typeof(PLScientistComputerScreen), "SearchStartPanel").GetValue(__instance) as UIWidget;
			var CreateButton = AccessTools.Method(typeof(PLUIScreen), "CreateButton", new Type[] { typeof(string), typeof(string), typeof(Vector3), typeof(Vector2), typeof(Color), typeof(Transform), typeof(UIWidget.Pivot) });
			var GXBtn = UIElements["Search_GX_Btn"];
			if (GXBtn != null)
			{
				UIElements["Search_Itm_Btn"] = CreateButton.Invoke(__instance, new object[] { "Search_Itm_Btn", "ITM", new Vector3(GXBtn.transform.localPosition.x,  -5f, 0f), new Vector2(72f, 72f), Color.white, SearchStartPanel.transform, UIWidget.Pivot.TopLeft }) as UIWidget;
			}
				

			
		}
	}

	[HarmonyPatch(typeof(PLScientistComputerScreen), "OnButtonClick")]
	internal class ScienceClickPatch
	{
		public static void Postfix(PLScientistComputerScreen __instance, UIWidget inButton)
		{
			var CurrentSearchType = AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchType");
			var CurrentSearchStatus = (ESearchStatus)AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchStatus").GetValue(__instance);
			var IsActiveTabIndex = (bool)AccessTools.Method(typeof(PLScientistComputerScreen), "IsActiveTabIndex", new Type[] { typeof(int), typeof(bool) }).Invoke(__instance, new object[] { 1, false });
			if (IsActiveTabIndex && CurrentSearchStatus == ESearchStatus.E_START_SEARCH)
            {
				if (inButton.name == "Search_Itm_Btn")
                {
					CurrentSearchType.SetValue(__instance,4);
					__instance.PlaySoundEventOnAllClonedScreens("play_ship_generic_internal_computer_ui_click");

				}
            }
		}
	}

	[HarmonyPatch(typeof(PLScientistComputerScreen), "Update")]
	internal class ScienceUpdate
	{
		public static void Postfix(PLScientistComputerScreen __instance)
		{
			var CurrentSearchType = (int)AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchType").GetValue(__instance);
			var IsActiveTabIndex = (bool)AccessTools.Method(typeof(PLScientistComputerScreen), "IsActiveTabIndex", new Type[] { typeof(int), typeof(bool) }).Invoke(__instance, new object[] { 1, false });
			var SearchTypeLabel = AccessTools.Field(typeof(PLScientistComputerScreen), "SearchTypeLabel").GetValue(__instance) as UILabel;
			if (IsActiveTabIndex)
            {
				if (SearchTypeLabel != null)
				{
					if (CurrentSearchType == 4)
						SearchTypeLabel.text = "ITM";
				}
			}

			
		}
	}


	[HarmonyPatch(typeof(PLScientistComputerScreen), "SetupResult")]

	internal class SetupResultPatch
    {
		public static bool Prefix(PLScientistComputerScreen __instance)
        {
			var CurrentSearchType = (int)AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchType").GetValue(__instance);
			var SearchResult_SectorPanel = AccessTools.Field(typeof(PLScientistComputerScreen), "SearchResult_SectorPanel").GetValue(__instance) as UIWidget;

			var UIElements = AccessTools.Field(typeof(PLScientistComputerScreen), "UIElements").GetValue(__instance) as Dictionary<string,UIWidget>;

			var SetupGeneralResult = AccessTools.Method(typeof(PLScientistComputerScreen), "SetupGeneralInfoResult");
			var HideAllSearchResultPanels = AccessTools.Method(typeof(PLScientistComputerScreen), "HideAllSearchResultPanels");
			var SetupSectorResult = AccessTools.Method(typeof(PLScientistComputerScreen), "SetupSectorResult");

			UILabel uilabel = (UILabel)UIElements["SearchResult_Name"];
			UILabel uilabel2 = (UILabel)UIElements["SearchResult_Desc"];
			UILabel uilabel3 = (UILabel)UIElements["SearchResult_StatsLeft"];
			UILabel uilabel4 = (UILabel)UIElements["SearchResult_StatsRight"];
			uilabel3.text = "";
			uilabel4.text = "";
			uilabel2.text = PLLocalize.Localize("Not Found", false);
			uilabel.text = PLLocalize.Localize("Not Found", false);
			HideAllSearchResultPanels.Invoke(__instance,null);
			if (CurrentSearchType == 0 || (CurrentSearchType != 3 && CurrentSearchType != 4))
			{
				SearchResult_SectorPanel.enabled = true;
				SetupSectorResult.Invoke(__instance, null);
				return false;
			}
			SetupGeneralResult.Invoke(__instance, null);

			return false;
		}
    }


	[HarmonyPatch(typeof(PLScientistComputerScreen), "SetupGeneralInfoResult")]
	internal class SearchPathc
	{
		struct GenericItem
        {
			public string Name;
			public string Description;
			public string LeftDesc;
			public string RightDesc;
			public string LeftDescExt;
			public string RightDescExt;
			public int ScrapToNextLvl;
			public int TotalScrap;
        }

		public static  int GetMatCostForComp(PLShipComponent shipComp)
		{
			return 2 + Mathf.RoundToInt(Mathf.Pow((float)shipComp.GetScaledMarketPrice(false) / 5500f, 1.1f));
		}

		public static int GetMatCostForItem(PLPawnItem shipComp)
		{
			return 1 + Mathf.RoundToInt(Mathf.Pow((float)shipComp.GetScaledMarketPrice(false) / 1300f, 1.1f));
		}

		public static PLPawnItem CreateItem(EPawnItemType inType, int inSubType, int inLevel)
        {
			PLPawnItem plpawnItem = null;
			switch (inType)
			{
				case EPawnItemType.E_HANDS:
					plpawnItem = new PLPawnItem_Hands();
					break;
				case EPawnItemType.E_PHASEPISTOL:
					plpawnItem = new PLPawnItem_PhasePistol();
					break;
				case EPawnItemType.E_REPAIRGUN:
					plpawnItem = new PLPawnItem_RepairGun();
					break;
				case EPawnItemType.E_FIREGUN:
					plpawnItem = new PLPawnItem_FireGun();
					break;
				case EPawnItemType.E_FOOD:
					if (inSubType == 38)
					{
						inSubType = 43;
					}
					inSubType = Mathf.Clamp(inSubType, 0, (int)EFoodType.PREMIUM_NOODLES);
					plpawnItem = new PLPawnItem_Food((EFoodType)inSubType);
					break;
				case EPawnItemType.E_ARTIFACT:
					inSubType = Mathf.Clamp(inSubType, 0, (int)EArtifactType.OLD_BATTERY);
					plpawnItem = new PLPawnItem_Artifact((EArtifactType)inSubType);
					break;
				case EPawnItemType.E_PIERCING_BEAM_PISTOL:
					plpawnItem = new PLPawnItem_PierceLaserPistol();
					break;
				case EPawnItemType.E_SMUGGLERS_PISTOL:
					plpawnItem = new PLPawnItem_SmugglersPistol();
					break;
				case EPawnItemType.E_BURST_PISTOL:
					plpawnItem = new PLPawnItem_BurstPistol();
					break;
				case EPawnItemType.E_RANGER:
					plpawnItem = new PLPawnItem_Ranger();
					break;
				case EPawnItemType.E_HEAVY_PISTOL:
					plpawnItem = new PLPawnItem_HeavyPistol();
					break;
				case EPawnItemType.E_HAND_SHOTGUN:
					plpawnItem = new PLPawnItem_HandShotgun();
					break;
				case EPawnItemType.E_ARMOR:
					plpawnItem = new PLPawnItem_Armor();
					break;
				case EPawnItemType.E_QUEST_ITEM:
					plpawnItem = new PLPawnItem_QuestItem(inSubType);
					break;
				case EPawnItemType.E_SCANNER:
					plpawnItem = new PLPawnItem_Scanner();
					break;
				case EPawnItemType.E_RESEARCH_MAT:
					inSubType = Mathf.Clamp(inSubType, 0, (int)EResearchMaterialType.STRANGE_ORE);
					plpawnItem = new PLPawnItem_ResearchMaterial((EResearchMaterialType)inSubType);
					break;
				case EPawnItemType.E_KEYCARD:
					plpawnItem = new PLPawnItem_Keycard(inSubType);
					break;
				case EPawnItemType.E_PULSE_GRENADE:
					plpawnItem = new PLPawnItem_PulseGrenade();
					break;
				case EPawnItemType.E_HEAL_GRENADE:
					plpawnItem = new PLPawnItem_HealGrenade();
					break;
				case EPawnItemType.E_MINI_GRENADE:
					plpawnItem = new PLPawnItem_MiniGrenade();
					break;
				case EPawnItemType.E_ANTIFIRE_GRENADE:
					plpawnItem = new PLPawnItem_AntiFireGrenade();
					break;
				case EPawnItemType.E_REPAIR_GRENADE:
					plpawnItem = new PLPawnItem_RepairGrenade();
					break;
				case EPawnItemType.E_FB_SELL_TOOL:
					plpawnItem = new PLPawnItem_FBSellTool();
					break;
				case EPawnItemType.E_HELD_BEAM_PISTOL:
					plpawnItem = new PLPawnItem_HeldBeamPistol();
					break;
				case EPawnItemType.E_HELD_BEAM_PISTOL_W_HEALING:
					plpawnItem = new PLPawnItem_HeldBeamPistol_WithHealing();
					break;
				case EPawnItemType.E_STUN_GRENADE:
					plpawnItem = new PLPawnItem_StunGrenade();
					break;
				case EPawnItemType.E_VORTEX_GRENADE:
					plpawnItem = new PLPawnItem_VortexGrenade();
					break;
				case EPawnItemType.E_ICE_SPIKES:
					plpawnItem = new PLPawnItem_IceSpikes(0);
					break;
				case EPawnItemType.E_WD_HEAVY:
					plpawnItem = new PLPawnItem_WDHeavy();
					break;
				case EPawnItemType.E_AMMO_CLIP:
					plpawnItem = new PLPawnItem_AmmoClip();
					break;
				case EPawnItemType.E_BATTERY:
					plpawnItem = new PLPawnItem_Battery();
					break;
				case EPawnItemType.E_SYRINGE:
					inSubType = Mathf.Clamp(inSubType, 0, (int)ESyringeType.HEAL_HEALTH_BONUS);
					plpawnItem = new PLPawnItem_Syringe((ESyringeType)inSubType);
					break;
			}
			if (plpawnItem != null)
			{
				plpawnItem.PawnItemType = inType;
				plpawnItem.SubType = inSubType;
				plpawnItem.Level = inLevel;
			}
			return plpawnItem;
		}
		public static  PLShipComponent CreateComponentFromHash(int type,int subtype, int level)
        {
			switch (type)
			{
				case 1:
					subtype = Mathf.Clamp(subtype, 0, (int)EShieldGeneratorType.E_POLYTECH_SHIELDS);
					return new PLShieldGenerator((EShieldGeneratorType)subtype,level);
				case 2:
					subtype = Mathf.Clamp(subtype, 0, (int)EWarpDriveType.SUB_WARPDRIVE);
					return new PLWarpDrive((EWarpDriveType)subtype, level);
				case 3:
					subtype = Mathf.Clamp(subtype, 0, (int)EReactorType.E_MAX);
					return new PLReactor((EReactorType)subtype, level);

				case 4:
					return new PLTeleporter("Teleporter", "Standard teleporter, installed on all ships", 0f, 0f);

				case 5:
					return new PLSensor_EM(level);
				case 6:
					subtype = Mathf.Clamp(subtype, 0, (int)EHullType.SUB_HULL);
					return new PLHull((EHullType)subtype, level);
				case 7:
					subtype = Mathf.Clamp(subtype, 0, (int)ECPUClass.E_MAX);
					return new PLCPU((ECPUClass)subtype, level);
				case 8:
					subtype = Mathf.Clamp(subtype, 0, (int)EO2GeneratorType.E_MAX);
					return new PLOxygenGenerator((EO2GeneratorType)subtype, level);
				case 9:
					subtype = Mathf.Clamp(subtype, 0, (int)EThrusterType.PROP);
					return new PLThruster((EThrusterType)subtype, level);
				case 10:
					subtype = Mathf.Clamp(subtype, 0, (int)ETurretType.SYLVASSI);
					return PLTurret.CreateTurretFromHash(subtype, level, 0);
				case 11:
					subtype = Mathf.Clamp(subtype, 0, 7);
					return PLMegaTurret.CreateMainTurretFromHash(subtype, level, 0);
				case 16:
					subtype = Mathf.Clamp(subtype, 0, (int)EHullPlatingType.MAX);
					return new PLHullPlating((EHullPlatingType)subtype, level);
				case 17:
					subtype = Mathf.Clamp(subtype, 0, (int)EWarpDriveProgramType.DRAIN);
					return new PLWarpDriveProgram((EWarpDriveProgramType)subtype, level);
				case 19:
					subtype = Mathf.Clamp(subtype, 0, (int)ENuclearDeviceType.WD_TACTICAL);
					return new PLNuclearDevice((ENuclearDeviceType)subtype, level);
				case 20:
					subtype = Mathf.Clamp(subtype, 0, (int)ETrackerMissileType.TORPEDO);
					return new PLTrackerMissile((ETrackerMissileType)subtype, level);
				case 21:
					return new PLScrapCargo(level);
				case 24:
					return new PLAutoTurret(level);
				case 25:
					subtype = Mathf.Clamp(subtype, 0, (int)EInertiaThrusterType.E_GIMBAL);
					return new PLInertiaThruster((EInertiaThrusterType)subtype, level);
				case 26:
					subtype = Mathf.Clamp(subtype, 0, (int)EManeuverThrusterType.E_MAX);
					return new PLManeuverThruster((EManeuverThrusterType)subtype, level);
				case 27:
					subtype = Mathf.Clamp(subtype, 0, (int)ECaptainsChairType.E_MAX);
					return new PLCaptainsChair((ECaptainsChairType)subtype, level);
				case 28:
					subtype = Mathf.Clamp(subtype, 0, (int)EExtractorType.E_PT_EXTRACTOR);
					return new PLExtractor((EExtractorType)subtype, level);

				case 32:
					subtype = Mathf.Clamp(subtype, 0, (int)ESensorDishType.E_NORMAL);
					return new PLSensorDish((ESensorDishType)subtype, level);

				case 33:
					subtype = Mathf.Clamp(subtype, 0, (int)ECloakingSystemType.E_MAX);
					return new PLCloakingSystem((ECloakingSystemType)subtype, level);
			}
			return null;
		}
		public static bool Prefix(PLScientistComputerScreen __instance)
		{
			var CurrentSearchType = (int)AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchType").GetValue(__instance);
			var CurrentSearchString = AccessTools.Field(typeof(PLScientistComputerScreen), "CurrentSearchString").GetValue(__instance) as string;
			var UIElements = AccessTools.Field(typeof(PLScientistComputerScreen), "UIElements").GetValue(__instance) as Dictionary<string, UIWidget>;
			int num = -1;
			if (int.TryParse(CurrentSearchString, out num) && PLGlobal.Instance.AllGeneralInfos != null && num >= 0)
			{
				GeneralInfo generalInfo = new GeneralInfo();
				generalInfo.Name = "Not Found";
				generalInfo.Desc = "Not Found";
				if (num < PLGlobal.Instance.AllGeneralInfos.Count && CurrentSearchString.Length != 6 && CurrentSearchType == 3)
				{
					generalInfo = PLGlobal.Instance.AllGeneralInfos[num];
				}
                else
                {
					if (CurrentSearchString.Length == 6)
                    {
						int Type = 0;
						int subtype = 0;
						int level = 0;
						var array = CurrentSearchString;
						Type = int.Parse(new string(array.Take(2).ToArray()));
						subtype = int.Parse(new string(array.Skip(2).Take(2).ToArray()));
						level = int.Parse(new string(array.Skip(4).Take(2).ToArray()));

						PLWare searchedItem = null;



						var genericItem = new GenericItem();

						


						if (CurrentSearchType == 4)
						{
							var t = Mathf.Clamp(Type, 0, (int)EPawnItemType.E_SYRINGE);
							searchedItem = CreateItem((EPawnItemType)t, subtype, level);
						}
						else
							searchedItem = CreateComponentFromHash(Type, subtype, level);

						if (searchedItem != null)
                        {
							if (searchedItem is PLShipComponent)
							{
								PLShipComponent shipComp = (PLShipComponent)searchedItem;

								genericItem.Name = shipComp.Name;
								genericItem.Description = shipComp.Desc;
								genericItem.LeftDesc = shipComp.GetStatLineLeft();
								genericItem.RightDesc = shipComp.GetStatLineRight();
								genericItem.LeftDescExt = shipComp.GetExtraLineLeft();

								genericItem.RightDescExt = shipComp.GetExtraLineRight();
								
								genericItem.ScrapToNextLvl = GetMatCostForComp(shipComp);
		
								for (int i = 0; i < level; i++)
								{
									shipComp.Level = i;
									genericItem.TotalScrap += GetMatCostForComp(shipComp);
								}
								shipComp.Level = level + 1;


							}
							else
							{
								PLPawnItem pawnItem = (PLPawnItem)searchedItem;
								genericItem.Name = pawnItem.GetItemName();
								genericItem.Description = pawnItem.Desc;
								genericItem.LeftDesc = pawnItem.GetStatLineLeft();
								genericItem.RightDesc = pawnItem.GetStatLineRight();

								genericItem.LeftDescExt = pawnItem.GetExtraLineLeft();
								genericItem.RightDescExt = pawnItem.GetExtraLineRight();

								genericItem.ScrapToNextLvl = GetMatCostForItem(pawnItem);
			
								for (int i = 0; i < level; i++)
								{
									pawnItem.Level = i;
									genericItem.TotalScrap += GetMatCostForItem(pawnItem);
								}
								pawnItem.Level = level + 1;
							}



							GeneralInfoStat statLeft = new GeneralInfoStat();
							GeneralInfoStat statRight = new GeneralInfoStat();
							var GXIndex = genericItem.Description.LastIndexOf("\n");
							if (GXIndex != -1)
							{
								generalInfo.Desc = genericItem.Description.Remove(GXIndex);
							}
							generalInfo.Desc += "\nNext Level: " + genericItem.ScrapToNextLvl.ToString() + " Scrap. Total Cost: " + genericItem.TotalScrap.ToString() + " Scrap.";

							generalInfo.Name = genericItem.Name;
							statLeft.Left = $"{genericItem.LeftDesc}";
							statLeft.Right = $"{genericItem.RightDesc}";

							statRight.Left = $"{genericItem.LeftDescExt}";
							statRight.Right = $"{genericItem.RightDescExt}";
							generalInfo.MyStats.Add(statLeft);
							generalInfo.MyStats.Add(statRight);
						}

					}
					
                }



				if (generalInfo == null)
				{
					generalInfo = new GeneralInfo();
					generalInfo.Name = "Not Found";
					generalInfo.Desc = "Not Found";
				}
				string LeftText = "";
				string RightText = "";
				UILabel NameLabel = (UILabel)UIElements["SearchResult_Name"];
				NameLabel.fontSize = 90;
				NameLabel.spacingY = 5;
				UILabel DescLabel = (UILabel)UIElements["SearchResult_Desc"];
				UILabel StatLeftLabel = (UILabel)UIElements["SearchResult_StatsLeft"];
				UILabel StatRightLabel = (UILabel)UIElements["SearchResult_StatsRight"];
				((UISprite)UIElements["SearchResult_FindSector"]).gameObject.SetActive(false);
				foreach (GeneralInfoStat generalInfoStat in generalInfo.MyStats)
				{
					LeftText += PLLocalize.Localize(generalInfoStat.Left, false) + "\n";
					RightText += PLLocalize.Localize(generalInfoStat.Right, false) + "\n";
				}
				string desc = generalInfo.Desc;
				StatLeftLabel.spacingY = 10;
				StatRightLabel.spacingY = 10;
				StatLeftLabel.text = LeftText;
				StatRightLabel.text = RightText;
				DescLabel.text = PLLocalize.Localize(desc, false);
				DescLabel.spacingY = 3;
				NameLabel.text = PLLocalize.Localize(generalInfo.Name, false);
			}
			return false;

		}
	}
}