using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minimalist.Bar.UI
{
    [RequireComponent(typeof(RectMask2D))]
    public class FillProxyBhv : FillableBhv 
    {
        // Private properties
        private RectMask2D RectMask => _rectMask == null ? this.GetComponent<RectMask2D>() : _rectMask;

        // Private fields
        private RectMask2D _rectMask;

        private void Awake()
        {
            _rectMask = RectMask;
        }

        public override void UpdateFill(float fillAmount)
        {
            base.UpdateFill(fillAmount);

            RectMask.padding = new Vector4(0, 0, (1 - fillAmount) * this.Width, 0);
        }
    }
}
