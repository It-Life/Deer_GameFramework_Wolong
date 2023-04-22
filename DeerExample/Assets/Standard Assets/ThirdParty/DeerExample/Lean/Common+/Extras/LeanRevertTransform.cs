using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This script will record the state of the current transform, and revert it on command.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanRevertTransform")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Revert Transform")]
	public class LeanRevertTransform : MonoBehaviour
	{
		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Dampening")] [FSA("Damping")] [SerializeField] private float damping = -1.0f;

		/// <summary>Call RecordTransform in Start?</summary>
		public bool RecordOnStart { set { recordOnStart = value; } get { return recordOnStart; } } [FSA("RecordOnStart")] [SerializeField] private bool recordOnStart = true;

		public bool RevertPosition { set { revertPosition = value; } get { return revertPosition; } } [FSA("RevertPosition")] [SerializeField] private bool revertPosition = true;

		public bool RevertRotation { set { revertRotation = value; } get { return revertRotation; } } [FSA("RevertRotation")] [SerializeField] private bool revertRotation = true;

		public bool RevertScale { set { revertScale = value; } get { return revertScale; } } [FSA("RevertScale")] [SerializeField] private bool revertScale = true;

		public float ThresholdPosition { set { thresholdPosition = value; } get { return thresholdPosition; } } [FSA("ThresholdPosition")] [SerializeField] private float thresholdPosition = 0.01f;

		public float ThresholdRotation { set { thresholdRotation = value; } get { return thresholdRotation; } } [FSA("ThresholdRotation")] [SerializeField] private float thresholdRotation = 0.01f;

		public float ThresholdScale { set { thresholdScale = value; } get { return thresholdScale; } } [FSA("ThresholdScale")] [SerializeField] private float thresholdScale = 0.01f;

		public Vector3 TargetPosition { set { targetPosition = value; } get { return targetPosition; } } [FSA("TargetPosition")] [SerializeField] private Vector3 targetPosition;

		public Quaternion TargetRotation { set { targetRotation = value; } get { return targetRotation; } } [FSA("TargetRotation")] [SerializeField] private Quaternion targetRotation = Quaternion.identity;

		public Vector3 TargetScale { set { targetScale = value; } get { return targetScale; } } [FSA("TargetScale")] [SerializeField] private Vector3 targetScale = Vector3.one;

		[SerializeField]
		private bool reverting;

		protected virtual void Start()
		{
			if (recordOnStart == true)
			{
				RecordTransform();
			}
		}

		[ContextMenu("Revert")]
		public void Revert()
		{
			reverting = true;
		}

		[ContextMenu("Stop Revert")]
		public void StopRevert()
		{
			reverting = false;
		}

		[ContextMenu("Record Transform")]
		public void RecordTransform()
		{
			targetPosition = transform.localPosition;
			targetRotation = transform.localRotation;
			targetScale    = transform.localScale;
		}

		protected virtual void Update()
		{
			if (reverting == true)
			{
				if (ReachedTarget() == true)
				{
					reverting = false;

					return;
				}

				// Get t value
				var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

				if (revertPosition == true)
				{
					transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, factor);
				}

				if (revertRotation == true)
				{
					transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, factor);
				}

				if (revertScale == true)
				{
					transform.localScale = Vector3. Lerp(transform.localScale, targetScale, factor);
				}
			}
		}

		private bool ReachedTarget()
		{
			if (revertPosition == true && Vector3.Distance(transform.localPosition, targetPosition) > thresholdPosition)
			{
				return false;
			}

			if (revertRotation == true && Quaternion.Angle(transform.localRotation, targetRotation) > thresholdRotation)
			{
				return false;
			}

			if (revertScale == true && Vector3.Distance(transform.localScale, targetScale) > thresholdScale)
			{
				return false;
			}

			return true;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanRevertTransform;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanRevertTransform_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("recordOnStart", "Call RecordTransform in Start?");
			Draw("revertPosition");
			Draw("revertRotation");
			Draw("revertScale");
			Draw("thresholdPosition");
			Draw("thresholdRotation");
			Draw("thresholdScale");
			Draw("targetPosition");
			Draw("targetRotation");
			Draw("targetScale");
		}
	}
}
#endif