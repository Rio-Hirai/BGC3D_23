using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class gaze_data : MonoBehaviour
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
                int CombinefocusableLayer = 0;
                //------------------------------

                //①どのぐらいまぶたを開いてるか-----------------
                //呼び出したデータ格納用の関数
                float LeftOpenness;
                float RightOpenness;
                //-------------------------------------------

                //④瞳孔の直径-------------------------------
                //呼び出したデータ格納用の関数
                float LeftPupiltDiameter;
                float RightPupiltDiameter;
                //-------------------------------------------

                private receiver script;

                void Start()
                {
                    // サーバと接続
                    script = this.GetComponent<receiver>();
                }

                //1フレーム毎に実行
                void FixedUpdate()
                {
                    ////おまけ------------------------------------
                    ////エラー確認ViveSR.Error.がWORKなら正常に動いている．（フレームワークの方に内蔵済みだからいらないかも）
                    //if (SRanipal_Eye_API.GetEyeData(ref eye) == ViveSR.Error.WORK)
                    //{
                    //    //一応機器が正常に動いてる時の処理をここにかける
                    //}
                    ////-------------------------------------------


                    ////⓪取得呼び出し-----------------------------
                    //SRanipal_Eye_API.GetEyeData(ref eye);
                    ////-------------------------------------------


                    ////①瞳孔位置---------------------（HMDを被ると検知される，目をつぶっても位置は返すが，HMDを外すとと止まる．目をつぶってるときはどこの値返してんのか謎．一応まぶた貫通してるっぽい？？？）
                    ////左の瞳孔位置を取得
                    //if (SRanipal_Eye.GetPupilPosition(EyeIndex.LEFT, out LeftPupil))
                    //{
                    //    //値が有効なら左の瞳孔位置を表示
                    //    //Debug.Log("Left Pupil" + LeftPupil.x + ", " + LeftPupil.y);
                    //}
                    ////右の瞳孔位置を取得
                    //if (SRanipal_Eye.GetPupilPosition(EyeIndex.RIGHT, out RightPupil))
                    //{
                    //    //値が有効なら右の瞳孔位置を表示
                    //    //Debug.Log("Right Pupil" + RightPupil.x + ", " + RightPupil.y);
                    //}
                    ////------------------------------


                    ////②まぶたの開き具合------------（HMDを被ってなくても1が返ってくる？？謎）
                    ////左のまぶたの開き具合を取得
                    //if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftBlink, eye))
                    //{
                    //    //値が有効なら左のまぶたの開き具合を表示
                    //    //Debug.Log("Left Blink" + LeftBlink);
                    //}
                    ////右のまぶたの開き具合を取得
                    //if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out RightBlink, eye))
                    //{
                    //    //値が有効なら右のまぶたの開き具合を表示
                    //    //Debug.Log("Right Blink" + RightBlink);
                    //}
                    ////------------------------------


                    ////③視線情報--------------------（目をつぶると検知されない）
                    ////両目の視線情報が有効なら視線情報を表示origin：起点，direction：レイの方向
                    //if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out CombineGazeRayorigin, out CombineGazeRaydirection, eye))
                    //{
                    //    //Debug.Log("COMBINE GazeRayorigin" + CombineGazeRayorigin.x + ", " + CombineGazeRayorigin.y + ", " + CombineGazeRayorigin.z);
                    //    //Debug.Log("COMBINE GazeRaydirection" + CombineGazeRaydirection.x + ", " + CombineGazeRaydirection.y + ", " + CombineGazeRaydirection.z);
                    //}

                    ////左目の視線情報が有効なら視線情報を表示origin：起点，direction：レイの方向
                    //if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out LeftGazeRayorigin, out LeftGazeRaydirection, eye))
                    //{
                    //    //Debug.Log("Left GazeRayorigin" + LeftGazeRayorigin.x + ", " + LeftGazeRayorigin.y + ", " + LeftGazeRayorigin.z);
                    //    //Debug.Log("Left GazeRaydirection" + LeftGazeRaydirection.x + ", " + LeftGazeRaydirection.y + ", " + LeftGazeRaydirection.z);
                    //}


                    ////右目の視線情報が有効なら視線情報を表示origin：起点，direction：レイの方向
                    //if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out RightGazeRayorigin, out RightGazeRaydirection, eye))
                    //{
                    //    //Debug.Log("Right GazeRayorigin" + RightGazeRayorigin.x + ", " + RightGazeRayorigin.y + ", " + RightGazeRayorigin.z);
                    //    //Debug.Log("Right GazeRaydirection" + RightGazeRaydirection.x + ", " + RightGazeRaydirection.y + ", " + RightGazeRaydirection.z);
                    //}
                    ////------------------------------

                    ////④焦点情報--------------------
                    ////radius, maxDistance，CombinefocusableLayerは省略可
                    //if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    //{
                    //    //Debug.Log("Combine Focus Point" + CombineFocus.point.x + ", " + CombineFocus.point.y + ", " + CombineFocus.point.z);
                    //}
                    ////------------------------------

                    //get_gaze_data(); // データ取得
                }

                public void get_gaze_data()
                {
                    //⓪取得呼び出し-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------

                    //①どのぐらいまぶたを開いてるか-----------------
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        LeftOpenness = eye.verbose_data.left.eye_openness;
                    }
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        RightOpenness = eye.verbose_data.right.eye_openness;
                    }
                    //-------------------------------------------

                    //②まぶたの開き具合------------
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftBlink, eye))
                    {
                    }
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out RightBlink, eye))
                    {
                    }
                    //------------------------------

                    //③瞳孔の直径-------------------------------
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        LeftPupiltDiameter = eye.verbose_data.left.pupil_diameter_mm;
                    }
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        RightPupiltDiameter = eye.verbose_data.right.pupil_diameter_mm;
                    }
                    //-------------------------------------------

                    //④焦点情報--------------------
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                    }
                    //------------------------------

                    //// データをリストに格納
                    //// タイムスタンプ，タスク番号，焦点座標x，焦点座標y，瞳孔径・右，瞳孔径・左，開き具合・右，開き具合・左，頭部角度・x，頭部角度・y，頭部角度・z
                    script.tasklogs3.Add(script.test_time + "," + (script.task_num) + "," + (CombineFocus.point.x) + "," + (CombineFocus.point.y) + "," + (RightPupiltDiameter) + "," + (LeftPupiltDiameter) + "," + (RightOpenness) + "," + (LeftOpenness) + "," + (script.HMDRotation.x) + "," + (script.HMDRotation.y) + "," + (script.HMDRotation.z));
                }

                public string get_gaze_data2()
                {
                    //⓪取得呼び出し-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------

                    //①どのぐらいまぶたを開いてるか-----------------
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        LeftOpenness = eye.verbose_data.left.eye_openness;
                    }
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        RightOpenness = eye.verbose_data.right.eye_openness;
                    }
                    //-------------------------------------------

                    //②まぶたの開き具合------------
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftBlink, eye))
                    {
                    }
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out RightBlink, eye))
                    {
                    }
                    //------------------------------

                    //③瞳孔の直径-------------------------------
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        LeftPupiltDiameter = eye.verbose_data.left.pupil_diameter_mm;
                    }
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        RightPupiltDiameter = eye.verbose_data.right.pupil_diameter_mm;
                    }
                    //-------------------------------------------

                    //④焦点情報--------------------
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                    }
                    //------------------------------

                    //// データをリストに格納
                    //// タイムスタンプ，タスク番号，焦点座標x，焦点座標y，瞳孔径・右，瞳孔径・左，開き具合・右，開き具合・左，頭部角度・x，頭部角度・y，頭部角度・z
                    return (script.test_time + "," + (script.task_num) + "," + (CombineFocus.point.x) + "," + (CombineFocus.point.y) + "," + (RightPupiltDiameter) + "," + (LeftPupiltDiameter) + "," + (RightOpenness) + "," + (LeftOpenness) + "," + (script.HMDRotation.x) + "," + (script.HMDRotation.y) + "," + (script.HMDRotation.z));
                }

                public void get_gaze_data_3()
                {
                    //⓪取得呼び出し-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------

                    //①どのぐらいまぶたを開いてるか-----------------
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        LeftOpenness = eye.verbose_data.left.eye_openness;
                    }
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        RightOpenness = eye.verbose_data.right.eye_openness;
                    }
                    //-------------------------------------------

                    //②まぶたの開き具合------------
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftBlink, eye))
                    {
                    }
                    if (SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out RightBlink, eye))
                    {
                    }
                    //------------------------------

                    //③瞳孔の直径-------------------------------
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        LeftPupiltDiameter = eye.verbose_data.left.pupil_diameter_mm;
                    }
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        RightPupiltDiameter = eye.verbose_data.right.pupil_diameter_mm;
                    }
                    //-------------------------------------------

                    //④焦点情報--------------------
                    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out CombineRay, out CombineFocus/*, CombineFocusradius, CombineFocusmaxDistance, CombinefocusableLayer*/))
                    {
                    }
                    //------------------------------

                    //// データをリストに格納
                    //// タイムスタンプ，タスク番号，焦点座標x，焦点座標y，瞳孔径・右，瞳孔径・左，開き具合・右，開き具合・左，頭部角度・x，頭部角度・y，頭部角度・z
                    ///
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var sw = new StreamWriter(@"C:\test\test1.txt", true, Encoding.GetEncoding("shift_jis"));
                    sw.Write(script.test_time + "," + (script.task_num) + "," + (CombineFocus.point.x) + "," + (CombineFocus.point.y) + "," + (RightPupiltDiameter) + "," + (LeftPupiltDiameter) + "," + (RightOpenness) + "," + (LeftOpenness) + "," + (script.HMDRotation.x) + "," + (script.HMDRotation.y) + "," + (script.HMDRotation.z)); ;

                    stopwatch.Stop();
                    Console.WriteLine("計測時間【StreamWriter】: " + stopwatch.Elapsed.TotalMilliseconds + "ms");
                    File.Delete(@"C:\test\test1.txt");
                }
            }
        }
    }
}
