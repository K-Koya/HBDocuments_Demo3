using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgingReachGoal : MonoBehaviour
{
    [SerializeField, Tooltip("キャラクターを誘導するTrack")]
    CinemachineSmoothPath _mainTrack = null;

    [SerializeField, Tooltip("カメラをトラックに乗せる前の固定カメラ")]
    CinemachineVirtualCamera _fixedCamera = null;

    [SerializeField, Tooltip("このゲートから移動できるシーンの名前")]
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
