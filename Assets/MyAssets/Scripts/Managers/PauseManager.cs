using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    /// <summary>true : ポーズ中</summary>
    bool _isPause = false;

    /// <summary>true : ポーズ中</summary>
    public bool IsPause => _isPause;

    /// <summary>ポーズ・再開処理を実施</summary>
    /// <param name="doPause"><list><item>true : ポーズ</item><item>false : 再開</item></list></param>
    public void DoPause(bool doPause)
    {
        _isPause = doPause;
    }
}
