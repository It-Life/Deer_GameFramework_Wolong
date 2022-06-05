using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SuperScrollView
{

    public class DragEventHelper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public delegate void OnDragEventHandler (PointerEventData eventData);
        public OnDragEventHandler mOnBeginDragHandler;
        public OnDragEventHandler mOnDragHandler;
        public OnDragEventHandler mOnEndDragHandler;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(mOnBeginDragHandler != null)
            {
                mOnBeginDragHandler(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (mOnDragHandler != null)
            {
                mOnDragHandler(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (mOnEndDragHandler != null)
            {
                mOnEndDragHandler(eventData);
            }
        }
    }
}
