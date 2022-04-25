using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField]
        private GameObject weaponPrefab;
        [SerializeField]
        private AnimatorOverrideController animatorOverrideController;

        public void Spawn(Transform handTransform, Animator animator)
        {
            Instantiate(weaponPrefab, handTransform);
            animator.runtimeAnimatorController = animatorOverrideController;
        }
    }
}