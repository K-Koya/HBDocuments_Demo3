using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorForDemoMovie : MonoBehaviour
{
    [SerializeField, Tooltip("カメラの注視対象 : キャラクター視点")]
    Transform _eyePoint = null;


    /// <summary>該当キャラクターのアニメーター</summary>
    Animator _anim = null;

    /// <summary>カメラの注視対象 : キャラクター視点</summary>
    public Transform EyePoint { get => _eyePoint; }


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToJumpToGoalGate()
    {
        _anim.Play(AnimParamManager.Instance.StateNameJumpToGoalGate);
    }

    public void SwitchToGlide()
    {
        _anim.Play(AnimParamManager.Instance.StateNameGlide);
    }

    public void SwitchToLandingBeforeGlide()
    {
        _anim.Play(AnimParamManager.Instance.StateNameLandingBeforeGlide);
    }

    public void SwitchToRecoverBeforeMiss()
    {

    }
}
