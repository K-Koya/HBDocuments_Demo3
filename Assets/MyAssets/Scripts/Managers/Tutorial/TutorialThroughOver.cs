using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialThroughOver : MonoBehaviour
{
    [SerializeField, Tooltip("�\������UI")]
    TutorialUI _ui = null;

    [SerializeField, Tooltip("�`���[�g���A�������ɕK�v�ȓ��͎���")]
    float _quotaTime = 3f;

    /// <summary>�`���[�g���A���N���A�̃A�C�R����\���������鎞��</summary>
    float _okAppeartime = 2f;

    /// <summary>True : ���̃`���[�g���A�����N���A</summary>
    bool _isCleared = false;

    /// <summary>����L�����N�^�[���</summary>
    ICharacterParameterForTutorial _character = null;

    /// <summary>�`���[�g���A���\��������R���[�`��</summary>
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

    /// <summary>�`���[�g���A���\��������R���[�`��</summary>
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

        //�N���A�A�C�R���\��
        _ui.AppearOKIcon(true);
        _character = null;

        yield return new WaitForSeconds(_okAppeartime);

        //�`���[�g���A�������
        _ui.AppearOKIcon(false);
        _ui.AppearCharacterThroughOver(false);
    }
}
