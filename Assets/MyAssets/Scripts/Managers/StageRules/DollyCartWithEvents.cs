using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class DollyCartWithEvents : MonoBehaviour
{
    [System.Serializable]
    public struct DataContainer
    {
        [SerializeField, Tooltip("�C�x���g��")]
        public string name;

        [SerializeField, Tooltip("�C�x���g�����ʒu")]
        public float position;

        [SerializeField, Tooltip("���s���\�b�h")]
        public UnityEvent list;

        /// <summary>true : ���s�ς݂ł���</summary>
        bool isRun;

        /// <summary>true : ���s�ς݂ł���</summary>
        public bool IsRun { get => isRun; set => isRun = value; }
    }
    [SerializeField, Tooltip("�C�x���g���X�g")]
    public DataContainer[] _events;

    /// <summary>�Y���̃h�[���[�J�[�g</summary>
    CinemachineDollyCart _dolly = null;

    void Start()
    {
        _dolly = GetComponent<CinemachineDollyCart>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseManager.Instance.IsPause)
        {
            return;
        }

        //�w��ʒu�ʉߌ�Ƀ��\�b�h���s
        for(int i = 0; i < _events.Length; i++) 
        {
            if (!_events[i].IsRun && _events[i].position <= _dolly.m_Position)
            {
                _events[i].list?.Invoke();
                _events[i].IsRun = true;
            }
            else if(_events[i].IsRun && _events[i].position > _dolly.m_Position)
            {
                _events[i].IsRun = false;
            }
        }
    }

    /// <summary>�Y���h�[���[�J�[�g�̑��x��ύX</summary>
    /// <param name="speed">Dolly��speed</param>
    public void SetDollyCartSpeed(float speed)
    {
        _dolly.m_Speed = speed;
    }
}
