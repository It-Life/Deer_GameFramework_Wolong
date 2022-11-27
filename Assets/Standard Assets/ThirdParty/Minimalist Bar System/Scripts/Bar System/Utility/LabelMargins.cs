using System;
using UnityEngine;

namespace Minimalist.Bar.Utility
{
    [Serializable]
    public class LabelMargins
    {
        // Public properties
        public float Left => _left;
        public float Top => _top;
        public float Right => _right;
        public float Bottom => _bottom;

        // Public fields
        [SerializeField] private float _left;
        [SerializeField] private float _top;
        [SerializeField] private float _right;
        [SerializeField] private float _bottom;
    }
}
