using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperScrollView
{

    public class DragChangSizeScript :MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler
    {
        bool mIsDragging = false;

        public Camera mCamera;

        public float mBorderSize = 10;

        public Texture2D mCursorTexture;

        public Vector2 mCursorHotSpot = new Vector2(16, 16);

        public bool mIsVertical = false;

        RectTransform mCachedRectTransform;

        public System.Action mOnDragBeginAction;
        public System.Action mOnDraggingAction;
        public System.Action mOnDragEndAction;
        public RectTransform CachedRectTransform
        {
            get
            {
                if (mCachedRectTransform == null)
                {
                    mCachedRectTransform = gameObject.GetComponent<RectTransform>();
                }
                return mCachedRectTransform;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetCursor(null, mCursorHotSpot, CursorMode.Auto);
        }

        void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
        {
            if (Input.mousePresent)
            {
                Cursor.SetCursor(texture, hotspot, cursorMode);
            }
        }

        void LateUpdate()
        {
            if (mCursorTexture == null)
            {
                return;
            }
            
            if(mIsDragging)
            {
                SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
                return;
            }

            Vector2 point;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, Input.mousePosition, mCamera, out point))
            {
                SetCursor(null, mCursorHotSpot, CursorMode.Auto);
                return;
            }
            if(mIsVertical)
            {
                float d = CachedRectTransform.rect.height + point.y;
                if (d < 0)
                {
                    SetCursor(null, mCursorHotSpot, CursorMode.Auto);
                }
                else if (d <= mBorderSize)
                {
                    SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
                }
                else
                {
                    SetCursor(null, mCursorHotSpot, CursorMode.Auto);
                }
            }
            else
            {
                float d = CachedRectTransform.rect.width - point.x;
                if (d < 0)
                {
                    SetCursor(null, mCursorHotSpot, CursorMode.Auto);
                }
                else if (d <= mBorderSize)
                {
                    SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
                }
                else
                {
                    SetCursor(null, mCursorHotSpot, CursorMode.Auto);
                }
            }
            

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            mIsDragging = true;
            if(mOnDragBeginAction != null)
            {
                mOnDragBeginAction();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            mIsDragging = false;
            if(mOnDragEndAction != null)
            {
                mOnDragEndAction();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 p1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, eventData.position, mCamera, out p1);
            if(mIsVertical)
            {
                if (p1.y < 0)
                {
                    CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -p1.y);
                }
            }
            else
            {
                if (p1.x > 0)
                {
                    CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, p1.x);
                }
            }
            if (mOnDraggingAction != null)
            {
                mOnDraggingAction();
            }
        }

    }
}
