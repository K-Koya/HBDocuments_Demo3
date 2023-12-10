using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

[DefaultExecutionOrder(-1)]
public class CharacterParameter : MonoBehaviour, ICharacterParameterForAnimator, ICharacterParameterForCamera, ICourseOutProcedurer, IStageGoalProcedurer
{
    #region �萔
    /// <summary>�ڒn������Ƃ邽�߂Ɏg�����̂̔��a���A�R���C�_�[�̔��a�̉��{�����������l</summary>
    const float _GROUND_CHECK_RADIUS_RATE = 0.99f;

    #endregion

    [Header("�L���b�V��")]
    #region �L���b�V��

    [SerializeField, Tooltip("�J���������ʒu�I�u�W�F�N�g")]
    Transform _cameraTarget = null;

    [SerializeField, Tooltip("���_�ʒu�I�u�W�F�N�g")]
    Transform _eyePoint = null;

    /// <summary>���Y�I�u�W�F�N�g�̃��W�b�h�{�f�B</summary>
    Rigidbody _rb = null;

    /// <summary>���Y�I�u�W�F�N�g�̃J�v�Z���R���C�_�[</summary>
    CapsuleCollider _collider = null;

    
    #endregion


    [Header("�蓮��`")]
    #region �����o�[
    [SerializeField, Tooltip("True : ���n���Ă���")]
    bool _isGround = false;

    [SerializeField, Tooltip("True : �W�����v����")]
    bool _isJump = false;

    [SerializeField, Tooltip("True : �u���[�L��")]
    bool _isBrake = false;

    /// <summary>True : �T�C�h�t���b�v�����{</summary>
    bool _isSideFlip = false;

    /// <summary>True : ��E�i�����z�������{</summary>
    bool _isRunOver = false;

    [SerializeField, Tooltip("�t�F���X��i���̏��z�����\�ȍ���")]
    float _fenseHeight = 0.8f; 

    [SerializeField, Tooltip("�n�ʂƕǂ̋��E�p�x")]
    float _slopeLimit = 45f;

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>�ڒn���̏��̖@��</summary>
    Vector3 _floorNormal = Vector3.up;

    /// <summary>�ڐG���̕ǂ̖@��</summary>
    Vector3 _wallNormal = Vector3.forward;
    
    /// <summary>�o����Ƃ݂Ȃ����߂̒��S�_����̋���</summary>
    float _slopeAngleThreshold = 1f;

    /// <summary>�q�ϑ��x</summary>
    float _resultSpeed = 0f;

    /// <summary>True : �d�͕�����ύX</summary>
    bool _doSwitchGravity = false;



    #endregion


    #region �v���p�e�B
    /// <summary>�J���������ʒu�I�u�W�F�N�g</summary>
    public Transform CameraTarget => _cameraTarget;

    /// <summary>�L�����N�^�[�̎��_�ʒu</summary>
    public Transform EyePoint => _eyePoint;

    /// <summary>True : ���n���Ă���</summary>
    public bool IsGround => _isGround;

    /// <summary>True : �W�����v����</summary>
    public bool IsJump { get => _isJump; set => _isJump = value; }

    /// <summary>True : �u���[�L��</summary>
    public bool IsBrake { get => _isBrake; set => _isBrake = value; }

    /// <summary>True : �T�C�h�t���b�v�����{</summary>
    public bool DoSideFlip
    {
        get
        {
            bool data = _isSideFlip;
            if(_isSideFlip) _isBrake = false;
            _isSideFlip = false;
            return data;
        }
        set => _isSideFlip = value;
    }

