using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField, Tooltip("ポーズ時に実行するメソッド")]
    UnityEvent _onPause = null;

    [SerializeField, Tooltip("ポーズ解除時に実行するメソッド")]
    UnityEvent _onResume = null;


    /// <summary>ポーズ前のtimeScaleを保管</summary>
    float _timeScaleCashe = 1f;


    /// <summary>true : ポーズ中</summary>
    bool _isPause = false;

    /// <summary>true : ポーズ中</summary>
    public bool IsPause => _isPause;


    /// <summary>ポーズ・再開処理を実施</summary>
    /// <param name="doPause"><list><item>true : ポーズ</item><item>false : 再開</item></list></param>
    public void DoPause(bool doPause)
    {
        if (doPause)
        {
            _onPause.Invoke();
            Cursor.visible = true;
            _timeScaleCashe = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            _onResume.Invoke();
            Cursor.visible = false;
            Time.timeScale = _timeScaleCashe;
        }

        _isPause = doPause;
    }

    /// <summary>マウスカーソル表示可否</summary>
    /// <param name="isAppear">true : 表示する</param>
    public void AppearCursor(bool isAppear)
    {
        Cursor.visible = isAppear;
    }
}
