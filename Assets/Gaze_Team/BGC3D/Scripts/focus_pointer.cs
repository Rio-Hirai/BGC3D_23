using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class focus_pointer : MonoBehaviour
{
    private FocusInfo FocusInfo;                     // �H�H�H
    private readonly float MaxDistance = 20;         // �H�H�H
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
    private static EyeData eyeData = new EyeData();  // �H�H�H
    private bool eye_callback_registered = false;    // �H�H�H

    public GameObject pointer;                       // �H�H�H

    // �T�[�o�[�ڑ�
    public GameObject Server;                        // �H�H�H
    //public GameObject EyePoint_sub;
    private receiver script;                         // �H�H�H

    private void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye) // �H�H�H
        {
            enabled = false; // �H�H�H
            return; // �H�H�H
        }

        script = Server.GetComponent<receiver>(); // �H�H�H
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
            Ray GazeRay; // �H�H�H
            int dart_board_layer_id = LayerMask.NameToLayer("Targets"); // �H�H�H
            bool eye_focus; // �H�H�H

            if (eye_callback_registered)
                eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
            else
                eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));

            if (eye_focus)
            {
                pointer.transform.position = FocusInfo.point; // �H�H�H
                break;
            }
            else
            {
                pointer.transform.position = new Vector3(0, 0, 0); // �H�H�H
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
        eyeData = eye_data; // �e�펋�������X�V
    }
}