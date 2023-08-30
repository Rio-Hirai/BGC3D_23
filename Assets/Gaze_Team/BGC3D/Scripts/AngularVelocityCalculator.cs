using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityCalculator : MonoBehaviour
{
    public bool IsRotating { get; private set; }        // 回転中しているかどうか
    public float AngularVelocity { get; private set; }  // 角速度[deg/s]
    public Vector3 Axis { get; private set; }           // 回転軸
    private Quaternion _prevRotation;                   // 前フレームの姿勢
    [SerializeField] private receiver Server;           // ？？？

    private void Start()
    {
        _prevRotation = transform.rotation; // ？？？
    }

    private void Update()
    {
        if (Server.approve_switch) // ？？？
        {
            var rotation = transform.rotation; // 現在フレームの姿勢を取得
            var diffRotation = Quaternion.Inverse(_prevRotation) * rotation; // 前フレームからの回転量を求める
            diffRotation.ToAngleAxis(out var angle, out var axis); // 回転した角度と軸（ローカル空間）を求める

            IsRotating = !Mathf.Approximately(angle, 0); // 回転角度が0以外なら回転しているとみなす
            AngularVelocity = angle / Time.deltaTime; // 回転角度から角速度を計算
            Axis = rotation * axis; // ローカル空間の回転軸をワールド空間に変換


            //--------------------------------------------------------------
            if (AngularVelocity > 7.0f)
            {
                Server.head_rot_switch = true;
            }
            else
            {
                Server.head_rot_switch = false;
            }
            //--------------------------------------------------------------


            _prevRotation = rotation; // 前フレームの姿勢を更新
        }
    }
}