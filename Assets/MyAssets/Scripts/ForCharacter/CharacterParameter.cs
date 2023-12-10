using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

[DefaultExecutionOrder(-1)]
public class CharacterParameter : MonoBehaviour, ICharacterParameterForAnimator, ICharacterParameterForCamera, ICourseOutProcedurer, IStageGoalProcedurer
{
    #region 定数
    /// <summary>接地判定をとるために使う球体の半径が、コライダーの半径の何倍かを示す数値</summary>
    const float _GROUND_CHECK_RADIUS_RATE = 0.99f;

    #endregion

    [Header("キャッシュ")]
    #region キャッシュ

    [SerializeField, Tooltip("カメラ注視位置オブジェクト")]
    Transform _cameraTarget = null;

    [SerializeField, Tooltip("視点位置オブジェクト")]
    Transform _eyePoint = null;

    /// <summary>当該オブジェクトのリジッドボディ</summary>
    Rigidbody _rb = null;

    /// <summary>当該オブジェクトのカプセルコライダー</summary>
    CapsuleCollider _collider = null;

    
    #endregion


    [Header("手動定義")]
    #region メンバー
    [SerializeField, Tooltip("True : 着地している")]
    bool _isGround = false;

    [SerializeField, Tooltip("True : ジャンプした")]
    bool _isJump = false;

    [SerializeField, Tooltip("True : ブレーキ中")]
    bool _isBrake = false;

    /// <summary>True : サイドフリップを実施</summary>
    bool _isSideFlip = false;

    /// <summary>True : 柵・段差乗り越えを実施</summary>
    bool _isRunOver = false;

    [SerializeField, Tooltip("フェンスや段差の乗り越えが可能な高さ")]
    float _fenseHeight = 0.8f; 

    [SerializeField, Tooltip("地面と壁の境界角度")]
    float _slopeLimit = 45f;

    /// <summary>キャラクターの重力向き</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>接地中の床の法線</summary>
    Vector3 _floorNormal = Vector3.up;

    /// <summary>接触中の壁の法線</summary>
    Vector3 _wallNormal = Vector3.forward;
    
    /// <summary>登れる坂とみなすための中心点からの距離</summary>
    float _slopeAngleThreshold = 1f;

    /// <summary>客観速度</summary>
    float _resultSpeed = 0f;

    /// <summary>True : 重力方向を変更</summary>
    bool _doSwitchGravity = false;



    #endregion


    #region プロパティ
    /// <summary>カメラ注視位置オブジェクト</summary>
    public Transform CameraTarget => _cameraTarget;

    /// <summary>キャラクターの視点位置</summary>
    public Transform EyePoint => _eyePoint;

    /// <summary>True : 着地している</summary>
    public bool IsGround => _isGround;

    /// <summary>True : ジャンプした</summary>
    public bool IsJump { get => _isJump; set => _isJump = value; }

    /// <summary>True : ブレーキ中</summary>
    public bool IsBrake { get => _isBrake; set => _isBrake = value; }

    /// <summary>True : サイドフリップを実施</summary>
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

    /// <summary>True : 柵・段差乗り越えを実施</summary>
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

    /// <summary>キャラクターの重力向き</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }

    /// <summary>接地中の床の法線</summary>
    public Vector3 FloorNormal => _floorNormal;

    /// <summary>接触中の壁の法線</summary>
    public Vector3 WallNormal => _wallNormal;

    /// <summary>客観速度</summary>
    public float ResultSpeed => _resultSpeed;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        /* 初期化処理 */
        transform.up = -_gravityDirection;
        _rb = GetComponent<Rigidbody>();

        /* 接地判定処理用 */
        _collider = GetComponent<CapsuleCollider>();

        //円弧半径から弧長を求める公式
        _slopeAngleThreshold = 2f * _collider.radius * Mathf.Sin(Mathf.Deg2Rad * _slopeLimit / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ時は処理しない
        if (PauseManager.Instance.IsPause) return;

        GroundCheck();
    }

