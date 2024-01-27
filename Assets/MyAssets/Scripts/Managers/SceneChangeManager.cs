using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    /// <summary>�t�F�[�h�p�R���[�`��</summary>
    Coroutine _fadeCoroutine = null;

    /// <summary>�J�n1�b�ҋ@</summary>
    WaitForSeconds _delay1Sec = new WaitForSeconds(1f);

    /// <summary>�t�F�[�h�J�n3�b��ɃV�[���J��</summary>
    WaitForSeconds _delay3Sec = new WaitForSeconds(3f);

    /// <summary>�t�F�[�h�C���E�A�E�g�̕���</summary>
    public enum FadeType : byte
    {
        /// <summary>�g��Ȃ�</summary>
        None = 0,
        /// <summary>�_����������ʂ𔒂�����</summary>
        WhiteoutWithFog,
        /// <summary>�~��̑�������������ʂ���������</summary>
        BlackoutWithCycle,
    }
    /// <summary>�t�F�[�h�C���E�A�E�g�̕���</summary>
    static FadeType _fadeType = FadeType.None;


    [SerializeField, Tooltip("�z���C�g�A�E�g�p�A�j���[�^�[")]
    Animator _WhiteoutAnim = null;

    [SerializeField, Tooltip("�_�̂悤�ȁA���̂悤�ȉ��𔭐�")]
    ParticleSystem _fog = null;

    /// <summary>�z���C�g�A�E�g�v��</summary>
    string _doWhiteout = "DoWhiteout";

    /// <summary>�z���C�g�A�E�g�p��</summary>
    string _onWhite = "OnWhite";

    /// <summary>�z���C�g�A�E�g����</summary>
    string _returnWhiteout = "ReturnWhiteout";


    [SerializeField, Tooltip("�u���b�N�A�E�g�p�A�j���[�^�[")]
    Animator _BlackoutAnim = null;

    /// <summary>�u���b�N�A�E�g�v��</summary>
    string _doBlackout = "DoBlackout";

    /// <summary>�u���b�N�A�E�g�p��</summary>
    string _onBlack = "OnBlack";

    /// <summary>�u���b�N�A�E�g����</summary>
    string _returnBlackout = "ReturnBlackout";


    void Start()
    {
        IsDontDestroyOnLoad = false;
        _WhiteoutAnim.enabled = false;
        _BlackoutAnim.enabled = false;
        _fadeCoroutine = StartCoroutine(Coroutine_SceneInit());
    }

    /// <summary>name�̃V�[���֑J��</summary>
    public void ChangeTo(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>���݂̃V�[�����ēǂݍ��݂���</summary>
    public void LoadSelf()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>�_����������ʂ𔒂�����t�F�[�h��name�̃V�[���֑J��</summary>
    public void WhiteoutWithFogAndChangeTo(string name)
    {
        _fadeType = FadeType.WhiteoutWithFog;
        _fadeCoroutine = StartCoroutine(Coroutine_WhiteoutWithFogAndChangeTo(name));
    }

    /// <summary>WhiteoutWithFogAndChangeTo�����{����R���[�`��</summary>
    IEnumerator Coroutine_WhiteoutWithFogAndChangeTo(string name)
    {
        _fog.Play();
        _WhiteoutAnim.enabled = true;
        _WhiteoutAnim.Play(_doWhiteout);
        yield return _delay3Sec;
        _fog.Stop(false);
        ChangeTo(name);
    }

    /// <summary>�V�[���J�n�Ńt�F�[�h���畜�A����R���[�`��</summary>
    IEnumerator Coroutine_SceneInit()
    {
        switch (_fadeType)
        {
            case FadeType.WhiteoutWithFog:

                _fog.Play();
                _WhiteoutAnim.enabled = true;
                _WhiteoutAnim.Play(_onWhite);

                yield return _delay1Sec;

                _fog.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                _WhiteoutAnim.Play(_returnWhiteout);

                break;
            case FadeType.BlackoutWithCycle:

                _BlackoutAnim.enabled = true;
                _BlackoutAnim.Play(_returnBlackout);
                break;

            default: break;
        }
    }

    /// <summary>�~�`�Ƀu���b�N�A�E�g�œ����V�[�������[�h</summary>
    public void BlackoutWithCycleAndChangeTo()
    {
        _fadeType = FadeType.BlackoutWithCycle;
        _fadeCoroutine = StartCoroutine(Coroutine_BlackoutWithCycleAndChangeTo());
    }

    /// <summary>�~�`�Ƀu���b�N�A�E�g��name�̃V�[���֑J��</summary>
    public void BlackoutWithCycleAndChangeTo(string name)
    {
        _fadeType = FadeType.BlackoutWithCycle;
        _fadeCoroutine = StartCoroutine(Coroutine_BlackoutWithCycleAndChangeTo(name));
    }

    /// <summary>BlackoutWithCycleAndChangeTo�����{����R���[�`��</summary>
    IEnumerator Coroutine_BlackoutWithCycleAndChangeTo()
    {
        yield return _delay1Sec;
        _BlackoutAnim.enabled = true;
        _BlackoutAnim.Play(_doBlackout);
        yield return _delay1Sec;
        LoadSelf();
    }

    /// <summary>BlackoutWithCycleAndChangeTo�����{����R���[�`��</summary>
    IEnumerator Coroutine_BlackoutWithCycleAndChangeTo(string name)
    {
        _BlackoutAnim.enabled = true;
        _BlackoutAnim.Play(_doBlackout);
        yield return _delay1Sec;
        ChangeTo(name);
    }

    /// <summary>�Q�[�����I������</summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
