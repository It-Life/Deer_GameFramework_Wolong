using Minimalist.Bar.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minimalist.Bar.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class BarCanvasBhv : EmptyBhv
    {
        // Public properties
        public RenderMode RenderMode => _barRenderMode;
        public Transform AnchorTransform { get => _anchorTransform; set => _anchorTransform = value; }

        // Private properties
        private Canvas Canvas => _canvas == null || !Application.isPlaying ? GetComponent<Canvas>() : _canvas;
        private BarBhv[] Bars => _bars == null || _bars.Length == 0 || !Application.isPlaying ? GetComponentsInChildren<BarBhv>() : _bars;

        // Private fields
        [SerializeField] private RenderMode _barRenderMode;
        [SerializeField] private Transform _anchorTransform;
        [SerializeField] private Camera _camera;
        private Canvas _canvas;
        private BarBhv[] _bars;

        private void Awake()
        {
            _camera = _camera == null ? Camera.main : _camera;

            _canvas = Canvas;

            _bars = Bars;
        }

        public void OnEnable()
        {
            SetHideFlags();

            UpdateName();
        }

        private void OnTransformChildrenChanged()
        {
            UpdateName();

            ResetBarScales();
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                LookAtCameraIf(true);
            }
        }

        private void SetHideFlags()
        {
            RectTransform.hideFlags = HideFlags.None;

            Canvas.hideFlags = HideFlags.NotEditable;

            hideFlags = HideFlags.None;
        }

        public void LookAtCameraIf(bool condition)
        {
            if (_barRenderMode == RenderMode.HUD)
            {
                return;
            }

            Vector3 alignment = !condition || _camera == null ? Vector3.forward : _camera.transform.forward;

            RectTransform.LookAt(RectTransform.position + alignment);
        }

        public void UpdateName()
        {
            //if (BarSystemManager.Instance.automaticObjectNaming)
            //{
            //    name = "Bar Canvas (" + _barRenderMode.ToString() + ")";

            //    foreach (BarBhv bar in Bars)
            //    {
            //        bar.UpdateName();
            //    }
            //}
        }

        public void UpdateCanvas()
        {
            switch (_barRenderMode)
            {
                case RenderMode.World:

                    RectTransform.SetParent(_anchorTransform, false);

                    RectTransform.localScale = Vector3.one / 100f;

                    Canvas.renderMode = UnityEngine.RenderMode.WorldSpace;

                    Canvas.worldCamera = _camera;

                    SizeDelta = Vector2.zero;

                    LocalPosition = Vector3.zero;

                    break;

                case RenderMode.HUD:

                    RectTransform.localScale = Vector3.one;

                    Canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceOverlay;

                    RectTransform.SetParent(BarSystemManager.Instance.transform, false);

                    break;
            }

            ResetBarScales();
        }

        private void ResetBarScales()
        {
            foreach (BarBhv bar in Bars)
            {
                bar.LocalScale = Vector3.one;
            }
        }
    }
}
