/*エスケープキーでゲーム終了(UnityEditor上では再生終了)*/
using UnityEngine;

public class Exit : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;

#else
		Application.Quit();
		
#endif
        }
    }
}