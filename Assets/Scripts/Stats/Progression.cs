using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField]
        private ProgressionCharacterClass[] progressionCharacterClasses;

        public float GetHealth(CharacterClass characterClass, int level)
        {
            foreach (var progressionClass in progressionCharacterClasses)
            {
                if (progressionClass.CharacterClass == characterClass)
                {
                    return progressionClass.Health[level - 1];
                }
            }
            return 0f;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField]
            private CharacterClass characterClass;
            [SerializeField]
            private float[] health;

            internal float[] Health { get => health; }
            internal CharacterClass CharacterClass { get => characterClass; }
        }
    }
}