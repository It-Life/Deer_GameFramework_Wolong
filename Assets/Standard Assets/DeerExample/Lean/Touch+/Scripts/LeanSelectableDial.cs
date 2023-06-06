using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This script allows you to twist the selected object around like a dial or knob.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectableDial")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Dial")]
	public class LeanSelectableDial : LeanSelectableByFingerBehaviour
	{
		[System.Serializable]
		public class Trigger
		{
			[Tooltip("The central Angle of this trigger in degrees.")]
			public float Angle;

			[Tooltip("The angle range of this trigger in degrees.\n\n90 = Quarter circle.\n180 = Half circle.")]
			public float Arc;

			[HideInInspector]
			public bool Inside;

			public UnityEvent OnEnter { get { if (onEnter == null) onEnter = new UnityEvent(); return onEnter; } } [SerializeField] private UnityEvent onEnter;

			public UnityEvent OnExit { get { if (onExit == null) onExit = new UnityEvent(); return onExit; } } [SerializeField] private UnityEvent onExit;

			public bool IsInside(float angle, bool clamp)
			{
				var range = Arc * 0.5f;

				if (clamp == false)
				{
					var delta  = Mathf.Abs(Mathf.DeltaAngle(Angle, angle));

					return delta < range;
				}

				return angle >= Angle - range && angle <= Angle + range;
			}
		}

		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>The camera this component will calculate using.
		/// None/null = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [FSA("Camera")] [SerializeField] private Camera _camera;

		/// <summary>The base rotation in local space.</summary>
		public Vector3 Tilt { set { tilt = value; } get { return tilt; } } [FSA("Tilt")] [SerializeField] private Vector3 tilt;

		/// <summary>The axis of the rotation in local space.</summary>
		public Vector3 Axis { set { axis = value; } get { return axis; } } [FSA("Axis")] [SerializeField] private Vector3 axis = Vector3.up;

		/// <summary>The angle of the dial in degrees.</summary>
		public float Angle { set { var newAngle = value; if (clamp == true) { newAngle = Mathf.Clamp(newAngle, clampMin, clampMax); } if (angle != newAngle) { angle = newAngle; if (onAngleChanged != null) onAngleChanged.Invoke(angle); } } get { return angle; } } [FSA("Angle")] [SerializeField] private float angle;

		/// <summary>Should the Angle value be clamped?</summary>
		public bool Clamp { set { clamp = value; } get { return clamp; } } [FSA("Clamp")] [SerializeField] private bool clamp;

		/// <summary>The minimum Angle value.</summary>
		public float ClampMin { set { clampMin = value; } get { return clampMin; } } [FSA("ClampMin")] [SerializeField] private float clampMin = -45.0f;

		/// <summary>The maximum Angle value.</summary>
		public float ClampMax { set { clampMax = value; } get { return clampMax; } } [FSA("ClampMax")] [SerializeField] private float clampMax = 45.0f;

		/// <summary>This allows you to perform a custom event when the dial is within a specified angle range.</summary>
		public List<Trigger> Triggers { get { if (triggers == null) triggers = new List<Trigger>(); return triggers; } } [FSA("Triggers")] [SerializeField] private List<Trigger> triggers;

		/// <summary>This event is invoked when the <b>Angle</b> changes.
		/// Float = Current Angle.</summary>
		public FloatEvent OnAngleChanged { get { if (onAngleChanged == null) onAngleChanged = new FloatEvent(); return onAngleChanged; } } [SerializeField] private FloatEvent onAngleChanged;

		private Vector2 oldPoint;

		private bool oldPointSet;

		[System.NonSerialized]
		private Canvas cachedCanvas;

		/// <summary>This method allows you to increase the <b>Angle</b> value from an external event (e.g. UI button click).</summary>
		public void IncrementAngle(float delta)
		{
			Angle += delta;
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.DrawLine(transform.position, transform.TransformPoint(axis));
		}
#endif

		protected virtual void Update()
		{
			var newAngle = angle;

			// Reset rotation and get axis
			transform.localEulerAngles = tilt;

			// Is this GameObject selected?
			if (Selectable != null && Selectable.IsSelected == true)
			{
				// Does it have a selected finger?
				var finger = Selectable.SelectingFinger;

				if (finger != null)
				{
					var newPoint = GetPoint(finger.ScreenPosition);

					if (oldPointSet == true)
					{
						newAngle -= Vector2.SignedAngle(newPoint, oldPoint);
					}

					oldPoint    = newPoint;
					oldPointSet = true;
				}
			}
			else
			{
				oldPointSet = false;
			}

			if (clamp == true)
			{
				newAngle = Mathf.Clamp(newAngle, clampMin, clampMax);
			}

			transform.Rotate(axis, angle, Space.Self);

			if (triggers != null)
			{
				for (var i = 0; i < triggers.Count; i++)
				{
					var trigger = triggers[i];

					if (trigger.IsInside(angle, clamp) == true)
					{
						if (trigger.Inside == false)
						{
							trigger.Inside = true;

							trigger.OnEnter.Invoke();
						}
					}
					else
					{
						if (trigger.Inside == true)
						{
							trigger.Inside = false;

							trigger.OnExit.Invoke();
						}
					}
				}
			}

			Angle = newAngle;
		}

		private Vector2 GetPoint(Vector2 screenPoint)
		{
			var rectTransform = transform as RectTransform;

			if (rectTransform != null)
			{
				var worldPoint = default(Vector3);

				if (cachedCanvas == null)
				{
					cachedCanvas = GetComponentInParent<Canvas>();
				}

				if (cachedCanvas != null)
				{
					if (cachedCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
					{
						if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, default(Camera), out worldPoint) == true)
						{
							return Quaternion.LookRotation(axis) * transform.InverseTransformPoint(worldPoint);
						}
					}
					else
					{
						if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, cachedCanvas.worldCamera, out worldPoint) == true)
						{
							return Quaternion.LookRotation(axis) * transform.InverseTransformPoint(worldPoint);
						}
					}
				}
			}
			else
			{
				// Make sure the camera exists
				var camera = LeanHelper.GetCamera(_camera, gameObject);

				if (camera != null)
				{
					var ray      = camera.ScreenPointToRay(screenPoint);
					var plane    = new Plane(transform.TransformDirection(axis), transform.position);
					var distance = default(float);

					if (plane.Raycast(ray, out distance) == true)
					{
						return Quaternion.Inverse(Quaternion.LookRotation(axis)) * transform.InverseTransformPoint(ray.GetPoint(distance));
					}
				}
				else
				{
					Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
				}
			}

			return oldPoint;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSelectableDial;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectableDial_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("_camera", "The camera we will be used.\n\nNone/null = MainCamera.");
			Draw("tilt", "The base rotation in local space.");
			Draw("axis", "The axis of the rotation in local space.");
			Draw("angle", "The angle of the dial in degrees.");

			Separator();

			Draw("clamp", "Should the Angle value be clamped?");
			BeginIndent();
				Draw("clampMin", "The minimum Angle value.", "Min");
				Draw("clampMax", "The maximum Angle value.", "Max");
			EndIndent();

			Separator();

			Draw("triggers", "This allows you to perform a custom event when the dial is within a specified angle range.");

			Separator();

			Draw("onAngleChanged");
		}
	}
}
#endif