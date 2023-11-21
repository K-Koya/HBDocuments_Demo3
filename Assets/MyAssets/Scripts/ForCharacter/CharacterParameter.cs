using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    CapsuleCollider _collider = default;

    
    #endregion


    [Header("�蓮��`")]
    #region �����o�[
    [SerializeField, Tooltip("True : ���n���Ă���")]
    bool _isGround = false;

    [SerializeField, Tooltip("True : �W�����v����")]
    bool _isJump = false;

    [SerializeField, Tooltip("True : �ǂɂ������Ă���")]
    bool _isWall = false;

    [SerializeField, Tooltip("�n�ʂƕǂ̋��E�p�x")]
    float _slopeLimit = 45f;

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>�ڒn���̏��̖@��</summary>
    Vector3 _floorNormal = Vector3.up;

    /// <summary>SphereCast���鎞�̊�_�ƂȂ���W</summary>
    Vector3 _castBasePosition = Vector3.zero;

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

    /// <summary>True : �ǂɂ������Ă���</summary>
    public bool IsWall => _isWall;

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }

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
        _castBasePosition = -_gravityDirection * _collider.radius; //_collider.center + _gravityDirection * ((_collider.height - _collider.radius * 2f) / 2f);

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

        RaycastHit hit;
        if (Physics.SphereCast(_castBasePosition + transform.position, _collider.radius * _GROUND_CHECK_RADIUS_RATE, _gravityDirection, out hit, _collider.radius, LayerManager.Instance.AllGround))
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
        _castBasePosition = -gravityDirection * _collider.radius;
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

    /*
    /// <summary>�R�[�X�A�E�g����</summary>
    /// <returns>�����A�j���[�V������������C���X�^���X</returns>
    public GameObject DoCourseOut()
    {
        

        //�R�[�X�A�E�g�A�j���[�V������L���ɂ���
        _stageCourseOut.SetActive(true);

        //���x�ƌ������ێ�
        _stageCourseOut.transform.position = transform.position;
        _stageCourseOut.transform.rotation = transform.rotation;
        Rigidbody rb = _stageCourseOut.GetComponent<Rigidbody>();
        rb.velocity = _rb.velocity;
        rb.AddTorque(2f, 0f, 0f, ForceMode.VelocityChange);
                
        //�������A�N�e�B�u�ɂ���
        gameObject.SetActive(false);

        return _stageCourseOut;
    }
    */

    /*
    /// <summary>�X�e�[�W�N���A����</summary>
    /// <returns>�����A�j���[�V������������C���X�^���X</returns>
    public GameObject DoStageGoal()
    {
        //�X�e�[�W�N���A�A�j���[�V������L���ɂ���
        _stageClear.SetActive(true);

        //�ʒu�ƌ������ێ�
        _stageClear.transform.position = transform.position;
        _stageClear.transform.rotation = transform.rotation;

        //�������A�N�e�B�u�ɂ���
        gameObject.SetActive(false);

        return _stageClear;
    }
    */
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
