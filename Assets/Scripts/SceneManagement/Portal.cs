using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField]
        private int SceneToLoad = -1;
        [SerializeField]
        private Transform spawnPoint;
        [SerializeField]
        private DestinationIdentifier destination;
        [SerializeField]
        private float fadeOutTime = 1f;
        [SerializeField]
        private float fadeInTime = 2f;
        [SerializeField]
        private float fadeWaitTime = .5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (SceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set on GameObject: " + this.name);
                yield break;
            }

            DontDestroyOnLoad(gameObject);   // This works only if the object is at the ROOT of the project
            Fader fader = FindObjectOfType<Fader>();

            yield return fader.FadeOut(fadeOutTime);

            //save current scene
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(SceneToLoad);

            //load new scene
            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (var portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != this.destination) continue;
                return portal;
            }
            return null;
        }
    }
}
