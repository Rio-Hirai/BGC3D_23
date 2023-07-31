using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceInFrontOfCamera : MonoBehaviour
{
    public Transform targetObject; // �z�u�������I�u�W�F�N�g
    public float distanceFromCamera = 5.0f; // �J��������̋���

    void Update()
    {
        // �J�����̈ʒu���擾
        Camera mainCamera = Camera.main;
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 cameraForward = mainCamera.transform.forward;

        // �J�����̐��ʂɃI�u�W�F�N�g��z�u
        targetObject.position = cameraPosition + cameraForward * distanceFromCamera;

        // �I�u�W�F�N�g���J�����Ɍ�����
        targetObject.LookAt(mainCamera.transform);
    }
}






