using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class target_size_set : MonoBehaviour
{
    public receiver Server; // �T�[�o�ڑ�
    public float dtime; // �݌v�������Ԃ��i�[����ϐ�

    void Start()
    {
        if (Server.target_size_mini_switch)
        {
            float distance_of_camera_to_target = Vector3.Distance(Server.head_obj.transform.position, this.transform.position);
            float angleRadians = 1.0f * Mathf.Deg2Rad;
            float height = (Mathf.Tan(angleRadians) * distance_of_camera_to_target);
            this.transform.localScale = new Vector3(height, height, height); // �^�[�Q�b�g�̑傫����������
        }
        else
        {
            this.transform.localScale = new Vector3(Server.target_size, Server.target_size, Server.target_size); // �^�[�Q�b�g�̑傫����������
        }

        this.name = "target_" + Server.target_id; // �^�[�Q�b�g�̖��O��������
        this.GetComponent<target_para_set>().Id = Server.target_id; // �^�[�Q�b�g��ID��������
        Server.target_id++; // �^�[�Q�b�g��ID��A�Ԃɂ��邽�߂ɉ��Z
    }
}
