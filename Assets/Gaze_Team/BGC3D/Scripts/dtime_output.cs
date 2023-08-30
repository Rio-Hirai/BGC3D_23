using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dtime_output : MonoBehaviour
{
    public GameObject score_object = null;      // Text�I�u�W�F�N�g
    [SerializeField] private receiver server;   // �T�[�o�ڑ�
    private float dtime = 0;                    // �H�H�H
    private string monitor = "";                // �H�H�H


    void Update()
    {
        Text score_text = score_object.GetComponent<Text>(); // �I�u�W�F�N�g����Text�R���|�[�l���g���擾


        //--------------------------------------------------------------
        if (server.DwellTarget) // �H�H�H
        {
            //--------------------------------------------------------------
            if (server.DwellTarget.GetComponent<target_para_set>().dtime > 0) // �H�H�H
            {
                if (server.DwellTarget.GetComponent<target_para_set>().dtime <= server.set_dtime) // �H�H�H
                {
                    dtime = server.DwellTarget.GetComponent<target_para_set>().dtime; // �H�H�H
                    score_object.GetComponent<Text>().color = Color.white; // �H�H�H
                }
                else
                {
                    score_object.GetComponent<Text>().color = server.target_color; // �H�H�H
                }
            }
            //--------------------------------------------------------------


            monitor = dtime.ToString(); // �H�H�H
        }
        else
        {
            monitor = ""; // �H�H�H
        }
        //--------------------------------------------------------------


        //--------------------------------------------------------------
        if (server.dtime_monitor) // �H�H�H
        {
            score_object.SetActive(true); // �H�H�H
        }
        else
        {
            score_object.SetActive(false); // �H�H�H
        }
        //--------------------------------------------------------------


        score_text.text = monitor; // �e�L�X�g�̕\�������ւ���
    }
}