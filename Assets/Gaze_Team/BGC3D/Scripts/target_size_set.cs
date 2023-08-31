using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class target_size_set : MonoBehaviour
{
    public receiver Server; // サーバ接続
    public float dtime; // 累計注視時間を格納する変数

    void Start()
    {
        if (Server.target_size_mini_switch)
        {
            float distance_of_camera_to_target = Vector3.Distance(Server.head_obj.transform.position, this.transform.position);
            float angleRadians = 1.0f * Mathf.Deg2Rad;
            float height = (Mathf.Tan(angleRadians) * distance_of_camera_to_target);
            this.transform.localScale = new Vector3(height, height, height); // ターゲットの大きさを初期化
        }
        else
        {
            this.transform.localScale = new Vector3(Server.target_size, Server.target_size, Server.target_size); // ターゲットの大きさを初期化
        }

        this.name = "target_" + Server.target_id; // ターゲットの名前を初期化
        this.GetComponent<target_para_set>().Id = Server.target_id; // ターゲットのIDを初期化
        Server.target_id++; // ターゲットのIDを連番にするために加算
    }
}
