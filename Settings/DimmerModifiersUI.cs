using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Attributes;
using Zenject;
using System.ComponentModel;

namespace Dimmer.Settings
{
    internal class DimmerModifiersUI : IInitializable, IDisposable, INotifyPropertyChanged
    {
        private readonly DimmerConfig _config;
        private readonly GameplaySetup _gameplaySetup;
        public event PropertyChangedEventHandler PropertyChanged;


        private DimmerModifiersUI(
#if !BS1_29_1
            GameplaySetup gameplaySetup,
#endif
            DimmerConfig config)
        {
#if !BS1_29_1
            _gameplaySetup = gameplaySetup;
#else
            _gameplaySetup = GameplaySetup.instance;
#endif
            _config = config;
        }

        public void Initialize()
        {
            _gameplaySetup.AddTab("Dimmer", "Dimmer.Settings.modifiers.bsml", this);
        }

        public void Dispose()
        {
            _gameplaySetup.RemoveTab("Dimmer");
        }

        [UIValue("DimmerEnabled")]
        private bool DimmerEnabled
        {
            get
            {
                return _config.DimmerEnabled;
            }
            set
            {
                _config.DimmerEnabled = value;
            }
        }

        [UIValue("DimmerOpacity")]
        private float DimmerOpacity
        {
            get
            {
                return _config.DimmerOpacity;
            }
            set
            {
                _config.DimmerOpacity = value;
            }
        }

        [UIValue("DimmerCamera2")]
        private bool DimmerCamera2
        {
            get
            {
                return _config.DimmerCamera2;
            }
            set
            {
                _config.DimmerCamera2 = value;
            }
        }
    }
}
