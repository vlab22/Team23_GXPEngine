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
        

        private Dictionary<uint, IHasDistanceToTarget> _fxChannelDistanceMap =
            new Dictionary<uint, IHasDistanceToTarget>();

        public EnemiesSoundManager() : base(false)
        {
            _soundManager = SoundManager.Instance;

            _soundMaxVolumeMap = new Dictionary<uint, float>()
            {
                {0, 0.2f}, //Airplane
                {1, 0.05f}, //Drone
                {8, 0.10f}, //Tornado
            };

            _soundMaxDistanceMap = new Dictionary<uint, float>()
            {
                {0, 1000f},
                {1, 500f},
                {8, 800},
            };

            CoroutineManager.StartCoroutine(Start(), this);
        }

        private IEnumerator Start()
        {
            while (_cam == null)
            {
                yield return null;
                _cam = MyGame.ThisInstance.Camera;
            }
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