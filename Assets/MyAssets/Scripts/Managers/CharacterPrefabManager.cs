using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPrefabManager : Singleton<CharacterPrefabManager>
{
    /// <summary>�Y���L�����N�^�[�����̃v���n�u�W</summary>
    [System.Serializable]
    public struct Prefabs
    {
        [SerializeField, Tooltip("�L�����N�^�[��")]
        string name;

        [SerializeField, Tooltip("����L�����N�^�[�p")]
        public GameObject forPlaying;

        [SerializeField, Tooltip("RigidBody���g���f���p�L�����N�^�[")]
        public GameObject forUseRigidBody;

        [SerializeField, Tooltip("RigidBody���g��Ȃ��f���p�L�����N�^�[")]
        public GameObject forNotUseRigidBody;
    }
    [SerializeField, Tooltip("�e�L�����N�^�[�̃v���n�u�W")]
    Prefabs[] _prefabs = null;

    /// <summary>�v���n�u�ꎮ�擾</summary>
    /// <param name="kind">�L�����N�^�[���</param>
    /// <returns>�v���n�u�ꎮ</returns>
    public Prefabs Get(CharacterKind kind)
    {
        return _prefabs[(int)kind];
    }
}

/// <summary>�L�����N�^�[���</summary>
public enum CharacterKind : byte
{
    /// <summary>�W�����C�`</summary>
    Junichi = 0,
    /// <summary>�G�~��</summary>
    Emiri = 1,
}
