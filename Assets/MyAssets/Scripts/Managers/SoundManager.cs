using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    /// <summary>地面の種類</summary>
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
        [SerializeField, Tooltip("種別")]
        public Ground type;

        [SerializeField, Tooltip("歩行音声リスト")]
        public AudioClip[] walkSounds;

        [SerializeField, Tooltip("走行音声リスト")]
        public AudioClip[] runSounds;

        [SerializeField, Tooltip("ジャンプ音声リスト")]
        public AudioClip[] jumpSounds;

        [SerializeField, Tooltip("着地音声リスト")]
        public AudioClip[] landingSounds;

        /// <summary>歩行音声を無作為に取得</summary>
        /// <param name="type">地面の種類</param>
        /// <returns>サウンド</returns>
        public AudioClip GetWalkSound()
        {
            if (walkSounds.Length < 1)
            {
                return null;
            }

            int rand = Random.Range(0, walkSounds.Length);
            return walkSounds[rand];
        }

        /// <summary>走行音声を無作為に取得</summary>
        /// <param name="type">地面の種類</param>
        /// <returns>サウンド</returns>
        public AudioClip GetRunSound()
        {
            if (runSounds.Length < 1)
            {
                return null;
            }

            int rand = Random.Range(0, runSounds.Length);
            return runSounds[rand];
        }

        /// <summary>ジャンプ音声を無作為に取得</summary>
        /// <param name="type">地面の種類</param>
        /// <returns>サウンド</returns>
        public AudioClip GetJumpSound()
        {
            if (jumpSounds.Length < 1)
            {
                return null;
            }

            int rand = Random.Range(0, jumpSounds.Length);
            return jumpSounds[rand];
        }

        /// <summary>着地音声を無作為に取得</summary>
        /// <param name="type">地面の種類</param>
        /// <returns>サウンド</returns>
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
    [SerializeField, Tooltip("地面の種別と音声を紐づけるリスト")]
    PairingSound[] _pairingSounds = null;

    /// <summary>歩行音声を取得</summary>
    /// <param name="tag">対象タグ</param>
    /// <returns>音声</returns>
    public AudioClip GetWalkSound(string tag)
    {
        if (_pairingSounds is null || _pairingSounds.Length < 1) return null;
        return _pairingSounds.Where(p => p.type.ToString() == tag)?.First().GetWalkSound();
    }

    /// <summary>走行音声を取得</summary>
    /// <param name="tag">対象タグ</param>
    /// <returns>音声</returns>
    public AudioClip GetRunSound(string tag)
    {
        if (_pairingSounds is null || _pairingSounds.Length < 1) return null;
        return _pairingSounds.Where(p => p.type.ToString() == tag)?.First().GetRunSound();
    }

    /// <summary>ジャンプ音声を取得</summary>
    /// <param name="tag">対象タグ</param>
    /// <returns>音声</returns>
    public AudioClip GetJumpSound(string tag)
    {
        if (_pairingSounds is null || _pairingSounds.Length < 1) return null;
        return _pairingSounds.Where(p => p.type.ToString() == tag)?.First().GetJumpSound();
    }

    /// <summary>着地音声を取得</summary>
    /// <param name="tag">対象タグ</param>
    /// <returns>音声</returns>
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
