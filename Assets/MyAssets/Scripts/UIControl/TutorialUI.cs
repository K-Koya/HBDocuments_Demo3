using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField, Tooltip("OK�\������A�C�R��")]
    GameObject _okIcon = null;

    [SerializeField, Tooltip("�J������]����")]
    GameObject _forCameraControl = null;

    [SerializeField, Tooltip("�L�����N�^�[���ʈړ�����")]
    GameObject _forCharacterMove = null;

    [SerializeField, Tooltip("�L�����N�^�[��E�i�����z������")]
    GameObject _forCharacterThroughOver = null;

    [SerializeField, Tooltip("�L�����N�^�[�W�����v����")]
    GameObject _forCharacterJump = null;

    [SerializeField, Tooltip("�L�����N�^�[�u���[�L����")]
    GameObject _forCharacterBrake = null;

    [SerializeField, Tooltip("�L�����N�^�[�����Ԃ葀��")]
    GameObject _forCharacterSideFlip = null;


    // Start is called before the first frame update
    void Start()
    {
        CloseUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>���쐬���\��</summary>
    public void AppearOKIcon(bool isAppear)
    {
        _okIcon.SetActive(isAppear);
    }

    /// <summary>�`���[�g���A���S��\��</summary>
    public void CloseUI()
    {
        _okIcon.SetActive(false);
        _forCameraControl.SetActive(false);
        _forCharacterMove.SetActive(false);
        _forCharacterThroughOver.SetActive(false);
        _forCharacterJump.SetActive(false);
        _forCharacterBrake.SetActive(false);
        _forCharacterSideFlip.SetActive(false);
    }

    /// <summary>�J������]���������\��</summary>
    public void AppearCameraControl(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCameraControl.SetActive(isAppear);
    }

    /// <summary>�L�����N�^�[��E�i�����z�����������\��</summary>
    public void AppearCharacterMove(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterMove.SetActive(isAppear);
    }

    /// <summary>�L�����N�^�[���ʈړ����������\��</summary>
    public void AppearCharacterThroughOver(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterThroughOver.SetActive(isAppear);
    }

    /// <summary>�L�����N�^�[�W�����v���������\��</summary>
    public void AppearCharacterJump(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterJump.SetActive(isAppear);
    }

    /// <summary>�L�����N�^�[�u���[�L���������\��</summary>
    public void AppearCharacterBrake(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterBrake.SetActive(isAppear);
    }

    /// <summary>�L�����N�^�[�����Ԃ葀�������\��</summary>
    public void AppearCharacterSideFlip(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterSideFlip.SetActive(isAppear);
    }
}
