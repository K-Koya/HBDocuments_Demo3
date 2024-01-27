using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgingReachRelayAccelGate : MonoBehaviour
{
    [SerializeField, Tooltip("アクセルゲート後の位置・姿勢・重力方向")]
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
