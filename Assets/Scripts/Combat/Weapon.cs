using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField]
        private GameObject equippedWeaponPrefab;
        [SerializeField]
        private AnimatorOverrideController animatorOverrideController;
        [SerializeField]
        private float weaponDamage = 5f;
        [SerializeField]
        private float weaponRange = 2f;
        [SerializeField]
        private bool isRightHanded = true;
        [SerializeField]
        private Projectile projectile;

        const string weaponName = "Weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            if (equippedWeaponPrefab != null)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);
                var weapon = Instantiate(equippedWeaponPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverrideController != null)
            {
                animator.runtimeAnimatorController = animatorOverrideController;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            var oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon is null)
                oldWeapon = leftHand.Find(weaponName);
            if (oldWeapon is null) { return; }

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        public bool HasProjectile()
        {
            return !(projectile is null);
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator)
        {
            var projectilInstance = Instantiate(
                projectile,
                GetHandTransform(rightHand, leftHand).position,
                Quaternion.identity
            );

            projectilInstance.SetTarget(target, instigator, weaponDamage);
        }

        public float WeaponDamage { get => weaponDamage; }

        public float WeaponRange { get => weaponRange; }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }
    }
}