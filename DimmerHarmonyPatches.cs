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

        private DimmerHarmonyPatches(DimmerConfig config)
        {
            _config = config;
            _dimmerRange = 1.0f - _config.RangeMin;
            _brightnessRange = _config.RangeMax - _config.RangeMin;
        }

        [AffinityPrefix]
        [AffinityPatch(typeof(BloomPrePassBackgroundLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(InstancedMaterialLightWithId), "ColorWasSet")]
        [AffinityPatch(typeof(LightWithIds.LightWithId), "ColorWasSet")]
        private void DimNewColor(ref Color newColor)
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
    }

}
