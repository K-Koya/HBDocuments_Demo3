using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CMFreeLookUtility : MonoBehaviour
{
    /// <summary>�J�����Y�[������FOV�ő�l</summary>
    const byte _ZOOM_FOV_MAX = 100;

    /// <summary>�J�����Y�[������FOV�ŏ��l</summary>
    const byte _ZOOM_FOV_MIN = 20;



    [SerializeField, Tooltip("true : �J�����Y�[����")]
    bool _isAbleToZoom = true;

    [SerializeField, Tooltip("FOV�̐��l�ω���")]
    short _deltaZoomFOV = 5;

    /// <summary>true : �J�����̃X�e�B�b�N������Y�[������ɕϊ�����</summary>
    bool _zoomFlag = false;

    /// <summary>�J����������Y�[������ɕϊ�����ۂ̓��͒l</summary>
    float _zoomValue = 0f;

    /// <summary>�J����������Y�[������ɕϊ�����ۂɃJ�����X�s�[�h��0�ɂ��邽�߁A0���畜�A���邽�߂̃L���b�V��:X��</summary>
    float _axisSpeedCacheX = 0f;

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

        //�Y�[������
        if (_zoomFlag)
        {
            _cm.m_Lens.FieldOfView = Mathf.Clamp(_cm.m_Lens.FieldOfView - _deltaZoomFOV * _zoomValue * 0.2f, _ZOOM_FOV_MIN, _ZOOM_FOV_MAX);
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

    /// <summary>InputSystem�̃J�����Y�[������̃R�[���o�b�N</summary>
    /// <param name="context">���͒l���</param>
    public void OnCameraZoomCall(InputAction.CallbackContext context)
    {
        //�|�[�Y���͎~�߂�
        if (PauseManager.Instance.IsPause) return;

        //�J�����Y�[�������
        if (!_isAbleToZoom) return;

        //���͒l�������������������
        if (context.started)
        {
            float delta = context.ReadValue<Vector2>().y;
            _cm.m_Lens.FieldOfView = Mathf.Clamp(_cm.m_Lens.FieldOfView - _deltaZoomFOV * delta, _ZOOM_FOV_MIN, _ZOOM_FOV_MAX);
        }
    }

    /// <summary>InputSystem�̃X�e�B�b�N������J�����Y�[������ɐ؂�ւ���{�^���̃R�[���o�b�N</summary>
    /// <param name="context">���͒l���</param>
    public void OnCameraZoomFlagCall(InputAction.CallbackContext context)
    {
        //�|�[�Y���͎~�߂�
        if (PauseManager.Instance.IsPause) return;

        //�J�����Y�[�������
        if (!_isAbleToZoom) return;

        //���͒l�������������������
        if (context.started)
        {
            _zoomFlag = true;
            _axisSpeedCacheX = _cm.m_XAxis.m_MaxSpeed;
            _cm.m_XAxis.m_MaxSpeed = 0f;
            _axisSpeedCacheY = _cm.m_YAxis.m_MaxSpeed;
            _cm.m_YAxis.m_MaxSpeed = 0f;
        }
    }

    /// <summary>InputSystem�̃J�����ړ�����̃R�[���o�b�N</summary>
    /// <param name="context">���͒l���</param>
    public void OnCameraMoveCall(InputAction.CallbackContext context)
    {
        //�|�[�Y���͎~�߂�
        if (PauseManager.Instance.IsPause) return;

        //�J�����Y�[�������
        if (!_isAbleToZoom) return;

        //�Y�[���t���O�������Ă���
        if (_zoomFlag)
        {
            //���͒l���Ȃ��Ȃ�ƃY�[���I��
            if (context.canceled)
            {
                _zoomFlag = false;
                _zoomValue = 0f;
                _cm.m_XAxis.m_MaxSpeed = _axisSpeedCacheX;
                _cm.m_YAxis.m_MaxSpeed = _axisSpeedCacheY;
            }
            //���͒l�����鎞�������{
            else if(context.performed)
            {
                _zoomValue = context.ReadValue<Vector2>().y;                
            }
        }
    }
}
