using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTrackEffect : MonoBehaviour
{
    [SerializeField, Tooltip("�G�t�F�N�g��`������l�p�`�̈�ӂ̔����̒���")]
    float _oneSideHalfLength = 1.5f;

    [SerializeField, Tooltip("�G�t�F�N�g��`������l�p�`��̃G�t�F�N�g�̑���")]
    float _effectMoveSpeed = 1f;

    [SerializeField, Tooltip("�X�^�[�g�n�_�ɖ߂�܂ł̑ҋ@����(s)")]
    float _repeatDelay = 3f;

    /// <summary>�R���[�`������_repeatDelay�����҂�����N���X</summary>
    WaitForSeconds _waitRepeatDelay = null;


    /// <summary>�G�t�F�N�g��`������l�p�`��̃G�t�F�N�g�̈ړ����x</summary>
    Vector3 _effectMoveVelocity = Vector3.zero;

    [SerializeField, Tooltip("TrailRenderer�����ʒu1")]
    TrailRenderer _trail1 = null;

    [SerializeField, Tooltip("TrailRenderer�����ʒu2")]
    TrailRenderer _trail2 = null;

    /// <summary>TrailRenderer��time�ۊ� 1</summary>
    float _trail1TimeCache = 0f;

    /// <summary>TrailRenderer��time�ۊ� 2</summary>
    float _trail2TimeCache = 0f;

    /// <summary>true : �Q�[�g�̏I�_�ɓ��B</summary>
    bool _isReachedTerminal = false;

    /// <summary>�Y���̃h�[���[�J�[�g</summary>
    CinemachineDollyCart _dolly = null;

    // Start is called before the first frame update
    void Start()
    {
        _dolly = GetComponent<CinemachineDollyCart>();

        _waitRepeatDelay = new WaitForSeconds(_repeatDelay);

        _trail1.transform.localPosition = new Vector3(_oneSideHalfLength, _oneSideHalfLength, 0f);
        _trail2.transform.localPosition = -_trail1.transform.localPosition;
        _effectMoveVelocity = Vector3.down;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isReachedTerminal || PauseManager.Instance.IsPause)
        {
            return;
        }

        Vector3 vel = _effectMoveVelocity * _effectMoveSpeed * Time.deltaTime;
        _trail1.transform.localPosition += vel;
        _trail2.transform.localPosition -= vel;

        Vector3 localPos1 = _trail1.transform.localPosition;
        Vector3 localPos2 = _trail2.transform.localPosition;
        if (localPos1.x > _oneSideHalfLength)
        {
            localPos1.x = _oneSideHalfLength;
            localPos2.x = -_oneSideHalfLength;
            _effectMoveVelocity = Vector3.down;
        }
        else if(localPos1.x < -_oneSideHalfLength)
        {
            localPos1.x = -_oneSideHalfLength;
            localPos2.x = _oneSideHalfLength;
            _effectMoveVelocity = Vector3.up;
        }
        else if (localPos1.y > _oneSideHalfLength)
        {
            localPos1.y = _oneSideHalfLength;
            localPos2.y = -_oneSideHalfLength;
            _effectMoveVelocity = Vector3.right;
        }
        else if (localPos1.y < -_oneSideHalfLength)
        {
            localPos1.y = -_oneSideHalfLength;
            localPos2.y = _oneSideHalfLength;
            _effectMoveVelocity = Vector3.left;
        }
        _trail1.transform.localPosition = localPos1;
        _trail2.transform.localPosition = localPos2;
    }

    /// <summary>��莞�Ԍ�ɃX�^�[�g�n�_�ɖ߂郁�\�b�h</summary>
    public void RepeatStartPosition()
    {
        StartCoroutine(RepeatStartPositionCoroutine());
    }

    /// <summary>��莞�Ԍ�ɃX�^�[�g�n�_�ɖ߂�R���[�`��</summary>
    IEnumerator RepeatStartPositionCoroutine()
    {
        _isReachedTerminal = true;

        yield return _waitRepeatDelay;

        _isReachedTerminal = false;
        _trail1.enabled = false;
        _trail2.enabled = false;
        _dolly.m_Position = 0f;
        _trail1TimeCache = _trail1.time;
        _trail2TimeCache = _trail2.time;

        yield return null;

        _trail1.enabled = true;
        _trail2.enabled = true;
        _trail1.time = 0f;
        _trail2.time = 0f;

        yield return null;

        _trail1.time = _trail1TimeCache;
        _trail2.time = _trail2TimeCache;
    }
}
