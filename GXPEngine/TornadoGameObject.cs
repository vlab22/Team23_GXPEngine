using System;
using System.Collections;
using System.Drawing;
using GXPEngine.Core;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngine
{
    public class TornadoGameObject : AnimationSprite, IHasDistanceToTarget
    {
        private int _frameSpeed = 40;
        private int _animationSpeed;
        private int _animationTimer;

        private GameObject _target;
        private bool _hasTarget;

        private IOnUpdateListener[] _onUpdateListeners = new IOnUpdateListener[0];
        private Vector2 _distanceToTarget;

        private Vector2[] _path = new Vector2[0];

        private float _speed = 90;
        private float _pointSpeed = 100;

        private int _throwAngleMin;
        private int _throwAngleMax;
        private float _throwDistance;
        
        private static Bitmap _Bitmap = new Bitmap("data/Tornado Sprite sheet.png");
        
        public TornadoGameObject(float pX, float pY, float pWidth, float pHeight, int pThrowAngleMin = 0, int pThrowAngleMax = 359, float pThrowDistance = 512) : base(_Bitmap, 6, 2)
        {
            _animationSpeed = _frameSpeed * frameCount;

            _throwAngleMin = pThrowAngleMin;
            _throwAngleMax = pThrowAngleMax;
            _throwDistance = pThrowDistance;
            
            SetOrigin(0, height);

            x = pX + pWidth * 0.5f;
            y = pY - pHeight * 0.5f;

            SetOrigin(width * 0.5f, height * 0.5f);
        }

        void Update()
        {
            if (!Enabled || !_hasTarget) return;

            _distanceToTarget = _target.Pos - _pos;

            int frame = Mathf.Round(Mathf.Map(_animationTimer, 0, _animationSpeed - 1, 0,
                frameCount - 1));

            SetFrame(frame);
            _animationTimer += Time.deltaTime;
            _animationTimer %= _animationSpeed;

            for (int i = 0; i < _onUpdateListeners.Length; i++)
            {
                _onUpdateListeners[i].OnUpdate(this, 8);
            }

            if (MyGame.Debug)
            {
                DrawBoundBox();
            }
        }

        private IEnumerator FollowPathRoutine()
        {
            if (_path.Length == 0)
            {
                yield break;
            }

            SetXY(_path[0].x, _path[0].y);

            while (Enabled || !Destroyed)
            {
                for (int i = 0; i < _path.Length; i++)
                {
                    var startPoint = _path[i];
                    int endPointIndex = GeneralTools.GetCircularArrayIndex(i + 1, _path.Length);
                    var endPoint = _path[endPointIndex];

                    var distPoint = endPoint - startPoint;
                    var distPointNorm = distPoint.Normalized;
                    float distPointMag = distPoint.Magnitude;

                    Vector2 nextPos = startPoint;
                    float nextDist;

                    do
                    {
                        nextPos += distPointNorm * _pointSpeed * Time.delta;

                        CanvasDebugger2.Instance.DrawEllipse(nextPos.x, nextPos.y, 100, 100, Color.Cyan);

                        yield return null;

                        var nextTornadoDist = nextPos - _pos;
                        var nextTornadoNorm = nextTornadoDist.Normalized;

                        var nextTornadoPos = nextTornadoNorm * _speed * Time.delta;
                        
                        Translate(nextTornadoPos.x, nextTornadoPos.y);
                        
                        nextDist = (nextPos - startPoint).Magnitude;

                    } while (nextDist < distPointMag);
                }
            }
        }

        public override Vector2[] GetExtents()
        {
            Vector2[] ret = new Vector2[4];
            ret[0] = TransformPoint(_bounds.left, _bounds.top);
            ret[1] = TransformPoint(_bounds.right,_bounds.top);
            ret[2] = TransformPoint(_bounds.right,_bounds.bottom);
            ret[3] = TransformPoint(_bounds.left, _bounds.bottom);
            return ret;
        }
        
        void DrawBoundBox()
        {
            var p0 = this.TransformPoint(_bounds.left, _bounds.top);
            var p1 = this.TransformPoint(_bounds.right,_bounds.top);
            var p2 = this.TransformPoint(_bounds.right,_bounds.bottom);
            var p3 = this.TransformPoint(_bounds.left, _bounds.bottom);

            CanvasDebugger2.Instance.DrawLine(p0.x, p0.y, p1.x, p1.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p1.x, p1.y, p2.x, p2.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p2.x, p2.y, p3.x, p3.y, Color.Blue);
            CanvasDebugger2.Instance.DrawLine(p3.x, p3.y, p0.x, p0.y, Color.Blue);
        }
        
        public IOnUpdateListener[] OnUpdateListeners
        {
            get => _onUpdateListeners;
            set => _onUpdateListeners = value;
        }

        public Vector2 Distance => _distanceToTarget;

        public GameObject gameObject => this;

        public GameObject Target
        {
            get => _target;
            set
            {
                _target = value;
                _hasTarget = value != null;
            }
        }

        public Vector2[] Path
        {
            get => _path;
            set
            {
                _path = value;
                if (value != null && value.Length > 0)
                    CoroutineManager.StartCoroutine(FollowPathRoutine(), this);
            }
        }

        public int ThrowAngleMin => _throwAngleMin;

        public int ThrowAngleMax => _throwAngleMax;

        public float ThrowDistance => _throwDistance;
    }
}