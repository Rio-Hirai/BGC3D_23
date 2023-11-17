using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class Server_Haptic : MonoBehaviour
{
    public Haptic_Feedback haptic_script;
    public bool Target_Setting;
    public bool Haptic_Feedback;
    public Vector3 Eye_Point;
    public Camera Camera;
    public GameObject Eye_Pointer;
    public GameObject Targets;
    public float distanceFromCamera = 3.0f; // カメラからの距離
    public float distance;
    public float Moved_Power;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Target_Setting)
        {
            PlaceObjectInFront();
            Target_Setting = false;
        }

        if (Haptic_Feedback)
        {
            distance = Vector3.Distance(Camera.transform.position, Eye_Pointer.transform.position);

            if (distance - distanceFromCamera <= 0)
            {
                distance = 0;
            }
            else
            {
                distance -= distanceFromCamera;
            }

            Moved_Power = haptic_script.Max_Power - distance * 100 * 2;
            if (Moved_Power < haptic_script.Min_Power)
            {
                Moved_Power = haptic_script.Min_Power + 25;
            }
        }
    }

    public void PlaceObjectInFront()
    {
        //// カメラの正面方向にオブジェクトを配置
        //Targets.transform.position = Targets.transform.position + Targets.transform.forward * distanceFromCamera;

        //// オブジェクトをカメラに向かせる
        //Targets.transform.LookAt(Camera.transform);

        Vector3 forward = Vector3.Scale(Camera.transform.forward, new Vector3(1, 0, 1)).normalized; // ユーザ（カメラ）の前方方向を取得
        Vector3 newPosition = Camera.transform.position + forward * distanceFromCamera; // ユーザ（カメラ）の位置をターゲット群の新しい位置に設定
        newPosition.y = Camera.transform.position.y; // ターゲット群とユーザ（カメラ）の高さを同じにする
        Targets.transform.position = newPosition; // ターゲット群を新しい位置に移動
        Targets.transform.LookAt(Camera.transform.position); // ターゲット群をユーザ（カメラ）の方向に向ける
        Vector3 rotation = Targets.transform.eulerAngles; // ターゲット群が逆を向いてしまうので180度回転させる
        rotation.y += 180; // ？？？
        Targets.transform.eulerAngles = rotation; // ？？？

        Target_Setting = false; // 機能フラグをリセット
    }
}
