using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationIKControl : MonoBehaviour
{
    /*
    [SerializeField] AvatarIKGoal goal;  // どの部位のIKを使用するか
    [SerializeField] AvatarIKHint hint;  // goalと同じ部位のヒントを選択
    [SerializeField] Transform goalTransform;// 最終的な位置
    [SerializeField] Transform hintTransform;// 肘や膝の位置のヒント
    */

    [SerializeField, Tooltip("視点方向")]
    Transform _lookAtPosition = null;

    [SerializeField, Tooltip("視点方向を見る度合"), Range(0, 1)]
    float _lookAtWeight = 0f;

    /*
    [SerializeField, Range(0, 1)]
    float _hintWeight = 0f;
    */

    /// <summary>該当アニメーター</summary>
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
