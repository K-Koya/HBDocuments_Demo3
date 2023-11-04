using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotationSimple : MonoBehaviour
{
    [SerializeField, Tooltip("回転速度")]
    float _speed = 1f;

    [SerializeField, Tooltip("回転平面の法線ベクトル")]
    Vector3 _rotateNormal = Vector3.zero;

    [SerializeField, Tooltip("true : 右方向に回転")]
    bool _isRotateRight = true;

    /// <summary>回転力をかける方向</summary>
    Vector3 _anglerVelocity = Vector3.zero;

    /// <summary>true : 停止させる</summary>
    bool _isStop = false;

    // Start is called before the first frame update
    void Start()
    {
        //回転方向を指定
        _anglerVelocity = _rotateNormal;
        if (!_isRotateRight) _anglerVelocity *= -1f;
    }

    void FixedUpdate()
    {
        //ポーズ時は止める
        if (PauseManager.Instance.IsPause) return;

        //停止指示
        if (_isStop) return;

        //常に同じ速度を維持
        transform.RotateAround(transform.position, _anglerVelocity, _speed);
        //_rb.angularVelocity = _anglerVelocity * _speed;
    }
}
