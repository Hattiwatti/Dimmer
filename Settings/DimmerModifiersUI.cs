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

        private DimmerModifiersUI(GameplaySetup gameplaySetup, DimmerConfig config)
        {
            _gameplaySetup = gameplaySetup;
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

        [UIValue("DimmerMethod")]
        private DimmerMethod Method
        {
            get
            {
                return _config.Method;
            }
            set
            {
                _config.Method = value;
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

        [UIValue("Multiplier")]
        private float Multiplier
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

        [UIValue("RangeMin")]
        private float RangeMin
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
                    this.PropertyChanged(this, new PropertyChangedEventArgs("RangeMax"));
                }
            }
        }

        [UIValue("RangeMax")]
        private float RangeMax
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
                    this.PropertyChanged(this, new PropertyChangedEventArgs("RangeMin"));
                }
            }
        }

        private static Dictionary<DimmerMethod, string> MethodToString = new Dictionary<DimmerMethod, string>
        {
            { DimmerMethod.Multiplier, "Multiplier" },
            { DimmerMethod.Range, "Range" }
        };

        [UIValue("MethodOptions")]
        private List<object> Modes => MethodToString.Keys.Cast<object>().ToList();

        [UIAction("MethodFormatter")]
        private string MethodDisplay(DimmerMethod method)
        {
            return MethodToString[method];
        }
    }
}
