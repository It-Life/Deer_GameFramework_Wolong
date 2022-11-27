using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Minimalist.Bar.UI
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class EmptyBhv : MonoBehaviour
    {
        // Public properties
        public bool IsActive
        {
            get
            {
                return GameObject.activeInHierarchy;
            }

            set
            {
                GameObject.SetActive(value);
            }
        }
        public Vector3 Position
        {
            get
            {
                return RectTransform.position;
            }

            set
            {
                RectTransform.position = value;
            }
        }
        public Vector3 LocalPosition
        {
            get
            {
                return RectTransform.localPosition;
            }

            set
            {
                RectTransform.localPosition = value;
            }
        }
        public Vector3 LocalScale
        {
            get
            {
                return RectTransform.localScale;
            }

            set
            {
                RectTransform.localScale = value;
            }
        }
        public Vector2 SizeDelta
        {
            get
            {
                return RectTransform.sizeDelta;
            }

            set
            {
                RectTransform.sizeDelta = value;
            }
        }
        public Vector2 AnchorMin
        {
            get
            {
                return RectTransform.anchorMin;
            }
        }
        public Vector2 AnchorMax
        {
            get
            {
                return RectTransform.anchorMax;
            }
        }
        public float Width
        {
            get
            {
                return RectTransform.rect.width;
            }
        }
        public float Height
        {
            get
            {
                return RectTransform.rect.height;
            }
        }

        // Protected properties
        protected GameObject GameObject => _gameObject == null || !Application.isPlaying ? gameObject : _gameObject;
        protected RectTransform RectTransform => _rectTransform == null || !Application.isPlaying ? GetComponent<RectTransform>() : _rectTransform;

        // Private fields
        private GameObject _gameObject;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _gameObject = GameObject;

            _rectTransform = RectTransform;
        }

        [ContextMenu("Set All Children Hide Flags to 'HideInHierarchy'")]
        protected void MakeAllChildrenHidden()
        {
            RectTransform[] children = GetComponentsInChildren<RectTransform>();

            foreach (RectTransform child in children)
            {
                if (child != RectTransform)
                {
                    child.hideFlags = HideFlags.HideInHierarchy;
                }
            }
        }

        [ContextMenu("Set All Children Hide Flags to 'NotEditable'")]
        protected void MakeAllChildrenNotEditable()
        {
            RectTransform[] children = GetComponentsInChildren<RectTransform>();

            foreach (RectTransform child in children)
            {
                if (child != RectTransform)
                {
                    child.hideFlags = HideFlags.NotEditable;
                }
            }
        }

        [ContextMenu("Set All Children Hide Flags to 'None'")]
        protected void ShowAllChildren()
        {
            RectTransform[] children = GetComponentsInChildren<RectTransform>();

            foreach (RectTransform child in children)
            {
                child.hideFlags = HideFlags.None;
            }
        }

        [ContextMenu("Make All Children Unpickable")]
        protected void MakeAllChildrenUnpickable()
        {
#if UNITY_EDITOR
            foreach (RectTransform child in RectTransform)
            {
                SceneVisibilityManager.instance.DisablePicking(child.gameObject, true);
            }
#endif
        }

        [ContextMenu("Make All Children Pickable")]
        protected void MakeAllChildrenPickable()
        {
#if UNITY_EDITOR
            foreach (RectTransform child in RectTransform)
            {
                SceneVisibilityManager.instance.EnablePicking(child.gameObject, true);
            }
#endif
        }
    }
}