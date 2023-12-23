using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravityArea : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //キャラクターの重力方向をこのオブジェクトの下方向へ
        if (other.TryGetComponent(out CharacterParameter param))
        {
            param.SwitchGravity(-transform.up);
        }
    }
}
