using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.Newtonsoft.Json.Linq;
using Valve.VR.Extras;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class Gaze_con_new : MonoBehaviour
            {
                public receiver script;                     // サーバと接続
                public SRanipal_GazeRay_BGC rayset;         // レイキャストのデータを取得
                public MovingAverageFilter filter;          // 移動平均フィルタを取得

                [SerializeField]
                private string tagName = "Targets";         // 注視可能対象の選定．インスペクターで変更可能

                public GameObject searchNearObj;            // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
                public GameObject oldNearObj;               // 前フレームに注視していたターゲット
                public Transform camera_obj;                // ユーザの位置
                private float searchWaitTime = 1 / 60;        // 検索の待機時間

                private float timer = 0f;                   // 検索までの待機時間計測用
                private Vector3 cursor_point;               // カーソルの位置
                private float target_size;                  // 注視しているターゲットの大きさ
                private float distance_of_camera_to_target; // ユーザとターゲット間の距離
                private float color_alpha = 0.45f;          // カーソルの透明度

                void Start()
                {
                    searchNearObj = Serch(); // 注視ターゲット周りを初期化
                    color_alpha = script.cursor_color.a; // カーソルの透明度を保存
                }

                void Update()
                {
                    // カーソルの位置と半径と色を更新
                    if (script.cursor_switch) // カーソル表示・非常時スイッチの有無
                    {
                        this.GetComponent<Renderer>().material.color = script.cursor_color; // 透明度を0より大きくしてカーソルを表示
                    }
                    else
                    {
                        script.cursor_color.a = 0f; // カーソルを透明化（＝非表示化）
                        this.GetComponent<Renderer>().material.color = script.cursor_color; // 透明度を0にしてカーソルを非表示
                    }
                    this.transform.position = cursor_point; // カーソルの位置を更新
                    this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious); // カーソルの大きさを更新

                    timer += Time.deltaTime; // 時間を計測
                    // 検索の待機時間を経過したら（処理が重くならないための処理なため場合によってはリファクタリング対象）
                    if (timer >= searchWaitTime)
                    {
                        searchNearObj = Serch(); // 指定したタグを持つオブジェクトのうち，このオブジェクトに最も近いゲームオブジェクトを取得

                        timer = 0; // 計測時間を初期化して再検索
                    }
                }

                private GameObject Serch()
                {
                    float nearDistance = 0; // 最も近いオブジェクトの距離を代入するための変数
                    float cursor_size_limit = distance_of_camera_to_target / 8; // カーソルの大きさの上限を設定するための変数（でないと無限に大きくなる）
                    GameObject searchTargetObj = null; // 検索された最も近いゲームオブジェクトを代入するための変数
                    script.cursor_count = 0; // バブルカーソル内に存在するターゲットの数を初期化
                    Vector3 ray1 = rayset.ray1; // 視線の方向ベクトルを保存（動的移動平均フィルタを用いるため）
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName); // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得

                    if (objs.Length == 0) return searchTargetObj; // 取得したゲームオブジェクトが0ならnullを返す(nullでもエラーにならないように処理)

                    // 移動平均フィルタの処理
                    if (script.MAverageFilter) // 移動平均フィルタの機能がオンの場合
                    {
                        foreach (GameObject obj in objs) // objsから1つずつobjに取り出す
                        {
                            Vector3 toObject = obj.transform.position - rayset.ray0; // 直線の始点からオブジェクトまでのベクトルを計算
                            Vector3 projection = Vector3.Project(toObject, ray1.normalized); // 直線に対するオブジェクトの投影点を計算
                            cursor_point = rayset.ray0 + projection; // 投影点を元に直線上の最も近い点を計算
                            float distance = Vector3.Distance(obj.transform.position, cursor_point); // ターゲットと直線上の最も近い点との距離を計算

                            if (distance < cursor_size_limit / 2) script.cursor_count++; // カーソル内に存在するターゲットをカウント
                        }

                        ray1 = filter.filter(rayset.ray1, script.cursor_count); // 視線の方向ベクトルに動的移動平均フィルタを適用（第一引数がフィルタリングする値，第二引数が窓の大きさ）
                    }
                    //--------------------------------------------------------------


                    // ここの条件を確認する！！（リファクタリング部分）
                    if (script.BlinkCount > -1)
                    {
                        foreach (GameObject obj in objs) // objsから1つずつobjに取り出す
                        {
                            Vector3 toObject = obj.transform.position - rayset.ray0; // 直線の始点からオブジェクトまでのベクトルを計算
                            Vector3 projection = Vector3.Project(toObject, ray1.normalized); // 直線に対するオブジェクトの投影点を計算
                            cursor_point = rayset.ray0 + projection; // 投影点を元に直線上の最も近い点を計算
                            float distance = Vector3.Distance(obj.transform.position, cursor_point); // ターゲットと直線上の最も近い点との距離を計算


                            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
                            if (nearDistance == 0 || nearDistance > distance)
                            {
                                nearDistance = distance; // nearDistanceを更新
                                searchTargetObj = obj; // searchTargetObjを更新
                                target_size = obj.transform.localScale.x; // 注視しているターゲットの大きさを更新．ターゲットの大きさが全次元で一律のためx次元のみ取得
                                distance_of_camera_to_target = Vector3.Distance(camera_obj.position, obj.transform.position); // ユーザとターゲット間の距離を更新

                                // カーソルの大きさの上限に抵触した場合の処理-------------------
                                if (nearDistance < (cursor_size_limit)) // オブジェクト間の距離が一定未満＝カーソルの大きさが最大未満の場合
                                {
                                    script.cursor_color.a = color_alpha; // カーソルの透明度を調整して表示
                                    script.DwellTarget = searchTargetObj; // 注視しているオブジェクトを更新
                                }
                                else
                                {
                                    script.cursor_color.a = 0; // カーソルの透明度を調整して非表示
                                    script.DwellTarget = null; // 注視しているオブジェクトを更新
                                }
                                //--------------------------------------------------------------
                            }
                            //--------------------------------------------------------------
                        }
                    }


                    // 最も近かったオブジェクトを返す
                    // 注視していたターゲットが変わった（連続注視が途切れた）場合---
                    if (oldNearObj != searchTargetObj || nearDistance > cursor_size_limit)
                    {
                        script.same_target = false;
                        script.select_target_id = -1; // 選択状態のターゲットのIDを初期化
                        if (script.total_DwellTime_mode == false) searchTargetObj.GetComponent<target_para_set>().dtime = 0; // 累計注視時間を初期化

                        script.BlinkFlag = 0; // 連続瞬きを初期化
                    }
                    //--------------------------------------------------------------


                    searchTargetObj.GetComponent<target_para_set>().dtime += Time.deltaTime; // 注視しているターゲットの累計注視時間を追加


                    // 一定時間注視していた場合--------------------------------------
                    if (((searchTargetObj.GetComponent<target_para_set>().dtime >= script.set_dtime || script.BlinkFlag > 0) && nearDistance * 2 + target_size < (cursor_size_limit)) || script.next_step__flag)
                    {
                        script.select_target_id = searchTargetObj.GetComponent<target_para_set>().Id; // 選択したターゲットのIDを更新（このIDが結果として出力される）
                        script.next_step__flag = false; // タスク間の休憩状態に遷移するためのフラグを更新
                    }
                    //--------------------------------------------------------------


                    oldNearObj = searchTargetObj; // 注視しているターゲットを更新
                    script.cursor_radious = nearDistance * 2 + target_size; // カーソルの大きさを更新

                    return searchTargetObj; // 最も近いオブジェクトを返す
                }
            }
        }
    }
}
