//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_GazeRay_v2 : MonoBehaviour
            {
                public int LengthOfRay = 25;                            // ���C�̍ő咷
                [SerializeField] private LineRenderer GazeRayRenderer;  // ���C�̐F
                [SerializeField] private Gradient _gradient;            // ���C�̐F
                [SerializeField] private Gradient _gradient0;           // ���C�̐F�i�����j
                private static EyeData_v2 eyeData = new EyeData_v2();   // �e�펋�������i�[����ϐ�
                private bool eye_callback_registered = false;           // callback�֌W

                public GameObject hit_point;                            // �|�C���^
                public GameObject objectName_now;                       // ���݂̃^�[�Q�b�g
                public GameObject objectName_new;                       // �V�����^�[�Q�b�g
                [SerializeField] private receiver script;               // �T�[�o�ڑ�


                // ���C�Z�o�p---------------------------------------------------
                [System.NonSerialized] public Vector3 ray0;             // �����̎n�_�x�N�g��
                [System.NonSerialized] public Vector3 ray1;             // �����̕����x�N�g��
                //--------------------------------------------------------------


                private void Start()
                {
                    // SRanipal_Eye_Framework������ɓ����Ă��Ȃ��ꍇ�̏���---------
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    //--------------------------------------------------------------

                    Assert.IsNotNull(GazeRayRenderer);

                    GazeRayRenderer.material = new Material(Shader.Find("Sprites/Default"));


                    // ���C�̕\������------------------------------------------------
                    if (script.raycast_switch)
                    {
                        GazeRayRenderer.colorGradient = _gradient; // ���C�̐F�𔒂ɕύX
                    }
                    else
                    {
                        GazeRayRenderer.colorGradient = _gradient0; // ���C�̐F�𓧖��ɕύX
                    }
                    //--------------------------------------------------------------
                }

                private void Update()
                {
                    // ���������擾-----------------------------------------------
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

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


                    // �����̕��������擾-----------------------------------------
                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData))
                        {
                        }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData))
                        {
                        }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData))
                        {
                        }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
                        {
                        }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
                        {
                        }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
                        {
                        }
                        else return;
                    }
                    //--------------------------------------------------------------


                    // ���C�̕`��---------------------------------------------------
                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal); // �H�H�H
                    GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f); // �H�H�H
                    GazeRayRenderer.SetPosition(1, (Camera.main.transform.position + Camera.main.transform.up * 0.085f) + GazeDirectionCombined * LengthOfRay); // �H�H�H

                    ray0 = Camera.main.transform.position - Camera.main.transform.up * 0.05f; // �H�H�H
                    ray1 = GazeDirectionCombined; // �H�H�H
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
    }
}
