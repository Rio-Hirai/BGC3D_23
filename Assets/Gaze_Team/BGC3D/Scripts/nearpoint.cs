using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nearpoint : MonoBehaviour
{
    [SerializeField]
    private string tagName = "Enemy";        // �C���X�y�N�^�[�ŕύX�\

    public GameObject searchNearObj;         // �ł��߂��I�u�W�F�N�g(public�C���q�ɂ��邱�ƂŊO���̃N���X����Q�Ƃł���)
    public GameObject oldNearObj;
    private float searchWaitTime = 0.02f;     // �����̑ҋ@����

    private float timer = 0f;                // �����܂ł̑ҋ@���Ԍv���p

    public Vector3 targetpoint;

    public Vector3 oldcursorpoint;

    public GameObject Server;
    private receiver script;
    // Start is called before the first frame update
    void Start()
    {
        // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
        script = Server.GetComponent<receiver>();
        searchNearObj = Serch();
        Debug.Log(searchNearObj);
        Debug.Log(script.cursor_radious);
    }

    // Update is called once per frame
    void Update()
    {
        // ���Ԃ̌o�߂ɍ��킹�Ď����I�Ɏ擾����ꍇ

        // ���Ԃ��v��
        timer += Time.deltaTime;

        // �����̑ҋ@���Ԃ��o�߂�����
        if (timer >= searchWaitTime)
        {

            // �w�肵���^�O�����Q�[���I�u�W�F�N�g�̂����A���̃Q�[���I�u�W�F�N�g�ɍł��߂��Q�[���I�u�W�F�N�g�P���擾
            searchNearObj = Serch();
            Debug.Log(searchNearObj);

            // �v�����Ԃ����������āA�Č���
            searchWaitTime = 0;
        }


        this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious);
    }

    /// <summary>
    /// �w�肳�ꂽ�^�O�̒��ōł��߂����̂��擾
    /// </summary>
    /// <returns></returns>
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

        // objs����P����obj�ϐ��Ɏ��o��
        foreach (GameObject obj in objs)
        {
            obj.GetComponent<Renderer>().material.color = Color.white;
            // obj�Ɏ��o�����Q�[���I�u�W�F�N�g�ƁA���̃Q�[���I�u�W�F�N�g�Ƃ̋������v�Z���Ď擾
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // nearDistance��0(�ŏ��͂�����)�A���邢��nearDistance��distance�����傫���l�Ȃ�
            if (nearDistance == 0 || nearDistance > distance)
            {

                // nearDistance���X�V
                nearDistance = distance;

                // searchTargetObj���X�V
                searchTargetObj = obj;
            }
        }

        //�ł��߂������I�u�W�F�N�g��Ԃ�
        //searchTargetObj.GetComponent<Renderer>().material.color = Color.green;

        if (oldNearObj != searchTargetObj)
        {
            searchTargetObj.GetComponent<target_size_set>().dtime = 0;
        }
        searchTargetObj.GetComponent<target_size_set>().dtime += Time.deltaTime;
        if (searchTargetObj.GetComponent<target_size_set>().dtime >= script.set_dtime)
        {
            searchTargetObj.GetComponent<Renderer>().material.color = Color.green;
        }
        oldNearObj = searchTargetObj;

        targetpoint = searchTargetObj.transform.position;
        //script.cursor_radious = nearDistance*2 + script.target_size;
        //Debug.Log("dis1 = " + nearDistance + ", pos = " + this.transform.position);

        if (nearDistance < 1.0f )
        {
            //script.cursor_radious = nearDistance * 2 + script.target_size;
            script.cursor_radious = nearDistance;
        } else
        {
            script.cursor_radious = 1.0f;
        }

        //Debug.Log("dis2 = " + nearDistance + ", pos = " + this.transform.position);
        Debug.Log("dis3 = " + (this.transform.position.magnitude));
        return searchTargetObj;
    }
}
