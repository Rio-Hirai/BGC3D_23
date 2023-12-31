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

            public class Gaze_con_new_v2 : MonoBehaviour
            {
                [SerializeField] private receiver script;   // サーバと接続
                public SRanipal_GazeRay_BGC_v2 rayset;      // レイキャストのデータを取得
                public MovingAverageFilter filter;          // 移動平均フィルタを取得

                [SerializeField]
                private string tagName = "Targets";         // 注視可能対象の選定．インスペクタで変更可能

                public GameObject searchNearObj;            // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
                public GameObject oldNearObj;               // 前フレームに注視していたターゲット
                public Transform camera_obj;                // ユーザの位置
                private float searchWaitTime = 1 / 60;      // 検索の待機時間

                private float timer = 0.0f;                 // 検索までの待機時間計測用
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
                    // カーソルの位置と半径と色を更新-------------------------------
                    if (script.cursor_switch) // カーソル表示機能がオンの場合
                    {
                        this.GetComponent<Renderer>().material.color = script.cursor_color; // 透明度を0より大きくしてカーソルを表示
                    }
                    else // カーソル表示機能がオフの場合
                    {
                        script.cursor_color.a = 0f; // カーソルを透明化（＝非表示化）
                        this.GetComponent<Renderer>().material.color = script.cursor_color; // 透明度を0にしてカーソルを非表示
                    }
                    //--------------------------------------------------------------


                    this.transform.position = cursor_point; // カーソルの位置を更新
                    this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious); // カーソルの大きさを更新

                    timer += Time.deltaTime; // 時間を計測


                    // 検索の待機時間を経過したら（処理が重くならないための処理なため場合によってはリファクタリング対象）
                    if (timer >= searchWaitTime)
                    {
                        searchNearObj = Serch(); // 指定したタグを持つオブジェクトのうち，このオブジェクトに最も近いゲームオブジェクトを取得
                        timer = 0; // 計測時間を初期化して再検索
                    }
                    // if (timer >= searchWaitTime)------------------------------------
                }


                // ターゲット検索------------------------------------------
                private GameObject Serch()
                {
                    float nearDistance = 999; // 最も近いオブジェクトの距離を代入するための変数
                    float cursor_size_limit = distance_of_camera_to_target / 8; // カーソルの大きさの上限を設定するための変数（でないと無限に大きくなる）
                    GameObject searchTargetObj = null; // 検索された最も近いゲームオブジェクトを代入するための変数
                    script.cursor_count = 0; // バブルカーソル内に存在するターゲットの数を初期化
                    Vector3 ray1 = rayset.ray1; // 視線の方向ベクトルを保存（動的移動平均フィルタを用いるため）
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName); // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得

                    if (objs.Length == 0) return searchTargetObj; // 取得したゲームオブジェクトが0ならnullを返す(nullでもエラーにならないように処理)


                    // 頷き選択ジェスチャの場合の処理----------------------------------
                    if (script.approve_switch == true && script.head_rot_switch == true) // ？？？
                    {
                        if (searchNearObj != null)
                        {
                            // 一定時間注視していた場合----------------------------------------------
                            if (script.select_flag) // ？？？
                            {
                                script.select_target_id = searchNearObj.GetComponent<target_para_set>().Id; // 選択したターゲットのIDを更新（このIDが結果として出力される）
                                script.next_step__flag = false; // タスク間の休憩状態に遷移するためのフラグを更新
                                script.select_flag = false; // ？？？
                                script.same_target = false; // ？？？
                            }
                            //-----------------------------------------------------------------------


                            //Bubble Cursorを表示----------------------------------------------------
                            if (script.cursor_switch) // ？？？
                            {
                                Vector3 toObject = searchNearObj.transform.position - rayset.ray0; // 直線の始点からオブジェクトまでのベクトルを計算
                                Vector3 projection = Vector3.Project(toObject, ray1.normalized); // 直線に対するオブジェクトの投影点を計算
                                cursor_point = rayset.ray0 + projection; // 投影点を元に直線上の最も近い点を計算
                            }
                            //-----------------------------------------------------------------------
                        }
                        //-----------------------------------------------------------------------


                        oldNearObj = searchNearObj; // 注視しているターゲットを更新
                        script.cursor_radious = (nearDistance * 2) + (target_size); // カーソルの大きさを更新

                        return searchNearObj; // 検索して発見したターゲットを返す
                    }
                    //----------------------------------------------------------------------


                    // 移動平均フィルタの処理--------------------------------------
                    if (script.MAverageFilter) // 移動平均フィルタの機能がオンの場合
                    {
                        foreach (GameObject obj in objs) // objsから1つずつobjに取り出す
                        {
                            Vector3 toObject = obj.transform.position - rayset.ray0; // 直線の始点からオブジェクトまでのベクトルを計算
                            Vector3 projection = Vector3.Project(toObject, ray1.normalized); // 直線に対するオブジェクトの投影点を計算
                            Vector3 closestPointOnRay = rayset.ray0 + projection; // 投影点を元に直線上の最も近い点を計算
                            float distance = Vector3.Distance(obj.transform.position, closestPointOnRay); // ターゲットと直線上の最も近い点との距離を計算
                            // obj.tag = tagName;  // 次回の処理のためにタグを初期化

                            if (distance < cursor_size_limit) obj.tag = "near"; // 視線の周辺にあるターゲットのタグを変更

                            if (distance < cursor_size_limit / 2) script.cursor_count++; // カーソル内に存在するターゲットをカウント
                        }

                        if (script.cursor_count > 0) objs = GameObject.FindGameObjectsWithTag("near"); // ターゲットリストを更新したタグを持つターゲットのみに更新

                        ray1 = filter.filter(rayset.ray1, script.cursor_count); // 視線の方向ベクトルに動的移動平均フィルタを適用（第一引数がフィルタリングする値，第二引数が窓の大きさ）
                    }
                    //--------------------------------------------------------------------


                    // Bubble Cursor------------------------------------------------
                    foreach (GameObject obj in objs) // objsから1つずつobjに取り出す
                    {
                        Vector3 toObject = obj.transform.position - rayset.ray0; // 直線の始点からオブジェクトまでのベクトルを計算
                        Vector3 projection = Vector3.Project(toObject, ray1.normalized); // 直線に対するオブジェクトの投影点を計算
                        Vector3 closestPointOnRay = rayset.ray0 + projection; // 投影点を元に直線上の最も近い点を計算
                        float distance = Vector3.Distance(obj.transform.position, closestPointOnRay); // ターゲットと直線上の最も近い点との距離を計算
                        float target_size_tmp = obj.transform.lossyScale.x; // ？？？


                        if (script.dtime_correction_mode == true && script.MAverageFilter == false && distance < cursor_size_limit / 2) script.cursor_count++; // カーソル内に存在するターゲットをカウント


                        // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きいなら
                        if (nearDistance == 999 || nearDistance > distance) // ？？？
                        {
                            if (nearDistance == 999 || distance < cursor_size_limit) // ？？？
                            {
                                nearDistance = distance; // nearDistanceを更新
                                searchTargetObj = obj; // searchTargetObjを更新
                                target_size = obj.transform.localScale.x; // 注視しているターゲットの大きさを更新．ターゲットの大きさが全次元で一律のためx次元のみ取得
                                distance_of_camera_to_target = Vector3.Distance(camera_obj.position, obj.transform.position); // ユーザとターゲット間の距離を更新


                                // カーソルの大きさの上限に抵触した場合の処理-------------------
                                if (nearDistance < cursor_size_limit) // オブジェクト間の距離が一定未満＝カーソルの大きさが最大未満の場合
                                {
                                    script.cursor_color.a = color_alpha; // カーソルの透明度を調整して表示
                                    script.DwellTarget = searchTargetObj; // 注視しているオブジェクトを更新
                                }
                                else // カーソルの大きさが上限より大きい場合
                                {
                                    script.cursor_color.a = 0; // カーソルの透明度を調整して非表示
                                    script.DwellTarget = null; // 注視しているオブジェクトを更新
                                }
                                //--------------------------------------------------------------
                            }
                            else // ？？？
                            {
                                searchTargetObj = null; // ？？？
                            }
                        }
                        //--------------------------------------------------------------

                        if (distance < (target_size_tmp / 10)) break; // 視線とターゲットの距離がほぼ0ならループを終了
                    }
                    // foreach (GameObject obj in objs) 終了------------------------


                    // 最も近かったオブジェクトを返す
                    // 注視していたターゲットが変わった（連続注視が途切れた）場合---
                    if (oldNearObj != searchTargetObj || nearDistance > cursor_size_limit) // 前フレームで取得したターゲットと同一，またカーソルの大きさが上限より大きい場合
                    {
                        script.same_target = false; // ？？？
                        script.select_target_id = -1; // 選択状態のターゲットのIDを初期化

                        if (searchTargetObj != null) // ターゲットが見つかっている場合
                        {
                            if (script.total_DwellTime_mode == false) // 累計注視時間モードがオンの場合
                            {
                                searchTargetObj.GetComponent<target_para_set>().dtime = 0; // 累計注視時間を初期化
                                script.ab_dtime = 0; // 累計注視時間を初期化
                            }
                        }

                        script.BlinkFlag = 0; // 連続瞬きを初期化
                    }
                    // if (oldNearObj != searchTargetObj || nearDistance > cursor_size_limit) 終了---


                    // ターゲットの密集度合いによる注視時間の補正-------------------
                    if (searchTargetObj != null) // ターゲットが見つかっている場合
                    {
                        float gain; // 加算する注視時間にかける係数
                        int maxsize = 15; // 周囲に存在するターゲットの上限


                        //--------------------------------------------------------------
                        if (script.cursor_count > maxsize) script.cursor_count = maxsize; // ターゲット数が上限に達している場合は上限に固定

                        if (script.dtime_correction_mode) // 注視時間の補正機能がオンの場合
                        {
                            gain = 2 - ((script.cursor_count - 1) * (1 / (maxsize - 1))); // ？？？
                        }
                        else // 注視時間の補正機能がオフの場合
                        {
                            gain = 1; // ？？？
                        }

                        float deltime = Time.deltaTime; // 前フレームからの経過時間を取得
                        searchTargetObj.GetComponent<target_para_set>().dtime += deltime * gain; // 注視しているターゲットの累計注視時間を追加
                        script.ab_dtime += deltime; // 累計注視時間を初期化
                        //--------------------------------------------------------------


                        if (script.approve_switch == true) searchTargetObj.GetComponent<target_para_set>().dtime = 0.0f; // 注視しているターゲットの累計注視時間を追加


                        // 一定時間注視していた場合--------------------------------------
                        if (searchTargetObj.GetComponent<target_para_set>().dtime >= script.set_dtime) // ？？？
                        {
                            script.select_target_id = searchTargetObj.GetComponent<target_para_set>().Id; // 選択したターゲットのIDを更新（このIDが結果として出力される）
                            script.next_step__flag = false; // タスク間の休憩状態に遷移するためのフラグを更新
                            script.select_flag = false; // ？？？
                        }
                        //---------------------------------------------------------------


                        // Bubble Cursorを表示-------------------------------------------
                        if (script.cursor_switch) // カーソル表示がオンの場合
                        {
                            Vector3 toObject = searchTargetObj.transform.position - rayset.ray0; // 直線の始点からオブジェクトまでのベクトルを計算
                            Vector3 projection = Vector3.Project(toObject, ray1.normalized); // 直線に対するオブジェクトの投影点を計算
                            cursor_point = rayset.ray0 + projection; // 投影点を元に直線上の最も近い点を計算
                        }
                        //---------------------------------------------------------------
                    }
                    // if (searchTargetObj != null) 終了----------------------------------


                    oldNearObj = searchTargetObj; // 注視しているターゲットを更新
                    script.cursor_radious = (nearDistance * 2) + (target_size); // カーソルの大きさを更新


                    //ターゲットのタグを初期化--------------------------------------
                    if (script.MAverageFilter) // 移動平均フィルタがオンの場合
                    {
                        foreach (GameObject obj in objs) // objsから1つずつobjに取り出す
                        {
                            obj.tag = tagName; // タグを初期化
                        }
                    }
                    //--------------------------------------------------------------

                    return searchTargetObj; // 最も近いオブジェクトを返す
                }
                //  GameObject Serch() 終了---------------------------
            }
        }
    }
}
