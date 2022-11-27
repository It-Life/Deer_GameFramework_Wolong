using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component controls the current GameObject's rotation, based on the specified Pitch and Yaw values.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanPitchYaw")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Pitch Yaw")]
	public class LeanPitchYaw : MonoBehaviour
	{
		/// <summary>If you want the rotation to be scaled by the camera FOV, then set the camera here.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [FSA("Camera")] [SerializeField] private Camera _camera;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Dampening")] [FSA("Damping")] [SerializeField] private float damping = -1.0f;

		/// <summary>This allows you to set the Pitch andYaw rotation value when calling the ResetRotation method.</summary>
		public Vector2 DefaultRotation { set { defaultRotation = value; } get { return defaultRotation; } } [FSA("DefaultRotation")] [SerializeField] private Vector2 defaultRotation;

		/// <summary>Pitch of the rotation in degrees.</summary>
		public float Pitch { set { pitch = value; } get { return pitch; } } [FSA("Pitch")] [SerializeField] private float pitch;

		/// <summary>The strength of the pitch changes with vertical finger movement.</summary>
		public float PitchSensitivity { set { pitchSensitivity = value; } get { return pitchSensitivity; } } [FSA("PitchSensitivity")] [SerializeField] private float pitchSensitivity = 0.25f;

		/// <summary>Limit the pitch to min/max?</summary>
		public bool PitchClamp { set { pitchClamp = value; } get { return pitchClamp; } } [FSA("PitchClamp")] [SerializeField] private bool pitchClamp = true;

		/// <summary>The minimum pitch angle in degrees.</summary>
		public float PitchMin { set { pitchMin = value; } get { return pitchMin; } } [FSA("PitchMin")] [SerializeField] private float pitchMin = -90.0f;

		/// <summary>The maximum pitch angle in degrees.</summary>
		public float PitchMax { set { pitchMax = value; } get { return pitchMax; } } [FSA("PitchMax")] [SerializeField] private float pitchMax = 90.0f;

		/// <summary>Yaw of the rotation in degrees.</summary>
		public float Yaw { set { yaw = value; } get { return yaw; } } [FSA("Yaw")] [SerializeField] private float yaw;

		/// <summary>The strength of the yaw changes with horizontal finger movement.</summary>
		public float YawSensitivity { set { yawSensitivity = value; } get { return yawSensitivity; } } [FSA("YawSensitivity")] [SerializeField] private float yawSensitivity = 0.25f;

		/// <summary>Limit the yaw to min/max?</summary>
		public bool YawClamp { set { yawClamp = value; } get { return yawClamp; } } [FSA("YawClamp")] [SerializeField] private bool yawClamp;

		/// <summary>The minimum yaw angle in degrees.</summary>
		public float YawMin { set { yawMin = value; } get { return yawMin; } } [FSA("YawMin")] [SerializeField] private float yawMin = -45.0f;

		/// <summary>The maximum yaw angle in degrees.</summary>
		public float YawMax { set { yawMax = value; } get { return yawMax; } } [FSA("YawMax")] [SerializeField] private float yawMax = 45.0f;

		[SerializeField]
		private float currentPitch;

		[SerializeField]
		private float currentYaw;

		/// <summary>This method resets the Pitch and Yaw values to the DefaultRotation value.</summary>
		[ContextMenu("Reset Rotation")]
		public virtual void ResetRotation()
		{
			pitch = defaultRotation.x;
			yaw   = defaultRotation.y;
		}

		/// <summary>This method will automatically update the <b>Pitch</b> and <b>Yaw</b> values based on the specified position in world space.</summary>
		public void RotateToPosition(Vector3 point)
		{
			RotateToDirection(point - transform.position);
		}

		/// <summary>This method will automatically update the <b>Pitch</b> and <b>Yaw</b> values based on the specified direction in world space.</summary>
		public void RotateToDirection(Vector3 xyz)
		{
			var longitude = Mathf.Atan2(xyz.x, xyz.z);
			var latitude  = Mathf.Asin(xyz.y / xyz.magnitude);
			var newPitch  = latitude  * -Mathf.Rad2Deg;
			var newYaw    = longitude *  Mathf.Rad2Deg;
			var delta     = Mathf.DeltaAngle(yaw, newYaw);

			pitch = newPitch;
			yaw  += delta;
		}

		public void SetPitch(float newPitch)
		{
			pitch = newPitch;
		}

		public void SetYaw(float newYaw)
		{
			var delta = Mathf.DeltaAngle(yaw, newYaw);

			yaw  += delta;
		}

		/// <summary>This method will automatically update the <b>Pitch</b> and <b>Yaw</b> values based on the specified position in screen space.</summary>
		public void RotateToScreenPosition(Vector2 screenPosition)
		{
			var camera = LeanHelper.GetCamera(_camera, gameObject);

			if (camera != null)
			{
				var xyz = camera.ScreenPointToRay(screenPosition).direction;

				RotateToDirection(xyz);
			}
		}

		public void Rotate(Vector2 delta)
		{
			var sensitivity = GetSensitivity();

			yaw   += delta.x *   yawSensitivity * sensitivity;
			pitch -= delta.y * pitchSensitivity * sensitivity;
		}

		public void RotatePitch(float delta)
		{
			pitch -= delta * pitchSensitivity * GetSensitivity();
		}

		public void RotateYaw(float delta)
		{
			yaw += delta * yawSensitivity * GetSensitivity();
		}

		protected virtual void Start()
		{
			currentPitch = pitch;
			currentYaw   = yaw;
		}

		protected virtual void LateUpdate()
		{
			if (pitchClamp == true)
			{
				pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
			}

			if (yawClamp == true)
			{
				yaw = Mathf.Clamp(yaw, yawMin, yawMax);
			}

			// Get t value
			var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			// Lerp the current values to the target ones
			currentPitch = Mathf.Lerp(currentPitch, pitch, factor);
			currentYaw   = Mathf.Lerp(currentYaw  , yaw  , factor);

			// Rotate to pitch and yaw values
			transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
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
	using TARGET = LeanPitchYaw;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanPitchYaw_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("_camera", "If you want the rotation to be scaled by the camera FOV, then set the camera here.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("defaultRotation", "This allows you to set the Pitch andYaw rotation value when calling the ResetRotation method.");
			
			Separator();
			
			Draw("pitch", "Pitch of the rotation in degrees.");
			BeginIndent();
				Draw("pitchSensitivity", "The strength of the pitch changes with vertical finger movement.", "Sensitivity");
				Draw("pitchClamp", "Limit the pitch to min/max?", "Clamp");
				if (Any(tgts, t => t.PitchClamp == true))
				{
					BeginIndent();
						Draw("pitchMin", "The minimum pitch angle in degrees.", "Min");
						Draw("pitchMax", "The maximum pitch angle in degrees.", "Max");
					EndIndent();
				}
			EndIndent();

			Separator();

			Draw("yaw", "Yaw of the rotation in degrees.");
			BeginIndent();
				Draw("yawSensitivity", "The strength of the yaw changes with horizontal finger movement.", "Sensitivity");
				Draw("yawClamp", "Limit the yaw to min/max?", "Clamp");
				if (Any(tgts, t => t.PitchClamp == true))
				{
					BeginIndent();
						Draw("yawMin", "The minimum yaw angle in degrees.", "Min");
						Draw("yawMax", "The maximum yaw angle in degrees.", "Max");
					EndIndent();
				}
			EndIndent();
		}
	}
}
#endif