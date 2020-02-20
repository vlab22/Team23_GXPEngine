using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXPEngine.GameLocalEvents;
using GXPEngine.Managers;
using TiledMapParserExtended;

namespace GXPEngine
{
    public class LevelManager : GameObject
    {
        private Level _level;

        private uint _totalScore;
        private uint _levelScore;

        private int _pizzaLives;

        private bool _isLevelEndingByLost;

        private Dictionary<LevelLocalEvent.EventType, uint> _scoreValuesMap;

        public LevelManager(Level pLevel)
        {
            _level = pLevel;

            LoadScoresValuesMap();

            LocalEvents.Instance.AddListener<LevelLocalEvent>(LevelLocalEventHandler);

            CoroutineManager.StartCoroutine(WaitForHUDInstance(), this);
        }

        public int PizzaLives
        {
            get { return _pizzaLives; }
            private set
            {
                _pizzaLives = value;
                HUD.Instance.UpdatePizzaLives(_pizzaLives);

                if (_pizzaLives <= 0 && _isLevelEndingByLost == false)
                {
                    _isLevelEndingByLost = true;
                    EndLevelByLost();
                }
            }
        }

        private IEnumerator WaitForHUDInstance()
        {
            while (HUD.Instance == null)
            {
                yield return null;
            }

            this.PizzaLives = 1;
        }

        private void LevelLocalEventHandler(LevelLocalEvent e)
        {
            switch (e.evt)
            {
                case LevelLocalEvent.EventType.NONE:
                    break;
                case LevelLocalEvent.EventType.LEVEL_START_COUNTER_START:
                    break;
                case LevelLocalEvent.EventType.LEVEL_START_COUNTER_END:
                    break;
                case LevelLocalEvent.EventType.DRONE_DETECTED_ENEMY:
                    break;
                case LevelLocalEvent.EventType.DRONE_HIT_PLAYER:
                    PizzaLives--;
                    
                    break;
                case LevelLocalEvent.EventType.STORK_GET_POINTS_EVADE_DRONE:
                    if (_scoreValuesMap.TryGetValue(LevelLocalEvent.EventType.STORK_GET_POINTS_EVADE_DRONE,
                        out uint score))
                    {
                        _levelScore += score;
                        
                        HUD.Instance.UpdateLevelScore(_levelScore);
                    }

                    break;
                case LevelLocalEvent.EventType.HUNTER_HIT_PLAYER:
                    PizzaLives--;
                    break;
                default:
                    break;
            }
        }

        private void EndLevelByLost()
        {
            //Send all drones away
            _level.DronesesManager.EndLevelAllDrones();

        }

        protected override void OnDestroy()
        {
            LocalEvents.Instance.RemoveListener<LevelLocalEvent>(LevelLocalEventHandler);
            base.OnDestroy();
        }

        void LoadScoresValuesMap()
        {
            _scoreValuesMap = new Dictionary<LevelLocalEvent.EventType, uint>();

            //Load File
            string[] valuesLines = File.ReadAllLines("data/ScoresValues.txt");

            for (int i = 0; i < valuesLines.Length; i++)
            {
                var line = valuesLines[i];
                var lineSplit = line.Split('=');
                if (lineSplit.Length != 2)
                {
                    throw new Exception("data/ScoresValues.txt incorrectly config, all lines must be key=value");
                }

                string key = lineSplit[0].Trim();
                string val = lineSplit[1].Trim();

                if (!LevelLocalEvent.EventType.TryParse(key, true, out LevelLocalEvent.EventType evt))
                {
                    Console.WriteLine($"Error: {key} not defined");
                    continue;
                }

                if (!uint.TryParse(val, out uint score))
                {
                    Console.WriteLine($"Error: {val} not a valid positive integer");
                    continue;
                }

                _scoreValuesMap.Add(evt, score);
            }
        }
    }
}