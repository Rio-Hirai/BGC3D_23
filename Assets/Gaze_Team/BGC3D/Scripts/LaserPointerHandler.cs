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
    public receiver Server;                     // �T�[�o

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
            GameObject testcube = GameObject.Find(e.target.name);
            Server.same_target = false;
            testcube.GetComponent<Renderer>().material.color = Server.target_color;

            if (this.tag == "Target")
            {
                Server.select_target_id = testcube.GetComponent<target_para_set>().Id;
            }
            else if (this.tag == "UI")
            {
                testcube.GetComponent<UI_default>().Click_flag = true;
            }
        }
    }
    //--------------------------------------------------------------


    // ���[�U�[�|�C���^�[��target�ɐG�ꂽ�Ƃ�-----------------------
    public void PointerInside(object sender, PointerEventArgs e)
    {
    }
    //--------------------------------------------------------------


    // ���[�U�[�|�C���^�[��target���痣�ꂽ�Ƃ�---------------------
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name);
        testcube.GetComponent<Renderer>().material.color = Color.white;
    }
    //--------------------------------------------------------------
}
