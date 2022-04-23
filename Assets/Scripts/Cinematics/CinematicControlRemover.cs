using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    //! We use events here
    public class CinematicControlRemover : MonoBehaviour
    {
        private GameObject player;

        private void Start()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
            player = GameObject.FindWithTag("Player");

        }

        private void DisableControl(PlayableDirector _)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
            Debug.Log("Disabled Control");
        }

        private void EnableControl(PlayableDirector _)
        {
            player.GetComponent<PlayerController>().enabled = true;
            Debug.Log("Enabled Control");
        }
    }
}
