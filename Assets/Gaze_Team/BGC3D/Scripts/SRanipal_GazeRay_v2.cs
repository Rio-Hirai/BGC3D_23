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
                public int LengthOfRay = 25;                            // ？？？
                [SerializeField] private LineRenderer GazeRayRenderer;  // ？？？
                [SerializeField] private Gradient _gradient;            // ？？？
                private static EyeData_v2 eyeData = new EyeData_v2();   // ？？？
                private bool eye_callback_registered = false;           // ？？？

                public float radius = 5.0f;                             // ？？？
                public float maxradius = 5.0f;                          // ？？？
                public GameObject hit_point;                            // ？？？
                public GameObject objectName_now;                       // ？？？
                public GameObject objectName_new;                       // ？？？
                public GameObject Lens_camera;                          // ？？？
                public GameObject Lens_rotation;                        // ？？？
                [SerializeField] private receiver script;               // サーバー接続

                // 移動平均フィルター関係---------------------------------------
                private int window = 25;                                // 窓の大きさ
                private int[] count = new int[5];                       // 新しい値の格納場所
                private Vector3[,] hit_position = new Vector3[5, 25];   // ？？？
                private Vector3 hit_position_fil;                       // ？？？
                //--------------------------------------------------------------

                // レイ算出用---------------------------------------------------
                [System.NonSerialized] public Vector3 ray0;             // ？？？
                [System.NonSerialized] public Vector3 ray1;             // ？？？
                //--------------------------------------------------------------


                private void Start()
                {
                    // SRanipal_Eye_Frameworkが正常に動いていない場合の処理---------
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    //--------------------------------------------------------------

                    Assert.IsNotNull(GazeRayRenderer);

                    GazeRayRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    GazeRayRenderer.colorGradient = _gradient;
                }

                private void Update()
                {
                    // 視線情報を取得-----------------------------------------------
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


                    // 視線の方向情報を取得-----------------------------------------
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


                    // レイの描画---------------------------------------------------
                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    GazeRayRenderer.SetPosition(1, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);

                    ray0 = Camera.main.transform.position - Camera.main.transform.up * 0.05f;
                    ray1 = GazeDirectionCombined;
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
                    eyeData = eye_data;
                }
            }
        }
    }
}
