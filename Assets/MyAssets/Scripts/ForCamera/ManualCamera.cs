using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ManualCamera : MonoBehaviour
{
    #region 定数
    /// <summary> カメラ方向が目的地に着いたとみなす距離 </summary>
    const float _CAMERA_REACH_DESTINATION_MARGIN = 0.01f;
    /// <summary>
    /// カメラ位置補正を適用するカメラ距離
    /// </summary>
    const float _CAMERA_USE_OFFSET_DISTANCE = 2.0f;
    /// <summary>
    /// カメラ縦回転角上限
    /// </summary>
    const float _CAMERA_VERTICAL_ANGLE_LIMIT_UPPER = 80.0f;
    /// <summary>
    /// カメラ縦回転角下限
    /// </summary>
    const float _CAMERA_VERTICAL_ANGLE_LIMIT_LOWER = -80.0f;
    /// <summary>
    /// カメラ横回転角閾値(360°)
    /// </summary>
    const float _CAMERA_ROUND_ANGLE_EXCHANGE = 360.0f;
    #endregion

    [Header("参照パラメータ")]
    [SerializeField, Tooltip("キャラクターや地形等、実態のオブジェクトを表示するカメラ")]
    CinemachineVirtualCamera _mainCamera = null;


    [Header("オプション可変数値")]
    [SerializeField, Tooltip("横方向入力のスイング量レート")]
    float _vSlideRate = 180.0f;

    [SerializeField, Tooltip("縦方向入力のスイング量レート")]
    float _hSlideRate = 180.0f;

    [SerializeField, Tooltip("横方向のカメラスライド慣性")]
    float _vSlideInertia = 2.0f;

    [SerializeField, Tooltip("縦方向のカメラスライド慣性")]
    float _hSlideInertia = 2.0f;

    [SerializeField, Tooltip("ズーム量レート")]
    float _zoomRate = 200.0f;

    [SerializeField, Tooltip("カメラズーム最遠距離")]
    float _distanceUpper = 9.0f;

    [SerializeField, Tooltip("カメラズーム最近距離")]
    float _distanceLower = 1.0f;


    #region パラメータ

    /// <summary>該当キャラクターのパラメータ</summary>
    ICharacterParameterForCamera _param = null;

    /// <summary>カメラの注視位置</summary>
    Transform _cameraTarget = null;

    /// <summary>カメラズームの距離</summary>
    float _cameraZoomDistance = 2.0f;

    /// <summary>カメラ横回転角</summary>
    float _roundAngle = 0.0f;

    /// <summary>1フレーム毎のカメラ横回転角</summary>
    float _deltaHAngle = 0.0f;

    /// <summary>カメラ縦回転角</summary>
    float _verticalAngle = 0.0f;

    /// <summary>1フレーム毎のカメラ縦回転角</summary>
    float _deltaVAngle = 0.0f;

    /// <summary>true : カメラの縦操作をカメラズームにする</summary>
    bool _doZoom = false;

    /// <summary>クロスヘア照準先の座標</summary>
    Vector3 _aimedPosition = Vector3.zero;

    /// <summary>メインカメラを向かわせる目的地</summary>
    Vector3 _cameraDestination = Vector3.zero;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        SeekPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ時は処理しない
        if (PauseManager.Instance.IsPause) return;

        if (_param is null || !_cameraTarget)
        {
            SeekPlayer();
        }
        //バーチャルカメラが起動中に限り操作
        else if(_mainCamera.gameObject.activeSelf)
        {
            SwitchGravity();

            ctrlDistance();
            setCameraTarget();
            cameraSwing();
            leadCameraDistination();
            avoidObject();
        }
    }

    /// <summary>操作キャラクターの位置情報をCinemachineに反映</summary>
    void SeekPlayer()
    {
        GameObject obj = GameObject.FindWithTag(TagManager.Instance.Player);
        if (obj) _param = obj.GetComponent<CharacterParameter>();

        if (_param is not null)
        {
            _cameraTarget = _param.CameraTarget ? _param.CameraTarget.transform : obj.transform;
        }
    }

    /// <summary>カメラスイング</summary>
    void cameraSwing()
    {
        //マウス入力情報取得
        Vector2 axis = InputUtility.GetCameraMoveDirection;
        //スティック押し込みがあれば、カメラ縦方向を無効
        if (_doZoom)
        {
            axis.y = 0.0f;
        }

        //1フレームのスイング量を計算
        _deltaHAngle = Mathf.Approximately(axis.x, 0.0f) ?
            Mathf.MoveTowards(_deltaHAngle, 0.0f, _hSlideInertia * _hSlideRate) : axis.x * _hSlideRate;
        _deltaVAngle = Mathf.Approximately(axis.y, 0.0f) ?
            Mathf.MoveTowards(_deltaVAngle, 0.0f, _vSlideInertia * _vSlideRate) : axis.y * _vSlideRate;

        //カメラ水平角度にスイング量を加算し角度値を、0～360°に補正
        _roundAngle = Mathf.Repeat(_roundAngle + (_deltaHAngle * Time.deltaTime), _CAMERA_ROUND_ANGLE_EXCHANGE);
        //カメラ垂直角度にスイング量を加算し角度値を、上限角～下限角に補正
        _verticalAngle = Mathf.Clamp(_verticalAngle + (-_deltaVAngle * Time.deltaTime), _CAMERA_VERTICAL_ANGLE_LIMIT_LOWER, _CAMERA_VERTICAL_ANGLE_LIMIT_UPPER);

        //カメラ回転量を角度より算出し合算
        Vector3 relativePos = Quaternion.Euler(_verticalAngle, _roundAngle, 0) * (Vector3.back * _cameraZoomDistance);
        relativePos = transform.rotation * relativePos;
        _cameraDestination = relativePos + transform.position;
    }

    /// <summary>カメラ距離 distance の設定</summary>
    void ctrlDistance()
    {
        float deltaDistance = InputUtility.GetCameraZoomValue;

        //ズームフラグ指定
        if (InputUtility.GetCameraZoomFlagDown)
        {
            _doZoom = !_doZoom;
        }

        //スティック押し込みがあり、かつ、スティック上下入力があればスティック入力を反映
        if (_doZoom)
        {
            deltaDistance = InputUtility.GetCameraMoveDirection.y * 0.1f;
        }

        deltaDistance *= (_zoomRate * Time.deltaTime);
        _cameraZoomDistance = Mathf.Clamp(_cameraZoomDistance - deltaDistance, _distanceLower, _distanceUpper);


        /*
        //ズーム用スライダーオブジェクトが存在している場合
        if (slider != null && slider != default)
        {
            //ズーム用スライダーの変化があればそちらを適用
            if (slider.IsChanged)
            {
                cameraZoomDistance = (distanceUpper - distanceLower) * slider.SliderValue + distanceLower;
                slider.IsChanged = false;
            }
            //他のズーム入力が確認された場合はスライダーにも反映
            else if (Mathf.Abs(deltaDistance) > 0.0f)
            {
                slider.SliderValue = (cameraZoomDistance - distanceLower) / (distanceUpper - distanceLower);
                slider.IsRequestSliderSet = true;
            }
        }
        */
    }

    /// <summary>カメラの注視対象を設定</summary>
    void setCameraTarget()
    {
        //カメラ焦点となる位置に近づいていなければ、近づける処理をする
        transform.position = _cameraTarget.transform.position;
        if (Vector3.SqrMagnitude(_cameraTarget.transform.position - transform.position) > Mathf.Pow(_CAMERA_REACH_DESTINATION_MARGIN, 2))
        {
            transform.position = Vector3.Lerp(_cameraTarget.transform.position, transform.position, Mathf.Min(20.0f * Time.deltaTime, 2.0f));
        }
    }

    /// <summary>メインカメラを目的地へ</summary>
    void leadCameraDistination()
    {
        _mainCamera.transform.position = _cameraDestination;
        _mainCamera.transform.LookAt(transform, transform.up);
    }

    /// <summary>カメラ位置を、障害物を避けるように補正</summary>
    void avoidObject()
    {
        RaycastHit rayhitGround;
        if (Physics.Linecast(_cameraTarget.transform.position, _mainCamera.transform.position, out rayhitGround, LayerManager.Instance.Ground))
        {
            //_mainCamera.transform.position = Vector3.Lerp(rayhitGround.point, _mainCamera.transform.position, 0.2f);
            _mainCamera.transform.position = rayhitGround.point;
        }
    }

    /// <summary>カメラの下方向を再設定</summary>
    void SwitchGravity()
    {
        float dot = Vector3.Dot(transform.up, _param.GravityDirection);
        if (dot > -0.99f)
        {
            //transform.up = Vector3.Lerp(transform.up, -_param.GravityDirection, 3f * Time.deltaTime);
            transform.up = Vector3.RotateTowards(transform.up, -_param.GravityDirection, 5f * Time.deltaTime ,10f);
        }
        else
        {
            transform.up = -_param.GravityDirection;
        }
    }
}
