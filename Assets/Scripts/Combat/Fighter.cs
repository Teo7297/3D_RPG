using RPG.Movement;
using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {

        [SerializeField]
        private float timeBetweenAttacks = 1f;
        private float timeSinceLastAttack = Mathf.Infinity;

        [SerializeField]
        private Transform rightHandTransform;
        [SerializeField]
        private Transform leftHandTransform;

        [SerializeField]
        private Weapon defaultWeapon;
        private LazyValue<Weapon> currentWeapon;

        private Health target;
        private Mover mover;

        private void Awake()
        {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            mover = GetComponent<Mover>();
        }

        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target is null) return;

            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget is null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            var weaponName = (string)state;
            var w = Resources.Load<Weapon>(weaponName);
            EquipWeapon(w);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.WeaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.PercentageBonus;
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();      //This triggers the animation that triggers Hit()
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //! This is an animation event, called from the attack animation
        private void Hit()
        {
            target?.TakeDamage(gameObject, GetComponent<BaseStats>().GetStat(Stat.Damage));
        }
        //! This is an animation event, called from the attack animation
        private void Shoot()
        {
            if (target is null) { return; }
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, GetComponent<BaseStats>().GetStat(Stat.Damage));
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.value.WeaponRange;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}