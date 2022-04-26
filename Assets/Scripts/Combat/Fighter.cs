using RPG.Movement;
using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {

        [SerializeField]
        private float timeBetweenAttacks = 1f;
        private float timeSinceLastAttack = Mathf.Infinity;

        [SerializeField]
        private Transform RightHandTransform;
        [SerializeField]
        private Weapon weapon;

        private Health target;
        private Mover mover;

        private void Start()
        {
            mover = GetComponent<Mover>();
            SpawnWeapon();
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

        private void SpawnWeapon()
        {
            if (weapon == null) { return; }
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(RightHandTransform, animator);
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
        void Hit()
        {
            target?.TakeDamage(weapon.WeaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= weapon.WeaponRange;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }

}