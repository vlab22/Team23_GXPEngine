using System;
using System.Drawing;
using GXPEngine.Core;
using MathfExtensions;

namespace GXPEngine
{
    public class FollowCamera : Camera
    {
        public float speed = 15f;
        public float interpVelocity;
        public float MinDistance;
        public float TargetFrontDistance = 300f;
        private GameObject _target;
        public Vector2 Offset;
        private Vector2 _targetPos;

        public Vector2 pos;

        private bool _hasSpeedListener;
        private IHasSpeed _iHasSpeed;

        private bool _followEnabled = true;

        public FollowCamera(int windowX, int windowY, int windowWidth, int windowHeight) : base(windowX, windowY,
            windowWidth, windowHeight)
        {
            _targetPos.x = this.x;
            _targetPos.y = this.y;
        }

        void Update()
        {
            if (_followEnabled && _target != null)
            {
                Vector2 pos = new Vector2(this.x, this.y);
                Vector2 targetP = new Vector2(_target.x, _target.y);

                if (_hasSpeedListener)
                {
                    //Target point is in front of target, set offset
                    float dist = Mathf.Map(_iHasSpeed.Speed, 0, _iHasSpeed.MaxSpeed, 0, TargetFrontDistance);

                    //TODO: make IHasSpeed has a Vector2 _forward
                    float xDir = Mathf.Cos(_target.rotation.DegToRad());
                    float yDir = Mathf.Sin(_target.rotation.DegToRad());

                    targetP = targetP + (new Vector2(xDir, yDir)) * dist;
                }

                Vector2 targetDirection = (targetP - pos);
                float directionMag = targetDirection.Magnitude;

                if (directionMag > 0)
                {
                    interpVelocity = directionMag * speed;

                    _targetPos = pos + (targetDirection.Normalized * interpVelocity * Time.deltaTime * 0.001f);

                    //var nextPos = Vector2.Lerp(pos, targetPos + offset, 0.25f);
                    // float nextX = Easing.Ease(Easing.Equation.CubicEaseOut, 1, pos.x, _targetPos.x, 4);
                    // float nextY = Easing.Ease(Easing.Equation.CubicEaseOut, 1, pos.y, _targetPos.y, 4);

                    float nextX = pos.x + Easing.Ease(Easing.Equation.CubicEaseOut, 1, 0, _targetPos.x - pos.x, 4);
                    float nextY = pos.y + Easing.Ease(Easing.Equation.CubicEaseOut, 1, 0, _targetPos.y - pos.y, 4);


                    this.SetXY(nextX, nextY);
                }
            }

            pos.x = x;
            pos.y = y;
        }

        public GameObject Target
        {
            get => _target;
            set
            {
                this.HasSpeed = value as IHasSpeed;
                _target = value;
            }
        }

        public IHasSpeed HasSpeed
        {
            get => _iHasSpeed;
            set
            {
                _hasSpeedListener = value != null;
                _iHasSpeed = value;
            }
        }

        public bool FollowEnabled
        {
            get => _followEnabled;
            set => _followEnabled = value;
        }
    }
}