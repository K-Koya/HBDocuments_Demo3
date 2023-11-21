using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class TitleManager : MonoBehaviour
{
    /// <summary>true : 既に一度タイトル画面を表示した</summary>
    static bool _WasAppearTitle = false;

    [SerializeField, Tooltip("タイトル時表示用カメラ")]
    CinemachineVirtualCamera _camera = null;

    [SerializeField, Tooltip("タイトル用UI")]
    GameObject _titleUI = null;

    [SerializeField, Tooltip("タイトル画面を閉じるときに行う動作")]
    UnityEvent _onCloseTitle = null;


    void Start()
    {
        if (_WasAppearTitle)
        {
            OnCloseTitle();
        }
        else
        {
            _camera.gameObject.SetActive(true);
            _titleUI.SetActive(true);
            Cursor.visible = true;
            enabled = true;
            _WasAppearTitle = true;
            StageManagerBase.Current.State = StateOnStage.Title;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(InputUtility.GetDownPause || InputUtility.GetDownDecide)
        {
            OnCloseTitle();
        }
    }

    /// <summary>タイトルUIを閉じる</summary>
    public void OnCloseTitle()
    {
        _camera.gameObject.SetActive(false);
        _titleUI.SetActive(false);
        Cursor.visible = false;
        enabled = false;
        StageManagerBase.Current.State = StateOnStage.IntroduceDemo;
    }
}
