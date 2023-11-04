using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
[DefaultExecutionOrder(100)]
public class CMFreeLookUtility : MonoBehaviour
{
    /// <summary>カメラズーム時のFOV最大値</summary>
    const byte _ZOOM_FOV_MAX = 120;

    /// <summary>カメラズーム時のFOV最小値</summary>
    const byte _ZOOM_FOV_MIN = 20;



    [SerializeField, Tooltip("true : カメラズーム可")]
    bool _isAbleToZoom = true;

    [SerializeField, Tooltip("FOVの数値変化量")]
    short _deltaZoomFOV = 5;

    /// <summary>true : カメラのスティック操作をズーム操作に変換する</summary>
    bool _zoomFlag = false;

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

        //ズーム入力切り替え
        if (InputUtility.GetCameraZoomFlagDown)
        {
            Debug.Log(_zoomFlag);

            if (_zoomFlag)
            {
                _zoomFlag = false;
                _cm.m_YAxis.m_MaxSpeed = _axisSpeedCacheY;
            }
            else
            {
                _zoomFlag = true;
                _axisSpeedCacheY = _cm.m_YAxis.m_MaxSpeed;
                _cm.m_YAxis.m_MaxSpeed = 0f;
            }
        }

        //ズーム実施
        if (_zoomFlag && InputUtility.GetCameraMoving)
        {
            CameraZoom(InputUtility.GetCameraMoveDirection.y * 0.2f);
        }
        else if(Mathf.Abs(InputUtility.GetCameraZoomValue) > 0f)
        {
            CameraZoom(InputUtility.GetCameraZoomValue);
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

    /// <summary>カメラズーム操作</summary>
    /// <param name="value">入力量</param>
    void CameraZoom(float value)
    {
        _cm.m_Lens.FieldOfView = Mathf.Clamp(_cm.m_Lens.FieldOfView - _deltaZoomFOV * value, _ZOOM_FOV_MIN, _ZOOM_FOV_MAX);
    }
}
