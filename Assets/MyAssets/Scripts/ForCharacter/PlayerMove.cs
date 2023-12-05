using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    /// <summary>重力の強さ</summary>
    const float _GRAVITY_POWER = 20f;

    /// <summary>ダッシュジャンプ時のジャンプ力補正値</summary>
    const float _DASH_JUMP_RATIO = 1.5f;

    /// <summary>ブレーキ状態を保つ時間</summary>
    const float _BRAKE_TIME = 0.5f;



    [SerializeField, Tooltip("プレイヤー操作の際に主観になるカメラ")]
    Transform _targetingCamera = null;

    /// <summary>当該オブジェクトのリジッドボディ</summary>
    Rigidbody _rb = null;

    /// <summary>該当キャラクターのパラメータ</summary>
    CharacterParameter _param = null;

    /// <summary>移動制御スクリプト</summary>
    System.Action _MoveControl = null;


    /// <summary>RigidbodyのDrag値：地上静止時</summary>
    float _dragForIdle = 1f;

    /// <summary>RigidbodyのDrag値：地上移動時</summary>
    float _dragForMove = 1f;

    /// <summary>RigidbodyのDrag値：地上移動のブレーキ時</summary>
    float _dragForBrake = 1f;

    /// <summary>RigidbodyのDrag値：空中時</summary>
    float _dragForAir = 1f;

    [SerializeField, Tooltip("歩行力レート")]
    float _speedRateWalk = 2f;

    [SerializeField, Tooltip("走行力レート")]
    float _speedRateRun = 4f;

    [SerializeField, Tooltip("空中移動力レート")]
    float _speedRateAir = 4f;

    [SerializeField, Tooltip("通常ジャンプ力レート")]
    float _jumpNormalRate = 5f;


    /// <summary>各種タイマー</summary>
    float _timer = 0f;


    /// <summary>移動力</summary>
    Vector3 _moveForce = Vector3.zero;

    /// <summary>重力</summary>
    Vector3 _gravityForce = Vector3.zero;

    /// <summary>ブレーキ方向</summary>
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

        //Unity標準の重力は使わない
        _rb.useGravity = false;
        //減速力初期化
        _rb.drag = _dragForIdle;
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ時は止める
        if (PauseManager.Instance.IsPause) return;

        SwitchMoveControl();
        _MoveControl();
    }

    void FixedUpdate()
    {
        //ポーズ時は止める
        if (PauseManager.Instance.IsPause) return;

        _rb.AddForce(_moveForce, ForceMode.Acceleration);
        _rb.AddForce(_gravityForce, ForceMode.Acceleration);
    }

    /// <summary>_MoveControlを現在のプレイヤーの状態から切り替える</summary>
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

    /// <summary> キャラクターを指定向きに回転させる </summary>
    /// <param name="targetDirection">目標向き</param>
    /// <param name="up">上方向（Vector.Zeroなら上方向を指定しない）</param>
    /// <param name="rotateSpeed">回転速度</param>
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

    /// <summary>地上における移動制御</summary>
    void GroundMove()
    {
        bool isVelocityOnPlaneZero = false;

        //向き指定
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

        //移動入力あり
        if (InputUtility.GetMove)
        {
            _rb.drag = _dragForMove;

            Vector3 cameraForwardOnPlayerFloor = Vector3.Normalize(Vector3.ProjectOnPlane(_targetingCamera.forward, _param.FloorNormal));
            Vector3 cameraRightOnPlayerFloor = Vector3.Normalize(Vector3.ProjectOnPlane(_targetingCamera.right, _param.FloorNormal));
            _moveForce = (cameraForwardOnPlayerFloor * InputUtility.GetMoveDirection.y)
                                + (cameraRightOnPlayerFloor * InputUtility.GetMoveDirection.x);
                        
            //ブレーキ継続判定
            if (_param.IsBrake)
            {
                //ブレーキ動作期限切れor解除
                if (_timer < 0f || Vector3.Dot(_brakeDirection, _moveForce) < 0.5f)
                {
                    _brakeDirection = Vector3.zero;
                    _timer = 0f;
                    _param.IsBrake = false;

                    //急加速
                    _rb.AddForce(_moveForce * _speedRateRun * 0.5f, ForceMode.VelocityChange);
                }
                else
                {
                    _timer -= Time.deltaTime;
                    return;
                }
            }
            //逆方向入力によるブレーキ判定
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
        //移動入力なし
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

        //ジャンプ入力直後
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

    /// <summary>空中落下時における移動制御</summary>
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

        //向き指定
        Vector3 velocityOnPlane = Vector3.ProjectOnPlane(_rb.velocity, -_param.GravityDirection);
        if (_moveForce.sqrMagnitude > 0.01f)
        {
            velocityOnPlane = Vector3.Normalize(velocityOnPlane);
            CharacterRotation(velocityOnPlane, -_param.GravityDirection, 90f);
        }

        _gravityForce = _param.GravityDirection * _GRAVITY_POWER;
    }
}
