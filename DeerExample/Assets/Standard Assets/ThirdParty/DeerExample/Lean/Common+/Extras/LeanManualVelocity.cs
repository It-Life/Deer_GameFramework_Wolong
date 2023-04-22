using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to add force to the current GameObject using events.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanManualVelocity")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Manual Velocity")]
	public class LeanManualVelocity : MonoBehaviour
	{
		/// <summary>If your Rigidbody is on a different GameObject, set it here.</summary>
		public GameObject Target { set { target = value; } get { return target; } } [FSA("Target")] [SerializeField] private GameObject target;

		/// <summary>The force mode.</summary>
		public ForceMode Mode { set { mode = value; } get { return mode; } } [FSA("Mode")] [SerializeField] private ForceMode mode;

		/// <summary>The applied velocity will be multiplied by this.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [FSA("Multiplier")] [SerializeField] private float multiplier = 1.0f;

		/// <summary>The velocity space.</summary>
		public Space Space { set { space = value; } get { return space; } } [FSA("Space")] [SerializeField] private Space space = Space.World;

		/// <summary>The first force direction.</summary>
		public Vector2 DirectionA { set { directionA = value; } get { return directionA; } } [FSA("DirectionA")] [SerializeField] private Vector2 directionA = Vector2.right;

		/// <summary>The second force direction.</summary>
		public Vector2 DirectionB { set { directionB = value; } get { return directionB; } } [FSA("DirectionB")] [SerializeField] private Vector2 directionB = Vector2.up;

		public void AddForceA(float delta)
		{
			AddForce(directionA * delta);
		}

		public void AddForceB(float delta)
		{
			AddForce(directionB * delta);
		}

		public void AddForceAB(Vector2 delta)
		{
			AddForce(directionA * delta.x + directionB * delta.y);
		}

		public void AddForceFromTo(Vector3 from, Vector3 to)
		{
			AddForce(to - from);
		}

		public void AddForce(Vector3 delta)
		{
			var finalGameObject = target != null ? target : gameObject;
			var rigidbody       = finalGameObject.GetComponent<Rigidbody>();

			if (rigidbody != null)
			{
				var force = delta * multiplier;

				if (space == Space.Self)
				{
					force = rigidbody.transform.rotation * force;
				}

				rigidbody.AddForce(force, mode);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanManualVelocity;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanManualVelocity_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("target", "If your Rigidbody is on a different GameObject, set it here.");
			Draw("mode", "The force mode.");
			Draw("multiplier", "The applied velocity will be multiplied by this.");
			Draw("space", "The velocity space.");
			Draw("directionA", "The first force direction.");
			Draw("directionB", "The second force direction.");
		}
	}
}
#endif