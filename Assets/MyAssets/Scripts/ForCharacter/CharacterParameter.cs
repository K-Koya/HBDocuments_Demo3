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

    /// <summary>キャラクターの状態管理フラグ</summary>
    class StateFlag
    {
        /// <summary>ビットフラグ</summary>
        public enum Flags : uint
        {
            /// <summary>初期化</summary>
            Clear = 0,
            /// <summary>true : ムービー等自動再生中</summary>
            OnMovie = 1,
            /// <summary>true : 自動操作中（操作を受け付けない）</summary>
            OnAutoMovement = 2,
            /// <summary>true : プレイヤーキャラクターである</summary>
            IsPlayer = 4,
            /// <summary>true : 敵キャラクターである</summary>
            IsEnemy = 8,
            /// <summary>true : 着地している</summary>
            IsGround = 16,
            /// <summary>true : ジャンプ中である</summary>
            IsJump = 32,
            /// <summary>true : ブレーキ中である</summary>
            IsBrake = 64,
            /// <summary>true : 壁張り付き中である</summary>
            IsWall = 128,
        }

        /// <summary>ビットフラグ管理変数</summary>
        Flags _flag = Flags.Clear;

        /// <summary>フラグセット</summary>
        /// <param name="flag">対象</param>
        /// <param name="value">値</param>
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

        /// <summary>フラグゲット</summary>
        /// <param name="flag">対象</param>
        /// <returns>値</returns>
        public bool Get(Flags flag)
        {
            return ((uint)_flag & (uint)flag) > 0;
        }
    }

    /// <summary>キャラクターの操作制限のかかる特殊な行動</summary>
    enum ExtraMotion : byte
    {
        /// <summary>未発生</summary>
        None = 0,
        /// <summary>サイドフリップ中</summary>
        SideFlip,
        /// <summary>柵・段差乗り越え中</summary>
        RunOver,
        /// <summary>重力変更中</summary>
        SwitchGravity,

        /// <summary>アクセルゲートから滑空する態勢</summary>
        AccelGateGlide,
    }




    [Header("手動定義")]
    #region メンバー
    [SerializeField, Tooltip("フェンス乗り越えもなく飛び乗れる高さ")]
    float _stairHeight = 0.2f;

    [SerializeField, Tooltip("フェンスや段差の乗り越えが可能な高さ")]
    float _fenseHeight = 0.8f; 

    [SerializeField, Tooltip("地面と壁の境界角度")]
    float _slopeLimit = 45f;

    /// <summary>キャラクターの操作制限のかかる特殊な行動の状態</summary>
    ExtraMotion _exMotion = ExtraMotion.None;

    /// <summary>True : ExtraMotion（キャラクターの操作制限のかかる特殊な行動）が初めて有効になった</summary>
    bool _isExtraMotionInit = false;

    /// <summary>各種状態を表すのビットフラグ</summary>
    StateFlag _state = new StateFlag();

    /// <summary>キャラクターの重力向き</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>接地中の床の法線</summary>
    Vector3 _floorNormal = Vector3.up;

    /// <summary>接触中の壁の法線</summary>
    Vector3? _wallNormal = null;

    /// <summary>登れる坂とみなすための中心点からの距離</summary>
    float _slopeAngleThreshold = 1f;

    /// <summary>客観速度</summary>
    float _resultSpeed = 0f;



    #endregion


    #region プロパティ
    /// <summary>カメラ注視位置オブジェクト</summary>
    public Transform CameraTarget => _cameraTarget;

    /// <summary>キャラクターの視点位置</summary>
    public Transform EyePoint => _eyePoint;

    /// <summary>True : ムービー等で自動的に動かしている</summary>
    public bool OnAutoMovement { get => _state.Get(StateFlag.Flags.OnAutoMovement); set => _state.Set(StateFlag.Flags.OnAutoMovement, value); }

    /// <summary>True : 着地している</summary>
    public bool IsGround => _state.Get(StateFlag.Flags.IsGround);

    /// <summary>True : ジャンプした</summary>
    public bool IsJump { get => _state.Get(StateFlag.Flags.IsJump); set => _state.Set(StateFlag.Flags.IsJump, value); }

    /// <summary>True : ブレーキ中</summary>
    public bool IsBrake { get => _state.Get(StateFlag.Flags.IsBrake); set => _state.Set(StateFlag.Flags.IsBrake, value); }

    /// <summary>True : 壁張り付き中</summary>
    public bool IsWall { get => _state.Get(StateFlag.Flags.IsWall); set => _state.Set(StateFlag.Flags.IsWall, value); }

    /// <summary>True : サイドフリップを実施中</summary>
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

    /// <summary>True : サイドフリップを実施直後</summary>
    public bool DoSideFlip
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.SideFlip;
    }

    /// <summary>True : 柵・段差乗り越えを実施中</summary>
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

    /// <summary>True : 柵・段差乗り越えを実施直後</summary>
    public bool DoRunOver
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.RunOver;
    }

    /// <summary>True : 重力が瞬間的に変わったため、適応中</summary>
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

    /// <summary>True : 重力が瞬間的に変わった直後</summary>
    public bool DoSwitchGravity
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.SwitchGravity;
    }

    /// <summary>True : アクセルゲートの滑空態勢中</summary>
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

    /// <summary>True : アクセルゲートの滑空態勢直後</summary>
    public bool DoAccelGateGlide
    {
        get => _isExtraMotionInit && _exMotion is ExtraMotion.AccelGateGlide;
    }

    /// <summary>True : アクセルゲートの滑空態勢を終えた直後</summary>
    public bool FinishAccelGateGlide
    {
        get => _isExtraMotionInit && _exMotion is not ExtraMotion.AccelGateGlide;
    }

    /// <summary>キャラクターの重力向き</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }

    /// <summary>接地中の床の法線</summary>
    public Vector3 FloorNormal => _floorNormal;

    /// <summary>接触中の壁の法線</summary>
    public Vector3? WallNormal => _wallNormal;

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
    }

    void FixedUpdate()
    {
        SpeedCheck();
        GroundCheck();

        //ExtraMotionの初期化フラグをリセット
        ResetExtraMotionInit();
    }

    /// <summary>ExtraMotionの要求から1フレーム経ち、初期化フラグのリセットを要求</summary>
    public void ResetExtraMotionInit()
    {
        _isExtraMotionInit = false;
    }

    /// <summary>接地判定処理</summary>
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

    /// <summary>正面の壁の接触判定処理</summary>
    /// <param name="direction">壁をスキャンする方向</param>
    /// <returns>1 => 接触位置(null : 接触していない), 2 => 壁の法線</returns>
    public (Vector3?, Vector3) FrontWallContactCheck(Vector3 direction)
    {
        Vector3 normal = Vector3.zero;
        Vector3? point = null;

        Vector3 capsuleCastBaseTop = -_gravityDirection * (_collider.height - (_collider.radius * 2f));
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_stairHeight + _collider.radius * 2f);

        _wallNormal = null;
        
        //壁の存在を取得
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

    /// <summary>正面の柵の接触判定処理</summary>
    /// <param name="direction">壁をスキャンする方向</param>
    /// <returns>柵上部座標</returns>
    public Vector3? FrontStepContactCheck(Vector3 direction)
    {
        Vector3? onStep = null;

        Vector3 forWallDetectBackLine = transform.position - (_collider.radius * transform.forward);
        Vector3 capsuleCastBaseTop = -_gravityDirection * (_collider.height + _fenseHeight - _collider.radius);
        Vector3 capsuleCastBaseBottom = -_gravityDirection * (_fenseHeight + _collider.radius);

        //壁の存在を取得できなければ、さらに柵・段差の向こう側の座標を取得
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

    /// <summary>移動速度測定</summary>
    void SpeedCheck()
    {
        _resultSpeed = _rb.velocity.magnitude;
    }

    /// <summary>イベント前後で重力方向を変更</summary>
    /// <param name="afterTransform">イベント後の姿勢サンプル</param>
    public void SwitchGravityInEvent(Transform afterTransform)
    {
        _gravityDirection = -afterTransform.up;
        transform.up = afterTransform.up;
        transform.rotation = Quaternion.LookRotation(afterTransform.forward, afterTransform.up);
    }

    /// <summary>重力方向を即座に変更</summary>
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

    /// <summary>重力方向を連続的に変更</summary>
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

    /// <summary>True : 壁張り付き中</summary>
    public bool IsWall { get; }

    /// <summary>True : サイドフリップを実施</summary>
    public bool DoSideFlip { get; }

    /// <summary>True : 柵・段差乗り越えを実施</summary>
    public bool DoRunOver { get; }

    /// <summary>True : 重力が瞬間的に変わった</summary>
    public bool DoSwitchGravity { get; }

    /// <summary>True : アクセルゲートの滑空態勢直後</summary>
    public bool DoAccelGateGlide { get; }

    /// <summary>True : アクセルゲートの滑空態勢を終えた直後</summary>
    public bool FinishAccelGateGlide { get; }
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

/// <summary>地面を見つけるための情報</summary>
public interface IDetectGround
{
    /// <summary>キャラクターの重力向き</summary>
    public Vector3 GravityDirection { get; }

    /// <summary>キャラクターの足元姿勢</summary>
    public Transform transform { get; }

    /// <summary>true : 着地している</summary>
    public bool IsGround { get; }
}

/// <summary>チュートリアル表示用</summary>
public interface ICharacterParameterForTutorial
{
    /// <summary>客観速度</summary>
    public float ResultSpeed { get; }

    /// <summary>true : 着地している</summary>
    public bool IsGround { get; }

    /// <summary>True : ジャンプした</summary>
    public bool IsJump { get; }

    /// <summary>True : ブレーキ中</summary>
    public bool IsBrake { get; }

    /// <summary>True : 壁張り付き中</summary>
    public bool IsWall { get; }

    /// <summary>True : サイドフリップを実施</summary>
    public bool DoSideFlip { get; }

    /// <summary>True : 柵・段差乗り越えを実施</summary>
    public bool DoRunOver { get; }

    /// <summary>True : 重力が瞬間的に変わった</summary>
    public bool DoSwitchGravity { get; }
}


/// <summary>ステージギミックからの干渉用</summary>
public interface IInteractGimmicks
{
    /// <summary>重力方向を即座に変更</summary>
    public void SwitchGravityImmediately(Vector3 gravityDirection);

    /// <summary>重力方向を連続的に変更</summary>
    public void SwichGravityChainning(Vector3 gravityDirection);

    /// <summary>キャラクターの足元姿勢</summary>
    public Transform transform { get; }
}

/// <summary>コースアウト判定を反映用</summary>
public interface ICourseOutProcedurer
{

}

/// <summary>中継用アクセルゲート干渉用</summary>
public interface IRelayAccelGateProcedurer
{
    /// <summary>True : アクセルゲートの滑空態勢中</summary>
    public bool IsAccelGateGlide { get; set; }

    /// <summary>True : ムービー等で自動的に動かしている</summary>
    public bool OnAutoMovement { get; set; }

    /// <summary>イベント前後で重力方向を変更</summary>
    /// <param name="afterTransform">イベント後の姿勢サンプル</param>
    public void SwitchGravityInEvent(Transform afterTransform);
}

/// <summary>ステージクリア判定を反映用</summary>
public interface IStageGoalProcedurer
{

}
