using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgingReachGoal : MonoBehaviour
{
    [SerializeField, Tooltip("�L�����N�^�[��U������Track")]
    CinemachineSmoothPath _mainTrack = null;

    [SerializeField, Tooltip("�J�������g���b�N�ɏ悹��O�̌Œ�J����")]
    CinemachineVirtualCamera _fixedCamera = null;

    [SerializeField, Tooltip("���̃Q�[�g����ړ��ł���V�[���̖��O")]
    string _nextSceneName = "EntranceStage";

    void Start()
    {
        _fixedCamera.Priority = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IStageGoalProcedurer param))
        {
            StageManagerBase.Current.PlayingToClearDemo(_mainTrack, _fixedCamera, _nextSceneName);
        }
    }
}
