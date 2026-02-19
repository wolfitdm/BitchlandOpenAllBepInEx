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
        private ConfigEntry<bool> configUseNewLockUpdate;
        private ConfigEntry<bool> configDefaultLockState;
        private ConfigEntry<bool> configDefaultPlayerOwnedState;
        private ConfigEntry<bool> configUsePlayerOwnedState;
        private ConfigEntry<bool> configUseReplaceIconText;

        public BitchlandOpenAllBepInEx()
        {
        }

        public static Type MyGetType(string originalClassName)
        {
            return Type.GetType(originalClassName + ",Assembly-CSharp");
        }

        private static string pluginKey = "General.Toggles";

        public static bool enableThisMod = false;
        public static bool useNewLockUpdate = false;
        public static bool defaultLockState = false;
        public static bool defaultPlayerOwnedState = false;
        public static bool usePlayerOwnedState = false;
        public static bool useReplaceIconText = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            configEnableMe = Config.Bind(pluginKey,
                                              "EnableThisMod",
                                              true,
                                             "Whether or not you want enable this mod (default true also yes, you want it, and false = no)");

            configUseNewLockUpdate = Config.Bind(pluginKey,
                                             "UseNewLockUpdate",
                                             true,
                                            "Whether or not you want use the new lock update (works on all future builds) (default true also yes, you want it, and false = no)");

            configDefaultLockState = Config.Bind(pluginKey,
                                 "DefaultLockState",
                                 false,
                                "DefaultLockState = false -> all doors/lockables are opened, true -> all doors/lockables are closed");

            configUsePlayerOwnedState = Config.Bind(pluginKey,
                    "UsePlayerOwnedState",
                    true,
                    "Whether or not you want use player owned state (default true also yes, you want it and false = no)");


            configDefaultPlayerOwnedState = Config.Bind(pluginKey,
                     "DefaultPlayerOwnedState",
                     true,
                    "DefaultPlayerOwnedState = true -> all doors/lockables owned you, false -> all doors/lockables not owned you");


            configUseReplaceIconText = Config.Bind(pluginKey,
                     "UseReplaceIconText",
                     false,
                    "true -> Replace interactText (Locked -> (Unlocked and replace (Unlocked -> (Locked if you open/close lockables (in the future you can set this to false)");


            enableThisMod = configEnableMe.Value;
            useNewLockUpdate = configUseNewLockUpdate.Value;
            defaultLockState = configDefaultLockState.Value;
            defaultPlayerOwnedState = configDefaultPlayerOwnedState.Value;
            usePlayerOwnedState = configUsePlayerOwnedState.Value;
            useReplaceIconText = configUseReplaceIconText.Value;

            PatchHarmonyMethods();

            Logger.LogInfo($"Plugin BitchlandOpenAllBepInEx BepInEx is loaded!");
        }

        public static void PatchHarmonyMethods()
        {
            if (!enableThisMod)
            {
                return;
            }

            if (useNewLockUpdate || !useReplaceIconText)
            {
                PatchHarmonyMethodUnity(typeof(int_Lockable), "Start", "Int_Lockable_Start", false, true);
            }
            else
            {
                PatchHarmonyMethodUnity(typeof(int_Lockable), "OnLocked", "Int_Lockable_OnLocked", false, true);
                PatchHarmonyMethodUnity(typeof(int_Lockable), "OnUnlocked", "Int_Lockable_OnUnlocked", false, true);
                PatchHarmonyMethodUnity(typeof(Interactible), "Interact", "int_Lockable_Interact", true, false);
                PatchHarmonyMethodUnity(typeof(Int_Door), "Interact", "Int_Door_Interact", true, false);
                PatchHarmonyMethodUnity(typeof(Int_Drive), "Interact", "Int_Drive_Interact", true, false);
                PatchHarmonyMethodUnity(typeof(int_MoveableDoor), "Interact", "Int_MoveableDoor_Interact", true, false);
                PatchHarmonyMethodUnity(typeof(Int_SexMachine), "Interact", "Int_SexMachine_Interact", false, true);
                PatchHarmonyMethodUnity(typeof(int_SexTubeBike), "Interact", "Int_SexTubeBike_Interact", true, false);
                PatchHarmonyMethodUnity(typeof(Int_Storage), "Interact", "Int_Storage_Interact", true, false);
                PatchHarmonyMethodUnity(typeof(TeleportDoor), "Interact", "TeleportDoor_Interact", true, false);
            }
        }
        public static void PatchHarmonyMethodUnity(Type originalClass, string originalMethodName, string patchedMethodName, bool usePrefix, bool usePostfix, Type[] parameters = null)
        {
            string uniqueId = "com.wolfitdm.BitchlandOpenAllBepInEx";
            Type uniqueType = typeof(BitchlandOpenAllBepInEx);

            // Create a new Harmony instance with a unique ID
            var harmony = new Harmony(uniqueId);

            if (originalClass == null)
            {
                Logger.LogInfo($"GetType originalClass == null");
                return;
            }

            MethodInfo patched = null;

            try
            {
                patched = AccessTools.Method(uniqueType, patchedMethodName);
            }
            catch (Exception ex)
            {
                patched = null;
            }

            if (patched == null)
            {
                Logger.LogInfo($"AccessTool.Method patched {patchedMethodName} == null");
                return;

            }

            // Or apply patches manually
            MethodInfo original = null;

            try
            {
                if (parameters == null)
                {
                    original = AccessTools.Method(originalClass, originalMethodName);
                }
                else
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
            }
            catch (AmbiguousMatchException ex)
            {
                Type[] nullParameters = new Type[] { };
                try
                {
                    if (patched == null)
                    {
                        parameters = nullParameters;
                    }

                    ParameterInfo[] parameterInfos = patched.GetParameters();

                    if (parameterInfos == null || parameterInfos.Length == 0)
                    {
                        parameters = nullParameters;
                    }

                    List<Type> parametersN = new List<Type>();

                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        ParameterInfo parameterInfo = parameterInfos[i];

                        if (parameterInfo == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name.StartsWith("__"))
                        {
                            continue;
                        }

                        Type type = parameterInfos[i].ParameterType;

                        if (type == null)
                        {
                            continue;
                        }

                        parametersN.Add(type);
                    }

                    parameters = parametersN.ToArray();
                }
                catch (Exception ex2)
                {
                    parameters = nullParameters;
                }

                try
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
                catch (Exception ex2)
                {
                    original = null;
                }
            }
            catch (Exception ex)
            {
                original = null;
            }

            if (original == null)
            {
                Logger.LogInfo($"AccessTool.Method original {originalMethodName} == null");
                return;
            }

            HarmonyMethod patchedMethod = new HarmonyMethod(patched);
            var prefixMethod = usePrefix ? patchedMethod : null;
            var postfixMethod = usePostfix ? patchedMethod : null;

            harmony.Patch(original,
                prefix: prefixMethod,
                postfix: postfixMethod);
        }

        public static void ReplaceIconText(int_Lockable i, bool state)
        {
            if (state == false)
            {
                if (i.InteractText.Contains("(Locked"))
                {
                    i.InteractText = i.InteractText.Replace("(Locked", "(Unlocked");
                    i.InteractIcon = i.DefaultInteractIcon;
                }

                if (i.InteractIcon == 1)
                {
                    i.InteractIcon = i.DefaultInteractIcon;
                }
            }
            else
            {
                if (i.InteractText.Contains("(Unlocked"))
                {
                    i.InteractText = i.InteractText.Replace("(Unlocked", "(Locked");
                    i.InteractIcon = 1;
                }

                if (i.InteractIcon == i.DefaultInteractIcon)
                {
                    i.InteractIcon = 1;
                }
            }
        }

        public static void setPlayerOwned(int_Lockable i, bool state)
        {
            if (!usePlayerOwnedState)
            {
                return;
            }

            try
            {
                i.PlayerOwned = state;
            } catch (Exception e) {
            }
        }
        public static void Int_Lockable_Start(object __instance)
        {
            if (!enableThisMod)
            {
                return;
            }

            int_Lockable _this = (int_Lockable)__instance;

            _this.Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return;
        }

        public static void Int_Lockable_OnLocked(object __instance)
        {
            int_Lockable _this = (int_Lockable)__instance;
            ReplaceIconText(_this, defaultLockState);

            return;
        }
        public static void Int_Lockable_OnUnlocked(object __instance)
        {
            int_Lockable _this = (int_Lockable)__instance;
            ReplaceIconText(_this, defaultLockState);

            return;
        }

        public static bool int_Lockable_Interact(Person person, object __instance)
        {
            if (!(__instance is int_Lockable))
            {
                return true;
            }

            int_Lockable _this = (int_Lockable)__instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }
        public static bool Int_Door_Interact(Person person, object __instance)
        {
            Int_Door _this = (Int_Door) __instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }
        public static bool Int_Drive_Interact(Person person, object __instance)
        {
            Int_Drive _this = (Int_Drive)__instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }

        public static bool Int_MoveableDoor_Interact(Person person, object __instance)
        {
            int_MoveableDoor _this = (int_MoveableDoor)__instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }

        public static bool Int_SexMachine_Interact(Person person, object __instance)
        {
            Int_SexMachine _this = (Int_SexMachine)__instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }

        public static bool Int_SexTubeBike_Interact(Person person, object __instance)
        {
            int_SexTubeBike _this = (int_SexTubeBike)__instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }

        public static bool Int_Storage_Interact(Person person, object __instance)
        {
            Int_Storage _this = (Int_Storage)__instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }

        public static bool TeleportDoor_Interact(Person person, object __instance)
        {
            TeleportDoor _this = (TeleportDoor)__instance;
            _this.m_Locked = defaultLockState;
            setPlayerOwned(_this, defaultPlayerOwnedState);

            return true;
        }
    }
}
