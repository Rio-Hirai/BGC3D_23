//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ViveSR.anipal.Eye
{
    public class focus_v2 : MonoBehaviour
    {
        private FocusInfo FocusInfo;                            // ���C�i�����j�ƏՓ˂����^�[�Q�b�g�̏����i�[����ϐ�
        private readonly float MaxDistance = 20;                // ���C�̍ő咷
        private static EyeData_v2 eyeData = new EyeData_v2();   // �e�펋�������i�[����ϐ�
        private bool eye_callback_registered = false;           // callback�֌W
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT }; // �H�H�H

        public receiver script;                                 // �T�[�o�ڑ�
        public GameObject pointer;                              // �|�C���^
        public GameObject objectName_now;                       // ���݂̃^�[�Q�b�g
        public GameObject objectName_new;                       // �V�����^�[�Q�b�g
        [SerializeField] private string tagName = "Targets";    // �����\�Ώۂ̑I��D�C���X�y�N�^�ŕύX�\

        private void Start()
        {
            // SRanipal_Eye_Framework������ɓ����Ă��Ȃ��ꍇ�̏���---------
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false; // �H�H�H
                return; // �H�H�H
            }
            //--------------------------------------------------------------
        }

        private void Update()
        {
            // ���������擾-----------------------------------------------
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING && SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
            //--------------------------------------------------------------


            // �I�u�W�F�N�g�I��---------------------------------------------
            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay; // ���C�̏��
                bool eye_focus; // ���C�ƏՓ˂��Ă���^�[�Q�b�g�̗L��
                int dart_board_layer_id = LayerMask.NameToLayer(tagName); // �w�肵�����C���̔ԍ����擾


                // �����̕��������擾-----------------------------------------
                if (eye_callback_registered)
                {
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                }
                else
                {
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));
                }
                //--------------------------------------------------------------


                // ���C�ƃ^�[�Q�b�g�̏Փˏ���-----------------------------------
                if (eye_focus) // ���C�ɏՓ˂��Ă���^�[�Q�b�g�����݂���ꍇ
                {
                    pointer.transform.position = FocusInfo.point; // �|�C���^�I�u�W�F�N�g�̈ʒu���X�V
                    objectName_new = FocusInfo.collider.gameObject; // ���C�ƏՓ˂��Ă���^�[�Q�b�g��ϐ��Ɋi�[
                    script.RayTarget = objectName_new; // ���C�ƏՓ˂��Ă���^�[�Q�b�g���X�V
                    script.DwellTarget = objectName_new; //�������Ă���I�u�W�F�N�g���X�V
                }
                else
                {
                    pointer.transform.position = new Vector3(0, 0, 0); // �|�C���^�I�u�W�F�N�g�̈ʒu��������
                    objectName_new = null;
                    script.RayTarget = null;
                    script.DwellTarget = null;
                }
                //--------------------------------------------------------------
            }
            //--------------------------------------------------------------


            // �I�u�W�F�N�g�I��---------------------------------------------
            if ((objectName_new != null) && (objectName_now != objectName_new) && (script.test_id == 3)) // �������Ă���I�u�W�F�N�g���قȂ�C���g�p��@���uGaze_Raycast�v�̏ꍇ
            {
                script.same_target = false; // �H�H�H
                script.selecting_target = null; // �I������Ă���^�[�Q�b�g��������
                script.select_target_id = -1; // �I������Ă���^�[�Q�b�g��ID��������
                if (script.total_DwellTime_mode == false) objectName_new.GetComponent<target_para_set>().dtime = 0; // �������Ԃ�������
                script.DwellTarget = objectName_new; //�������Ă���I�u�W�F�N�g���X�V
            }

            if ((objectName_new != null) && (script.test_id == 3)) // �I�u�W�F�N�g����łȂ��C���g�p��@���uGaze_Raycast�v�̏ꍇ
            {
                objectName_new.GetComponent<target_para_set>().dtime += Time.deltaTime; // �������̃I�u�W�F�N�g�̑��A���������Ԃ�ǉ�
                objectName_now = objectName_new; //�������Ă���I�u�W�F�N�g���X�V
            }
            //--------------------------------------------------------------


            //--------------------------------------------------------------
            if (script.pointer_switch)
            {
                pointer.SetActive(true); // �H�H�H
            }
            else
            {
                pointer.SetActive(false); // �H�H�H
            }
            //--------------------------------------------------------------
        }

        private void Release()
        {
            if (eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
        }

        private static void EyeCallback(ref EyeData_v2 eye_data)
        {
            eyeData = eye_data; // �e�펋�������X�V
        }
    }
}