using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorForDemoMovie : MonoBehaviour
{
    [SerializeField, Tooltip("ステート名 : LandingBeforeGlide")]
    string _stateNameLandingBeforeGlide = "LandingBeforeGlide";

    [SerializeField, Tooltip("ステート名 : JumpToGoalGate")]
    string _stateNameJumpToGoalGate = "JumpToGoalGate";

    [SerializeField, Tooltip("ステート名 : Glide")]
    string _stateNameGlide = "Glide";
        

    /// <summary>該当キャラクターのアニメーター</summary>
    Animator _anim = null;

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
        _anim.Play(_stateNameJumpToGoalGate);
    }

    public void SwitchToGlide()
    {
        _anim.Play(_stateNameGlide);
    }

    public void SwitchToLandingBeforeGlide()
    {
        _anim.Play(_stateNameLandingBeforeGlide);
    }
}
