using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevLocker.Tools
{
	/// <summary>
	/// Some editor helper functions.
	/// </summary>
	public static class EditorGUITools
	{
		public static Rect FixIndent(Rect position, SerializedProperty property)
		{
			if (property.depth > 0) {
				EditorGUI.indentLevel += property.depth;
				position = EditorGUI.IndentedRect(position);
				EditorGUI.indentLevel -= property.depth;
			} else if (EditorGUI.indentLevel == 0) {
				// HACK: When 0 depth and 0 indentLevel (first level field), indentation is a bit wrong.
				position.x += 12.0f;
				position.width -= 12.0f;
			}

			return position;
		}


		public static List<Object> GetReferencesFromSerializedList(SerializedProperty list)
		{
			list = list.Copy();

			int arrayLength = 0;

			list.Next(true); // skip generic field
			list.Next(true); // advance to array size field

			// Get the array size
			arrayLength = list.intValue;

			list.Next(true); // advance to first array index

			// Write values to list
			List<Object> values = new List<Object>(arrayLength);
			int lastIndex = arrayLength - 1;
			for (int i = 0; i < arrayLength; i++) {
				values.Add(list.objectReferenceValue);  // copy the value to the list
				if (i < lastIndex) list.Next(false);    // advance without drilling into children
			}

			return values;
		}
	}

}
