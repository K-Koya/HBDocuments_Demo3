using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchGravityRoundBelt : MonoBehaviour
{
    [SerializeField, Tooltip("transform�Q�Ɠ_")]
    Transform _reference = null;

    [SerializeField, Tooltip("true : Reference���d�͕����Ƃ��ĎQ��\nfalse : �V������Ƃ��ĎQ��")]
    bool _isGravityCenter = false;

    [SerializeField, Tooltip("true : �d�͕�����Reference��YZ���ʂɓ��e\nfalse : ���e�����ɋ���ɏd�͂𔭐�")]
    bool _isProjectionYZ = false;

    /// <summary>�d�͊�������Ώ�</summary>
    List<IInteractGimmicks> _interacts = new List<IInteractGimmicks>();

    void OnTriggerEnter(Collider other)
    {
        //�����n���Ɋ܂߂�
        if (other.TryGetComponent(out IInteractGimmicks param))
        {
            SetGravityInOut(param);
            _interacts.Add(param);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //�����n������폜
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
