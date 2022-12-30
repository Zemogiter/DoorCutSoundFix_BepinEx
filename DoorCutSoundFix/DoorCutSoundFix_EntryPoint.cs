using BepInEx;
using BepInEx.Logging;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Options;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;


namespace DoorCutSoundFix
{
    [BepInPlugin("com.zemogiter.doorcutsoundfix", "DoorCutSoundFix", "2.0.5")]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    public class DoorCutSoundFix_EntryPoint : BaseUnityPlugin
    {
        private static bool _initialized = false;

        private static bool _success = true;

        public static ManualLogSource _logger = null;

        private void Awake()
        {
            _logger = base.Logger;
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            base.Logger.LogInfo("INFO: Initializing Door Cut Sound Fix mod...");
            try
            {
                global::DoorCutSoundFix.DoorCutSoundFix_EntryPoint.Start();
            }
            catch (Exception ex)
            {
                _success = false;
                base.Logger.LogInfo($"ERROR: Exception caught! Message=[{ex.Message}] StackTrace=[{ex.StackTrace}]");
                if (ex.InnerException != null)
                {
                    base.Logger.LogInfo($"ERROR: Inner exception => Message=[{ex.InnerException!.Message}] StackTrace=[{ex.InnerException!.StackTrace}]");
                }
            }
            base.Logger.LogInfo(_success ? "INFO: Door Cut Sound Fix mod initialized successfully." : "ERROR: Door Cut Sound Fix mod initialization failed.");
        }

        private static Harmony HarmonyInstance = null;

        public static TechType CyclopsHatchConnector = TechType.None;

        public static void Start()
        {
            if (InitializeHarmony())
            {
                PatchAll();
            }
        }

        public static bool InitializeHarmony()
        {
            if ((HarmonyInstance = new Harmony("com.zemogiter.doorcutsoundfix")) == null)
            {
                Debug.Log("ERROR: Unable to initialize Harmony!");
                return false;
            }
            return true;
        }

        public static void PatchAll()
        {
            MethodInfo originalCode = typeof(LaserCutObject).GetMethod("LaserCutObject", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo newCode = typeof(LaserCutObject_OnEnable_Patch).GetMethod("Prefix", BindingFlags.Static | BindingFlags.Public);
            HarmonyInstance.Patch(originalCode, new HarmonyMethod(newCode));
        }
    }
}
