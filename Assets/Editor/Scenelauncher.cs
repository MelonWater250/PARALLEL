using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using System.Linq;

/// <summary>
/// シーン一覧を表示(shift + s)
/// </summary>
public class SceneLauncher : EditorWindow
{
    private SceneAsset[] sceneArray;
    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("**MyWindow**/Scene Launcher #s")]
    static void Open()
    {
        GetWindow<SceneLauncher>("SceneLauncher");
    }

    void OnFocus()
    {
        ReloadScenes();
    }

    void OnGUI()
    {
        if (sceneArray == null) { ReloadScenes(); }

        if (sceneArray.Length == 0)
        {
            EditorGUILayout.LabelField("シーンファイル(.unity)が存在しません");
            return;
        }

        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var scene in sceneArray)
        {
            var path = AssetDatabase.GetAssetPath(scene);
            if (GUILayout.Button(path))
            {
                EditorSceneManager.OpenScene(path);
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// シーン一覧の再読み込み
    /// </summary>
    void ReloadScenes()
    {
        sceneArray = GetAllSceneAssets().ToArray();
    }

    /// <summary>
    /// プロジェクト内に存在するすべてのシーンファイルを取得する
    /// </summary>
    static IEnumerable<SceneAsset> GetAllSceneAssets()
    {
        return AssetDatabase.FindAssets("t:SceneAsset")
       .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
       .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)))
       .Where(obj => obj != null)
       .Select(obj => (SceneAsset)obj);
    }
}