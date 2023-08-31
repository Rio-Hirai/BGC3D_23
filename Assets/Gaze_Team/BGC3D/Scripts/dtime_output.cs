using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dtime_output : MonoBehaviour
{
    public GameObject score_object = null;      // Textオブジェクト
    [SerializeField] private receiver server;   // サーバ接続
    private float dtime = 0;                    // 注視時間を格納する変数
    private string monitor = "";                // 出力用に文字列に変換した注視時間を格納する変数


    void Update()
    {
        Text score_text = score_object.GetComponent<Text>(); // オブジェクトからTextコンポーネントを取得
        score_object.SetActive(true); // テキストオブジェクトを表示


        //--------------------------------------------------------------
        if (server.DwellTarget) // 注視しているターゲットが存在する場合
        {
            //--------------------------------------------------------------
            if (server.DwellTarget.GetComponent<target_para_set>().dtime > 0) // ターゲットの注視時間が0以上の場合
            {
                score_object.GetComponent<Text>().color = server.DwellTarget.GetComponent<Renderer>().material.color; // ？？？

                if (score_object.GetComponent<Text>().color == server.target_color) // ？？？
                {

                }
                else // ？？？
                {
                    dtime = server.ab_dtime; // ？？？
                }
            }
            //--------------------------------------------------------------


            monitor = dtime.ToString(); // 注視時間を文字列に変更
        }
        //--------------------------------------------------------------


        score_text.text = monitor; // テキストの表示を入れ替える
    }
}