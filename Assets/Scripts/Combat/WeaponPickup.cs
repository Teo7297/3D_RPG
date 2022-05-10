using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField]
        private WeaponConfig weapon;
        [SerializeField]
        private float respawnTime = 5f;

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickUp(callingController.GetComponent<Fighter>());
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
            PickUp(other.GetComponent<Fighter>());
        }

        private void PickUp(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
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