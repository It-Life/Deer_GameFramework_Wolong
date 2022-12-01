using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Common
{
	/// <summary>This component allows you to randomly invoke one of the specified events when you manually call the <b>Invoke</b> method.</summary>
	[HelpURL(LeanHelper.PlusHelpUrlPrefix + "LeanRandomEvents")]
	[AddComponentMenu(LeanHelper.ComponentPathPrefix + "Random Events")]
	public class LeanRandomEvents : MonoBehaviour
	{
		public List<UnityEvent> Events { get { if (events == null) events = new List<UnityEvent>(); return events; } } [SerializeField] private List<UnityEvent> events;

		[ContextMenu("Invoke")]
		public void Invoke()
		{
			if (events != null && events.Count > 0)
			{
				var index   = Random.Range(0, events.Count);
				var element = events[index];

				if (element != null)
				{
					element.Invoke();
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Common.Editor
{
	using TARGET = LeanRandomEvents;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanRandomEvents_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("events");
		}
	}
}
#endif