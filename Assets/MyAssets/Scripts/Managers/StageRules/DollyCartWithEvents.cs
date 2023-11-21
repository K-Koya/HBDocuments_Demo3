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
        [SerializeField, Tooltip("イベント名")]
        public string name;

        [SerializeField, Tooltip("イベント発生位置")]
        public float position;

        [SerializeField, Tooltip("実行メソッド")]
        public UnityEvent list;

        /// <summary>true : 実行済みである</summary>
        bool isRun;

        /// <summary>true : 実行済みである</summary>
        public bool IsRun { get => isRun; set => isRun = value; }
    }
    [SerializeField, Tooltip("イベントリスト")]
    public DataContainer[] _events;

    /// <summary>該当のドーリーカート</summary>
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

        //指定位置通過後にメソッド実行
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

    /// <summary>該当ドーリーカートの速度を変更</summary>
    /// <param name="speed">Dollyのspeed</param>
    public void SetDollyCartSpeed(float speed)
    {
        _dolly.m_Speed = speed;
    }
}
