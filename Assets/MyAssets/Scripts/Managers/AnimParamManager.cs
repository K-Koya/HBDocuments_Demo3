using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimParamManager : Singleton<AnimParamManager>
{
    [Header("Emotion�p")]
    [SerializeField, Tooltip("�p�����[�^��:DoBlink")]
    string _paramNameDoBlink = "DoBlink";


    [Header("Humanoid����p")]
    [SerializeField, Tooltip("�p�����[�^��:Speed")]
    string _paramNameSpeed = "Speed";

    [SerializeField, Tooltip("�p�����[�^��:IsJump")]
    string _paramNameIsJump = "IsJump";

    [SerializeField, Tooltip("�p�����[�^��:IsGround")]
    string _paramNameIsGround = "IsGround";

    [SerializeField, Tooltip("�p�����[�^��:IsBrake")]
    string _paramNameIsBrake = "IsBrake";

    [SerializeField, Tooltip("�p�����[�^��:DoSwitchGravity")]
    string _paramNameDoSwitchGravity = "DoSwitchGravity";

    [SerializeField, Tooltip("�p�����[�^��:DoSideFlip")]
    string _paramNameDoSideFlip = "DoSideFlip";

    [SerializeField, Tooltip("�p�����[�^��:DoRunOver")]
    string _paramNameDoRunOver = "DoRunOver";


    [Header("��������p")]
    [SerializeField, Tooltip("�X�e�[�g�� : LandingBeforeGlide")]
    string _stateNameLandingBeforeGlide = "LandingBeforeGlide";

    [SerializeField, Tooltip("�X�e�[�g�� : JumpToGoalGate")]
    string _stateNameJumpToGoalGate = "JumpToGoalGate";

    [SerializeField, Tooltip("�X�e�[�g�� : Glide")]
    string _stateNameGlide = "Glide";


    #region �v���p�e�B
    /// <summary>�p�����[�^��:DoBlink</summary>
    public string ParamNameDoBlink => _paramNameDoBlink;
    /// <summary>�p�����[�^��:Speed</summary>
    public string ParamNameSpeed => _paramNameSpeed;
    /// <summary>�p�����[�^��:IsJump</summary>
    public string ParamNameIsJump => _paramNameIsJump;
    /// <summary>�p�����[�^��:IsGround</summary>
    public string ParamNameIsGround => _paramNameIsGround;
    /// <summary>�p�����[�^��:IsBrake</summary>
    public string ParamNameIsBrake => _paramNameIsBrake;
    /// <summary>�p�����[�^��:DoSwitchGravity</summary>
    public string ParamNameDoSwitchGravity => _paramNameDoSwitchGravity;
    /// <summary>�p�����[�^��:DoSideFlip</summary>
    public string ParamNameDoSideFlip => _paramNameDoSideFlip;
    /// <summary>�p�����[�^��:DoRunOver</summary>
    public string ParamNameDoRunOver => _paramNameDoRunOver;
    /// <summary>�p�����[�^��:LandingBeforeGlide</summary>
    public string StateNameLandingBeforeGlide => _stateNameLandingBeforeGlide;
    /// <summary>�p�����[�^��:JumpToGoalGate</summary>
    public string StateNameJumpToGoalGate => _stateNameJumpToGoalGate;
    /// <summary>�p�����[�^��:Glide</summary>
    public string StateNameGlide => _stateNameGlide;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        IsDontDestroyOnLoad = true;
    }
}
