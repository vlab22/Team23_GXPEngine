using System;
using System.Drawing;
using GXPEngine.Core;
using MathfExtensions;

namespace GXPEngine
{
    public class HudArrowToObjective : Sprite
    {
        private GameObject _target;
        private bool _hasTarget;

        private GameObject _targetOrigin;
        private bool _hasTargetOrigin;

        public HudArrowToObjective() : base("data/Hud Arrow.png", false, false)
        {
            SetOrigin(width, height * 0.5f);

            color = ColorTools.ColorToUInt(Color.Red);
        }

        void Update()
        {
            if (!_hasTarget || !_hasTargetOrigin || !Enabled) return;

            var direction = _target.Pos - _targetOrigin.Pos;
            var directionNorm = direction.Normalized;

            var angle = Mathf.Atan2(directionNorm.y, directionNorm.x);

            float sin = Mathf.Sin(angle);
            float cos = Mathf.Cos(angle);

            this.rotation = angle.RadToDegree();

            var nextPos = parent.InverseTransformPoint(_targetOrigin.x + cos * game.width * 0.5f,
                _targetOrigin.y + sin * game.height * 0.5f);
            
            this.SetXY(nextPos.x, nextPos.y);

            Console.WriteLine($"{this}: angle: {angle.RadToDegree():0.00} | {cos:0.00} | {sin:0.00} | pos: {_pos} ");

            CanvasDebugger2.Instance.DrawLine(parent.TransformPoint(_targetOrigin.Pos), _target.Pos, Color.Blue);
        }

        public GameObject Target
        {
            get => _target;
            set
            {
                _target = value;
                _hasTarget = value != null;
            }
        }

        public GameObject TargetOrigin
        {
            get => _targetOrigin;
            set
            {
                _targetOrigin = value;
                _hasTargetOrigin = value != null;
            }
        }
    }
}