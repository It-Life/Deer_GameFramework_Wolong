using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component controls the current GameObject's rotation, based on the specified Pitch and Yaw values.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanOrbit")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Orbit")]
	public class LeanOrbit : MonoBehaviour
	{
		/// <summary>If you want the rotation to be scaled by the camera FOV, then set the camera here.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [FSA("Camera")] [SerializeField] private Camera _camera;

		/// <summary>The camera will orbit around this point.</summary>
		public Transform Pivot { set { pivot = value; } get { return pivot; } } [FSA("Pivot")] [SerializeField] private Transform pivot;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Dampening")] [FSA("Damping")] [SerializeField] private float damping = -1.0f;

		/// <summary>The strength of the pitch changes with vertical finger movement.</summary>
		public float PitchSensitivity { set { pitchSensitivity = value; } get { return pitchSensitivity; } } [FSA("PitchSensitivity")] [SerializeField] private float pitchSensitivity = 0.25f;

		/// <summary>The strength of the yaw changes with horizontal finger movement.</summary>
		public float YawSensitivity { set { yawSensitivity = value; } get { return yawSensitivity; } } [FSA("YawSensitivity")] [SerializeField] private float yawSensitivity = 0.25f;

		[SerializeField]
		private Vector3 remainingDelta;

		public void Rotate(Vector2 delta)
		{
			var sensitivity = GetSensitivity();

			delta.x *= pitchSensitivity * sensitivity;
			delta.y *=   yawSensitivity * sensitivity;

			RotatePitch(-delta.y);
			RotateYaw  ( delta.x);
		}

		public Vector3 GetPivotPoint()
		{
			if (pivot != null)
			{
				return pivot.position;
			}

			return Vector3.zero;
		}

		public void RotatePitch(float delta)
		{
			delta *= pitchSensitivity * GetSensitivity();

			var oldPosition = transform.position;
			var pivotPoint  = GetPivotPoint();
			var angles      = Quaternion.LookRotation(pivotPoint - transform.position, Vector3.up).eulerAngles;

			var pitch = Mathf.DeltaAngle(0.0f, angles.x);
			var yaw   = angles.y;

			if (pitch + delta < -89.0f) delta = -89.0f - pitch;
			if (pitch + delta >  89.0f) delta =  89.0f - pitch;

			transform.position += remainingDelta;

			transform.RotateAround(pivotPoint, Quaternion.Euler(0.0f, yaw, 0.0f) * Vector3.right, delta);

			if (damping >= 0.0f)
			{
				remainingDelta += transform.position - (oldPosition + remainingDelta);

				transform.position = oldPosition;
			}
			else
			{
				remainingDelta = Vector3.zero;
			}
		}

		public void RotateYaw(float delta)
		{
			delta *= yawSensitivity * GetSensitivity();

			var oldPosition = transform.position;

			transform.position += remainingDelta;

			transform.RotateAround(pivot.position, Vector3.up, delta);

			if (damping >= 0.0f)
			{
				remainingDelta += transform.position - (oldPosition + remainingDelta);

				transform.position = oldPosition;
			}
			else
			{
				remainingDelta = Vector3.zero;
			}
		}

		protected virtual void LateUpdate()
		{
			// Get t value
			var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			transform.position += remainingDelta - newDelta;

			if (pivot != null)
			{
				transform.LookAt(pivot, Vector3.up);
			}

			remainingDelta = newDelta;
		}

		private float GetSensitivity()
		{
			// Has a camera been set?
			if (_camera != null)
			{
				// Adjust sensitivity by FOV?
				if (_camera.orthographic == false)
				{
					return _camera.fieldOfView / 90.0f;
				}
			}

			return 1.0f;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanOrbit;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanOrbit_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("_camera", "If you want the rotation to be scaled by the camera FOV, then set the camera here.");
			Draw("pivot", "The camera will orbit around this point.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");

			Draw("pitchSensitivity", "The strength of the pitch changes with vertical finger movement.");
			Draw("yawSensitivity", "The strength of the yaw changes with horizontal finger movement.");
		}
	}
}
#endif