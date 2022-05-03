using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;
namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression;
        [SerializeField]
        private GameObject levelUpEffect;
        [SerializeField]
        private bool shouldUseModifiers;

        private Experience experience;

        private LazyValue<int> currentLevel;
        public event Action onLevelUp;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        public float GetStat(Stat stat)
        {
            var baseStatValue = progression.GetStat(stat, characterClass, GetLevel());

            if (!shouldUseModifiers) { return baseStatValue; }

            var additiveStatValue = baseStatValue + GetAdditiveModifier(stat);

            var multiplicativeStatValue = additiveStatValue * (1f + GetPercentageModifier(stat) / 100f);

            return multiplicativeStatValue;
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            if (experience == null) return startingLevel;

            float currentXP = experience.ExperiencePoints;
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            float total = 0f;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            float total = 0f;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpEffect, transform);
        }
    }
}