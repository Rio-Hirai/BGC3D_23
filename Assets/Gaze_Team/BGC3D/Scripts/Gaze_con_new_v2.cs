using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.Newtonsoft.Json.Linq;
using Valve.VR.Extras;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class Gaze_con_new_v2 : MonoBehaviour
            {
                [SerializeField] private receiver script;   // �T�[�o�Ɛڑ�
                public SRanipal_GazeRay_BGC_v2 rayset;      // ���C�L���X�g�̃f�[�^���擾
                public MovingAverageFilter filter;          // �ړ����σt�B���^���擾

                [SerializeField]
                private string tagName = "Targets";         // �����\�Ώۂ̑I��D�C���X�y�N�^�ŕύX�\

                public GameObject searchNearObj;            // �ł��߂��I�u�W�F�N�g(public�C���q�ɂ��邱�ƂŊO���̃N���X����Q�Ƃł���)
                public GameObject oldNearObj;               // �O�t���[���ɒ������Ă����^�[�Q�b�g
                public Transform camera_obj;                // ���[�U�̈ʒu
                private float searchWaitTime = 1 / 60;      // �����̑ҋ@����

                private float timer = 0.0f;                 // �����܂ł̑ҋ@���Ԍv���p
                private Vector3 cursor_point;               // �J�[�\���̈ʒu
                private float target_size;                  // �������Ă���^�[�Q�b�g�̑傫��
                private float distance_of_camera_to_target; // ���[�U�ƃ^�[�Q�b�g�Ԃ̋���
                private float color_alpha = 0.45f;          // �J�[�\���̓����x


                void Start()
                {
                    searchNearObj = Serch(); // �����^�[�Q�b�g�����������
                    color_alpha = script.cursor_color.a; // �J�[�\���̓����x��ۑ�
                }

                void Update()
                {
                    // �J�[�\���̈ʒu�Ɣ��a�ƐF���X�V-------------------------------
                    if (script.cursor_switch) // �J�[�\���\���E��펞�X�C�b�`�̗L��
                    {
                        this.GetComponent<Renderer>().material.color = script.cursor_color; // �����x��0���傫�����ăJ�[�\����\��
                    }
                    else
                    {
                        script.cursor_color.a = 0f; // �J�[�\���𓧖����i����\�����j
                        this.GetComponent<Renderer>().material.color = script.cursor_color; // �����x��0�ɂ��ăJ�[�\�����\��
                    }
                    //--------------------------------------------------------------


                    this.transform.position = cursor_point; // �J�[�\���̈ʒu���X�V
                    this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious); // �J�[�\���̑傫�����X�V

                    timer += Time.deltaTime; // ���Ԃ��v��


                    // �����̑ҋ@���Ԃ��o�߂�����i�������d���Ȃ�Ȃ����߂̏����Ȃ��ߏꍇ�ɂ���Ă̓��t�@�N�^�����O�Ώہj
                    if (timer >= searchWaitTime)
                    {
                        searchNearObj = Serch(); // �w�肵���^�O�����I�u�W�F�N�g�̂����C���̃I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g���擾
                        timer = 0; // �v�����Ԃ����������čČ���
                    }
                    //--------------------------------------------------------------
                }

                private GameObject Serch()
                {
                    float nearDistance = 999; // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
                    float cursor_size_limit = distance_of_camera_to_target / 8; // �J�[�\���̑傫���̏����ݒ肷�邽�߂̕ϐ��i�łȂ��Ɩ����ɑ傫���Ȃ�j
                    GameObject searchTargetObj = null; // �������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
                    script.cursor_count = 0; // �o�u���J�[�\�����ɑ��݂���^�[�Q�b�g�̐���������
                    Vector3 ray1 = rayset.ray1; // �����̕����x�N�g����ۑ��i���I�ړ����σt�B���^��p���邽�߁j
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName); // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾

                    if (objs.Length == 0) return searchTargetObj; // �擾�����Q�[���I�u�W�F�N�g��0�Ȃ�null��Ԃ�(null�ł��G���[�ɂȂ�Ȃ��悤�ɏ���)


                    // �����I���W�F�X�`���̏ꍇ�̏���----------------------------------
                    if (script.approve_switch == true && script.head_rot_switch == true)
                    {
                        // ��莞�Ԓ������Ă����ꍇ--------------------------------------
                        if (searchNearObj != null && script.select_flag)
                        {
                            script.select_target_id = searchNearObj.GetComponent<target_para_set>().Id; // �I�������^�[�Q�b�g��ID���X�V�i����ID�����ʂƂ��ďo�͂����j
                            script.next_step__flag = false; // �^�X�N�Ԃ̋x�e��ԂɑJ�ڂ��邽�߂̃t���O���X�V
                            script.select_flag = false;
                            script.same_target = false;
                        }


                        //Bubble Cursor��\��-------------------------------------------
                        if (searchNearObj != null && script.cursor_switch)
                        {
                            Vector3 toObject = searchNearObj.transform.position - rayset.ray0; // �����̎n�_����I�u�W�F�N�g�܂ł̃x�N�g�����v�Z
                            Vector3 projection = Vector3.Project(toObject, ray1.normalized); // �����ɑ΂���I�u�W�F�N�g�̓��e�_���v�Z
                            cursor_point = rayset.ray0 + projection; // ���e�_�����ɒ�����̍ł��߂��_���v�Z
                        }
                        //--------------------------------------------------------------


                        oldNearObj = searchNearObj; // �������Ă���^�[�Q�b�g���X�V
                        script.cursor_radious = (nearDistance * 2) + (target_size); // �J�[�\���̑傫�����X�V

                        return searchNearObj;
                    }
                    //--------------------------------------------------------------


                    // �ړ����σt�B���^�̏���--------------------------------------
                    if (script.MAverageFilter) // �ړ����σt�B���^�̋@�\���I���̏ꍇ
                    {
                        foreach (GameObject obj in objs) // objs����1����obj�Ɏ��o��
                        {
                            Vector3 toObject = obj.transform.position - rayset.ray0; // �����̎n�_����I�u�W�F�N�g�܂ł̃x�N�g�����v�Z
                            Vector3 projection = Vector3.Project(toObject, ray1.normalized); // �����ɑ΂���I�u�W�F�N�g�̓��e�_���v�Z
                            Vector3 closestPointOnRay = rayset.ray0 + projection; // ���e�_�����ɒ�����̍ł��߂��_���v�Z
                            float distance = Vector3.Distance(obj.transform.position, closestPointOnRay); // �^�[�Q�b�g�ƒ�����̍ł��߂��_�Ƃ̋������v�Z
                            // obj.tag = tagName;  // ����̏����̂��߂Ƀ^�O��������

                            if (distance < cursor_size_limit) obj.tag = "near"; // �����̎��ӂɂ���^�[�Q�b�g�̃^�O��ύX

                            if (distance < cursor_size_limit / 2) script.cursor_count++; // �J�[�\�����ɑ��݂���^�[�Q�b�g���J�E���g
                        }

                        if (script.cursor_count > 0) objs = GameObject.FindGameObjectsWithTag("near"); // �^�[�Q�b�g���X�g���X�V�����^�O�����^�[�Q�b�g�݂̂ɍX�V

                        ray1 = filter.filter(rayset.ray1, script.cursor_count); // �����̕����x�N�g���ɓ��I�ړ����σt�B���^��K�p�i���������t�B���^�����O����l�C�����������̑傫���j
                    }
                    //--------------------------------------------------------------


                    // Bubble Cursor------------------------------------------------
                    foreach (GameObject obj in objs) // objs����1����obj�Ɏ��o��
                    {
                        Vector3 toObject = obj.transform.position - rayset.ray0; // �����̎n�_����I�u�W�F�N�g�܂ł̃x�N�g�����v�Z
                        Vector3 projection = Vector3.Project(toObject, ray1.normalized); // �����ɑ΂���I�u�W�F�N�g�̓��e�_���v�Z
                        Vector3 closestPointOnRay = rayset.ray0 + projection; // ���e�_�����ɒ�����̍ł��߂��_���v�Z
                        float distance = Vector3.Distance(obj.transform.position, closestPointOnRay); // �^�[�Q�b�g�ƒ�����̍ł��߂��_�Ƃ̋������v�Z
                        float target_size_tmp = obj.transform.lossyScale.x; // �H�H�H


                        //// �ړ����σt�B���^�����s���ɏ������y������v���O�����A---------
                        //if (distance < (cursor_size_limit / 2)) continue; // �����̎��͂ɂȂ��^�[�Q�b�g�̏ꍇ�ɏ������X�L�b�v
                        ////--------------------------------------------------------------
                        ///


                        if (script.dtime_correction_mode == true && script.MAverageFilter == false && distance < cursor_size_limit / 2) script.cursor_count++; // �J�[�\�����ɑ��݂���^�[�Q�b�g���J�E���g

                        // nearDistance��0(�ŏ��͂�����)�A���邢��nearDistance��distance�����傫���Ȃ�
                        if (nearDistance == 999 || nearDistance > distance)
                        {
                            if (nearDistance == 999 || distance < cursor_size_limit)
                            {
                                nearDistance = distance; // nearDistance���X�V
                                searchTargetObj = obj; // searchTargetObj���X�V
                                target_size = obj.transform.localScale.x; // �������Ă���^�[�Q�b�g�̑傫�����X�V�D�^�[�Q�b�g�̑傫�����S�����ňꗥ�̂���x�����̂ݎ擾
                                distance_of_camera_to_target = Vector3.Distance(camera_obj.position, obj.transform.position); // ���[�U�ƃ^�[�Q�b�g�Ԃ̋������X�V


                                // �J�[�\���̑傫���̏���ɒ�G�����ꍇ�̏���-------------------
                                if (nearDistance < cursor_size_limit) // �I�u�W�F�N�g�Ԃ̋�������薢�����J�[�\���̑傫�����ő喢���̏ꍇ
                                {
                                    script.cursor_color.a = color_alpha; // �J�[�\���̓����x�𒲐����ĕ\��
                                    script.DwellTarget = searchTargetObj; // �������Ă���I�u�W�F�N�g���X�V
                                }
                                else
                                {
                                    script.cursor_color.a = 0; // �J�[�\���̓����x�𒲐����Ĕ�\��
                                    script.DwellTarget = null; // �������Ă���I�u�W�F�N�g���X�V
                                }
                                //--------------------------------------------------------------
                            }
                            else
                            {
                                searchTargetObj = null; // �H�H�H
                            }
                        }
                        //--------------------------------------------------------------

                        if (distance < (target_size_tmp / 10)) break; // �����ƃ^�[�Q�b�g�̋������ق�0�Ȃ烋�[�v���I��
                    }
                    // foreach (GameObject obj in objs) �I��------------------------


                    // �ł��߂������I�u�W�F�N�g��Ԃ�
                    // �������Ă����^�[�Q�b�g���ς�����i�A���������r�؂ꂽ�j�ꍇ---
                    if (oldNearObj != searchTargetObj || nearDistance > cursor_size_limit)
                    {
                        script.same_target = false;
                        script.select_target_id = -1; // �I����Ԃ̃^�[�Q�b�g��ID��������
                        if (searchTargetObj != null)
                        {
                            if (script.total_DwellTime_mode == false) searchTargetObj.GetComponent<target_para_set>().dtime = 0; // �݌v�������Ԃ�������
                        }

                        script.BlinkFlag = 0; // �A���u����������
                    }
                    //--------------------------------------------------------------


                    if (searchTargetObj != null)
                    {
                        float gain;
                        int maxsize = 21;

                        if (script.cursor_count > maxsize) script.cursor_count = maxsize;

                        if (script.dtime_correction_mode)
                        {
                            gain = 2 - ((script.cursor_count - 1) * 0.1f);
                        }
                        else
                        {
                            gain = 1;
                        }

                        searchTargetObj.GetComponent<target_para_set>().dtime += Time.deltaTime * gain; // �������Ă���^�[�Q�b�g�̗݌v�������Ԃ�ǉ�
                    }

                    if ((searchTargetObj != null) && (script.approve_switch == true)) searchTargetObj.GetComponent<target_para_set>().dtime = 0f;


                    // ��莞�Ԓ������Ă����ꍇ--------------------------------------
                    if ((searchTargetObj != null) && (searchTargetObj.GetComponent<target_para_set>().dtime >= script.set_dtime))
                    {
                        script.select_target_id = searchTargetObj.GetComponent<target_para_set>().Id; // �I�������^�[�Q�b�g��ID���X�V�i����ID�����ʂƂ��ďo�͂����j
                        script.next_step__flag = false; // �^�X�N�Ԃ̋x�e��ԂɑJ�ڂ��邽�߂̃t���O���X�V
                        script.select_flag = false;
                    }

                    //if (((searchTargetObj.GetComponent<target_para_set>().dtime >= script.set_dtime || script.BlinkFlag > 0) && nearDistance * 2 + target_size < (cursor_size_limit)) || script.next_step__flag)
                    //{
                    //    script.select_target_id = searchTargetObj.GetComponent<target_para_set>().Id; // �I�������^�[�Q�b�g��ID���X�V�i����ID�����ʂƂ��ďo�͂����j
                    //    script.next_step__flag = false; // �^�X�N�Ԃ̋x�e��ԂɑJ�ڂ��邽�߂̃t���O���X�V
                    //}
                    //--------------------------------------------------------------


                    //Bubble Cursor��\��-------------------------------------------
                    if (searchTargetObj != null && script.cursor_switch)
                    {
                        Vector3 toObject = searchTargetObj.transform.position - rayset.ray0; // �����̎n�_����I�u�W�F�N�g�܂ł̃x�N�g�����v�Z
                        Vector3 projection = Vector3.Project(toObject, ray1.normalized); // �����ɑ΂���I�u�W�F�N�g�̓��e�_���v�Z
                        cursor_point = rayset.ray0 + projection; // ���e�_�����ɒ�����̍ł��߂��_���v�Z
                    }
                    //--------------------------------------------------------------


                    oldNearObj = searchTargetObj; // �������Ă���^�[�Q�b�g���X�V
                    script.cursor_radious = (nearDistance * 2) + (target_size); // �J�[�\���̑傫�����X�V


                    //�^�[�Q�b�g�̃^�O��������----------------------------------------
                    if (script.MAverageFilter)
                    {
                        foreach (GameObject obj in objs) // objs����1����obj�Ɏ��o��
                        {
                            obj.tag = tagName;
                        }
                    }
                    //--------------------------------------------------------------

                    return searchTargetObj; // �ł��߂��I�u�W�F�N�g��Ԃ�
                }
            }
        }
    }
}
