using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionFootPoint : MonoBehaviour
{
    /// <summary>�����̈ʒu�Ƃ��ē��e����e�́A�ő�\������</summary>
    const float _RANGE_OF_FOOT_POINT_SHADOW = 8f;

    /// <summary>�Y���L�����N�^�[�̃p�����[�^</summary>
    IDetectGround _param = null;

    /// <summary>�����̈ʒu�Ƃ��ē��e����I�u�W�F�N�g�̃����_�[���[</summary>
    Renderer _footPointShadowRenderer = null;

    /// <summary>�e�e�N�X�`�����</summary>
    Material _shadowMat = null;

    
    // Start is called before the first frame update
    void Start()
    {
        if (CharacterPrefabManager.Instance.ForFootPointShadow)
        {
            _footPointShadowRenderer = Instantiate(CharacterPrefabManager.Instance.ForFootPointShadow).GetComponent<Renderer>();
            _shadowMat = _footPointShadowRenderer.material;
        }

        _param = GetComponent<CharacterParameter>();
        if (_param is null) _param = GetComponentInParent<CharacterParameter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.IsPause)
        {
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, _RANGE_OF_FOOT_POINT_SHADOW, LayerManager.Instance.AllGround))
        {
            _footPointShadowRenderer.gameObject.SetActive(true);
            _footPointShadowRenderer.transform.position = hit.point + _footPointShadowRenderer.transform.up * 0.01f;
            _footPointShadowRenderer.transform.up = hit.normal;

            float ratio = Vector3.SqrMagnitude(hit.point - _param.transform.position) / (_RANGE_OF_FOOT_POINT_SHADOW * _RANGE_OF_FOOT_POINT_SHADOW);

            Color color = _shadowMat.color;
            color.a = 1f - ratio;
            _shadowMat.color = color;
        }
        else
        {
            _footPointShadowRenderer.gameObject.SetActive(false);
        }
    }
}
