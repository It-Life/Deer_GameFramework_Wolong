using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component scales the current selectable based on the selecting finger pressure.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectablePressureScale")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Pressure Scale")]
	public class LeanSelectablePressureScale : LeanSelectableByFingerBehaviour
	{
		/// <summary>The default scale with no pressure.</summary>
		public Vector3 BaseScale { set { baseScale = value; } get { return baseScale; } } [FSA("BaseScale")] [SerializeField] private Vector3 baseScale = Vector3.one;

		/// <summary>The amount BaseScale gets multiplied based on the finger pressure.</summary>
		public float PressureMultiplier { set { pressureMultiplier = value; } get { return pressureMultiplier; } } [FSA("PressureMultiplier")] [SerializeField] private float pressureMultiplier = 0.25f;

		/// <summary>Limit pressure to a range of 0..1?</summary>
		public bool PressureClamp { set { pressureClamp = value; } get { return pressureClamp; } } [FSA("PressureClamp")] [SerializeField] private bool pressureClamp;

		protected virtual void Update()
		{
			// Get pressure
			var pressure = 0.0f;

			if (Selectable != null && Selectable.SelectingFinger != null)
			{
				pressure = Selectable.SelectingFinger.Pressure;
			}

			// Clamp?
			if (pressureClamp == true)
			{
				pressure = Mathf.Clamp01(pressure);
			}

			transform.localScale = baseScale + baseScale * pressure * pressureMultiplier;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSelectablePressureScale;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanSelectablePressureScale_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("baseScale", "The default scale with no pressure.");
			Draw("pressureMultiplier", "The amount BaseScale gets multiplied based on the finger pressure.");
			Draw("pressureClamp", "Limit pressure to a range of 0..1?");
		}
	}
}
#endif