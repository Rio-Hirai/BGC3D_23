using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class LaserPointerHandler : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;   // �E�R���g���[���̃��C
    public SteamVR_LaserPointer laserPointer2;  // ���R���g���[���̃��C
    public receiver Server;                     // �T�[�o�Ɛڑ�

    void Awake()
    {
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
        laserPointer2.PointerIn += PointerInside;
        laserPointer2.PointerOut += PointerOutside;
        laserPointer2.PointerClick += PointerClick;
    }

    void Start()
    {
    }


    // ���[�U�[�|�C���^�[��target�ɏœ_�����킹�ăg���K�[���Ђ����Ƃ�
    public void PointerClick(object sender, PointerEventArgs e)
    {
        if(Server.laserswitch)
        {
            GameObject testcube = GameObject.Find(e.target.name); // �œ_�����킹���^�[�Q�b�g���擾
            Server.same_target = false; // �H�H�H
            testcube.GetComponent<Renderer>().material.color = Server.target_color; // �œ_�����킹���^�[�Q�b�g�̐F��ύX


            // �^�O�ɉ���������---------------------------------------------
            if (testcube.tag == "Targets") // �œ_�����킹���^�[�Q�b�g�̃^�O���uTargets�v�̏ꍇ
            {
                Server.select_target_id = testcube.GetComponent<target_para_set>().Id; // �I�����ꂽ�^�[�Q�b�g��ID���X�V
            }
            else if (testcube.tag == "UI") // �œ_�����킹���^�[�Q�b�g�̃^�O���uUI�v�̏ꍇ
            {
                testcube.GetComponent<UI_default>().Click_flag = true; // �I�����ꂽUI�̃N���b�N�t���O��True�ɂ���
            }
            //--------------------------------------------------------------
        }
    }
    //--------------------------------------------------------------


    // ���[�U�[�|�C���^�[��target�ɐG�ꂽ�Ƃ�-----------------------
    public void PointerInside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name); // �H�H�H
        Server.DwellTarget = testcube; // �H�H�H
    }
    //--------------------------------------------------------------


    // ���[�U�[�|�C���^�[��target���痣�ꂽ�Ƃ�---------------------
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name); // �H�H�H
        Server.DwellTarget = null; // �H�H�H
        Server.select_target_id = -1; // �H�H�H
        Server.selecting_target = null; // �H�H�H
        testcube.GetComponent<Renderer>().material.color = Color.white; // �H�H�H
    }
    //--------------------------------------------------------------
}
