using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.Quantity
{
    [System.Serializable]
    public class Dynamics
    {
        public DynamicsType Type
        {
            get
            {
                return _dynamicsType;
            }
        }
        public float DeltaPercentage
        {
            get
            {
                return _deltaPercentage / 100f;
            }
        }
        public float SignedDeltaPercentage
        {
            get
            {
                return _deltaPercentage / 100f * (float)_dynamicsType;
            }
        }
        public float DeltaTime
        {
            get
            {
                return _deltaTime;
            }
        }
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
            }
        }

        [SerializeField]
        private DynamicsType _dynamicsType = DynamicsType.Accumulation;
        [SerializeField, Range(0f, 100f)]
        private float _deltaPercentage = 1f;
        [SerializeField, Min(.02f)]
        private float _deltaTime = .5f;
        [SerializeField]
        private bool _enabled = true;
    }
}