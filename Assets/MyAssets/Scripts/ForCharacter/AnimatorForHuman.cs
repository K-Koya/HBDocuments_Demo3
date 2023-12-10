using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class AnimatorForHuman : MonoBehaviour
{
    [SerializeField, Tooltip("�p�����[�^��:Speed")]
    string _paramNameSpeed = "Speed";

    [SerializeField, Tooltip("�p�����[�^��:IsJump")]
    string _paramNameIsJump = "IsJump";

    [SerializeField, Tooltip("�p�����[�^��:IsGround")]
    string _paramNameIsGround = "IsGround";

    [SerializeField, Tooltip("�p�����[�^��:IsBrake")]
    string _paramNameIsBrake = "IsBrake";

    [SerializeField, Tooltip("�p�����[�^��:DoSideFlip")]
    string _paramNameDoSideFlip = "DoSideFlip";

    [SerializeField, Tooltip("�p�����[�^��:DoRunOver")]
    string _paramNameDoRunOver = "DoRunOver";

    /// <summary>�Y���L�����N�^�[�̃A�j���[�^�[</summary>
    Animator _anim = null;

    /// <summary>�Y���L�����N�^�[�̃p�����[�^</summary>
    ICharacterParameterForAnimator _param = null;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _param = GetComponentInParent<ICharacterParameterForAnimator>();
        if(_param is null) _param = GetComponent<ICharacterParameterForAnimator>();
    }

    // Update is called once per frame
    void Update()
    {        
        _anim.SetFloat(_paramNameSpeed, _param.ResultSpeed);
        _anim.SetBool(_paramNameIsGround, _param.IsGround);
        _anim.SetBool(_paramNameIsJump, _param.IsJump);
        _anim.SetBool(_paramNameIsBrake, _param.IsBrake);

        if (_param.DoSideFlip)
        {
            _anim.SetTrigger(_paramNameDoSideFlip);
        }

        if (_param.DoRunOver)
        {
            _anim.SetTrigger(_paramNameDoRunOver);
        }
    }
}
