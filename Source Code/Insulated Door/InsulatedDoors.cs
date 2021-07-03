using HarmonyLib;
using static STRINGS.UI;
using Database;
using System.Collections.Generic;

namespace davkas88.InsulatedDoorsMod
{
    public class InsulatedDoorsModPatches

    {

        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Init_Patch
        {
            public static void Prefix()
            {
                DoorHelpers.DoorBuildMenu(InsulatedManualPressureDoorConfig.ID, InsulatedManualPressureDoorConfig.menu, InsulatedManualPressureDoorConfig.pred);
                DoorHelpers.DoorBuildMenu(InsulatedPressureDoorConfig.ID, InsulatedPressureDoorConfig.menu, InsulatedPressureDoorConfig.pred);
                DoorHelpers.DoorBuildMenu(TinyInsulatedManualPressureDoorConfig.ID, TinyInsulatedManualPressureDoorConfig.menu, TinyInsulatedManualPressureDoorConfig.pred);
                DoorHelpers.DoorBuildMenu(TinyInsulatedPressureDoorConfig.ID, TinyInsulatedPressureDoorConfig.menu, TinyInsulatedPressureDoorConfig.pred);

            }
            public static void Postfix()
            {
                DoorHelpers.DoorTechTree(InsulatedManualPressureDoorConfig.ID, InsulatedManualPressureDoorConfig.tech);
                DoorHelpers.DoorTechTree(InsulatedPressureDoorConfig.ID, InsulatedPressureDoorConfig.tech);
                DoorHelpers.DoorTechTree(TinyInsulatedManualPressureDoorConfig.ID, TinyInsulatedManualPressureDoorConfig.tech);
                DoorHelpers.DoorTechTree(TinyInsulatedPressureDoorConfig.ID, TinyInsulatedPressureDoorConfig.tech);
            }
        }
    }

    [HarmonyPatch(typeof(BuildingComplete), "OnSpawn")]
    public class InsulatedDoor_BuildingComplete_OnSpawn
    {
        public static void Postfix(ref BuildingComplete __instance)
        {

            if (string.Compare(__instance.name, "InsulatedManualPressureDoorComplete") == 0)
            {
                __instance.gameObject.AddOrGet<InsulatingDoor>();
            }
            if (string.Compare(__instance.name, "InsulatedPressureDoorComplete") == 0)
            {
                __instance.gameObject.AddOrGet<InsulatingDoor>();
            }
            if (string.Compare(__instance.name, "TinyInsulatedManualPressureDoorComplete") == 0)
            {
                __instance.gameObject.AddOrGet<InsulatingDoor>();
            }
            if (string.Compare(__instance.name, "TinyInsulatedPressureDoorComplete") == 0)
            {
                __instance.gameObject.AddOrGet<InsulatingDoor>();
            }

            InsulatingDoor insulatingDoor = __instance.gameObject.GetComponent<InsulatingDoor>();

            if (insulatingDoor != null)
            {
                insulatingDoor.SetInsulation(__instance.gameObject, insulatingDoor.door.building.Def.ThermalConductivity);
            }
        }
    }

    public class DoorHelpers
    {
        public static void DoorBuildMenu(string door, string menu, string pred)
        {
            var hashedMenuCategory = new HashedString(menu);
            int index = TUNING.BUILDINGS.PLANORDER.FindIndex(x => x.category == hashedMenuCategory);
            if (index < 0)
            {
                return;
            }
            else
            {
                IList<string> data = TUNING.BUILDINGS.PLANORDER[index].data;
                int num = -1;
                foreach (string str in data)
                {
                    if (str.Equals(pred))
                        num = data.IndexOf(str);
                }
                if (num == -1)
                    return;
                else
                    data.Insert(num + 1, door);
            }
        }

        public static void DoorTechTree(string door, string group)
        {
            if (group == "none") return;
#if VANILLA
            Techs.TECH_GROUPING[group] = new List<string>((IEnumerable<string>)Techs.TECH_GROUPING[group])
            {
                door
            }.ToArray();
#endif

#if SPACED_OUT

            var researchTechGroup = Db.Get().Techs?.TryGet(group);
		    if (researchTechGroup != null)
		    {
                researchTechGroup.unlockedItemIDs.Add(door);
		    }
#endif
        }
    }


    public class STRINGS
    {
        public class BUILDINGS
        {
            public class PREFABS
            {
                public class INSULATEDMANUALPRESSUREDOOR
                {
                    public static LocString NAME = FormatAsLink("Insulated Manual Airlock", InsulatedManualPressureDoorConfig.ID);
                    public static LocString DESC = "The lowered thermal conductivity of insulated door slows any heat passing through them.";
                    public static LocString EFFECT = "Mantain temperature between two rooms";
                }
                public class INSULATEDPRESSUREDOOR
                {
                    public static LocString NAME = FormatAsLink("Insulated Mechanized Airlock", InsulatedPressureDoorConfig.ID);
                    public static LocString DESC = "Tiny Mechanized Airlocks fitted for very tiny duplicant.";
                    public static LocString EFFECT = "Mantain temperature between two rooms";
                }
                public class TINYINSULATEDMANUALPRESSUREDOOR
                {
                    public static LocString NAME = FormatAsLink("`Tiny Insulated Manual Airlock", TinyInsulatedManualPressureDoorConfig.ID);
                    public static LocString DESC = "The lowered thermal conductivity of insulated door slows any heat passing through them.";
                    public static LocString EFFECT = "Mantain temperature between two rooms";
                }
                public class TINYINSULATEDPRESSUREDOOR
                {
                    public static LocString NAME = FormatAsLink("Tiny Insulated Mechanized Airlock", TinyInsulatedPressureDoorConfig.ID);
                    public static LocString DESC = "Tiny Mechanized Airlocks fitted for very tiny duplicant.";
                    public static LocString EFFECT = "Mantain temperature between two rooms";
                }
            }
        }
    }
}
