
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevLocker.Tools
{
	/// <summary>
	/// Inherit and implement this property drawer to draw all desired properties as one line (useful for lists).
	/// Examples:
	/*
	[CustomPropertyDrawer(typeof(MyClass))]
	public class MyClassPropertyDrawer : OneLineBasePropertyDrawer
	{
		protected override PropertyDescriptor[] Properties => new[]
		{
			new PropertyDescriptor("MyProp1", 1.0f, true),
			new PropertyDescriptor("MyProp2", new GUIContent("Pretty Prop Name", "Tooltip text"), 2.0f),

		};
	}
	*/
	///
	/// Note: override CompactElementLabels property to control if the drawer should compress "Element 12" text when shown in list.
	/// </summary>
	public abstract class OneLineBasePropertyDrawer : PropertyDrawer
	{
		protected struct PropertyDescriptor
		{
			public PropertyDescriptor(string propertyName, float widthRatio = 1.0f, bool drawAsObjectField = false, GUIContent label = null, float labelWidth = 30.0f)
			{
				PropertyName = propertyName;
				WidthRatio = widthRatio;
				DrawAsObjectField = drawAsObjectField;
				Label = label;
				LabelWidth = labelWidth;
			}

			public PropertyDescriptor(string propertyName, float widthRatio, GUIContent label, float labelWidth = 30.0f)
				: this(propertyName, widthRatio, false, label, labelWidth)
			{
			}
			public PropertyDescriptor(string propertyName, GUIContent label, float labelWidth = 30.0f)
				: this(propertyName, 1.0f, label, labelWidth)
			{
			}

			public string PropertyName;
			public float WidthRatio;
			public bool DrawAsObjectField;  // If true, will use EditorGUI.ObjectField() which might fix issues with indents.
			public GUIContent Label;
			public float LabelWidth;
		}

		protected virtual float Padding => 4.0f;
		protected virtual bool CompactElementLabels => true;
		protected abstract PropertyDescriptor[] Properties { get; }

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (CompactElementLabels) {
				if (label.text.StartsWith("Element")) {
					EditorGUIUtility.labelWidth = 40.0f;
					label.text = label.text.Replace("Element", "El.");

				} else if (property.propertyPath.LastOrDefault() == ']') {  // Array element: YourType.Array.data[0]

					// If your class / struct starts with a string field,
					// Unity takes the string value as label instead of "Element XX".
					// It's a nice feature, but doesn't look good in our situation.
					EditorGUIUtility.labelWidth = 40.0f;
					var leftBracketIndex = property.propertyPath.LastIndexOf('[');
					if (leftBracketIndex != -1) {
						var elementIndex = property.propertyPath.Substring(leftBracketIndex + 1, property.propertyPath.Length - leftBracketIndex - 2);
						label.text = $"El. {elementIndex}";
					}
				}
			}

			label = EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var totalRatio = Properties.Sum(pd => pd.WidthRatio);
			var propertiesOnlyWidth = position.width - (Properties.Length - 1) * Padding;

			var rect = position;
			rect.width = 0.0f;

			for (int i = 0; i < Properties.Length; ++i) {
				var pd = Properties[i];


				var ratio = pd.WidthRatio / totalRatio;
				var width = ratio * propertiesOnlyWidth;

				// Move after the last property rect.
				rect.x += rect.width;

				if (pd.Label != null) {
					rect.width = pd.LabelWidth;

					GUI.Label(rect, pd.Label);

					rect.x += rect.width;
					width -= pd.LabelWidth;
				}


				rect.width = width;

				var targetProperty = property.FindPropertyRelative(pd.PropertyName);
				if (targetProperty == null) {
					Debug.LogError($"Couldn't find property relative with name {pd.PropertyName}");
				}
				if (pd.DrawAsObjectField) {
					EditorGUI.ObjectField(rect, targetProperty, new GUIContent());
				} else {
					EditorGUI.PropertyField(rect, targetProperty, new GUIContent());
				}

				rect.x += Padding;
			}

			EditorGUI.EndProperty();
		}
	}


}
