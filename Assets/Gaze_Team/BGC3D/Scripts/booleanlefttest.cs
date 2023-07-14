using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;

public class booleanlefttest : MonoBehaviour
{

    //InteractUIボタンが押されてるのかを判定するためのIuiという関数にSteamVR_Actions.default_InteractUIを固定
    private SteamVR_Action_Boolean Iui = SteamVR_Actions.default_InteractUI;
    //結果の格納用Boolean型関数interacrtui
    private Boolean interacrtui;

    //1フレーム毎に呼び出されるUpdateメゾット
    void Update()
    {
        //結果をGetStateで取得してinteracrtuiに格納
        //SteamVR_Input_Sources.機器名（今回は左コントローラ）
        interacrtui = Iui.GetState(SteamVR_Input_Sources.RightHand);
        //interacrtuiの中身を確認
        //Debug.Log(interacrtui);
    }
}
