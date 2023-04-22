using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component causes the current Transform to chase the specified position.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanChase")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Chase")]
	public class LeanChase : MonoBehaviour
	{
		/// <summary>The transform that will be chased.</summary>
		public Transform Destination { set { destination = value; } get { return destination; } } [FSA("Destination")] public Transform destination;

		/// <summary>If Target is set, then this allows you to set the offset in local space.</summary>
		public Vector3 DestinationOffset { set { destinationOffset = value; } get { return destinationOffset; } } [FSA("DestinationOffset")] [SerializeField] protected Vector3 destinationOffset;

		/// <summary>The world space position that will be followed.
		/// NOTE: If Destination is set, then this value will be overridden.</summary>
		public Vector3 Position { set { position = value; } get { return position; } } [FSA("Position")] [SerializeField] protected Vector3 position;

		/// <summary>The world space offset that will be followed.</summary>
		public Vector3 Offset { set { offset = value; } get { return offset; } } [FSA("Offset")] [SerializeField] protected Vector3 offset;

		/// <summary>This allows you to control how quickly this Transform moves to the target position.
		/// -1 = Instantly change.
		/// 0 = No change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Dampening")] [FSA("Damping")] [SerializeField] private float damping = -1.0f;

		/// <summary>This allows you to control how quickly this Transform linearly moves to the target position.
		/// 0 = No linear movement.
		/// 1 = One linear position per second.</summary>
		public float Linear { set { linear = value; } get { return linear; } } [FSA("Linear")] [SerializeField] private float linear;

		/// <summary>Ignore Z for 2D?</summary>
		public bool IgnoreZ { set { ignoreZ = value; } get { return ignoreZ; } } [FSA("IgnoreZ")] [SerializeField] protected bool ignoreZ;

		/// <summary>Should the chase keep updating, even if you haven't called the SetPosition method this frame?</summary>
		public bool Continuous { set { continuous = value; } get { return continuous; } } [FSA("Continuous")] [SerializeField] protected bool continuous;

		/// <summary>Automatically set the Position value to the transform.position in Start?</summary>
		public bool SetPositionOnStart { set { setPositionOnStart = value; } get { return setPositionOnStart; } } [FSA("SetPositionOnStart")] [SerializeField] private bool setPositionOnStart = true;

		[System.NonSerialized]
		protected bool positionSet;

		/// <summary>This method will override the Position value based on the specified value.</summary>
		public virtual void SetPosition(Vector3 newPosition)
		{
			destination = null;
			position    = newPosition;
			positionSet = true;
		}

		/// <summary>This method will immediately move this Transform to the target position.</summary>
		[ContextMenu("Snap To Position")]
		public void SnapToPosition()
		{
			UpdatePosition(-1.0f, 0.0f);
		}

		protected virtual void Start()
		{
			if (setPositionOnStart == true)
			{
				position = transform.position;
			}
		}

		protected virtual void Update()
		{
			UpdatePosition(damping, linear);
		}

		protected virtual void UpdatePosition(float damping, float linear)
		{
			if (positionSet == true || continuous == true)
			{
				if (destination != null)
				{
					position = destination.TransformPoint(destinationOffset);
				}

				var currentPosition = transform.position;
				var targetPosition  = position + offset;

				if (ignoreZ == true)
				{
					targetPosition.z = currentPosition.z;
				}

				// Get t value
				var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

				// Move current value to the target one
				currentPosition = Vector3.Lerp(currentPosition, targetPosition, factor);
				currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, linear * Time.deltaTime);

				// Apply new point
				transform.position = currentPosition;

				positionSet = false;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanChase;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanChase_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("destination", "The transform that will be chased.");
			Draw("destinationOffset", "If Target is set, then this allows you to set the offset in local space.");

			Separator();

			Draw("position", "The world space position that will be followed.\n\nNOTE: If Destination is set, then this value will be overridden.");
			Draw("offset", "The world space offset that will be followed.");

			Separator();

			Draw("damping", "This allows you to control how quickly this Transform moves to the target position.\n\n-1 = Instantly change.\n\n0 = No change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("linear", "This allows you to control how quickly this Transform linearly moves to the target position.\n\n0 = No linear movement.\n\n1 = One linear position per second.");
			Draw("ignoreZ", "Ignore Z for 2D?");
			Draw("continuous", "Should the chase keep updating, even if you haven't called the SetPosition method this frame?");
			Draw("setPositionOnStart", "Automatically set the Position value to the transform.position in Start?");
		}
	}
}
#endif