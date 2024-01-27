using Cinemachine;
using UnityEngine;

public class StageManagerBase : MonoBehaviour
{
    /// <summary>�o�[�`�����J�����̕W��Priority</summary>
    const byte _BASE_CAMERA_PRIORITY = 20;


    /// <summary>���݂̃X�e�[�W�̃}�l�[�W���[</summary>
    static protected StageManagerBase _Current = null;

    /// <summary>���݂̑���L�����N�^�[</summary>
    static protected CharacterKind _CurrentCharacter = CharacterKind.Junichi;

    /// <summary>�f�����[�r�[�\���p�L�����N�^�[</summary>
    protected GameObject _characterForDemo = null;

    /// <summary>�R�[�X�A�E�g���\���p�L�����N�^�[</summary>
    protected GameObject _characterForCourseOut = null;

    /// <summary>����L�����N�^�[</summary>
    protected GameObject _characterForControl = null;


    /// <summary>�f�����[�r�[�\���p�L�����N�^�[�̃A�j���[�V��������</summary>
    protected AnimatorForDemoMovie _characterForDemoAnim = null;


    [SerializeField, Tooltip("�X�e�[�W���펞�̏��")]
    protected StateOnStage _state = StateOnStage.Playing;


    [Header("�X�e�[�W�J�n���̃f���p���i")]
    [SerializeField, Tooltip("�L�����N�^�[�̍ŏ��̐��ʕ����Ɠ��������̃I�u�W�F�N�g���w��")]
    protected Transform _characterFrontReference = null;

    [SerializeField, Tooltip("�X�e�[�W�J�n���̃f���𗬂��J����")]
    protected CinemachineVirtualCamera _introCamera = null;

    [SerializeField, Tooltip("�X�e�[�W�J�n���̃f���𗬂��J������Y�킹��DollyCart")]
    protected CinemachineDollyCart _introCameraDollyCart = null;

    [SerializeField, Tooltip("�X�e�[�W�J�n�f�����ɃL�����N�^�[��Y�킹��DollyCart")]
    protected CinemachineDollyCart _introDollyCart = null;


    [Header("�X�e�[�W���풆�̕��i")]
    [SerializeField, Tooltip("�L�����N�^�[��n�`���A���Ԃ̃I�u�W�F�N�g��\������J����")]
    protected CinemachineVirtualCamera _mainCamera = null;


    [Header("�X�e�[�W�R�[�X�A�E�g�̃f���p���i")]
    [SerializeField, Tooltip("�X�e�[�W�R�[�X�A�E�g�ȂǁA���s���ɗ��p����J����")]
    protected CinemachineVirtualCamera _missCamera = null;

    [SerializeField, Tooltip("�R�[�X�A�E�g��A�����j���[�Ɉڍs���邩���w��")]
    protected float _missMenuSwitchTime = 3f;

    /*
    [Header("�R�[�X�A�E�g��̕��A�f���p���i")]
    [SerializeField, Tooltip("�R�[�X�A�E�g���A��A�ŏ��̐��ʕ����Ɠ��������̃I�u�W�F�N�g���w��")]
    protected Transform _revivalFrontReference = null;

    [SerializeField, Tooltip("�R�[�X�A�E�g���A�f���̃J����")]
    protected CinemachineVirtualCamera _revivalCamera = null;
    */


    [Header("�X�e�[�W�N���A���̃f���p���i")]
    [SerializeField, Tooltip("�X�e�[�W�N���A���̃f���𗬂����̃J����")]
    protected CinemachineVirtualCamera _clearAnimationCamera = null;

    [SerializeField, Tooltip("�X�e�[�W�N���A���ɃL�����N�^�[��Y�킹��DollyCart")]
    protected CinemachineDollyCart _clearDollyCart = null;

    /// <summary>�X�e�[�W�N���A���̃f���𗬂��͂��߂̃J����</summary>
    protected CinemachineVirtualCamera _clearCamera = null;

    /// <summary>���ɔ�ԃV�[����</summary>
    protected string _nextSceneName = "EntranceStage";


    /// <summary>�e�펞�Ԑ���p�^�C�}�[</summary>
    protected float _timer = 0f;


    /// <summary>�f���J�������f���Ă��鎞�ɒ�������Ώ�</summary>
    protected Transform _demoCameraTarget = null;

    /// <summary>�f���J�������f���Ă��鎞�ɏ�����ɂȂ�x�N�g��</summary>
    protected Vector3 _demoCameraUp = Vector3.up;



    /// <summary>True : ���������������{����</summary>
    protected bool _doInit = true;


    /// <summary>�X�e�[�W���펞�̏��</summary>
    public StateOnStage State { get => _state; set { _doInit = true; _state = value; } }

    /// <summary>�f���J�������f���Ă��鎞�ɒ�������Ώ�</summary>
    public Transform DemoCameraTarget { get => _demoCameraTarget; set => _demoCameraTarget = value; }


