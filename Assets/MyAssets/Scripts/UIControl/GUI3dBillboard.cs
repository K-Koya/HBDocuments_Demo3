using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>3次元空間中のGUIをカメラの方向へ向ける</summary>
public class GUI3dBillboard : MonoBehaviour {

    [SerializeField, Tooltip("対象のカメラのタグ")]
    string _MainCameraTag = "MainCamera";

    /// <summary>カメラオブジェクト</summary>
    GameObject _MainCameraObj = default;


    // Use this for initialization
    void Start () {

        _MainCameraObj = GameObject.FindWithTag(_MainCameraTag);
    }
	
	// Update is called once per frame
	void Update () {

        transform.rotation = _MainCameraObj.transform.rotation;
	}
}
