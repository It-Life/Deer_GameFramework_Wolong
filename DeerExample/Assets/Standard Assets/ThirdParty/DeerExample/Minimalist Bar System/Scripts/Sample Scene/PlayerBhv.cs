using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minimalist.Bar.Quantity;

namespace Minimalist.Bar.SampleScene
{
    public class PlayerBhv : CharacterBhv
    {
        // Public fields
        public QuantityBhv mana;
        public QuantityBhv fury;
        public QuantityBhv cast;
        public QuantityBhv experience;

        [Header("Heal Ability:")]
        public float healCastTime = 1;
        public float healAmount = 25;
        public float healCost = 25;
        public QuantityType healCostResource = QuantityType.Mana;

        [Header("Movement:")]
        public float movementSpeed = 1f;

        // Private fields
        private Transform _transform;
        private Dictionary<QuantityType, QuantityBhv> _resources;
        private float _castTimer;
        private bool _casting;

        public void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void Start()
        {
            PopulateResourcesDictionary();
        }

        private void OnValidate()
        {
            PopulateResourcesDictionary();
        }

        private void PopulateResourcesDictionary()
        {
            _resources = new Dictionary<QuantityType, QuantityBhv>();
            _resources.Add(QuantityType.Health, health);
            _resources.Add(QuantityType.Mana, mana);
            _resources.Add(QuantityType.Fury, fury);
        }

        private void Update()
        {
            HandlePlayerMovement();
        }

        private void HandlePlayerMovement()
        {
            if (Input.GetKey(KeyCode.A))
            {
                _transform.Translate(Vector3.left * Time.deltaTime * (movementSpeed + fury.FillAmount), Space.World);
            }
            if (Input.GetKey(KeyCode.D))
            {
                _transform.Translate(Vector3.right * Time.deltaTime * (movementSpeed + fury.FillAmount), Space.World);
            }
            if (Input.GetKey(KeyCode.W))
            {
                _transform.Translate(Vector3.forward * Time.deltaTime * (movementSpeed + fury.FillAmount), Space.World);
            }
            if (Input.GetKey(KeyCode.S))
            {
                _transform.Translate(Vector3.back * Time.deltaTime * (movementSpeed + fury.FillAmount), Space.World);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryToHeal();
            }
        }

        private bool TryToHeal()
        {
            if (_casting)
            {
                cast.OnInvalidAmount.Invoke();

                return false;
            }

            else
            {
                if (_resources[healCostResource].Amount >= healCost)
                {
                    StartCoroutine(CastHeal());

                    return true;
                }

                else
                {
                    _resources[healCostResource].OnInvalidAmount.Invoke();
                }

                return false;
            }
        }

        private IEnumerator CastHeal()
        {
            _casting = true;

            _castTimer = 0;

            while (_castTimer < healCastTime)
            {
                float lerp = _castTimer / healCastTime;

                cast.Amount = Mathf.Lerp(cast.MinimumAmount, cast.MaximumAmount, lerp);

                _castTimer += Time.deltaTime;

                yield return null;
            }

            _casting = false;

            Heal();

            _resources[healCostResource].Amount -= healCost;

            cast.Amount = 0;

            GainXP();
        }

        private void Heal()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10);

            foreach (Collider collider in colliders)
            {
                CharacterBhv character = collider.GetComponent<CharacterBhv>();

                if (character != null && character.team == team)
                {
                    character.ReceiveHeal(healAmount);
                }
            }
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            fury.Amount += .1f * fury.Capacity;

            if (_casting)
            {
                _castTimer = Mathf.Clamp(_castTimer - healCastTime * .25f, 0, healCastTime);
            }
        }

        private void GainXP()
        {
            experience.Amount += experience.SegmentAmount;

            if (experience.Amount == experience.MaximumAmount)
            {
                StartCoroutine(LevelUp());
            }
        }

        private IEnumerator LevelUp()
        {
            yield return new WaitForSeconds(2f);

            float previousMaximum = experience.MaximumAmount;

            experience.MaximumAmount += experience.Capacity * 1.25f;

            experience.MinimumAmount = previousMaximum;

            experience.Amount = experience.MinimumAmount;

            health.MaximumAmount += health.SegmentAmount;

            health.Amount = health.MaximumAmount;

            mana.Amount = mana.MaximumAmount;

            fury.Amount = fury.MaximumAmount;
        }
    }
}