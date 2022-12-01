using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.Quantity
{
    [DisallowMultipleComponent]
    public class QuantityBhv : MonoBehaviour
    {
        // Constants fields
        private const float MAX_SEGMENT_COUNT = 50;

        // Public properties
        public QuantityType Type
        {
            get
            {
                return _quantityType;
            }

            set
            {
                _quantityType = value;
                _onTypeChanged.Invoke();
            }
        }
        public float MaximumAmount
        {
            get
            {
                return _maximumAmount;
            }

            set
            {
                _maximumAmount = value;
                _minimumAmount = ValidateMinimumAmount(_minimumAmount);
                _segmentCount = ValidateSegmentCount(Capacity / _segmentAmount, out bool overflowing);
                _segmentAmount = overflowing ? ValidateSegmentAmount(Capacity / _segmentCount) : _segmentAmount;
                Amount = ValidateCurrentAmount(_currentAmount);
            }
        }
        public float MinimumAmount
        {
            get
            {
                return _minimumAmount;
            }

            set
            {
                _minimumAmount = value;
                _maximumAmount = ValidateMaximumAmount(_maximumAmount);
                _segmentCount = ValidateSegmentCount(Capacity / _segmentAmount, out bool overflowing);
                _segmentAmount = overflowing ? ValidateSegmentAmount(Capacity / _segmentCount) : _segmentAmount;
                Amount = ValidateCurrentAmount(_currentAmount);
            }
        }
        public float Capacity
        {
            get
            {
                return _maximumAmount - _minimumAmount;
            }
        }
        public float Amount
        {
            get
            {
                return _currentAmount;
            }

            set
            {
                float previousAmount = _currentAmount;
                _currentAmount = ValidateCurrentAmount(value);
                _fillAmount = (_currentAmount - _minimumAmount) / Capacity;
                _onAmountChanged.Invoke();
                if (_currentAmount != previousAmount && (value == _maximumAmount || value == _minimumAmount) || value > _maximumAmount || value < _minimumAmount)
                {
                    _onInvalidAmount.Invoke();
                }
            }
        }
        public float FillAmount
        {
            get
            {
                return _fillAmount;
            }

            set
            {
                _fillAmount = value;
                _currentAmount = _fillAmount * Capacity + _minimumAmount;
                _onAmountChanged.Invoke();
            }
        }
        public bool IsSegmented
        {
            get
            {
                return _isSegmented;
            }

            set
            {
                _isSegmented = value;
                _onAmountChanged.Invoke();
            }
        }
        public float SegmentAmount
        {
            get
            {
                return _segmentAmount;
            }

            set
            {
                _segmentAmount = ValidateSegmentAmount(value);
                _segmentCount = ValidateSegmentCount(Capacity / _segmentAmount, out bool overflowing);
                _onAmountChanged.Invoke();
            }
        }
        public int SegmentCount
        {
            get
            {
                return _segmentCount;
            }

            set
            {
                _segmentCount = ValidateSegmentCount(value, out bool overflowing);
                _segmentAmount = ValidateSegmentAmount(Capacity / _segmentCount);
                _onAmountChanged.Invoke();
            }
        }
        public Dynamics PassiveDynamics
        {
            get
            {
                return _passiveDynamics;
            }
        }
        public UnityEvent OnTypeChanged
        {
            get
            {
                return _onTypeChanged;
            }

            set
            {
                _onTypeChanged = value;
            }
        }
        public UnityEvent OnAmountChanged
        {
            get
            {
                return _onAmountChanged;
            }

            set
            {
                _onAmountChanged = value;
            }
        }
        public UnityEvent OnInvalidAmount
        {
            get
            {
                return _onInvalidAmount;
            }

            set
            {
                _onInvalidAmount = value;
            }
        }

        // Private fields
        [SerializeField] private QuantityType _quantityType;
        [SerializeField] private float _maximumAmount = 100;
        [SerializeField] private float _minimumAmount = 0;
        [SerializeField, ReadOnly] private float _currentAmount;
        [SerializeField, Range(0, 1)] private float _fillAmount = .5f;
        [SerializeField] private bool _isSegmented = true;
        [SerializeField] private float _segmentAmount = 25;
        [SerializeField, Range(1, MAX_SEGMENT_COUNT)] private int _segmentCount = 4;
        [SerializeField] private Dynamics _passiveDynamics;
        [SerializeField] private UnityEvent _onTypeChanged = new UnityEvent();
        [SerializeField] private UnityEvent _onAmountChanged = new UnityEvent();
        [SerializeField] private UnityEvent _onInvalidAmount = new UnityEvent();
        [SerializeField] private Coroutine _dynamicsCoroutine;

        private float ValidateMaximumAmount(float value)
        {
            return Mathf.Max(_minimumAmount + .01f, value);
        }
        private float ValidateMinimumAmount(float value)
        {
            return Mathf.Min(_maximumAmount - .01f, value);
        }
        private float ValidateCurrentAmount(float value)
        {
            return Mathf.Clamp(value, _minimumAmount, _maximumAmount);
        }
        private float ValidateSegmentAmount(float value)
        {
            return Mathf.Clamp(value, Capacity / MAX_SEGMENT_COUNT, Capacity);
        }
        private int ValidateSegmentCount(float value, out bool overflowing)
        {
            overflowing = value < 1 || value > MAX_SEGMENT_COUNT;

            return Mathf.CeilToInt(Mathf.Clamp(value, 1, MAX_SEGMENT_COUNT));
        }

        public void OnUndoRedoCallback()
        {
            MaximumAmount = MaximumAmount;
            MinimumAmount = MinimumAmount;
            FillAmount = FillAmount;
            IsSegmented = IsSegmented;
            SegmentAmount = SegmentAmount;
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                StartPassiveDynamics();
            }
        }

        public void StartPassiveDynamics()
        {
            StopPassiveDynamics();

            _dynamicsCoroutine = StartCoroutine(PassiveDynamicsRoutine());
        }

        public void StopPassiveDynamics()
        {
            if (_dynamicsCoroutine != null)
            {
                StopCoroutine(_dynamicsCoroutine);
            }
        }

        private IEnumerator PassiveDynamicsRoutine()
        {
            while (Application.isPlaying)
            {
                if (PassiveDynamics.Enabled)
                {
                    Amount = ValidateCurrentAmount(Amount + Capacity * PassiveDynamics.SignedDeltaPercentage);

                    yield return new WaitForSeconds(PassiveDynamics.DeltaTime);
                }

                else
                {
                    yield return null;
                }
            }
        }

        private void OnValidate()
        {
            this.PseudoResetOnCreation();
        }

        private void PseudoResetOnCreation()
        {
#if UNITY_EDITOR
            if (Event.current != null)
            {
                if (Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste")
                {
                    _onTypeChanged = new UnityEvent();
                    _onAmountChanged = new UnityEvent();
                    _onInvalidAmount = new UnityEvent();
                }
            }
#endif
        }

        #region UnityEvent Subscription Handling
        public void AddListener(UnityEvent unityEvent, UnityAction unityAction)
        {
#if UNITY_EDITOR
            UnityEventTools.AddPersistentListener(unityEvent, unityAction);

            unityEvent.SetPersistentListenerState(unityEvent.GetPersistentEventCount() - 1, UnityEventCallState.EditorAndRuntime);

            CleanUp(unityEvent);
#endif
        }

        public void RemoveListener(UnityEvent unityEvent, int index)
        {
#if UNITY_EDITOR
            UnityEventTools.RemovePersistentListener(unityEvent, index);

            CleanUp(unityEvent);
#endif
        }

        private void CleanUp(UnityEvent unityEvent)
        {
#if UNITY_EDITOR
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                if (unityEvent.GetPersistentTarget(i) == null)
                {
                    UnityEventTools.RemovePersistentListener(unityEvent, i);
                }
            }
#endif
        }
        #endregion
    }
}