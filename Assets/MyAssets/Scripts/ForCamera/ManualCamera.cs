using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ManualCamera : MonoBehaviour
{
    #region �萔
    /// <summary> �J�����������ړI�n�ɒ������Ƃ݂Ȃ����� </summary>
    const float _CAMERA_REACH_DESTINATION_MARGIN = 0.01f;
    /// <summary>
    /// �J�����ʒu�␳��K�p����J��������
    /// </summary>
    const float _CAMERA_USE_OFFSET_DISTANCE = 2.0f;
    /// <summary>
    /// �J�����c��]�p���
    /// </summary>
    const float _CAMERA_VERTICAL_ANGLE_LIMIT_UPPER = 80.0f;
    /// <summary>
    /// �J�����c��]�p����
    /// </summary>
    const float _CAMERA_VERTICAL_ANGLE_LIMIT_LOWER = -80.0f;
    /// <summary>
    /// �J��������]�p臒l(360��)
    /// </summary>
    const float _CAMERA_ROUND_ANGLE_EXCHANGE = 360.0f;
    #endregion

    [Header("�Q�ƃp�����[�^")]
    [SerializeField, Tooltip("�L�����N�^�[��n�`���A���Ԃ̃I�u�W�F�N�g��\������J����")]
    CinemachineVirtualCamera _mainCamera = null;


    [Header("�I�v�V�����ϐ��l")]
    [SerializeField, Tooltip("���������͂̃X�C���O�ʃ��[�g")]
    float _vSlideRate = 180.0f;

    [SerializeField, Tooltip("�c�������͂̃X�C���O�ʃ��[�g")]
    float _hSlideRate = 180.0f;

    [SerializeField, Tooltip("�������̃J�����X���C�h����")]
    float _vSlideInertia = 2.0f;

    [SerializeField, Tooltip("�c�����̃J�����X���C�h����")]
    float _hSlideInertia = 2.0f;

    [SerializeField, Tooltip("�Y�[���ʃ��[�g")]
    float _zoomRate = 200.0f;

    [SerializeField, Tooltip("�J�����Y�[���ŉ�����")]
    float _distanceUpper = 9.0f;

    [SerializeField, Tooltip("�J�����Y�[���ŋߋ���")]
    float _distanceLower = 1.0f;


    #region �p�����[�^

    /// <summary>�Y���L�����N�^�[�̃p�����[�^</summary>
    ICharacterParameterForCamera _param = null;

    /// <summary>�J�����̒����ʒu</summary>
    Transform _cameraTarget = null;

    /// <summary>�J�����Y�[���̋���</summary>
    float _cameraZoomDistance = 2.0f;

    /// <summary>�J��������]�p</summary>
    float _roundAngle = 0.0f;

    /// <summary>1�t���[�����̃J��������]�p</summary>
    float _deltaHAngle = 0.0f;

    /// <summary>�J�����c��]�p</summary>
    float _verticalAngle = 0.0f;

    /// <summary>1�t���[�����̃J�����c��]�p</summary>
    float _deltaVAngle = 0.0f;

    /// <summary>true : �J�����̏c������J�����Y�[���ɂ���</summary>
    bool _doZoom = false;

    /// <summary>�N���X�w�A�Ə���̍��W</summary>
    Vector3 _aimedPosition = Vector3.zero;

    /// <summary>���C���J�����������킹��ړI�n</summary>
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
        //�|�[�Y���͏������Ȃ�
        if (PauseManager.Instance.IsPause) return;

        if (_param is null || !_cameraTarget)
        {
            SeekPlayer();
        }
        //�o�[�`�����J�������N�����Ɍ��葀��
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

    /// <summary>����L�����N�^�[�̈ʒu����Cinemachine�ɔ��f</summary>
    void SeekPlayer()
    {
        GameObject obj = GameObject.FindWithTag(TagManager.Instance.Player);
        if (obj) _param = obj.GetComponent<CharacterParameter>();

        if (_param is not null)
        {
            _cameraTarget = _param.CameraTarget ? _param.CameraTarget.transform : obj.transform;
        }
    }

    /// <summary>�J�����X�C���O</summary>
    void cameraSwing()
    {
        //�}�E�X���͏��擾
        Vector2 axis = InputUtility.GetCameraMoveDirection;
        //�X�e�B�b�N�������݂�����΁A�J�����c�����𖳌�
        if (_doZoom)
        {
            axis.y = 0.0f;
        }

        //1�t���[���̃X�C���O�ʂ��v�Z
        _deltaHAngle = Mathf.Approximately(axis.x, 0.0f) ?
            Mathf.MoveTowards(_deltaHAngle, 0.0f, _hSlideInertia * _hSlideRate) : axis.x * _hSlideRate;
        _deltaVAngle = Mathf.Approximately(axis.y, 0.0f) ?
            Mathf.MoveTowards(_deltaVAngle, 0.0f, _vSlideInertia * _vSlideRate) : axis.y * _vSlideRate;

        //�J���������p�x�ɃX�C���O�ʂ����Z���p�x�l���A0�`360���ɕ␳
        _roundAngle = Mathf.Repeat(_roundAngle + (_deltaHAngle * Time.deltaTime), _CAMERA_ROUND_ANGLE_EXCHANGE);
        //�J���������p�x�ɃX�C���O�ʂ����Z���p�x�l���A����p�`�����p�ɕ␳
        _verticalAngle = Mathf.Clamp(_verticalAngle + (-_deltaVAngle * Time.deltaTime), _CAMERA_VERTICAL_ANGLE_LIMIT_LOWER, _CAMERA_VERTICAL_ANGLE_LIMIT_UPPER);

        //�J������]�ʂ��p�x���Z�o�����Z
        Vector3 relativePos = Quaternion.Euler(_verticalAngle, _roundAngle, 0) * (Vector3.back * _cameraZoomDistance);
        relativePos = transform.rotation * relativePos;
        _cameraDestination = relativePos + transform.position;
    }

    /// <summary>�J�������� distance �̐ݒ�</summary>
    void ctrlDistance()
    {
        float deltaDistance = InputUtility.GetCameraZoomValue;

        //�Y�[���t���O�w��
        if (InputUtility.GetCameraZoomFlagDown)
        {
            _doZoom = !_doZoom;
        }

        //�X�e�B�b�N�������݂�����A���A�X�e�B�b�N�㉺���͂�����΃X�e�B�b�N���͂𔽉f
        if (_doZoom)
        {
            deltaDistance = InputUtility.GetCameraMoveDirection.y * 0.1f;
        }

        deltaDistance *= (_zoomRate * Time.deltaTime);
        _cameraZoomDistance = Mathf.Clamp(_cameraZoomDistance - deltaDistance, _distanceLower, _distanceUpper);


        /*
        //�Y�[���p�X���C�_�[�I�u�W�F�N�g�����݂��Ă���ꍇ
        if (slider != null && slider != default)
        {
            //�Y�[���p�X���C�_�[�̕ω�������΂������K�p
            if (slider.IsChanged)
            {
                cameraZoomDistance = (distanceUpper - distanceLower) * slider.SliderValue + distanceLower;
                slider.IsChanged = false;
            }
            //���̃Y�[�����͂��m�F���ꂽ�ꍇ�̓X���C�_�[�ɂ����f
            else if (Mathf.Abs(deltaDistance) > 0.0f)
            {
                slider.SliderValue = (cameraZoomDistance - distanceLower) / (distanceUpper - distanceLower);
                slider.IsRequestSliderSet = true;
            }
        }
        */
    }

    /// <summary>�J�����̒����Ώۂ�ݒ�</summary>
    void setCameraTarget()
    {
        //�J�����œ_�ƂȂ�ʒu�ɋ߂Â��Ă��Ȃ���΁A�߂Â��鏈��������
        transform.position = _cameraTarget.transform.position;
        if (Vector3.SqrMagnitude(_cameraTarget.transform.position - transform.position) > Mathf.Pow(_CAMERA_REACH_DESTINATION_MARGIN, 2))
        {
            transform.position = Vector3.Lerp(_cameraTarget.transform.position, transform.position, Mathf.Min(20.0f * Time.deltaTime, 2.0f));
        }
    }

    /// <summary>���C���J������ړI�n��</summary>
    void leadCameraDistination()
    {
        _mainCamera.transform.position = _cameraDestination;
        _mainCamera.transform.LookAt(transform, transform.up);
    }

    /// <summary>�J�����ʒu���A��Q���������悤�ɕ␳</summary>
    void avoidObject()
    {
        RaycastHit rayhitGround;
        if (Physics.Linecast(_cameraTarget.transform.position, _mainCamera.transform.position, out rayhitGround, LayerManager.Instance.Ground))
        {
            //_mainCamera.transform.position = Vector3.Lerp(rayhitGround.point, _mainCamera.transform.position, 0.2f);
            _mainCamera.transform.position = rayhitGround.point;
        }
    }

    /// <summary>�J�����̉��������Đݒ�</summary>
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
