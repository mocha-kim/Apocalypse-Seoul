using System;
using Manager;
using UnityEditor;
using UnityEngine;

namespace Tool
{
    public class SceneInitializer : EditorWindow
    {
        private static GameObject _mainCamera;

        private string sceneName;
        
        [MenuItem("Tools/SceneInitializer")]
        static void Init()
        {
            _mainCamera = ResourceManager.GetPrefab("MainCamera");
            
            var window = (SceneInitializer)GetWindow(typeof(SceneInitializer));
            window.Show();
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            sceneName = EditorGUILayout.TextField("Scene Name", sceneName);

            if (GUILayout.Button("Initialize"))
            {
                Debug.Log(_mainCamera.name);
            }
        }
    }
}
