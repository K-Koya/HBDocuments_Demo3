using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorForDemoMovie : MonoBehaviour
{
    [SerializeField, Tooltip("�X�e�[�g�� : LandingBeforeGlide")]
    string _stateNameLandingBeforeGlide = "LandingBeforeGlide";

    [SerializeField, Tooltip("�X�e�[�g�� : JumpToGoalGate")]
    string _stateNameJumpToGoalGate = "JumpToGoalGate";

    [SerializeField, Tooltip("�X�e�[�g�� : Glide")]
    string _stateNameGlide = "Glide";
        

    /// <summary>�Y���L�����N�^�[�̃A�j���[�^�[</summary>
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
