using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialThroughOver : MonoBehaviour
{
    [SerializeField, Tooltip("表示するUI")]
    TutorialUI _ui = null;

    [SerializeField, Tooltip("チュートリアル完了に必要な入力時間")]
    float _quotaTime = 3f;

    /// <summary>チュートリアルクリアのアイコンを表示し続ける時間</summary>
    float _okAppeartime = 2f;

    /// <summary>True : このチュートリアルをクリア</summary>
    bool _isCleared = false;

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

        _ui.AppearCharacterThroughOver(false);
        _ui.AppearOKIcon(false);
    }

    /// <summary>チュートリアル表示をするコルーチン</summary>
    IEnumerator Enumerator()
    {
        _character = GameObject.FindWithTag(TagManager.Instance.Player).GetComponent<CharacterParameter>();
        _ui.CloseUI();
        _ui.AppearCharacterThroughOver(true);
        _isCleared = false;

        while (!_isCleared)
        {
            yield return null;
            if (_character.DoRunOver)
            {
                _character.DoRunOver = true;
                _isCleared = true;
            }
        }

        //クリアアイコン表示
        _ui.AppearOKIcon(true);
        _character = null;

        yield return new WaitForSeconds(_okAppeartime);

        //チュートリアルを閉じる
        _ui.AppearOKIcon(false);
        _ui.AppearCharacterThroughOver(false);
    }
}
