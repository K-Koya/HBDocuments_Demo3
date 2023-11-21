using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    CapsuleCollider _collider = default;

    
    #endregion


    [Header("手動定義")]
    #region メンバー
    [SerializeField, Tooltip("True : 着地している")]
    bool _isGround = false;

    [SerializeField, Tooltip("True : ジャンプした")]
    bool _isJump = false;

    [SerializeField, Tooltip("True : 壁にくっついている")]
    bool _isWall = false;

    [SerializeField, Tooltip("地面と壁の境界角度")]
    float _slopeLimit = 45f;

    /// <summary>キャラクターの重力向き</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>接地中の床の法線</summary>
    Vector3 _floorNormal = Vector3.up;

    /// <summary>SphereCastする時の基準点となる座標</summary>
    Vector3 _castBasePosition = Vector3.zero;

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

    /// <summary>True : 壁にくっついている</summary>
    public bool IsWall => _isWall;

    /// <summary>キャラクターの重力向き</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }

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
        _castBasePosition = -_gravityDirection * _collider.radius; //_collider.center + _gravityDirection * ((_collider.height - _collider.radius * 2f) / 2f);

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
        _castBasePosition = -gravityDirection * _collider.radius;
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

    /*
    /// <summary>コースアウト処理</summary>
    /// <returns>自動アニメーションをかけるインスタンス</returns>
    public GameObject DoCourseOut()
    {
        

        //コースアウトアニメーションを有効にする
        _stageCourseOut.SetActive(true);

        //速度と向きを維持
        _stageCourseOut.transform.position = transform.position;
        _stageCourseOut.transform.rotation = transform.rotation;
        Rigidbody rb = _stageCourseOut.GetComponent<Rigidbody>();
        rb.velocity = _rb.velocity;
        rb.AddTorque(2f, 0f, 0f, ForceMode.VelocityChange);
                
        //自分を非アクティブにする
        gameObject.SetActive(false);

        return _stageCourseOut;
    }
    */

    /*
    /// <summary>ステージクリア処理</summary>
    /// <returns>自動アニメーションをかけるインスタンス</returns>
    public GameObject DoStageGoal()
    {
        //ステージクリアアニメーションを有効にする
        _stageClear.SetActive(true);

        //位置と向きを維持
        _stageClear.transform.position = transform.position;
        _stageClear.transform.rotation = transform.rotation;

        //自分を非アクティブにする
        gameObject.SetActive(false);

        return _stageClear;
    }
    */
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
