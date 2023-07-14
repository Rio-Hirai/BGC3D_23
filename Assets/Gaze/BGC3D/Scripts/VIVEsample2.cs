using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class VIVEsample2 : MonoBehaviour
            {

                //⓪取得呼び出し-----------------------------
                //呼び出したデータ格納用の関数
                EyeData eye;
                //-------------------------------------------

                //①どのぐらいまぶたを開いてるか-----------------
                //呼び出したデータ格納用の関数
                float LeftOpenness;
                float RightOpenness;
                //-------------------------------------------

                //②視線の起点の座標(角膜の中心）mm単位------
                //呼び出したデータ格納用の関数
                Vector3 LeftGazeOrigin;
                Vector3 RightGazeOrigin;
                //-------------------------------------------

                //③瞳孔の位置-------------------------------
                //呼び出したデータ格納用の関数
                Vector2 LeftPupilPosition;
                Vector2 RightPupilPosition;
                //-------------------------------------------

                //④瞳孔の直径-------------------------------
                //呼び出したデータ格納用の関数
                float LeftPupiltDiameter;
                float RightPupiltDiameter;
                //-------------------------------------------

                // サーバー接続
                public GameObject Server;
                private receiver script;
                
                void Start()
                {
                    script = Server.GetComponent<receiver>();
                }

                //1フレーム毎に実行
                void Update()
                {
                    //⓪取得呼び出し-----------------------------
                    SRanipal_Eye_API.GetEyeData(ref eye);
                    //-------------------------------------------


                    //①どのぐらいまぶたを開いてるか-----------------
                    //左目を開いてるかが妥当ならば取得　なぜかHMD付けてなくてもTrueがでる，謎．
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        //LeftOpenness = eye.verbose_data.left.eye_openness;
                        //Debug.Log("Left Openness：" + LeftOpenness);
                    }

                    //右目を開いてるかが妥当ならば取得　なぜかHMD付けてなくてもTrueがでる，謎．
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY))
                    {
                        //RightOpenness = eye.verbose_data.right.eye_openness;
                        //Debug.Log("Right Openness：" + RightOpenness);
                    }
                    //-------------------------------------------


                    //②視線の起点の座標(角膜の中心）mm単位------ -
                    ////左目の眼球データ（視線原点）が妥当ならば取得　目をつぶるとFalse　判定精度はまあまあ
                    //if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY))
                    //{
                    //    //LeftGazeOrigin = eye.verbose_data.left.gaze_origin_mm;
                    //    //Debug.Log("Left GazeOrigin：" + LeftGazeOrigin.x + ", " + LeftGazeOrigin.y + ", " + LeftGazeOrigin.z);
                    //}

                    //////右目の眼球データ（視線原点）が妥当ならば取得　目をつぶるとFalse　判定精度はまあまあ
                    //if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY))
                    //{
                    //    //RightGazeOrigin = eye.verbose_data.right.gaze_origin_mm;
                    //    //Debug.Log("Right GazeOrigin：" + RightGazeOrigin.x + ", " + RightGazeOrigin.y + ", " + RightGazeOrigin.z);
                    //}
                    ////-------------------------------------------


                    //③瞳孔の位置-------------------------------
                    ////左目の瞳孔の正規化位置が妥当ならば取得　目をつぶるとFalse 判定精度は微妙
                    //if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY))
                    //{
                    //    //LeftPupilPosition = eye.verbose_data.left.pupil_position_in_sensor_area;
                    //    //Debug.Log("Left Pupil Position：" + LeftPupilPosition.x + ", " + LeftPupilPosition.y);
                    //}

                    //////右目の瞳孔の正規化位置が妥当ならば取得　目をつぶるとFalse　判定精度は微妙
                    //if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_POSITION_IN_SENSOR_AREA_VALIDITY))
                    //{
                    //    //RightPupilPosition = eye.verbose_data.right.pupil_position_in_sensor_area;
                    //    //Debug.Log("Right GazeOrigin：" + RightPupilPosition.x + ", " + RightPupilPosition.y);
                    //}
                    ////-------------------------------------------


                    //④瞳孔の直径-------------------------------
                    //左目の瞳孔の直径が妥当ならば取得　目をつぶるとFalse 判定精度はまあまあ
                    if (eye.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        LeftPupiltDiameter = eye.verbose_data.left.pupil_diameter_mm;
                        //Debug.Log("Left Pupilt Diameter：" + LeftPupiltDiameter);
                        script.LeftPupiltDiameter_flag = 1;
                        script.LeftPupiltDiameter = LeftPupiltDiameter;
                    } else
                    {
                        script.LeftPupiltDiameter_flag = 0;
                    }

                    ////右目の瞳孔の直径が妥当ならば取得　目をつぶるとFalse　判定精度はまあまあ
                    if (eye.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY))
                    {
                        RightPupiltDiameter = eye.verbose_data.right.pupil_diameter_mm;
                        //Debug.Log("Right Pupilt Diameter：" + RightPupiltDiameter);
                        script.RightPupiltDiameter_flag = 1;
                        script.RightPupiltDiameter = RightPupiltDiameter;
                    } else
                    {
                        script.RightPupiltDiameter_flag = 0;
                    }
                    //-------------------------------------------
                }
            }
        }
    }
}
