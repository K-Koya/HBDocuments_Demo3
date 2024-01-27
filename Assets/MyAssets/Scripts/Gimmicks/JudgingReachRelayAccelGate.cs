using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgingReachRelayAccelGate : MonoBehaviour
{
    [SerializeField, Tooltip("�A�N�Z���Q�[�g��̈ʒu�E�p���E�d�͕���")]
    Transform _targetTransform = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IRelayAccelGateProcedurer param))
        {
            param.OnAutoMovement = true;
            param.IsAccelGateGlide = true;
        }
    }
}
