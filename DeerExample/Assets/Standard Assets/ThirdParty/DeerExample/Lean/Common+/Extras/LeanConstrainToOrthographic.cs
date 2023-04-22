using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component will constrain the current <b>transform.position</b> to the specified <b>LeanPlane</b> shape.</summary>
	[DefaultExecutionOrder(200)]
	[HelpURL(LeanHelper.HelpUrlPrefix + "LeanConstrainToOrthographic")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Constrain To Orthographic")]
	public class LeanConstrainToOrthographic : MonoBehaviour
	{
		/// <summary>The camera whose orthographic size will be used.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [FSA("Camera")] [SerializeField] private Camera _camera;

		/// <summary>The plane this transform will be constrained to.</summary>
		public LeanPlane Plane { set { plane = value; } get { return plane; } } [FSA("Plane")] [SerializeField] private LeanPlane plane;

		protected virtual void LateUpdate()
		{
			// Make sure the camera exists
			var camera = LeanHelper.GetCamera(_camera, gameObject);

			if (camera != null)
			{
				if (plane != null)
				{
					var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
					var hit = default(Vector3);

					if (plane.TryRaycast(ray, ref hit, 0.0f, false) == true)
					{
						var oldPosition = transform.position;
						var local       = plane.transform.InverseTransformPoint(hit);
						var snapped     = local;
						var size        = new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);

						if (plane.ClampX == true)
						{
							var min = plane.MinX + size.x;
							var max = plane.MaxX - size.x;

							if (min > max)
							{
								snapped.x = (min + max) * 0.5f;
							}
							else
							{
								snapped.x = Mathf.Clamp(local.x, min, max);
							}
						}

						if (plane.ClampY == true)
						{
							var min = plane.MinY + size.y;
							var max = plane.MaxY - size.y;

							if (min > max)
							{
								snapped.y = (min + max) * 0.5f;
							}
							else
							{
								snapped.y = Mathf.Clamp(local.y, min, max);
							}
						}

						if (local != snapped)
						{
							var delta       = oldPosition - hit;
							var newPosition = plane.transform.TransformPoint(snapped) + delta;

							if (Mathf.Approximately(oldPosition.x, newPosition.x) == false ||
								Mathf.Approximately(oldPosition.y, newPosition.y) == false ||
								Mathf.Approximately(oldPosition.z, newPosition.z) == false)
							{
								transform.position = newPosition;
							}
						}
					}
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanConstrainToOrthographic;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanConstrainToOrthographic_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("_camera", "The camera whose orthographic size will be used.");
			Draw("plane", "The plane this transform will be constrained to.");
		}
	}
}
#endif