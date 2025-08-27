using Dimmer.Settings;
using SiraUtil.Affinity;
using UnityEngine;
using Chroma.Colorizer;
using HarmonyLib;

namespace Dimmer
{
    internal class DimmerHarmonyPatches : IAffinity
    {
        private readonly DimmerConfig _config;
        private SpriteLightWithId _feetMarker = null;

        private DimmerHarmonyPatches(DimmerConfig config)
        {
            _config = config;

            GameObject feet = GameObject.Find("PlayersPlace/Feet");
            if (feet != null)
            {
                _feetMarker = feet.GetComponent<SpriteLightWithId>();
            }
        }

        private void DimColor(ref Color color)
        {
            if (!_config.DimmerEnabled || !Plugin.IsPlayingChart)
            {
                return;
            }

            if (_config.ColorMultiplier != 1.0f)
            {
                color.r *= _config.ColorMultiplier;
                color.g *= _config.ColorMultiplier;
                color.b *= _config.ColorMultiplier;
            }

            if (_config.AlphaMultiplier != 1.0f)
            {
                color.a *= _config.AlphaMultiplier;
            }

            if (_config.LimitColorComponents && (color.maxColorComponent is float maxComponent && maxComponent > _config.MaxColorComponent))
            {
                float multiplier = _config.MaxColorComponent / maxComponent;
                color.r *= multiplier;
                color.g *= multiplier;
                color.b *= multiplier;
            }

            if (_config.LimitBrightness && color.a > _config.MaxBrightness)
            {
                color.a = _config.MaxBrightness;
            }
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(BloomPrePassBackgroundLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(InstancedMaterialLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(LightWithIds.LightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(ColorArrayLightWithIds.ColorArrayLightWithId), "ColorWasSet")]
        private void ColorWasSetNew(ref ILightWithId __instance, ref Color newColor)
        {
            DimColor(ref newColor);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(BeatLine), "ColorWasSet")]
        [AffinityPatch(typeof(BloomPrePassBackgroundColorsGradientElementWithLightId), "ColorWasSet")]
        [AffinityPatch(typeof(BloomPrePassBackgroundColorsGradientTintColorWithLightId), "ColorWasSet")]
        [AffinityPatch(typeof(BloomPrePassBackgroundColorsGradientTintColorWithLightIds), "ColorWasSet")]
        [AffinityPatch(typeof(DirectionalLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(DirectionalLightWithIds), "ColorWasSet")]
        [AffinityPatch(typeof(DirectionalLightWithLightGroupIds), "ColorWasSet")]
        [AffinityPatch(typeof(EnableRendererWithLightId), "ColorWasSet")]
        [AffinityPatch(typeof(MaterialLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(MaterialLightWithIds), "ColorWasSet")]
        [AffinityPatch(typeof(ParticleSystemLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(ParticleSystemLightWithIds), "ColorWasSet")]
        [AffinityPatch(typeof(PointLightWithIds), "ColorWasSet")]
        [AffinityPatch(typeof(RectangleFakeGlowLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(SongTimeSyncedVideoPlayer), "ColorWasSet")]
        [AffinityPatch(typeof(SpawnRotationChevron), "ColorWasSet")]
        [AffinityPatch(typeof(SpriteLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(TubeBloomPrePassLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(UnityLightWithId), "ColorWasSet")]
        private void ColorWasSet(ref ILightWithId __instance, ref Color color)
        {
            // Don't dim the feet
            if (__instance == _feetMarker)
            {
                return;
            }

            DimColor(ref color);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(TubeBloomPrePassLight), nameof(TubeBloomPrePassLight.color), AffinityMethodType.Setter)]
        private void SetColor(ref Color value)
        {
            DimColor(ref value);
        }
        
        [AffinityPrefix]
        [AffinityPatch(typeof(ObstacleColorizer), "Refresh")]
        private void ObstacleColorizerRefresh(ObstacleColorizer __instance)
        {
            if (!_config.OverrideChromaWallAlpha)
                return;

            Traverse traverse = Traverse.Create(__instance as ObjectColorizer).Field("_color");
            Color? color = traverse.GetValue<Color?>();

            if (!color.HasValue)
                return;

            Color dimmedColor = color.Value;
            dimmedColor.a = _config.ChromaWallAlpha;

            traverse.SetValue(dimmedColor);
        }
    }
}
