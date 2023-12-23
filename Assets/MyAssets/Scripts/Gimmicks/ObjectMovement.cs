using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>���E��Q�����������M�~�b�N���Ǘ�</summary>
public class ObjectMovement : MonoBehaviour
{
    /// <summary>�ړ��揇�̃��[�v���@</summary>
    public enum LoopType : byte
    {
        /// <summary>�Ō�̒n�_�ɒ����ƍŏ��̒n�_�܂œ����ă��[�v����</summary>
        ContinuousMove = 0,
        /// <summary>�Ō�̒n�_�ɒ����ƍŏ��̒n�_�܂Ń��[�v���ă��[�v����</summary>
        ContinuousWrap = 1,
        /// <summary>�Ō�̒n�_�ɒ����Ƌt�̏��Ԃɍŏ��̒n�_�܂ł��ǂ�</summary>
        ContinuousReverse = 2,
    }

    /// <summary>�ړ���������@</summary>
    public enum HowToMove : byte
    {
        /// <summary>Transform.position��Vector3�����Z������@</summary>
        AddPosition = 0,
        /// <summary>�����I�u�W�F�N�g�ɃA�^�b�`����Ă���rigidbody��Velocity��p������@</summary>
        RigidbodyVelocity = 1,
    }


    /// <summary>�ړ���� </summary>
    [System.Serializable]
    public class TripInfo
    {
        [SerializeField, Tooltip("���O")]
        string _name = "name";

        [SerializeField, Tooltip("�ړ���̍��W�W �ŏ��̗v�f���珇�X�Ɉړ�")]
        Vector3 _point = new Vector3();

        [SerializeField, Tooltip("���̈ړ���֌������܂ł̉��Z�l\n" +
                                "�ړ���ɓ������邽�тɎ��̃��X�g�̑��x�ňړ�������悤�ɂȂ�\n")]
        float _speed = 1.0f;

        [SerializeField, Tooltip("�ړ���֓����������̑؍ݎ���\n" +
                                "�ړ���ɓ������������̃��X�g�̎��Ԃ����؍݂���")]
        float _stayTime = 0.0f;


        /// <summary>�R���X�g���N�^ ���O ���Έʒu ���x �؍ݎ��� ��C�ӂɐݒ�</summary>
        public TripInfo(string name = "name", Vector3 point = default, float speed = 1.0f, float stayTime = 0.0f)
        {
            _name = name;
            _point = point;
            _speed = speed;
            _stayTime = stayTime;
        }

        /* �v���p�e�B */
        /// <summary>���O</summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>�ړ���̍��W</summary>
        public Vector3 Point { get => _point; set => _point = value; }
        /// <summary>���̈ړ���֌������܂ł̑��x�l</summary>
        public float Speed { get => _speed; set => _speed = value; }
        /// <summary>�ړ���֓����������̑؍ݎ���</summary>
        public float StayTime { get => _stayTime; set => _stayTime = value; }
    }

    /// <summary>�Y���I�u�W�F�N�g��Rigidbody</summary>
    Rigidbody _rb = null;

    /// <summary>true : ��~������</summary>
    bool _isStop = false;




    [SerializeField, Tooltip("���̖ړI�n�ԍ�")]
    int _tripCount = 0;

    /// <summary>�ҋ@����</summary>
    float _stayTime = 0.0f;

    /// <summary>true : �ҋ@���ł���</summary>
    bool _isStaying = false;




    [Header("�ړ��̐ݒ�")]
    [SerializeField, Tooltip("�ړ���������@���w��\n���I�u�W�F�N�g����rigidbody���Ȃ���΋����I��AddPosition�Ɠ�������ɂȂ�")]
    HowToMove _howToMove = HowToMove.AddPosition;

    [SerializeField, Tooltip("�ړ���񃊃X�g 0�Ԗڂ̂��̂������ʒu�Ƃ���1�Ԗڂ̂��̂��ɗ��p����\n" +
                                "�ړ�����W�A���x�A�����������̑؍ݎ��Ԃ�o�^")]
    List<TripInfo> _tripInfos = new List<TripInfo>();




    [Header("���[�v�̐ݒ�")]
    [SerializeField, Tooltip("�ړ��揇�̃��[�v���@")]
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
    public List<TripInfo> TripInfos { get => _tripInfos; set => _tripInfos = value; }


