using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component allows you to translate the specified GameObject when you call methods like <b>TranslateA</b>, which can be done from events.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanManualTranslate")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Manual Translate")]
	public class LeanManualTranslate : MonoBehaviour
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

		/// <summary>If you call the ResetPosition method, the position will be set to this.</summary>
		public Vector3 DefaultPosition { set { defaultPosition = value; } get { return defaultPosition; } } [FSA("DefaultPosition")] [SerializeField] private Vector3 defaultPosition;

		[SerializeField]
		private Vector3 remainingDelta;

		/// <summary>This method will reset the position to the specified DefaultPosition value.</summary>
		[ContextMenu("Reset Position")]
		public void ResetPosition()
		{
			var finalTransform = target != null ? target.transform : transform;
			var oldPosition    = finalTransform.localPosition;

			if (space == Space.Self)
			{
				finalTransform.localPosition = defaultPosition;
			}
			else
			{
				finalTransform.position = defaultPosition;
			}

			remainingDelta = finalTransform.localPosition - oldPosition;

			// Revert
			finalTransform.localPosition = oldPosition;
		}

		/// <summary>This method will cause the position to immediately snap to its final value.</summary>
		[ContextMenu("Snap To Target")]
		public void SnapToTarget()
		{
			UpdatePosition(1.0f);
		}

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

		protected virtual void Update()
		{
			var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

			UpdatePosition(factor);
		}

		private void UpdatePosition(float factor)
		{
			var finalTransform = target != null ? target.transform : transform;
			var newDelta       = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			finalTransform.position += remainingDelta - newDelta;

			remainingDelta = newDelta;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanManualTranslate;

	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanManualTranslate_Editor : LeanEditor
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
			Draw("defaultPosition", "If you call the ResetPosition method, the position will be set to this.");
		}
	}
}
#endif