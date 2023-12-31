//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.IO;
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
            public class gaze_data_v2 : MonoBehaviour
            {
                private static EyeData_v2 eyeData = new EyeData_v2();
                private static VerboseData VerboseData = new VerboseData();
                private bool eye_callback_registered = false;

                [SerializeField]
                private receiver Server; // サーバー接続

                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }

                    //csv_results = File.AppendText("/Gaze_Team/BGC3D/Scripts/test_results/eyedata.csv");
                    SRanipal_Eye.WrapperRegisterEyeDataCallback(
                        Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                }

                private void Update()
                {
                }

                public string get_gaze_data2()
                {
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING && SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return null;

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

                    // 瞼の開き具合
                    float leftopeness, rightopness;
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.LEFT, out leftopeness, eyeData))
                        {
                        }
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.LEFT, out leftopeness, eyeData))
                        {
                        }
                    }
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.RIGHT, out rightopness, eyeData))
                        {
                        }
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetEyeOpenness(EyeIndex.RIGHT, out rightopness, eyeData))
                        {
                        }
                    }

                    // 瞳孔位置
                    Vector2 left_pupilpos, right_pupilpos;
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out left_pupilpos, eyeData))
                        {
                        }
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetPupilPosition(EyeIndex.LEFT, out left_pupilpos, eyeData))
                        {
                        }
                    }
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out right_pupilpos, eyeData))
                        {
                        }
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetPupilPosition(EyeIndex.RIGHT, out right_pupilpos, eyeData))
                        {
                        }
                    }

                    // 視線情報
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetVerboseData(out VerboseData, eyeData))
                        {
                        }
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetVerboseData(out VerboseData, eyeData))
                        {
                        }
                    }

                    // 視線情報
                    Ray CombineRay;
                    FocusInfo CombineFocus;
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                        {
                        }
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                        {
                        }
                    }

                    return (Server.test_time + "," + (Server.task_num) + "," + (CombineFocus.point.x) + "," + (CombineFocus.point.y) + "," + (VerboseData.right.pupil_diameter_mm) + "," + (VerboseData.left.pupil_diameter_mm) + "," + (rightopness) + "," + (leftopeness) + "," + (Server.HMDRotation.x) + "," + (Server.HMDRotation.y) + "," + (Server.HMDRotation.z) + "," + (Server.lightValue));
                }

                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
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