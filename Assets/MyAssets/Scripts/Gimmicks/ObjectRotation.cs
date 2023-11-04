using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>床・障害物等が回転するギミックを管理</summary>
public class ObjectRotation : MonoBehaviour
{
    /// <summary>回転先順のループ方法</summary>
    public enum LoopType : byte
    {
        /// <summary>最後の回転先に着くと最初の回転まで動いてループする</summary>
        ContinuousMove = 0,
        /// <summary>最後の回転先に着くと最初の回転までワープしてループする</summary>
        ContinuousWrap = 1,
        /// <summary>最後の回転先に着くと逆の順番に最初の回転までたどる</summary>
        ContinuousReverse = 2,
    }

    /// <summary>回転させる方法</summary>
    public enum HowToMove : byte
    {
        /// <summary>Transform.rotationに加算する方法</summary>
        AddEulerAngle= 0,
        /// <summary>同じオブジェクトにアタッチされているrigidbodyのangularVelocityを用いる方法</summary>
        RigidbodyVelocity = 1,
    }

    /// <summary>回転情報</summary>
    [System.Serializable]
    public class RotateInfo
    {
        [SerializeField, Tooltip("名前")]
        string _name = "name";

        [SerializeField, Tooltip("回転先の座標 最初の要素から順々に回転")]
        Vector3 _direction = default;

        [SerializeField, Tooltip("true : 目的角度に向けて、近い向きへ回転する")]
        bool _isNearRotation = true;

        [SerializeField, Tooltip("次の回転先へ向かうまでの加算値\n" +
                                "回転先に到着するたびに次のリストの速度で回転させるようになる\n")]
        float _speed = 1.0f;

        [SerializeField, Tooltip("回転先へ到着した時の滞在時間\n" +
                                "回転先に到着した時今のリストの時間だけ滞在する")]
        float _stayTime = 0.0f;


        /// <summary>コンストラクタ 名前 相対位置 速度 滞在時間 を任意に設定</summary>
        public RotateInfo(string name = "name", Vector3 direction = default, float speed = 1.0f, float stayTime = 0.0f)
        {
            _name = name;
            _direction = direction;
            _speed = speed;
            _stayTime = stayTime;
        }

        /* プロパティ */
        /// <summary>名前</summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>正面方向ベクトル</summary>
        public Vector3 Direction { get => _direction; set => _direction = value; }
        /// <summary>次の回転角へ向かうまでの速度値</summary>
        public float Speed { get => _speed; set => _speed = value; }
        /// <summary>回転先へ到着した時の滞在時間</summary>
        public float StayTime { get => _stayTime; set => _stayTime = value; }
    }

    /// <summary>該当オブジェクトのRigidbody</summary>
    Rigidbody _rb = null;

    /// <summary>true : 停止させる</summary>
    bool _isStop = false;

    /// <summary>何番目の回転先へ向かっているか</summary>
    int _tripCount = 0;

    /// <summary>待機時間</summary>
    float _stayTime = 0.0f;

    /// <summary>true : 待機中である</summary>
    bool _isStaying = false;


    [Header("回転の設定")]
    [SerializeField, Tooltip("回転させる方法を指定\n同オブジェクト内にrigidbodyがなければ強制的にAddEulerAngleと同じ動作になる")]
    HowToMove _howToMove = HowToMove.AddEulerAngle;

    [SerializeField, Tooltip("回転情報リスト 0番目のものを初期角度として1番目のものを先に利用する\n" +
                                "回転先角度、速度、到着した時の滞在時間を登録")]
    List<RotateInfo> _rotateInfos = new List<RotateInfo>();


    [Header("ループの設定")]
    [SerializeField, Tooltip("回転先順のループ方法")]
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
    public List<RotateInfo> RotateInfos { get => _rotateInfos; set => _rotateInfos = value; }


    /// <summary>Transform.forwardを方法による移動処理
    /// 到着判定を返り値とする</summary>
    /// <param name="destiny">目的の正面方向</param>
    /// <param name="velocityAngle">速さ</param>
    /// <returns>目的地への到達フラグ</returns>
    bool RotateTowards(Vector3 destiny, float velocityAngle)
    {
        //回転
        transform.forward = Vector3.RotateTowards(transform.forward, destiny, velocityAngle * Time.deltaTime, 1f);

        //到着判定をして返す
        return (destiny == transform.position);
    }


    /// <summary>rigidbodyのAngleVelocityを用いる方法による回転処理</summary>
    /// <param name="destiny">目的回転ベクトル</param>
    /// <param name="velocityAngle">速さ</param>
    /// <returns>目的地への到達フラグ</returns>
    bool RotateRigidbodyVelocity(Vector3 destiny, float velocityAngle)
    {
        //rigidbodyが未定義ならTransform.positionにVector3を加算する方法を利用
        if (_rb == null) return RotateTowards(destiny, velocityAngle);

        bool isArrival = false;

        //現在地から目的地までの距離
        Vector3 remain = destiny.normalized - transform.forward;

        //rigidbody.angularVelocityに代入する変数
        Vector3 velocity = remain.normalized * velocityAngle;

        //velocityよりも目的地までの距離のほうが短ければ、目的地までの距離を利用
        if (Mathf.Pow(velocityAngle * Time.deltaTime, 2.0f) >= remain.sqrMagnitude)
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
