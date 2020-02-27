using System;
using System.Collections;
using System.Collections.Generic;

namespace GXPEngine
{
    public class EnemiesSoundManager : GameObject, IOnUpdateListener
    {
        private SoundManager _soundManager;
        private FollowCamera _cam;

        private Dictionary<uint, float> _soundMaxVolumeMap;
        private Dictionary<uint, float> _soundMaxDistanceMap;

        private Dictionary<GameObject, SoundChannel[]>
            _fxChannelEnemyMap = new Dictionary<GameObject, SoundChannel[]>();

        private Dictionary<uint, IHasDistanceToTarget> _fxChannelDistanceMap =
            new Dictionary<uint, IHasDistanceToTarget>();

        private readonly Dictionary<Type, int> _channelsMaxQtdPerType = new Dictionary<Type, int>()
        {
            {typeof(Airplane), 5}
        };

        private Dictionary<Type, int> _channelsCounterPerType = new Dictionary<Type, int>()
        {
            {typeof(Airplane), 0}
        };

        public EnemiesSoundManager() : base(false)
        {
            _soundManager = SoundManager.Instance;

            _soundMaxVolumeMap = new Dictionary<uint, float>()
            {
                {0, 0.2f}, //Airplane
                {1, 0.05f}, //Drone
            };

            _soundMaxDistanceMap = new Dictionary<uint, float>()
            {
                {0, 1000f},
                {1, 500f},
            };

            CoroutineManager.StartCoroutine(Start(), this);
        }

        private IEnumerator Start()
        {
            do
            {
                _cam = MyGame.ThisInstance.Camera;
                yield return null;
            } while (_cam == null);
        }

        public void CreateChannel(GameObject go)
        {
            // var goType = go.GetType();
            //
            // if (go is Airplane)
            // {
            //     _soundManager.CreateFxChannel(typeof(Airplane), new uint[] {0, 1, 2, 3});
            //     _channelsCounterPerType[goType] += 1;
            // }
        }

        private void UpdateVolumesRelativeToDistanceToTarget(IHasDistanceToTarget iHasDistance, uint soundId)
        {
            var dist = iHasDistance.Distance.Magnitude;
            float currentDistance = float.MaxValue;

            if (_fxChannelDistanceMap.TryGetValue(soundId, out var currentHasDistance))
            {
                currentDistance = currentHasDistance.Distance.Magnitude;
            }

            if (dist <= currentDistance)
            {
                _fxChannelDistanceMap[soundId] = iHasDistance;
                currentDistance = dist;
            }
            else
            {
                return;
            }

            float vol = 0;

            if (currentDistance > _soundMaxDistanceMap[soundId])
            {
                _soundManager.SetFxVolume(soundId, 0);
            }
            else
            {
                vol = Mathf.Map(dist, _soundMaxDistanceMap[soundId], 0, 0f, _soundMaxVolumeMap[soundId]);

                _soundManager.SetFxVolume(soundId, vol);
            }

            // Console.WriteLine(
            //     $"{iHasDistance.gameObject.name}_{iHasDistance.GetHashCode()}: dist: {dist:0.00} | currDist: {currentDistance:0.00} |  max: {_soundMaxDistanceMap[soundId]:0.00} | vol: {vol:0.00} | {Time.time}");
        }

        void IOnUpdateListener.OnUpdate(GameObject go, int intVal)
        {
            UpdateVolumesRelativeToDistanceToTarget((IHasDistanceToTarget) go, (uint) intVal);
        }
    }
}