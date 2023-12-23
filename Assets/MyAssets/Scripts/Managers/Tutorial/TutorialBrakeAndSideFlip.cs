using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class TutorialBrakeAndSideFlip : MonoBehaviour
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

        _ui.AppearCharacterBrake(false);
        _ui.AppearCharacterSideFlip(false);
        _ui.AppearOKIcon(false);
    }

    /// <summary>チュートリアル表示をするコルーチン</summary>
    IEnumerator Enumerator()
    {
        _character = GameObject.FindWithTag(TagManager.Instance.Player).GetComponent<CharacterParameter>();
        _ui.CloseUI();
        _ui.AppearCharacterBrake(true);
        _isCleared = false;

        while (!_isCleared)
        {
            yield return null;
            if (_character.IsBrake)
            {
                _isCleared = true;
            }
        }

        //クリアアイコン表示
        _ui.AppearOKIcon(true);

        yield return new WaitForSeconds(0.5f);

        //チュートリアル表示切り替え
        _ui.CloseUI();
        _ui.AppearCharacterSideFlip(true);
        _isCleared = false;

        while (!_isCleared)
        {
            yield return null;
            if (_character.DoSideFlip)
            {
                _character.DoSideFlip = true;
                _isCleared = true;
            }
        }

        //クリアアイコン表示
        _ui.AppearOKIcon(true);
        _character = null;

        yield return new WaitForSeconds(_okAppeartime);

        //チュートリアルを閉じる
        _ui.AppearOKIcon(false);
        _ui.AppearCharacterSideFlip(false);
    }
}
