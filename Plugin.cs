using Dimmer.Installers;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPALogger = IPA.Logging.Logger;
using SiraUtil.Zenject;
using UnityEngine.SceneManagement;
using Dimmer.Settings;

namespace Dimmer
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.github.Hattiwatti.Dimmer";
        internal static readonly Harmony harmony = new Harmony("com.github.Hattiwatti.Dimmer");
        internal static Plugin Instance { get; private set; }
        /// <summary>
        /// Use to send log messages through BSIPA.
        /// </summary>
        internal static IPALogger Log { get; private set; }
        internal static bool IsPlayingChart { get; private set; } = false;

        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config, Zenjector zenjector)
        {
            Log = logger;
            Instance = this;
            zenjector.Install<DimmerAppInstaller>(Location.App, config.Generated<DimmerConfig>());
            zenjector.Install<DimmerMenuInstaller>(Location.Menu);
            zenjector.Install<DimmerPlayerInstaller>(Location.Player);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            harmony.PatchAll(typeof(Plugin).Assembly);
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            IsPlayingChart = newScene.name == "GameCore";
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            harmony.UnpatchSelf();
        }

    }
}
