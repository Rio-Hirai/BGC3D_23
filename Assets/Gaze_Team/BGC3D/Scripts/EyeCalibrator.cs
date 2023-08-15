using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeCalibrator : MonoBehaviour
{
    [SerializeField] private receiver server; // サーバ接続

    void Update()
    {
        // 視線のキャリブレーション処理---------------------------------
        // 現状はinspector側から手動でeye_calibrationをTrueにすることで実行するが，別Scriptでeye_calibrationを変更することでも実行は可能
        if (server.eye_calibration) // 視線のキャリブレーション用フラグがTrueの場合
        {
            SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero); // 視線のキャリブレーションを実行
            server.eye_calibration = false; // 視線のキャリブレーション用フラグをFalseに更新
        }
        //--------------------------------------------------------------
    }
}
