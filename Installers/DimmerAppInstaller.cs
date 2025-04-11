using Dimmer.Settings;
using Zenject;

namespace Dimmer.Installers
{
    internal class DimmerAppInstaller : Installer
    {
        private readonly DimmerConfig _config;

        private DimmerAppInstaller(DimmerConfig config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            this.Container.BindInstance<DimmerConfig>(_config);
        }
    }

}
