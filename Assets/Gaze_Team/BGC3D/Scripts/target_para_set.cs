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
    public GameObject Server;   // �T�[�o�Ɛڑ�
    private receiver script;    // �T�[�o�Ɛڑ�
    private results script2;    // �T�[�o�Ɛڑ�
    public float dtime;         // ��������
    public int Id;              // �^�[�Q�b�gID


    void Start()
    {
        script = Server.GetComponent<receiver>(); // �T�[�o�Ɛڑ�

        if (script.target_size_mini_switch) // �^�[�Q�b�g�̏k�����@�\���I���̏ꍇ
        {
            float distance_of_camera_to_target = Vector3.Distance(script.head_obj.transform.position, this.transform.position); // ���[�U�ƃ^�[�Q�b�g�Ԃ̋������v�Z
            float angleRadians = 1.0f * Mathf.Deg2Rad; // �p�x�����W�A���ɕϊ�
            float height = (Mathf.Tan(angleRadians) * distance_of_camera_to_target); // �^�[�Q�b�g�̍ŏ��T�C�Y���v�Z
            this.transform.localScale = new Vector3(height, height, height); // �^�[�Q�b�g�̃T�C�Y��ύX
        }

        if (script.target_a_id != 99) // �^�[�Q�b�g�p�^�[�����u�����_���v�̏ꍇ
        {
            if (this.GetComponent<target_size_set>()) // �^�[�Q�b�g��target_size_set�������Ă���ꍇ
            {
                this.GetComponent<target_size_set>().enabled = false; // target_size_set���I�t
            }
        }
    }

    void Update()
    {
        if (Server.GetComponent<receiver>().enabled == false) // �T�[�o���I�t�̏ꍇ
        {
            // ���ʕ\���p�̏���---------------------------------------------
            for (int i = 1; i < script2.csvDatas.Count; i++) // �H�H�H
            {
                if (Id == int.Parse(script2.csvDatas[i][2])) this.GetComponent<Renderer>().material.color = Color.blue; // �H�H�H
            }
            //--------------------------------------------------------------

            return; // �I��
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
            else if (Id == 999 && script.taskflag == true) // �H�H�H
            {
                dtime = 0; // �݌v�������Ԃ�0�ɍX�V
            }
        }
        else
        {
            if (script.DwellTarget != this.gameObject) // ���̃^�[�Q�b�g�ƒ������̃^�[�Q�b�g���قȂ�ꍇ
            {
                dtime = 0; // �݌v�������Ԃ�0�ɍX�V
            }
        }
        //--------------------------------------------------------------


        // ������Ԃ���I����Ԃւ̈ڍs---------------------------------
        if (dtime >= script.set_dtime) // �݌v�������Ԃ��ݒ肵�����Ԉȏ�̏ꍇ
        {
            script.selecting_target = this.gameObject; // �I�����ꂽ�^�[�Q�b�g���X�V
            script.select_target_id = Id; // �I�����ꂽ�^�[�Q�b�g��ID���X�V
            script.same_target = false; // �H�H�H
        }
        //--------------------------------------------------------------


        // �^�X�N�őI�������^�[�Q�b�g�̏����擾���鏈��---------------------
        if (script.gaze_data_switch) // �����f�[�^�̏o�͋@�\���I���̏ꍇ
        {
            if (script.tasknums[script.task_num] == Id) // �^�X�N�Œ񎦂���Ă���^�[�Q�b�g��ID�Ɠ���ID�������Ă���ꍇ
            {
                script.taskObject = this.gameObject; // �^�X�N�Œ񎦂���Ă���^�[�Q�b�g���X�V
            }
        }
        //--------------------------------------------------------------


        // �F�ω��̏����i���t�@�N�^�����O�ς݁j----------------------------
        float color_correction = 0.0f; // ���x�␳�p�̕ϐ����`
        if (script.bright_correction_mode) color_correction = script.Brightness / 160.0f; // ���x�␳���[�h���I���Ȃ疾�x�␳�p�̃Q�C�����i�[

        if (script.output_flag || Id == script.select_target_id) // �������ʂ��o�͂��ꂽ�C�܂��I����Ԃ̃^�[�Q�b�g��ID�Ɠ���ID�������Ă���ꍇ
        {
            // this.GetComponent<Renderer>().material.color = script.target_color; // �^�[�Q�b�g�̐F��ύX
            this.GetComponent<Renderer>().material.color = new Color(color_correction, 1, color_correction); // �^�[�Q�b�g�̐F��ύX
        }
        else if (Id == 999) // ID��999�i�������^�[�Q�b�g�j�̏ꍇ
        {
            if (script.DwellTarget != null && script.DwellTarget.name == this.name)
            {
                // this.GetComponent<Renderer>().material.color = script.select_color; // �^�[�Q�b�g�̐F��ύX
                this.GetComponent<Renderer>().material.color = new Color(script.select_color.r, script.select_color.g + color_correction * (1.0f - script.select_color.g), color_correction); // �^�[�Q�b�g�̐F��ύX
            }
            else
            {
                // this.GetComponent<Renderer>().material.color = Color.black; // �^�[�Q�b�g�����F�ɕύX
                this.GetComponent<Renderer>().material.color = new Color(color_correction, color_correction, color_correction); // �^�[�Q�b�g�̐F��ύX
            }
        }
        else if (script.DwellTarget != null) // ������Ԃ̃^�[�Q�b�g�����݂��Ȃ��ꍇ
        {
            if (script.DwellTarget.name == this.name) // ������Ԃ̃^�[�Q�b�g�̖��O�Ɠ������O�̏ꍇ�D���O����ID�̕����܂���Ӑ���S�ۂł���̂ŗv���t�@�N�^�����O
            {
                // this.GetComponent<Renderer>().material.color = script.select_color; // �^�[�Q�b�g�̐F��ύX
                this.GetComponent<Renderer>().material.color = new Color(script.select_color.r, script.select_color.g + color_correction * (1.0f - script.select_color.g), color_correction); // �^�[�Q�b�g�̐F��ύX
            }
            else if (script.target_p_id != 99 && Id == script.tasknums[script.task_num] && script.taskflag) // ��ID�Ɠ���ID�������Ă���ꍇ
            {
                // this.GetComponent<Renderer>().material.color = Color.blue; // �^�[�Q�b�g��F�ɕύX
                this.GetComponent<Renderer>().material.color = new Color(color_correction, color_correction, 1); // �^�[�Q�b�g�̐F��ύX
            }
            else
            {
                this.GetComponent<Renderer>().material.color = Color.white; // �^�[�Q�b�g�𔒐F�ɕύX
            }
        }
        else if (script.target_p_id != 99 && Id == script.tasknums[script.task_num] && script.taskflag) // ��ID�Ɠ���ID�������Ă���ꍇ
        {
            // this.GetComponent<Renderer>().material.color = Color.blue; // �^�[�Q�b�g��F�ɕύX
            this.GetComponent<Renderer>().material.color = new Color(color_correction, color_correction, 1); // �^�[�Q�b�g�̐F��ύX
        }
        else // �ȏ�̏����ɊY�����Ȃ��ꍇ
        {
            this.GetComponent<Renderer>().material.color = Color.white; // �^�[�Q�b�g�𔒐F�ɕύX
        }
        //--------------------------------------------------------------
    }
}
