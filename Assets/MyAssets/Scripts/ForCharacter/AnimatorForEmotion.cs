using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorForEmotion : MonoBehaviour
{
    /// <summary>�x�[�X�ɂȂ�u���̎��ԊԊu</summary>
    const float _BASE_BLINK_TIME = 7f;

    /// <summary>�Y���A�j���[�^�[</summary>
    Animator _anim = null;

    [SerializeField, Tooltip("�p�����[�^��:DoBlink")]
    string _paramNameDoBlink = "DoBlink";

    /// <summary>�u������</summary>
    float _blinkTimer = 0f;





    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _blinkTimer = Random.Range(0.5f, _BASE_BLINK_TIME);
    }

    // Update is called once per frame
    void Update()
    {
        //�u���Ǘ�
        _blinkTimer -= Time.deltaTime;
        if (_blinkTimer < 0f)
        {
            _anim.SetTrigger(_paramNameDoBlink);
            _blinkTimer = Random.Range(0.5f, _BASE_BLINK_TIME);
        }
    }
}
