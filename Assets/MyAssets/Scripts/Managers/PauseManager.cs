using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    /// <summary>true : �|�[�Y��</summary>
    bool _isPause = false;

    /// <summary>true : �|�[�Y��</summary>
    public bool IsPause => _isPause;

    /// <summary>�|�[�Y�E�ĊJ���������{</summary>
    /// <param name="doPause"><list><item>true : �|�[�Y</item><item>false : �ĊJ</item></list></param>
    public void DoPause(bool doPause)
    {
        _isPause = doPause;
    }
}
