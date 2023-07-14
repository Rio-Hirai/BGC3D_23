using System.Collections;
using System.Collections.Generic;
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

            public class Gaze_con_3 : MonoBehaviour
            {
                //?取得呼び出し-----------------------------
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
                    }
                    else
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
                    // デバッグ用
                    if (script.bubble_switch)
                    {
                        script.cursor_color.a = 0.5f;
                        coloralpha = 0.5f;
                    }
                    else
                    {
                        script.cursor_color.a = 0.0f;
                        coloralpha = 0.0f;
                    }

                    // cursorの色を指定
                    this.GetComponent<Renderer>().material.color = script.cursor_color;

                    //エラー確認ViveSR.Error.がWORKなら正常に動いている．（フレームワークの方に内蔵済みだからいらないかも）
                    if (SRanipal_Eye_API.GetEyeData(ref eye) == ViveSR.Error.WORK)
                    {
                        //一応機器が正常に動いてる時の処理をここにかける
                    }
                    //-------------------------------------------

                    //?取得呼び出し-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------

                    //②まぶたの開き具合------------（HMDを被ってなくても1が返ってくる？？謎）
                    //左のまぶたの開き具合を取得
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out script.LeftBlink, eye))
                    {
                        //値が有効なら左のまぶたの開き具合を表示
                        //Debug.Log("Left Blink" + script.LeftBlink);
                    }
                    //右のまぶたの開き具合を取得
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out script.RightBlink, eye))
                    {
                        //値が有効なら右のまぶたの開き具合を表示
                        //Debug.Log("Right Blink" + script.RightBlink);
                    }
                    //------------------------------

                    //④焦点情報--------------------
                    //radius, maxDistance，CombinefocusableLayerは省略可
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                        //Debug.Log("Combine Focus Point" + CombineFocus.point.x + ", " + CombineFocus.point.y + ", " + CombineFocus.point.z);
                        //Debug.Log("new = "  + );
                        //if (Mathf.Abs(new_eye_position.magnitude - old_eye_position.magnitude)
                        // && new_eye_position.magnitude > script.pointvalue2

                        new_eye_position = CombineFocus.point;

                        //if (script.test_id== 0)
                        //{
                        //    Debug.Log("OKOKOK");
                        //    Debug.Log(new_eye_position);
                        //}

                        if ((new_eye_position.magnitude - old_eye_position.magnitude >= script.pointvalue) && script.BlinkCount == 0)
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

                    if (script.BlinkCount > -1)
                    {
                        // objsから１つずつobj変数に取り出す
                        foreach (GameObject obj in objs)
                        {
                            //if (obj.name != script.target_clone.name)
                            //{
                            //    obj.GetComponent<Renderer>().material.color = Color.white;
                            //}
                            // objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
                            float distance = Vector3.Distance(obj.transform.position, transform.position);

                            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
                            if (nearDistance == 0 || nearDistance > distance)
                            {

                                // nearDistanceを更新
                                nearDistance = distance;

                                // searchTargetObjを更新
                                searchTargetObj = obj;
                                if (nearDistance < (script.Depth / 16))
                                {
                                    script.DwellTarget = searchTargetObj;
                                    script.pointvalue = 0.01f - (nearDistance * nearDistance / 10);
                                }
                                else
                                {
                                    script.DwellTarget = null;
                                    script.pointvalue = 0.000001f;
                                }

                                if (script.RayTarget == searchTargetObj)
                                {
                                    script.pointvalue = script.pointvalue2;
                                }
                            }
                        }
                    }

                    // 最も近かったオブジェクトを返す
                    // 注視していたターゲットが変わった（連続注視が途切れた）場合
                    // searchTargetObj.GetComponent<Renderer>().material.color = Color.green;
                    if (oldNearObj != searchTargetObj || nearDistance > (script.Depth / 16))
                    {
                        script.same_target = false;
                        script.select_target_id = -1;
                        searchTargetObj.GetComponent<target_para_set>().dtime = 0;
                        script.BlinkFlag = 0;
                        //searchTargetObj.GetComponent<Renderer>().material.color = Color.yellow;

                        if (nearDistance * 2 + script.target_size < (script.Depth / 8))
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
                        //searchTargetObj.GetComponent<Renderer>().material.color = script.target_color;
                    }

                    //// ターゲットの距離に対する誘目対策
                    //if (nearDistance * 2 + script.target_size > (script.Depth / 8))
                    //{
                    //    script.pointvalue = 0.0001f;
                    //}
                    //else
                    //{
                    //    script.pointvalue = 0.05f;
                    //}

                    // 注視しているターゲットを更新
                    oldNearObj = searchTargetObj;
                    //Debug.Log(searchTargetObj);

                    targetpoint = searchTargetObj.transform.position;
                    script.cursor_radious = nearDistance * 2 + script.target_size;
                    // Debug.Log("dis1 = " + nearDistance + ", pos = " + this.transform.position);

                    // カーソルの大きさの調整
                    if (nearDistance * 2 + script.target_size < (script.Depth / 8))
                    {
                        script.cursor_radious = nearDistance * 2 + script.target_size * 2;
                        script.lens_flag = true;
                        //script.cursor_radious = nearDistance;
                    }
                    else
                    {
                        script.cursor_radious = (script.Depth / 8);
                        script.lens_flag = false;
                    }

                    if (Vector3.Distance(script.head_obj.transform.position, this.transform.position) < (script.Depth - 1.0f))
                    {
                        //script.bubble_switch = false;
                        if (script.test_id != 2)
                        {
                            this.GetComponent<Collider>().enabled = false;
                        }
                    } else
                    {
                        //script.bubble_switch= true;
                        if (script.test_id != 2)
                        {
                            this.GetComponent<Collider>().enabled = true;
                        }
                    }
                    //Debug.Log(Vector3.Distance(script.head_obj.transform.position, this.transform.position));

                    //if(script.selecting_target.name == oldNearObj.name)
                    //{
                    //    Debug.Log("OK");
                    //    script.pointvalue = 0.00f;
                    //} else
                    //{

                    //}

                    //// カーソルの透明度の調整
                    //float CursorColorAlpha = Vector3.Distance(script.head_obj.transform.position, transform.position);
                    ////float CursorColorAlpha = Vector3.Distance(script.head_obj.transform.position, searchTargetObj.transform.position);
                    ////Debug.Log("CursorColorAlpha = " + CursorColorAlpha);
                    //if (CursorColorAlpha < 1.0f)
                    //{
                    //    //script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.00f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.00f);
                    //}
                    //else if (CursorColorAlpha < script.Depth / 3)
                    //{
                    //    script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.001f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.001f);
                    //}
                    //else if (CursorColorAlpha < script.Depth / 2)
                    //{
                    //    //script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.005f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.005f);
                    //}
                    //else if (CursorColorAlpha < script.Depth / 1.5)
                    //{
                    //    //script.cursor_color.a = 0.0f;
                    //    //script.pointvalue = 0.01f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.01f);
                    //}
                    //else if (CursorColorAlpha < script.Depth && script.target_alpha_switch)
                    //{
                    //    //script.cursor_color.a = coloralpha * (CursorColorAlpha * CursorColorAlpha / (script.Depth * script.Depth * script.Depth));
                    //    //script.pointvalue = 0.01f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.01f);
                    //    //Debug.Log("clear = " + script.cursor_color.a);
                    //}
                    //else
                    //{
                    //    script.cursor_color.a = coloralpha;
                    //    //script.pointvalue = 0.03f;
                    //    script.pointvalue = pointvalue(script.pointvalue, 0.05f);
                    //}

                    //Debug.Log("dis2 = " + nearDistance + ", pos = " + this.transform.position);
                    //Debug.Log("dis3 = " + (this.transform.position.magnitude));
                    return searchTargetObj;
                }

                private float pointvalue(float nowpoint, float newpoint)
                {
                    return Mathf.Min(nowpoint, newpoint);
                }
            }
        }
    }
}
