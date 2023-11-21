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

    /// <summary>name�̃V�[���֑J��</summary>
    public void ChangeTo(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>���݂̃V�[�����ēǂݍ��݂���</summary>
    public void LoadSelf()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>�Q�[�����I������</summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
