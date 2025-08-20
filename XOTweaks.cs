using MelonLoader;
using System.Collections;
using UnityEngine;
using ScheduleOne.Persistence;
using Newtonsoft.Json;
using HarmonyLib;
using MelonLoader.Utils;
using ScheduleOne.Trash;
using ScheduleOne.Map.Infrastructure;
using ScheduleOne.DevUtilities;
using ScheduleOne.PlayerScripts;
using ScheduleOne.GameTime;
using Funly.SkyStudio;
using StylizedWater2;
using System.Drawing;
using System;
using ScheduleOne.UI;
using System.ComponentModel;
using System.Reflection;

[assembly: MelonInfo(typeof(XOTweaks.XOTweaks), XOTweaks.BuildInfo.Name, XOTweaks.BuildInfo.Version, XOTweaks.BuildInfo.Author, XOTweaks.BuildInfo.DownloadLink)]
[assembly: MelonColor()]
[assembly: MelonOptionalDependencies("FishNet.Runtime")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace XOTweaks
{
    public static class BuildInfo
    {
        public const string Name = "XOTweaks";
        public const string Description = "Performance, Utility, Configuration";
        public const string Author = "XOWithSauce";
        public const string Company = null;
        public const string Version = "1.0";
        public const string DownloadLink = null;
    }

    [System.Serializable]
    public class ModConfig
    {
        public bool sunShadowDisabled = true;
        public float sunLightIntensityMult = 0.3f; // at 1 default 

        public bool moonShadowDisabled = true; // default false
        public float moonLightIntensityMult = 9.5f; // at 1 default

        public bool streetLightShadowDisabled = true; // default false

        public bool optimizedLightApply = true; // Wether to apply the below
        public float optimizedLightDistance = 50f; // default 50-80-100f -> Lower distance culls lights earlier

        public float fogEndDistanceMult = 180f; // default 250.0f

        public int trashClearThreshold = 300; // how often recent trash is cleared

        public float timeProgressMult = 1f; // default 1f -> change config & hotreload if need to speed up or slow down time

        public bool disableBushes = true; // default Bushes object is on

        public bool disableSea = false; // default Sea object is on

        public float flashLightIntensity = 3.5f; // default 2.7, 3.5 is recommended if sunShadowDisabled is false

        public float flashLightRange = 30f; // default 8.0, 30 is recommended if sunShadowDisabled is false

        public float contrastMult = 2f; // default 1f

        public bool hotReloadEnabled = true; // while true, press Left CTRL + R buttons to Hot Reload config changes if supported

        public float camFarClipPlane = 230f; // Default 3000 
    }

    public static class ConfigLoader
    {
        private static string path = Path.Combine(MelonEnvironment.ModsDirectory, "XOTweaks", "config.json");
        public static ModConfig Load()
        {
            ModConfig config;
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    config = JsonConvert.DeserializeObject<ModConfig>(json);
                }
                catch (Exception ex)
                {
                    config = new ModConfig();
                    MelonLogger.Warning("Failed to read XOTweaks config: " + ex);
                }
            }
            else
            {
                config = new ModConfig();
                Save(config);
            }
            return config;
        }

        public static void Save(ModConfig config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("Failed to save FeeningNPCs config: " + ex);
            }
        }
    }
    public class XOTweaks : MelonMod
    {
        static ModConfig currentConfig;
        private bool registered = false;
        private bool firstTimeLoad = false;
        static bool reloading = false;

        static Light sun;
        static LightShadows sunShadowType = LightShadows.None;
        static Light moon;
        static LightShadows moonShadowType = LightShadows.None;

        static GameObject bushesRef = null;

        static TrashManager[] mgrs;
        static object trashCoro = null;

        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
            currentConfig = ConfigLoader.Load();
            MelonLogger.Msg("XOTweaks - by XOWithSauce | Performance, Utility, Configuration");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (buildIndex == 1)
            {
                if (LoadManager.Instance != null && !registered && !firstTimeLoad)
                {
                    firstTimeLoad = true;
                    LoadManager.Instance.onLoadComplete.AddListener((UnityEngine.Events.UnityAction)OnLoadCompleteCb);
                }
            }
            if (buildIndex != 1)
            {
                if (registered)
                {
                    registered = false;
                }
            }
        }

        private void OnLoadCompleteCb()
        {
            if (registered) return;

            currentConfig = ConfigLoader.Load();
            MelonCoroutines.Start(SetupSettings());
            trashCoro = MelonCoroutines.Start(TrashRoutine());
            registered = true;
        }

        public override void OnUpdate()
        {
            if (!registered || currentConfig == null) return;

            if (currentConfig.hotReloadEnabled)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R))
                {
                    if (!reloading)
                    {
                        reloading = true;
                        if (trashCoro != null)
                            MelonCoroutines.Stop(trashCoro);
                        MelonLogger.Msg("Hot Reload XOTweaks config");
                        currentConfig = ConfigLoader.Load();
                        trashCoro = MelonCoroutines.Start(TrashRoutine());
                        MelonCoroutines.Start(SetupSettings());
                    }
                }
            }
        }

        [HarmonyPatch(typeof(TimeOfDayController), "UpdateSkyForCurrentTime")]
        public static class TimeOfDayControllerPatch
        {
            [HarmonyPostfix]
            public static void Postfix(TimeOfDayController __instance)
            {
                // Apply sun light intensity multiplier
                if (__instance.sunOrbit != null && __instance.sunOrbit.BodyLight != null)
                {
                    __instance.sunOrbit.BodyLight.intensity *= XOTweaks.currentConfig.sunLightIntensityMult;
                }

                if (__instance.moonOrbit != null && __instance.moonOrbit.BodyLight != null)
                {
                    __instance.moonOrbit.BodyLight.intensity *= XOTweaks.currentConfig.moonLightIntensityMult;
                }
            }
        }

        public static IEnumerator SetupSettings()
        {
            yield return new WaitForSeconds(5f);

            sun = ScheduleOne.FX.EnvironmentFX.Instance.SunLight;
            if (sunShadowType == LightShadows.None)
                sunShadowType = sun.shadows;

            moon = ScheduleOne.FX.EnvironmentFX.Instance.MoonLight;
            if (moonShadowType == LightShadows.None)
                moonShadowType = moon.shadows;

            if (currentConfig.sunShadowDisabled)
            {
                //MelonLogger.Msg($"Disabling Sun Light Shadow casting");
                sun.shadows = LightShadows.None;
            }
            else
            {
                //MelonLogger.Msg($"Enabling Sun Light Shadow casting");
                sun.shadows = sunShadowType;
            }

            if (currentConfig.moonShadowDisabled)
            {
                //MelonLogger.Msg($"Disabling Moon Light Shadow casting");
                moon.shadows = LightShadows.None;
            }
            else
            {
                //MelonLogger.Msg($"Enabling Moon Light Shadow casting");
                moon.shadows = moonShadowType;
            }

            StreetLight[] slights = UnityEngine.Object.FindObjectsOfType<StreetLight>(true);
            if (currentConfig.streetLightShadowDisabled)
            {
                //MelonLogger.Msg($"Disabling Street Light Shadow casting");
                foreach (StreetLight light in slights)
                    light.ShadowsEnabled = false;
            }
            else
            {
                //MelonLogger.Msg($"Enabling Street Light Shadow casting");
                foreach (StreetLight light in slights)
                    light.ShadowsEnabled = true;
            }


            //MelonLogger.Msg($"Adjusting Optimized Light Range");
            if (currentConfig.optimizedLightApply)
            {
                OptimizedLight[] lights = UnityEngine.Object.FindObjectsOfType<OptimizedLight>(true);
                foreach (OptimizedLight light in lights)
                {
                    light.MaxDistance = currentConfig.optimizedLightDistance;
                }
            }

            //MelonLogger.Msg($"Applying Fog settings");
            //MelonLogger.Msg($"EndDistance previous {ScheduleOne.FX.EnvironmentFX.Instance.fogEndDistanceMultiplier}");
            ScheduleOne.FX.EnvironmentFX.Instance.fogEndDistanceMultiplier = currentConfig.fogEndDistanceMult;

            //MelonLogger.Msg($"Applying Contrast settings, previous {ScheduleOne.FX.EnvironmentFX.Instance.contractMultiplier}");
            ScheduleOne.FX.EnvironmentFX.Instance.contractMultiplier = currentConfig.contrastMult;
            //MelonLogger.Msg($"Contrast after: {ScheduleOne.FX.EnvironmentFX.Instance.contractMultiplier}");

            //MelonLogger.Msg($"Time Progression updated, previous: {NetworkSingleton<TimeManager>.Instance.TimeProgressionMultiplier}");
            NetworkSingleton<TimeManager>.Instance.TimeProgressionMultiplier = currentConfig.timeProgressMult;

            if (currentConfig.disableBushes)
            {
                //MelonLogger.Msg($"Disabling bushes");
                GameObject go = GameObject.Find("Bushes");
                if (go != null && go.transform.parent.name == "Container")
                {
                    go.SetActive(false);
                    if (bushesRef == null)
                        bushesRef = go;
                }
            }
            else
            {
                //MelonLogger.Msg($"Enabling bushes");
                GameObject go = GameObject.Find("Bushes");
                if (go != null && go.transform.parent.name == "Container")
                    go.SetActive(true);
            }

            //MelonLogger.Msg($"Changing Sea render");
            if (currentConfig.disableSea)
                UnityEngine.Object.FindObjectOfType<StylizedWater2.WaterObject>(true).gameObject.SetActive(false);
            else
                UnityEngine.Object.FindObjectOfType<StylizedWater2.WaterObject>(true).gameObject.SetActive(true);

            // Flashlight, traverse from Camera hierarchy -> light object
            //MelonLogger.Msg("Changing flashlight");
            Transform playerCam = PlayerSingleton<PlayerCamera>.Instance.transform.GetChild(0);
            if (playerCam != null)
            {
                Transform flash = playerCam.Find("Flashlight");
                if (flash != null)
                {
                    Transform spot = flash.Find("Spot Light");
                    if (spot != null)
                    {
                        Light flashLight = spot.GetComponent<Light>();
                        flashLight.intensity = currentConfig.flashLightIntensity;
                        flashLight.range = currentConfig.flashLightRange;
                    }
                }
            }

            //MelonLogger.Msg($"Applying Far Clip, previous: {PlayerSingleton<PlayerCamera>.Instance.Camera.farClipPlane}");
            PlayerSingleton<PlayerCamera>.Instance.Camera.farClipPlane = currentConfig.camFarClipPlane;

            reloading = false;
            MelonLogger.Msg("Configuration applied");
            yield return null;
        }

        private IEnumerator TrashRoutine()
        {
            yield return new WaitForSeconds(5f);
            if (!registered) yield break;
            if (currentConfig.trashClearThreshold > 9000) yield break;
            for (; ; )
            {
                yield return new WaitForSeconds(currentConfig.trashClearThreshold);
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1) { continue; }
                //MelonLogger.Msg("Clearing trash...");
                mgrs = UnityEngine.Object.FindObjectsOfType<TrashManager>(true);
                foreach (TrashManager mgr in mgrs)
                {
                    mgrs[0].DestroyAllTrash();
                }
            }
        }

    }
}