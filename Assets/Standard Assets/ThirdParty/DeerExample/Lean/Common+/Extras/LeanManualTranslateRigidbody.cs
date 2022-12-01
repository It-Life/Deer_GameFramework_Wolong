using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to translate the specified Rigidbody when you call methods like <b>TranslateA</b>, which can be done from events.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanManualTranslateRigidbody")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Manual Translate Rigidbody")]
	public class LeanManualTranslateRigidbody : MonoBehaviour
	{
		/// <summary>If you want this component to work on a different GameObject, then specify it here. This can be used to improve organization if your GameObject already has many components.</summary>
		public GameObject Target { set { target = value; } get { return target; } } [FSA("Target")] [SerializeField] private GameObject target;

		/// <summary>This allows you to set the coordinate space the translation will use.</summary>
		public Space Space { set { space = value; } get { return space; } } [FSA("Space")] [SerializeField] private Space space;

		/// <summary>The first translation direction, used when calling TranslateA or TranslateAB.</summary>
		public Vector3 DirectionA { set { directionA = value; } get { return directionA; } } [FSA("DirectionA")] [SerializeField] private Vector3 directionA = Vector3.right;

		/// <summary>The first second direction, used when calling TranslateB or TranslateAB.</summary>
		public Vector3 DirectionB { set { directionB = value; } get { return directionB; } } [FSA("DirectionB")] [SerializeField] private Vector3 directionB = Vector3.up;

		/// <summary>The translation distance is multiplied by this.
		/// 1 = Normal distance.
		/// 2 = Double distance.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [FSA("Multiplier")] [SerializeField] private float multiplier = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [FSA("Dampening")] [SerializeField] private float damping = 10.0f;

		/// <summary>If you enable this then the translation will be multiplied by Time.deltaTime. This allows you to maintain frame rate independent movement.</summary>
		public bool ScaleByTime { set { scaleByTime = value; } get { return scaleByTime; } } [FSA("ScaleByTime")] [SerializeField] private bool scaleByTime;

		/// <summary>This allows you to control how much momentum is retained when the dragging fingers are all released.
		/// NOTE: This requires <b>Dampening</b> to be above 0.</summary>
		public float Inertia { set { inertia = value; } get { return inertia; } } [FSA("Inertia")] [SerializeField] [Range(0.0f, 1.0f)] private float inertia;

		/// <summary>If you want this component to override velocity enable this, otherwise disable this and rely on Rigidbody.drag.</summary>
		public bool ResetVelocityInUpdate { set { resetVelocityInUpdate = value; } get { return resetVelocityInUpdate; } } [FSA("ResetVelocityInUpdate")] [SerializeField] private bool resetVelocityInUpdate = true;

		[SerializeField]
		private Vector3 remainingDelta;

		/// <summary>This method allows you to translate along DirectionA, with the specified multiplier.</summary>
		public void TranslateA(float magnitude)
		{
			Translate(directionA * magnitude);
		}

		/// <summary>This method allows you to translate along DirectionB, with the specified multiplier.</summary>
		public void TranslateB(float magnitude)
		{
			Translate(directionB * magnitude);
		}

		/// <summary>This method allows you to translate along DirectionA and DirectionB, with the specified multipliers.</summary>
		public void TranslateAB(Vector2 magnitude)
		{
			Translate(directionA * magnitude.x + directionB * magnitude.y);
		}

		/// <summary>This method allows you to translate along the specified vector in local space.</summary>
		public void Translate(Vector3 vector)
		{
			if (space == Space.Self)
			{
				var finalTransform = target != null ? target.transform : transform;

				vector = finalTransform.TransformVector(vector);
			}

			TranslateWorld(vector);
		}

		/// <summary>This method allows you to translate along the specified vector in world space.</summary>
		public void TranslateWorld(Vector3 vector)
		{
			if (scaleByTime == true)
			{
				vector *= Time.deltaTime;
			}

			remainingDelta += vector * multiplier;
		}

		protected virtual void FixedUpdate()
		{
			var finalTransform = target != null ? target.transform : transform;
			var factor         = LeanHelper.GetDampenFactor(Damping, Time.fixedDeltaTime);
			var newDelta       = Vector3.Lerp(remainingDelta, Vector3.zero, factor);
			var rigidbody      = finalTransform.GetComponent<Rigidbody>();

			if (rigidbody != null)
			{
				rigidbody.velocity += (remainingDelta - newDelta)  / Time.fixedDeltaTime;
			}

			remainingDelta = newDelta;
		}

		protected virtual void Update()
		{
			if (resetVelocityInUpdate == true)
			{
				var finalGameObject = target != null ? target : gameObject;
				var rigidbody       = finalGameObject.GetComponent<Rigidbody>();

				if (rigidbody != null)
				{
					rigidbody.velocity = Vector3.zero;
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanManualTranslateRigidbody;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanManualTranslateRigidbody_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("target", "If you want this component to work on a different GameObject, then specify it here. This can be used to improve organization if your GameObject already has many components.");
			Draw("space", "This allows you to set the coordinate space the translation will use.");
			Draw("directionA", "The first translation direction, used when calling TranslateA or TranslateAB.");
			Draw("directionB", "The first translation direction, used when calling TranslateB or TranslateAB.");

			Separator();

			Draw("multiplier", "The translation distance is multiplied by this.\n\n1 = Normal distance.\n\n2 = Double distance.");
			Draw("scaleByTime", "If you enable this then the translation will be multiplied by Time.deltaTime. This allows you to maintain frame rate independent movement.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("inertia", "This allows you to control how much momentum is retained when the dragging fingers are all released.\n\nNOTE: This requires <b>Dampening</b> to be above 0.");
			Draw("resetVelocityInUpdate", "If you want this component to override velocity enable this, otherwise disable this and rely on Rigidbody.drag.");
		}
	}
}
#endif