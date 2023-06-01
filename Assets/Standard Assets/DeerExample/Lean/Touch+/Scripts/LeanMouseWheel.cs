using UnityEngine;
using UnityEngine.Events;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to add mouse wheel control to other components.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanMouseWheel")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Mouse Wheel")]
	public class LeanMouseWheel : MonoBehaviour
	{
		public enum ModifyType
		{
			None,
			Sign
		}

		public enum CoordinateType
		{
			ZeroBased,
			OneBased
		}

		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>Do nothing if this LeanSelectable isn't selected?</summary>
		public LeanSelectable RequiredSelectable { set { requiredSelectable = value; } get { return requiredSelectable; } } [FSA("RequiredSelectable")] [SerializeField] private LeanSelectable requiredSelectable;

		/// <summary>When using simulated fingers, should they be created from a specific mouse button?
		/// -1 = Ignore.
		/// 0 = Left Mouse.
		/// 1 = Right Mouse.
		/// 2 = Middle Mouse.</summary>
		public int RequiredMouseButton { set { requiredMouseButton = value; } get { return requiredMouseButton; } } [FSA("RequiredMouseButton")] [SerializeField] private int requiredMouseButton = -1;

		/// <summary>Should the scroll delta be modified before use?
		/// Sign = The swipe delta will either be 1 or -1.</summary>
		public ModifyType Modify { set { modify = value; } get { return modify; } } [FSA("Modify")] [SerializeField] private ModifyType modify;

		/// <summary>This final delta value will be multiplied by this.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [FSA("Multiplier")] [SerializeField] private float multiplier = 1.0f;

		/// <summary>The coordinate space of the output delta values.
		/// ZeroBased = Scrolling where 0 means no scroll.
		/// OneBased = ZeroBased + 1. Scrolling where 1 means no scroll. This is suitable for use with components where you multiply a value.</summary>
		public CoordinateType Coordinate { set { coordinate = value; } get { return coordinate; } } [FSA("Coordinate")] [SerializeField] private CoordinateType coordinate;

		/// <summary>Called when the mouse scrolls.
		/// Float = Scroll delta.</summary>
		public FloatEvent OnDelta { get { if (onDelta == null) onDelta = new FloatEvent(); return onDelta; } } [SerializeField] private FloatEvent onDelta;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			requiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif

		protected virtual void Awake()
		{
			if (requiredSelectable == null)
			{
				requiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void Update()
		{
			if (requiredSelectable != null && requiredSelectable.IsSelected == false)
			{
				return;
			}

			if (requiredMouseButton >= 0 && LeanInput.GetMousePressed(requiredMouseButton) == false)
			{
				return;
			}

			var finalDelta = LeanInput.GetMouseWheelDelta();

			if (finalDelta == 0.0f)
			{
				return;
			}

			switch (modify)
			{
				case ModifyType.Sign:
				{
					finalDelta = Mathf.Sign(finalDelta);
				}
				break;
			}

			finalDelta *= multiplier;

			switch (coordinate)
			{
				case CoordinateType.OneBased:
				{
					finalDelta += 1.0f;
				}
				break;
			}

			if (onDelta != null)
			{
				onDelta.Invoke(finalDelta);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanMouseWheel;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanMouseWheel_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("requiredSelectable", "Do nothing if this LeanSelectable isn't selected?");
			Draw("requiredMouseButton", "When using simulated fingers, should they be created from a specific mouse button?\n\n-1 = Ignore.\n\n0 = Left Mouse.\n\n1 = Right Mouse.\n\n2 = Middle Mouse.");
			Draw("modify", "Should the scroll delta be modified before use?\n\nSign = The swipe delta will either be 1 or -1.");
			Draw("multiplier", "This final delta value will be multiplied by this.");
			Draw("coordinate", "The coordinate space of the output delta values.\n\nZeroBased = Scrolling where 0 means no scroll.\n\nOneBased = ZeroBased + 1. Scrolling where 1 means no scroll. This is suitable for use with components where you multiply a value.");
			
			Separator();
			
			Draw("onDelta");
		}
	}
}
#endif