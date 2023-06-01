using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Minimalist.Bar.Quantity;

namespace Minimalist.Bar.UI
{
    public abstract class QuantitySubscriberBhv : EmptyBhv
    {
        // Public properties
        public QuantityBhv Quantity => _quantity;

        // Protected properties
        protected UnityAction OnQuantityTypeChangedAction { get => _onQuantityTypeChangedAction; set => _onQuantityTypeChangedAction = value; }
        protected UnityAction OnQuantityAmountChangedAction { get => _onQuantityAmountChangedAction; set => _onQuantityAmountChangedAction = value; }
        protected UnityAction OnQuantityInvalidAmountAction { get => _onQuantityInvalidAmountAction; set => _onQuantityInvalidAmountAction = value; }

        // Private serialized fields
        [Header("Subscription:")]
        [SerializeField] private QuantityBhv _quantity;

        // Private fields
        private QuantityBhv _previousQuantity;
        private UnityAction _onQuantityTypeChangedAction;
        private UnityAction _onQuantityAmountChangedAction;
        private UnityAction _onQuantityInvalidAmountAction;

        protected abstract void SpecifyQuantityEventActions();

        protected virtual void OnEnable()
        {
            StartListeningTo(_quantity);
        }

        protected virtual void OnDisable()
        {
            StopListeningTo(_quantity);
        }

        protected virtual void OnValidate()
        {
            SpecifyQuantityEventActions();
            
            UpdateQuantitySubscription();
        }

        private void UpdateQuantitySubscription()
        {
            StartListeningTo(_quantity);

            if (_quantity != _previousQuantity)
            {
                StopListeningTo(_previousQuantity);
            }

            _previousQuantity = _quantity;
        }

        private void StartListeningTo(QuantityBhv quantity)
        {
            if (quantity == null)
            {
                return;
            }

            if (!IsListeningTo(quantity.OnTypeChanged))
            {
                quantity.AddListener(quantity.OnTypeChanged, _onQuantityTypeChangedAction);
            }

            if (!IsListeningTo(quantity.OnAmountChanged))
            {
                quantity.AddListener(quantity.OnAmountChanged, _onQuantityAmountChangedAction);
            }

            if (!IsListeningTo(quantity.OnInvalidAmount))
            {
                quantity.AddListener(quantity.OnInvalidAmount, _onQuantityInvalidAmountAction);
            }
        }

        private void StopListeningTo(QuantityBhv quantity)
        {
            if (quantity == null)
            {
                return;
            }

            int index;

            if (IsListeningTo(quantity.OnTypeChanged, out index))
            {
                quantity.RemoveListener(quantity.OnTypeChanged, index);
            }

            if (IsListeningTo(quantity.OnAmountChanged, out index))
            {
                quantity.RemoveListener(quantity.OnAmountChanged, index);
            }

            if (IsListeningTo(quantity.OnInvalidAmount, out index))
            {
                quantity.RemoveListener(quantity.OnInvalidAmount, index);
            }
        }

        private bool IsListeningTo(UnityEvent unityEvent)
        {
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                if (unityEvent.GetPersistentTarget(i) == this)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsListeningTo(UnityEvent unityEvent, out int index)
        {
            index = -1;

            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                if (unityEvent.GetPersistentTarget(i) == this)
                {
                    index = i;

                    return true;
                }
            }

            return false;
        }
    }
}