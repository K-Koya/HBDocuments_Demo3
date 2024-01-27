using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

[DefaultExecutionOrder(-1)]
public class CharacterParameter : MonoBehaviour, 
    ICharacterParameterForAnimator, 
    ICharacterParameterForCamera, 
    ICharacterParameterForTutorial,
    IDetectGround,
    ICourseOutProcedurer, 
    IRelayAccelGateProcedurer, 
    IStageGoalProcedurer, 
    IInteractGimmicks
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

    /// <summary>�L�����N�^�[�̏�ԊǗ��t���O</summary>
    class StateFlag
    {
        /// <summary>�r�b�g�t���O</summary>
        public enum Flags : uint
        {
            /// <summary>������</summary>
            Clear = 0,
            /// <summary>true : ���[�r�[�������Đ���</summary>
            OnMovie = 1,
            /// <summary>true : �������쒆�i������󂯕t���Ȃ��j</summary>
            OnAutoMovement = 2,
            /// <summary>true : �v���C���[�L�����N�^�[�ł���</summary>
            IsPlayer = 4,
            /// <summary>true : �G�L�����N�^�[�ł���</summary>
            IsEnemy = 8,
            /// <summary>true : ���n���Ă���</summary>
            IsGround = 16,
            /// <summary>true : �W�����v���ł���</summary>
            IsJump = 32,
            /// <summary>true : �u���[�L���ł���</summary>
            IsBrake = 64,
            /// <summary>true : �ǒ���t�����ł���</summary>
            IsWall = 128,
        }

        /// <summary>�r�b�g�t���O�Ǘ��ϐ�</summary>
        Flags _flag = Flags.Clear;

        /// <summary>�t���O�Z�b�g</summary>
        /// <param name="flag">�Ώ�</param>
        /// <param name="value">�l</param>
        public void Set(Flags flag, bool value)
        {
            if (value)
            {
                _flag |= (Flags)(uint.MaxValue & (uint)flag);
            }
            else
            {
                _flag &= (Flags)(uint.MaxValue ^ (uint)flag);
            }
        }

        /// <summary>�t���O�Q�b�g</summary>
        /// <param name="flag">�Ώ�</param>
        /// <returns>�l</returns>
        public bool Get(Flags flag)
        {
            return ((uint)_flag & (uint)flag) > 0;
        }
    }

    /// <summary>�L�����N�^�[�̑��쐧���̂��������ȍs��</summary>
    enum ExtraMotion : byte
    {
        /// <summary>������</summary>
        None = 0,
        /// <summary>�T�C�h�t���b�v��</summary>
        SideFlip,
        /// <summary>��E�i�����z����</summary>
        RunOver,
        /// <summary>�d�͕ύX��</summary>
        SwitchGravity,

        /// <summary>�A�N�Z���Q�[�g���犊�󂷂�Ԑ�</summary>
        AccelGateGlide,
    }




    [Header("�蓮��`")]
    #region �����o�[
    [SerializeField, Tooltip("�t�F���X���z�����Ȃ���я��鍂��")]
    float _stairHeight = 0.2f;

    [SerializeField, Tooltip("�t�F���X��i���̏��z�����\�ȍ���")]
    float _fenseHeight = 0.8f; 

    [SerializeField, Tooltip("�n�ʂƕǂ̋��E�p�x")]
    float _slopeLimit = 45f;

    /// <summary>�L�����N�^�[�̑��쐧���̂��������ȍs���̏��</summary>
    ExtraMotion _exMotion = ExtraMotion.None;

    /// <summary>True : ExtraMotion�i�L�����N�^�[�̑��쐧���̂��������ȍs���j�����߂ėL���ɂȂ���</summary>
    bool _isExtraMotionInit = false;

    /// <summary>�e���Ԃ�\���̃r�b�g�t���O</summary>
    StateFlag _state = new StateFlag();

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>�ڒn���̏��̖@��</summary>
    Vector3 _floorNormal = Vector3.up;

    /// <summary>�ڐG���̕ǂ̖@��</summary>
    Vector3? _wallNormal = null;

    /// <summary>�o����Ƃ݂Ȃ����߂̒��S�_����̋���</summary>
    float _slopeAngleThreshold = 1f;

    /// <summary>�q�ϑ��x</summary>
    float _resultSpeed = 0f;



    #endregion


    #region �v���p�e�B
    /// <summary>�J���������ʒu�I�u�W�F�N�g</summary>
    public Transform CameraTarget => _cameraTarget;

    /// <summary>�L�����N�^�[�̎��_�ʒu</summary>
    public Transform EyePoint => _eyePoint;

    /// <summary>True : ���[�r�[���Ŏ����I�ɓ������Ă���</summary>
    public bool OnAutoMovement { get => _state.Get(StateFlag.Flags.OnAutoMovement); set => _state.Set(StateFlag.Flags.OnAutoMovement, value); }

    /// <summary>True : ���n���Ă���</summary>
    public bool IsGround => _state.Get(StateFlag.Flags.IsGround);

    /// <summary>True : �W�����v����</summary>
    public bool IsJump { get => _state.Get(StateFlag.Flags.IsJump); set => _state.Set(StateFlag.Flags.IsJump, value); }

    /// <summary>True : �u���[�L��</summary>
    public bool IsBrake { get => _state.Get(StateFlag.Flags.IsBrake); set => _state.Set(StateFlag.Flags.IsBrake, value); }

    /// <summary>True : �ǒ���t����</summary>
    public bool IsWall { get => _state.Get(StateFlag.Flags.IsWall); set => _state.Set(StateFlag.Flags.IsWall, value); }

    /// <summary>True : �T�C�h�t���b�v�����{��</summary>
    public bool IsSideFlip 
    { 
        get => _exMotion is ExtraMotion.SideFlip;
        set
        {
            _isExtraMotionInit = value;
            if (value) _exMotion = ExtraMotion.SideFlip;
            else _exMotion = ExtraMotion.None;
        }
    }

    /// <summary>True : �T�C�h�t���b�v�����{����</summary>
    public bool DoSideFlip
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.SideFlip;
    }

    /// <summary>True : ��E�i�����z�������{��</summary>
    public bool IsRunOver
    {
        get => _exMotion is ExtraMotion.RunOver;
        set
        {
            _isExtraMotionInit = value;
            if(value) _exMotion = ExtraMotion.RunOver;
            else _exMotion = ExtraMotion.None;
        }
    }

    /// <summary>True : ��E�i�����z�������{����</summary>
    public bool DoRunOver
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.RunOver;
    }

    /// <summary>True : �d�͂��u�ԓI�ɕς�������߁A�K����</summary>
    public bool IsSwitchGravity
    {
        get => _exMotion is ExtraMotion.SwitchGravity;
        set
        {
            _isExtraMotionInit = value;
            if (value) _exMotion = ExtraMotion.SwitchGravity;
            else _exMotion = ExtraMotion.None;
        }
    }

    /// <summary>True : �d�͂��u�ԓI�ɕς��������</summary>
    public bool DoSwitchGravity
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.SwitchGravity;
    }

    /// <summary>True : �A�N�Z���Q�[�g�̊���Ԑ���</summary>
    public bool IsAccelGateGlide
    {
        get => _exMotion is ExtraMotion.AccelGateGlide;
        set
        {
            _isExtraMotionInit = true;
            if (value)
            {
                _exMotion = ExtraMotion.AccelGateGlide;
            }
            else
            {
                _exMotion = ExtraMotion.None;
            }
        }
    }

    /// <summary>True : �A�N�Z���Q�[�g�̊���Ԑ�����</summary>
    public bool DoAccelGateGlide
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.AccelGateGlide;
    }

    /// <summary>True : �A�N�Z���Q�[�g�̊���Ԑ����I��������</summary>
    public bool FinishAccelGateGlide
    {
        get => _isExtraMotionInit && _exMotion is not ExtraMotion.AccelGateGlide;
    }

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }

    /// <summary>�ڒn���̏��̖@��</summary>
    public Vector3 FloorNormal => _floorNormal;

    /// <summary>�ڐG���̕ǂ̖@��</summary>
    public Vector3? WallNormal => _wallNormal;

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
    }

    void FixedUpdate()
    {
        SpeedCheck();
        GroundCheck();

        //ExtraMotion�̏������t���O�����Z�b�g
        ResetExtraMotionInit();
    }

    /// <summary>ExtraMotion�̗v������1�t���[���o���A�������t���O�̃��Z�b�g��v��</summary>
    public void ResetExtraMotionInit()
    {
        _isExtraMotionInit = false;
    }

    /// <summary>�ڒn���菈��</summary>
    void GroundCheck()
    {
        _state.Set(StateFlag.Flags.IsGround, false);

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

                _state.Set(StateFlag.Flags.IsGround, true);
            }
            else
            {
                transform.parent = null;
                _floorNormal = -_gravityDirection;
            }
        }
        else
        {
            transform.parent = null;
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
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_stairHeight + _collider.radius * 2f);

        _wallNormal = null;
        
        //�ǂ̑��݂��擾
        RaycastHit hit;
        if (Physics.CapsuleCast(capsuleCastBaseTop + transform.position, capsuleCastBaseBottom + transform.position,
                                _collider.radius * _GROUND_CHECK_RADIUS_RATE, direction, out hit, _collider.radius, LayerManager.Instance.AllGround))
        {
            normal = hit.normal;
            point = hit.point;

            _wallNormal = normal;
        }

        return (point, normal);
    }

    /// <summary>���ʂ̍�̐ڐG���菈��</summary>
    /// <param name="direction">�ǂ��X�L�����������</param>
    /// <returns>��㕔���W</returns>
    public Vector3? FrontStepContactCheck(Vector3 direction)
    {
        Vector3? onStep = null;

        Vector3 forWallDetectBackLine = transform.position - (_collider.radius * transform.forward);
        Vector3 capsuleCastBaseTop = -_gravityDirection * (_collider.height + _fenseHeight - _collider.radius);
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_fenseHeight + _collider.radius);

        //�ǂ̑��݂��擾�ł��Ȃ���΁A����ɍ�E�i���̌��������̍��W���擾
        if (!Physics.CapsuleCast(capsuleCastBaseTop + forWallDetectBackLine, capsuleCastBaseBottom + forWallDetectBackLine,
                                _collider.radius * _GROUND_CHECK_RADIUS_RATE, direction, _collider.radius * 2.5f, LayerManager.Instance.AllGround))
        {
            RaycastHit hit;
            Vector3 sphereCastBase = capsuleCastBaseTop + transform.position + (_collider.radius * 2.1f * direction);
            if (Physics.SphereCast(sphereCastBase, _collider.radius, _gravityDirection, out hit, _collider.height, LayerManager.Instance.AllGround))
            {
                onStep = hit.point;
            }
            else
            {
                onStep = capsuleCastBaseBottom + transform.position + (_collider.radius * 2.1f * direction);
            }
        }

        return onStep;
    }

    /// <summary>�ړ����x����</summary>
    void SpeedCheck()
    {
        _resultSpeed = _rb.velocity.magnitude;
    }

    /// <summary>�C�x���g�O��ŏd�͕�����ύX</summary>
    /// <param name="afterTransform">�C�x���g��̎p���T���v��</param>
    public void SwitchGravityInEvent(Transform afterTransform)
    {
        _gravityDirection = -afterTransform.up;
        transform.up = afterTransform.up;
        transform.rotation = Quaternion.LookRotation(afterTransform.forward, afterTransform.up);
    }

    /// <summary>�d�͕����𑦍��ɕύX</summary>
    public void SwitchGravityImmediately(Vector3 gravityDirection)
    {
        float dot = Vector3.Dot(_gravityDirection, gravityDirection);

        if (dot < 1f)
        {
            _gravityDirection = gravityDirection;
            Vector3 forward = Vector3.ProjectOnPlane(_rb.velocity, -gravityDirection);
            transform.up = -gravityDirection;
            transform.rotation = Quaternion.LookRotation(forward, -gravityDirection);
            IsSwitchGravity = true;
        }
    }

    /// <summary>�d�͕�����A���I�ɕύX</summary>
    public void SwichGravityChainning(Vector3 gravityDirection)
    {
        float dot = Vector3.Dot(_gravityDirection, gravityDirection);

        if (dot < 1f)
        {
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, -gravityDirection);

            if (forward != Vector3.zero)
            {
                _gravityDirection = gravityDirection;
                transform.up = -gravityDirection;
                transform.rotation = Quaternion.LookRotation(forward, -gravityDirection);
            }
        }
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

    /// <summary>True : �ǒ���t����</summary>
    public bool IsWall { get; }

    /// <summary>True : �T�C�h�t���b�v�����{</summary>
    public bool DoSideFlip { get; }

    /// <summary>True : ��E�i�����z�������{</summary>
    public bool DoRunOver { get; }

    /// <summary>True : �d�͂��u�ԓI�ɕς����</summary>
    public bool DoSwitchGravity { get; }

    /// <summary>True : �A�N�Z���Q�[�g�̊���Ԑ�����</summary>
    public bool DoAccelGateGlide { get; }

    /// <summary>True : �A�N�Z���Q�[�g�̊���Ԑ����I��������</summary>
    public bool FinishAccelGateGlide { get; }
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

