using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgingCourseOut : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out ICourseOutProcedurer param))
        {
            StageManagerBase.Current.State = StateOnStage.MissDemo;
        }
    }
}
