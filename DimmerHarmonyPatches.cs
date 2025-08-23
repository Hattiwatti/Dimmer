using Dimmer.Settings;
using SiraUtil.Affinity;
using UnityEngine;

namespace Dimmer
{
    internal class DimmerHarmonyPatches : IAffinity
    {
        private readonly DimmerConfig _config;

        private float _dimmerRange = 1.0f;

        private float _brightnessRange = 1.0f;

        private SpriteLightWithId _feetMarker = null;

        private DimmerHarmonyPatches(DimmerConfig config)
        {
            _config = config;
            _dimmerRange = 1.0f - _config.RangeMin;
            _brightnessRange = _config.RangeMax - _config.RangeMin;

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

            switch (_config.Mode)
            {
                case DimmerMode.Multiplier:
                    if (_config.DimRGBChannel)
                    {
                        color.r *= _config.Multiplier;
                        color.g *= _config.Multiplier;
                        color.b *= _config.Multiplier;
                    }
                    if (_config.DimAlphaChannel)
                    {
                        color.a *= _config.Multiplier;
                    }
                    break;
                case DimmerMode.Range:
                    if (_config.DimRGBChannel && color.maxColorComponent > _config.RangeMin)
                    {
                        float maxComponentMultiplier = (color.maxColorComponent - _config.RangeMin) / _dimmerRange;
                        float dimmedMaxComponent = maxComponentMultiplier * _brightnessRange + _config.RangeMin;
                        float dimmerMultiplier = dimmedMaxComponent / color.maxColorComponent;
                        color.r *= dimmerMultiplier;
                        color.g *= dimmerMultiplier;
                        color.b *= dimmerMultiplier;
                    }
                    if (_config.DimAlphaChannel && color.a > _config.RangeMin)
                    {
                        float alphaMultiplier = (color.a - _config.RangeMin) / _dimmerRange;
                        color.a = alphaMultiplier * _brightnessRange + _config.RangeMin;
                    }
                    break;
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
        [AffinityPatch(typeof(ParametricBoxController), "Refresh")]
        private void Refresh(ref ParametricBoxController __instance)
        {
            //DimColor(ref __instance.color);
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(TubeBloomPrePassLight), nameof(TubeBloomPrePassLight.color), AffinityMethodType.Setter)]
        private void SetColor(ref Color value)
        {
            DimColor(ref value);
        }
    }

}
