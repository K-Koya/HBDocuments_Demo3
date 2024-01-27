using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimParamManager : Singleton<AnimParamManager>
{
    [Header("Emotion用")]
    [SerializeField, Tooltip("パラメータ名:DoBlink")]
    string _paramNameDoBlink = "DoBlink";


    [Header("Humanoid操作用")]
    [SerializeField, Tooltip("パラメータ名:Speed")]
    string _paramNameSpeed = "Speed";

    [SerializeField, Tooltip("パラメータ名:IsJump")]
    string _paramNameIsJump = "IsJump";

    [SerializeField, Tooltip("パラメータ名:IsGround")]
    string _paramNameIsGround = "IsGround";

    [SerializeField, Tooltip("パラメータ名:IsBrake")]
    string _paramNameIsBrake = "IsBrake";

    [SerializeField, Tooltip("パラメータ名:DoSwitchGravity")]
    string _paramNameDoSwitchGravity = "DoSwitchGravity";

    [SerializeField, Tooltip("パラメータ名:DoSideFlip")]
    string _paramNameDoSideFlip = "DoSideFlip";

    [SerializeField, Tooltip("パラメータ名:DoRunOver")]
    string _paramNameDoRunOver = "DoRunOver";


    [Header("自動操作用")]
    [SerializeField, Tooltip("ステート名 : LandingBeforeGlide")]
    string _stateNameLandingBeforeGlide = "LandingBeforeGlide";

    [SerializeField, Tooltip("ステート名 : JumpToGoalGate")]
    string _stateNameJumpToGoalGate = "JumpToGoalGate";

    [SerializeField, Tooltip("ステート名 : Glide")]
    string _stateNameGlide = "Glide";


    #region プロパティ
    /// <summary>パラメータ名:DoBlink</summary>
    public string ParamNameDoBlink => _paramNameDoBlink;
    /// <summary>パラメータ名:Speed</summary>
    public string ParamNameSpeed => _paramNameSpeed;
    /// <summary>パラメータ名:IsJump</summary>
    public string ParamNameIsJump => _paramNameIsJump;
    /// <summary>パラメータ名:IsGround</summary>
    public string ParamNameIsGround => _paramNameIsGround;
    /// <summary>パラメータ名:IsBrake</summary>
    public string ParamNameIsBrake => _paramNameIsBrake;
    /// <summary>パラメータ名:DoSwitchGravity</summary>
    public string ParamNameDoSwitchGravity => _paramNameDoSwitchGravity;
    /// <summary>パラメータ名:DoSideFlip</summary>
    public string ParamNameDoSideFlip => _paramNameDoSideFlip;
    /// <summary>パラメータ名:DoRunOver</summary>
    public string ParamNameDoRunOver => _paramNameDoRunOver;
    /// <summary>パラメータ名:LandingBeforeGlide</summary>
    public string StateNameLandingBeforeGlide => _stateNameLandingBeforeGlide;
    /// <summary>パラメータ名:JumpToGoalGate</summary>
    public string StateNameJumpToGoalGate => _stateNameJumpToGoalGate;
    /// <summary>パラメータ名:Glide</summary>
    public string StateNameGlide => _stateNameGlide;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        IsDontDestroyOnLoad = true;
    }
}
