using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component automatically rotates the current GameObject based on movement.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanRotateToPosition")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Rotate To Position")]
	public class LeanRotateToPosition : MonoBehaviour
	{
		public enum PositionType
		{
			PreviousPosition,
			ManuallySetPosition
		}

		public enum RotateType
		{
			None,
			Forward,
			TopDown,
			Side2D
		}

		/// <summary>The <b>Transform</b> that will be rotated.
		/// None/Null = This GameObject's Transform.</summary>
		public Transform Target { set { target = value; } get { return target; } } [FSA("Target")] [SerializeField] private Transform target;

		/// <summary>This allows you choose the method used to calculate the position we will rotate toward.
		/// PreviousPosition = This component will automatically calculate positions based on the <b>Transform.position</b>.
		/// ManuallySetPosition = You must manually call the <b>SetPosition</b> method to update the rotation.</summary>
		public PositionType Position { set { position = value; } get { return position; } } [FSA("Position")] [SerializeField] private PositionType position;

		/// <summary>This allows you to set the minimum amount of movement required to trigger the rotation to update. This is useful to prevent tiny movements from causing the rotation to change unexpectedly.</summary>
		public float Threshold { set { threshold = value; } get { return threshold; } } [FSA("Threshold")] [SerializeField] private float threshold = 0.1f;

		/// <summary>If you enable this the rotation will be reversed.</summary>
		public bool Invert { set { invert = value; } get { return invert; } } [FSA("Invert")] [SerializeField] private bool invert;

		/// <summary>This allows you choose the method used to find the target rotation.</summary>
		public RotateType RotateTo { set { rotateTo = value; } get { return rotateTo; } } [FSA("RotateTo")] [SerializeField] private RotateType rotateTo;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Dampening")] [FSA("Damping")] [SerializeField] private float damping = 10.0f;

		[SerializeField]
		private Vector3 previousPosition;

		[SerializeField]
		private Vector3 previousDelta;

		private Transform FinalTransform
		{
			get
			{
				return target != null ? target : transform;
			}
		}

		/// <summary>If <b>Position</b> is set to <b>ManuallySetPosition</b>, then this method allows you to set the position we will rotate to.</summary>
		public void SetPosition(Vector3 position)
		{
			var currentPosition = FinalTransform.position;

			if (Vector3.Distance(currentPosition, position) > threshold)
			{
				SetDelta(position - currentPosition);
			}
		}

		/// <summary>This method allows you to override the position delta used to calculate the rotation.
		/// NOTE: This should be non-zero.</summary>
		public void SetDelta(Vector3 delta)
		{
			if (delta.sqrMagnitude > 0.0f)
			{
				previousDelta = delta;
			}
		}

		/// <summary>If your <b>Transform</b> has teleported, then call this to reset the cached position.</summary>
		public void ResetPosition()
		{
			previousPosition = FinalTransform.position;
		}

		protected virtual void Start()
		{
			ResetPosition();
		}

		protected virtual void OnEnable()
		{
			ResetPosition();
		}

		protected virtual void LateUpdate()
		{
			// Cache
			var finalTransform = FinalTransform;

			// Update position and delta
			var currentPosition = finalTransform.position;

			if (position == PositionType.PreviousPosition && Vector3.Distance(previousPosition, currentPosition) > threshold)
			{
				SetDelta(currentPosition - previousPosition);

				previousPosition = currentPosition;
			}

			// Update rotation
			var currentRotation = finalTransform.localRotation;
			var factor          = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			if (previousDelta.sqrMagnitude > 0.0f)
			{
				UpdateRotation(finalTransform, previousDelta);
			}

			finalTransform.localRotation = Quaternion.Slerp(currentRotation, finalTransform.localRotation, factor);
		}

		private void UpdateRotation(Transform finalTransform, Vector3 vector)
		{
			if (invert == true)
			{
				vector = -vector;
			}

			switch (rotateTo)
			{
				case RotateType.Forward:
				{
					finalTransform.forward = vector;
				}
				break;

				case RotateType.TopDown:
				{
					var yaw = Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg;

					finalTransform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
				}
				break;

				case RotateType.Side2D:
				{
					var roll = Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;

					finalTransform.rotation = Quaternion.Euler(0.0f, 0.0f, -roll);
				}
				break;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanRotateToPosition;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanRotateToPosition_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("target", "The <b>Transform</b> that will be rotated.\n\nNone/Null = This GameObject's Transform.");
			Draw("position", "This allows you choose the method used to calculate the position we will rotate toward.\n\nPreviousPosition = This component will automatically calculate positions based on the <b>Transform.position</b>.\n\nManuallySetPosition = You must manually call the <b>SetPosition</b> method to update the rotation.");
			Draw("threshold", "This allows you to set the minimum amount of movement required to trigger the rotation to update. This is useful to prevent tiny movements from causing the rotation to change unexpectedly.");
			Draw("invert", "If you enable this the rotation will be reversed.");
			Draw("rotateTo", "This allows you choose the method used to find the target rotation.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
		}
	}
}
#endif