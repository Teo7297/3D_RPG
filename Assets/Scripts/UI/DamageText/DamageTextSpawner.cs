using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField]
        private DamageText damageTextPrefab;


        public void Spawn(float damageAmount)
        {
            var dmgTextInstance = Instantiate<DamageText>(damageTextPrefab, transform);
        }
    }
}