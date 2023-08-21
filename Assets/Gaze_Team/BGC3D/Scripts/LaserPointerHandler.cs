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
    public receiver Server;                     // サーバと接続

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
            GameObject testcube = GameObject.Find(e.target.name); // 焦点を合わせたターゲットを取得
            Server.same_target = false; // ？？？
            testcube.GetComponent<Renderer>().material.color = Server.target_color; // 焦点を合わせたターゲットの色を変更


            // タグに応じた処理---------------------------------------------
            if (testcube.tag == "Targets") // 焦点を合わせたターゲットのタグが「Targets」の場合
            {
                Server.select_target_id = testcube.GetComponent<target_para_set>().Id; // 選択されたターゲットのIDを更新
            }
            else if (testcube.tag == "UI") // 焦点を合わせたターゲットのタグが「UI」の場合
            {
                testcube.GetComponent<UI_default>().Click_flag = true; // 選択されたUIのクリックフラグをTrueにする
            }
            //--------------------------------------------------------------
        }
    }
    //--------------------------------------------------------------


    // レーザーポインターがtargetに触れたとき-----------------------
    public void PointerInside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name); // ？？？
        Server.DwellTarget = testcube; // ？？？
    }
    //--------------------------------------------------------------


    // レーザーポインターがtargetから離れたとき---------------------
    public void PointerOutside(object sender, PointerEventArgs e)
    {
        GameObject testcube = GameObject.Find(e.target.name); // ？？？
        Server.DwellTarget = null; // ？？？
        Server.select_target_id = -1; // ？？？
        Server.selecting_target = null; // ？？？
        testcube.GetComponent<Renderer>().material.color = Color.white; // ？？？
    }
    //--------------------------------------------------------------
}
