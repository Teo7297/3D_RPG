using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField]
        private DamageText damageTextPrefab;


        public void Spawn(float damageAmount)
        {
            var x = Random.Range(-.3f, .3f) + transform.position.x;
            var y = Random.Range(-.3f, .3f) + transform.position.y;
            var dmgTextInstance = Instantiate<DamageText>(damageTextPrefab, new Vector3(x, y, transform.position.z), Quaternion.identity, transform);
            dmgTextInstance.SetValue(damageAmount);
        }
    }
}