using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterParameter : MonoBehaviour
{
    #region �萔
    /// <summary>�ڒn������Ƃ邽�߂Ɏg�����̂̔��a���A�R���C�_�[�̔��a�̉��{�����������l</summary>
    const float _GROUND_CHECK_RADIUS_RATE = 0.99f;

    #endregion

    [Header("�L���b�V��")]
    #region �L���b�V��

    [SerializeField, Tooltip("�J���������ʒu�I�u�W�F�N�g")]
    protected Transform _cameraTarget = null;

    [SerializeField, Tooltip("���_�ʒu�I�u�W�F�N�g")]
    protected Transform _eyePoint = null;

    /// <summary>���Y�I�u�W�F�N�g�̃J�v�Z���R���C�_�[</summary>
    CapsuleCollider _collider = default;

    #endregion

    [Header("�蓮��`")]
    #region �����o�[
    [SerializeField, Tooltip("True : ���n���Ă���")]
    bool _isGround = false;

    [SerializeField, Tooltip("True : �ǂɂ������Ă���")]
    bool _isWall = false;

    [SerializeField, Tooltip("�n�ʂƕǂ̋��E�p�x")]
    float _slopeLimit = 45f;


    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>SphereCast���鎞�̊�_�ƂȂ���W</summary>
    Vector3 _castBasePosition = Vector3.zero;

    /// <summary>�o����Ƃ݂Ȃ����߂̒��S�_����̋���</summary>
    float _slopeAngleThreshold = 1f;

    #endregion


    #region �v���p�e�B
    /// <summary>�J���������ʒu�I�u�W�F�N�g</summary>
    public Transform CameraTarget => _cameraTarget;

    /// <summary>�L�����N�^�[�̎��_�ʒu</summary>
    public Transform EyePoint => _eyePoint;

    /// <summary>True : ���n���Ă���</summary>
    public bool IsGround => _isGround;

    /// <summary>True : �ǂɂ������Ă���</summary>
    public bool IsWall => _isWall;

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        /* �ڒn���菈���p */
        _collider = GetComponent<CapsuleCollider>();
        //_collider.providesContacts = true;
        _castBasePosition = _collider.center + Vector3.down * ((_collider.height - _collider.radius * 2f) / 2f);

        //�~�ʔ��a����ʒ������߂����
        _slopeAngleThreshold = 2f * _collider.radius * Mathf.Sin(Mathf.Deg2Rad * _slopeLimit / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        //�|�[�Y���͏������Ȃ�
        if (PauseManager.Instance.IsPause) return;

        GroundCheck();
    }

    /// <summary>�ڒn���菈��</summary>
    void GroundCheck()
    {
        _isGround = false;
        RaycastHit hit;
        if (Physics.SphereCast(_castBasePosition + transform.position, _collider.radius * _GROUND_CHECK_RADIUS_RATE, _gravityDirection, out hit, _collider.radius, LayerManager.Instance.AllGround))
        {
            if (Vector3.SqrMagnitude(transform.position - hit.point) < _slopeAngleThreshold * _slopeAngleThreshold)
            {
                _isGround = true;
            }
        }
    }
}
