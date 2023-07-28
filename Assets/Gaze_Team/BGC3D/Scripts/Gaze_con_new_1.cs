using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
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

            public class Gaze_con_new_1 : MonoBehaviour
            {
                // �T�[�o�[�ڑ�
                public GameObject Server;
                private receiver script;

                public float color_alpha = 0.45f;

                [SerializeField]
                private string tagName = "Enemy";        // �C���X�y�N�^�[�ŕύX�\

                public GameObject searchNearObj;         // �ł��߂��I�u�W�F�N�g(public�C���q�ɂ��邱�ƂŊO���̃N���X����Q�Ƃł���)
                public GameObject oldNearObj;
                private float searchWaitTime = 0.02f;     // �����̑ҋ@����

                private float timer = 0f;                // �����܂ł̑ҋ@���Ԍv���p

                public SRanipal_GazeRaySample rayset;
                public GameObject camera_obj;

                private Vector3 cursor_point;
                private float target_size;
                private float distance_of_camera_to_target;

                void Start()
                {
                    // �T�[�o�Ɛڑ�
                    script = Server.GetComponent<receiver>();
                    // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
                    script = Server.GetComponent<receiver>();

                    searchNearObj = Serch();
                }

                void Update()
                {
                    // cursor�̈ʒu�Ɣ��a�ƐF���w��
                    if (script.bubblegaze_switch)
                    {
                        //script.cursor_color.a = color_alpha;
                        this.GetComponent<Renderer>().material.color = script.cursor_color;
                    }
                    else
                    {
                        script.cursor_color.a = 0f;
                        this.GetComponent<Renderer>().material.color = script.cursor_color;
                    }
                    this.transform.position = cursor_point;
                    this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious);

                    // ���Ԃ̌o�߂ɍ��킹�Ď����I�Ɏ擾����ꍇ
                    // ���Ԃ��v��
                    timer += Time.deltaTime;
                    // �����̑ҋ@���Ԃ��o�߂�����
                    if (timer >= searchWaitTime)
                    {
                        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
                        searchNearObj = Serch();

                        // �v�����Ԃ����������āA�Č���
                        searchWaitTime = 0;
                    }
                }

                private GameObject Serch()
                {
                    // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
                    float nearDistance = 0;
                    // script.cursor_radious = nearDistance * 2 + script.target_size;

                    // �������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
                    GameObject searchTargetObj = null;

                    // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

                    // �擾�����Q�[���I�u�W�F�N�g�� 0 �Ȃ�null��߂�(�g�p����ꍇ�ɂ�null�ł��G���[�ɂȂ�Ȃ������ɂ��Ă�������)
                    if (objs.Length == 0)
                    {
                        return searchTargetObj;
                    }

                    // �����̏������m�F����I�I�i���t�@�N�^�����O�����j
                    if (script.BlinkCount > -1)
                    {
                        // objs����P����obj�ϐ��Ɏ��o��
                        foreach (GameObject obj in objs)
                        {
                            // �����̎n�_����I�u�W�F�N�g�܂ł̃x�N�g�����v�Z���܂�
                            Vector3 toObject = obj.transform.position - rayset.ray0;

                            // �����ɑ΂���I�u�W�F�N�g�̓��e�_���v�Z���܂�
                            Vector3 projection = Vector3.Project(toObject, rayset.ray1.normalized);

                            // ���e�_�����ɒ�����̍ł��߂��_���v�Z���܂�
                            cursor_point = rayset.ray0 + projection;

                            // �^�[�Q�b�g�ƒ�����̍ł��߂��_�Ƃ̋������v�Z���܂�
                            float distance = Vector3.Distance(obj.transform.position, cursor_point);

                            // nearDistance��0(�ŏ��͂�����)�A���邢��nearDistance��distance�����傫���l�Ȃ�
                            if (nearDistance == 0 || nearDistance > distance)
                            {
                                // nearDistance���X�V
                                nearDistance = distance;

                                // searchTargetObj���X�V
                                searchTargetObj = obj;
                                target_size = obj.transform.localScale.x;
                                distance_of_camera_to_target = Vector3.Distance(camera_obj.transform.position, obj.transform.position);

                                // 
                                if (nearDistance < (distance_of_camera_to_target / 8)) // �I�u�W�F�N�g�Ԃ̋�������薢���̏ꍇ
                                {
                                    script.DwellTarget = searchTargetObj;
                                    script.cursor_color.a = color_alpha;
                                }
                                else
                                {
                                    script.DwellTarget = null;
                                    script.cursor_color.a = 0;
                                }
                            }
                        }
                    }

                    // �ł��߂������I�u�W�F�N�g��Ԃ�
                    // �������Ă����^�[�Q�b�g���ς�����i�A���������r�؂ꂽ�j�ꍇ
                    if (oldNearObj != searchTargetObj || nearDistance > (distance_of_camera_to_target / 8))
                    {
                        script.same_target = false;
                        script.select_target_id = -1;
                        searchTargetObj.GetComponent<target_para_set>().dtime = 0;
                        script.BlinkFlag = 0;

                        if (nearDistance * 2 + target_size < (distance_of_camera_to_target / 8))
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
                    }

                    // �������Ă���^�[�Q�b�g���X�V
                    oldNearObj = searchTargetObj;

                    script.cursor_radious = nearDistance * 2 + target_size;

                    // �ł��߂��I�u�W�F�N�g��Ԃ�
                    return searchTargetObj;
                }
            }
        }
    }
}
