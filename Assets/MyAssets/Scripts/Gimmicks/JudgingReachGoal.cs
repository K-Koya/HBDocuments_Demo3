using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgingReachGoal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IStageGoalProcedurer param))
        {
            StageManagerBase.Current.State = StateOnStage.ClearDemo;
        }
    }
}
