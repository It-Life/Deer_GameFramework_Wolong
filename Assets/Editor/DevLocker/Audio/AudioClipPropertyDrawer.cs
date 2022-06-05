using UnityEditor;
using UnityEngine;

namespace DevLocker.Tools.Audio
{
	/// <summary>
	/// Draw "P" play button next to the reference.
	/// </summary>
	[CustomPropertyDrawer(typeof(AudioClip))]
	public class AudioClipPropertyDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			const float PLAY_BTN_WIDTH = 20.0f;
			const float PADDING = 4.0f;

			var refRect = new Rect(position.position, new Vector2(position.width - PLAY_BTN_WIDTH - PADDING, EditorGUIUtility.singleLineHeight));
			var playBtnRect = new Rect(position.position, new Vector2(PLAY_BTN_WIDTH, EditorGUIUtility.singleLineHeight));
			playBtnRect.x += refRect.width + PADDING;

			EditorGUI.PropertyField(refRect, property, GUIContent.none);
			if (GUI.Button(playBtnRect, "P") && property.objectReferenceValue) {
				AudioEditorUtils.PlayClip((AudioClip) property.objectReferenceValue);
			}

			EditorGUI.EndProperty();
		}
	}

}
