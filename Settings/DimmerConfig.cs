using System.Security.Policy;

namespace Dimmer.Settings
{

    public class DimmerConfig
    {
        public bool DimmerEnabled { get; set; } = true;
        public float ColorMultiplier { get; set; } = 1.0f;
        public float AlphaMultiplier { get; set; } = 1.0f;
        public bool LimitColorComponents { get; set; } = false;
        public float MaxColorComponent { get; set; } = 1.0f;
        public bool LimitBrightness { get; set; } = false;
        public float MaxBrightness { get; set; } = 1.0f;
        public bool OverrideChromaWallAlpha { get; set; } = false;
        public float ChromaWallAlpha {  get; set; } = 1.0f;
    }
}
