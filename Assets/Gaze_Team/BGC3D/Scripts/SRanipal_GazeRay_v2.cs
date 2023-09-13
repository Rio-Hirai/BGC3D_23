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
                public int LengthOfRay = 25;                            // レイの最大長
                [SerializeField] private LineRenderer GazeRayRenderer;  // レイの色
                [SerializeField] private Gradient _gradient;            // レイの色
                [SerializeField] private Gradient _gradient0;           // レイの色（透明）
                private static EyeData_v2 eyeData = new EyeData_v2();   // 各種視線情報を格納する変数
                private bool eye_callback_registered = false;           // callback関係

                public GameObject hit_point;                            // ポインタ
                public GameObject objectName_now;                       // 現在のターゲット
                public GameObject objectName_new;                       // 新しいターゲット
                [SerializeField] private receiver script;               // サーバ接続


                // レイ算出用---------------------------------------------------
                [System.NonSerialized] public Vector3 ray0;             // 視線の始点ベクトル
                [System.NonSerialized] public Vector3 ray1;             // 視線の方向ベクトル
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


                    // レイの表示処理------------------------------------------------
                    if (script.raycast_switch)
                    {
                        GazeRayRenderer.colorGradient = _gradient; // レイの色を白に変更
                    }
                    else
                    {
                        GazeRayRenderer.colorGradient = _gradient0; // レイの色を透明に変更
                    }
                    //--------------------------------------------------------------
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
                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal); // ？？？
                    GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f); // ？？？
                    GazeRayRenderer.SetPosition(1, (Camera.main.transform.position + Camera.main.transform.up * 0.085f) + GazeDirectionCombined * LengthOfRay); // ？？？

                    ray0 = Camera.main.transform.position - Camera.main.transform.up * 0.05f; // ？？？
                    ray1 = GazeDirectionCombined; // ？？？
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
                    eyeData = eye_data; // 各種視線情報を更新
                }
            }
        }
    }
}
