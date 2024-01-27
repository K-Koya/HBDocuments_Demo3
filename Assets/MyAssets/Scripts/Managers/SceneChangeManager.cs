using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    /// <summary>フェード用コルーチン</summary>
    Coroutine _fadeCoroutine = null;

    /// <summary>開始1秒待機</summary>
    WaitForSeconds _delay1Sec = new WaitForSeconds(1f);

    /// <summary>フェード開始3秒後にシーン遷移</summary>
    WaitForSeconds _delay3Sec = new WaitForSeconds(3f);

    /// <summary>フェードイン・アウトの方式</summary>
    public enum FadeType : byte
    {
        /// <summary>使わない</summary>
        None = 0,
        /// <summary>雲を巻かせつつ画面を白くする</summary>
        WhiteoutWithFog,
        /// <summary>円状の窓を小さくしつつ画面を黒くする</summary>
        BlackoutWithCycle,
    }
    /// <summary>フェードイン・アウトの方式</summary>
    static FadeType _fadeType = FadeType.None;


    [SerializeField, Tooltip("ホワイトアウト用アニメーター")]
    Animator _WhiteoutAnim = null;

    [SerializeField, Tooltip("雲のような、霧のような煙を発生")]
    ParticleSystem _fog = null;

    /// <summary>ホワイトアウト要求</summary>
    string _doWhiteout = "DoWhiteout";

    /// <summary>ホワイトアウト継続</summary>
    string _onWhite = "OnWhite";

    /// <summary>ホワイトアウト解除</summary>
    string _returnWhiteout = "ReturnWhiteout";


    [SerializeField, Tooltip("ブラックアウト用アニメーター")]
    Animator _BlackoutAnim = null;

    /// <summary>ブラックアウト要求</summary>
    string _doBlackout = "DoBlackout";

    /// <summary>ブラックアウト継続</summary>
    string _onBlack = "OnBlack";

    /// <summary>ブラックアウト解除</summary>
    string _returnBlackout = "ReturnBlackout";


    void Start()
    {
        IsDontDestroyOnLoad = false;
        _WhiteoutAnim.enabled = false;
        _BlackoutAnim.enabled = false;
        _fadeCoroutine = StartCoroutine(Coroutine_SceneInit());
    }

    /// <summary>nameのシーンへ遷移</summary>
    public void ChangeTo(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>現在のシーンを再読み込みする</summary>
    public void LoadSelf()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>雲を巻かせつつ画面を白くするフェードでnameのシーンへ遷移</summary>
    public void WhiteoutWithFogAndChangeTo(string name)
    {
        _fadeType = FadeType.WhiteoutWithFog;
        _fadeCoroutine = StartCoroutine(Coroutine_WhiteoutWithFogAndChangeTo(name));
    }

    /// <summary>WhiteoutWithFogAndChangeToを実施するコルーチン</summary>
    IEnumerator Coroutine_WhiteoutWithFogAndChangeTo(string name)
    {
        _fog.Play();
        _WhiteoutAnim.enabled = true;
        _WhiteoutAnim.Play(_doWhiteout);
        yield return _delay3Sec;
        _fog.Stop(false);
        ChangeTo(name);
    }

    /// <summary>シーン開始でフェードから復帰するコルーチン</summary>
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

    /// <summary>円形にブラックアウトで同じシーンをロード</summary>
    public void BlackoutWithCycleAndChangeTo()
    {
        _fadeType = FadeType.BlackoutWithCycle;
        _fadeCoroutine = StartCoroutine(Coroutine_BlackoutWithCycleAndChangeTo());
    }

    /// <summary>円形にブラックアウトでnameのシーンへ遷移</summary>
    public void BlackoutWithCycleAndChangeTo(string name)
    {
        _fadeType = FadeType.BlackoutWithCycle;
        _fadeCoroutine = StartCoroutine(Coroutine_BlackoutWithCycleAndChangeTo(name));
    }

    /// <summary>BlackoutWithCycleAndChangeToを実施するコルーチン</summary>
    IEnumerator Coroutine_BlackoutWithCycleAndChangeTo()
    {
        yield return _delay1Sec;
        _BlackoutAnim.enabled = true;
        _BlackoutAnim.Play(_doBlackout);
        yield return _delay1Sec;
        LoadSelf();
    }

    /// <summary>BlackoutWithCycleAndChangeToを実施するコルーチン</summary>
    IEnumerator Coroutine_BlackoutWithCycleAndChangeTo(string name)
    {
        _BlackoutAnim.enabled = true;
        _BlackoutAnim.Play(_doBlackout);
        yield return _delay1Sec;
        ChangeTo(name);
    }

    /// <summary>ゲームを終了する</summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
