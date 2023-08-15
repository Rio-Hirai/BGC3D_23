//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ViveSR.anipal.Eye
{
    public class focus_v1 : MonoBehaviour
    {
        private FocusInfo FocusInfo;
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        private static EyeData eyeData = new EyeData();
        private bool eye_callback_registered = false;

        public receiver script; // �T�[�o�[�ڑ�
        public GameObject pointer;
        public GameObject objectName_now;
        public GameObject objectName_new;
        public GameObject Lens_camera;

        private void Start()
        {
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
                return;
            }
        }

        private void Update()
        {
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                eye_callback_registered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }

            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay;
                int dart_board_layer_id = LayerMask.NameToLayer("Targets");
                bool eye_focus;
                if (eye_callback_registered)
                    eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                else
                    eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));

                if (eye_focus)
                {
                    pointer.transform.position = FocusInfo.point;
                    if (FocusInfo.collider.gameObject != null)
                    {
                        objectName_new = FocusInfo.collider.gameObject;
                        script.RayTarget = objectName_new;
                        //objectName_new.GetComponent<Renderer>().material.color = Color.blue;
                    }
                    break;
                } else
                {
                    pointer.transform.position = new Vector3(0,0,0);
                }
            }

            // �I�u�W�F�N�g�I��
            if ((objectName_now != objectName_new) && (script.test_id == 3)) // �������Ă���I�u�W�F�N�g���قȂ�C���������C�L���X�g���[�h�̏ꍇ
            {
                script.same_target = false;
                script.select_target_id = -1;
                if (script.total_DwellTime_mode == false) objectName_new.GetComponent<target_para_set>().dtime = 0; //�������Ԃ�������
                //objectName_now.GetComponent<Renderer>().material.color = Color.white;
                //objectName_new.GetComponent<Renderer>().material.color = Color.yellow;
                script.selecting_target = objectName_new; //�������Ă���I�u�W�F�N�g���X�V
                script.lens_flag = true;
                script.lens_flag2 = true;
            }

            if ((objectName_new != null) && (script.test_id == 3)) // �I�u�W�F�N�g����łȂ��C���������C�L���X�g���[�h�̏ꍇ
            {
                objectName_new.GetComponent<target_para_set>().dtime += Time.deltaTime; // �������̃I�u�W�F�N�g�̑��A���������Ԃ�ǉ��D

                if (objectName_new.GetComponent<target_para_set>().dtime >= script.set_dtime || script.next_step__flag) //��莞�Ԓ����������I�������������ꍇ
                {
                    script.select_target_id = objectName_new.GetComponent<target_para_set>().Id; //ID���X�V
                    script.next_step__flag = false; //
                    //objectName_new.GetComponent<Renderer>().material.color = script.target_color;
                }

                objectName_now = objectName_new; //�������Ă���I�u�W�F�N�g���X�V

                if (Vector3.Distance(pointer.transform.position, objectName_now.transform.position) > 0.2f)
                {
                    script.lens_flag = false;
                    script.lens_flag2 = false;
                }
            }
        }
        private void Release()
        {
            if (eye_callback_registered == true)
            {
                SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
        }
        private static void EyeCallback(ref EyeData eye_data)
        {
            eyeData = eye_data;
        }
    }
}