    /// <summary>�X�e�[�W�J�n�f�����ɃL�����N�^�[��Y�킹��DollyCart</summary>
    public CinemachineDollyCart IntroAnimationCart => _introDollyCart;

    /// <summary>�X�e�[�W�N���A���ɃL�����N�^�[��Y�킹��DollyCart</summary>
    public CinemachineDollyCart ClearAnimationCart => _clearDollyCart;


    /// <summary>���݂̃X�e�[�W�̃}�l�[�W���[</summary>
    static public StageManagerBase Current => _Current;



    void Awake()
    {
        _Current = this;
    }

    void Start()
    {
        //TODO
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
        {
            _CurrentCharacter = (CharacterKind)Random.Range(0, 2);
        }

        //�L�����N�^�[�z�u
        var prefabs = CharacterPrefabManager.Instance.Get(_CurrentCharacter);
        if (!_characterForControl) _characterForControl = Instantiate(prefabs.forPlaying);
        if (!_characterForCourseOut) _characterForCourseOut = Instantiate(prefabs.forUseRigidBody);
        if (!_characterForDemo) _characterForDemo = Instantiate(prefabs.forNotUseRigidBody);
        _characterForControl.transform.position = _introDollyCart.m_Path.transform.position;
        _characterForControl.transform.forward = _characterFrontReference.transform.forward;

        _characterForDemoAnim = _characterForDemo.GetComponentInChildren<AnimatorForDemoMovie>();

        //�L�����N�^�[�񊈐���
        _characterForControl.SetActive(false);
        _characterForCourseOut.SetActive(false);
        _characterForDemo.SetActive(false);

        //�J�����񊈐���
        _introCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(false);
        _missCamera.gameObject.SetActive(false);
        //_revivalCamera.gameObject.SetActive(false);
        _clearAnimationCamera.gameObject.SetActive(false);

        //�h�[���[�J�[�g�񊈐���
        _introDollyCart.gameObject.SetActive(false);
        _introCameraDollyCart.gameObject.SetActive(false);
        _clearDollyCart.gameObject.SetActive(false);

        //�J�����`��D��x�X�V
        _introCamera.Priority = _BASE_CAMERA_PRIORITY;
        _mainCamera.Priority = _BASE_CAMERA_PRIORITY + 1;
        _missCamera.Priority = _BASE_CAMERA_PRIORITY;
        //_revivalCamera.Priority = _BASE_CAMERA_PRIORITY;
    }

