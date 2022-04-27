using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private float lifeTime = 1f;
        [SerializeField]
        private bool shouldChaseTarget = false;
        [SerializeField]
        private GameObject hitEffect;

        private Health target;

        private float damage;

        private void Start()
        {
            if (target is null) { return; }
            transform.LookAt(GetAimPosition());
        }

        private void Update()
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0f) { Destroy(gameObject); }
            if (target is null) { return; }
            if (shouldChaseTarget && !target.IsDead())
                transform.LookAt(GetAimPosition());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }

        private Vector3 GetAimPosition()
        {
            var targetCapsule = target.GetComponent<CapsuleCollider>();
            var position = target.transform.position;

            if (targetCapsule is null) { return position; }

            return new Vector3(
                position.x,
                position.y + (targetCapsule.height * .5f),
                position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) { return; }

            target.TakeDamage(damage);

            DestroyHead();

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            foreach (var part in GetComponentsInChildren<ParticleSystem>())
                part.Stop();

            StopProjectile();
        }

        private void DestroyHead()
        {
            var head = GetComponentInChildren<DestroyOnImpact>();
            if (head != null)
                Destroy(head.gameObject);
        }

        private void StopProjectile()
        {
            GetComponent<CapsuleCollider>().enabled = false;
            transform.parent = target.transform;
            target = null;
        }
    }
}