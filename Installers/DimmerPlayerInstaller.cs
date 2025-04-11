using Dimmer;
using Zenject;

namespace Dimmer.Installers
{
    internal class DimmerPlayerInstaller : Installer
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesTo<DimmerHarmonyPatches>().AsSingle();
        }
    }

}
