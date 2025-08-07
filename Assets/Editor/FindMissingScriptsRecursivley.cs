using UnityEditor;
using UnityEngine;

namespace AndroidUltimatePlugin.Helpers.Editor
{
    public class FindMissingScriptsRecursively : EditorWindow
    {
        static bool deleteEmptyScripts = false;
        static int _goCount = 0, _componentsCount = 0, _missingCount = 0;

        [MenuItem("Tools/FindMissingScriptsRecursively")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindMissingScriptsRecursively));
        }

        public void OnGUI()
        {
            deleteEmptyScripts = EditorGUILayout.Toggle("Delete missing scripts", deleteEmptyScripts);

            EditorGUILayout.Space();

            if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
            {
                FindInSelected();
            }

            if (GUILayout.Button("Find Missing Scripts"))
            {
                FindAll();
            }
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Component Scanned:");
                EditorGUILayout.LabelField("" + (_componentsCount == -1 ? "---" : _componentsCount.ToString()));
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Object Scanned:");
                EditorGUILayout.LabelField("" + (_goCount == -1 ? "---" : _goCount.ToString()));
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Possible Missing Scripts:");
                EditorGUILayout.LabelField("" + (_missingCount == -1 ? "---" : _missingCount.ToString()));
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void FindAll()
        {
            _componentsCount = 0;
            _goCount = 0;
            _missingCount = 0;
            
            string[] assetsPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in assetsPaths)
            {
                Object[] data = LoadAllAssetsAtPath(assetPath);
                foreach (Object o in data)
                {
                    if (o != null)
                    {
                        if (o is GameObject)
                        {
                            FindInGO((GameObject) o);
                        }
                    }
                }
            }
            
            Debug.Log($"Searched {_goCount} GameObjects, {_componentsCount} components, found {_missingCount} missing");
        }

        public static Object[] LoadAllAssetsAtPath(string assetPath)
        {
            return typeof(SceneAsset).Equals(AssetDatabase.GetMainAssetTypeAtPath(assetPath))
                ?
                // prevent error "Do not use readobjectthreaded on scene objects!"
                new[] {AssetDatabase.LoadMainAssetAtPath(assetPath)}
                : AssetDatabase.LoadAllAssetsAtPath(assetPath);
        }

        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            _goCount = 0;
            _componentsCount = 0;
            _missingCount = 0;
            foreach (GameObject g in go)
            {

                FindInGO(g);
            }

            Debug.Log($"Searched {_goCount} GameObjects, {_componentsCount} components, found {_missingCount} missing");
        }

        private static void FindInGO(GameObject g)
        {
            _goCount++;
            Component[] components = g.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                _componentsCount++;

                if (components[i] == null)
                {
                    _missingCount++;

                    string path = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        t = t.parent;
                        path = t.name + "/" + path;
                    }

                    Debug.Log($"{path} has a missing script at position {i}", g);

                    if (deleteEmptyScripts)
                    {
                        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g);
                        if (removed > 0)
                        {
                            Debug.Log($"Removed {removed} missing script(s) from {g.name}", g);
                            EditorUtility.SetDirty(g);
                        }
                    }
                }
            }

            // Recurse through children
            foreach (Transform child in g.transform)
            {
                FindInGO(child.gameObject);
            }
        }


    }
}