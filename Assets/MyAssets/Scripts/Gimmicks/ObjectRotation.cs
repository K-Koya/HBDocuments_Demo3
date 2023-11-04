using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>���E��Q��������]����M�~�b�N���Ǘ�</summary>
public class ObjectRotation : MonoBehaviour
{
    /// <summary>��]�揇�̃��[�v���@</summary>
    public enum LoopType : byte
    {
        /// <summary>�Ō�̉�]��ɒ����ƍŏ��̉�]�܂œ����ă��[�v����</summary>
        ContinuousMove = 0,
        /// <summary>�Ō�̉�]��ɒ����ƍŏ��̉�]�܂Ń��[�v���ă��[�v����</summary>
        ContinuousWrap = 1,
        /// <summary>�Ō�̉�]��ɒ����Ƌt�̏��Ԃɍŏ��̉�]�܂ł��ǂ�</summary>
        ContinuousReverse = 2,
    }

    /// <summary>��]��������@</summary>
    public enum HowToMove : byte
    {
        /// <summary>Transform.rotation�ɉ��Z������@</summary>
        AddEulerAngle= 0,
        /// <summary>�����I�u�W�F�N�g�ɃA�^�b�`����Ă���rigidbody��angularVelocity��p������@</summary>
        RigidbodyVelocity = 1,
    }

    /// <summary>��]���</summary>
    [System.Serializable]
    public class RotateInfo
    {
        [SerializeField, Tooltip("���O")]
        string _name = "name";

        [SerializeField, Tooltip("��]��̍��W �ŏ��̗v�f���珇�X�ɉ�]")]
        Vector3 _direction = default;

        [SerializeField, Tooltip("true : �ړI�p�x�Ɍ����āA�߂������։�]����")]
        bool _isNearRotation = true;

        [SerializeField, Tooltip("���̉�]��֌������܂ł̉��Z�l\n" +
                                "��]��ɓ������邽�тɎ��̃��X�g�̑��x�ŉ�]������悤�ɂȂ�\n")]
        float _speed = 1.0f;

        [SerializeField, Tooltip("��]��֓����������̑؍ݎ���\n" +
                                "��]��ɓ������������̃��X�g�̎��Ԃ����؍݂���")]
        float _stayTime = 0.0f;


        /// <summary>�R���X�g���N�^ ���O ���Έʒu ���x �؍ݎ��� ��C�ӂɐݒ�</summary>
        public RotateInfo(string name = "name", Vector3 direction = default, float speed = 1.0f, float stayTime = 0.0f)
        {
            _name = name;
            _direction = direction;
            _speed = speed;
            _stayTime = stayTime;
        }

        /* �v���p�e�B */
        /// <summary>���O</summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>���ʕ����x�N�g��</summary>
        public Vector3 Direction { get => _direction; set => _direction = value; }
        /// <summary>���̉�]�p�֌������܂ł̑��x�l</summary>
        public float Speed { get => _speed; set => _speed = value; }
        /// <summary>��]��֓����������̑؍ݎ���</summary>
        public float StayTime { get => _stayTime; set => _stayTime = value; }
    }

    /// <summary>�Y���I�u�W�F�N�g��Rigidbody</summary>
    Rigidbody _rb = null;

    /// <summary>true : ��~������</summary>
    bool _isStop = false;

    /// <summary>���Ԗڂ̉�]��֌������Ă��邩</summary>
    int _tripCount = 0;

    /// <summary>�ҋ@����</summary>
    float _stayTime = 0.0f;

    /// <summary>true : �ҋ@���ł���</summary>
    bool _isStaying = false;


    [Header("��]�̐ݒ�")]
    [SerializeField, Tooltip("��]��������@���w��\n���I�u�W�F�N�g����rigidbody���Ȃ���΋����I��AddEulerAngle�Ɠ�������ɂȂ�")]
    HowToMove _howToMove = HowToMove.AddEulerAngle;

    [SerializeField, Tooltip("��]��񃊃X�g 0�Ԗڂ̂��̂������p�x�Ƃ���1�Ԗڂ̂��̂��ɗ��p����\n" +
                                "��]��p�x�A���x�A�����������̑؍ݎ��Ԃ�o�^")]
    List<RotateInfo> _rotateInfos = new List<RotateInfo>();


    [Header("���[�v�̐ݒ�")]
    [SerializeField, Tooltip("��]�揇�̃��[�v���@")]
    LoopType _loopType = LoopType.ContinuousMove;

    /// <summary>���݂̃��[�v��</summary>
    int _looptime = 0;

    /// <summary>true : �t���ɓ������Ă��鏊 loopType��ContinuousReverse�̎��̂ݎg�p</summary>
    bool _isReverse = false;

    [SerializeField, Tooltip("���[�v�����\n0�ɂ���Ɩ����Ƀ��[�v����")]
    int _maxLoopTime = 0;


    /// <summary>true : ��~������</summary>
    public bool IsStop { get => _isStop; set => _isStop = value; }

    /// <summary>�ړ���񃊃X�g</summary>
    public List<RotateInfo> RotateInfos { get => _rotateInfos; set => _rotateInfos = value; }


    /// <summary>Transform.forward����@�ɂ��ړ�����
    /// ���������Ԃ�l�Ƃ���</summary>
    /// <param name="destiny">�ړI�̐��ʕ���</param>
    /// <param name="velocityAngle">����</param>
    /// <returns>�ړI�n�ւ̓��B�t���O</returns>
    bool RotateTowards(Vector3 destiny, float velocityAngle)
    {
        //��]
        transform.forward = Vector3.RotateTowards(transform.forward, destiny, velocityAngle * Time.deltaTime, 1f);

        //������������ĕԂ�
        return (destiny == transform.position);
    }


    /// <summary>rigidbody��AngleVelocity��p������@�ɂ���]����</summary>
    /// <param name="destiny">�ړI��]�x�N�g��</param>
    /// <param name="velocityAngle">����</param>
    /// <returns>�ړI�n�ւ̓��B�t���O</returns>
    bool RotateRigidbodyVelocity(Vector3 destiny, float velocityAngle)
    {
        //rigidbody������`�Ȃ�Transform.position��Vector3�����Z������@�𗘗p
        if (_rb == null) return RotateTowards(destiny, velocityAngle);

        bool isArrival = false;

        //���ݒn����ړI�n�܂ł̋���
        Vector3 remain = destiny.normalized - transform.forward;

        //rigidbody.angularVelocity�ɑ������ϐ�
        Vector3 velocity = remain.normalized * velocityAngle;

        //velocity�����ړI�n�܂ł̋����̂ق����Z����΁A�ړI�n�܂ł̋����𗘗p
        if (Mathf.Pow(velocityAngle * Time.deltaTime, 2.0f) >= remain.sqrMagnitude)
        {
            velocity = remain / Time.deltaTime;
            isArrival = true;
        }

        //�ړ�
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
