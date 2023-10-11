using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterParameter : MonoBehaviour
{
    #region 定数
    /// <summary>接地判定をとるために使う球体の半径が、コライダーの半径の何倍かを示す数値</summary>
    const float _GROUND_CHECK_RADIUS_RATE = 0.99f;

    #endregion

    [Header("キャッシュ")]
    #region キャッシュ

    [SerializeField, Tooltip("カメラ注視位置オブジェクト")]
    protected Transform _cameraTarget = null;

    [SerializeField, Tooltip("視点位置オブジェクト")]
    protected Transform _eyePoint = null;

    /// <summary>当該オブジェクトのカプセルコライダー</summary>
    CapsuleCollider _collider = default;

    #endregion

    [Header("手動定義")]
    #region メンバー
    [SerializeField, Tooltip("True : 着地している")]
    bool _isGround = false;

    [SerializeField, Tooltip("True : 壁にくっついている")]
    bool _isWall = false;

    [SerializeField, Tooltip("地面と壁の境界角度")]
    float _slopeLimit = 45f;


    /// <summary>キャラクターの重力向き</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>SphereCastする時の基準点となる座標</summary>
    Vector3 _castBasePosition = Vector3.zero;

    /// <summary>登れる坂とみなすための中心点からの距離</summary>
    float _slopeAngleThreshold = 1f;

    #endregion


    #region プロパティ
    /// <summary>カメラ注視位置オブジェクト</summary>
    public Transform CameraTarget => _cameraTarget;

    /// <summary>キャラクターの視点位置</summary>
    public Transform EyePoint => _eyePoint;

    /// <summary>True : 着地している</summary>
    public bool IsGround => _isGround;

    /// <summary>True : 壁にくっついている</summary>
    public bool IsWall => _isWall;

    /// <summary>キャラクターの重力向き</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        /* 接地判定処理用 */
        _collider = GetComponent<CapsuleCollider>();
        //_collider.providesContacts = true;
        _castBasePosition = _collider.center + Vector3.down * ((_collider.height - _collider.radius * 2f) / 2f);

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

    /// <summary>接地判定処理</summary>
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
