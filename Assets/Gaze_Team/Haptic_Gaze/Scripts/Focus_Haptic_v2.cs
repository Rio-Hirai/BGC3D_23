//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ViveSR.anipal.Eye
{
    public class Focus_Haptic_v2 : MonoBehaviour
    {
        private FocusInfo FocusInfo;
        private readonly float MaxDistance = 20;
        private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
        private static EyeData_v2 eyeData = new EyeData_v2();
        private bool eye_callback_registered = false;
        public GameObject pointer;                              // ポインタ
        [SerializeField] private string tagName = "Targets";    // 注視可能対象の選定．インスペクタで変更可能
        [SerializeField] private Server_Haptic server;

        private void Start()
        {
            if (!SRanipal_Eye_Framework.Instance.EnableEye)
            {
                enabled = false;
                return;
            }
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

            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay;
                int dart_board_layer_id = LayerMask.NameToLayer(tagName);
                bool eye_focus;

                if (eye_callback_registered)
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                else
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));

                if (eye_focus)
                {
                    DartBoard dartBoard = FocusInfo.transform.GetComponent<DartBoard>();

                    pointer.transform.position = FocusInfo.point; // ポインタオブジェクトの位置を更新
                    server.Haptic_Feedback = true;

                    if (dartBoard != null) dartBoard.Focus(FocusInfo.point);
                    break;
                }
                else
                {
                    pointer.transform.position = new Vector3(0, 0, 0);
                    server.Haptic_Feedback = false;
                }
            }
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
