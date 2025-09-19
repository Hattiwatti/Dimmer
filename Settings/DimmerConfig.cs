using System.Security.Policy;

namespace Dimmer.Settings
{

    public class DimmerConfig
    {
        public bool DimmerEnabled { get; set; } = false;
        public float DimmerOpacity { get; set; } = 0.90f;
    }
}
