using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using SemanticVersioning;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine;

namespace BitchlandOpenAllBepInEx
{
    [BepInPlugin("com.wolfitdm.BitchlandOpenAllBepInEx", "BitchlandOpenAllBepInEx Plugin", "1.0.0.0")]
    public class BitchlandOpenAllBepInEx : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private ConfigEntry<bool> configEnableMe;

        public BitchlandOpenAllBepInEx()
        {
        }

        public static Type MyGetType(string originalClassName)
        {
            return Type.GetType(originalClassName + ",Assembly-CSharp");
        }

        private static string pluginKey = "General.Toggles";

        public static bool enableThisMod = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            configEnableMe = Config.Bind(pluginKey,
                                              "EnableThisMod",
                                              true,
                                             "Whether or not you want enable this mod (default true also yes, you want it, and false = no)");


            enableThisMod = configEnableMe.Value;

            Harmony.CreateAndPatchAll(typeof(BitchlandOpenAllBepInEx));

            Logger.LogInfo($"Plugin BitchlandOpenAllBepInEx BepInEx is loaded!");
        }

        [HarmonyPatch(typeof(Int_Door), "Interact")]
        [HarmonyPrefix] // call before the original method is called
        public static bool Int_Door_Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            Int_Door _this = (Int_Door) __instance;
            _this.m_Locked = false;

            return true;
        }

        [HarmonyPatch(typeof(Int_Drive), "Interact")]
        [HarmonyPrefix] // call before the original method is called
        public static bool Int_Drive_Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            Int_Drive _this = (Int_Drive)__instance;
            _this.m_Locked = false;

            return true;
        }

        [HarmonyPatch(typeof(int_MoveableDoor), "Interact")]
        [HarmonyPrefix] // call before the original method is called
        public static bool Int_MoveableDoor_Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            int_MoveableDoor _this = (int_MoveableDoor)__instance;
            _this.m_Locked = false;

            return true;
        }

        [HarmonyPatch(typeof(Int_SexMachine), "Interact")]
        [HarmonyPrefix] // call before the original method is called
        public static bool Int_SexMachine_Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            Int_SexMachine _this = (Int_SexMachine)__instance;
            _this.m_Locked = false;

            return true;
        }

        [HarmonyPatch(typeof(int_SexTubeBike), "Interact")]
        [HarmonyPrefix] // call before the original method is called
        public static bool Int_SexTubeBike_Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            int_SexTubeBike _this = (int_SexTubeBike)__instance;
            _this.m_Locked = false;

            return true;
        }

        [HarmonyPatch(typeof(Int_Storage), "Interact")]
        [HarmonyPrefix] // call before the original method is called
        public static bool Int_Storage_Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            Int_Storage _this = (Int_Storage)__instance;
            _this.m_Locked = false;

            return true;
        }

        [HarmonyPatch(typeof(TeleportDoor), "Interact")]
        [HarmonyPrefix] // call before the original method is called
        public static bool TeleportDoor_Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            TeleportDoor _this = (TeleportDoor)__instance;
            _this.m_Locked = false;

            return true;
        }
    }
}
