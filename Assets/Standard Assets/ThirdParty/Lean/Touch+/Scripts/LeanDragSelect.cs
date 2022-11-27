using System.Collections.Generic;
using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to select multiple objects by dragging across the screen through them.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanDragSelect")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Select")]
	public class LeanDragSelect : MonoBehaviour
	{
		class FingerData : LeanFingerData
		{
			public LeanSelectableByFinger LastSelectable;
		}

		/// <summary>The select component that will be used.</summary>
		public LeanSelectByFinger Select { set { select = value; } get { return select; } } [FSA("Select")] [SerializeField] private LeanSelectByFinger select;

		/// <summary>If you begin dragging while objects are already selected, skip?</summary>
		public bool RequireNoSelectables { set { requireNoSelectables = value; } get { return requireNoSelectables; } } [FSA("RequireNoSelectables")] [SerializeField] private bool requireNoSelectables;

		/// <summary>If you begin dragging on a point that isn't above a selectable object, skip?</summary>
		public bool RequireInitialSelection { set { requireInitialSelection = value; } get { return requireInitialSelection; } } [FSA("RequireInitialSelection")] [SerializeField] private bool requireInitialSelection;

		/// <summary>Automatically deselect all objects when the drag starts?</summary>
		public bool DeselectAllAtStart { set { deselectAllAtStart = value; } get { return deselectAllAtStart; } } [FSA("DeselectAllAtStart")] [SerializeField] private bool deselectAllAtStart;

		/// <summary>Must the next selected object be within a specified world space distance?\n\n0 = Any distance.</summary>
		public float MaximumSeparation { set { maximumSeparation = value; } get { return maximumSeparation; } } [FSA("MaximumSeparation")] [SerializeField] private float maximumSeparation;

		[System.NonSerialized]
		private List<FingerData> fingerDatas;

		[System.NonSerialized]
		private bool waitingForSelection;

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown   += HandleFingerDown;
			LeanTouch.OnFingerUpdate += HandleFingerUpdate;
			LeanTouch.OnFingerUp     += HandleFingerUp;

			LeanSelectableByFinger.OnAnySelectedFinger += HandleAnySelectedFinger;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown   -= HandleFingerDown;
			LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
			LeanTouch.OnFingerUp     -= HandleFingerUp;

			LeanSelectableByFinger.OnAnySelectedFinger -= HandleAnySelectedFinger;
		}

		private void HandleFingerDown(LeanFinger finger)
		{
			if (select != null)
			{
				if (requireNoSelectables == true && select.Selectables.Count > 0)
				{
					return;
				}

				if (finger.Index == LeanTouch.HOVER_FINGER_INDEX)
				{
					return;
				}

				if (deselectAllAtStart == true)
				{
					select.DeselectAll();
				}

				if (requireInitialSelection == true)
				{
					waitingForSelection = true;

					select.SelectScreenPosition(finger);

					waitingForSelection = false;
				}
				else
				{
					LeanFingerData.FindOrCreate(ref fingerDatas, finger);

					select.SelectScreenPosition(finger);
				}
			}
		}

		private void HandleFingerUpdate(LeanFinger finger)
		{
			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (fingerData != null)
			{
				if (select != null)
				{
					select.SelectScreenPosition(finger);
				}
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			LeanFingerData.Remove(fingerDatas, finger);
		}

		private void HandleAnySelectedFinger(LeanSelectByFinger select, LeanSelectableByFinger selectable, LeanFinger finger)
		{
			if (waitingForSelection == true)
			{
				LeanFingerData.FindOrCreate(ref fingerDatas, finger);
			}

			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (fingerData != null)
			{
				// Good selection?
				if (maximumSeparation <= 0.0f || fingerData.LastSelectable == null || Vector3.Distance(fingerData.LastSelectable.transform.position, selectable.transform.position) <= maximumSeparation)
				{
					fingerData.LastSelectable = selectable;
				}
				// Too far to select?
				else
				{
					selectable.Deselect();
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanDragSelect;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanDragSelect_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("select", "The select component that will be used.");
			Draw("requireNoSelectables", "If you begin dragging while objects are already selected, skip?");
			Draw("requireInitialSelection", "If you begin dragging on a point that isn't above a selectable object, skip?");
			Draw("deselectAllAtStart", "Automatically deselect all objects when the drag starts?");
			Draw("maximumSeparation", "Must the next selected object be within a specified world space distance?\n\n0 = Any distance.");
		}
	}
}
#endif