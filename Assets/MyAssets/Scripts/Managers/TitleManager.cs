using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class TitleManager : MonoBehaviour
{
    /// <summary>true : ���Ɉ�x�^�C�g����ʂ�\������</summary>
    static bool _WasAppearTitle = false;

    [SerializeField, Tooltip("�^�C�g�����\���p�J����")]
    CinemachineVirtualCamera _camera = null;

    [SerializeField, Tooltip("�^�C�g���pUI")]
    GameObject _titleUI = null;

    [SerializeField, Tooltip("�^�C�g����ʂ����Ƃ��ɍs������")]
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

    /// <summary>�^�C�g��UI�����</summary>
    public void OnCloseTitle()
    {
        _camera.gameObject.SetActive(false);
        _titleUI.SetActive(false);
        Cursor.visible = false;
        enabled = false;
        StageManagerBase.Current.State = StateOnStage.IntroduceDemo;
    }
}
