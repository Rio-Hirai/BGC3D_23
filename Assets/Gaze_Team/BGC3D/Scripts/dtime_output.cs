using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dtime_output : MonoBehaviour
{
    public GameObject score_object = null; // Textオブジェクト

    public receiver server;

    private float dtime = 0;

    private string monitor = "";

    // 初期化
    void Start()
    {
    }

    // 更新
    void Update()
    {
        // オブジェクトからTextコンポーネントを取得
        Text score_text = score_object.GetComponent<Text>();

        if (server.dtime_monitor)
        {
            if (server.DwellTarget)
            {
                if (server.DwellTarget.GetComponent<target_para_set>().dtime > 0)
                {
                    if (server.DwellTarget.GetComponent<target_para_set>().dtime <= server.set_dtime)
                    {
                        dtime = server.DwellTarget.GetComponent<target_para_set>().dtime;
                        score_object.GetComponent<Text>().color = Color.white;
                    }
                    else
                    {
                        score_object.GetComponent<Text>().color = server.target_color;
                    }
                }

                monitor = dtime.ToString();
            }
            else
            {
                monitor = "";
            }
        }

        // テキストの表示を入れ替える
        score_text.text = monitor;
    }
}