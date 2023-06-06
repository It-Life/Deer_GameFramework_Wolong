using UnityEngine;

namespace Lean.Common
{
	/// <summary>This component allows you to rescale the current GameObject using events.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanManualRescale")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Manual Rescale")]
	public class LeanManualRescale : MonoBehaviour
	{
		/// <summary>If you want this component to work on a different GameObject, then specify it here. This can be used to improve organization if your GameObject already has many components.</summary>
		public GameObject Target { set { target = value; } get { return target; } } [SerializeField] private GameObject target;

		/// <summary>The first scale axes, used when calling <b>ScaleA</b> or <b>ScaleAB</b>.</summary>
		public Vector3 AxesA { set { axesA = value; } get { return axesA; } } [SerializeField] private Vector3 axesA = Vector3.right;

		/// <summary>The second scale axes, used when calling <b>ScaleB</b> or <b>ScaleAB</b>.</summary>
		public Vector3 AxesB { set { axesB = value; } get { return axesB; } } [SerializeField] private Vector3 axesB = Vector3.up;

		/// <summary>The scale value is multiplied by this.
		/// 1 = Normal distance.
		/// 2 = Double distance.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private float multiplier = 1.0f;

		/// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
		/// -1 = Instantly change.
		/// 1 = Slowly change.
		/// 10 = Quickly change.</summary>
		public float Damping { set { damping = value; } get { return damping; } } [SerializeField] private float damping = 10.0f;

		/// <summary>If you enable this then the scale will be multiplied by Time.deltaTime. This allows you to maintain frame rate independent movement.</summary>
		public bool ScaleByTime { set { scaleByTime = value; } get { return scaleByTime; } } [SerializeField] private bool scaleByTime;

		/// <summary>If you call the <b>ResetScale</b> method, the scale will be set to this.</summary>
		public Vector3 DefaultScale { set { defaultScale = value; } get { return defaultScale; } } [SerializeField] private Vector3 defaultScale = Vector3.one;

		[SerializeField]
		private Vector3 remainingDelta;

		/// <summary>This method will reset the scale to the specified <b>DefaultScale</b> value.</summary>
		[ContextMenu("Reset Scale")]
		public void ResetScale()
		{
			var finalTransform = target != null ? target.transform : transform;

			remainingDelta = defaultScale - finalTransform.localScale;
		}

		/// <summary>This method will cause the scale to immediately snap to its final value.</summary>
		[ContextMenu("Snap To Target")]
		public void SnapToTarget()
		{
			UpdateScale(1.0f);
		}

		/// <summary>This method allows you to scale by <b>AxesA</b>, with the specified multiplier.</summary>
		public void AddScaleA(float magnitude)
		{
			AddScale(axesA * magnitude);
		}

		/// <summary>This method allows you to scale by <b>AxesB</b>, with the specified multiplier.</summary>
		public void AddScaleB(float magnitude)
		{
			AddScale(axesB * magnitude);
		}

		/// <summary>This method allows you to scale by <b>AxesA</b> and <b>AxesB</b>, with the specified multipliers.</summary>
		public void AddScaleAB(Vector2 magnitude)
		{
			AddScale(axesA * magnitude.x + axesB * magnitude.y);
		}

		/// <summary>This method allows you to scale by the specified vector in local space.</summary>
		public void AddScale(Vector3 vector)
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

			UpdateScale(factor);
		}

		private void UpdateScale(float factor)
		{
			var finalTransform = target != null ? target.transform : transform;
			var newDelta       = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			finalTransform.localScale += remainingDelta - newDelta;

			remainingDelta = newDelta;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanManualRescale;

	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanManualRescale_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("target", "If you want this component to work on a different GameObject, then specify it here. This can be used to improve organization if your GameObject already has many components.");
			Draw("axesA", "The first scale axes, used when calling ScaleA or ScaleAB.");
			Draw("axesB", "The second scale axes, used when calling ScaleB or ScaleAB.");

			Separator();

			Draw("multiplier", "The scale value is multiplied by this.\n\n1 = Normal distance.\n\n2 = Double distance.");
			Draw("scaleByTime", "If you enable this then the scale will be multiplied by Time.deltaTime. This allows you to maintain frame rate independent movement.");
			Draw("damping", "If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.\n\n-1 = Instantly change.\n\n1 = Slowly change.\n\n10 = Quickly change.");
			Draw("defaultScale", "If you call the ResetPosition method, the position will be set to this.");
		}
	}
}
#endif