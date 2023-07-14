using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class LaserPointerHandler : MonoBehaviour
{
    //右手用
    public SteamVR_LaserPointer laserPointer;
    //左手用
    public SteamVR_LaserPointer laserPointer2;

    public GameObject dummy;
    public GameObject Server;
    private receiver script;

    public string tarObj_name;

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
        script = Server.GetComponent<receiver>();
    }

    //レーザーポインターをtargetに焦点をあわせてトリガーをひいたとき
    public void PointerClick(object sender, PointerEventArgs e)
    {
        //Debug.Log("PointerClick" + e.target.name);
        GameObject testcube = GameObject.Find(e.target.name);
        //Debug.Log("PointerClick" + testcube.name + "=" + e.target.name);
        script.same_target = false;
        testcube.GetComponent<Renderer>().material.color = script.target_color;

        script.select_target_id = testcube.GetComponent<target_para_set>().Id;
        //this.GetComponent<receiver>().target_clone = testcube;
    }

    //レーザーポインターがtargetに触れたとき
    public void PointerInside(object sender, PointerEventArgs e)
    {
    }

    //レーザーポインターがtargetから離れたとき
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name);
        testcube.GetComponent<Renderer>().material.color = Color.white;
        this.GetComponent<receiver>().target_clone = dummy;
    }
}
