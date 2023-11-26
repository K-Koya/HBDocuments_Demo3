using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorForEmotion : MonoBehaviour
{
    /// <summary>ベースになる瞬きの時間間隔</summary>
    const float _BASE_BLINK_TIME = 7f;

    /// <summary>該当アニメーター</summary>
    Animator _anim = null;

    [SerializeField, Tooltip("パラメータ名:DoBlink")]
    string _paramNameDoBlink = "DoBlink";

    /// <summary>瞬き時間</summary>
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
        //瞬き管理
        _blinkTimer -= Time.deltaTime;
        if (_blinkTimer < 0f)
        {
            _anim.SetTrigger(_paramNameDoBlink);
            _blinkTimer = Random.Range(0.5f, _BASE_BLINK_TIME);
        }
    }
}
