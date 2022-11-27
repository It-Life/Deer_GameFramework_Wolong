using UnityEngine;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component will constrain the current transform.localScale to the specified range.</summary>
	[DefaultExecutionOrder(200)]
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanConstrainScale")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Constrain Scale")]
	public class LeanConstrainScale : MonoBehaviour
	{
		/// <summary>Should each axis be checked separately? If not, the relative x/y/z values will be maintained.</summary>
		//[Tooltip("Should each axis be checked separately? If not, the relative x/y/z values will be maintained.")]
		//public bool Independent;

		/// <summary>Should there be a minimum transform.localScale?</summary>
		public bool Minimum { set { minimum = value; } get { return minimum; } } [FSA("Minimum")] [SerializeField] private bool minimum;

		/// <summary>The minimum transform.localScale value.</summary>
		public Vector3 MinimumScale { set { minimumScale = value; } get { return minimumScale; } } [FSA("MinimumScale")] [SerializeField] private Vector3 minimumScale = Vector3.one;

		/// <summary>Should there be a maximum transform.localScale?</summary>
		public bool Maximum { set { maximum = value; } get { return maximum; } } [FSA("Maximum")] [SerializeField] private bool maximum;

		/// <summary>The maximum transform.localScale value.</summary>
		public Vector3 MaximumScale { set { maximumScale = value; } get { return maximumScale; } } [FSA("MaximumScale")] [SerializeField] private Vector3 maximumScale = Vector3.one;

		protected virtual void LateUpdate()
		{
			var oldScale = transform.localScale;
			var newScale = oldScale;

			//if (Independent == true)
			{
				if (minimum == true)
				{
					newScale.x = Mathf.Max(newScale.x, minimumScale.x);
					newScale.y = Mathf.Max(newScale.y, minimumScale.y);
					newScale.z = Mathf.Max(newScale.z, minimumScale.z);
				}

				if (maximum == true)
				{
					newScale.x = Mathf.Min(newScale.x, maximumScale.x);
					newScale.y = Mathf.Min(newScale.y, maximumScale.y);
					newScale.z = Mathf.Min(newScale.z, maximumScale.z);
				}
			}
			/*
			else
			{
				if (Minimum == true)
				{
					var best  = 1.0f;
					var found = false;

					if (scale.x < MinimumScale.x)
					{
						var current = scale.x / MinimumScale.x;
						found = true;
					}

					if (found == true)
					{
						scale *= best;
					}
				}
			}
			*/

			if (Mathf.Approximately(oldScale.x, newScale.x) == false ||
				Mathf.Approximately(oldScale.y, newScale.y) == false ||
				Mathf.Approximately(oldScale.z, newScale.z) == false)
			{
				transform.localScale = newScale;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanConstrainScale;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanConstrainScale_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("minimum", "Should there be a minimum transform.localScale?");
			if (Any(tgts, t => t.Minimum == true))
			{
				BeginIndent();
					Draw("minimumScale", "Should there be a minimum transform.localScale?", "Scale");
				EndIndent();
			}
			Draw("maximum", "Should there be a maximum transform.localScale?");
			if (Any(tgts, t => t.Maximum == true))
			{
				BeginIndent();
					Draw("maximumScale", "Should there be a maximum transform.localScale?", "Scale");
				EndIndent();
			}
		}
	}
}
#endif