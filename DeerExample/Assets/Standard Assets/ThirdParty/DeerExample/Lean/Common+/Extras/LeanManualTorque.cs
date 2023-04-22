using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to add angular force to the current GameObject using events.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanManualTorque")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Manual Torque")]
	public class LeanManualTorque : MonoBehaviour
	{
		/// <summary>If your Rigidbody is on a different GameObject, set it here.</summary>
		public GameObject Target { set { target = value; } get { return target; } } [FSA("Target")] [SerializeField] private GameObject target;

		public ForceMode Mode { set { mode = value; } get { return mode; } } [FSA("Mode")] [SerializeField] private ForceMode mode;

		/// <summary>Fixed multiplier for the force.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [FSA("Multiplier")] [SerializeField] private float multiplier = 1.0f;

		/// <summary>The velocity space.</summary>
		public Space Space { set { space = value; } get { return space; } } [FSA("Space")] [SerializeField] private Space space = Space.World;

		/// <summary>The first force axis.</summary>
		public Vector3 AxisA { set { axisA = value; } get { return axisA; } } [FSA("AxisA")] [SerializeField] private Vector3 axisA = Vector3.down;

		/// <summary>The second force axis.</summary>
		public Vector3 AxisB { set { axisB = value; } get { return axisB; } } [FSA("AxisB")] [SerializeField] private Vector3 axisB = Vector3.right;

		public void AddTorqueA(float delta)
		{
			AddTorque(axisA * delta);
		}

		public void AddTorqueB(float delta)
		{
			AddTorque(axisB * delta);
		}

		public void AddTorqueAB(Vector2 delta)
		{
			AddTorque(axisA * delta.x + axisB * delta.y);
		}

		public void AddTorqueFromTo(Vector3 from, Vector3 to)
		{
			AddTorque(to - from);
		}

		public void AddTorque(Vector3 delta)
		{
			var finalGameObject = target != null ? target : gameObject;
			var rigidbody       = finalGameObject.GetComponent<Rigidbody>();

			if (rigidbody != null)
			{
				var torque = delta * multiplier;

				if (space == Space.Self)
				{
					torque = rigidbody.transform.rotation * torque;
				}

				rigidbody.AddTorque(torque, mode);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanManualTorque;

	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanManualTorque_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("target", "If your Rigidbody is on a different GameObject, set it here.");
			Draw("mode", "");
			Draw("multiplier", "Fixed multiplier for the force.");
			Draw("space", "The velocity space.");
			Draw("axisA", "The first force axis.");
			Draw("axisB", "The second force axis.");
		}
	}
}
#endif