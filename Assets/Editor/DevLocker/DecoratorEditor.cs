using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DevLocker.Tools
{
	/// <summary>
	/// A base class for creating editors that decorate Unity's built-in editor types.
	/// Credits for this class goes to its author Mr.Lior Tal.
	/// http://www.tallior.com/extending-unity-inspectors/
	/// 
	/// This version is tweaked a bit and some corner-cases were fixed.
	/// </summary>
	public abstract class DecoratorEditor : Editor
	{
		// empty array for invoking methods using reflection
		private static readonly object[] EMPTY_ARRAY = new object[0];

		#region Editor Fields

		/// <summary>
		/// Type object for the internally used (decorated) editor.
		/// </summary>
		private System.Type decoratedEditorType;

		/// <summary>
		/// Type object for the object that is edited by this editor.
		/// </summary>
		private System.Type editedObjectType;

		// Some methods forbid using the 'targets' property and allow only the 'target' one.
		// For this purpose, keep two instances of the decorated editor type configured accordingly.
		// Example: OnSceneGUI() or OnPreviewGUI() allow only the 'target' property.
		//			If this is not respected, this error is logged:
		//			"The targets array should not be used inside OnSceneGUI or OnPreviewGUI. Use the single target property instead. UnityEditor.Editor:get_targets()"
		private Editor editorInstanceMultiTargets;
		private Editor editorInstanceSingleTarget;

		#endregion

		private static Dictionary<string, System.Type> decoratedEditorTypes = new Dictionary<string, System.Type>();
		private static Dictionary<string, MethodInfo> decoratedMethods = new Dictionary<string, MethodInfo>();

		private static Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));

		protected Editor EditorInstanceMultiTargets {
			get {
				if (editorInstanceMultiTargets == null && targets != null && targets.Length > 0) {
					editorInstanceMultiTargets = Editor.CreateEditor(targets, decoratedEditorType);
				}

				if (editorInstanceMultiTargets == null) {
					Debug.LogError("Could not create editor!");
				}

				return editorInstanceMultiTargets;
			}
		}


		protected Editor EditorInstanceSingleTarget {
			get {
				if (editorInstanceSingleTarget == null && target != null) {
					// HACK: Unity has this magic field m_AllowMultiObjectAccess that suppresses usage of SerializedObject or targets in some rare cases like OnSceneGUI.
					var editorType = typeof(Editor);
					var allowField = editorType.GetField("m_AllowMultiObjectAccess", BindingFlags.Static | BindingFlags.NonPublic);
					var prevValue = allowField.GetValue(null);
					allowField.SetValue(null, true);

					editorInstanceSingleTarget = Editor.CreateEditor(target, decoratedEditorType);

					allowField.SetValue(null, prevValue);
				}

				if (editorInstanceSingleTarget == null) {
					Debug.LogError("Could not create editor!");
				}

				return editorInstanceSingleTarget;
			}
		}

		public DecoratorEditor(string editorTypeName)
		{
			if (!decoratedEditorTypes.TryGetValue(editorTypeName, out this.decoratedEditorType)) {
				this.decoratedEditorType = editorAssembly.GetTypes().Where(t => t.Name == editorTypeName).FirstOrDefault();
				decoratedEditorTypes.Add(editorTypeName, this.decoratedEditorType);
			}

			Init();

			// Check CustomEditor types.
			var originalEditedType = GetCustomEditorType(decoratedEditorType);

			if (originalEditedType != editedObjectType) {
				throw new System.ArgumentException(
					string.Format("Type {0} does not match the editor {1} type {2}",
							  editedObjectType, editorTypeName, originalEditedType));
			}
		}

		private System.Type GetCustomEditorType(System.Type type)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;

			var attributes = type.GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
			var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

			return field.GetValue(attributes[0]) as System.Type;
		}

		private void Init()
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;

			var attributes = this.GetType().GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
			var field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();

			editedObjectType = field.GetValue(attributes[0]) as System.Type;
		}

		void OnDisable()
		{
			if (editorInstanceMultiTargets != null) {
				DestroyImmediate(editorInstanceMultiTargets);
			}
			if (editorInstanceSingleTarget != null) {
				DestroyImmediate(editorInstanceSingleTarget);
			}
		}

		/// <summary>
		/// Delegates a method call with the given name to the decorated editor instance.
		/// </summary>
		protected void CallInspectorMethod(string methodName, bool multiTargets = true)
		{
			CallInspectorMethodWithParams(multiTargets ? EditorInstanceMultiTargets : EditorInstanceSingleTarget, methodName, EMPTY_ARRAY);
		}

		protected void CallInspectorMethodWithParams(Editor decoratedEditorInstance, string methodName, params object[] args)
		{
			MethodInfo method = null;

			// Add MethodInfo to cache
			if (!decoratedMethods.ContainsKey(methodName)) {
				var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

				method = decoratedEditorType.GetMethod(methodName, flags);

				if (method != null) {
					decoratedMethods[methodName] = method;
				} else {
					Debug.LogError(string.Format("Could not find method {0}", methodName));
				}
			} else {
				method = decoratedMethods[methodName];
			}

			if (method != null) {
				method.Invoke(decoratedEditorInstance, args);
			}
		}

		// Copy this method if your desired editor supports it.
		//public virtual void OnSceneGUI()
		//{
		//	CallInspectorMethod("OnSceneGUI", false);
		//}

		public virtual void OnSceneDrag(SceneView sceneView)
		{
			CallInspectorMethodWithParams(EditorInstanceMultiTargets, "OnSceneDrag", sceneView);
		}

		protected override void OnHeaderGUI()
		{
			CallInspectorMethod("OnHeaderGUI");
		}

		public override void OnInspectorGUI()
		{
			EditorInstanceMultiTargets.OnInspectorGUI();
		}

		public override void DrawPreview(Rect previewArea)
		{
			EditorInstanceMultiTargets.DrawPreview(previewArea);
		}

		public override string GetInfoString()
		{
			return EditorInstanceMultiTargets.GetInfoString();
		}

		public override GUIContent GetPreviewTitle()
		{
			return EditorInstanceMultiTargets.GetPreviewTitle();
		}

		public override bool HasPreviewGUI()
		{
			return EditorInstanceMultiTargets.HasPreviewGUI();
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			EditorInstanceMultiTargets.OnInteractivePreviewGUI(r, background);
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			EditorInstanceSingleTarget.OnPreviewGUI(r, background);
		}

		public override void OnPreviewSettings()
		{
			EditorInstanceMultiTargets.OnPreviewSettings();
		}

		public override void ReloadPreviewInstances()
		{
			EditorInstanceMultiTargets.ReloadPreviewInstances();
		}

		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			return EditorInstanceMultiTargets.RenderStaticPreview(assetPath, subAssets, width, height);
		}

		public override bool RequiresConstantRepaint()
		{
			return EditorInstanceMultiTargets.RequiresConstantRepaint();
		}

		public override bool UseDefaultMargins()
		{
			return EditorInstanceMultiTargets.UseDefaultMargins();
		}
	}
}
