using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>床・障害物等が動くギミックを管理</summary>
public class ObjectMovement : MonoBehaviour
{
    /// <summary>移動先順のループ方法</summary>
    public enum LoopType : byte
    {
        /// <summary>最後の地点に着くと最初の地点まで動いてループする</summary>
        ContinuousMove = 0,
        /// <summary>最後の地点に着くと最初の地点までワープしてループする</summary>
        ContinuousWrap = 1,
        /// <summary>最後の地点に着くと逆の順番に最初の地点までたどる</summary>
        ContinuousReverse = 2,
    }

    /// <summary>移動させる方法</summary>
    public enum HowToMove : byte
    {
        /// <summary>Transform.positionにVector3を加算する方法</summary>
        AddPosition = 0,
        /// <summary>同じオブジェクトにアタッチされているrigidbodyのVelocityを用いる方法</summary>
        RigidbodyVelocity = 1,
    }


    /// <summary>移動情報 </summary>
    [System.Serializable]
    public class TripInfo
    {
        [SerializeField, Tooltip("名前")]
        string _name = "name";

        [SerializeField, Tooltip("移動先の座標集 最初の要素から順々に移動")]
        Vector3 _point = new Vector3();

        [SerializeField, Tooltip("次の移動先へ向かうまでの加算値\n" +
                                "移動先に到着するたびに次のリストの速度で移動させるようになる\n")]
        float _speed = 1.0f;

        [SerializeField, Tooltip("移動先へ到着した時の滞在時間\n" +
                                "移動先に到着した時今のリストの時間だけ滞在する")]
        float _stayTime = 0.0f;


        /// <summary>コンストラクタ 名前 相対位置 速度 滞在時間 を任意に設定</summary>
        public TripInfo(string name = "name", Vector3 point = default, float speed = 1.0f, float stayTime = 0.0f)
        {
            _name = name;
            _point = point;
            _speed = speed;
            _stayTime = stayTime;
        }

        /* プロパティ */
        /// <summary>名前</summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>移動先の座標</summary>
        public Vector3 Point { get => _point; set => _point = value; }
        /// <summary>次の移動先へ向かうまでの速度値</summary>
        public float Speed { get => _speed; set => _speed = value; }
        /// <summary>移動先へ到着した時の滞在時間</summary>
        public float StayTime { get => _stayTime; set => _stayTime = value; }
    }

    /// <summary>該当オブジェクトのRigidbody</summary>
    Rigidbody _rb = null;

    /// <summary>true : 停止させる</summary>
    bool _isStop = false;




    [SerializeField, Tooltip("次の目的地番号")]
    int _tripCount = 0;

    /// <summary>待機時間</summary>
    float _stayTime = 0.0f;

    /// <summary>true : 待機中である</summary>
    bool _isStaying = false;




    [Header("移動の設定")]
    [SerializeField, Tooltip("移動させる方法を指定\n同オブジェクト内にrigidbodyがなければ強制的にAddPositionと同じ動作になる")]
    HowToMove _howToMove = HowToMove.AddPosition;

    [SerializeField, Tooltip("移動情報リスト 0番目のものを初期位置として1番目のものを先に利用する\n" +
                                "移動先座標、速度、到着した時の滞在時間を登録")]
    List<TripInfo> _tripInfos = new List<TripInfo>();




    [Header("ループの設定")]
    [SerializeField, Tooltip("移動先順のループ方法")]
    LoopType _loopType = LoopType.ContinuousMove;

    /// <summary>現在のループ回数</summary>
    int _looptime = 0;

    /// <summary>true : 逆順に動かしている所 loopTypeがContinuousReverseの時のみ使用</summary>
    bool _isReverse = false;

    [SerializeField, Tooltip("ループする回数\n0にすると無限にループする")]
    int _maxLoopTime = 0;



    /// <summary>true : 停止させる</summary>
    public bool IsStop { get => _isStop; set => _isStop = value; }

    /// <summary>移動情報リスト</summary>
    public List<TripInfo> TripInfos { get => _tripInfos; set => _tripInfos = value; }


    /// <summary>Transform.positionにVector3を加算する方法による移動処理
    /// 到着判定を返り値とする</summary>
    /// <param name="destiny">目的地座標</param>
    /// <param name="velocityMagnitude">速さ</param>
    /// <returns>目的地への到達フラグ</returns>
    bool MoveTowards(Vector3 destiny, float velocityMagnitude)
    {
        //移動
        transform.position = Vector3.MoveTowards(transform.position, destiny, velocityMagnitude * Time.deltaTime);

        //到着判定をして返す
        return (destiny == transform.position);
    }

