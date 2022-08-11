using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BrunoMikoski.SceneHierarchyKeeper
{
    [InitializeOnLoad]
    public static class SceneStateKeeper
    {
        private const string HIERARCHY_DATA_STORAGE_KEY = "_cachedHierarchyData_storage_key";
        private const string FRESH_BOOT_KEY = "fresh_boot_key";
        private const int CHILD_LIST_CAPACITY = 512;

        private static SceneData cachedSceneData;
        private static SceneData SceneData
        {
            get
            {
                if (cachedSceneData == null)
                    cachedSceneData = LoadOrCreateData();
                return cachedSceneData;
            }
        }
        
        private static readonly Dictionary<Transform, string> SceneItemsCache = new Dictionary<Transform, string>();
        private static readonly Dictionary<string, GameObject> PathToGameObjectsCache = new Dictionary<string, GameObject>();
        private static readonly List<Transform> ChildListTransform = new List<Transform>(CHILD_LIST_CAPACITY);
        private static readonly Dictionary<Scene, List<GameObject>> SelectionHistory = new Dictionary<Scene, List<GameObject>>();
        private static readonly HashSet<Scene> ClearedSceneChangeLists = new HashSet<Scene>();
       
        static SceneStateKeeper()
        {
            if (Application.isBatchMode)
                return;
            
            EditorSceneManager.sceneOpened += OnSceneOpened;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EditorSceneManager.sceneClosing += OnSceneClosing;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.quitting += OnEditorApplicationQuitting;
            
            if (SessionState.GetBool(FRESH_BOOT_KEY, true))
            {
                EditorApplication.update += RestoreFromAllOpenScenes;
                SessionState.SetBool(FRESH_BOOT_KEY, false);
            }

        }

        private static void OnEditorApplicationQuitting()
        {
            SaveFromAllOpenScenes();
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            StoreScenedData(scene);
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            RestoreSceneData(scene);
        }

        private static void OnSceneClosing(Scene scene, bool removingscene)
        {
            StoreScenedData(scene);
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            RestoreSceneData(scene);
        }
        
        private static void SaveFromAllOpenScenes()
        {
            if (!UnityHierarchyTools.IsHierarchyWindowOpen())
                return;
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                StoreScenedData(SceneManager.GetSceneAt(i));
            }
        }
        
        private static void RestoreFromAllOpenScenes()
        {
            EditorApplication.update -= RestoreFromAllOpenScenes;

            if (!UnityHierarchyTools.IsHierarchyWindowOpen())
                return;
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                RestoreSceneData(SceneManager.GetSceneAt(i));
            }
        }

        private static void OnSelectionChanged()
        {
            if (!SceneKeeperTools.IsSelectionKeeperActive())
                return;
            
            if (!UnityHierarchyTools.IsHierarchyWindowOpen())
                return;

            if (Selection.objects.Length == 0)
            {
                if (EditorWindow.focusedWindow == UnityHierarchyTools.HierarchyWindow)
                {
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        Scene scene = SceneManager.GetSceneAt(i);
                        if (!SelectionHistory.ContainsKey(scene))
                            continue;
                        
                        SelectionHistory[scene].Clear();
                    }
                }
                return;
            }
            
            ClearedSceneChangeLists.Clear();
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                Object selectedObject = Selection.objects[i];
                if (selectedObject is GameObject selectedGameObject)
                {
                    Scene selectedGameObjectScene = selectedGameObject.scene;

                    if (!SelectionHistory.ContainsKey(selectedGameObjectScene))
                    {
                        SelectionHistory.Add(selectedGameObjectScene, new List<GameObject>());
                    }
                    else
                    {
                        if (!ClearedSceneChangeLists.Contains(selectedGameObjectScene))
                        {
                            SelectionHistory[selectedGameObjectScene].Clear();
                            ClearedSceneChangeLists.Add(selectedGameObjectScene);
                        }
                    }

                    SelectionHistory[selectedGameObjectScene].Add(selectedGameObject);
                }
            }
        }

        private static void RestoreSceneData(Scene scene)
        {
            if (scene.buildIndex == -1)
                return;

            if (!UnityHierarchyTools.IsHierarchyWindowOpen())
                return;

            HashSet<string> alreadySelectedGameObjectPaths = new HashSet<string>();

            RestoreSelectionData(scene, ref alreadySelectedGameObjectPaths);
            RestoreHierarchyData(scene, ref alreadySelectedGameObjectPaths);
        }

        private static void RestoreHierarchyData(Scene scene, ref HashSet<string> alreadySelectedGameObjectPaths)
        {
            if (!SceneKeeperTools.IsHierarchyKeeperActive()) 
                return;

            for (int i = 0; i < SceneData.alwaysExpanded.Count; i++)
            {
                string alwaysExpandedItemPath = SceneData.alwaysExpanded[i];
                if (TryToFindInAllOpenScenes(alwaysExpandedItemPath, out GameObject targetGameObject))
                {
                    alreadySelectedGameObjectPaths.Add(alwaysExpandedItemPath);
                    UnityHierarchyTools.SetExpanded(targetGameObject.GetInstanceID(), true);
                }
            }
            
            if (!SceneData.TryGetSceneData(scene.path, out HierarchyData sceneHierarchyData)) 
                return;

            UnityHierarchyTools.SetExpanded(scene.handle, true);

            for (int i = 0; i < sceneHierarchyData.itemsPath.Count; i++)
            {
                string expandedItemPath = sceneHierarchyData.itemsPath[i];

                if (alreadySelectedGameObjectPaths.Contains(expandedItemPath))
                    continue;

                if (!TryToFindBySceneRootObjects(scene, expandedItemPath, out GameObject gameObjectAtPath))
                    continue;

                UnityHierarchyTools.SetExpanded(gameObjectAtPath.GetInstanceID(), true);
            }
        }

        private static void RestoreSelectionData(Scene scene, ref HashSet<string> alreadySelectedGameObjectPaths)
        {
            if (!SceneKeeperTools.IsSelectionKeeperActive()) 
                return;

            if (!SceneData.TryGetSceneSelectionData(scene.path, out SelectionData resultSelectionData)) 
                return;
            
            List<Object> selectedObjects = new List<Object>();
            for (int i = 0; i < resultSelectionData.itemPath.Count; i++)
            {
                string targetItemPath = resultSelectionData.itemPath[i];
                if (TryToFindBySceneRootObjects(scene, targetItemPath, out GameObject targetGameObject))
                {
                    selectedObjects.Add(targetGameObject);
                    alreadySelectedGameObjectPaths.Add(targetItemPath);
                }
            }

            if (selectedObjects.Count > 0)
                Selection.objects = selectedObjects.ToArray();
        }

        private static void StoreScenedData(Scene targetScene)
        {
            if (targetScene.buildIndex == -1)
                return;

            if (!UnityHierarchyTools.IsHierarchyWindowOpen())
                return;

            HashSet<GameObject> alreadySelectedGameObjects = new HashSet<GameObject>();
            StoreSelectionData(targetScene, ref alreadySelectedGameObjects);
            StoreHierarchyData(targetScene, ref alreadySelectedGameObjects);
            
            SaveData();
        }

        private static void StoreHierarchyData(Scene targetScene, ref HashSet<GameObject> alreadySelectedGameObjects)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) 
                return;

            if (!SceneKeeperTools.IsHierarchyKeeperActive()) 
                return;

            int[] expandedItemIDs = UnityHierarchyTools.GetExpandedItems();

            HierarchyData data = SceneData.GetOrAddSceneData(targetScene.path);
            data.itemsPath.Clear();

            for (int i = 0; i < expandedItemIDs.Length; i++)
            {
                int instanceID = expandedItemIDs[i];
                Object targetObj = EditorUtility.InstanceIDToObject(instanceID);

                if (targetObj == null)
                    continue;

                if (targetObj.hideFlags.HasFlag(HideFlags.HideInHierarchy)
                    || targetObj.hideFlags.HasFlag(HideFlags.NotEditable)
                    || targetObj.hideFlags.HasFlag(HideFlags.HideAndDontSave))
                    continue;

                if (targetObj is GameObject gameObject)
                {
                    if (alreadySelectedGameObjects.Contains(gameObject))
                        continue;

                    string scenePath = gameObject.scene.path;

                    if (!string.Equals(targetScene.path, scenePath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    data.itemsPath.Add(gameObject.transform.GetPath());
                }
            }
        }

        private static void StoreSelectionData(Scene targetScene, ref HashSet<GameObject> alreadySelectedGameObjects)
        {
            if (!SceneKeeperTools.IsSelectionKeeperActive()) 
                return;

            if (Application.isPlaying && SceneKeeperTools.IsIgnoringPlaytimeSelection())
                return;

            if (!TryGetLastValidSelectionForScene(targetScene, out List<GameObject> selectedGameObjects)) 
                return;
            
            SelectionData sceneSelectionData = SceneData.GetOrAddSceneSelectionData(targetScene.path);
            sceneSelectionData.itemPath.Clear();

            for (int i = 0; i < selectedGameObjects.Count; i++)
            {
                GameObject selectedGameObject = selectedGameObjects[i];
                if (selectedGameObject == null)
                    continue;

                sceneSelectionData.itemPath.Add(selectedGameObject.transform.GetPath());

                alreadySelectedGameObjects.Add(selectedGameObject);
            }
        }

        private static bool TryGetLastValidSelectionForScene(Scene targetScene, out List<GameObject> resultGameObjects)
        {
            return SelectionHistory.TryGetValue(targetScene, out resultGameObjects);
        }

        private static bool TryToFindInAllOpenScenes(string targetItemPath, out GameObject resultGameObject)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (TryToFindBySceneRootObjects(SceneManager.GetSceneAt(i), targetItemPath, out resultGameObject))
                    return true;
            }
            
            resultGameObject = null;
            return false;
        }

        private static bool TryToFindBySceneRootObjects(Scene scene, string targetItemPath, out GameObject resultGameObject)
        {
            if (PathToGameObjectsCache.TryGetValue(targetItemPath, out resultGameObject))
            {
                if (resultGameObject != null)
                    return true;
                
                PathToGameObjectsCache.Remove(targetItemPath);
            }
            
            resultGameObject = GameObject.Find(targetItemPath);
            if (resultGameObject != null)
            {
                PathToGameObjectsCache.Add(targetItemPath, resultGameObject);
                return true;
            }
            
            GameObject[] objects = scene.GetRootGameObjects();
            for (int i = 0; i < objects.Length; i++)
            {
                GameObject rootGameObject = objects[i];

                rootGameObject.GetComponentsInChildren(true, ChildListTransform);
                for (int j = 0; j < ChildListTransform.Count; j++)
                {
                    Transform transform = ChildListTransform[j];
                    if (!SceneItemsCache.TryGetValue(transform, out string itemPath))
                    {
                        itemPath = transform.GetPath();
                        SceneItemsCache.Add(transform, itemPath);
                    }
                    
                    if (itemPath.Equals(targetItemPath, StringComparison.OrdinalIgnoreCase))
                    {
                        resultGameObject = transform.gameObject;
                        PathToGameObjectsCache.Add(targetItemPath, resultGameObject);
                        return true;
                    }
                }
            }
            return false;
        }
        
        private static string GetPath(this Transform transform)
        {
            if (transform.parent == null)
                return transform.name;
            
            return GetPath(transform.parent) + "/" + transform.name;
        }

        private static void SaveData()
        {
            string json = EditorJsonUtility.ToJson(SceneData);
            EditorPrefs.SetString($"{Application.productName}{HIERARCHY_DATA_STORAGE_KEY}", json);
        }
        
        private static SceneData LoadOrCreateData()
        {
            SceneData instance = new SceneData();

            string hierarchyDataJson =
                EditorPrefs.GetString($"{Application.productName}{HIERARCHY_DATA_STORAGE_KEY}", "");
            if (!string.IsNullOrEmpty(hierarchyDataJson))
            {
                instance = JsonUtility.FromJson<SceneData>(hierarchyDataJson);
            }

            return instance;
        }

        public static bool IsObjectsExpanded(params Object[] objects)
        {
            GameObject[] gameObjects = new GameObject[objects.Length];
            for (int i = 0; i < objects.Length; i++)
            {
                gameObjects[i] = (GameObject) objects[i];
            }

            return IsObjectsExpanded(gameObjects);
        }
        
        public static bool IsObjectsExpanded(params GameObject[] gameObjects)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                GameObject gameObject = gameObjects[i];
                if (gameObject == null)
                    continue;
                
                if (!SceneData.alwaysExpanded.Contains(gameObject.transform.GetPath()))
                    return false;
            }

            return true;
        }

        public static void SetAlwaysExpanded(bool alwaysExpanded, params Object[] objects)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                Object obj = objects[i];
                if (obj is GameObject gameObject)
                {
                    string gameObjectPath = gameObject.transform.GetPath();
                    if (alwaysExpanded)
                    {
                        if(!SceneData.alwaysExpanded.Contains(gameObjectPath))
                            SceneData.alwaysExpanded.Add(gameObjectPath);
                    }
                    else
                    {
                        SceneData.alwaysExpanded.Remove(gameObjectPath);
                    }
                }
            }
            SaveData();
        }
    } 
}
