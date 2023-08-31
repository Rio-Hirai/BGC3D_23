using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class gaze_data_output : MonoBehaviour
{
    [SerializeField] private receiver server;
    [SerializeField] private gaze_data_callback_v2 data;


    void Update()
    {
        if (server.output_flag == false && server.taskflag == true)
        {
            server.result_output_every(data.get_gaze_data(), server.streamWriter_gaze, false); // 視線関係のデータを取得＆書き出し
        }
    }
}