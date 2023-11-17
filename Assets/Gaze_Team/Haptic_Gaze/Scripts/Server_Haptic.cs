using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class Server_Haptic : MonoBehaviour
{
    public Haptic_Feedback haptic_script;
    public bool Target_Setting;
    public bool Haptic_Feedback;
    public Vector3 Eye_Point;
    public Camera Camera;
    public GameObject Eye_Pointer;
    public GameObject Targets;
    public float distanceFromCamera = 3.0f; // �J��������̋���
    public float distance;
    public float Moved_Power;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Target_Setting)
        {
            PlaceObjectInFront();
            Target_Setting = false;
        }

        if (Haptic_Feedback)
        {
            distance = Vector3.Distance(Camera.transform.position, Eye_Pointer.transform.position);

            if (distance - distanceFromCamera <= 0)
            {
                distance = 0;
            }
            else
            {
                distance -= distanceFromCamera;
            }

            Moved_Power = haptic_script.Max_Power - distance * 100 * 2;
            if (Moved_Power < haptic_script.Min_Power)
            {
                Moved_Power = haptic_script.Min_Power + 25;
            }
        }
    }

    public void PlaceObjectInFront()
    {
        //// �J�����̐��ʕ����ɃI�u�W�F�N�g��z�u
        //Targets.transform.position = Targets.transform.position + Targets.transform.forward * distanceFromCamera;

        //// �I�u�W�F�N�g���J�����Ɍ�������
        //Targets.transform.LookAt(Camera.transform);

        Vector3 forward = Vector3.Scale(Camera.transform.forward, new Vector3(1, 0, 1)).normalized; // ���[�U�i�J�����j�̑O���������擾
        Vector3 newPosition = Camera.transform.position + forward * distanceFromCamera; // ���[�U�i�J�����j�̈ʒu���^�[�Q�b�g�Q�̐V�����ʒu�ɐݒ�
        newPosition.y = Camera.transform.position.y; // �^�[�Q�b�g�Q�ƃ��[�U�i�J�����j�̍����𓯂��ɂ���
        Targets.transform.position = newPosition; // �^�[�Q�b�g�Q��V�����ʒu�Ɉړ�
        Targets.transform.LookAt(Camera.transform.position); // �^�[�Q�b�g�Q�����[�U�i�J�����j�̕����Ɍ�����
        Vector3 rotation = Targets.transform.eulerAngles; // �^�[�Q�b�g�Q���t�������Ă��܂��̂�180�x��]������
        rotation.y += 180; // �H�H�H
        Targets.transform.eulerAngles = rotation; // �H�H�H

        Target_Setting = false; // �@�\�t���O�����Z�b�g
    }
}
