using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravityArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //キャラクターの重力方向をこのオブジェクトの下方向へ
        if (other.TryGetComponent(out IInteractGimmicks param))
        {
            param.SwitchGravityImmediately(-transform.up);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //キャラクターの重力方向をこのオブジェクトの下方向へ
        if (collision.collider.TryGetComponent(out IInteractGimmicks param))
        {
            param.SwitchGravityImmediately(-transform.up);
        }
    }
}