    /// <summary>Transform.position��Vector3�����Z������@�ɂ��ړ�����
    /// ���������Ԃ�l�Ƃ���</summary>
    /// <param name="destiny">�ړI�n���W</param>
    /// <param name="velocityMagnitude">����</param>
    /// <returns>�ړI�n�ւ̓��B�t���O</returns>
    bool MoveTowards(Vector3 destiny, float velocityMagnitude)
    {
        //�ړ�
        transform.position = Vector3.MoveTowards(transform.position, destiny, velocityMagnitude * Time.deltaTime);

        //������������ĕԂ�
        return (destiny == transform.position);
    }

    /// <summary>rigidbody��Velocity��p������@�ɂ��ړ�����</summary>
    /// <param name="destiny">�ړI�n���W</param>
    /// <param name="velocityMagnitude">����</param>
    /// <returns>�ړI�n�ւ̓��B�t���O</returns>
    bool MoveRigidbodyVelocity(Vector3 destiny, float velocityMagnitude)
    {
        //rigidbody������`�Ȃ�Transform.position��Vector3�����Z������@�𗘗p
        if (_rb == null) return MoveTowards(destiny, velocityMagnitude);

        bool isArrival = false;

        //���ݒn����ړI�n�܂ł̋���
        Vector3 remain = destiny - transform.position;

        //rigidbody.velocity�ɑ������ϐ�
        Vector3 velocity = remain.normalized * velocityMagnitude;

        //velocity�����ړI�n�܂ł̋����̂ق����Z����΁A�ړI�n�܂ł̋����𗘗p
        if (Mathf.Pow(velocityMagnitude * Time.deltaTime, 2.0f) >= remain.sqrMagnitude)
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
        if (_rb)
        {
            _rb.useGravity = false;
        }

        //list�ɒl���Ȃ��ꍇ�͗�O����̂��ߊ�l��Add
        if (_tripInfos.Count <= 0) _tripInfos.Add(new TripInfo());
        
        //�����ړ��悪�z��͈͊O�Ȃ�␳
        if(_tripCount > _tripInfos.Count - 2)
        {
            _tripCount = _tripInfos.Count - 1;
        }

        //�����ʒu�𒲐�
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
        //�|�[�Y���ł��邩�A��~�w��������ꍇ�A���������Ȃ�
        if (_isStop) return;

        //�ő僋�[�v�񐔂�0�łȂ��A�����[�v�񐔂��ő�l�ɒB�����ꍇ�A���������Ȃ�
        if (_maxLoopTime != 0 && _maxLoopTime <= _looptime) return;

        //�ҋ@���ł���
        if (_isStaying)
        {
            //�^�C�}�[�����Z
            _stayTime += Time.deltaTime;

            //rigidbody�������velocity��0��
            if (_rb != null && _rb != default) _rb.velocity = Vector3.zero;

            //�ҋ@���Ԃ��I������
            if (_stayTime >= _tripInfos[_tripCount].StayTime)
            {
                //���̃t���[������ړ��������ĊJ
                _isStaying = false;

                //���[�v���@�ɉ����ăJ�E���g�A�b�v
                switch (_loopType)
                {
                    case LoopType.ContinuousMove:
                        {
                            //�J�E���g�A�b�v
                            _tripCount = (_tripCount + 1) % _tripInfos.Count;
                            break;
                        }
                    case LoopType.ContinuousWrap:
                        {
                            //�J�E���g�A�b�v
                            _tripCount = (_tripCount + 1) % _tripInfos.Count;
                            //���[�v����
                            if (_tripCount <= 0) transform.position = _tripInfos[0].Point;
                            break;
                        }
                    case LoopType.ContinuousReverse:
                        {
                            //�J�E���g�_�E��
                            if (_isReverse)
                            {
                                _tripCount = (_tripCount - 1) % _tripInfos.Count;
                                if (_tripCount <= 0) _isReverse = false;
                            }
                            //�J�E���g�A�b�v
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

        //�����t���O
        bool isArrival = false;


        //�ړ����@�ʂɏ�����ݒ�
        switch (_howToMove)
        {
            case HowToMove.AddPosition:
                {
                    //�ړ�����&��������
                    isArrival = MoveTowards(_tripInfos[_tripCount].Point, _tripInfos[_tripCount].Speed);
                    break;
                }
            case HowToMove.RigidbodyVelocity:
                {
                    //�ړ�����&��������
                    isArrival = MoveRigidbodyVelocity(_tripInfos[_tripCount].Point, _tripInfos[_tripCount].Speed);
                    break;
                }
        }

        //��������
        if (isArrival)
        {
            //�ҋ@�t���O�𗧂Ă�
            _isStaying = true;

            //�ҋ@���ԏ�����
            _stayTime = 0.0f;

            //�����ʒu(0�Ԗڂ̗v�f)�ɓ������邽�т�looptime�����Z
            if (_tripCount <= 0) _looptime++;
        }
    }
}
