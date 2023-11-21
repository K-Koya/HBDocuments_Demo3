using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrefabManager : Singleton<CharacterPrefabManager>
{
    /// <summary>該当キャラクター向けのプレハブ集</summary>
    [System.Serializable]
    public struct Prefabs
    {
        [SerializeField, Tooltip("キャラクター名")]
        string name;

        [SerializeField, Tooltip("操作キャラクター用")]
        public GameObject forPlaying;

        [SerializeField, Tooltip("RigidBodyを使うデモ用キャラクター")]
        public GameObject forUseRigidBody;

        [SerializeField, Tooltip("RigidBodyを使わないデモ用キャラクター")]
        public GameObject forNotUseRigidBody;
    }
    [SerializeField, Tooltip("各キャラクターのプレハブ集")]
    Prefabs[] _prefabs = null;

    /// <summary>プレハブ一式取得</summary>
    /// <param name="kind">キャラクター種別</param>
    /// <returns>プレハブ一式</returns>
    public Prefabs Get(CharacterKind kind)
    {
        return _prefabs[(int)kind];
    }
}

/// <summary>キャラクター種別</summary>
public enum CharacterKind : byte
{
    /// <summary>ジュンイチ</summary>
    Junichi = 0,
    /// <summary>エミリ</summary>
    Emiri = 1,
}
