using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    /// <summary>�n�ʂ̎��</summary>
    enum Ground : byte
    {
        DefaultGround = 0,
        MetallicGround = 1,
        GrassAndLeafGround = 2,
        WoodGround = 3,
    }

    [System.Serializable]
    struct PairingSound
    {
        [SerializeField, Tooltip("���")]
        public Ground type;

        [SerializeField, Tooltip("���s�������X�g")]
        public AudioClip[] walkSounds;

        [SerializeField, Tooltip("���s�������X�g")]
        public AudioClip[] runSounds;

        [SerializeField, Tooltip("�W�����v�������X�g")]
        public AudioClip[] jumpSounds;

        [SerializeField, Tooltip("���n�������X�g")]
        public AudioClip[] landingSounds;

        /// <summary>���s�����𖳍�ׂɎ擾</summary>
        /// <param name="type">�n�ʂ̎��</param>
        /// <returns>�T�E���h</returns>
        public AudioClip GetWalkSound()
        {
            if (walkSounds.Length < 1)
            {
                return null;
            }

            int rand = Random.Range(0, walkSounds.Length);
            return walkSounds[rand];
        }

        /// <summary>���s�����𖳍�ׂɎ擾</summary>
        /// <param name="type">�n�ʂ̎��</param>
        /// <returns>�T�E���h</returns>
        public AudioClip GetRunSound()
        {
            if (runSounds.Length < 1)
            {
                return null;
            }

            int rand = Random.Range(0, runSounds.Length);
            return runSounds[rand];
        }

        /// <summary>�W�����v�����𖳍�ׂɎ擾</summary>
        /// <param name="type">�n�ʂ̎��</param>
        /// <returns>�T�E���h</returns>
        public AudioClip GetJumpSound()
        {
            if (jumpSounds.Length < 1)
            {
                return null;
            }

            int rand = Random.Range(0, jumpSounds.Length);
            return jumpSounds[rand];
        }

        /// <summary>���n�����𖳍�ׂɎ擾</summary>
        /// <param name="type">�n�ʂ̎��</param>
        /// <returns>�T�E���h</returns>
        public AudioClip GetlandingSound()
        {
            if (landingSounds.Length < 1)
            {
                return null;
            }

            int rand = Random.Range(0, landingSounds.Length - 1);
            return landingSounds[rand];
        }
    }
    [SerializeField, Tooltip("�n�ʂ̎�ʂƉ�����R�Â��郊�X�g")]
    PairingSound[] _pairingSounds = null;

    /// <summary>���s�������擾</summary>
    /// <param name="tag">�Ώۃ^�O</param>
    /// <returns>����</returns>
    public AudioClip GetWalkSound(string tag)
    {
        if (_pairingSounds is null || _pairingSounds.Length < 1) return null;
        return _pairingSounds.Where(p => p.type.ToString() == tag)?.First().GetWalkSound();
    }

    /// <summary>���s�������擾</summary>
    /// <param name="tag">�Ώۃ^�O</param>
    /// <returns>����</returns>
    public AudioClip GetRunSound(string tag)
    {
        if (_pairingSounds is null || _pairingSounds.Length < 1) return null;
        return _pairingSounds.Where(p => p.type.ToString() == tag)?.First().GetRunSound();
    }

    /// <summary>�W�����v�������擾</summary>
    /// <param name="tag">�Ώۃ^�O</param>
    /// <returns>����</returns>
    public AudioClip GetJumpSound(string tag)
    {
        if (_pairingSounds is null || _pairingSounds.Length < 1) return null;
        return _pairingSounds.Where(p => p.type.ToString() == tag)?.First().GetJumpSound();
    }

    /// <summary>���n�������擾</summary>
    /// <param name="tag">�Ώۃ^�O</param>
    /// <returns>����</returns>
    public AudioClip GetLandingSound(string tag)
    {
        if (_pairingSounds is null || _pairingSounds.Length < 1) return null;
        return _pairingSounds.Where(p => p.type.ToString() == tag)?.First().GetlandingSound();
    } 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
