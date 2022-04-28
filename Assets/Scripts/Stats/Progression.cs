using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField]
        private ProgressionCharacterClass[] progressionCharacterClasses;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();
            // foreach (var progressionClass in progressionCharacterClasses)
            // {
            //     if (progressionClass.CharacterClass != characterClass) { continue; }

            //     foreach (var progressionStat in progressionClass.Stats)
            //     {
            //         if (progressionStat.Stat != stat) { continue; }
            //         if (progressionStat.Levels.Length < level) { continue; }

            //         return progressionStat.Levels[level - 1];
            //     }
            // }
            // return 0f;
            float[] levels = lookupTable[characterClass][stat];
            if (levels.Length < level) return 0f;
            return levels[level - 1];
        }

        private void BuildLookup()
        {
            if (lookupTable != null) { return; }

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (var progressionClass in progressionCharacterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (var progressionStat in progressionClass.Stats)
                {
                    statLookupTable[progressionStat.Stat] = progressionStat.Levels;
                }

                lookupTable[progressionClass.CharacterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField]
            private CharacterClass characterClass;
            [SerializeField]
            private ProgressionStat[] stats;

            internal CharacterClass CharacterClass { get => characterClass; }
            internal ProgressionStat[] Stats { get => stats; }
        }

        [System.Serializable]
        class ProgressionStat
        {
            [SerializeField]
            private Stat stat;
            [SerializeField]
            private float[] levels;

            internal float[] Levels { get => levels; }
            internal Stat Stat { get => stat; }
        }
    }
}