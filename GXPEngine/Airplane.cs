using System;
using System.Collections;
using System.Drawing;
using System.Net;
using GXPEngine.Core;
using MathfExtensions;

namespace GXPEngine
{
    public class Airplane : AnimationSprite
    {
        private float _speed;
        private int _lifeTime;

        private CompoundCollider[] _compoundColliders;

        public Airplane() : base(
            "data/Airplane6.png", 1, 1, -1, false, false)
        {
            _compoundColliders = new CompoundCollider[4];

            _compoundColliders[0] = new CompoundCollider(-171, -35, 411, 64);
            _compoundColliders[1] = new CompoundCollider(-239, -85, 68, 166);
            _compoundColliders[2] = new CompoundCollider(-58, -250, 100, 213);
            _compoundColliders[3] = new CompoundCollider(-58, 30, 100, 213);

            for (int i = 0; i < _compoundColliders.Length; i++)
            {
                _compoundColliders[i].name = $"{this.name}_CompoundCollider_{i}";
                AddChild(_compoundColliders[i]);
            }
        }

        public void LoadStartupData(float pX, float pY, float pWidth, float pHeight, Level pLevel, float pSpeed = 200,
            float pRotation = 0, int pLifeTime = 12000)
        {
            _speed = pSpeed;
            _lifeTime = pLifeTime;

            float originalWidth = width;
            float originalHeight = height;

            SetOrigin(0, height);

            float lScaleX = pWidth / width;
            float lScaleY = pHeight / height;

            SetScaleXY(lScaleX, lScaleY);

            x = pX; // + width / 2;
            y = pY; // - height + height / 2f;

            float diag = Mathf.Sqrt(width * width + height * height);

            SetOrigin(originalWidth / 2f, originalHeight / 2f);

            Turn(pRotation);

            x = pX + Mathf.Cos(rotation.DegToRad()) * width * 0.5f;
            y = pY + Mathf.Sin(rotation.DegToRad()) * width * 0.5f;

            var pos = new Vector2(x - pX, y - pY);
            var perp = new Vector2(pos.y, -pos.x).Normalized;

            pos = perp * height * 0.5f;

            x += pos.x;
            y += pos.y;
        }

        void Update()
        {
            if (!this.Enabled) return;

            float delta = Time.deltaTime * 0.001f;

            Move(_speed * delta, 0);
        }
    }
}