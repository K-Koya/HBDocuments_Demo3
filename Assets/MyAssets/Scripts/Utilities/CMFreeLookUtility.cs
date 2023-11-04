using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
[DefaultExecutionOrder(100)]
public class CMFreeLookUtility : MonoBehaviour
{
    /// <summary>�J�����Y�[������FOV�ő�l</summary>
    const byte _ZOOM_FOV_MAX = 120;

    /// <summary>�J�����Y�[������FOV�ŏ��l</summary>
    const byte _ZOOM_FOV_MIN = 20;



    [SerializeField, Tooltip("true : �J�����Y�[����")]
    bool _isAbleToZoom = true;

    [SerializeField, Tooltip("FOV�̐��l�ω���")]
    short _deltaZoomFOV = 5;

    /// <summary>true : �J�����̃X�e�B�b�N������Y�[������ɕϊ�����</summary>
    bool _zoomFlag = false;

    /// <summary>�J����������Y�[������ɕϊ�����ۂɃJ�����X�s�[�h��0�ɂ��邽�߁A0���畜�A���邽�߂̃L���b�V��:Y��</summary>
    float _axisSpeedCacheY = 0f;

    /// <summary>���Y��CinemachineFreeLook</summary>
    CinemachineFreeLook _cm = default;


    /// <summary>true : �J�����Y�[����</summary>
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
        //�|�[�Y���͎~�߂�
        if (PauseManager.Instance.IsPause) return;

        _cm.enabled = true;

        if (!_cm.Follow || !_cm.LookAt)
        {
            SeekPlayer();
        }

        //�Y�[�����͐؂�ւ�
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

        //�Y�[�����{
        if (_zoomFlag && InputUtility.GetCameraMoving)
        {
            CameraZoom(InputUtility.GetCameraMoveDirection.y * 0.2f);
        }
        else if(Mathf.Abs(InputUtility.GetCameraZoomValue) > 0f)
        {
            CameraZoom(InputUtility.GetCameraZoomValue);
        }
    }

    /// <summary>����L�����N�^�[�̈ʒu����Cinemachine�ɔ��f</summary>
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

    /// <summary>�J�����Y�[������</summary>
    /// <param name="value">���͗�</param>
    void CameraZoom(float value)
    {
        _cm.m_Lens.FieldOfView = Mathf.Clamp(_cm.m_Lens.FieldOfView - _deltaZoomFOV * value, _ZOOM_FOV_MIN, _ZOOM_FOV_MAX);
    }
}
