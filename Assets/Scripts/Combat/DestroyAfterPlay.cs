using UnityEngine;

namespace RPG.Combat
{
    public class DestroyAfterPlay : MonoBehaviour
    {
        private ParticleSystem effect;

        private void Start()
        {
            effect = GetComponent<ParticleSystem>();
        }
        private void Update()
        {
            if (!effect.IsAlive())
                Destroy(gameObject);
        }
    }
}