    /// <summary>rigidbodyのVelocityを用いる方法による移動処理</summary>
    /// <param name="destiny">目的地座標</param>
    /// <param name="velocityMagnitude">速さ</param>
    /// <returns>目的地への到達フラグ</returns>
    bool MoveRigidbodyVelocity(Vector3 destiny, float velocityMagnitude)
    {
        //rigidbodyが未定義ならTransform.positionにVector3を加算する方法を利用
        if (_rb == null) return MoveTowards(destiny, velocityMagnitude);

        bool isArrival = false;

        //現在地から目的地までの距離
        Vector3 remain = destiny - transform.position;

        //rigidbody.velocityに代入する変数
        Vector3 velocity = remain.normalized * velocityMagnitude;

        //velocityよりも目的地までの距離のほうが短ければ、目的地までの距離を利用
        if (Mathf.Pow(velocityMagnitude * Time.deltaTime, 2.0f) >= remain.sqrMagnitude)
        {
            velocity = remain / Time.deltaTime;
            isArrival = true;
        }

        //移動
        _rb.velocity = velocity;

        return isArrival;
    }


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb)
        {
            _rb.useGravity = false;
        }

        //listに値がない場合は例外回避のため基準値をAdd
        if (_tripInfos.Count <= 0) _tripInfos.Add(new TripInfo());
        
        //初期移動先が配列範囲外なら補正
        if(_tripCount > _tripInfos.Count - 2)
        {
            _tripCount = _tripInfos.Count - 1;
        }

        //初期位置を調整
        if(_tripCount == 0)
        {
            transform.position = _tripInfos.Last().Point;
        }
        else
        {
            transform.position = _tripInfos[_tripCount - 1].Point;
        }
    }

    void FixedUpdate()
    {
        //ポーズ中であるか、停止指示がある場合、処理をしない
        if (_isStop) return;

        //最大ループ回数が0でなく、かつループ回数が最大値に達した場合、処理をしない
        if (_maxLoopTime != 0 && _maxLoopTime <= _looptime) return;

        //待機中である
        if (_isStaying)
        {
            //タイマーを加算
            _stayTime += Time.deltaTime;

            //rigidbodyがあればvelocityを0に
            if (_rb != null && _rb != default) _rb.velocity = Vector3.zero;

            //待機時間が終了した
            if (_stayTime >= _tripInfos[_tripCount].StayTime)
            {
                //次のフレームから移動処理を再開
                _isStaying = false;

                //ループ方法に応じてカウントアップ
                switch (_loopType)
                {
                    case LoopType.ContinuousMove:
                        {
                            //カウントアップ
                            _tripCount = (_tripCount + 1) % _tripInfos.Count;
                            break;
                        }
                    case LoopType.ContinuousWrap:
                        {
                            //カウントアップ
                            _tripCount = (_tripCount + 1) % _tripInfos.Count;
                            //ワープ処理
                            if (_tripCount <= 0) transform.position = _tripInfos[0].Point;
                            break;
                        }
                    case LoopType.ContinuousReverse:
                        {
                            //カウントダウン
                            if (_isReverse)
                            {
                                _tripCount = (_tripCount - 1) % _tripInfos.Count;
                                if (_tripCount <= 0) _isReverse = false;
                            }
                            //カウントアップ
                            else
                            {
                                _tripCount = (_tripCount + 1) % _tripInfos.Count;
                                if (_tripCount >= _tripInfos.Count - 1) _isReverse = true;
                            }

                            break;
                        }
                }
            }

            return;
        }

        //到着フラグ
        bool isArrival = false;


        //移動方法別に処理を設定
        switch (_howToMove)
        {
            case HowToMove.AddPosition:
                {
                    //移動処理&到着判定
                    isArrival = MoveTowards(_tripInfos[_tripCount].Point, _tripInfos[_tripCount].Speed);
                    break;
                }
            case HowToMove.RigidbodyVelocity:
                {
                    //移動処理&到着判定
                    isArrival = MoveRigidbodyVelocity(_tripInfos[_tripCount].Point, _tripInfos[_tripCount].Speed);
                    break;
                }
        }

        //到着した
        if (isArrival)
        {
            //待機フラグを立てる
            _isStaying = true;

            //待機時間初期化
            _stayTime = 0.0f;

            //初期位置(0番目の要素)に到着するたびにlooptimeを加算
            if (_tripCount <= 0) _looptime++;
        }
    }
}
