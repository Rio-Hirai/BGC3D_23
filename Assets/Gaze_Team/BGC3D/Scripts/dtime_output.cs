using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dtime_output : MonoBehaviour
{
    public GameObject score_object = null; // Textオブジェクト

    public receiver server;

    // 初期化
    void Start()
    {
    }

    // 更新
    void Update()
    {
        // オブジェクトからTextコンポーネントを取得
        Text score_text = score_object.GetComponent<Text>();

        float dtime = 0;

        if (server.DwellTarget) dtime = server.DwellTarget.GetComponent<target_para_set>().dtime;

        // テキストの表示を入れ替える
        score_text.text = dtime.ToString();
    }
}