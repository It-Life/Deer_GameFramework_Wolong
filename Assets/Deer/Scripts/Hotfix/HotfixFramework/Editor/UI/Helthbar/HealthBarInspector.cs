using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;


[CustomEditor(typeof(HealthBar))]
[CanEditMultipleObjects]
public class _levelSelectionLogic : Editor 
{
	bool showSettings = true;
	//bool healthLink = false;
    bool alphaSettings = true;
    bool hitSettings = true;

	public override void OnInspectorGUI()
	{
		HealthBar myTarget = (HealthBar)target;
        Undo.RecordObject(target, "Undo");

		EditorGUILayout.BeginVertical("Box");
			GUILayout.Space(5);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (10);
			//healthLink = EditorGUILayout.Foldout(healthLink,"Health Link");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal ();
/*			if(healthLink)
			{
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(5);
				EditorGUILayout.BeginVertical("Box");
					myTarget.healthLink.targetScript = (MonoBehaviour)EditorGUILayout.ObjectField ("Health Script", myTarget.healthLink.targetScript, typeof(MonoBehaviour), true);
					myTarget.healthLink.fieldName = EditorGUILayout.TextField ("Health var name", myTarget.healthLink.fieldName);
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal ();
			}*/
			myTarget.curHealth = EditorGUILayout.FloatField("Current Health", myTarget.curHealth);
			myTarget.HealthbarPrefab = (RectTransform)EditorGUILayout.ObjectField ("HealthbarPrefab",myTarget.HealthbarPrefab, typeof(RectTransform), false);
			myTarget.yOffset = EditorGUILayout.FloatField ("Y Offset", myTarget.yOffset);
			EditorGUILayout.BeginHorizontal ();
				GUILayout.Space (10);
				showSettings = EditorGUILayout.Foldout(showSettings,"Other Settings");
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal ();


			if(showSettings)
			{
				EditorGUILayout.BeginVertical("Box");
					GUILayout.Space(5);

					myTarget.keepSize = EditorGUILayout.Toggle("Fixed Size", myTarget.keepSize);
					GUILayout.Space(5);

					myTarget.scale = EditorGUILayout.FloatField ("Scale", myTarget.scale);
					myTarget.sizeOffsets = EditorGUILayout.Vector2Field("Size Offsets", myTarget.sizeOffsets);

					EditorGUILayout.BeginHorizontal ();
						myTarget.DrawOFFDistance = EditorGUILayout.ToggleLeft("Draw Distance", myTarget.DrawOFFDistance, GUILayout.Width(100));
						GUILayout.FlexibleSpace();
						if(myTarget.DrawOFFDistance)
							myTarget.drawDistance = EditorGUILayout.FloatField("", myTarget.drawDistance, GUILayout.Width(100));
					EditorGUILayout.EndHorizontal ();
					GUILayout.Space(5);

					myTarget.showHealthInfo = EditorGUILayout.ToggleLeft("Health Info", myTarget.showHealthInfo, GUILayout.Width(100));

					if(myTarget.showHealthInfo)
					{
						myTarget.healthInfoAlignment = (HealthBar.HealthInfoAlignment)EditorGUILayout.EnumPopup("Alignment", myTarget.healthInfoAlignment);
						myTarget.healthInfoSize = EditorGUILayout.FloatField("Size Factor", myTarget.healthInfoSize);
					}
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                        EditorGUILayout.BeginVertical();
                        alphaSettings = EditorGUILayout.Foldout(alphaSettings, "Alpha Settings");
                        if (alphaSettings)
                        {
                            EditorGUILayout.HelpBox("States alphas and it's fade speeds. Zero is no fade.", MessageType.Info);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 45;
                            EditorGUIUtility.fieldWidth = 45;
                            myTarget.alphaSettings.defaultAlpha = EditorGUILayout.Slider("Default", myTarget.alphaSettings.defaultAlpha, 0, 1);
                            EditorGUIUtility.labelWidth = 75;
                            myTarget.alphaSettings.defaultFadeSpeed = EditorGUILayout.FloatField("Fade Speed", myTarget.alphaSettings.defaultFadeSpeed);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 45;
                            EditorGUIUtility.fieldWidth = 45;
                            myTarget.alphaSettings.fullAplpha = EditorGUILayout.Slider("Full ", myTarget.alphaSettings.fullAplpha, 0, 1);
                            EditorGUIUtility.labelWidth = 75;
                            myTarget.alphaSettings.fullFadeSpeed = EditorGUILayout.FloatField("Fade Speed", myTarget.alphaSettings.fullFadeSpeed);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUIUtility.labelWidth = 45;
                            EditorGUIUtility.fieldWidth = 45;
                            myTarget.alphaSettings.nullAlpha = EditorGUILayout.Slider("Null ", myTarget.alphaSettings.nullAlpha, 0, 1);
                            EditorGUIUtility.labelWidth = 75;
                            myTarget.alphaSettings.nullFadeSpeed = EditorGUILayout.FloatField("Fade Speed", myTarget.alphaSettings.nullFadeSpeed);
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(10);
                            EditorGUILayout.BeginVertical();
                            hitSettings = EditorGUILayout.Foldout(hitSettings, "Hit Settings");
                            if (hitSettings)
                            {
                                myTarget.alphaSettings.onHit.onHitAlpha = EditorGUILayout.Slider("On Hit Alpha", myTarget.alphaSettings.onHit.onHitAlpha, 0, 1);
                                myTarget.alphaSettings.onHit.fadeSpeed = EditorGUILayout.FloatField("Fade Speed", myTarget.alphaSettings.onHit.fadeSpeed);
                                myTarget.alphaSettings.onHit.duration = EditorGUILayout.FloatField("Duration", myTarget.alphaSettings.onHit.duration);
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical();
			}
		EditorGUILayout.EndVertical();

		if (GUI.changed)
			EditorUtility.SetDirty(myTarget);
	}
}
