using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        [SerializeField]
        private bool alreadyTriggered = false;
        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyTriggered && other.tag.Equals("Player"))
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }

        }
    }
}
