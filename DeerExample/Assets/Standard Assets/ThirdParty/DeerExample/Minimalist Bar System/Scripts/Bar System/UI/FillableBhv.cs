using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.UI
{
    public class FillableBhv : ImageBhv
    {
        // Public properties
        public float FillAmount
        {
            get
            {
                return _fillAmount;
            }

            set
            {
                this.StartFilling(value);
            }
        }
        public float FillTime
        {
            get
            {
                return _fillTime;
            }

            set
            {
                _fillTime = value;
            }
        }

        // Public fields
        [SerializeField] private FillProxyBhv _fillProxy;

        // Private fields
        [SerializeField, Range(0, 1), ReadOnly] private float _fillAmount;
        private float _fillTime;
        private Coroutine _fillCoroutine;
#if UNITY_EDITOR
        private EditorCoroutine _fillEditorCoroutine;
#endif

        private void Start()
        {
            _fillAmount = Image.fillAmount;
        }

        private void StartFilling(float fillAmount)
        {
            this.StopFilling();

            if (Application.isPlaying)
            {
                _fillCoroutine = StartCoroutine(FillCoroutine(fillAmount, _fillTime));
            }
#if UNITY_EDITOR
            else
            {
                _fillEditorCoroutine = EditorCoroutineUtility.StartCoroutine(FillCoroutine(fillAmount, _fillTime), this);
            }
#endif
        }

        private void StopFilling()
        {
            if (Application.isPlaying)
            {
                if (_fillCoroutine != null)
                {
                    StopCoroutine(_fillCoroutine);
                }
            }
#if UNITY_EDITOR
            else
            {
                if (_fillEditorCoroutine != null)
                {
                    EditorCoroutineUtility.StopCoroutine(_fillEditorCoroutine);
                }
            }
#endif
        }

        private IEnumerator FillCoroutine(float targetFillAmount, float lerpPeriod)
        {
            float lerpFrequency = 1 / lerpPeriod;

            float previousFillAmount = FillAmount;

            float lerpTimer = 0;

            float lerp;

            while (lerpTimer < lerpPeriod / 2)
            {
                lerpTimer += Time.deltaTime;

                lerp = Mathf.Clamp01(1 - (Mathf.Cos(2 * Mathf.PI * lerpFrequency * lerpTimer) + 1) / 2);

                _fillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, lerp);

                UpdateFill(_fillAmount);

                yield return null;
            }

            _fillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, 1);

            UpdateFill(_fillAmount);
        }

        public virtual void UpdateFill(float fillAmount)
        {
            Image.fillAmount = fillAmount;

            if (_fillProxy != null)
            {
                _fillProxy.UpdateFill(fillAmount);
            }

#if UNITY_EDITOR
            if (Application.isEditor)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
#endif
        }
    }
}