using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    #region �萔
    /// <summary>�ڒn������Ƃ邽�߂Ɏg�����̂̔��a���A�R���C�_�[�̔��a�̉��{�����������l</summary>
    const float _GROUND_CHECK_RADIUS_RATE = 0.99f;

    #endregion

    #region �����o

    /// <summary>���Y�I�u�W�F�N�g�̃J�v�Z���R���C�_�[</summary>
    CapsuleCollider _collider = default;

    [SerializeField, Tooltip("True : ���n���Ă���")]
    bool _isGround = false;

    [SerializeField, Tooltip("True : �ǂɂ������Ă���")]
    bool _isWall = false;

    [SerializeField, Tooltip("�n�ʂƕǂ̋��E�p�x")]
    float _slopeLimit = 45f;

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    Vector3 _gravityDirection = Vector3.down;

    /// <summary>SphereCast���鎞�̊�_�ƂȂ���W</summary>
    Vector3 _castBasePosition = Vector3.zero;

    /// <summary>�o����Ƃ݂Ȃ����߂̒��S�_����̋���</summary>
    float _slopeAngleThreshold = 1f;


    #endregion

    #region �v���p�e�B
    /// <summary>True : ���n���Ă���</summary>
    public bool IsGround { get => _isGround; }

    /// <summary>True : �ǂɂ������Ă���</summary>
    public bool IsWall { get => _isWall; }

    /// <summary>�L�����N�^�[�̏d�͌���</summary>
    public Vector3 GravityDirection { get => _gravityDirection; set => _gravityDirection = value; }
    #endregion

    /*
    void OnEnable()
    {
        Physics.ContactEvent += Checker;
    }

    void OnDisable()
    {
        Physics.ContactEvent -= Checker;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        //_collider.providesContacts = true;
        _castBasePosition = _collider.center + Vector3.down * ((_collider.height - _collider.radius * 2f) / 2f);

        //�~�ʔ��a����ʒ������߂����
        _slopeAngleThreshold = 2f * _collider.radius * Mathf.Sin(Mathf.Deg2Rad * _slopeLimit / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.IsPause) return;

        _isGround = false;
        RaycastHit hit;
        if (Physics.SphereCast(_castBasePosition + transform.position, _collider.radius * _GROUND_CHECK_RADIUS_RATE, _gravityDirection, out hit, _collider.radius, LayerManager.Instance.AllGround))
        {
            if (Vector3.SqrMagnitude(transform.position - hit.point) < _slopeAngleThreshold * _slopeAngleThreshold)
            {
                _isGround = true;
            }
        }
    }

    /*
    /// <summary>Physics.ContactEvent�f���Q�[�g�ŌĂяo�����\�b�h �l�X�����蔻����Ƃ�</summary>
    /// <param name="scene"></param>
    /// <param name="headers"></param>
    void Checker(PhysicsScene scene, NativeArray<ContactPairHeader>.ReadOnly headers)
    {
        if (PauseManager.Instance.IsPause) return;

        _isGround = false;
        foreach(ContactPairHeader header in headers) 
        {
            for(int i = 0; i < header.PairCount; i++)
            {
                var contactPair = header.GetContactPair(i);
                Debug.Log($"{contactPair.Collider.name}��{contactPair.OtherCollider.name}��{contactPair.ContactCount}�ӏ��ŏՓ�");
            }
        }
    }
    */
}
