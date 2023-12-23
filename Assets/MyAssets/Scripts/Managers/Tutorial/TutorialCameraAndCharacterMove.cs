using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class TutorialCameraAndCharacterMove : MonoBehaviour
{
    [SerializeField, Tooltip("�\������UI")]
    TutorialUI _ui = null;

    [SerializeField, Tooltip("�`���[�g���A�������ɕK�v�ȓ��͎���")]
    float _quotaTime = 3f;

    /// <summary>���͎��ԃJ�E���^�[</summary>
    float _timer = 0f;

    /// <summary>�`���[�g���A���N���A�̃A�C�R����\���������鎞��</summary>
    float _okAppeartime = 2f;

    /// <summary>True : ���̃`���[�g���A�����N���A</summary>
    bool _isCleared = false;

    /// <summary>����J�������</summary>
    ICameraForTutorial _camera = null;

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

        _ui.AppearCameraControl(false);
        _ui.AppearCharacterMove(false);
        _ui.AppearOKIcon(false);
    }

    /// <summary>�`���[�g���A���\��������R���[�`��</summary>
    IEnumerator Enumerator()
    {
        _camera = FindFirstObjectByType<ManualCamera>();
        _ui.AppearCameraControl(true);
        _isCleared = false;
        _timer = _quotaTime;

        //�J�����`���[�g���A��
        while (!_isCleared)
        {
            yield return null;

            //���͂���
            if (_camera.DeltaVAngle > 0f || _camera.DeltaHAngle > 0f)
            {
                if (_timer > 0f)
                {
                    _timer -= Time.deltaTime;
                }
                //�N���A
                else
                {
                    _isCleared = true;
                    _timer = 0f;
                }
            }
        }

        //�N���A�A�C�R���\��
        _ui.AppearOKIcon(true);
        _camera = null;
        
        yield return new WaitForSeconds(0.5f);

        //�`���[�g���A���\���؂�ւ�
        _character = GameObject.FindWithTag(TagManager.Instance.Player).GetComponent<CharacterParameter>();
        _ui.CloseUI();
        _ui.AppearCharacterMove(true);
        _isCleared = false;
        _timer = _quotaTime;

        //�ړ��`���[�g���A��
        while (!_isCleared)
        {
            yield return null;

            //���͂���
            if (_character.ResultSpeed > 0.5f)
            {
                if (_timer > 0f)
                {
                    _timer -= Time.deltaTime;
                }
                //�N���A
                else
                {
                    _isCleared = true;
                    _timer = 0f;
                }
            }
        }

        //�N���A�A�C�R���\��
        _ui.AppearOKIcon(true);
        _character = null;

        yield return new WaitForSeconds(_okAppeartime);

        //�`���[�g���A�������
        _ui.AppearOKIcon(false);
        _ui.AppearCharacterMove(false);
    }
}
