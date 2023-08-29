using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class approve_test : MonoBehaviour
{
    [SerializeField] private Transform head;// ユーザの頭部位置
    [SerializeField] private receiver Server;

    private float lastRotationX;                // 前回のx軸の回転
    private float timer;                        // タイマー
    private float rotationThreshold = 9.0f;      // 回転閾値（度）
    private float timeThreshold_min = 0.20f;    // 時間閾値（秒）
    private float timeThreshold_max = 0.40f;    // 時間閾値（秒）


    // 効果音-------------------------------------------------------
    public AudioSource audioSource;         // 音響設定
    public AudioClip sound_OK;              // 指示通りのターゲットを選択できた時の音
    //--------------------------------------------------------------

    void Start()
    {
        // 初期化-------------------------------------------------------
        lastRotationX = head.rotation.eulerAngles.x;
        timer = 0;
        //--------------------------------------------------------------
    }

    void Update()
    {
        timer += Time.deltaTime; // 時間を更新

        // 指定した時間が経過したら、x軸の回転をチェック
        if (timer >= timeThreshold_min)
        {
            float currentRotationX = head.rotation.eulerAngles.x; // 現在のx軸の回転を取得
            float rotationChange = Mathf.Abs(Mathf.Abs(currentRotationX) - Mathf.Abs(lastRotationX)); // 前回の回転からの変化を計算
            if (rotationChange > 180) rotationChange -= 360; // 必要に応じて360度減らす（360度以上の回転を避けるため）


            // 回転が閾値を超えている場合、メッセージを出力-----------------
            if ((Mathf.Abs(rotationChange) > rotationThreshold) && (lastRotationX < currentRotationX))
            {
                // 前回の回転と時間を更新---------------------------------------
                lastRotationX = currentRotationX;
                timer = 0f;
                //--------------------------------------------------------------


                audioSource.PlayOneShot(sound_OK); // 正解した時の効果音を鳴らす
                Server.select_flag = true;
            }
            else if (timer >= timeThreshold_max)
            {
                // 前回の回転と時間を更新---------------------------------------
                lastRotationX = currentRotationX;
                timer = 0f;
                //--------------------------------------------------------------
            }
            //--------------------------------------------------------------
        }
    }
}
