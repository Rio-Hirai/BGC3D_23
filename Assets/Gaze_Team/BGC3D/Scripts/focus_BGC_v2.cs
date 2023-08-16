using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class focus_BGC_v2 : MonoBehaviour
{
    private FocusInfo FocusInfo;
    private readonly float MaxDistance = 20;
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
    private static EyeData_v2 eyeData = new EyeData_v2();
    private bool eye_callback_registered = false;

    public receiver script; // �T�[�o�[�ڑ�
    public GameObject pointer; // �|�C���^
    public GameObject objectName_now; // ���݂̃^�[�Q�b�g
    public GameObject objectName_new; // �V�����^�[�Q�b�g

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
        if (script.test_id == 6)
        {
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

            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay;
                int dart_board_layer_id = LayerMask.NameToLayer("Targets");
                bool eye_focus;
                if (eye_callback_registered)
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                else
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));

                if (eye_focus)
                {
                    pointer.transform.position = FocusInfo.point;
                    if (FocusInfo.collider.gameObject != null)
                    {
                        objectName_new = FocusInfo.collider.gameObject;
                        script.RayTarget = objectName_new;
                    }
                    break;
                }
                else
                {
                    pointer.transform.position = new Vector3(0, 0, 0);
                }
            }

            // �I�u�W�F�N�g�I��
            if (objectName_now != objectName_new) script.DwellTarget = objectName_new; //�������Ă���I�u�W�F�N�g���X�V

            if (objectName_new != null) // �I�u�W�F�N�g����łȂ��C���������C�L���X�g���[�h�̏ꍇ
            {
                if (script.DwellTarget == objectName_new)
                {
                    objectName_new.GetComponent<target_para_set>().dtime += Time.deltaTime; // �������̃I�u�W�F�N�g�̑��A���������Ԃ�ǉ�
                    objectName_now = objectName_new; //�������Ă���I�u�W�F�N�g���X�V
                }
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