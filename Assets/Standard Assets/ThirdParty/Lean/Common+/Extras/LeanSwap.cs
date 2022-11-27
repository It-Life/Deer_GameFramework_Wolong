using UnityEngine;
using System.Collections.Generic;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Common
{
	/// <summary>This component will swap the target GameObject with one of the specified prefabs when swiping.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanSwap")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Swap")]
	public class LeanSwap : MonoBehaviour
	{
		/// <summary>The current index within the Prefabs list.</summary>
		public int Index { set { index = value; } get { return index; } } [FSA("Delay")] [SerializeField] private int index;

		/// <summary>The alternative prefabs that can be swapped to.</summary>
		public List<Transform> Prefabs { get { if (prefabs == null) prefabs = new List<Transform>(); return prefabs; } } [FSA("Prefabs")] [SerializeField] private List<Transform> prefabs;

		[SerializeField]
		private Transform clone;

		[SerializeField]
		private Transform clonePrefab;

		/// <summary>This method forces the swap to update if it's been modified.</summary>
		[ContextMenu("Update Swap")]
		public void UpdateSwap()
		{
			var prefab = GetPrefab();

			if (clone != null)
			{
				if (clonePrefab == prefab)
				{
					return;
				}

				LeanHelper.Destroy(clone.gameObject);

				clone       = null;
				clonePrefab = null;
			}

			if (prefabs != null && prefabs.Count > 0)
			{
				clone = Instantiate(prefab);

				clone.transform.SetParent(transform, false);

				clonePrefab = prefab;
			}
		}

		/// <summary>This method allows you to swap to the specified index.</summary>
		public void SwapTo(int newIndex)
		{
			index = newIndex;

			UpdateSwap();
		}

		/// <summary>This method allows you to swap to the previous index.</summary>
		[ContextMenu("Swap To Previous")]
		public void SwapToPrevious()
		{
			index -= 1;

			UpdateSwap();
		}

		/// <summary>This method allows you to swap to the next index.</summary>
		[ContextMenu("Swap To Next")]
		public void SwapToNext()
		{
			index += 1;

			UpdateSwap();
		}

		private Transform GetPrefab()
		{
			if (prefabs != null && prefabs.Count > 0)
			{
				// Wrap index to stay within Prefabs.length
				index %= prefabs.Count;

				if (index < 0)
				{
					index += prefabs.Count;
				}

				return prefabs[index];
			}

			return null;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanSwap;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSwap_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("index", "The current index within the Prefabs list.");
			Draw("prefabs", "The alternative prefabs that can be swapped to.");
		}
	}
}
#endif