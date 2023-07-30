using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceInFrontOfCamera : MonoBehaviour
{
    public Transform targetObject; // 配置したいオブジェクト
    public float distanceFromCamera = 5.0f; // カメラからの距離

    void Update()
    {
        // カメラの位置を取得
        Camera mainCamera = Camera.main;
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;

        // カメラの正面にオブジェクトを配置
        targetObject.position = cameraPosition + cameraForward * distanceFromCamera;

        // オブジェクトをカメラに向ける
        targetObject.LookAt(mainCamera.transform);
    }
}






