using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundForCharacter : MonoBehaviour
{
    /// <summary>�Y���L�����N�^�[�̃p�����[�^</summary>
    IDetectGround _param = null;

    [SerializeField, Tooltip("�������甭�����鉹���o���X�s�[�J�[")]
    AudioSource _footSpeaker = null;

    /// <summary>true : 1�t���[���O�n��ɂ���</summary>
    bool _isBeforeGround = false;

    

    /*
    [System.Serializable]
    struct PairingTexture
    {
        [SerializeField, Tooltip("�e�N�X�`��")]
        public Texture tex;

        [SerializeField, Tooltip("���")]
        public Ground type;
    }
    [SerializeField, Tooltip("�e�N�X�`���Ƃ��̎�ʂ�R�Â��郊�X�g")]
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
        //������
        if (_isBeforeGround && !_param.IsGround)
        {
            //�n�ʂ̃}�e���A���E�e�N�X�`�����m�F
            RaycastHit hit;
            if (Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, 2f, LayerManager.Instance.AllGround))
            {
                AudioClip clip = SoundManager.Instance.GetJumpSound(hit.collider.tag);
                _footSpeaker.clip = clip;
                _footSpeaker.Play();
            }
        }
        //���n��
        else if (!_isBeforeGround && _param.IsGround)
        {
            //�n�ʂ̃}�e���A���E�e�N�X�`�����m�F
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

    /// <summary>�A�j���[�^�[�C�x���g���A���s���ɑ���n�ʂɂ���</summary>
    public void EmitWalkFootStep()
    {
        //�n�ʂ̃}�e���A���E�e�N�X�`�����m�F
        RaycastHit hit;
        if (Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, 1.2f, LayerManager.Instance.AllGround))
        {
            AudioClip clip = SoundManager.Instance.GetWalkSound(hit.collider.tag);
            _footSpeaker.clip = clip;
            _footSpeaker.Play();
        }
    }

    /// <summary>�A�j���[�^�[�C�x���g���A���s���ɑ���n�ʂɂ���</summary>
    public void EmitRunFootStep()
    {
        //�n�ʂ̃}�e���A���E�e�N�X�`�����m�F
        RaycastHit hit;
        if (Physics.Raycast(_param.transform.position + _param.transform.up, _param.GravityDirection, out hit, 1.2f, LayerManager.Instance.AllGround))
        {
            AudioClip clip = SoundManager.Instance.GetRunSound(hit.collider.tag);
            _footSpeaker.clip = clip;
            _footSpeaker.Play();
        }
    }
}
