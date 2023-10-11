using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : Singleton<TagManager>
{
    [Header("タグ名を以下にアサイン")]

    #region メンバ
    [SerializeField, Tooltip("メインカメラのタグ")]
    string _mainCamera = "MainCamera";
    [SerializeField, Tooltip("プレイヤーのタグ")]
    string _player = "Player";
    [SerializeField, Tooltip("敵のタグ")]
    string _enemy = "Enemy";
    [SerializeField, Tooltip("味方キャラクターのタグ")]
    string _allies = "Allies";
    [SerializeField, Tooltip("敵の出現地点タグ")]
    string _spawner = "Spawner";
    [SerializeField, Tooltip("床タグ")]
    string _floor = "Floor";
    #endregion

    #region プロパティ
    /// <summary>メインカメラのタグ</summary>
    public string MainCamera { get => _mainCamera; }
    /// <summary>プレイヤーのタグ</summary>
    public string Player { get => _player; }
    /// <summary>敵のタグ</summary>
    public string Enemy { get => _enemy; }
    /// <summary>味方キャラクターのタグ</summary>
    public string Allies { get => _allies; }
    /// <summary>攻撃情報を持つコライダーのタグ</summary>
    public string Spawner { get => _spawner; }
    /// <summary>床となるコライダーのタグ</summary>
    public string Floor { get => _floor; }
    #endregion

}

public static partial class ForRegisterExtensionMethods
{
    /// <summary>CompareTagの複数バージョン 一つでも一致すればtrueを返却</summary>
    /// <param name="mb">MonoBehaviourから呼び出し</param>
    /// <param name="tags">タグ名配列</param>
    /// <returns></returns>
    public static bool CompareTags(this MonoBehaviour mb, params string[] tags)
    {
        return tags.Count(tag => mb.CompareTag(tag)) > 0;
    }
} 
