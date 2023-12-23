using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravityArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //�L�����N�^�[�̏d�͕��������̃I�u�W�F�N�g�̉�������
        if (other.TryGetComponent(out CharacterParameter param))
        {
            param.SwitchGravity(-transform.up);
        }
    }
}
