using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class gaze_data_callback_v2 : MonoBehaviour
{
    private static EyeData_v2 eyeData = new EyeData_v2();   // �e�펋�������i�[����ϐ�
    private static bool eye_callback_registered = false;    // callback�֌W
    private FocusInfo CombineFocus;                         // ���������̏����i�[����ϐ�

    [SerializeField] private receiver server;               // �T�[�o�Ɛڑ�


    private void Update()
    {
        // ���̕����̃R�[�h�����s����ƃR�[���o�b�N�֐������s�����-----
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


        // ���������Ɋւ�����-----------------------------------------
        Ray CombineRay; // ���C�����i�[����ϐ����`
        if (eye_callback_registered) // �H�H�H
        {
            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/)) { } // ���������Ɋւ�������擾
        }
        else // �H�H�H
        {
            if (SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/)) { } // �����̕����Ɋւ�������擾
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
        eyeData = eye_data; // �e�펋�������X�V
    }

    public string get_gaze_data() // �e�펋������csv�`���̕�����ŏo�͂��邽�߂̊֐��D���̃X�N���v�g���A�^�b�`����Ă���X�N���v�g�ł���Ύg�p�\
    {
        return (server.test_time + "," + (server.task_num + 1) + "," + (server.tasknums[server.task_num]) + "," + (server.taskObject.transform.localPosition.x) + "," + (server.taskObject.transform.localPosition.y) + "," + (server.taskObject.transform.localPosition.z) + "," + (CombineFocus.point.x) + "," + (CombineFocus.point.y) + "," + (eyeData.verbose_data.right.pupil_diameter_mm) + "," + (eyeData.verbose_data.left.pupil_diameter_mm) + "," + (eyeData.verbose_data.right.eye_openness) + "," + (eyeData.verbose_data.left.eye_openness) + "," + (server.HMDRotation.x) + "," + (server.HMDRotation.y) + "," + (server.HMDRotation.z) + "," + (server.lightValue) + "," + (server.center_flag));
    }
}
