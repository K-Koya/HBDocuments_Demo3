using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravityRoundBelt : MonoBehaviour
{
    [SerializeField, Tooltip("transform参照点")]
    Transform _reference = null;

    [SerializeField, Tooltip("true : Referenceを重力方向として参照\nfalse : 天井方向として参照")]
    bool _isGravityCenter = false;

    [SerializeField, Tooltip("true : 重力方向をReferenceのYZ平面に投影\nfalse : 投影せずに球状に重力を発生")]
    bool _isProjectionYZ = false;

    /// <summary>重力干渉をする対象</summary>
    List<IInteractGimmicks> _interacts = new List<IInteractGimmicks>();

    void OnTriggerEnter(Collider other)
    {
        //処理系統に含める
        if (other.TryGetComponent(out IInteractGimmicks param))
        {
            SetGravityInOut(param);
            _interacts.Add(param);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //処理系統から削除
        if (other.TryGetComponent(out IInteractGimmicks param))
        {
            SetGravityInOut(param);
            _interacts.Remove(param);
        }
    }

    void FixedUpdate()
    {
        foreach (var param in _interacts)
        {
            Vector3 gravityDirection = param.transform.position - _reference.position;
            if (_isGravityCenter)
            {
                gravityDirection *= -1f;
            }
            if (_isProjectionYZ)
            {
                gravityDirection = Vector3.ProjectOnPlane(gravityDirection, _reference.right);
            }
            gravityDirection = Vector3.Normalize(gravityDirection);

            param.SwichGravityChainning(gravityDirection);
        }
    }

    void SetGravityInOut(IInteractGimmicks param)
    {
        Vector3 castDirection = param.transform.position - _reference.position;
        Vector3 castOrigin = _reference.position;

        if (_isGravityCenter)
        {
            castDirection *= -1f;
            castOrigin = param.transform.position;
        }

        RaycastHit hit;
        if (Physics.Raycast(castOrigin, castDirection, out hit, castDirection.sqrMagnitude, LayerManager.Instance.AllGround))
        {
            param.SwichGravityChainning(-hit.normal);
        }
    }
}
