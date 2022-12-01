using UnityEngine;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This script allows you to paint the vertices of the current MeshFilter's mesh.</summary>
	[RequireComponent(typeof(MeshFilter))]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanDragColorMesh")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Color Mesh")]
	public class LeanDragColorMesh : MonoBehaviour
	{
		/// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
		public LeanFingerFilter Use = new LeanFingerFilter(true);

		/// <summary>The color you want to paint the hit triangles.</summary>
		public Color PaintColor { set { paintColor = value; } get { return paintColor; } } [FSA("PaintColor")] [SerializeField] private Color paintColor;

		/// <summary>The camera this component will calculate using.
		/// None/null = MainCamera.</summary>
		public Camera Camera { set { _camera = value; } get { return _camera; } } [FSA("Camera")] [SerializeField] private Camera _camera;

		// The cached mesh filter
		[System.NonSerialized]
		private MeshFilter cachedMeshFilter;

		// Stores a duplicate of the MeshFilter's mesh
		private Mesh modifiedMesh;

		private int[] modifiedIndices;

		// Stores the current vertex position array
		private Color[] modifiedColors;

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
		public void AddFinger(LeanFinger finger)
		{
			Use.AddFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
		public void RemoveFinger(LeanFinger finger)
		{
			Use.RemoveFinger(finger);
		}

		/// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
		public void RemoveAllFingers()
		{
			Use.RemoveAllFingers();
		}

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}
#endif

		protected virtual void Awake()
		{
			Use.UpdateRequiredSelectable(gameObject);
		}

		protected virtual void Update()
		{
			var fingers = Use.UpdateAndGetFingers();

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				Paint(fingers[i]);
			}
		}

		private void Paint(LeanFinger finger)
		{
			// Make sure the mesh filter and mesh exist
			if (cachedMeshFilter == null) cachedMeshFilter = GetComponent<MeshFilter>();

			if (cachedMeshFilter.sharedMesh != null)
			{
				// Duplicate mesh?
				if (modifiedMesh == null)
				{
					modifiedMesh = cachedMeshFilter.sharedMesh = Instantiate(cachedMeshFilter.sharedMesh);
				}

				// Duplicate indices and colors?
				if (modifiedColors == null || modifiedColors.Length != modifiedMesh.vertexCount)
				{
					modifiedIndices = modifiedMesh.triangles;
					modifiedColors  = modifiedMesh.colors;

					// If the mesh has no vertex colors, make some
					if (modifiedColors == null || modifiedColors.Length == 0)
					{
						modifiedColors = new Color[modifiedMesh.vertexCount];

						for (var i = modifiedMesh.vertexCount - 1; i >= 0; i--)
						{
							modifiedColors[i] = Color.white;
						}
					}
				}

				// Raycast under the finger and paint the hit triangle
				var hit = default(RaycastHit);

				if (Physics.Raycast(finger.GetRay(_camera), out hit) == true)
				{
					if (hit.collider.gameObject == gameObject)
					{
						var index = hit.triangleIndex * 3;
						var a     = modifiedIndices[index + 0];
						var b     = modifiedIndices[index + 1];
						var c     = modifiedIndices[index + 2];
							
						modifiedColors[a] = Color.black;
						modifiedColors[b] = Color.black;
						modifiedColors[c] = Color.black;

						modifiedMesh.colors = modifiedColors;
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Editor
{
	using TARGET = LeanDragColorMesh;

	[UnityEditor.CanEditMultipleObjects]
	[UnityEditor.CustomEditor(typeof(TARGET))]
	public class LeanDragColorMesh_Editor : LeanEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);
			
			Draw("Use");
			Draw("paintColor", "The color you want to paint the hit triangles.");
			Draw("_camera", "The camera this component will calculate using.\n\nNone/null = MainCamera.");
		}
	}
}
#endif