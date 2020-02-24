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

        private GameObject _cam;
        
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

            var nextPos = Vector2.zero;

            Vector2 viewPortOrigin = new Vector2(_cam.x - game.width * 0.5f, _cam.y - game.height * 0.5f);
            Vector2 viewPortEnd = new Vector2(_cam.x + game.width * 0.5f, _cam.y + game.height * 0.5f);

            if (_target.x > viewPortOrigin.x && _target.x < viewPortEnd.x && _target.y > viewPortOrigin.y &&
                _target.y < viewPortEnd.y)
            {
                Console.WriteLine($"{this}: in screen");
                return;
            }
            
            float slope = Mathf.Tan(angle);
            
            //Find line equation y = m*x + b
            var b = _targetOrigin.y - slope * _targetOrigin.x;
            
            // if (_target.y < viewPortOrigin.y)
            // {
                nextPos.x = (viewPortOrigin.y - b) / slope;
                nextPos.y = slope * nextPos.x + b;

                nextPos = parent.InverseTransformPoint(nextPos.x, nextPos.y);
            // }
            // else if (_target.y > viewPortEnd.y)
            // {
            //     nextPos.x = (viewPortEnd.y - b) / slope;
            //     nextPos.y = slope * nextPos.x + b;
            //
            //     nextPos = parent.InverseTransformPoint(nextPos.x, nextPos.y);
            // }

            if (nextPos.x > game.width)
            {
                nextPos.y = slope * viewPortEnd.x + b;
                nextPos.x = (nextPos.y - b) / slope;

                nextPos = parent.InverseTransformPoint(nextPos.x, nextPos.y);
            }
            else if (nextPos.x < 0)
            {
                nextPos.y = slope * viewPortOrigin.x + b;
                nextPos.x = (nextPos.y - b) / slope;

                nextPos = parent.InverseTransformPoint(nextPos.x, nextPos.y);
            }

            //nextPos.x = Mathf.Clamp(nextPos.x, 0, game.width);
            //nextPos.y = Mathf.Clamp(nextPos.y, 0, game.height);

            // float sin = Mathf.Sin(angle);
            // float cos = Mathf.Cos(angle);
            //
            // this.rotation = angle.RadToDegree();
            //
            // var nextPos = parent.InverseTransformPoint(_targetOrigin.x + cos * game.width * 0.5f,
            //     _targetOrigin.y + sin * game.height * 0.5f);
            
            this.rotation = angle.RadToDegree();
            this.SetXY(nextPos.x, nextPos.y);

            Console.WriteLine($"{this}: angle: {angle.RadToDegree():0.00} |  pos: {_pos} | cam: {_cam.Pos} | vO: {viewPortOrigin} | vE: {viewPortEnd}");

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

        public GameObject Cam
        {
            get => _cam;
            set => _cam = value;
        }
    }
}