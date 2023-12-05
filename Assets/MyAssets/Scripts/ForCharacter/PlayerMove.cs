using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    /// <summary>�d�͂̋���</summary>
    const float _GRAVITY_POWER = 20f;

    /// <summary>�_�b�V���W�����v���̃W�����v�͕␳�l</summary>
    const float _DASH_JUMP_RATIO = 1.5f;

    /// <summary>�u���[�L��Ԃ�ۂ���</summary>
    const float _BRAKE_TIME = 0.5f;



    [SerializeField, Tooltip("�v���C���[����̍ۂɎ�ςɂȂ�J����")]
    Transform _targetingCamera = null;

    /// <summary>���Y�I�u�W�F�N�g�̃��W�b�h�{�f�B</summary>
    Rigidbody _rb = null;

    /// <summary>�Y���L�����N�^�[�̃p�����[�^</summary>
    CharacterParameter _param = null;

    /// <summary>�ړ�����X�N���v�g</summary>
    System.Action _MoveControl = null;


    /// <summary>Rigidbody��Drag�l�F�n��Î~��</summary>
    float _dragForIdle = 1f;

    /// <summary>Rigidbody��Drag�l�F�n��ړ���</summary>
    float _dragForMove = 1f;

    /// <summary>Rigidbody��Drag�l�F�n��ړ��̃u���[�L��</summary>
    float _dragForBrake = 1f;

    /// <summary>Rigidbody��Drag�l�F�󒆎�</summary>
    float _dragForAir = 1f;

    [SerializeField, Tooltip("���s�̓��[�g")]
    float _speedRateWalk = 2f;

    [SerializeField, Tooltip("���s�̓��[�g")]
    float _speedRateRun = 4f;

    [SerializeField, Tooltip("�󒆈ړ��̓��[�g")]
    float _speedRateAir = 4f;

    [SerializeField, Tooltip("�ʏ�W�����v�̓��[�g")]
    float _jumpNormalRate = 5f;


    /// <summary>�e��^�C�}�[</summary>
    float _timer = 0f;


    /// <summary>�ړ���</summary>
    Vector3 _moveForce = Vector3.zero;

    /// <summary>�d��</summary>
    Vector3 _gravityForce = Vector3.zero;

    /// <summary>�u���[�L����</summary>
    Vector3 _brakeDirection = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        if (!_targetingCamera)
        {
            _targetingCamera = GameObject.FindGameObjectWithTag(TagManager.Instance.MainCamera).transform;
        }

        _param = GetComponent<CharacterParameter>();
        _rb = GetComponent<Rigidbody>();

        //Unity�W���̏d�͎͂g��Ȃ�
        _rb.useGravity = false;
        //�����͏�����
        _rb.drag = _dragForIdle;
    }

    // Update is called once per frame
    void Update()
    {
        //�|�[�Y���͎~�߂�
        if (PauseManager.Instance.IsPause) return;

        SwitchMoveControl();
        _MoveControl();
    }

    void FixedUpdate()
    {
        //�|�[�Y���͎~�߂�
        if (PauseManager.Instance.IsPause) return;

        _rb.AddForce(_moveForce, ForceMode.Acceleration);
        _rb.AddForce(_gravityForce, ForceMode.Acceleration);
    }

    /// <summary>_MoveControl�����݂̃v���C���[�̏�Ԃ���؂�ւ���</summary>
    void SwitchMoveControl()
    {
        if (_param.IsGround)
        {
            _MoveControl = GroundMove;
        }
        else
        {
            _MoveControl = AirMove;
        }
    }

    /// <summary> �L�����N�^�[���w������ɉ�]������ </summary>
    /// <param name="targetDirection">�ڕW����</param>
    /// <param name="up">������iVector.Zero�Ȃ��������w�肵�Ȃ��j</param>
    /// <param name="rotateSpeed">��]���x</param>
    void CharacterRotation(Vector3 targetDirection, Vector3 up, float rotateSpeed)
    {
        if (targetDirection.sqrMagnitude > 0.0f)
        {
            Vector3 trunDirection = transform.right;
            Quaternion charDirectionQuaternion = Quaternion.identity;
            if (up.sqrMagnitude > 0f) charDirectionQuaternion = Quaternion.LookRotation(targetDirection + (trunDirection * 0.001f), up);
            else charDirectionQuaternion = Quaternion.LookRotation(targetDirection + (trunDirection * 0.001f));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, charDirectionQuaternion, rotateSpeed * Time.deltaTime);
        }
    }

    /// <summary>�n��ɂ�����ړ�����</summary>
    void GroundMove()
    {
        bool isVelocityOnPlaneZero = false;

        //�����w��
        Vector3 velocityOnPlane = Vector3.ProjectOnPlane(_rb.velocity, -_param.GravityDirection);
        if (velocityOnPlane.sqrMagnitude > 0.01f)
        {
            velocityOnPlane = Vector3.Normalize(velocityOnPlane);
            CharacterRotation(velocityOnPlane, -_param.GravityDirection, 720f);
        }
        else
        {
            velocityOnPlane = Vector3.zero;
            isVelocityOnPlaneZero = true;
        }

        //�ړ����͂���
        if (InputUtility.GetMove)
        {
            _rb.drag = _dragForMove;

            Vector3 cameraForwardOnPlayerFloor = Vector3.Normalize(Vector3.ProjectOnPlane(_targetingCamera.forward, _param.FloorNormal));
            Vector3 cameraRightOnPlayerFloor = Vector3.Normalize(Vector3.ProjectOnPlane(_targetingCamera.right, _param.FloorNormal));
            _moveForce = (cameraForwardOnPlayerFloor * InputUtility.GetMoveDirection.y)
                                + (cameraRightOnPlayerFloor * InputUtility.GetMoveDirection.x);
                        
            //�u���[�L�p������
            if (_param.IsBrake)
            {
                //�u���[�L��������؂�or����
                if (_timer < 0f || Vector3.Dot(_brakeDirection, _moveForce) < 0.5f)
                {
                    _brakeDirection = Vector3.zero;
                    _timer = 0f;
                    _param.IsBrake = false;

                    //�}����
                    _rb.AddForce(_moveForce * _speedRateRun * 0.5f, ForceMode.VelocityChange);
                }
                else
                {
                    _timer -= Time.deltaTime;
                    return;
                }
            }
            //�t�������͂ɂ��u���[�L����
            else if (!isVelocityOnPlaneZero && Vector3.Dot(velocityOnPlane, _moveForce) < -0.5f)
            {
                _brakeDirection = _moveForce;
                _timer = _BRAKE_TIME;
                _param.IsBrake = true;
                return;
            }

            _moveForce *= _speedRateRun;
            _gravityForce = -_param.FloorNormal * _GRAVITY_POWER;
        }
        //�ړ����͂Ȃ�
        else
        {
            if (_param.IsBrake)
            {
                _brakeDirection = Vector3.zero;
                _timer = 0f;
                _param.IsBrake = false;
            }

            _rb.drag = _dragForIdle;
            _moveForce = Vector3.zero;

            _gravityForce = -_param.FloorNormal * _GRAVITY_POWER;
        }

        //�W�����v���͒���
        if (InputUtility.GetDownJump)
        {
            _param.IsJump = true;
            _rb.drag = _dragForAir;
            float ratio = InputUtility.GetMove ? _DASH_JUMP_RATIO : 1f;
            _rb.AddForce(_jumpNormalRate * ratio * -_param.GravityDirection, ForceMode.VelocityChange);
        }
        else if(!InputUtility.GetJump)
        {
            _param.IsJump = false;
        }
    }

    /// <summary>�󒆗������ɂ�����ړ�����</summary>
    void AirMove()
    {
        _rb.drag = _dragForAir;

        if (InputUtility.GetMove)
        {
            Vector3 cameraForwardOnPlayerFloor = Vector3.Normalize(Vector3.ProjectOnPlane(_targetingCamera.forward, -_param.GravityDirection));
            Vector3 cameraRightOnPlayerFloor = Vector3.Normalize(Vector3.ProjectOnPlane(_targetingCamera.right, -_param.GravityDirection));
            _moveForce = ((cameraForwardOnPlayerFloor * InputUtility.GetMoveDirection.y)
                                + (cameraRightOnPlayerFloor * InputUtility.GetMoveDirection.x))
                                * _speedRateAir;
        }
        else
        {
            _moveForce = Vector3.zero;
        }

        if (_param.IsJump && !InputUtility.GetJump)
        {
            _param.IsJump = false;
            _rb.velocity = Vector3.ProjectOnPlane(_rb.velocity, -_param.GravityDirection);
        }

        //�����w��
        Vector3 velocityOnPlane = Vector3.ProjectOnPlane(_rb.velocity, -_param.GravityDirection);
        if (_moveForce.sqrMagnitude > 0.01f)
        {
            velocityOnPlane = Vector3.Normalize(velocityOnPlane);
            CharacterRotation(velocityOnPlane, -_param.GravityDirection, 90f);
        }

        _gravityForce = _param.GravityDirection * _GRAVITY_POWER;
    }
}
