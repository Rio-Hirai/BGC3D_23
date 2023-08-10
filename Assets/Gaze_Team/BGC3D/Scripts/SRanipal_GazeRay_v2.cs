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
                public int LengthOfRay = 25;
                [SerializeField] private LineRenderer GazeRayRenderer;
                [SerializeField] private Gradient _gradient;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;

                //private Ray ray;
                //private FocusInfo focusInfo;
                public float radius = 5.0f;
                public float maxradius = 5.0f;
                //private float LeftOpenness, RightOpenness;
                public GameObject hit_point;
                public GameObject objectName_now;
                public GameObject objectName_new;
                public GameObject Lens_camera;

                public GameObject Lens_rotation;

                //private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };

                // �ړ����σt�B���^�[�֌W
                private int window = 25; // ���̑傫��
                private int[] count = new int[5]; // �V�����l�̊i�[�ꏊ
                private Vector3[,] hit_position = new Vector3[5, 25];
                private Vector3 hit_position_fil;

                // �T�[�o�[�ڑ�
                public receiver script;

                // ���C�Z�o�p
                public Vector3 ray0;
                public Vector3 ray1;

                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }

                    Assert.IsNotNull(GazeRayRenderer);

                    GazeRayRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    GazeRayRenderer.colorGradient = _gradient;
                    //GazeRayRenderer.startColor = Color.red;
                    //GazeRayRenderer.endColor = Color.green;
                }

                private void Update()
                {
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
                            //// �K�v�ȕϐ����`
                            //Vector3 GazeDirectionCombined2 = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                            //RaycastHit hit;

                            // �ړ����σt�B���^�[��K�p
                            //GazeDirectionCombined2 = Move_filter2(GazeDirectionCombined2, 0);
                            //Vector3 CameraMainTransform2 = Move_filter2(Camera.main.transform.position, 1);

                            //// �ړ����σt�B���^�[��K�p
                            //Vector3 CameraMainTransform2 = Camera.main.transform.position;

                            //// �I�u�W�F�N�g�̃��C���[�w��
                            //int layerMask = 7 << 10;

                            // ���C�̏Փ˔���
                            //if (Physics.Raycast((Camera.main.transform.position - Camera.main.transform.up * 0.05f), (Camera.main.transform.position + (Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal)) * LengthOfRay), out hit, Mathf.Infinity, layerMask))
                            //{
                            //    string objectName = hit.collider.gameObject.name;
                            //    hit_point.transform.position = hit.point;

                            //    objectName_new = hit.collider.gameObject;
                            //}

                            //// �I�u�W�F�N�g�I��
                            //if (objectName_now != objectName_new)
                            //{
                            //    script.same_target = false;
                            //    script.select_target_id = -1;
                            //    objectName_new.GetComponent<target_para_set>().dtime = 0;
                            //    //objectName_now.GetComponent<Renderer>().material.color = Color.white;
                            //    //objectName_new.GetComponent<Renderer>().material.color = Color.yellow;
                            //    script.selecting_target = objectName_new;
                            //    script.lens_flag = true;
                            //    script.lens_flag2 = true;
                            //}
                            //objectName_new.GetComponent<target_para_set>().dtime += Time.deltaTime;
                            //if (objectName_new.GetComponent<target_para_set>().dtime >= script.set_dtime)
                            //{
                            //    script.select_target_id = objectName_new.GetComponent<target_para_set>().Id;
                            //    //objectName_new.GetComponent<Renderer>().material.color = script.target_color;
                            //}

                            //objectName_now = objectName_new;

                            //if (Vector3.Distance(hit_point.transform.position, objectName_now.transform.position) > 0.2f)
                            //{
                            //    script.lens_flag = false;
                            //    script.lens_flag2 = false;
                            //}
                        }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
                        {
                        }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
                        {
                        }
                        else return;
                    }

                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    GazeRayRenderer.SetPosition(1, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);

                    ray0 = Camera.main.transform.position - Camera.main.transform.up * 0.05f;
                    //ray1 = Camera.main.transform.position + GazeDirectionCombined * LengthOfRay;
                    ray1 = GazeDirectionCombined;

                    //// �œ_�����킹��
                    //foreach (GazeIndex index in GazePriority)
                    //{
                    //    Ray GazeRay;
                    //    int dart_board_layer_id = LayerMask.NameToLayer("NoReflection");
                    //    bool eye_focus;
                    //    if (eye_callback_registered)
                    //        eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                    //    else
                    //        eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));

                    //    if (eye_focus)
                    //    {
                    //        DartBoard dartBoard = FocusInfo.transform.GetComponent<DartBoard>();
                    //        if (dartBoard != null) dartBoard.Focus(FocusInfo.point);
                    //        break;
                    //    }
                    //}
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
                    eyeData = eye_data;
                }

                // �ړ����σt�B���^�[
                public Vector3 Move_filter2(Vector3 position, int coll)
                {
                    if (count[coll] > window - 1)
                    {
                        count[coll] = 0;
                    }

                    hit_position[coll, count[coll]] = position;
                    hit_position_fil = new Vector3(0f, 0f, 0f);
                    for (int i = 0; i < window; i++)
                    {
                        hit_position_fil += hit_position[coll, i];
                    }
                    count[coll]++;

                    return hit_position_fil /= window;
                }
            }
        }
    }
}
