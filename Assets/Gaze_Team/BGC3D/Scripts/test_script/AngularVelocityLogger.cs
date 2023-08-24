using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityLogger : MonoBehaviour
{
    // あらかじめ計算スクリプトをアタッチしておく
    [SerializeField] private AngularVelocityCalculator _calculator;

    private void LateUpdate()
    {
        // 回転中かどうか
        if (_calculator.IsRotating)
        {
            // 回転している場合は、角速度と回転軸を出力
            print($"角速度 = {_calculator.AngularVelocity}, 回転軸 = {_calculator.Axis}");
        }
        else
        {
            // 回転していない場合
            print("回転していない");
        }
    }
}
