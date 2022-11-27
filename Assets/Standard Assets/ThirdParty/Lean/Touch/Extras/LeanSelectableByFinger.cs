using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;
using Lean.Common;

namespace Lean.Touch
{
	/// <summary>This component makes this GameObject selectable.
	/// If your game is 3D then make sure this GameObject or a child has a Collider component.
	/// If your game is 2D then make sure this GameObject or a child has a Collider2D component.
	/// If your game is UI based then make sure this GameObject or a child has a graphic with "Raycast Target" enabled.
	/// To then select it, you can add the LeanSelect and LeanFingerTap components to your scene. You can then link up the LeanFingerTap.OnTap event to LeanSelect.SelectScreenPosition.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSelectableByFinger")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable By Finger")]
	public class LeanSelectableByFinger : LeanSelectable
	{
		public enum UseType
		{
			AllFingers,
			OnlySelectingFingers,
			IgnoreSelectingFingers
		}

		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}

		/// <summary>This allows you to control which fingers will be used by components that require this selectable.</summary>
		public UseType Use { set { use = value; } get { return use; } } [SerializeField] private UseType use;

		/// <summary>This event is called when selection begins (finger = the finger that selected this).</summary>
		public LeanFingerEvent OnSelectedFinger { get { if (onSelectedFinger == null) onSelectedFinger = new LeanFingerEvent(); return onSelectedFinger; } } [FSA("onSelect")] [SerializeField] private LeanFingerEvent onSelectedFinger;

		/// <summary>This event is called when selection begins (finger = the finger that selected this).</summary>
		public LeanFingerEvent OnSelectedFingerUp { get { if (onSelectedFingerUp == null) onSelectedFingerUp = new LeanFingerEvent(); return onSelectedFingerUp; } } [FSA("onSelectUp")] [SerializeField] private LeanFingerEvent onSelectedFingerUp;

		public static event System.Action<LeanSelectByFinger, LeanSelectableByFinger, LeanFinger> OnAnySelectedFinger;

		// The fingers that were used to select this GameObject
		// If a finger goes up then it will be removed from this list
		[System.NonSerialized]
		private List<LeanFinger> selectingFingers = new List<LeanFinger>();

		/// <summary>This tells you the first or earliest still active finger that initiated selection of this object.
		/// NOTE: If the selecting finger went up then this may return null.</summary>
		public LeanFinger SelectingFinger
		{
			get
			{
				if (selectingFingers.Count > 0)
				{
					return selectingFingers[0];
				}

				return null;
			}
		}

		/// <summary>This tells you every currently active finger that selected this object.</summary>
		public List<LeanFinger> SelectingFingers
		{
			get
			{
				return selectingFingers;
			}
		}

		public void SelectSelf(LeanFinger finger)
		{
			if (SelfSelected == false)
			{
				SelfSelected = true;

				if (finger.Up == false)
				{
					selectingFingers.Add(finger);
				}

				if (onSelectedFinger != null)
				{
					onSelectedFinger.Invoke(finger);
				}

				if (finger.Up == false && onSelectedFingerUp != null)
				{
					onSelectedFingerUp.Invoke(finger);
				}
			}
		}

		/// <summary>If requiredSelectable is set and not selected, the fingers list will be empty. If selected then the fingers list will only contain the selecting finger.</summary>
		public static List<LeanFinger> GetFingers(bool ignoreIfStartedOverGui, bool ignoreIfOverGui, int requiredFingerCount = 0, LeanSelectable requiredSelectable = null)
		{
			var fingers = LeanTouch.GetFingers(ignoreIfStartedOverGui, ignoreIfOverGui, requiredFingerCount);

			if (requiredSelectable != null)
			{
				if (requiredSelectable.IsSelected == true)
				{
					var requiredSelectableByFinger = requiredSelectable as LeanSelectableByFinger;

					if (requiredSelectableByFinger != null)
					{
						switch (requiredSelectableByFinger.use)
						{
							case UseType.AllFingers:
							{
							}
							break;

							case UseType.OnlySelectingFingers:
							{
								fingers.Clear();

								fingers.AddRange(requiredSelectableByFinger.selectingFingers);
							}
							break;

							case UseType.IgnoreSelectingFingers:
							{
								foreach (var selectingFinger in requiredSelectableByFinger.selectingFingers)
								{
									fingers.Remove(selectingFinger);
								}
							}
							break;
						}
					}

					if (requiredFingerCount > 0 && fingers.Count != requiredFingerCount)
					{
						fingers.Clear();
					}
				}
				else
				{
					fingers.Clear();
				}
			}

			return fingers;
		}

		/// <summary>If the specified finger selected an object, this will return the first one.</summary>
		public static LeanSelectableByFinger FindSelectable(LeanFinger finger)
		{
			foreach (var selectable in Instances)
			{
				var selectableByFinger = selectable as LeanSelectableByFinger;

				if (selectableByFinger != null && selectableByFinger.IsSelectedBy(finger) == true)
				{
					return selectableByFinger;
				}
			}

			return null;
		}

		/// <summary>This tells you if the current selectable was selected by the specified finger.</summary>
		public bool IsSelectedBy(LeanFinger finger)
		{
			for (var i = selectingFingers.Count - 1; i >= 0; i--)
			{
				if (selectingFingers[i] == finger)
				{
					return true;
				}
			}

			return false;
		}

		public static void InvokeAnySelectedFinger(LeanSelectByFinger select, LeanSelectableByFinger selectable, LeanFinger finger)
		{
			if (OnAnySelectedFinger != null)
			{
				OnAnySelectedFinger.Invoke(select, selectable, finger);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			LeanTouch.OnFingerUp += HandleFingerUp;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			LeanTouch.OnFingerUp -= HandleFingerUp;
		}

		private bool AnyFingersSet
		{
			get
			{
				for (var i = selectingFingers.Count - 1; i >= 0; i--)
				{
					if (selectingFingers[i].Set == true)
					{
						return true;
					}
				}

				return false;
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			if (selectingFingers.Remove(finger) == true)
			{
				if (onSelectedFingerUp != null)
				{
					onSelectedFingerUp.Invoke(finger);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSelectableByFinger;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanSelectableByFinger_Editor : Common.Editor.LeanSelectable_Editor
	{
		[System.NonSerialized] TARGET tgt; [System.NonSerialized] TARGET[] tgts;

		protected override void OnInspector()
		{
			GetTargets(out tgt, out tgts);

			Draw("use", "This allows you to control which fingers will be used by components that require this selectable.");

			base.OnInspector();
		}

		protected override void DrawEvents(bool showUnusedEvents)
		{
			base.DrawEvents(showUnusedEvents);

			if (showUnusedEvents == true || Any(tgts, t => t.OnSelectedFinger.GetPersistentEventCount() > 0))
			{
				Draw("onSelectedFinger");
			}

			if (showUnusedEvents == true || Any(tgts, t => t.OnSelectedFingerUp.GetPersistentEventCount() > 0))
			{
				Draw("onSelectedFingerUp");
			}
		}
	}
}
#endif