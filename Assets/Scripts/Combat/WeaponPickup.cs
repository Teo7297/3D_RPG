using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField]
        private Weapon weapon;
        [SerializeField]
        private float respawnTime = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Equals("Player")) { return; }
            other.GetComponent<Fighter>().EquipWeapon(weapon);
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