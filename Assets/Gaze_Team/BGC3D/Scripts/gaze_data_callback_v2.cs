using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class gaze_data_callback_v2 : MonoBehaviour
{
    private static EyeData_v2 eyeData = new EyeData_v2();
    private static bool eye_callback_registered = false;

    public receiver server;

    private Ray CombineRay;
    private FocusInfo CombineFocus;


    private void Update()
    {
        // この部分のコードを実行するとコールバック関数が実行される-----
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


        // 視線情報-----------------------------------------------------
        if (eye_callback_registered)
        {
            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/)) { }
        }
        else
        {
            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/)) { }
        }
        //--------------------------------------------------------------
    }

    private void OnDisable()
    {
        Release();
    }

    void OnApplicationQuit()
    {
        Release();
    }

    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }

    public string get_gaze_data()
    {
        return (server.test_time + "," + (server.task_num) + "," + (CombineFocus.point.x) + "," + (CombineFocus.point.y) + "," + (eyeData.verbose_data.right.pupil_diameter_mm) + "," + (eyeData.verbose_data.left.pupil_diameter_mm) + "," + (eyeData.verbose_data.right.eye_openness) + "," + (eyeData.verbose_data.left.eye_openness) + "," + (server.HMDRotation.x) + "," + (server.HMDRotation.y) + "," + (server.HMDRotation.z) + "," + (server.lightValue));
    }

    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        eyeData = eye_data;
    }
}
