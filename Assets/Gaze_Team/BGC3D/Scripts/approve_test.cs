using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class approve_test : MonoBehaviour
{
    public Transform head;

    private float lastRotationX; // 前回のx軸の回転
    private float timer; // タイマー
    private float rotationThreshold = 10f; // 回転閾値（度）
    private float timeThreshold = 0.3f; // 時間閾値（秒）

    void Start()
    {
        // 初期化
        lastRotationX = head.rotation.eulerAngles.x;
        timer = 0;
    }

    void Update()
    {
        // 時間を更新
        timer += Time.deltaTime;

        // 指定した時間が経過したら、x軸の回転をチェック
        if (timer >= timeThreshold)
        {
            // 現在のx軸の回転を取得
            float currentRotationX = head.rotation.eulerAngles.x;

            // 前回の回転からの変化を計算
            float rotationChange = Mathf.Abs(currentRotationX - lastRotationX);

            // 必要に応じて360度減らす（360度以上の回転を避けるため）
            if (rotationChange > 180) rotationChange -= 360;

            // 回転が閾値を超えている場合、メッセージを出力
            if (Mathf.Abs(rotationChange) > rotationThreshold)
            {
                Debug.Log("The x rotation changed by more than " + rotationThreshold + " degrees in " + timeThreshold + " seconds.");
            }

            // 前回の回転と時間を更新
            lastRotationX = currentRotationX;
            timer = 0;
        }
    }
}
