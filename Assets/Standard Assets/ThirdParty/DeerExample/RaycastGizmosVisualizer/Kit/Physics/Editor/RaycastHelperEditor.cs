using UnityEngine;
using UnityEditor;

namespace Kit.Physic
{
	[CustomEditor(typeof(RaycastHelper))]
	public class RaycastHelperEditor : Editor
	{
		SerializedProperty
			rayTypeProp, distanceProp, localPositionProp, radiusProp,
			memoryArraySizeProp, unSyncRotationProp, localRotationProp,
			localPoint1Prop, localPoint2Prop,
			halfExtendsProp, fixedUpdateProp, debugMethodProp, debugLogProp,
			layerMaskProp, queryTriggerInteractionProp,
			colorProp, hitColorProp, onHitProp;
		private void OnEnable()
		{
			rayTypeProp = serializedObject.FindProperty("m_RayType");
			distanceProp = serializedObject.FindProperty("m_Distance");
			localPositionProp = serializedObject.FindProperty("m_LocalPosition");
			localPoint1Prop = serializedObject.FindProperty("m_LocalPoint1");
			localPoint2Prop = serializedObject.FindProperty("m_LocalPoint2");
			radiusProp = serializedObject.FindProperty("m_Radius");
			memoryArraySizeProp = serializedObject.FindProperty("m_MemoryArraySize");
			unSyncRotationProp = serializedObject.FindProperty("m_UnSyncRotation");
			localRotationProp = serializedObject.FindProperty("m_LocalRotation");
			halfExtendsProp = serializedObject.FindProperty("m_HalfExtends");
			fixedUpdateProp = serializedObject.FindProperty("m_FixedUpdate");
			debugMethodProp = serializedObject.FindProperty("m_DebugMethod");
			debugLogProp = serializedObject.FindProperty("m_DebugLog");
			layerMaskProp = serializedObject.FindProperty("m_LayerMask");
			queryTriggerInteractionProp = serializedObject.FindProperty("m_QueryTriggerInteraction");
			colorProp = serializedObject.FindProperty("m_Color");
			hitColorProp = serializedObject.FindProperty("m_HitColor");
			onHitProp = serializedObject.FindProperty("OnHit");
		}
		public override void OnInspectorGUI()
		{
			serializedObject.UpdateIfRequiredOrScript();
			EditorGUI.BeginChangeCheck();

			// EditorGUILayout.PropertyField(rayTypeProp);
			RaycastHelper.eRayType type = (RaycastHelper.eRayType)rayTypeProp.intValue;
			RaycastHelper.eRayType newType = (RaycastHelper.eRayType)EditorGUILayout.EnumPopup(rayTypeProp.displayName, type);
			RaycastHelper instance = serializedObject.targetObject as RaycastHelper;
			if (EditorGUI.EndChangeCheck() && type != newType)
			{
				instance.RayType = newType;
			}

			EditorGUI.BeginChangeCheck();
			switch (type)
			{
				case RaycastHelper.eRayType.Raycast:
					{
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(localPositionProp);
					}
					break;
				case RaycastHelper.eRayType.RaycastAll:
					{
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(localPositionProp);
						DefineMemorySize(instance);
					}
					break;
				case RaycastHelper.eRayType.SphereCast:
					{
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(radiusProp);
					}
					break;
				case RaycastHelper.eRayType.SphereCastAll:
					{
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(radiusProp);
						DefineMemorySize(instance);
					}
					break;
				case RaycastHelper.eRayType.SphereOverlap:
					{
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(radiusProp);
						DefineMemorySize(instance);
					}
					break;
				case RaycastHelper.eRayType.BoxCast:
					{
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(localRotationProp);
						EditorGUILayout.PropertyField(unSyncRotationProp);
						EditorGUILayout.PropertyField(halfExtendsProp);
					}
					break;
				case RaycastHelper.eRayType.BoxCastAll:
					{
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(localRotationProp);
						EditorGUILayout.PropertyField(unSyncRotationProp);
						EditorGUILayout.PropertyField(halfExtendsProp);
						DefineMemorySize(instance);
					}
					break;
				case RaycastHelper.eRayType.BoxOverlap:
					{
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(localRotationProp);
						EditorGUILayout.PropertyField(unSyncRotationProp);
						EditorGUILayout.PropertyField(halfExtendsProp);
						DefineMemorySize(instance);
					}
					break;
				case RaycastHelper.eRayType.CapsuleCast:
					{
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(radiusProp);
						EditorGUILayout.PropertyField(localPoint1Prop);
						EditorGUILayout.PropertyField(localPoint2Prop);
					}
					break;
				case RaycastHelper.eRayType.CapsuleCastAll:
					{
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(distanceProp);
						EditorGUILayout.PropertyField(radiusProp);
						EditorGUILayout.PropertyField(localPoint1Prop);
						EditorGUILayout.PropertyField(localPoint2Prop);
						DefineMemorySize(instance);
					}
					break;
				case RaycastHelper.eRayType.CapsuleOverlap:
					{
						EditorGUILayout.PropertyField(localPositionProp);
						EditorGUILayout.PropertyField(radiusProp);
						EditorGUILayout.PropertyField(localPoint1Prop);
						EditorGUILayout.PropertyField(localPoint2Prop);
						DefineMemorySize(instance);
					}
					break;
				default:
					EditorGUILayout.HelpBox("Non-Implement inspector session.", MessageType.Error);
					break;
			}

			EditorGUILayout.PropertyField(fixedUpdateProp);
			if (fixedUpdateProp.boolValue)
			{
				EditorGUILayout.HelpBox("Fixed update checking result in performance issue,\nrecommend \"CheckPhysic()\" manually.", MessageType.Warning);
			}
			else if (EditorApplication.isPlaying &&
				serializedObject.targetObjects.Length == 1 &&
				GUILayout.Button("Trigger Manually", GUILayout.Height(50f)))
			{
				instance.CheckPhysic(false);
			}

			EditorGUILayout.PropertyField(layerMaskProp);
			EditorGUILayout.PropertyField(queryTriggerInteractionProp);

			EditorGUILayout.PropertyField(debugMethodProp);
			EditorGUILayout.PropertyField(debugLogProp);
			EditorGUILayout.PropertyField(colorProp);
			EditorGUILayout.PropertyField(hitColorProp);

			EditorGUILayout.PropertyField(onHitProp);

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				SceneView.RepaintAll();
			}
		}

		private void DefineMemorySize(RaycastHelper helper)
		{
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				SceneView.RepaintAll();
			}

			EditorGUI.BeginChangeCheck();
			int tmp = EditorGUILayout.IntField(memoryArraySizeProp.displayName, memoryArraySizeProp.intValue);
			if (EditorGUI.EndChangeCheck() && memoryArraySizeProp.intValue != tmp && tmp >= 0)
			{
				helper.SetMemorySize(tmp);
				serializedObject.ApplyModifiedProperties();
				SceneView.RepaintAll();
			}

			if (memoryArraySizeProp.intValue == 0)
			{
				EditorGUILayout.HelpBox(RaycastHelper.ZERO_MEMORY, MessageType.Warning);
			}
			EditorGUI.BeginChangeCheck();
		}
	}
}