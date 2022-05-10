using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField]
        private Weapon equippedWeaponPrefab;
        [SerializeField]
        private AnimatorOverrideController animatorOverrideController;
        [SerializeField]
        private float weaponDamage = 5f;
        [SerializeField]
        private float percentageBonus = 0f;
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
                var handTransform = GetHandTransform(rightHand, leftHand);
                var weapon = Instantiate(equippedWeaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
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

        public bool HasProjectile()
        {
            return !(projectile is null);
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            var projectilInstance = Instantiate(
                projectile,
                GetHandTransform(rightHand, leftHand).position,
                Quaternion.identity
            );

            projectilInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float WeaponDamage { get => weaponDamage; }

        public float WeaponRange { get => weaponRange; }

        public float PercentageBonus { get => percentageBonus; }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            var oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon is null)
                oldWeapon = leftHand.Find(weaponName);
            if (oldWeapon is null) { return; }

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            return isRightHanded ? rightHand : leftHand;
        }
    }
}