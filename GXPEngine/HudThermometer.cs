using System;
using System.Drawing;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class HudThermometer : Sprite
    {
        private Sprite _barForeground0;
        private Sprite _barForeground1;

        private Sprite _barBackGround;

        private float _value;

        private int _barY = 42;

        public HudThermometer() : base("data/Thermometer Bg.png", false, false)
        {
            _barBackGround = new Sprite("data/Thermometer Bar BG.png", false, false);
            AddChild(_barBackGround);

            _barBackGround.color = ColorTools.ColorToUInt(Color.FromArgb(255, 18, 119, 171));

            //x:20 y42
            _barBackGround.SetOrigin(_barBackGround.width * 0.5f, _barBackGround.height);
            _barBackGround.SetXY(_barBackGround.width * 0.5f + 20, _barBackGround.height + 42);
            
            _barForeground0 = new Sprite("data/Thermometer Bar BG.png", false, false);
            AddChild(_barForeground0);

            _barForeground0.color = ColorTools.ColorToUInt(Color.FromArgb(255, 18, 119, 171));

            //x:20 y42
            _barForeground0.SetOrigin(_barForeground0.width * 0.5f, _barForeground0.height * 0.7f);
            _barForeground0.SetXY(_barForeground0.width * 0.5f + 20, _barForeground0.height * 0.7f + 42);

            _barForeground1 = new Sprite("data/Thermometer Bar Foreground.png", false, false);
            AddChild(_barForeground1);

            _barForeground1.color = ColorTools.ColorToUInt(Color.FromArgb(255, 237, 28, 36));

            //x:20 y42
            _barForeground1.SetOrigin(_barForeground1.width * 0.5f, _barForeground1.height * 0.7f);
            _barForeground1.SetXY(_barForeground1.width * 0.5f + 20, _barForeground1.height * 0.7f + 42);
        }

        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0, 1f);

                float fore = Mathf.Map(_value, 0.3f, 1, 0, 1);
                fore = Mathf.Clamp(fore, 0, 1);

                float back = Mathf.Map(_value, 0, 0.3f, 0, 0.3f);
                back = Mathf.Clamp(back, 0, 0.3f);
                
                _barForeground1.SetScaleXY(1, fore);
                _barForeground0.SetScaleXY(1, fore);
                _barBackGround.SetScaleXY(1, back);

            }
        }
    }
}