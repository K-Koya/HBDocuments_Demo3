using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundForCharacter : MonoBehaviour
{
    /// <summary>該当キャラクターのパラメータ</summary>
    IDetectGround _param = null;

    [SerializeField, Tooltip("足元から発生する音を出すスピーカー")]
    AudioSource _footSpeaker = null;

    /// <summary>true : 1フレーム前地上にいた</summary>
    bool _isBeforeGround = false;

    

    /*
    [System.Serializable]
    struct PairingTexture
    {
        [SerializeField, Tooltip("テクスチャ")]
        public Texture tex;

        [SerializeField, Tooltip("種別")]
        public Ground type;
    }
    [SerializeField, Tooltip("テクスチャとその種別を紐づけるリスト")]
    PairingTexture[] _pairingTextures = null;
    */

    // Start is called before the first frame update
    void Start()
    {
        _param = GetComponent<CharacterParameter>();
        if (_param is null) _param = GetComponentInParent<CharacterParameter>();
    }

    // Update is called once per frame
    void Update()
    {
        //離陸音
        if (_isBeforeGround && !_param.IsGround)
        {
            //地面のマテリアル・テクスチャを確認
            RaycastHit hit;
            if (Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, 2f, LayerManager.Instance.AllGround))
            {
                AudioClip clip = SoundManager.Instance.GetJumpSound(hit.collider.tag);
                _footSpeaker.clip = clip;
                _footSpeaker.Play();
            }
        }
        //着地音
        else if (!_isBeforeGround && _param.IsGround)
        {
            //地面のマテリアル・テクスチャを確認
            RaycastHit hit;
            if (Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, 1.5f, LayerManager.Instance.AllGround))
            {
                AudioClip clip = SoundManager.Instance.GetLandingSound(hit.collider.tag);
                _footSpeaker.clip = clip;
                _footSpeaker.Play();
            }
        }
        _isBeforeGround = _param.IsGround;
    }

    /// <summary>アニメーターイベントより、歩行時に足を地面につけた</summary>
    public void EmitWalkFootStep()
    {
        //地面のマテリアル・テクスチャを確認
        RaycastHit hit;
        if (Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, 1.2f, LayerManager.Instance.AllGround))
        {
            AudioClip clip = SoundManager.Instance.GetWalkSound(hit.collider.tag);
            _footSpeaker.clip = clip;
            _footSpeaker.Play();
        }
    }

    /// <summary>アニメーターイベントより、走行時に足を地面につけた</summary>
    public void EmitRunFootStep()
    {
        //地面のマテリアル・テクスチャを確認
        RaycastHit hit;
        if (Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, 1.2f, LayerManager.Instance.AllGround))
        {
            AudioClip clip = SoundManager.Instance.GetRunSound(hit.collider.tag);
            _footSpeaker.clip = clip;
            _footSpeaker.Play();
        }
    }
}
