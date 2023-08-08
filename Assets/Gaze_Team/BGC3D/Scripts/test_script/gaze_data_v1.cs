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
            public class gaze_data_v1 : MonoBehaviour
            {
                private static EyeData eyeData = new EyeData();
                private static VerboseData VerboseData = new VerboseData();
                private bool eye_callback_registered = false;

                [SerializeField]
                private receiver Server; // サーバー接続

                StreamWriter csv_results;

                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }

                    //csv_results = File.AppendText("/Gaze_Team/BGC3D/Scripts/test_results/eyedata.csv");
                    SRanipal_Eye.WrapperRegisterEyeDataCallback(
                        Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
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

                    // 瞼の開き具合
                    float leftopeness, rightopness;
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out leftopeness, eyeData))
                        {
                        }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out leftopeness, eyeData))
                        {
                        }
                        else return;
                    }
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out rightopness, eyeData))
                        {
                        }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out rightopness, eyeData))
                        {
                        }
                        else return;
                    }

                    // 瞳孔位置
                    Vector2 left_pupilpos, right_pupilpos;
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye.GetPupilPosition(EyeIndex.LEFT, out left_pupilpos, eyeData))
                        {
                        }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye.GetPupilPosition(EyeIndex.LEFT, out left_pupilpos, eyeData))
                        {
                        }
                        else return;
                    }
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye.GetPupilPosition(EyeIndex.RIGHT, out right_pupilpos, eyeData))
                        {
                        }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye.GetPupilPosition(EyeIndex.RIGHT, out right_pupilpos, eyeData))
                        {
                        }
                        else return;
                    }

                    // 視線情報
                    SRanipal_Eye.GetVerboseData(out VerboseData, eyeData);
                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye.GetVerboseData(out VerboseData, eyeData))
                        {
                        }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye.GetVerboseData(out VerboseData, eyeData))
                        {
                        }
                        else return;
                    }

                    Debug.Log("Data = " + leftopeness + "," + rightopness + "," + left_pupilpos + "," + right_pupilpos);
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
    }
}