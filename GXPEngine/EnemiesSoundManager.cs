using System;
using System.Collections;
using System.Collections.Generic;

namespace GXPEngine
{
    public class EnemiesSoundManager : GameObject, IOnUpdateListener
    {
        private SoundManager _soundManager;
        private FollowCamera _cam;

        private Dictionary<int, float> _soundMaxVolumeMap;
        private Dictionary<int, float> _soundMaxDistanceMap;

        private Dictionary<GameObject, SoundChannel[]>
            _fxChannelEnemyMap = new Dictionary<GameObject, SoundChannel[]>();

        public EnemiesSoundManager() : base(false)
        {
            _soundManager = SoundManager.Instance;

            _soundMaxVolumeMap = new Dictionary<int, float>()
            {
                {0, 0.7f}
            };

            _soundMaxDistanceMap = new Dictionary<int, float>()
            {
                {0, 1000f},
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
            if (go is Airplane)
            {
                _soundManager.CreateFxChannel(go, new int[] {0});
            }
        }

        private void UpdateVolumesRelativeToDistanceToTarget(GameObject soundMaker, int soundId)
        {
            var iHasDistance = soundMaker as IHasDistanceToTarget;

            float dist = iHasDistance.Distance.Magnitude;

            float vol = 0;

            if (dist > _soundMaxDistanceMap[soundId])
            {
                _soundManager.StopFx(soundMaker, soundId);
            }
            else
            {
                vol = Mathf.Map(dist, _soundMaxDistanceMap[soundId], 0, 0, _soundMaxVolumeMap[soundId]);

                _soundManager.SetFxVolume(soundMaker, soundId, vol);
            }

            Console.WriteLine(
                $"{this}: dist: {dist:0.00} | max: {_soundMaxDistanceMap[soundId]:0.00} | vol: {vol:0.00}");
        }

        void IOnUpdateListener.OnUpdate(GameObject go, int intVal)
        {
            UpdateVolumesRelativeToDistanceToTarget(go, intVal);
        }
    }
}