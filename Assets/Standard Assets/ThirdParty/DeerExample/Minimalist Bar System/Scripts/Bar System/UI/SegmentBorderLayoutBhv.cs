using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minimalist.Bar.Quantity;

namespace Minimalist.Bar.UI
{
    public class SegmentBorderLayoutBhv : EmptyBhv
    {
        // Public properties
        public Color Color
        {
            set
            {
                foreach (BorderBhv border in Borders)
                {
                    border.Color = value;
                }
            }
        }

        // Private properties
        private BorderBhv[] Borders => _borders == null || _borders.Length == 0 ? GetComponentsInChildren<BorderBhv>(true) : _borders;

        // Private fields
        private BorderBhv[] _borders;

        private void Awake()
        {
            _borders = Borders;
        }

        public void UpdateSegmentDelimeters(QuantityBhv quantity, float borderWidth)
        {
            float effectiveWidth = Width + borderWidth;

            for (int i = 0; i < Borders.Length; i++)
            {
                float currentAmount = (i + 1) * quantity.SegmentAmount;

                Borders[i].SizeDelta = new Vector2(borderWidth, 0);

                if (currentAmount < quantity.Capacity && quantity.IsSegmented)
                {
                    float x = currentAmount / quantity.Capacity * effectiveWidth - effectiveWidth / 2f;

                    Borders[i].LocalPosition = new Vector3(x, 0, 0);

                    Borders[i].SizeDelta = new Vector2(borderWidth, 0);

                    Borders[i].IsActive = true;
                }

                else
                {
                    Borders[i].LocalPosition = Vector3.zero;

                    Borders[i].IsActive = false;
                }
            }
        }
    }
}