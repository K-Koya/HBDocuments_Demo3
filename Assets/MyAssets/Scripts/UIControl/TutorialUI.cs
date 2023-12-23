using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField, Tooltip("OK表示するアイコン")]
    GameObject _okIcon = null;

    [SerializeField, Tooltip("カメラ回転操作")]
    GameObject _forCameraControl = null;

    [SerializeField, Tooltip("キャラクター平面移動操作")]
    GameObject _forCharacterMove = null;

    [SerializeField, Tooltip("キャラクター柵・段差乗り越え操作")]
    GameObject _forCharacterThroughOver = null;

    [SerializeField, Tooltip("キャラクタージャンプ操作")]
    GameObject _forCharacterJump = null;

    [SerializeField, Tooltip("キャラクターブレーキ操作")]
    GameObject _forCharacterBrake = null;

    [SerializeField, Tooltip("キャラクター横宙返り操作")]
    GameObject _forCharacterSideFlip = null;


    // Start is called before the first frame update
    void Start()
    {
        CloseUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>操作成功表示</summary>
    public void AppearOKIcon(bool isAppear)
    {
        _okIcon.SetActive(isAppear);
    }

    /// <summary>チュートリアル全非表示</summary>
    public void CloseUI()
    {
        _okIcon.SetActive(false);
        _forCameraControl.SetActive(false);
        _forCharacterMove.SetActive(false);
        _forCharacterThroughOver.SetActive(false);
        _forCharacterJump.SetActive(false);
        _forCharacterBrake.SetActive(false);
        _forCharacterSideFlip.SetActive(false);
    }

    /// <summary>カメラ回転操作説明を表示</summary>
    public void AppearCameraControl(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCameraControl.SetActive(isAppear);
    }

    /// <summary>キャラクター柵・段差乗り越え操作説明を表示</summary>
    public void AppearCharacterMove(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterMove.SetActive(isAppear);
    }

    /// <summary>キャラクター平面移動操作説明を表示</summary>
    public void AppearCharacterThroughOver(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterThroughOver.SetActive(isAppear);
    }

    /// <summary>キャラクタージャンプ操作説明を表示</summary>
    public void AppearCharacterJump(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterJump.SetActive(isAppear);
    }

    /// <summary>キャラクターブレーキ操作説明を表示</summary>
    public void AppearCharacterBrake(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterBrake.SetActive(isAppear);
    }

    /// <summary>キャラクター横宙返り操作説明を表示</summary>
    public void AppearCharacterSideFlip(bool isAppear)
    {
        if (isAppear) CloseUI();
        _forCharacterSideFlip.SetActive(isAppear);
    }
}
