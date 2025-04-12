namespace Dimmer.Settings
{
    public enum DimmerMode
    {
        Multiplier,
        Range,
    }
    public class DimmerConfig
    {
        public bool DimmerEnabled { get; set; } = true;
        public DimmerMode Mode { get; set; } = DimmerMode.Multiplier;
        public bool DimAlphaChannel { get; set; } = true;
        public bool DimRGBChannel { get; set; } = false;
        public float Multiplier { get; set; } = 1.0f;
        public float RangeMin { get; set; } = 0.0f;
        public float RangeMax { get; set; } = 1.0f;
    }
}
