using Dimmer.Settings;
using Zenject;

namespace Dimmer.Installers
{
    internal class DimmerMenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesTo<DimmerModifiersUI>().AsSingle();
        }
    }
}