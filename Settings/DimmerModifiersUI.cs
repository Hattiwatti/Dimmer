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

        [UIValue("DimmerOverlayEnabled")]
        private bool DimmerOverlayEnabled
        {
            get
            {
                return _config.DimmerOverlayEnabled;
            }
            set
            {
                _config.DimmerOverlayEnabled = value;
            }
        }

        [UIValue("OverlayOpacity")]
        private float OverlayOpacity
        {
            get
            {
                return _config.DimmerOverlayOpacity;
            }
            set
            {
                _config.DimmerOverlayOpacity = value;
            }
        }

        [UIValue("LightDimmerEnabled")]
        private bool LightDimmerEnabled
        {
            get
            {
                return _config.LightDimmerEnabled;
            }
            set
            {
                _config.LightDimmerEnabled = value;
            }
        }

        [UIValue("ColorMultiplier")]
        private float ColorMultiplier
        {
            get
            {
                return _config.ColorMultiplier;
            }
            set
            {
                _config.ColorMultiplier = value;
            }
        }

        [UIValue("AlphaMultiplier")]
        private float AlphaMultiplier
        {
            get
            {
                return _config.AlphaMultiplier;
            }
            set
            {
                _config.AlphaMultiplier = value;
            }
        }

        [UIValue("LimitColorComponents")]
        private bool LimitColorComponents
        {
            get
            {
                return _config.LimitColorComponents;
            }
            set
            {
                _config.LimitColorComponents = value;
            }
        }

        [UIValue("MaxColorComponent")]
        private float MaxColorComponent
        {
            get
            {
                return _config.MaxColorComponent;
            }
            set
            {
                _config.MaxColorComponent = value;
            }
        }

        [UIValue("LimitBrightness")]
        private bool LimitBrightness
        {
            get
            {
                return _config.LimitBrightness;
            }
            set
            {
                _config.LimitBrightness = value;
            }
        }

        [UIValue("MaxBrightness")]
        private float MaxBrightness
        {
            get
            {
                return _config.MaxBrightness;
            }
            set
            {
                _config.MaxBrightness = value;
            }
        }

        [UIValue("OverrideChromaWallAlpha")]
        private bool OverrideChromaWallAlpha
        {
            get
            {
                return _config.OverrideChromaWallAlpha;
            }
            set
            {
                _config.OverrideChromaWallAlpha = value;
            }
        }

        [UIValue("ChromaWallAlpha")]
        private float ChromaWallAlpha
        {
            get
            {
                return _config.ChromaWallAlpha;
            }
            set
            {
                _config.ChromaWallAlpha = value;
            }
        }
    }
}
