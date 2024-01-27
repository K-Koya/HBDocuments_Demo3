using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravityArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //�L�����N�^�[�̏d�͕��������̃I�u�W�F�N�g�̉�������
        if (other.TryGetComponent(out IInteractGimmicks param))
        {
            param.SwitchGravityImmediately(-transform.up);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //�L�����N�^�[�̏d�͕��������̃I�u�W�F�N�g�̉�������
        if (collision.collider.TryGetComponent(out IInteractGimmicks param))
        {
            param.SwitchGravityImmediately(-transform.up);
        }
    }
}
