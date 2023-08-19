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
        this.transform.localScale = new Vector3(Server.target_size, Server.target_size, Server.target_size); // ターゲットの大きさを初期化
        this.name = "target_" + Server.target_id; // ターゲットの名前を初期化
        this.GetComponent<target_para_set>().Id = Server.target_id; // ターゲットのIDを初期化
        Server.target_id++; // ターゲットのIDを連番にするために加算
    }
}
