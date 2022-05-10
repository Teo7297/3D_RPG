using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onHit;

        public void OnHit()
        {
            print("Weapon Hit " + gameObject.name);
            onHit.Invoke();
        }
    }
}