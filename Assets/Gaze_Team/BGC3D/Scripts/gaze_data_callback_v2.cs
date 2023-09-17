using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class gaze_data_callback_v2 : MonoBehaviour
{
    private static EyeData_v2 eyeData = new EyeData_v2();   // 各種視線情報を格納する変数
    private static bool eye_callback_registered = false;    // callback関係
    private FocusInfo CombineFocus;                         // 視線方向の情報を格納する変数

    [SerializeField] private receiver server;               // サーバと接続


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


        // 視線方向に関する情報-----------------------------------------
        Ray CombineRay; // レイ情報を格納する変数を定義
        if (eye_callback_registered) // ？？？
        {
            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/)) { } // 視線方向に関する情報を取得
        }
        else // ？？？
        {
            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/)) { } // 視線の方向に関する情報を取得
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

    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        eyeData = eye_data; // 各種視線情報を更新
    }

    public string get_gaze_data() // 各種視線情報をcsv形式の文字列で出力するための関数．このスクリプトがアタッチされているスクリプトであれば使用可能
    {
        return (server.test_time + "," + (server.task_num + 1) + "," + (server.tasknums[server.task_num]) + "," + (server.taskObject.transform.localPosition.x) + "," + (server.taskObject.transform.localPosition.y) + "," + (server.taskObject.transform.localPosition.z) + "," + (CombineFocus.point.x) + "," + (CombineFocus.point.y) + "," + (eyeData.verbose_data.right.pupil_diameter_mm) + "," + (eyeData.verbose_data.left.pupil_diameter_mm) + "," + (eyeData.verbose_data.right.eye_openness) + "," + (eyeData.verbose_data.left.eye_openness) + "," + (server.HMDRotation.x) + "," + (server.HMDRotation.y) + "," + (server.HMDRotation.z) + "," + (server.lightValue) + "," + (server.center_flag));
    }
}
