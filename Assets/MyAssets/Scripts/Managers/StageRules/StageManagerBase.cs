using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManagerBase : MonoBehaviour
{
    /// <summary>バーチャルカメラの標準Priority</summary>
    const byte _BASE_CAMERA_PRIORITY = 20;

    /// <summary>現在のステージのマネージャー</summary>
    static StageManagerBase _Current = null;

    /// <summary>現在の操作キャラクター</summary>
    static CharacterKind _CurrentCharacter = CharacterKind.Emiri;

    /// <summary>デモムービー表示用キャラクター</summary>
    static GameObject _CharacterForDemo = null;

    /// <summary>コースアウト時表示用キャラクター</summary>
    static GameObject _CharacterForCourseOut = null;

    /// <summary>操作キャラクター</summary>
    static GameObject _CharacterForControl = null;


    [SerializeField, Tooltip("ステージ挑戦時の状態")]
    StateOnStage _state = StateOnStage.Playing;


    [Header("ステージ開始時のデモ用部品")]
    [SerializeField, Tooltip("ステージ開始時のデモを流すカメラ")]
    CinemachineVirtualCamera _introCamera = null;

    [SerializeField, Tooltip("ステージ開始時のデモを流すカメラを添わせるDollyCart")]
    CinemachineDollyCart _introCameraDollyCart = null;

    [SerializeField, Tooltip("ステージ開始デモ時にキャラクターを添わせるDollyCart")]
    CinemachineDollyCart _introDollyCart = null;


    [Header("ステージ挑戦中の部品")]
    [SerializeField, Tooltip("キャラクターや地形等、実態のオブジェクトを表示するカメラ")]
    CinemachineVirtualCamera _mainCamera = null;


    [Header("ステージコースアウトのデモ用部品")]
    [SerializeField, Tooltip("ステージコースアウトなど、失敗時に利用するカメラ")]
    CinemachineVirtualCamera _missCamera = null;

    [SerializeField, Tooltip("コースアウト後、いつメニューに移行するかを指定")]
    float _missMenuSwitchTime = 3f;


    [Header("ステージクリア時のデモ用部品")]
    [SerializeField, Tooltip("ステージクリア時のデモを流すはじめのカメラ")]
    CinemachineVirtualCamera _clearCamera = null;

    [SerializeField, Tooltip("ステージクリア時のデモを流す次のカメラ")]
    CinemachineVirtualCamera _clearAnimationCamera = null;

    [SerializeField, Tooltip("ステージクリア時にキャラクターを添わせるDollyCart")]
    CinemachineDollyCart _clearDollyCart = null;


    /// <summary>各種時間制御用タイマー</summary>
    float _timer = 0f;


    /// <summary>デモカメラを映している時に注視する対象</summary>
    Transform _demoCameraTarget = null;

    /// <summary>デモカメラを映している時に上向きになるベクトル</summary>
    Vector3 _demoCameraUp = Vector3.up;




    
    /// <summary>True : 初期化処理を実施する</summary>
    bool _doInit = true;


    /// <summary>ステージ挑戦時の状態</summary>
    public StateOnStage State { get => _state; set { _doInit = true; _state = value; } }

    /// <summary>デモカメラを映している時に注視する対象</summary>
    public Transform DemoCameraTarget { get => _demoCameraTarget; set => _demoCameraTarget = value; }


    /// <summary>ステージ開始デモ時にキャラクターを添わせるDollyCart</summary>
    public CinemachineDollyCart IntroAnimationCart => _introDollyCart;

    /// <summary>ステージクリア時にキャラクターを添わせるDollyCart</summary>
    public CinemachineDollyCart ClearAnimationCart => _clearDollyCart;


    /// <summary>現在のステージのマネージャー</summary>
    static public StageManagerBase Current => _Current;

    /// <summary>デモムービー表示用キャラクター</summary>
    static public GameObject CharacterForDemo { get => _CharacterForDemo; set => _CharacterForDemo = value; }



    void Awake()
    {
        _Current = this;
    }

    void Start()
    {
        //キャラクター配置
        var prefabs = CharacterPrefabManager.Instance.Get(_CurrentCharacter);
        if (!_CharacterForControl) _CharacterForControl = Instantiate(prefabs.forPlaying);
        if (!_CharacterForCourseOut) _CharacterForCourseOut = Instantiate(prefabs.forUseRigidBody);
        if (!_CharacterForDemo) _CharacterForDemo = Instantiate(prefabs.forNotUseRigidBody);
        _CharacterForControl.transform.position = _introDollyCart.m_Path.transform.position;

        //キャラクター非活性化
        _CharacterForControl.SetActive(false);
        _CharacterForCourseOut.SetActive(false);
        _CharacterForDemo.SetActive(false);

        //カメラ非活性化
        _introCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(false);
        _missCamera.gameObject.SetActive(false);
        _clearCamera.gameObject.SetActive(false);
        _clearAnimationCamera.gameObject.SetActive(false);

        //ドーリーカート非活性化
        _introDollyCart.gameObject.SetActive(false);
        _introCameraDollyCart.gameObject.SetActive(false);
        _clearDollyCart.gameObject.SetActive(false);

        //カメラ描画優先度更新
        _introCamera.Priority = _BASE_CAMERA_PRIORITY;
        _mainCamera.Priority = _BASE_CAMERA_PRIORITY + 1;
        _missCamera.Priority = _BASE_CAMERA_PRIORITY;
        _clearCamera.Priority = _BASE_CAMERA_PRIORITY;
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
                    _CharacterForCourseOut.SetActive(false);
                    _CharacterForDemo.SetActive(true);
                    _CharacterForControl.SetActive(false);

                    _mainCamera.gameObject.SetActive(false);
                    _introCamera.Priority = _mainCamera.Priority + 1;
                    _introCamera.gameObject.SetActive(true);
                    _introDollyCart.gameObject.SetActive(true);
                    _introCameraDollyCart.gameObject.SetActive(true);
                    _introDollyCart.m_Position = 0f;
                    _CharacterForDemo.transform.parent = _introDollyCart.transform;
                    _CharacterForDemo.transform.localPosition = Vector3.zero;
                    _CharacterForDemo.transform.localRotation = Quaternion.identity;

                    _demoCameraTarget = _CharacterForDemo.transform;
                    _demoCameraUp = _introDollyCart.transform.parent.up;

                    _doInit = false;
                }

                //デモのスキップ
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
                    _CharacterForCourseOut.SetActive(false);
                    _CharacterForDemo.SetActive(false);
                    _CharacterForControl.SetActive(true);

                    _mainCamera.gameObject.SetActive(true);
                    _mainCamera.Priority = _introCamera.Priority + 1;
                    _introCamera.gameObject.SetActive(false);
                    _introDollyCart.gameObject.SetActive(false);
                    _introCameraDollyCart.gameObject.SetActive(false);

                    _doInit = false;
                }

                //ポーズ要求
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
                    _CharacterForCourseOut.SetActive(false);
                    _CharacterForDemo.SetActive(true);
                    _CharacterForControl.SetActive(false);

                    _clearCamera.gameObject.SetActive(true);
                    _clearCamera.Priority = _mainCamera.Priority + 1;
                    _clearDollyCart.gameObject.SetActive(true);
                    _CharacterForDemo.transform.parent = _clearDollyCart.transform;
                    _CharacterForDemo.transform.localPosition = Vector3.zero;
                    _CharacterForDemo.transform.localRotation = Quaternion.identity;
                    _mainCamera.gameObject.SetActive(false);
                    _demoCameraTarget = _CharacterForDemo.transform;
                    _demoCameraUp = _mainCamera.transform.parent.up;

                    _doInit = false;
                }

                _clearCamera.transform.LookAt(_demoCameraTarget.position, _demoCameraUp);

                break;

            case StateOnStage.MissDemo:

                if (_doInit)
                {
                    _CharacterForCourseOut.SetActive(true);
                    _CharacterForDemo.SetActive(false);

                    //速度と向きを維持
                    _CharacterForCourseOut.transform.position = _CharacterForControl.transform.position;
                    _CharacterForCourseOut.transform.rotation = _CharacterForControl.transform.rotation;
                    Rigidbody rbCourseOut = _CharacterForCourseOut.GetComponent<Rigidbody>();
                    Rigidbody rbControl = _CharacterForControl.GetComponent<Rigidbody>();
                    rbCourseOut.velocity = rbControl.velocity * 0.8f;
                    rbCourseOut.AddTorque(_CharacterForControl.transform.right * 2f, ForceMode.VelocityChange);

                    _CharacterForControl.SetActive(false);

                    _missCamera.transform.position = _mainCamera.transform.position;
                    _missCamera.gameObject.SetActive(true);
                    _missCamera.Priority = _mainCamera.Priority + 1;
                    _mainCamera.gameObject.SetActive(false);
                    _demoCameraTarget = _CharacterForCourseOut.transform;
                    _demoCameraUp = _mainCamera.transform.parent.up;

                    _timer = _missMenuSwitchTime;

                    _doInit = false;
                }

                _missCamera.transform.LookAt(_demoCameraTarget.position, _demoCameraUp);

                //指定時間後にシーンを再ロード
                if(_timer < 0f)
                {
                    _timer = 0f;
                    SceneChangeManager.Instance.LoadSelf();
                }
                else
                {
                    _timer -= Time.deltaTime;
                }

                break;

            default: break;

        }
    }

    /// <summary>導入デモからステージ挑戦中に変更</summary>
    public void IntroduceToPlaying()
    {
        State = StateOnStage.Playing;
    }

    /// <summary>ステージクリアデモからメニューに変更</summary>
    public void ClearDemoToMenu()
    {
        State = StateOnStage.ClearMenu;
    }

    /// <summary>ミスデモからメニューに変更</summary>
    public void MissDemoToMenu()
    {
        State = StateOnStage.MissMenu;
    }
}

/// <summary>ステージ挑戦時の状態</summary>
public enum StateOnStage : byte
{
    /// <summary>ステージ挑戦中</summary>
    Playing = 0,
    /// <summary>挑戦前導入デモ</summary>
    IntroduceDemo = 1,
    /// <summary>ステージクリア時にアクセルゲートに向かうまでのアニメーション</summary>
    ClearDemo = 2,
    /// <summary>ステージクリア後メニュー</summary>
    ClearMenu = 5,
    /// <summary>ミス時デモ</summary>
    MissDemo = 10,
    /// <summary>ミス時メニュー</summary>
    MissMenu = 11,

    /// <summary>タイトル</summary>
    Title = 100,
}
