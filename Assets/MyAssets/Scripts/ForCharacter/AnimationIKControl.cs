using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationIKControl : MonoBehaviour
{
    /*
    [SerializeField] AvatarIKGoal goal;  // �ǂ̕��ʂ�IK���g�p���邩
    [SerializeField] AvatarIKHint hint;  // goal�Ɠ������ʂ̃q���g��I��
    [SerializeField] Transform goalTransform;// �ŏI�I�Ȉʒu
    [SerializeField] Transform hintTransform;// �I��G�̈ʒu�̃q���g
    */

    [SerializeField, Tooltip("���_����")]
    Transform _lookAtPosition = null;

    [SerializeField, Tooltip("���_����������x��"), Range(0, 1)]
    float _lookAtWeight = 0f;

    /*
    [SerializeField, Range(0, 1)]
    float _hintWeight = 0f;
    */

    /// <summary>�Y���A�j���[�^�[</summary>
    Animator _animator = null;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        /*
        _animator.SetIKPosition(goal, goalTransform.position);
        _animator.SetIKHintPosition(hint, hintTransform.position);

        _animator.SetIKHintPositionWeight(hint, _hintWeight);
        _animator.SetIKPositionWeight(goal, _weight);
        */

        _animator.SetLookAtWeight(_lookAtWeight);
        _animator.SetLookAtPosition(_lookAtPosition.position);
    }
}
