using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding {
	/// <summary>Helper for creating editors</summary>
	[CustomEditor(typeof(VersionedMonoBehaviour), true)]
	[CanEditMultipleObjects]
	public class EditorBase : Editor {
		static System.Collections.Generic.Dictionary<string, string> cachedTooltips;
		static System.Collections.Generic.Dictionary<string, string> cachedURLs;
		Dictionary<string, SerializedProperty> props = new Dictionary<string, SerializedProperty>();
		Dictionary<string, string> localTooltips = new Dictionary<string, string>();

		static GUIContent content = new GUIContent();
		static GUIContent showInDocContent = new GUIContent("Show in online documentation", "");
		static GUILayoutOption[] noOptions = new GUILayoutOption[0];
		public static System.Func<string> getDocumentationURL;

		protected HashSet<string> remainingUnhandledProperties;

		static void LoadMeta () {
			if (cachedTooltips == null) {
				var filePath = EditorResourceHelper.editorAssets + "/tooltips.tsv";

				try {
					cachedURLs = System.IO.File.ReadAllLines(filePath).Select(l => l.Split('\t')).Where(l => l.Length == 2).ToDictionary(l => l[0], l => l[1]);
					cachedTooltips = new System.Collections.Generic.Dictionary<string, string>();
				} catch {
					cachedURLs = new System.Collections.Generic.Dictionary<string, string>();
					cachedTooltips = new System.Collections.Generic.Dictionary<string, string>();
				}
			}
		}

		static string FindURL (System.Type type, string path) {
			// Find the correct type if the path was not an immediate member of #type
			while (true) {
				var index = path.IndexOf('.');
				if (index == -1) break;
				var fieldName = path.Substring(0, index);
				var remaining = path.Substring(index + 1);
				var field = type.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				if (field != null) {
					type = field.FieldType;
					path = remaining;
				} else {
					// Could not find the correct field
					return null;
				}
			}

			// Find a documentation entry for the field, fall back to parent classes if necessary
			while (type != null) {
				var url = FindURL(type.FullName + "." + path);
				if (url != null) return url;
				type = type.BaseType;
			}
			return null;
		}

		static string FindURL (string path) {
			LoadMeta();
			string url;
			cachedURLs.TryGetValue(path, out url);
			return url;
		}

		static string FindTooltip (string path) {
			LoadMeta();

			string tooltip;
			cachedTooltips.TryGetValue(path, out tooltip);
			return tooltip;
		}

		string FindLocalTooltip (string path) {
			string result;

			if (!localTooltips.TryGetValue(path, out result)) {
				var fullPath = target.GetType().Name + "." + path;
				result = localTooltips[path] = FindTooltip(fullPath);
			}
			return result;
		}

		protected virtual void OnEnable () {
			foreach (var target in targets) if (target != null) (target as IVersionedMonoBehaviourInternal).UpgradeFromUnityThread();
		}

		public sealed override void OnInspectorGUI () {
			EditorGUI.indentLevel = 0;
			serializedObject.Update();
			try {
				Inspector();
				InspectorForRemainingAttributes(false, true);
			} catch (System.Exception e) {
				// This exception type should never be caught. See https://docs.unity3d.com/ScriptReference/ExitGUIException.html
				if (e is ExitGUIException) throw e;
				Debug.LogException(e, target);
			}
			serializedObject.ApplyModifiedProperties();
			if (targets.Length == 1 && (target as MonoBehaviour).enabled) {
				var attr = target.GetType().GetCustomAttributes(typeof(UniqueComponentAttribute), true);
				for (int i = 0; i < attr.Length; i++) {
					string tag = (attr[i] as UniqueComponentAttribute).tag;
					foreach (var other in (target as MonoBehaviour).GetComponents<MonoBehaviour>()) {
						if (!other.enabled || other == target) continue;
						if (other.GetType().GetCustomAttributes(typeof(UniqueComponentAttribute), true).Select(c => (c as UniqueComponentAttribute).tag == tag).Any()) {
							EditorGUILayout.HelpBox("This component and " + other.GetType().Name + " cannot be used at the same time", MessageType.Warning);
						}
					}
				}
			}
		}

		protected virtual void Inspector () {
			InspectorForRemainingAttributes(true, false);
		}

		/// <summary>Draws an inspector for all fields that are likely not handled by the editor script itself</summary>
		protected virtual void InspectorForRemainingAttributes (bool showHandled, bool showUnhandled) {
			if (remainingUnhandledProperties == null) {
				remainingUnhandledProperties = new HashSet<string>();

				var tp = serializedObject.targetObject.GetType();
				var handledAssemblies = new List<System.Reflection.Assembly>();

				// Find all types for which we have a [CustomEditor(type)] attribute.
				// Unity hides this field, so we have to use reflection to get it.
				var customEditorAttrs = this.GetType().GetCustomAttributes(typeof(CustomEditor), true).Cast<CustomEditor>().ToArray();
				foreach (var attr in customEditorAttrs) {
					var inspectedTypeField = attr.GetType().GetField("m_InspectedType", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					var inspectedType = inspectedTypeField.GetValue(attr) as System.Type;
					if (!handledAssemblies.Contains(inspectedType.Assembly)) {
						handledAssemblies.Add(inspectedType.Assembly);
					}
				}
				bool enterChildren = true;
				for (var prop = serializedObject.GetIterator(); prop.NextVisible(enterChildren); enterChildren = false) {
					var name = prop.propertyPath;
					var field = tp.GetField(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if (field == null) {
						// Can happen for some built-in Unity fields. They are not important
						continue;
					} else {
						var declaringType = field.DeclaringType;
						var foundOtherAssembly = false;
						var foundThisAssembly = false;
						while (declaringType != null) {
							if (handledAssemblies.Contains(declaringType.Assembly)) {
								foundThisAssembly = true;
								break;
							} else {
								foundOtherAssembly = true;
							}
							declaringType = declaringType.BaseType;
						}
						if (foundOtherAssembly && foundThisAssembly) {
							// This is a field in a class in a different assembly, which inherits from a class in one of the handled assemblies.
							// That probably means the editor script doesn't explicitly know about that field and we should show it anyway.
							remainingUnhandledProperties.Add(prop.propertyPath);
						}
					}
				}
			}

			// Basically the same as DrawDefaultInspector, but with tooltips
			bool enterChildren2 = true;

			for (var prop = serializedObject.GetIterator(); prop.NextVisible(enterChildren2); enterChildren2 = false) {
				var handled = !remainingUnhandledProperties.Contains(prop.propertyPath);
				if ((showHandled && handled) || (showUnhandled && !handled)) {
					PropertyField(prop.propertyPath);
				}
			}
		}

		protected SerializedProperty FindProperty (string name) {
			if (!props.TryGetValue(name, out SerializedProperty res)) res = props[name] = serializedObject.FindProperty(name);
			if (res == null) throw new System.ArgumentException(name);
			return res;
		}

		protected void Section (string label) {
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
		}

		/// <summary>Bounds field using center/size instead of center/extent</summary>
		protected void BoundsField (string propertyPath) {
			PropertyField(propertyPath + ".m_Center", "Center");
			var extentsProp = FindProperty(propertyPath + ".m_Extent");
			var r = EditorGUILayout.GetControlRect();
			var label = EditorGUI.BeginProperty(r, new GUIContent("Size"), extentsProp);
			extentsProp.vector3Value = 0.5f * EditorGUI.Vector3Field(r, label, extentsProp.vector3Value * 2.0f);
			EditorGUI.EndProperty();
		}

		protected void FloatField (string propertyPath, string label = null, string tooltip = null, float min = float.NegativeInfinity, float max = float.PositiveInfinity) {
			PropertyField(propertyPath, label, tooltip);
			Clamp(propertyPath, min, max);
		}

		protected void FloatField (SerializedProperty prop, string label = null, string tooltip = null, float min = float.NegativeInfinity, float max = float.PositiveInfinity) {
			PropertyField(prop, label, tooltip);
			Clamp(prop, min, max);
		}

		protected bool PropertyField (string propertyPath, string label = null, string tooltip = null) {
			return PropertyField(FindProperty(propertyPath), label, tooltip, propertyPath);
		}

		protected bool PropertyField (SerializedProperty prop, string label = null, string tooltip = null) {
			return PropertyField(prop, label, tooltip, prop.propertyPath);
		}

		bool PropertyField (SerializedProperty prop, string label, string tooltip, string propertyPath) {
			content.text = label ?? prop.displayName;
			content.tooltip = tooltip ?? FindTooltip(propertyPath);
			var contextClick = IsContextClick();
			EditorGUILayout.PropertyField(prop, content, true, noOptions);
			// Disable context clicking on arrays (as Unity has its own very useful context menu for the array elements)
			if (contextClick && !prop.isArray && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
			return prop.propertyType == SerializedPropertyType.Boolean ? !prop.hasMultipleDifferentValues && prop.boolValue : true;
		}

		bool IsContextClick () {
			// Capturing context clicks turned out to be a bad idea.
			// It prevents things like reverting to prefab values and other nice things.
			return false;
			// return Event.current.type == EventType.ContextClick;
		}

		void CaptureContextClick (string propertyPath) {
			var url = FindURL(target.GetType(), propertyPath);

			if (url != null && getDocumentationURL != null) {
				Event.current.Use();
				var menu = new GenericMenu();
				menu.AddItem(showInDocContent, false, () => Application.OpenURL(getDocumentationURL() + url));
				menu.ShowAsContext();
			}
		}

		protected void Popup (string propertyPath, GUIContent[] options, string label = null) {
			var prop = FindProperty(propertyPath);

			content.text = label ?? prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			var contextClick = IsContextClick();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
			int newVal = EditorGUILayout.Popup(content, prop.propertyType == SerializedPropertyType.Enum ? prop.enumValueIndex : prop.intValue, options);
			if (EditorGUI.EndChangeCheck()) {
				if (prop.propertyType == SerializedPropertyType.Enum) prop.enumValueIndex = newVal;
				else prop.intValue = newVal;
			}
			EditorGUI.showMixedValue = false;
			if (contextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) CaptureContextClick(propertyPath);
		}

		protected void Mask (string propertyPath, string[] options, string label = null) {
			var prop = FindProperty(propertyPath);

			content.text = label ?? prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			var contextClick = IsContextClick();
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = prop.hasMultipleDifferentValues;
			int newVal = EditorGUILayout.MaskField(content, prop.intValue, options);
			if (EditorGUI.EndChangeCheck()) {
				prop.intValue = newVal;
			}
			EditorGUI.showMixedValue = false;
			if (contextClick && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) CaptureContextClick(propertyPath);
		}

		protected void IntSlider (string propertyPath, int left, int right) {
			var contextClick = IsContextClick();
			var prop = FindProperty(propertyPath);

			content.text = prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			EditorGUILayout.IntSlider(prop, left, right, content, noOptions);
			if (contextClick && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
		}

		protected void Slider (string propertyPath, float left, float right) {
			var contextClick = IsContextClick();
			var prop = FindProperty(propertyPath);

			content.text = prop.displayName;
			content.tooltip = FindTooltip(propertyPath);
			EditorGUILayout.Slider(prop, left, right, content, noOptions);
			if (contextClick && Event.current.type == EventType.Used) CaptureContextClick(propertyPath);
		}

		protected void Clamp (SerializedProperty prop, float min, float max = float.PositiveInfinity) {
			if (!prop.hasMultipleDifferentValues) prop.floatValue = Mathf.Clamp(prop.floatValue, min, max);
		}

		protected void Clamp (string name, float min, float max = float.PositiveInfinity) {
			Clamp(FindProperty(name), min, max);
		}

		protected void ClampInt (string name, int min, int max = int.MaxValue) {
			var prop = FindProperty(name);

			if (!prop.hasMultipleDifferentValues) prop.intValue = Mathf.Clamp(prop.intValue, min, max);
		}
	}
}
