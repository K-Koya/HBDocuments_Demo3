using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class AnimatorForHuman : MonoBehaviour
{
    /// <summary>該当キャラクターのアニメーター</summary>
    Animator _anim = null;

    /// <summary>該当キャラクターのパラメータ</summary>
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
        if (PauseManager.Instance.IsPause)
        {
            return;
        }

        if (_param.DoAccelGateGlide)
        {
            _anim.Play(AnimParamManager.Instance.StateNameJumpToGoalGate);
        }
        else if (_param.FinishAccelGateGlide)
        {
            _anim.Play(AnimParamManager.Instance.StateNameLandingBeforeGlide);
        }

        _anim.SetFloat(AnimParamManager.Instance.ParamNameSpeed, _param.ResultSpeed);
        _anim.SetBool(AnimParamManager.Instance.ParamNameIsGround, _param.IsGround);
        _anim.SetBool(AnimParamManager.Instance.ParamNameIsJump, _param.IsJump);
        _anim.SetBool(AnimParamManager.Instance.ParamNameIsBrake, _param.IsBrake);
          
        if (_param.DoSideFlip)
        {
            _anim.SetTrigger(AnimParamManager.Instance.ParamNameDoSideFlip);
        }

        if (_param.DoRunOver)
        {
            _anim.SetTrigger(AnimParamManager.Instance.ParamNameDoRunOver);
        }
    }

    void FixedUpdate()
    {
        if (_param.DoSwitchGravity)
        {
            _anim.SetTrigger(AnimParamManager.Instance.ParamNameDoSwitchGravity);
        }
    }
}
