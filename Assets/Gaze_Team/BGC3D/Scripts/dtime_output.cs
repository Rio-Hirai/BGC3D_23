using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dtime_output : MonoBehaviour
{
    public GameObject score_object = null; // Text�I�u�W�F�N�g

    public receiver server;

    // ������
    void Start()
    {
    }

    // �X�V
    void Update()
    {
        // �I�u�W�F�N�g����Text�R���|�[�l���g���擾
        Text score_text = score_object.GetComponent<Text>();

        float dtime = 0;

        if (server.DwellTarget) dtime = server.DwellTarget.GetComponent<target_para_set>().dtime;

        // �e�L�X�g�̕\�������ւ���
        score_text.text = dtime.ToString();
    }
}