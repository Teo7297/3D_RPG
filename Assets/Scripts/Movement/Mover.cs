using System.Collections.Generic;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField]
        private float maxSpeed = 5.66f;
        [SerializeField]
        private float maxNavPathLength = 40f;

        private NavMeshAgent navMeshAgent;
        private Health health;
        private Animator animator;
        private ActionScheduler actionScheduler;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            navMeshAgent.enabled = !health.IsDead();

            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            //get velocity from navMeshAgent (we get the global velocity!)
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;

            //convert velocity from global to local (we just need to know how fast we are moving, the global value may be influenced by other things!)
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            //Get the speed on the z axis
            float speed = localVelocity.z;

            //Pass the value to the animator
            animator.SetFloat("forwardSpeed", speed);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            //calculate path and return false if too long
            NavMeshPath path = new NavMeshPath();
            //! path passed by ref and modified inside CalculatePath
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; }  //! this disables unreachable zones paths
            if (GetPathLength(path) > maxNavPathLength) { return false; }

            return true;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            //We cancel the attack when we start moving
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        //! This is called after awake but before start
        public void RestoreState(object state)
        {
            // We could use a cast, but in case of error an exception would be thrown..
            //* SerializableVector3 position = (SerializableVector3)state;

            // In case we need to cast and we are not sure of the object type we should use the as statement, in case of error it returns null:
            //* SerializableVector3 position = state as SerializableVector3;

            Dictionary<string, object> data = (Dictionary<string, object>)state;
            var position = (SerializableVector3)data["position"];
            var rotation = (SerializableVector3)data["rotation"];

            navMeshAgent.Warp(position.ToVector());
            transform.eulerAngles = rotation.ToVector();
        }

        //*We can also use a struct to save multiple values:
        // [System.Serializable]
        // struct MoverSaveData
        // {
        //     public SerializableVector3 position;
        //     public SerializableVector3 rotation;
        // }
        // MoverSaveData data = new MoverSaveData();
        // data.position = ....transform.position;
        // ..rotation..
        // return data;

        private float GetPathLength(NavMeshPath path)
        {
            var total = 0f;
            if (path.corners.Length < 2) { return total; }
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }
    }
}
