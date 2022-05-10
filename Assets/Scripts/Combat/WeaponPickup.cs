using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField]
        private WeaponConfig weapon;
        [SerializeField]
        private float healthToRestore;
        [SerializeField]
        private float respawnTime = 5f;

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickUp(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Equals("Player")) { return; }
            PickUp(other.gameObject);
        }

        private void PickUp(GameObject subject)
        {
            if(weapon != null)
            {subject.GetComponent<Fighter>().EquipWeapon(weapon);}
            if(healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }
            StartCoroutine(HideForSeconds(respawnTime));
            //Destroy(gameObject);
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            TogglePickup(false);
            yield return new WaitForSeconds(seconds);
            TogglePickup(true);
        }

        private void TogglePickup(bool show)
        {
            GetComponent<Collider>().enabled = show;
            foreach (Transform child in transform)
                child.gameObject.SetActive(show);
        }
    }
}