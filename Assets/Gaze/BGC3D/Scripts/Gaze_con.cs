﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class Gaze_con : MonoBehaviour
            {
                //⓪取得呼び出し-----------------------------
                //呼び出したデータ格納用の関数
                EyeData eye;
                //-------------------------------------------

                //①瞳孔位置--------------------
                //x,y軸
                //左の瞳孔位置格納用関数
                Vector2 LeftPupil;
                //左の瞳孔位置格納用関数
                Vector2 RightPupil;
                //------------------------------

                //②まぶたの開き具合------------
                //左のまぶたの開き具合格納用関数
                float LeftBlink;
                //右のまぶたの開き具合格納用関数
                float RightBlink;
                //------------------------------

                //③視線情報--------------------
                //origin：起点，direction：レイの方向　x,y,z軸
                //両目の視線格納変数
                Vector3 CombineGazeRayorigin;
                Vector3 CombineGazeRaydirection;
                //左目の視線格納変数
                Vector3 LeftGazeRayorigin;
                Vector3 LeftGazeRaydirection;
                //右目の視線格納変数
                Vector3 RightGazeRayorigin;
                Vector3 RightGazeRaydirection;
                //------------------------------

                //④焦点情報--------------------
                //両目の焦点格納変数
                //レイの始点と方向（多分③の内容と同じ）
                Ray CombineRay;
                /*レイがどこに焦点を合わせたかの情報．Vector3 point : 視線ベクトルと物体の衝突位置，float distance : 見ている物体までの距離，
                   Vector3 normal:見ている物体の面の法線ベクトル，Collider collider : 衝突したオブジェクトのCollider，Rigidbody rigidbody：衝突したオブジェクトのRigidbody，Transform transform：衝突したオブジェクトのTransform*/
                //焦点位置にオブジェクトを出すためにpublicにしています．
                public static FocusInfo CombineFocus;
                //レイの半径
                float CombineFocusradius;
                //レイの最大の長さ
                float CombineFocusmaxDistance;
                //オブジェクトを選択的に無視するために使用されるレイヤー ID
                //int CombinefocusableLayer = 0;
                //------------------------------

                // サーバー接続
                public GameObject Server;
                //public GameObject EyePoint_sub;
                private receiver script;

                public int cnt = 0;

                public Vector3 old_eye_position;
                public Vector3 new_eye_position;


                [SerializeField]
                private string tagName = "Enemy";        // インスペクターで変更可能

                public GameObject searchNearObj;         // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
                public GameObject oldNearObj;
                private float searchWaitTime = 0.02f;     // 検索の待機時間

                private float timer = 0f;                // 検索までの待機時間計測用

                public Vector3 targetpoint;

                public Vector3 oldcursorpoint;

                public float coloralpha;

                void Start()
                {
                    // サーバと接続
                    script = Server.GetComponent<receiver>();
                    // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
                    script = Server.GetComponent<receiver>();
                    if (script.cursor_switch)
                    {
                        coloralpha = script.cursor_color.a;
                    } else
                    {
                        coloralpha = 0.0f;
                    }
                    
                    searchNearObj = Serch();
                    //Debug.Log(searchNearObj);
                    //Debug.Log(script.cursor_radious);
                }

                //1フレーム毎に実行
                void Update()
                {
                    // cursorの色を指定
                    this.GetComponent<Renderer>().material.color = script.cursor_color;

                    //エラー確認ViveSR.Error.がWORKなら正常に動いている．（フレームワークの方に内蔵済みだからいらないかも）
                    if (SRanipal_Eye_API.GetEyeData(ref eye) == ViveSR.Error.WORK)
                    {
                        //一応機器が正常に動いてる時の処理をここにかける
                    }
                    //-------------------------------------------

                    //⓪取得呼び出し-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------

                    //④焦点情報--------------------
                    //radius, maxDistance，CombinefocusableLayerは省略可
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                        //Debug.Log("Combine Focus Point" + CombineFocus.point.x + ", " + CombineFocus.point.y + ", " + CombineFocus.point.z);
                        //Debug.Log("new = "  + );
                        //if (Mathf.Abs(new_eye_position.magnitude - old_eye_position.magnitude)
                        // && new_eye_position.magnitude > script.pointvalue2
                        new_eye_position = CombineFocus.point;
                        if (new_eye_position.magnitude - old_eye_position.magnitude >= script.pointvalue)
                        {
                            script.select_flag_2 = 1;
                            this.transform.position = CombineFocus.point;
                        }
                        old_eye_position = CombineFocus.point;
                    }
                    //------------------------------

                    // 時間の経過に合わせて自動的に取得する場合

                    // 時間を計測
                    timer += Time.deltaTime;

                    // 検索の待機時間を経過したら
                    if (timer >= searchWaitTime)
                    {

                        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
                        searchNearObj = Serch();
                        //Debug.Log(searchNearObj);

                        // 計測時間を初期化して、再検索
                        searchWaitTime = 0;
                    }


                    this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious);
                }
                private GameObject Serch()
                {
                    // 最も近いオブジェクトの距離を代入するための変数
                    float nearDistance = 0;
                    script.cursor_radious = nearDistance;

                    // 検索された最も近いゲームオブジェクトを代入するための変数
                    GameObject searchTargetObj = null;

                    // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
                    GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

                    // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておくこと)
                    if (objs.Length == 0)
                    {
                        return searchTargetObj;
                    }

                    // objsから１つずつobj変数に取り出す
                    foreach (GameObject obj in objs)
                    {
                        if (obj.name != script.target_clone.name)
                        {
                            obj.GetComponent<Renderer>().material.color = Color.white;
                        }
                        // objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
                        float distance = Vector3.Distance(obj.transform.position, transform.position);

                        // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
                        if (nearDistance == 0 || nearDistance > distance)
                        {

                            // nearDistanceを更新
                            nearDistance = distance;

                            // searchTargetObjを更新
                            searchTargetObj = obj;
                        }
                    }

                    //最も近かったオブジェクトを返す
                    //searchTargetObj.GetComponent<Renderer>().material.color = Color.green;
                    if (oldNearObj != searchTargetObj)
                    {
                        searchTargetObj.GetComponent<target_para_set>().dtime = 0;
                        searchTargetObj.GetComponent<Renderer>().material.color = Color.yellow;
                        if (nearDistance * 2 + script.target_size < 1.0f)
                        {
                            script.lens_flag = true;
                        }
                    }
                    searchTargetObj.GetComponent<target_para_set>().dtime += Time.deltaTime;
                    if (searchTargetObj.GetComponent<target_para_set>().dtime >= script.set_dtime && nearDistance * 2 + script.target_size < 1.0f)
                    {
                        searchTargetObj.GetComponent<Renderer>().material.color = script.target_color;
                    }
                    oldNearObj = searchTargetObj;
                    //Debug.Log(searchTargetObj);

                    targetpoint = searchTargetObj.transform.position;
                    //script.cursor_radious = nearDistance*2 + script.target_size;
                    //Debug.Log("dis1 = " + nearDistance + ", pos = " + this.transform.position);

                    if (nearDistance * 2 + script.target_size < 1.0f)
                    {
                        script.cursor_radious = nearDistance * 2 + script.target_size;
                        script.lens_flag = true;
                        //script.cursor_radious = nearDistance;
                    }
                    else
                    {
                        script.cursor_radious = 1.0f;
                        script.lens_flag = false;
                    }

                    // カーソルの透明度の調整
                    float CursorColorAlpha = Vector3.Distance(script.head_obj.transform.position, searchTargetObj.transform.position);
                    if (CursorColorAlpha < 3.0f && script.target_alpha_switch)
                    {
                        script.cursor_color.a = coloralpha * (CursorColorAlpha * CursorColorAlpha / 9);
                    } else
                    {
                        script.cursor_color.a = coloralpha;
                    }

                    //Debug.Log("dis2 = " + nearDistance + ", pos = " + this.transform.position);
                    //Debug.Log("dis3 = " + (this.transform.position.magnitude));
                    return searchTargetObj;
                }
            }
        }
    }
}
