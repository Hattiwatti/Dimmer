using System;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Attributes;
using Zenject;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        [UIValue("DimmerMode")]
        private DimmerMode Mode
        {
            get
            {
                return _config.Mode;
            }
            set
            {
                _config.Mode = value;
            }
        }

        [UIValue("DimAlphaChannel")]
        private bool DimAlphaChannel
        {
            get
            {
                return _config.DimAlphaChannel;
            }
            set
            {
                _config.DimAlphaChannel = value;
            }
        }

        [UIValue("DimRGBChannel")]
        private bool DimRGBChannel
        {
            get
            {
                return _config.DimRGBChannel;
            }
            set
            {
                _config.DimRGBChannel = value;
            }
        }

        [UIValue("DimmerMultiplier")]
        private float DimmerMultiplier
        {
            get
            {
                return _config.Multiplier;
            }
            set
            {
                _config.Multiplier = value;
            }
        }

        [UIValue("DimmerRangeMin")]
        private float DimmerRangeMin
        {
            get
            {
                return _config.RangeMin;
            }
            set
            {
                _config.RangeMin = value;
                if (_config.RangeMax < value)
                {
                    _config.RangeMax = value;
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DimmerRangeMax"));
                }
            }
        }

        [UIValue("DimmerRangeMax")]
        private float DimmerRangeMax
        {
            get
            {
                return _config.RangeMax;
            }
            set
            {
                _config.RangeMax = value;
                if (_config.RangeMin > value)
                {
                    _config.RangeMin = value;
                    this.PropertyChanged(this, new PropertyChangedEventArgs("DimmerRangeMin"));
                }
            }
        }

        private static Dictionary<DimmerMode, string> ModeToString = new Dictionary<DimmerMode, string>
        {
            { DimmerMode.Multiplier, "Multiplier" },
            { DimmerMode.Range, "Range" }
        };

        [UIValue("ModeOptions")]
        private List<object> Modes => ModeToString.Keys.Cast<object>().ToList();

        [UIAction("ModeFormatter")]
        private string ModeDisplay(DimmerMode method)
        {
            return ModeToString[method];
        }
    }
}
