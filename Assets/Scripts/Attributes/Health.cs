using System;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {

        [SerializeField]
        private float regenerationPercentage = 70f;

        private LazyValue<float> healthPoints;

        private bool isDead = false;

        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took damage: " + damage);

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            CheckIfShouldDie();
            if (IsDead())
                AwardExperience(instigator);
        }


        public float GetPercentage()
        {
            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health)) * 100;
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        //* When regenerating hp we want to cap it at 70%
        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100f);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        private void CheckIfShouldDie()
        {
            if (healthPoints.value <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
        }

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience is null) { return; }

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            CheckIfShouldDie();
        }
    }
}