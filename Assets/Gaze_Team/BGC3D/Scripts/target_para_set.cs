using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class target_para_set : MonoBehaviour
{
    public GameObject Server;
    private receiver script;
    private results script2;
    public float dtime;
    public int Id;

    void Start()
    {
        script = Server.GetComponent<receiver>();
        script2 = Server.GetComponent<results>();
    }

    void Update()
    {
        if (Server.GetComponent<receiver>().enabled == false)
        {
            // ���ʕ\���p�̏���---------------------------------------------
            for (int i = 1; i < script2.csvDatas.Count; i++)
            {
                if (Id == int.Parse(script2.csvDatas[i][2]))
                {
                    this.GetComponent<Renderer>().material.color = Color.blue;
                }
            }
            //--------------------------------------------------------------

            return;
        }


        // �ݐϒ������ԃ��[�h���̏���----------------------------------
        if (script.total_DwellTime_mode) // �ݐϒ������ԃ��[�h���I���̎�
        {
            if (script.DwellTarget != this.gameObject) // ���̃^�[�Q�b�g�ƒ������̃^�[�Q�b�g���قȂ�ꍇ
            {
                if (dtime > 0) // �݌v�������Ԃ�0���傫���ꍇ
                {
                    dtime -= Time.deltaTime; // �݌v�������Ԃ�񒍎����ԕ����炷
                }
                else
                {
                    dtime = 0; // �݌v�������Ԃ�0�ɂ���
                }
            }

            if(Id != 999 && script.taskflag == false) // ID��999�i�������^�[�Q�b�g�j����^�X�N��Ԃ̏ꍇ
            {
                dtime = 0; // �݌v�������Ԃ�0�ɂ���D���̏����̂����Ŕ�^�X�N���ɗ��K���s���Ȃ����߁C�v���Ǖ���
            }
        }
        else
        {
            if (script.DwellTarget != this.gameObject) // ���̃^�[�Q�b�g�ƒ������̃^�[�Q�b�g���قȂ�ꍇ
            {
                dtime = 0; // �݌v�������Ԃ�0�ɂ���
            }
        }
        //--------------------------------------------------------------


        if (dtime >= script.set_dtime) // �݌v�������Ԃ��ݒ肵�����Ԉȏ�̏ꍇ�C
        {
            script.selecting_target = this.gameObject; // �I�����ꂽ�^�[�Q�b�g���X�V
            script.select_target_id = Id; // �I�����ꂽ�^�[�Q�b�g��ID���X�V
            script.same_target = false; // �H�H�H
        }


        // �F�ω��̏����i�v���t�@�N�^�����O�����j-----------------------
        if (script.output_flag || Id == script.select_target_id)
        {
            this.GetComponent<Renderer>().material.color = script.target_color; //
        }
        else if (Id == 999)
        {
            this.GetComponent<Renderer>().material.color = Color.black; // �^�[�Q�b�g�����F�ɕύX
        }
        else if (script.DwellTarget != null)
        {
            if (script.DwellTarget.name == this.name)
            {
                this.GetComponent<Renderer>().material.color = script.select_color; // �^�[�Q�b�g�̐F��ύX
            }
            else if (Id == script.tasknums[script.task_num] && script.taskflag)
            {
                this.GetComponent<Renderer>().material.color = Color.blue; // �^�[�Q�b�g��F�ɕύX
            }
            else
            {
                this.GetComponent<Renderer>().material.color = Color.white; // �^�[�Q�b�g�𔒐F�ɕύX
            }
        }
        else if (Id == script.tasknums[script.task_num] && script.taskflag)
        {
            this.GetComponent<Renderer>().material.color = Color.blue; // �^�[�Q�b�g��F�ɕύX
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.white; // �^�[�Q�b�g�𔒐F�ɕύX
        }







        //if (script.controller_switch)
        //{
        //    if (script.output_flag) // �Z�b�V�����I����
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color;
        //    }
        //    else if (Id == script.tasknums[script.task_num] && script.taskflag) // �񎦂����^�[�Q�b�g��ID�������C���^�X�N���̏ꍇ
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.blue; // �^�[�Q�b�g��F�ɕύX
        //    }
        //    else if (Id == 999) // �^�[�Q�b�g�����F��ID�����ꍇ
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.black; // �^�[�Q�b�g�����F�ɕύX
        //    }
        //    else if (script.DwellTarget != null) // �������̃^�[�Q�b�g�������ꍇ
        //    {
        //        if (script.DwellTarget.GetComponent<target_para_set>().Id == Id) // �������̃^�[�Q�b�g��ID���X�V
        //        {
        //            this.GetComponent<Renderer>().material.color = script.select_color; // �^�[�Q�b�g�̐F��ύX
        //        }
        //    }
        //    else
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.white; // �^�[�Q�b�g�𔒐F�ɕύX
        //    }
        //}
        //else
        //{
        //    if (script.output_flag) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color; //
        //    }
        //    else if (Id == script.select_target_id) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color; //
        //    }
        //    else if (script.DwellTarget == null) //
        //    {
        //        if (script.taskflag == false && Id != 999) //
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.white; //
        //        }
        //        else if (Id == script.select_target_id) //
        //        {
        //            this.GetComponent<Renderer>().material.color = script.target_color; //
        //        }
        //        else if (Id == script.tasknums[script.task_num]) //
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.blue; //
        //        }
        //        else if (Id == 999) //
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.black; //
        //        }
        //        else
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.white; //
        //        }
        //    }
        //    else if (script.DwellTarget.GetComponent<target_para_set>().Id == Id) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.select_color; //
        //    }
        //    else if (script.taskflag == false && Id != 999) //
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.white; //
        //    }
        //    else if (Id == script.select_target_id) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color; //
        //    }
        //    else if (Id == script.tasknums[script.task_num]) //
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.blue; //
        //    }
        //    else if (Id == 999) //
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.black; //
        //    }
        //    else
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.white; //
        //    }
        //}
    }

    private void OnTriggerEnter(Collider collider)
    {
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider collider)
    {
    }
}
