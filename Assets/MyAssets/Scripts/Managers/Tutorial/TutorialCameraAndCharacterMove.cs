using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class TutorialCameraAndCharacterMove : MonoBehaviour
{
    [SerializeField, Tooltip("表示するUI")]
    TutorialUI _ui = null;

    [SerializeField, Tooltip("チュートリアル完了に必要な入力時間")]
    float _quotaTime = 3f;

    /// <summary>入力時間カウンター</summary>
    float _timer = 0f;

    /// <summary>チュートリアルクリアのアイコンを表示し続ける時間</summary>
    float _okAppeartime = 2f;

    /// <summary>True : このチュートリアルをクリア</summary>
    bool _isCleared = false;

    /// <summary>操作カメラ情報</summary>
    ICameraForTutorial _camera = null;

    /// <summary>操作キャラクター情報</summary>
    ICharacterParameterForTutorial _character = null;

    /// <summary>チュートリアル表示をするコルーチン</summary>
    Coroutine _appearSequence = null;



    private void OnTriggerEnter(Collider other)
    {
        _appearSequence = StartCoroutine(Enumerator());
    }


    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(_appearSequence);
        _appearSequence = null;

        _ui.AppearCameraControl(false);
        _ui.AppearCharacterMove(false);
        _ui.AppearOKIcon(false);
    }

    /// <summary>チュートリアル表示をするコルーチン</summary>
    IEnumerator Enumerator()
    {
        _camera = FindFirstObjectByType<ManualCamera>();
        _ui.AppearCameraControl(true);
        _isCleared = false;
        _timer = _quotaTime;

        //カメラチュートリアル
        while (!_isCleared)
        {
            yield return null;

            //入力あり
            if (_camera.DeltaVAngle > 0f || _camera.DeltaHAngle > 0f)
            {
                if (_timer > 0f)
                {
                    _timer -= Time.deltaTime;
                }
                //クリア
                else
                {
                    _isCleared = true;
                    _timer = 0f;
                }
            }
        }

        //クリアアイコン表示
        _ui.AppearOKIcon(true);
        _camera = null;
        
        yield return new WaitForSeconds(0.5f);

        //チュートリアル表示切り替え
        _character = GameObject.FindWithTag(TagManager.Instance.Player).GetComponent<CharacterParameter>();
        _ui.CloseUI();
        _ui.AppearCharacterMove(true);
        _isCleared = false;
        _timer = _quotaTime;

        //移動チュートリアル
        while (!_isCleared)
        {
            yield return null;

            //入力あり
            if (_character.ResultSpeed > 0.5f)
            {
                if (_timer > 0f)
                {
                    _timer -= Time.deltaTime;
                }
                //クリア
                else
                {
                    _isCleared = true;
                    _timer = 0f;
                }
            }
        }

        //クリアアイコン表示
        _ui.AppearOKIcon(true);
        _character = null;

        yield return new WaitForSeconds(_okAppeartime);

        //チュートリアルを閉じる
        _ui.AppearOKIcon(false);
        _ui.AppearCharacterMove(false);
    }
}
