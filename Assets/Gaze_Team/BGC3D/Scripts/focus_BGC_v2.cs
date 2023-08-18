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
        // SRanipal_Eye_Framework������ɓ����Ă��Ȃ��ꍇ�̏���---------
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
        //--------------------------------------------------------------
    }

    private void Update()
    {
        if (script.test_id == 6)
        {
            // ���������擾-----------------------------------------------
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


            // �I�u�W�F�N�g�I��---------------------------------------------
            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay; // ���C�̏��
                bool eye_focus; // ���C�ƏՓ˂��Ă���^�[�Q�b�g�̗L��
                int dart_board_layer_id = LayerMask.NameToLayer("Targets"); // �w�肵�����C���̔ԍ����擾


                // �����̕��������擾-----------------------------------------
                if (eye_callback_registered)
                {
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                }
                else
                {
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));
                }
                //--------------------------------------------------------------


                // ���C�ƃ^�[�Q�b�g�̏Փˏ���-----------------------------------
                if (eye_focus) // ���C�ɏՓ˂��Ă���^�[�Q�b�g�����݂���ꍇ
                {
                    pointer.transform.position = FocusInfo.point; // �|�C���^�I�u�W�F�N�g�̈ʒu���X�V
                    if (FocusInfo.collider.gameObject != null) // ���C�ɏՓ˂��Ă���^�[�Q�b�g�����݂��Ȃ��ꍇ
                    {
                        objectName_new = FocusInfo.collider.gameObject; // ���C�ƏՓ˂��Ă���^�[�Q�b�g��ϐ��Ɋi�[
                        script.RayTarget = objectName_new; // ���C�ƏՓ˂��Ă���^�[�Q�b�g���X�V
                    }
                    break; // foreach����E�o�i���C�ƏՓ˂��Ă���^�[�Q�b�g�����������߁D�����̃^�[�Q�b�g��������ꍇ�͏��O����j
                }
                else
                {
                    pointer.transform.position = new Vector3(0, 0, 0); // �|�C���^�I�u�W�F�N�g�̈ʒu��������
                }
                //--------------------------------------------------------------
            }
            //--------------------------------------------------------------


            // �I�u�W�F�N�g�I��---------------------------------------------
            if (objectName_now != objectName_new) script.DwellTarget = objectName_new; //�������Ă���I�u�W�F�N�g���X�V

            if (objectName_new != null) // �I�u�W�F�N�g����łȂ��C���������C�L���X�g���[�h�̏ꍇ
            {
                if (script.DwellTarget == objectName_new)
                {
                    objectName_new.GetComponent<target_para_set>().dtime += Time.deltaTime; // �������̃I�u�W�F�N�g�̑��A���������Ԃ�ǉ�
                    objectName_now = objectName_new; //�������Ă���I�u�W�F�N�g���X�V
                }
            }
            //--------------------------------------------------------------
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