/// <summary>�n�ʂ������邽�߂̏��</summary>
public interface IDetectGround
{
    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    public Vector3 GravityDirection { get; }

    /// <summary>�L�����N�^�[�̑����p��</summary>
    public Transform transform { get; }

    /// <summary>true : ���n���Ă���</summary>
    public bool IsGround { get; }
}

/// <summary>�`���[�g���A���\���p</summary>
public interface ICharacterParameterForTutorial
{
    /// <summary>�q�ϑ��x</summary>
    public float ResultSpeed { get; }

    /// <summary>true : ���n���Ă���</summary>
    public bool IsGround { get; }

    /// <summary>True : �W�����v����</summary>
    public bool IsJump { get; }

    /// <summary>True : �u���[�L��</summary>
    public bool IsBrake { get; }

    /// <summary>True : �ǒ���t����</summary>
    public bool IsWall { get; }

    /// <summary>True : �T�C�h�t���b�v�����{</summary>
    public bool DoSideFlip { get; }

    /// <summary>True : ��E�i�����z�������{</summary>
    public bool DoRunOver { get; }

    /// <summary>True : �d�͂��u�ԓI�ɕς����</summary>
    public bool DoSwitchGravity { get; }
}


/// <summary>�X�e�[�W�M�~�b�N����̊��p</summary>
public interface IInteractGimmicks
{
    /// <summary>�d�͕����𑦍��ɕύX</summary>
    public void SwitchGravityImmediately(Vector3 gravityDirection);

    /// <summary>�d�͕�����A���I�ɕύX</summary>
    public void SwichGravityChainning(Vector3 gravityDirection);

    /// <summary>�L�����N�^�[�̑����p��</summary>
    public Transform transform { get; }
}

/// <summary>�R�[�X�A�E�g����𔽉f�p</summary>
public interface ICourseOutProcedurer
{

}

/// <summary>���p�p�A�N�Z���Q�[�g���p</summary>
public interface IRelayAccelGateProcedurer
{
    /// <summary>True : �A�N�Z���Q�[�g�̊���Ԑ���</summary>
    public bool IsAccelGateGlide { get; set; }

    /// <summary>True : ���[�r�[���Ŏ����I�ɓ������Ă���</summary>
    public bool OnAutoMovement { get; set; }

    /// <summary>�C�x���g�O��ŏd�͕�����ύX</summary>
    /// <param name="afterTransform">�C�x���g��̎p���T���v��</param>
    public void SwitchGravityInEvent(Transform afterTransform);
}

/// <summary>�X�e�[�W�N���A����𔽉f�p</summary>
public interface IStageGoalProcedurer
{

}
