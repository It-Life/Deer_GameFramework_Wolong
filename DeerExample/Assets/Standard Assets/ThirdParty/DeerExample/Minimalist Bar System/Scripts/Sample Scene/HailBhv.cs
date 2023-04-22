using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.SampleScene
{
    public class HailBhv : MonoBehaviour
    {
        public float TimeScale
        {
            get
            {
                return _particleSystem.main.simulationSpeed;
            }

            set
            {
                ParticleSystem.MainModule main = _particleSystem.main;

                main.simulationSpeed = value;
            }
        }

        public float damagePerHit = 15;

        private ParticleSystem _particleSystem;
        private List<ParticleCollisionEvent> _collisionEvents;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            _collisionEvents = new List<ParticleCollisionEvent>();
        }

        private void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = _particleSystem.GetCollisionEvents(other, _collisionEvents);

            IDamageable damageable = other.GetComponent<IDamageable>();

            for (int i = 0; i < numCollisionEvents; i++)
            {
                float speed = _collisionEvents[i].velocity.magnitude;

                if (damageable != null && speed > 5)
                {
                    damageable.TakeDamage(damagePerHit);
                }
            }
        }
    }
}