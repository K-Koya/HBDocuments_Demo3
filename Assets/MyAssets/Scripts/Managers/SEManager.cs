using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : Singleton<SEManager>
{
    [SerializeField, Tooltip("効果音 : 石畳・歩行時足音")]
    AudioClip[] _footStampWalkOnRocks = null;

    /// <summary>石畳・歩行時足音の数</summary>
    byte _numberOfFootStampWalkOnRock = 1;

    [SerializeField, Tooltip("効果音 : 石畳・走行時足音")]
    AudioClip[] _footStampRunOnRocks = null;

    /// <summary>石畳・走行時足音の数</summary>
    byte _numberOfFootStampRunOnRock = 1;

    [SerializeField, Tooltip("効果音 : 銃声小")]
    AudioClip _gunshotSmall = null;

    [SerializeField, Tooltip("効果音 : 銃声大")]
    AudioClip _gunshotLarge = null;

    [SerializeField, Tooltip("効果音 : マガジンを外す時の音")]
    AudioClip _magazineRemove = null;

    [SerializeField, Tooltip("効果音 : マガジンを着ける時の音")]
    AudioClip _magazineConnect = null;

    [SerializeField, Tooltip("効果音 : プルバック音")]
    AudioClip _pullBack = null;

    [SerializeField, Tooltip("効果音 : 銃弾が物に当たった時の音")]
    AudioClip[] _bulletRicochets = null;

    /// <summary>銃弾が物に当たった時の音の数</summary>
    byte _numberOfBulletRicochets = 1;



    /// <summary>効果音 : 石畳・歩行時足音</summary>
    public AudioClip FootStampWalkOnRock { get => _footStampWalkOnRocks[Random.Range(0, _numberOfFootStampWalkOnRock)]; }
    /// <summary>効果音 : 石畳・走行時足音</summary>
    public AudioClip FootStampRunOnRock { get => _footStampRunOnRocks[Random.Range(0, _numberOfFootStampRunOnRock)]; }
    /// <summary>効果音 : 銃声小</summary>
    public AudioClip GunShotSmall { get => _gunshotSmall; }
    /// <summary>効果音 : 銃声大</summary>
    public AudioClip GunShotLarge { get => _gunshotLarge; }
    /// <summary>効果音 : マガジンを外す時の音</summary>
    public AudioClip MagazineRemove { get => _magazineRemove; }
    /// <summary>効果音 : マガジンを着ける時の音</summary>
    public AudioClip MagazineConnect { get => _magazineConnect; }
    /// <summary>効果音 : プルバック音</summary>
    public AudioClip PullBack { get => _pullBack; }
    /// <summary>効果音 : 銃弾が物に当たった時の音</summary>
    public AudioClip BulletRicochets { get => _bulletRicochets[Random.Range(0, _numberOfBulletRicochets)]; }



    // Start is called before the first frame update
    void Start()
    {
        _numberOfFootStampWalkOnRock = (byte)_footStampWalkOnRocks.Length;
        _numberOfFootStampRunOnRock = (byte)_footStampRunOnRocks.Length;
        _numberOfBulletRicochets = (byte)_bulletRicochets.Length;
    }
}
