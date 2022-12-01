using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.Utility
{
    [System.Serializable]
    public class BarGradient
    {
        // Constants fields
        private const int RESOLUTION = 100;

        // Private fields
        [SerializeField]
        private Gradient _gradient = new Gradient()
        {
            colorKeys = new GradientColorKey[2]
            {
                new GradientColorKey(Color.magenta, 0),
                new GradientColorKey(Color.cyan, 1)
            },

            alphaKeys = new GradientAlphaKey[2]
            {
                new GradientAlphaKey(1, 0),
                new GradientAlphaKey(1, 1)
            }
        };
        private Color[] _gradientLUT;

        public void PopulateGradientLUT()
        {
            _gradientLUT = new Color[RESOLUTION + 1];

            for (int i = 0; i <= RESOLUTION; i++)
            {
                _gradientLUT[i] = _gradient.Evaluate((float)i / RESOLUTION);
            }
        }

        public Color Evaluate(float time)
        {
            if (_gradientLUT == null || _gradientLUT.Length == 0)
            {
                this.PopulateGradientLUT();
            }

            return _gradientLUT[Mathf.RoundToInt(time * RESOLUTION)];
        }
    }
}
