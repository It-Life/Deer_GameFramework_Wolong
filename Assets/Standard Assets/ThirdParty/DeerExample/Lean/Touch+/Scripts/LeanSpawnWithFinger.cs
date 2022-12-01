using UnityEngine;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This component allows you to spawn a prefab at a point relative to a finger and the specified ScreenDepth.
	/// NOTE: To trigger the prefab spawn you must call the Spawn method on this component from somewhere.</summary>
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSpawnWithFinger")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Spawn With Finger")]
	public class LeanSpawnWithFinger : MonoBehaviour
	{
		public enum RotateType
		{
			ThisTransform,
			ScreenDepthNormal
		}

		[System.Serializable]
		public class FingerData : LeanFingerData
		{
			public Transform      Clone;
			public LeanSelectable Selectable;
		}

		/// <summary>The prefab that this component can spawn.</summary>
		public Transform Prefab { set { prefab = value; } get { return prefab; } } [FSA("Prefab")] [SerializeField] private Transform prefab;

		/// <summary>How should the spawned prefab be rotated?</summary>
		public RotateType RotateTo { set { rotateTo = value; } get { return rotateTo; } } [FSA("RotateTo")] [SerializeField] private RotateType rotateTo;

		/// <summary>Hold on to the spawned clone while the spawning finger is still being held?</summary>
		public bool DragAfterSpawn { set { dragAfterSpawn = value; } get { return dragAfterSpawn; } } [FSA("DragAfterSpawn")] [SerializeField] private bool dragAfterSpawn;

		/// <summary>If the spawned object is dropped on top of the wrong GameObject (e.g. UI), destroy it?</summary>
		public bool DestroyIfBlocked { set { destroyIfBlocked = value; } get { return destroyIfBlocked; } } [SerializeField] private bool destroyIfBlocked;

		public LeanScreenQuery BlockedScreenQuery = new LeanScreenQuery(LeanScreenQuery.MethodType.Raycast, 1 << 5); // Layer 5 = UI

		/// <summary>If the spawned object isn't dropped on top of the specified GameObjects, destroy it?</summary>
		public bool DestroyIfMissing { set { destroyIfMissing = value; } get { return destroyIfMissing; } } [SerializeField] private bool destroyIfMissing;

		public LeanScreenQuery MissingScreenQuery = new LeanScreenQuery(LeanScreenQuery.MethodType.Raycast);

		/// <summary>If the specified prefab is selectable, select it when spawned?</summary>
		public bool SelectOnSpawn { set { selectOnSpawn = value; } get { return selectOnSpawn; } } [SerializeField] private bool selectOnSpawn;

		/// <summary>If you want the spawned component to be a selected with a specific select component, you can specify it here.
		/// None/null = It will be self selected.</summary>
		public LeanSelect SelectWith { set { selectWith = value; } get { return selectWith; } } [SerializeField] private LeanSelect selectWith;

		/// <summary>If the selecting finger goes up, deselect the object?</summary>
		public bool DeselectOnUp { set { deselectOnUp = value; } get { return deselectOnUp; } } [SerializeField] private bool deselectOnUp;

		/// <summary>The conversion method used to find a world point from a screen point.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.FixedDistance, Physics.DefaultRaycastLayers, 10.0f);

		/// <summary>This allows you to offset the finger position.</summary>
		public Vector2 PixelOffset { set { pixelOffset = value; } get { return pixelOffset; } } [FSA("PixelOffset")] [SerializeField] private Vector2 pixelOffset;

		/// <summary>If you want the pixels to scale based on device resolution, then specify the canvas whose scale you want to use here.</summary>
		public Canvas PixelScale { set { pixelScale = value; } get { return pixelScale; } } [FSA("PixelScale")] [SerializeField] private Canvas pixelScale;

		/// <summary>This allows you to offset the spawned object position.</summary>
		public Vector3 WorldOffset { set { worldOffset = value; } get { return worldOffset; } } [FSA("WorldOffset")] [SerializeField] private Vector3 worldOffset;

		/// <summary>This allows you transform the WorldOffset to be relative to the specified Transform.</summary>
		public Transform WorldRelativeTo { set { worldRelativeTo = value; } get { return worldRelativeTo; } } [FSA("WorldRelativeTo")] [SerializeField] private Transform worldRelativeTo;

		[SerializeField]
		private List<FingerData> fingerDatas;

		private static Stack<FingerData> fingerDataPool = new Stack<FingerData>();

		/// <summary>This will spawn Prefab at the specified finger based on the ScreenDepth setting.</summary>
		public void Spawn(LeanFinger finger)
		{
			if (prefab != null && finger != null)
			{
				// Spawn and position
				var clone      = Instantiate(prefab);
				var fingerData = default(FingerData);

				UpdateSpawnedTransform(finger, clone);

				clone.gameObject.SetActive(true);

				// Drag?
				if (dragAfterSpawn == true)
				{
					fingerData = LeanFingerData.FindOrCreate(ref fingerDatas, finger);

					fingerData.Clone = clone;
				}

				// Select?
				if (selectOnSpawn == true)
				{
					var selectable         = clone.GetComponent<LeanSelectable>();
					var selectableByFinger = selectable as LeanSelectableByFinger;
					var selectWithByFinger = selectWith as LeanSelectByFinger;

					if (selectableByFinger != null)
					{
						if (selectWithByFinger != null)
						{
							selectWithByFinger.Select(selectableByFinger, finger);
						}
						else if (selectWith != null)
						{
							selectWith.Select(selectableByFinger);
						}
						else
						{
							selectableByFinger.SelectSelf(finger);
						}
					}
					else if (selectable != null)
					{
						if (selectWithByFinger != null)
						{
							selectWithByFinger.Select(selectable, finger);
						}
						else if (selectWith != null)
						{
							selectWith.Select(selectable);
						}
						else
						{
							selectable.SelfSelected = true;
						}
					}

					if (fingerData != null)
					{
						fingerData.Selectable = selectable;
					}
				}
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerUp += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerUp -= HandleFingerUp;
		}

		protected virtual void Update()
		{
			for (var i = fingerDatas.Count - 1; i >= 0; i--)
			{
				var fingerData = fingerDatas[i];

				if (fingerData.Clone != null)
				{
					UpdateSpawnedTransform(fingerData.Finger, fingerData.Clone);
				}
			}
		}

		private void UpdateSpawnedTransform(LeanFinger finger, Transform instance)
		{
			// Grab screen position of finger, and optionally offset it
			var screenPoint = finger.ScreenPosition;

			if (pixelScale != null)
			{
				screenPoint += pixelOffset * pixelScale.scaleFactor;
			}
			else
			{
				screenPoint += pixelOffset;
			}

			// Converted screen position to world position, and optionally offset it
			var worldPoint = ScreenDepth.Convert(screenPoint, gameObject, instance);

			if (worldRelativeTo != null)
			{
				worldPoint += worldRelativeTo.TransformPoint(worldOffset);
			}
			else
			{
				worldPoint += worldOffset;
			}

			// Write position
			instance.position = worldPoint;

			// Write rotation
			switch (rotateTo)
			{
				case RotateType.ThisTransform:
				{
					instance.rotation = transform.rotation;
				}
				break;

				case RotateType.ScreenDepthNormal:
				{
					instance.up = LeanScreenDepth.LastWorldNormal;
				}
				break;
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			var fingerData = LeanFingerData.Find(fingerDatas, finger);

			if (deselectOnUp == true && fingerData != null && fingerData.Selectable != null)
			{
				fingerData.Selectable.Deselect();
			}

			if (fingerData != null && fingerData.Clone != null)
			{
				if (destroyIfBlocked == true || destroyIfMissing == true)
				{
					LeanScreenQuery.ChangeLayers(fingerData.Clone.gameObject, false, true);

					if (destroyIfBlocked == true && BlockedScreenQuery.Query<Component>(gameObject, finger.ScreenPosition) != null)
					{
						Destroy(fingerData.Clone.gameObject);
					}

					if (destroyIfMissing == true && MissingScreenQuery.Query<Component>(gameObject, finger.ScreenPosition) == null)
					{
						Destroy(fingerData.Clone.gameObject);
					}

					LeanScreenQuery.RevertLayers();
				}
			}

			LeanFingerData.Remove(fingerDatas, finger, fingerDataPool);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanSpawnWithFinger;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET), true)]
	public class LeanSpawnWithFinger_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			BeginError(Any(tgts, t => t.Prefab == null));
				Draw("prefab");
			EndError();
			Draw("rotateTo", "How should the spawned prefab be rotated?");
			Draw("dragAfterSpawn", "Hold on to the spawned clone while the spawning finger is still being held?");
			if (Any(tgts, t => t.DragAfterSpawn == true))
			{
				BeginIndent();
					Draw("ScreenDepth");
					Draw("destroyIfBlocked", "If the spawned object is dropped on top of the wrong GameObject (e.g. UI), destroy it?");
					if (Any(tgts, t => t.DestroyIfBlocked == true))
					{
						BeginIndent();
							Draw("BlockedScreenQuery");
						EndIndent();
					}
					Draw("destroyIfMissing", "If the spawned object isn't dropped on top of the specified GameObjects, destroy it?");
					if (Any(tgts, t => t.DestroyIfMissing == true))
					{
						BeginIndent();
							Draw("MissingScreenQuery");
						EndIndent();
					}
				EndIndent();
			}
			Draw("selectOnSpawn", "If the specified prefab is selectable, select it when spawned?");
			if (Any(tgts, t => t.SelectOnSpawn == true))
			{
				BeginIndent();
					Draw("selectWith", "If you want the spawned component to be a selected with a specific select component, you can specify it here.\n\nNone/null = It will be self selected.");
					Draw("deselectOnUp", "If the selecting finger goes up, deselect the object?");
				EndIndent();
			}

			Separator();

			Draw("pixelOffset", "This allows you to offset the finger position.");
			Draw("pixelScale", "If you want the pixels to scale based on device resolution, then specify the canvas whose scale you want to use here.");

			Separator();

			Draw("worldOffset", "This allows you to offset the spawned object position.");
			Draw("worldRelativeTo", "This allows you transform the WorldOffset to be relative to the specified Transform.");
		}
	}
}
#endif