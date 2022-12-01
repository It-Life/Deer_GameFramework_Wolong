using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component will constrain the current Transform.rotation values so that its facing direction doesn't deviate too far from the target direction.</summary>
	[DefaultExecutionOrder(200)]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanConstrainToDirection")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Constrain To Direction")]
	public class LeanConstrainToDirection : MonoBehaviour
	{
		/// <summary>This allows you to specify which local direction is considered forward on this GameObject.
		/// Leave this as the default (0,0,1) if you're not sure.</summary>
		public Vector3 Forward { set { forward = value; } get { return forward; } } [FSA("Forward")] [SerializeField] private Vector3 forward = Vector3.forward;

		/// <summary>This allows you to specify the target direction you want to constrain to. For example, (0,1,0) is up.</summary>
		public Vector3 Direction { set { direction = value; } get { return direction; } } [FSA("Direction")] [SerializeField] private Vector3 direction = Vector3.forward;

		/// <summary>If you want to constrain the direction relative to a Transform, you can specify it here.</summary>
		public Transform RelativeTo { set { relativeTo = value; } get { return relativeTo; } } [FSA("relativeTo")] [SerializeField] private Transform relativeTo;

		/// <summary>This allows you to specify the minimum angle delta between the Forward and Direction vectors in degrees.</summary>
		public float MinAngle { set { minAngle = value; } get { return minAngle; } } [FSA("MinAngle")] [SerializeField] [Range(0.0f, 180.0f)] public float minAngle = 0.0f;

		/// <summary>This allows you to specify the maximum angle delta between the Forward and Direction vectors in degrees.</summary>
		public float MaxAngle { set { maxAngle = value; } get { return maxAngle; } } [FSA("MaxAngle")] [SerializeField] [Range(0.0f, 180.0f)] public float maxAngle = 90.0f;

		protected virtual void LateUpdate()
		{
			if (forward != Vector3.zero && direction != Vector3.zero)
			{
				var dir = direction;

				if (relativeTo != null)
				{
					dir = relativeTo.TransformDirection(dir);
				}

				var fwd         = transform.TransformDirection(forward);
				var angle       = Vector3.Angle(dir, fwd);
				var oldRotation = transform.rotation;
				var newRotation = oldRotation;

				if (angle < minAngle)
				{
					var fixedFwd = Vector3.RotateTowards(fwd.normalized, -dir.normalized, (minAngle - angle) * Mathf.Deg2Rad, 1.0f);

					newRotation = Quaternion.FromToRotation(fwd, fixedFwd) * oldRotation;
				}
				else if (angle > maxAngle)
				{
					var fixedFwd = Vector3.RotateTowards(fwd.normalized, dir.normalized, (angle - maxAngle) * Mathf.Deg2Rad, 1.0f);

					newRotation = Quaternion.FromToRotation(fwd, fixedFwd) * oldRotation;
				}

				if (Mathf.Approximately(oldRotation.x, newRotation.x) == false ||
					Mathf.Approximately(oldRotation.y, newRotation.y) == false ||
					Mathf.Approximately(oldRotation.z, newRotation.z) == false ||
					Mathf.Approximately(oldRotation.w, newRotation.w) == false)
				{
					transform.rotation = newRotation;
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			if (forward != Vector3.zero)
			{
				var fwd = transform.TransformDirection(forward);

				Gizmos.DrawLine(transform.position, fwd);
			}

			if (direction != Vector3.zero)
			{
				var dir = direction;

				if (relativeTo != null)
				{
					dir = relativeTo.TransformDirection(dir);
				}

				Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
				
				DrawCone(dir, minAngle * Mathf.Deg2Rad);
				DrawCone(dir, maxAngle * Mathf.Deg2Rad);
			}
		}

		private void DrawCone(Vector3 dir, float angle)
		{
			for (var i = 0; i < 360; i++)
			{
				var a = (Mathf.PI * 2.0f / 360.0f) * i;
				var x = Mathf.Sin(a);
				var y = Mathf.Cos(a);
				var d = Mathf.Cos(angle);
				var r = Mathf.Sin(angle) * dir.magnitude;

				Gizmos.DrawLine(transform.position, transform.position + dir * d + new Vector3(x * r, y * r, 0.0f));
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanConstrainToDirection;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanConstrainToDirection_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("forward", "This allows you to specify which local direction is considered forward on this GameObject.\n\nLeave this as the default (0,0,1) if you're not sure.");
			Draw("direction", "This allows you to specify the target direction you want to constrain to. For example, (0,1,0) is up.");
			Draw("relativeTo", "If you want to constrain the direction relative to a Transform, you can specify it here.");
			Draw("minAngle", "This allows you to specify the minimum angle delta between the Forward and Direction vectors in degrees.");
			Draw("maxAngle", "This allows you to specify the maximum angle delta between the Forward and Direction vectors in degrees.");
		}
	}
}
#endif