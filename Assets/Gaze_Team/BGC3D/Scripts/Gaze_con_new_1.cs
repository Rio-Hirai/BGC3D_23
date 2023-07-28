using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class Gaze_con_new_1 : MonoBehaviour
            {
                // サーバー接続
                public GameObject Server;
                private receiver script;

                public float color_alpha = 0.45f;

                [SerializeField]
                private string tagName = "Enemy";        // インスペクターで変更可能

                public GameObject searchNearObj;         // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
                public GameObject oldNearObj;
                private float searchWaitTime = 0.02f;     // 検索の待機時間

                private float timer = 0f;                // 検索までの待機時間計測用

                public SRanipal_GazeRaySample rayset;
                public GameObject camera_obj;

                private Vector3 cursor_point;
                private float target_size;
                private float distance_of_camera_to_target;

                void Start()
                {
                    // サーバと接続
                    script = Server.GetComponent<receiver>();
                    // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
                    script = Server.GetComponent<receiver>();

                    searchNearObj = Serch();
                }

                void Update()
                {
                    // cursorの位置と半径と色を指定
                    if (script.bubblegaze_switch)
                    {
                        //script.cursor_color.a = color_alpha;
                        this.GetComponent<Renderer>().material.color = script.cursor_color;
                    }
                    else
                    {
                        script.cursor_color.a = 0f;
                        this.GetComponent<Renderer>().material.color = script.cursor_color;
                    }
                    this.transform.position = cursor_point;
                    this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious);

                    // 時間の経過に合わせて自動的に取得する場合
                    // 時間を計測
                    timer += Time.deltaTime;
                    // 検索の待機時間を経過したら
                    if (timer >= searchWaitTime)
                    {
                        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
                        searchNearObj = Serch();

                        // 計測時間を初期化して、再検索
                        searchWaitTime = 0;
                    }
                }

                private GameObject Serch()
                {
                    // 最も近いオブジェクトの距離を代入するための変数
                    float nearDistance = 0;
                    // script.cursor_radious = nearDistance * 2 + script.target_size;

                    // 検索された最も近いゲームオブジェクトを代入するための変数
                    GameObject searchTargetObj = null;

                    // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

                    // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておくこと)
                    if (objs.Length == 0)
                    {
                        return searchTargetObj;
                    }

                    // ここの条件を確認する！！（リファクタリング部分）
                    if (script.BlinkCount > -1)
                    {
                        // objsから１つずつobj変数に取り出す
                        foreach (GameObject obj in objs)
                        {
                            // 直線の始点からオブジェクトまでのベクトルを計算します
                            Vector3 toObject = obj.transform.position - rayset.ray0;

                            // 直線に対するオブジェクトの投影点を計算します
                            Vector3 projection = Vector3.Project(toObject, rayset.ray1.normalized);

                            // 投影点を元に直線上の最も近い点を計算します
                            cursor_point = rayset.ray0 + projection;

                            // ターゲットと直線上の最も近い点との距離を計算します
                            float distance = Vector3.Distance(obj.transform.position, cursor_point);

                            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
                            if (nearDistance == 0 || nearDistance > distance)
                            {
                                // nearDistanceを更新
                                nearDistance = distance;

                                // searchTargetObjを更新
                                searchTargetObj = obj;
                                target_size = obj.transform.localScale.x;
                                distance_of_camera_to_target = Vector3.Distance(camera_obj.transform.position, obj.transform.position);

                                // 
                                if (nearDistance < (distance_of_camera_to_target / 8)) // オブジェクト間の距離が一定未満の場合
                                {
                                    script.DwellTarget = searchTargetObj;
                                    script.cursor_color.a = color_alpha;
                                }
                                else
                                {
                                    script.DwellTarget = null;
                                    script.cursor_color.a = 0;
                                }
                            }
                        }
                    }

                    // 最も近かったオブジェクトを返す
                    // 注視していたターゲットが変わった（連続注視が途切れた）場合
                    if (oldNearObj != searchTargetObj || nearDistance > (distance_of_camera_to_target / 8))
                    {
                        script.same_target = false;
                        script.select_target_id = -1;
                        searchTargetObj.GetComponent<target_para_set>().dtime = 0;
                        script.BlinkFlag = 0;

                        if (nearDistance * 2 + target_size < (distance_of_camera_to_target / 8))
                        {
                            script.lens_flag = true;
                        }
                    }

                    // 時間を計測
                    searchTargetObj.GetComponent<target_para_set>().dtime += Time.deltaTime;

                    // 一定時間注視していた場合
                    if (((searchTargetObj.GetComponent<target_para_set>().dtime >= script.set_dtime || script.BlinkFlag > 0) && nearDistance * 2 + script.target_size < (script.Depth / 8)) || script.next_step__flag)
                    {
                        script.select_target_id = searchTargetObj.GetComponent<target_para_set>().Id;
                        script.next_step__flag = false;
                    }

                    // 注視しているターゲットを更新
                    oldNearObj = searchTargetObj;

                    script.cursor_radious = nearDistance * 2 + target_size;

                    // 最も近いオブジェクトを返す
                    return searchTargetObj;
                }
            }
        }
    }
}
