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
        this.transform.localScale = new Vector3(Server.target_size, Server.target_size, Server.target_size); // �^�[�Q�b�g�̑傫����������
        this.name = "target_" + Server.target_id; // �^�[�Q�b�g�̖��O��������
        this.GetComponent<target_para_set>().Id = Server.target_id; // �^�[�Q�b�g��ID��������
        Server.target_id++; // �^�[�Q�b�g��ID��A�Ԃɂ��邽�߂ɉ��Z
    }
}
