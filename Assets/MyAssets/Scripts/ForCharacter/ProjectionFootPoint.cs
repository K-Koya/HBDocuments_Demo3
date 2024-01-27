using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionFootPoint : MonoBehaviour
{
    /// <summary>足元の位置として投影する影の、最大表示距離</summary>
    const float _RANGE_OF_FOOT_POINT_SHADOW = 8f;

    /// <summary>該当キャラクターのパラメータ</summary>
    IDetectGround _param = null;

    /// <summary>足元の位置として投影するオブジェクトのレンダーラー</summary>
    Renderer _footPointShadowRenderer = null;

    /// <summary>影テクスチャ情報</summary>
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
