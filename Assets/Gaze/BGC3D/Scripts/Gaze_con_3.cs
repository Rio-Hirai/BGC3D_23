using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class Gaze_con_3 : MonoBehaviour
            {
                //?�擾�Ăяo��-----------------------------
                //�Ăяo�����f�[�^�i�[�p�̊֐�
                EyeData eye;
                //-------------------------------------------

                //�@���E�ʒu--------------------
                //x,y��
                //���̓��E�ʒu�i�[�p�֐�
                Vector2 LeftPupil;
                //���̓��E�ʒu�i�[�p�֐�
                Vector2 RightPupil;
                //------------------------------

                //�B�������--------------------
                //origin�F�N�_�Cdirection�F���C�̕����@x,y,z��
                //���ڂ̎����i�[�ϐ�
                Vector3 CombineGazeRayorigin;
                Vector3 CombineGazeRaydirection;
                //���ڂ̎����i�[�ϐ�
                Vector3 LeftGazeRayorigin;
                Vector3 LeftGazeRaydirection;
                //�E�ڂ̎����i�[�ϐ�
                Vector3 RightGazeRayorigin;
                Vector3 RightGazeRaydirection;
                //------------------------------

                //�C�œ_���--------------------
                //���ڂ̏œ_�i�[�ϐ�
                //���C�̎n�_�ƕ����i�����B�̓��e�Ɠ����j
                Ray CombineRay;
                /*���C���ǂ��ɏœ_�����킹�����̏��DVector3 point : �����x�N�g���ƕ��̂̏Փˈʒu�Cfloat distance : ���Ă��镨�̂܂ł̋����C
                   Vector3 normal:���Ă��镨�̖̂ʂ̖@���x�N�g���CCollider collider : �Փ˂����I�u�W�F�N�g��Collider�CRigidbody rigidbody�F�Փ˂����I�u�W�F�N�g��Rigidbody�CTransform transform�F�Փ˂����I�u�W�F�N�g��Transform*/
                //�œ_�ʒu�ɃI�u�W�F�N�g���o�����߂�public�ɂ��Ă��܂��D
                public static FocusInfo CombineFocus;
                //���C�̔��a
                float CombineFocusradius;
                //���C�̍ő�̒���
                float CombineFocusmaxDistance;
                //�I�u�W�F�N�g��I��I�ɖ������邽�߂Ɏg�p����郌�C���[ ID
                //int CombinefocusableLayer = 0;
                //------------------------------

                // �T�[�o�[�ڑ�
                public GameObject Server;
                //public GameObject EyePoint_sub;
                private receiver script;

                public int cnt = 0;

                public Vector3 old_eye_position;
                public Vector3 new_eye_position;


                [SerializeField]
                private string tagName = "Enemy";        // �C���X�y�N�^�[�ŕύX�\

                public GameObject searchNearObj;         // �ł��߂��I�u�W�F�N�g(public�C���q�ɂ��邱�ƂŊO���̃N���X����Q�Ƃł���)
                public GameObject oldNearObj;
                private float searchWaitTime = 0.02f;     // �����̑ҋ@����

                private float timer = 0f;                // �����܂ł̑ҋ@���Ԍv���p

                public Vector3 targetpoint;

                public Vector3 oldcursorpoint;

                public float coloralpha;

                void Start()
                {
                    // �T�[�o�Ɛڑ�
                    script = Server.GetComponent<receiver>();
                    // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
                    script = Server.GetComponent<receiver>();
                    if (script.cursor_switch)
                    {
                        coloralpha = script.cursor_color.a;
                    }
                    else
                    {
                        coloralpha = 0.0f;
                    }

                    searchNearObj = Serch();
                    //Debug.Log(searchNearObj);
                    //Debug.Log(script.cursor_radious);
                }

                //1�t���[�����Ɏ��s
                void Update()
                {
                    // �f�o�b�O�p
                    if (script.bubble_switch)
                    {
                        script.cursor_color.a = 0.5f;
                        coloralpha = 0.5f;
                    }
                    else
                    {
                        script.cursor_color.a = 0.0f;
                        coloralpha = 0.0f;
                    }

                    // cursor�̐F���w��
                    this.GetComponent<Renderer>().material.color = script.cursor_color;

                    //�G���[�m�FViveSR.Error.��WORK�Ȃ琳��ɓ����Ă���D�i�t���[�����[�N�̕��ɓ����ς݂����炢��Ȃ������j
                    if (SRanipal_Eye_API.GetEyeData(ref eye) == ViveSR.Error.WORK)
                    {
                        //�ꉞ�@�킪����ɓ����Ă鎞�̏����������ɂ�����
                    }
                    //-------------------------------------------

                    //?�擾�Ăяo��-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------

                    //�A�܂Ԃ��̊J���------------�iHMD�����ĂȂ��Ă�1���Ԃ��Ă���H�H��j
                    //���̂܂Ԃ��̊J������擾
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out script.LeftBlink, eye))
                    {
                        //�l���L���Ȃ獶�̂܂Ԃ��̊J�����\��
                        //Debug.Log("Left Blink" + script.LeftBlink);
                    }
                    //�E�̂܂Ԃ��̊J������擾
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out script.RightBlink, eye))
                    {
                        //�l���L���Ȃ�E�̂܂Ԃ��̊J�����\��
                        //Debug.Log("Right Blink" + script.RightBlink);
                    }
                    //------------------------------

                    //�C�œ_���--------------------
                    //radius, maxDistance�CCombinefocusableLayer�͏ȗ���
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                        //Debug.Log("Combine Focus Point" + CombineFocus.point.x + ", " + CombineFocus.point.y + ", " + CombineFocus.point.z);
                        //Debug.Log("new = "  + );
                        //if (Mathf.Abs(new_eye_position.magnitude - old_eye_position.magnitude)
                        // && new_eye_position.magnitude > script.pointvalue2

                        new_eye_position = CombineFocus.point;

                        //if (script.test_id== 0)
                        //{
                        //    Debug.Log("OKOKOK");
                        //    Debug.Log(new_eye_position);
                        //}

                        if ((new_eye_position.magnitude - old_eye_position.magnitude >= script.pointvalue) && script.BlinkCount == 0)
                        {
                            script.select_flag_2 = 1;
                            this.transform.position = CombineFocus.point;
                        }
                        old_eye_position = CombineFocus.point;
                    }
                    //------------------------------

                    // ���Ԃ̌o�߂ɍ��킹�Ď����I�Ɏ擾����ꍇ

                    // ���Ԃ��v��
                    timer += Time.deltaTime;

                    // �����̑ҋ@���Ԃ��o�߂�����
                    if (timer >= searchWaitTime)
                    {

                        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
                        searchNearObj = Serch();
                        //Debug.Log(searchNearObj);

                        // �v�����Ԃ����������āA�Č���
                        searchWaitTime = 0;
                    }


                    this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious);
                }
                private GameObject Serch()
                {
                    // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
                    float nearDistance = 0;
                    script.cursor_radious = nearDistance;

                    // �������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
                    GameObject searchTargetObj = null;

                    // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

                    // �擾�����Q�[���I�u�W�F�N�g�� 0 �Ȃ�null��߂�(�g�p����ꍇ�ɂ�null�ł��G���[�ɂȂ�Ȃ������ɂ��Ă�������)
                    if (objs.Length == 0)
                    {
                        return searchTargetObj;
                    }

                    if (script.BlinkCount > -1)
                    {
                        // objs����P����obj�ϐ��Ɏ��o��
                        foreach (GameObject obj in objs)
                        {
                            //if (obj.name != script.target_clone.name)
                            //{
                            //    obj.GetComponent<Renderer>().material.color = Color.white;
                            //}
                            // obj�Ɏ��o�����Q�[���I�u�W�F�N�g�ƁA���̃Q�[���I�u�W�F�N�g�Ƃ̋������v�Z���Ď擾
                            float distance = Vector3.Distance(obj.transform.position, transform.position);

                            // nearDistance��0(�ŏ��͂�����)�A���邢��nearDistance��distance�����傫���l�Ȃ�
                            if (nearDistance == 0 || nearDistance > distance)
                            {

                                // nearDistance���X�V
                                nearDistance = distance;

                                // searchTargetObj���X�V
                                searchTargetObj = obj;
                                if (nearDistance < (script.Depth / 16))
                                {
                                    script.DwellTarget = searchTargetObj;
                                    script.pointvalue = 0.01f - (nearDistance * nearDistance / 10);
                                }
                                else
                                {
                                    script.DwellTarget = null;
                                    script.pointvalue = 0.000001f;
                                }

                                if (script.RayTarget == searchTargetObj)
                                {
                                    script.pointvalue = script.pointvalue2;
                                }
                            }
                        }
                    }

                    // �ł��߂������I�u�W�F�N�g��Ԃ�
                    // �������Ă����^�[�Q�b�g���ς�����i�A���������r�؂ꂽ�j�ꍇ
                    // searchTargetObj.GetComponent<Renderer>().material.color = Color.green;
                    if (oldNearObj != searchTargetObj || nearDistance > (script.Depth / 16))
                    {
                        script.same_target = false;
                        script.select_target_id = -1;
                        searchTargetObj.GetComponent<target_para_set>().dtime = 0;
                        script.BlinkFlag = 0;
                        //searchTargetObj.GetComponent<Renderer>().material.color = Color.yellow;

                        if (nearDistance * 2 + script.target_size < (script.Depth / 8))
                        {
                            script.lens_flag = true;
                        }
                    }

                    // ���Ԃ��v��
                    searchTargetObj.GetComponent<target_para_set>().dtime += Time.deltaTime;

                    // ��莞�Ԓ������Ă����ꍇ
                    if (((searchTargetObj.GetComponent<target_para_set>().dtime >= script.set_dtime || script.BlinkFlag > 0) && nearDistance * 2 + script.target_size < (script.Depth / 8)) || script.next_step__flag)
                    {
                        script.select_target_id = searchTargetObj.GetComponent<target_para_set>().Id;
                        script.next_step__flag = false;
                        //searchTargetObj.GetComponent<Renderer>().material.color = script.target_color;
                    }

                    //// �^�[�Q�b�g�̋����ɑ΂���U�ڑ΍�
                    //if (nearDistance * 2 + script.target_size > (script.Depth / 8))
                    //{
                    //    script.pointvalue = 0.0001f;
                    //}
                    //else
                    //{
                    //    script.pointvalue = 0.05f;
                    //}

                    // �������Ă���^�[�Q�b�g���X�V
                    oldNearObj = searchTargetObj;
                    //Debug.Log(searchTargetObj);

                    targetpoint = searchTargetObj.transform.position;
                    script.cursor_radious = nearDistance * 2 + script.target_size;
                    // Debug.Log("dis1 = " + nearDistance + ", pos = " + this.transform.position);

                    // �J�[�\���̑傫���̒���
                    if (nearDistance * 2 + script.target_size < (script.Depth / 8))
                    {
                        script.cursor_radious = nearDistance * 2 + script.target_size * 2;
                        script.lens_flag = true;
                        //script.cursor_radious = nearDistance;
                    }
                    else
                    {
                        script.cursor_radious = (script.Depth / 8);
                        script.lens_flag = false;
                    }

                    if (Vector3.Distance(script.head_obj.transform.position, this.transform.position) < (script.Depth - 1.0f))
                    {
                        //script.bubble_switch = false;
                        if (script.test_id != 2)
                        {
                            this.GetComponent<Collider>().enabled = false;
                        }
                    } else
                    {
                        //script.bubble_switch= true;
                        if (script.test_id != 2)
                        {
                            this.GetComponent<Collider>().enabled = true;
                        }
                    }
                    //Debug.Log(Vector3.Distance(script.head_obj.transform.position, this.transform.position));

                    //if(script.selecting_target.name == oldNearObj.name)
                    //{
                    //    Debug.Log("OK");
                    //    script.pointvalue = 0.00f;
                    //} else
                    //{

                    //}

                    //// �J�[�\���̓����x�̒���
                    //float CursorColorAlpha = Vector3.Distance(script.head_obj.transform.position, transform.position);
                    ////float CursorColorAlpha = Vector3.Distance(script.head_obj.transform.position, searchTargetObj.transform.position);
                    ////Debug.Log("CursorColorAlpha = " + CursorColorAlpha);
                    //if (CursorColorAlpha < 1.0f)
                    //{
                    //    //script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.00f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.00f);
                    //}
                    //else if (CursorColorAlpha < script.Depth / 3)
                    //{
                    //    script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.001f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.001f);
                    //}
                    //else if (CursorColorAlpha < script.Depth / 2)
                    //{
                    //    //script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.005f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.005f);
                    //}
                    //else if (CursorColorAlpha < script.Depth / 1.5)
                    //{
                    //    //script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.01f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.01f);
                    //}
                    //else if (CursorColorAlpha < script.Depth && script.target_alpha_switch)
                    //{
                    //    //script.cursor_color.a = coloralpha * (CursorColorAlpha * CursorColorAlpha / (script.Depth * script.Depth * script.Depth));
                    //    //script.pointvalue = 0.01f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.01f);
                    //    //Debug.Log("clear = " + script.cursor_color.a);
                    //}
                    //else
                    //{
                    //    script.cursor_color.a = coloralpha;
                    //    //script.pointvalue = 0.03f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.05f);
                    //}

                    //Debug.Log("dis2 = " + nearDistance + ", pos = " + this.transform.position);
                    //Debug.Log("dis3 = " + (this.transform.position.magnitude));
                    return searchTargetObj;
                }

                private float pointvalue(float nowpoint, float newpoint)
                {
                    return Mathf.Min(nowpoint, newpoint);
                }
            }
        }
    }
}
