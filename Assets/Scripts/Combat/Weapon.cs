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

        public void Spawn(Transform handTransform, Animator animator)
        {
            if (equippedWeaponPrefab != null)
            {
                Instantiate(equippedWeaponPrefab, handTransform);
            }
            if (animatorOverrideController != null)
            {
                animator.runtimeAnimatorController = animatorOverrideController;
            }
        }

        public float WeaponDamage { get => weaponDamage; }

        public float WeaponRange { get => weaponRange; }
    }
}