    void FixedUpdate()
    {
        SpeedCheck();
    }

    /// <summary>接地判定処理</summary>
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

    /// <summary>正面の壁の接触判定処理</summary>
    /// <param name="direction">壁をスキャンする方向</param>
    /// <returns>1 => 接触位置(null : 接触していない), 2 => 壁の法線</returns>
    public (Vector3?, Vector3) FrontWallContactCheck(Vector3 direction)
    {
        Vector3 normal = Vector3.zero;
        Vector3? point = null;

        Vector3 capsuleCastBaseTop = -_gravityDirection * (_collider.height - (_collider.radius * 2f));
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_collider.radius * 2f);
        
        //壁の存在を取得
        RaycastHit hit;
        if (Physics.CapsuleCast(capsuleCastBaseTop + transform.position, capsuleCastBaseBottom + transform.position,
                                _collider.radius * _GROUND_CHECK_RADIUS_RATE, direction, out hit, 0.1f, LayerManager.Instance.Ground))
        {
            normal = hit.normal;
            point = hit.point;
        }

        return (point, normal);
    }

    /// <summary>正面の柵の接触判定処理</summary>
    /// <param name="direction">壁をスキャンする方向</param>
    /// <returns>柵上部座標</returns>
    public Vector3? FrontStepContactCheck(Vector3 direction)
    {
        Vector3? onStep = null;

        Vector3 capsuleCastBaseTop = -_gravityDirection * (_collider.height + _fenseHeight - _collider.radius);
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_fenseHeight + _collider.radius + 0.05f);

        //壁の存在を取得できなければ、さらに柵・段差の向こう側の座標を取得
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

    /// <summary>移動速度測定</summary>
    void SpeedCheck()
    {
        _resultSpeed = _rb.velocity.magnitude;
    }

    /// <summary>重力方向を変更</summary>
    public void SwitchGravity(Vector3 gravityDirection)
    {
        _gravityDirection = gravityDirection;
        Vector3 forward = Vector3.ProjectOnPlane(_rb.velocity, -gravityDirection);
        transform.up = -gravityDirection;
        transform.rotation = Quaternion.LookRotation(forward, -gravityDirection);
        _doSwitchGravity = true;
    }

    /// <summary>重力方向の変更があったことを通知</summary>
    /// <returns>True : 変更あり</returns>
    public bool DoGravitySwitch()
    {
        bool result = _doSwitchGravity;
        _doSwitchGravity = false;
        return result;
    }

}

/// <summary>Animator参照用</summary>
public interface ICharacterParameterForAnimator
{
    /// <summary>客観速度</summary>
    public float ResultSpeed { get; }

    /// <summary>true : 着地している</summary>
    public bool IsGround { get; }

    /// <summary>True : ジャンプした</summary>
    public bool IsJump { get; }

    /// <summary>True : ブレーキ中</summary>
    public bool IsBrake { get; }

    /// <summary>True : サイドフリップを実施</summary>
    public bool DoSideFlip { get; }

    /// <summary>True : 柵・段差乗り越えを実施</summary>
    public bool DoRunOver { get; }

    /// <summary>重力方向の変更があったことを通知</summary>
    /// <returns>True : 変更あり</returns>
    public bool DoGravitySwitch();
}

/// <summary>カメラ制御参照用</summary>
public interface ICharacterParameterForCamera
{
    /// <summary>カメラ注視位置オブジェクト</summary>
    public Transform CameraTarget { get; }

    /// <summary>キャラクターの視点位置</summary>
    public Transform EyePoint { get; }

    /// <summary>キャラクターの重力向き</summary>
    public Vector3 GravityDirection { get; }
}

/// <summary>ステージギミックからの干渉用</summary>
public interface IInteractGimmicks
{

}

/// <summary>コースアウト判定を反映用</summary>
public interface ICourseOutProcedurer
{

}

/// <summary>ステージクリア判定を反映用</summary>
public interface IStageGoalProcedurer
{

}
