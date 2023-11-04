using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotationSimple : MonoBehaviour
{
    [SerializeField, Tooltip("��]���x")]
    float _speed = 1f;

    [SerializeField, Tooltip("��]���ʂ̖@���x�N�g��")]
    Vector3 _rotateNormal = Vector3.zero;

    [SerializeField, Tooltip("true : �E�����ɉ�]")]
    bool _isRotateRight = true;

    /// <summary>��]�͂����������</summary>
    Vector3 _anglerVelocity = Vector3.zero;

    /// <summary>true : ��~������</summary>
    bool _isStop = false;

    // Start is called before the first frame update
    void Start()
    {
        //��]�������w��
        _anglerVelocity = _rotateNormal;
        if (!_isRotateRight) _anglerVelocity *= -1f;
    }

    void FixedUpdate()
    {
        //�|�[�Y���͎~�߂�
        if (PauseManager.Instance.IsPause) return;

        //��~�w��
        if (_isStop) return;

        //��ɓ������x���ێ�
        transform.RotateAround(transform.position, _anglerVelocity, _speed);
        //_rb.angularVelocity = _anglerVelocity * _speed;
    }
}
