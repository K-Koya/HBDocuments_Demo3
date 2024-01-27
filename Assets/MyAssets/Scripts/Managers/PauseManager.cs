using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField, Tooltip("�|�[�Y���Ɏ��s���郁�\�b�h")]
    UnityEvent _onPause = null;

    [SerializeField, Tooltip("�|�[�Y�������Ɏ��s���郁�\�b�h")]
    UnityEvent _onResume = null;


    /// <summary>�|�[�Y�O��timeScale��ۊ�</summary>
    float _timeScaleCashe = 1f;


    /// <summary>true : �|�[�Y��</summary>
    bool _isPause = false;

    /// <summary>true : �|�[�Y��</summary>
    public bool IsPause => _isPause;


    /// <summary>�|�[�Y�E�ĊJ���������{</summary>
    /// <param name="doPause"><list><item>true : �|�[�Y</item><item>false : �ĊJ</item></list></param>
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

    /// <summary>�}�E�X�J�[�\���\����</summary>
    /// <param name="isAppear">true : �\������</param>
    public void AppearCursor(bool isAppear)
    {
        Cursor.visible = isAppear;
    }
}
