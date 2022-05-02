using RPG.Saving;
using UnityEngine;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField]
        private float experiencePoints;

        //public delegate void ExperienceGainedDelegate();
        //public event ExperienceGainedDelegate onExperienceGained;
        public event Action onExperienceGained;

        public float ExperiencePoints { get => experiencePoints; }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public object CaptureState()
        {
            return ExperiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}