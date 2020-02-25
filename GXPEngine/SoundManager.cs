using System;
using System.Collections;
using System.Collections.Generic;

namespace GXPEngine
{
    public class SoundManager : GameObject
    {
        private static SoundManager _instance;

        private static object _lock = new object();

        private Sound[] _musics = new Sound[]
        {
            // new Sound("data/Music1.ogg", true, true),
            // new Sound("data/Music2.ogg", true, true),
            // new Sound("data/Music3.ogg", true, true),
        };

        private SoundChannel _currentMusicChannel;

        private Sound[] _fxs = new Sound[]
        {
            new Sound("data/AirplaneEngine.ogg", true, false) //0
        };

        private Sound[] _explosionFxs = new Sound[]
        {
            //new Sound("data/110115__ryansnook__small-explosion.ogg", false, false),
        };

        public Sound[] _hudFxs = new Sound[]
        {
            // new Sound("data/171697__nenadsimic__menu-selection-click.wav", false, false), //0
            // new Sound("data/150222__pumodi__menu-select.wav", false, false), //1
            // new Sound("data/beep1.wav", false, false), //2
            // new Sound("data/beep2 long.wav", false, false), //3
            // new Sound("data/totya__children-yeah-long-and-laugh.ogg", false, true), //4
            // new Sound("data/kids-boo.ogg", false, true), //5
        };

        private Dictionary<GameObject, SoundChannel[]> _fxChannelsMap;
        private SoundChannel[] _fxExplosionChannels;
        private SoundChannel[] _fxHudChannels;

        private SoundManager()
        {
            _fxChannelsMap = new Dictionary<GameObject, SoundChannel[]>();
            _fxExplosionChannels = new SoundChannel[_explosionFxs.Length];
            _fxHudChannels = new SoundChannel[_hudFxs.Length];
        }

        // public void PlayFx(int index)
        // {
        //     index = index % _fxs.Length;
        //
        //     if (_fxChannelsMap[index] != null && _fxChannelsMap[index].IsPlaying)
        //     {
        //         _fxChannelsMap[index].Stop();
        //     }
        //
        //     _fxChannelsMap[index] = _fxs[index].Play();
        // }

        public void CreateFxChannel(GameObject go, int[] soundIds)
        {
            if (_fxChannelsMap.ContainsKey(go))
            {
                return;
            }

            SoundChannel[] soundChannels = new SoundChannel[soundIds.Length];
            for (int i = 0; i < soundIds.Length; i++)
            {
                var soundChannel = _fxs[i].Play(true);
                soundChannel.Stop();

                soundChannels[i] = soundChannel;
            }

            _fxChannelsMap.Add(go, soundChannels);
        }

        public void PlayFxChannel(GameObject go, int soundId)
        {
            var channel = _fxChannelsMap[go][soundId];

            if (channel.IsPlaying)
            {
                channel.Stop();
            }

            channel = _fxs[soundId].Play();
        }

        public void StopFx(GameObject go, int soundId)
        {
            var channel = _fxChannelsMap[go][soundId];

            if (channel.IsPlaying)
            {
                channel.Stop();
            }
        }

        public bool IsFxPlaying(GameObject go, int soundId)
        {
            var channel = _fxChannelsMap[go][soundId];

            return channel.IsPlaying;
        }

        // public void SetEngineFxPitch(int index, float pitch)
        // {
        //     if (_fxChannelsMap[index] == null)
        //     {
        //         PlayFx(index);
        //     }
        //
        //     _fxChannelsMap[index].Frequency = pitch;
        // }

        public void SetFxVolume(GameObject go, int soundId, float vol)
        {
            var channel = _fxChannelsMap[go][soundId];

            if (!channel.IsPlaying)
            {
                channel = _fxs[soundId].Play();
            }
            
            channel.Volume = vol;
        }

        public void PlayExplosionFx(int index = -1)
        {
            if (index == -1)
            {
                index = MRandom.Range(0, _explosionFxs.Length);
            }
            else
            {
                index = index % _explosionFxs.Length;
            }

            if (_fxExplosionChannels[index] != null && _fxExplosionChannels[index].IsPlaying)
                _fxExplosionChannels[index].Stop();

            _fxExplosionChannels[index] = _explosionFxs[index].Play();
        }

        public void PlayHudFx(int index = -1)
        {
            if (index == -1)
            {
                index = MRandom.Range(0, _hudFxs.Length);
            }
            else
            {
                index = index % _hudFxs.Length;
            }

            _fxHudChannels[index] = _hudFxs[index].Play();
        }

        public void PlayMusic(int index)
        {
            if (_currentMusicChannel != null && _currentMusicChannel.IsPlaying)
            {
                _currentMusicChannel.Stop();
            }

            index = Mathf.Abs(index % _musics.Length);

            _currentMusicChannel = _musics[index].Play();
            _currentMusicChannel.Volume = 0.6f;
        }

        public void StopMusic()
        {
            if (_currentMusicChannel != null && _currentMusicChannel.IsPlaying)
            {
                _currentMusicChannel.Stop();
            }
        }

        public void SetCurrentMusicVolume(float vol)
        {
            _currentMusicChannel.Volume = vol;
        }

        public void FadeOutCurrentMusic(int duration)
        {
            CoroutineManager.StartCoroutine(FadeOutMusicRoutine(duration), this);
        }

        public void StopAllSounds()
        {
            foreach (var kv in _fxChannelsMap)
            {
                if (kv.Value != null)
                {
                    for (int i = 0; i < kv.Value.Length; i++)
                    {
                        kv.Value[i].Stop();
                    }
                }
            }
        }

        private IEnumerator FadeOutMusicRoutine(int duration)
        {
            int time = 0;
            float currentVolume = _currentMusicChannel.Volume;
            float faeOutSpeed = _currentMusicChannel.Volume / duration;
            while (time < duration)
            {
                currentVolume -= faeOutSpeed * Time.deltaTime;
                SetCurrentMusicVolume(currentVolume);
                yield return null;

                time += Time.deltaTime;
            }

            SetCurrentMusicVolume(0);
            StopMusic();
        }

        public static SoundManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SoundManager();
                    }

                    return _instance;
                }
            }
        }

        void Update()
        {
            // foreach (var kv in _fxChannelsMap)
            // {
            //     for (int i = 0; i < kv.Value.Length; i++)
            //     {
            //         Console.WriteLine($"{this}: {kv.Key.name} | {kv.Value[i].IsPlaying} | {kv.Value[i].IsPaused}");
            //     }
            // }
        }

        public Sound[] Fxs
        {
            get { return _fxs; }
        }
    }
}