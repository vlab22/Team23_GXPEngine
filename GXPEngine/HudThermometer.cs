using System;
using System.Drawing;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class HudThermometer : AnimationSprite
    {
        private float _value;

        public HudThermometer() : base("data/Thermometer Spritesheet.png", 25, 4)
        {
            
        }

        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0, 1f);

                int frame = Mathf.Round(Mathf.Map(_value, 1f, 0f, 0, _cols * 4));
                
                SetFrame(frame);
            }
        }
    }
}