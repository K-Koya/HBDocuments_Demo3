using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 3次元空間中のGUIにサイズ調整をかける
/// </summary>
public class GUI3dFixScreenScale : MonoBehaviour {

    /// <summary>
    /// GUI3D初期サイズ
    /// </summary>
    Vector3 firstScale = Vector3.one;

    /// <summary>
    /// メインカメラ用タグ
    /// </summary>
    [SerializeField]
    string mainCameraTag = "MainCamera";
    /// <summary>
    /// カメラオブジェクト
    /// </summary>
    GameObject cameraObj = default;



    // Use this for initialization
    void Start () {

        cameraObj = GameObject.FindWithTag(mainCameraTag);
        firstScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {

        transform.localScale = firstScale * GetDistance() * 0.4f;
    }


    /// <summary>
    /// カメラからの距離を取得
    /// </summary>
    float GetDistance()
    {
        return (transform.position - cameraObj.transform.position).magnitude;
    }

}