    /// <summary>True : ��E�i�����z�������{</summary>
    public bool DoRunOver
    {
        get
        {
            bool data = _isRunOver;
            _isRunOver = false;
            return data;
        }
        set => _isRunOver = value;
    }

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }

    /// <summary>�ڒn���̏��̖@��</summary>
    public Vector3 FloorNormal => _floorNormal;

    /// <summary>�ڐG���̕ǂ̖@��</summary>
    public Vector3 WallNormal => _wallNormal;

    /// <summary>�q�ϑ��x</summary>
    public float ResultSpeed => _resultSpeed;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        /* ���������� */
        transform.up = -_gravityDirection;
        _rb = GetComponent<Rigidbody>();

        /* �ڒn���菈���p */
        _collider = GetComponent<CapsuleCollider>();

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

    void FixedUpdate()
    {
        SpeedCheck();
    }

    /// <summary>�ڒn���菈��</summary>
    void GroundCheck()
    {
        _isGround = false;

        Vector3 sphereCastBasePosition = -_gravityDirection * _collider.radius;

        RaycastHit hit;
        if (Physics.SphereCast(sphereCastBasePosition + transform.position, _collider.radius * _GROUND_CHECK_RADIUS_RATE, _gravityDirection, out hit, _collider.radius, LayerManager.Instance.AllGround))
        {
            if (Vector3.SqrMagnitude(transform.position - hit.point) < _slopeAngleThreshold * _slopeAngleThreshold)
            {
                Vector3 localScale = hit.transform.localScale;
                transform.localScale = new Vector3(1f / localScale.x, 1f / localScale.y, 1f / localScale.z);
                transform.parent = hit.transform;
                _floorNormal = hit.normal;
                _isGround = true;
            }
            else
            {
                _floorNormal = -_gravityDirection;
            }
        }
        else
        {
            _floorNormal = -_gravityDirection;
        }
    }

    /// <summary>���ʂ̕ǂ̐ڐG���菈��</summary>
    /// <param name="direction">�ǂ��X�L�����������</param>
    /// <returns>1 => �ڐG�ʒu(null : �ڐG���Ă��Ȃ�), 2 => �ǂ̖@��</returns>
    public (Vector3?, Vector3) FrontWallContactCheck(Vector3 direction)
    {
        Vector3 normal = Vector3.zero;
        Vector3? point = null;

        Vector3 capsuleCastBaseTop = -_gravityDirection * (_collider.height - (_collider.radius * 2f));
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_collider.radius * 2f);
        
        //�ǂ̑��݂��擾
        RaycastHit hit;
        if (Physics.CapsuleCast(capsuleCastBaseTop + transform.position, capsuleCastBaseBottom + transform.position,
                                _collider.radius * _GROUND_CHECK_RADIUS_RATE, direction, out hit, 0.1f, LayerManager.Instance.Ground))
        {
            normal = hit.normal;
            point = hit.point;
        }

        return (point, normal);
    }

    /// <summary>���ʂ̍�̐ڐG���菈��</summary>
    /// <param name="direction">�ǂ��X�L�����������</param>
    /// <returns>��㕔���W</returns>
    public Vector3? FrontStepContactCheck(Vector3 direction)
    {
        Vector3? onStep = null;

        Vector3 capsuleCastBaseTop = -_gravityDirection * (_collider.height + _fenseHeight - _collider.radius);
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_fenseHeight + _collider.radius + 0.05f);

        //�ǂ̑��݂��擾�ł��Ȃ���΁A����ɍ�E�i���̌��������̍��W���擾
        if (!Physics.CapsuleCast(capsuleCastBaseTop + transform.position, capsuleCastBaseBottom + transform.position,
                                _collider.radius * _GROUND_CHECK_RADIUS_RATE, direction, _collider.radius * 2f, LayerManager.Instance.Ground))
        {
            RaycastHit hit;
            onStep = capsuleCastBaseTop + transform.position + (direction * _collider.radius * 2.1f);
            if (Physics.SphereCast(onStep.Value, _collider.radius, _gravityDirection, out hit, _collider.height, LayerManager.Instance.Ground))
            {
                onStep = hit.point;
            }
        }

        return onStep;
    }

    /// <summary>�ړ����x����</summary>
    void SpeedCheck()
    {
        _resultSpeed = _rb.velocity.magnitude;
    }

    /// <summary>�d�͕�����ύX</summary>
    public void SwitchGravity(Vector3 gravityDirection)
    {
        _gravityDirection = gravityDirection;
        Vector3 forward = Vector3.ProjectOnPlane(_rb.velocity, -gravityDirection);
        transform.up = -gravityDirection;
        transform.rotation = Quaternion.LookRotation(forward, -gravityDirection);
        _doSwitchGravity = true;
    }

    /// <summary>�d�͕����̕ύX�����������Ƃ�ʒm</summary>
    /// <returns>True : �ύX����</returns>
    public bool DoGravitySwitch()
    {
        bool result = _doSwitchGravity;
        _doSwitchGravity = false;
        return result;
    }

}

/// <summary>Animator�Q�Ɨp</summary>
public interface ICharacterParameterForAnimator
{
    /// <summary>�q�ϑ��x</summary>
    public float ResultSpeed { get; }

    /// <summary>true : ���n���Ă���</summary>
    public bool IsGround { get; }

    /// <summary>True : �W�����v����</summary>
    public bool IsJump { get; }

    /// <summary>True : �u���[�L��</summary>
    public bool IsBrake { get; }

    /// <summary>True : �T�C�h�t���b�v�����{</summary>
    public bool DoSideFlip { get; }

    /// <summary>True : ��E�i�����z�������{</summary>
    public bool DoRunOver { get; }

    /// <summary>�d�͕����̕ύX�����������Ƃ�ʒm</summary>
    /// <returns>True : �ύX����</returns>
    public bool DoGravitySwitch();
}

/// <summary>�J��������Q�Ɨp</summary>
public interface ICharacterParameterForCamera
{
    /// <summary>�J���������ʒu�I�u�W�F�N�g</summary>
    public Transform CameraTarget { get; }

    /// <summary>�L�����N�^�[�̎��_�ʒu</summary>
    public Transform EyePoint { get; }

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    public Vector3 GravityDirection { get; }
}

/// <summary>�X�e�[�W�M�~�b�N����̊��p</summary>
public interface IInteractGimmicks
{

}

/// <summary>�R�[�X�A�E�g����𔽉f�p</summary>
public interface ICourseOutProcedurer
{

}

/// <summary>�X�e�[�W�N���A����𔽉f�p</summary>
public interface IStageGoalProcedurer
{

}
