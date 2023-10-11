using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CMFreeLookUtility : MonoBehaviour
{
    /// <summary>カメラズーム時のFOV最大値</summary>
    const byte _ZOOM_FOV_MAX = 100;

    /// <summary>カメラズーム時のFOV最小値</summary>
    const byte _ZOOM_FOV_MIN = 20;



    [SerializeField, Tooltip("true : カメラズーム可")]
    bool _isAbleToZoom = true;

    [SerializeField, Tooltip("FOVの数値変化量")]
    short _deltaZoomFOV = 5;

    /// <summary>true : カメラのスティック操作をズーム操作に変換する</summary>
    bool _zoomFlag = false;

    /// <summary>カメラ操作をズーム操作に変換する際の入力値</summary>
    float _zoomValue = 0f;

    /// <summary>カメラ操作をズーム操作に変換する際にカメラスピードを0にするため、0から復帰するためのキャッシュ:X軸</summary>
    float _axisSpeedCacheX = 0f;

    /// <summary>カメラ操作をズーム操作に変換する際にカメラスピードを0にするため、0から復帰するためのキャッシュ:Y軸</summary>
    float _axisSpeedCacheY = 0f;

    /// <summary>当該のCinemachineFreeLook</summary>
    CinemachineFreeLook _cm = default;


    /// <summary>true : カメラズーム可</summary>
    public bool IsAbleToZoom { get => _isAbleToZoom; set => _isAbleToZoom = value; }


    // Start is called before the first frame update
    void Start()
    {
        _cm = GetComponent<CinemachineFreeLook>();
        SeekPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //ポーズ時は止める
        if (PauseManager.Instance.IsPause) return;

        _cm.enabled = true;

        if (!_cm.Follow || !_cm.LookAt)
        {
            SeekPlayer();
        }

        //ズーム入力
        if (_zoomFlag)
        {
            _cm.m_Lens.FieldOfView = Mathf.Clamp(_cm.m_Lens.FieldOfView - _deltaZoomFOV * _zoomValue * 0.2f, _ZOOM_FOV_MIN, _ZOOM_FOV_MAX);
        }
    }

    /// <summary>操作キャラクターの位置情報をCinemachineに反映</summary>
    void SeekPlayer()
    {
        GameObject obj = GameObject.FindWithTag(TagManager.Instance.Player);
        CharacterParameter _param = null;
        if (obj) _param = obj.GetComponent<CharacterParameter>();

        if (_param)
        {
            _cm.Follow = _param.transform;
            _cm.LookAt = _param.CameraTarget ? _param.CameraTarget.transform : _param.transform;
        }
    }

    /// <summary>InputSystemのカメラズーム操作のコールバック</summary>
    /// <param name="context">入力値情報</param>
    public void OnCameraZoomCall(InputAction.CallbackContext context)
    {
        //ポーズ時は止める
        if (PauseManager.Instance.IsPause) return;

        //カメラズーム操作可否
        if (!_isAbleToZoom) return;

        //入力値が加わった時だけ操作
        if (context.started)
        {
            float delta = context.ReadValue<Vector2>().y;
            _cm.m_Lens.FieldOfView = Mathf.Clamp(_cm.m_Lens.FieldOfView - _deltaZoomFOV * delta, _ZOOM_FOV_MIN, _ZOOM_FOV_MAX);
        }
    }

    /// <summary>InputSystemのスティック操作をカメラズーム操作に切り替えるボタンのコールバック</summary>
    /// <param name="context">入力値情報</param>
    public void OnCameraZoomFlagCall(InputAction.CallbackContext context)
    {
        //ポーズ時は止める
        if (PauseManager.Instance.IsPause) return;

        //カメラズーム操作可否
        if (!_isAbleToZoom) return;

        //入力値が加わった時だけ操作
        if (context.started)
        {
            _zoomFlag = true;
            _axisSpeedCacheX = _cm.m_XAxis.m_MaxSpeed;
            _cm.m_XAxis.m_MaxSpeed = 0f;
            _axisSpeedCacheY = _cm.m_YAxis.m_MaxSpeed;
            _cm.m_YAxis.m_MaxSpeed = 0f;
        }
    }

    /// <summary>InputSystemのカメラ移動操作のコールバック</summary>
    /// <param name="context">入力値情報</param>
    public void OnCameraMoveCall(InputAction.CallbackContext context)
    {
        //ポーズ時は止める
        if (PauseManager.Instance.IsPause) return;

        //カメラズーム操作可否
        if (!_isAbleToZoom) return;

        //ズームフラグが入っている
        if (_zoomFlag)
        {
            //入力値がなくなるとズーム終了
            if (context.canceled)
            {
                _zoomFlag = false;
                _zoomValue = 0f;
                _cm.m_XAxis.m_MaxSpeed = _axisSpeedCacheX;
                _cm.m_YAxis.m_MaxSpeed = _axisSpeedCacheY;
            }
            //入力値がある時だけ実施
            else if(context.performed)
            {
                _zoomValue = context.ReadValue<Vector2>().y;                
            }
        }
    }
}
