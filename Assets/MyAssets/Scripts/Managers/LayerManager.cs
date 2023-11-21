using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : Singleton<LayerManager>
{
    [Header("以下に対応レイヤを指定")]

    #region メンバ
    [SerializeField, Tooltip("地面レイヤ")]
    LayerMask _Ground = default;
    [SerializeField, Tooltip("カメラがすり抜ける地面レイヤ")]
    LayerMask _SeeThroughGround = default;
    [SerializeField, Tooltip("敵レイヤ")]
    LayerMask _Enemy = default;
    [SerializeField, Tooltip("プレイヤーのレイヤ")]
    LayerMask _Player = default;
    [SerializeField, Tooltip("味方レイヤ")]
    LayerMask _Allies = default;
    #endregion

    #region プロパティ
    /// <summary>地面レイヤ</summary>
    public LayerMask Ground { get => _Ground; }
    /// <summary>カメラがすり抜ける地面レイヤ</summary>
    public LayerMask SeeThroughGround { get => _SeeThroughGround; }
    /// <summary>敵レイヤ</summary>
    public LayerMask Enemy { get => _Enemy; }
    /// <summary>プレイヤーのレイヤ</summary>
    public LayerMask Player { get => _Player; }
    /// <summary>味方レイヤ</summary>
    public LayerMask Allies { get => _Allies; }
    /// <summary>全ての地面レイヤ</summary>
    public LayerMask AllGround { get => _Ground | _SeeThroughGround; }
    /// <summary>全てのキャラクターのレイヤ</summary>
    public LayerMask AllCharacter { get => _Enemy | _Player | _Allies; }
    /// <summary>全ての味方キャラクターのレイヤ</summary>
    public LayerMask AllAllies { get => _Allies | _Player; }
    /// <summary>レティクルが乗る（ターゲットできるオブジェクト）のレイヤ</summary>
    public LayerMask OnTheReticle { get => _Allies | _Enemy | _Ground; }

    #endregion
}
