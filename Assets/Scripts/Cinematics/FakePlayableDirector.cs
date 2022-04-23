using System;
using UnityEngine;

//! EXAMPLE DUMMY CLASS NOT ACTUALLY USED
namespace RPG.Cinematics
{

    public class FakePlayableDirector : MonoBehaviour
    {
        public event Action<float> onFinish;

        private void Start()
        {
            Invoke("OnFinish", 3f);
        }

        private void OnFinish()
        {
            onFinish(4.3f); // This calls ALL the callbacks registered in the event list in the order they are inserted
        }
    }
}