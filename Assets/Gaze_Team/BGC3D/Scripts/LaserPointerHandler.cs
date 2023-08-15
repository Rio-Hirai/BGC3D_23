using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class LaserPointerHandler : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;   // 右コントローラのレイ
    public SteamVR_LaserPointer laserPointer2;  // 左コントローラのレイ
    public receiver Server;                     // サーバ

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


    // レーザーポインターをtargetに焦点をあわせてトリガーをひいたとき
    public void PointerClick(object sender, PointerEventArgs e)
    {
        if(Server.laserswitch)
        {
            GameObject testcube = GameObject.Find(e.target.name);
            Server.same_target = false;
            testcube.GetComponent<Renderer>().material.color = Server.target_color;
            Debug.Log("OK = ");

            if (testcube.tag == "Targets")
            {
                Server.select_target_id = testcube.GetComponent<target_para_set>().Id;
                Debug.Log("OK_id");
            }
            else if (testcube.tag == "UI")
            {
                testcube.GetComponent<UI_default>().Click_flag = true;
                Debug.Log("OK_ui");
            }
        }
    }
    //--------------------------------------------------------------


    // レーザーポインターがtargetに触れたとき-----------------------
    public void PointerInside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name);
        Server.DwellTarget = testcube;
    }
    //--------------------------------------------------------------


    // レーザーポインターがtargetから離れたとき---------------------
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name);
        testcube.GetComponent<Renderer>().material.color = Color.white;
    }
    //--------------------------------------------------------------
}