    void OnDestroy()
    {
        _Current = null;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case StateOnStage.IntroduceDemo:

                if (_doInit)
                {
                    _characterForCourseOut.SetActive(false);
                    _characterForDemo.SetActive(true);
                    _characterForControl.SetActive(false);

                    _mainCamera.gameObject.SetActive(false);
                    _introCamera.Priority = _mainCamera.Priority + 1;
                    _introCamera.gameObject.SetActive(true);
                    _introDollyCart.gameObject.SetActive(true);
                    _introCameraDollyCart.gameObject.SetActive(true);
                    _introDollyCart.m_Position = 0f;
                    _characterForDemo.transform.parent = _introDollyCart.transform;
                    _characterForDemo.transform.localPosition = Vector3.zero;
                    _characterForDemo.transform.localRotation = Quaternion.identity;

                    _demoCameraTarget = _characterForDemoAnim.EyePoint;
                    _demoCameraUp = _introDollyCart.transform.parent.up;

                    _doInit = false;
                }

                //�f���̃X�L�b�v
                if (InputUtility.GetDownDecide)
                {
                    _introDollyCart.m_Position = _introDollyCart.m_Path.PathLength;
                    _state = StateOnStage.Playing;
                }

                _introCamera.transform.LookAt(_demoCameraTarget.position, _demoCameraUp);

                break;

            case StateOnStage.Playing:

                if (_doInit)
                {
                    _characterForCourseOut.SetActive(false);
                    _characterForDemo.SetActive(false);
                    _characterForControl.SetActive(true);

                    _mainCamera.gameObject.SetActive(true);
                    _mainCamera.Priority = _introCamera.Priority + 1;
                    _introCamera.gameObject.SetActive(false);
                    _introDollyCart.gameObject.SetActive(false);
                    _introCameraDollyCart.gameObject.SetActive(false);

                    _doInit = false;
                }

                //�|�[�Y�v��
                if (InputUtility.GetDownPause)
                {
                    if (PauseManager.Instance.IsPause)
                    {
                        PauseManager.Instance.DoPause(false);
                    }
                    else
                    {
                        PauseManager.Instance.DoPause(true);
                    }
                }

                break;

            case StateOnStage.ClearDemo:

                if (_doInit)
                {
                    _characterForCourseOut.SetActive(false);
                    _characterForDemo.SetActive(true);
                    _characterForControl.SetActive(false);

                    _clearCamera.gameObject.SetActive(true);
                    _clearCamera.Priority = _mainCamera.Priority + 1;
                    _clearDollyCart.gameObject.SetActive(true);
                    _characterForDemo.transform.parent = _clearDollyCart.transform;
                    _characterForDemo.transform.localPosition = Vector3.zero;
                    _characterForDemo.transform.localRotation = Quaternion.identity;
                    _mainCamera.gameObject.SetActive(false);
                    _demoCameraTarget = _characterForDemo.transform;
                    _demoCameraUp = _mainCamera.transform.parent.up;

                    _characterForDemoAnim.SwitchToJumpToGoalGate();

                    _doInit = false;
                }

                _clearCamera.transform.LookAt(_demoCameraTarget.position, _demoCameraUp);

                break;

            case StateOnStage.MissDemo:

                if (_doInit)
                {
                    _characterForCourseOut.SetActive(true);
                    _characterForDemo.SetActive(false);

                    //���x�ƌ������ێ�
                    _characterForCourseOut.transform.position = _characterForControl.transform.position;
                    _characterForCourseOut.transform.rotation = _characterForControl.transform.rotation;
                    Rigidbody rbCourseOut = _characterForCourseOut.GetComponent<Rigidbody>();
                    Rigidbody rbControl = _characterForControl.GetComponent<Rigidbody>();
                    rbCourseOut.velocity = rbControl.velocity * 0.8f;
                    rbCourseOut.AddTorque(_characterForControl.transform.right * 2f, ForceMode.VelocityChange);

                    _characterForControl.SetActive(false);

                    _missCamera.transform.position = _mainCamera.transform.position;
                    _missCamera.gameObject.SetActive(true);
                    _missCamera.Priority = _mainCamera.Priority + 1;
                    _mainCamera.gameObject.SetActive(false);
                    _demoCameraTarget = _characterForCourseOut.transform;
                    _demoCameraUp = _mainCamera.transform.parent.up;

                    SceneChangeManager.Instance.BlackoutWithCycleAndChangeTo();

                    _doInit = false;
                }

                _missCamera.transform.LookAt(_demoCameraTarget.position, _demoCameraUp);

                break;

            case StateOnStage.MissedIntroduce:

                if (_doInit)
                {
                    /*
                    _CharacterForCourseOut.SetActive(true);
                    _CharacterForDemo.SetActive(false);
                    _CharacterForControl.SetActive(false);
                    */

                    _state = StateOnStage.Playing;
                    _doInit = false;
                }

                break;

            default: break;

        }
    }

    /// <summary>�����f������X�e�[�W���풆�ɕύX</summary>
    public void IntroduceToPlaying()
    {
        _characterForDemo.transform.parent = null;
        _characterForDemo.transform.position = _characterForControl.transform.position;
        State = StateOnStage.Playing;
    }

    /// <summary>�X�e�[�W�N���A�f�����烁�j���[�ɕύX</summary>
    public void ClearDemoToMenu()
    {
        State = StateOnStage.ClearMenu;
    }

    /// <summary>�~�X�f�����烁�j���[�ɕύX</summary>
    public void MissDemoToMenu()
    {
        State = StateOnStage.MissMenu;
    }

    /// <summary>�X�e�[�W���풆����X�e�[�W�N���A�f���ɕύX</summary>
    public void PlayingToClearDemo(CinemachineSmoothPath mainTrack, CinemachineVirtualCamera fixedCamera, string nextScene)
    {
        _clearDollyCart.m_Path = mainTrack;
        _clearCamera = fixedCamera;
        _nextSceneName = nextScene;
        State = StateOnStage.ClearDemo;
    }

    /// <summary>�����f�������n</summary>
    public void IntroLanding()
    {
        _characterForDemoAnim.SwitchToLandingBeforeGlide();
    }

    /// <summary>�X�e�[�W�N���A�f���r���̃J�����ύX</summary>
    public void ClearDemoSwitchCamera()
    {
        _clearAnimationCamera.Priority = _clearCamera.Priority + 1;
        _clearAnimationCamera.gameObject.SetActive(true);
        _clearCamera.gameObject.SetActive(false);
    }

    /// <summary>�o�^���ꂽ�V�[���ɔ��</summary>
    public void SceneChange()
    {
        SceneChangeManager.Instance.WhiteoutWithFogAndChangeTo(_nextSceneName);
    }
}

/// <summary>�X�e�[�W���펞�̏��</summary>
public enum StateOnStage : byte
{
    /// <summary>�X�e�[�W���풆</summary>
    Playing = 0,
    /// <summary>����O�����f��</summary>
    IntroduceDemo = 1,
    /// <summary>�X�e�[�W�N���A���ɃA�N�Z���Q�[�g�Ɍ������܂ł̃A�j���[�V����</summary>
    ClearDemo = 2,
    /// <summary>�X�e�[�W�N���A�チ�j���[</summary>
    ClearMenu = 5,
    /// <summary>�~�X���f��</summary>
    MissDemo = 10,
    /// <summary>�~�X�����j���[</summary>
    MissMenu = 11,
    /// <summary>�~�X�㓱���f��</summary>
    MissedIntroduce = 12,

    /// <summary>�^�C�g��</summary>
    Title = 30,


}
