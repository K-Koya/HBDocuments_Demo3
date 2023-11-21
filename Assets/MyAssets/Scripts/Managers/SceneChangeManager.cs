using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    void Start()
    {
        IsDontDestroyOnLoad = false;
    }

    /// <summary>nameのシーンへ遷移</summary>
    public void ChangeTo(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>現在のシーンを再読み込みする</summary>
    public void LoadSelf()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>ゲームを終了する</summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
