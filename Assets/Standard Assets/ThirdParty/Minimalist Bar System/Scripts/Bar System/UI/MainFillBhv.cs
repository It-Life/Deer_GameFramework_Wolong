using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.UI
{
    public class MainFillBhv : FillableBhv
    {
        // Public properties
        public BarGradient Gradient
        {
            get
            {
                return _barGradient;
            }

            set
            {
                _barGradient = value;

                _barGradient.PopulateGradientLUT();

                this.UpdateFill(this.FillAmount);
            }
        }

        // Private fields
        [SerializeField, ReadOnly] private BarGradient _barGradient;
        private Coroutine _flashCoroutine;
#if UNITY_EDITOR
        private EditorCoroutine _flashEditorCoroutine;
#endif
        private bool _isFlashing;

        public void StartFlashing(Color flashColor, int repetitionCount, float lerpSpeed)
        {
            this.StopFlashing();

            if (Application.isPlaying)
            {
                _flashCoroutine = StartCoroutine(FlashRoutine(flashColor, repetitionCount, lerpSpeed));
            }
#if UNITY_EDITOR
            else
            {
                _flashEditorCoroutine = EditorCoroutineUtility.StartCoroutine(FlashRoutine(flashColor, repetitionCount, lerpSpeed), this);
            }
#endif
        }

        private void StopFlashing()
        {
            if (Application.isPlaying)
            {
                if (_flashCoroutine != null)
                {
                    StopCoroutine(_flashCoroutine);
                }
            }
#if UNITY_EDITOR
            else
            {
                if (_flashEditorCoroutine != null)
                {
                    EditorCoroutineUtility.StopCoroutine(_flashEditorCoroutine);
                }
            }
#endif
        }

        private IEnumerator FlashRoutine(Color flashColor, int lerpRepetitions, float lerpFrequency)
        {
            _isFlashing = true;

            float lerpPeriod = 1 / lerpFrequency;

            Color currentColor, previousColor = Color;

            float lerpTimer = 0;

            float lerp;

            while (lerpTimer < lerpPeriod * lerpRepetitions)
            {
                lerpTimer += Time.deltaTime;

                lerp = Mathf.Clamp01(1 - (Mathf.Cos(2 * Mathf.PI * lerpFrequency * lerpTimer) + 1) / 2);

                currentColor = Color.Lerp(lerpTimer < lerpPeriod / 2 ? previousColor : Gradient.Evaluate(FillAmount), flashColor, lerp);

                UpdateColor(currentColor);

                yield return null;
            }

            UpdateColor(Gradient.Evaluate(FillAmount));

            _isFlashing = false;
        }

        public override void UpdateFill(float fillAmount)
        {
            base.UpdateFill(fillAmount);

            if (!_isFlashing)
            {
                this.UpdateColor(Gradient.Evaluate(fillAmount));
            }
        }

        public void UpdateColor(Color color)
        {
            Color = color;
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
#endif
        }
    }
}