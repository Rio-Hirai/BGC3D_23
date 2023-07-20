using System.Collections;
using System.Collections.Generic;
using Valve.VR;
using Valve.VR.Extras;
using UnityEngine;
using System.IO;
using System;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using UnityEngine.UIElements;
using UnityEngine.XR.OpenXR.Input;
using ViveSR.anipal.Eye;
using UnityEngine.XR;
using UnityEngine.Rendering.PostProcessing;

public class receiver : MonoBehaviour
{
    // 実験用パラメータ
    public enum test_pattern_list
    {
        Zero_Cursor, // カーソルなし
        Bubble_Gaze_Cursor1, // BGC
        Bubble_Gaze_Cursor2, // BGC with RayCast
        Gaze_Raycast, // 視線によるレイキャスト
        Controller_Raycast // コントローラによるレイキャスト
    }
    public test_pattern_list test_pattern = test_pattern_list.Bubble_Gaze_Cursor1; //　手法切り替え用のリスト構造
    private int target_p_id;
    public enum target_pattern_list
    {
        Null, // ターゲット無し
        High_Density, // 高密度条件
        High_Occlusion, // 高オクルージョン条件
        Density_and_Occlusion // 密度＆オクルージョン条件
    }
    public target_pattern_list target_pattern = target_pattern_list.High_Density; //　条件切り替え用のリスト構造
    public bool bubble_switch; // バブルカーソルの表示・非表示
    public float pointvalue; // サッケード運動に対する閾値
    public float pointvalue2; // 同上（ほぼ使ってない）

    public int test_id;
    public int tester_id; // 被験者のID
    public string tester_name; // 被験者の名前
    public int target_amount_all; // ターゲットの総数
    public int target_amount_select; // 選択する数
    public int target_amount_count; // 繰り返し回数
    private string input_start_time;
    public int task_num = 0; // タスクの番号
    public int select_target_id = 0; // 選択されたターゲットのID
    public float test_time = 0; // 実験時間
    public float Depth; // 奥行距離

    public string now_test_pattern;
    public string now_target_pattern;

    //②まぶたの開き具合------------
    //左のまぶたの開き具合格納用関数
    public float LeftBlink;
    //右のまぶたの開き具合格納用関数
    public float RightBlink;
    //------------------------------
    public int BlinkFlag; // これがTrueになった瞬間にターゲット選択を確定させるので，瞬き関係はこれを弄るだけで十分．
    public int BlinkCount;
    public int BlinkSwitch;
    public float BlinkTime;

    // 注視時間
    public float set_dtime = 0.6f;
    public GameObject selecting_target;
    public GameObject DwellTarget;
    public GameObject RayTarget;

    private List<int> numbers;
    public List<int> tasknums;
    public List<string> tasklogs;
    public List<string> tasklogs2;
    public List<string> tasklogs3;
    private List<float> task_start_time;
    private List<float> task_end_time;
    private string filePath;
    public bool same_target;
    public bool taskflag;
    public bool output_flag;
    public bool next_step__flag;
    public bool error_output_flag; //強制中断用フラグ
    //private bool target_pos_set = false;
    private int logoutput_count = 0;

    // ターゲットの位置調整
    // public GameObject CameraRig;
    public GameObject Camera;
    public GameObject[] target_set;

    // ターゲットのパラメータ
    public float target_size;
    public float target_distance;
    public float point_size;
    public int target_amount;
    public GameObject target_objects;
    //public List<int> target_ids = new List<int>();
    //public int target_id;

    // 誘目対策
    //public float pointvalue2;

    // 色設定
    public Color target_color;
    public Color select_color;
    public Color cursor_color;

    // 各種機能切り替え
    public bool bubblegaze_switch;
    public bool gazeraycast_switch;
    public bool controller_switch;

    public GameObject bubblegaze;
    public GameObject gazeraycast;
    public GameObject controller_R;
    public GameObject controller_L;

    public bool laserswitch;
    public bool lens_switch;
    public bool cursor_switch;
    public bool target_alpha_switch;

    // クローンID
    public float target_id;
    public GameObject target_clone;

    //public int select_flag;
    //public int select_cnt;
    //public int select_cnt_tmp;
    public byte color_alpha;
    public float cursor_radious;

    public float LeftPupiltDiameter;
    public float RightPupiltDiameter;
    public int LeftPupiltDiameter_flag;
    public int RightPupiltDiameter_flag;

    //public GameObject eyePoint;
    public int select_flag_2;
    public Vector3 old_eye_position;
    public Vector3 new_eye_position;
    //public int select_flag_gaze;
    //public int select_flag_other;
    public GameObject head_obj;

    // Bubble Gaze Lens用
    public GameObject Lens_Object;
    public bool lens_flag;
    public bool lens_flag2;

    // コントローラボタン
    // [SerializeField] SteamVR_Input_Sources hand;
    // [SerializeField] SteamVR_Action_Boolean action;
    private SteamVR_Action_Boolean GrabG = SteamVR_Actions.default_GrabGrip;
    //public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
    // public SteamVR_Behaviour_Pose pose;
    // bool isActive = false;

    //結果の格納用Boolean型関数grapgrip
    public Boolean grapgrip;
    public Boolean trackpad;

    private int switch_flag = 0;

    // 効果音
    public AudioClip sound_OK;
    public AudioClip sound_NG;
    public AudioClip sound_START;
    public AudioClip sound_END;
    AudioSource audioSource;

    private float test_time_tmp;

    public gaze_data gaze_data;
    public float DeltaTime;
    public bool cd_flag;

    //ファイル生成
    private StreamWriter streamWriter_gaze;

    private Quaternion HMDRotationQ; // HMDの角度
    public Vector3 HMDRotation; // HMDの角度

    // 画面明度
    public float brightness;
    private ColorGrading colorgrading;

    void Start()
    {
        // モード管理
        switch (test_pattern.ToString())
        {
            case "Zero_Cursor":
                test_id = 0;
                break;
            case "Bubble_Gaze_Cursor1":
                test_id = 1;
                break;
            case "Bubble_Gaze_Cursor2":
                test_id = 2;
                break;
            case "Gaze_Raycast":
                test_id = 3;
                break;
            case "Controller_Raycast":
                test_id = 4;
                break;
            default:
                test_id = 0;
                break;

        }

        // タスク条件管理
        switch (target_pattern.ToString())
        {
            case "High_Density":
                target_p_id = 1;
                target_amount_all = 25;
                target_amount_select = 25;
                target_amount_count = 1;
                Depth = 5.0f;
                break;
            case "High_Occlusion":
                target_p_id = 2;
                target_amount_all = 4;
                target_amount_select = 24;
                target_amount_count = 6;
                Depth = 5.0f;
                break;
            case "Density_and_Occlusion":
                target_p_id = 3;
                target_amount_all = 124;
                target_amount_select = 25;
                target_amount_count = 1;
                Depth = 3.5f;
                break;
            default:
                target_p_id = 0;
                target_amount_all = 1;
                target_amount_select = 1;
                target_amount_count = 1;
                break;

        }

        // モード管理
        if (test_id == 0)
        {
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false;
        }
        else if (test_id < 3)
        {
            bubblegaze_switch = true;
            bubblegaze.SetActive(true);
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false;

            if (test_id == 2)
            {
                gazeraycast.SetActive(true);
            }
        }
        else if (test_id == 3)
        {
            gazeraycast_switch = true;
            gazeraycast.SetActive(true);
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false;
        }
        else if (test_id == 4)
        {
            controller_switch = true;
            controller_R.GetComponent<SteamVR_LaserPointer>().active = true;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = true;
        }

        if (test_id == 2)
        {
            bubblegaze.GetComponent<Collider>().enabled = false;
            
        }
        else
        {
            bubblegaze.GetComponent<Collider>().enabled = true;
        }

        //　ターゲットの初期化
        set_testpattern();

        //ファイル名作成
        DateTime dt = DateTime.Now;
        input_start_time = dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
        filePath = Application.dataPath + "/Gaze_Team23/BGC3D/Scripts/test_results/" + "test_id = " + test_id + "___" + "target_p_id = " + target_p_id + "___" + "tester_id  = " + tester_id + "___" + tester_name + "___" + input_start_time;
        //filePath = Application.dataPath + "/BGS3D/Scripts/test_results/" + test_id + "_" + test_pattern + "_" + target_p_id + "_" + target_pattern + "_" + tester_id + "_" + tester_name + ".txt";
        streamWriter_gaze = File.AppendText(filePath + "_gaze_data.csv");
        result_output_every("timestamp,taskNo,gaze_x,gaze_y,pupil_r,pupil_l,blink_r,blink_l,hmd_x,hmd_y,hmd_z", streamWriter_gaze, false);

        // ログ作成
        tasklogs = new List<string>();
        task_start_time = new List<float>();
        task_end_time = new List<float>();

        // ターゲット作成
        target_id = 0;
        for (int i = 0; i < target_amount; i++)
        {
            float target_x = 0.0f;
            float target_y = 0.0f;
            float target_z = 0.0f;
            while (!(target_x > target_distance || target_x < -target_distance))
            {
                target_x = UnityEngine.Random.Range(-(target_distance + 1.0f), target_distance + 1.0f);
            }
            while (!(target_z > target_distance || target_z < -target_distance))
            {
                target_z = UnityEngine.Random.Range(-(target_distance + 1.0f), target_distance + 1.0f);
            }
            //target_x = Random.Range(-1.5f, 1.5f);
            target_y = UnityEngine.Random.Range(-1.0f, 2.2f);
            //target_z = Random.Range(-1.5f, 1.5f);
            Instantiate(target_objects, new Vector3(target_x, target_y, target_z), Quaternion.identity);
        }

        audioSource = GetComponent<AudioSource>();



        //// コントローラ設定
        //if (pose == null)
        //    pose = this.GetComponent<SteamVR_Behaviour_Pose>();
        //if (pose == null)
        //    Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);

        //if (interactWithUI == null)
        //    Debug.LogError("No ui interaction action has been set on this component.", this);

        // 画面の明度設定
        colorgrading = this.GetComponent<ColorGrading>();
    }

    // Update is called once per frame
    [Obsolete]
    void Update()
    {
        if (SteamVR_Actions.default_SnapTurnLeft.GetStateDown(SteamVR_Input_Sources.Any) && switch_flag == 0)
        {
            Debug.Log("LEFT");
            if (test_id > 1)
            {
                test_id -= 1;
            }
            else
            {
                test_id = 4;
            }
            switch_flag = 1;
        }
        else if (SteamVR_Actions.default_SnapTurnRight.GetStateDown(SteamVR_Input_Sources.Any) && switch_flag == 0)
        {
            Debug.Log("RIGHT");
            if (test_id < 4)
            {
                test_id += 1;
            }
            else
            {
                test_id = 1;
            }
            switch_flag = 1;
        }
        else
        {
            switch_flag = 0;
        }
        //trackpad = SteamVR_Actions.default_Teleport.GetStateDown(SteamVR_Input_Sources.Any);


        if (now_test_pattern != test_pattern.ToString() || switch_flag == 1)
        {
            now_test_pattern = test_pattern.ToString();

            if (DwellTarget != null)
            {
                DwellTarget.GetComponent<Renderer>().material.color = Color.white;
            }


            // モード管理
            //switch (test_pattern.ToString())
            //{
            //    case "Zero_Cursor":
            //        test_id = 0;
            //        break;
            //    case "Bubble_Gaze_Cursor1":
            //        test_id = 1;
            //        break;
            //    case "Bubble_Gaze_Cursor2":
            //        test_id = 2;
            //        break;
            //    case "Bubble_Gaze_Cursor3":
            //        test_id = 5;
            //        break;
            //    case "Gaze_Raycast":
            //        test_id = 3;
            //        break;
            //    case "Controller_Raycast":
            //        test_id = 4;
            //        break;
            //    default:
            //        test_id = 0;
            //        break;
            //}

            // パラメータ初期化
            bubblegaze_switch = false;
            gazeraycast_switch = false;
            controller_switch = false;
            bubblegaze.SetActive(false);
            gazeraycast.SetActive(false);
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false;

            // パラメータ更新
            if (test_id == 0)
            {
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false;
            }
            else if (test_id < 3)
            {
                bubblegaze_switch = true;
                bubblegaze.SetActive(true);
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false;

                if (test_id == 2)
                {
                    gazeraycast.SetActive(true);
                }
            }
            else if (test_id == 3)
            {
                gazeraycast_switch = true;
                gazeraycast.SetActive(true);
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false;
            }
            else if (test_id == 4)
            {
                controller_switch = true;
                controller_R.GetComponent<SteamVR_LaserPointer>().active = true;
                controller_L.GetComponent<SteamVR_LaserPointer>().active = true;
            }

            if (test_id == 2)
            {
                bubblegaze.GetComponent<Collider>().enabled = false;
            }
            else
            {
                bubblegaze.GetComponent<Collider>().enabled = true;
            }
        }

        if (now_target_pattern != target_pattern.ToString())
        {
            now_target_pattern = target_pattern.ToString();
        }


        // ターゲット位置の調整
        //grapgrip = GrabG.GetState(SteamVR_Input_Sources.RightHand);
        //Debug.Log(SteamVR_Actions.default_GrabGrip.GetState(SteamVR_Input_Sources.Any));
        grapgrip = SteamVR_Actions.default_GrabGrip.GetState(SteamVR_Input_Sources.Any);
        // grapgrip = interactWithUI.GetState(pose.inputSource);
        if (grapgrip)
        {
            //target_pos_set = true;

            target_set[target_p_id - 1].SetActive(true);
            Transform myTransform = target_set[target_p_id - 1].transform;
            Vector3 pos = myTransform.position;
            pos.y = Camera.transform.position.y;
            pos.z = Depth;
            myTransform.position = pos;
            //myTransform.LookAt(Camera.transform);
        }

        // 時間計測
        test_time += Time.deltaTime;
        //DeltaTime = Time.deltaTime;

        // タスクの推移管理
        if (select_target_id == 999 && taskflag == false)
        {
            taskflag = true;
            tasklogs.Add("");
            task_start_time.Add(test_time);
            test_time_tmp = test_time;
            logoutput_count = 0;
        }

        // タスクチェック
        if (taskflag)
        {
            if (select_target_id != -1 && select_target_id != 999 && same_target == false)
            {
                tasklogs2.Add((task_num + 1) + "," + tasknums[task_num] + "," + select_target_id + "," + (test_time - test_time_tmp));
                test_time_tmp = test_time;
                if (select_target_id == tasknums[task_num])
                {
                    tasklogs2[tasklogs2.Count - 1] += ("," + (test_time - task_start_time[task_num]) + "," + logoutput_count);
                }
                else
                {
                    logoutput_count++;
                }

                if (select_target_id == tasknums[task_num])
                {
                    same_target = true;

                    tasklogs[task_num] += ("select_target = " + select_target_id + ": " + test_time + "\n");
                    //tasklogs2.Add(task_num + "," + tasknums[task_num] + "," + select_target_id + "," + ",");

                    if (task_num < target_amount_select)
                    {
                        task_end_time.Add(test_time);
                        task_num++;
                        test_time_tmp = 0;
                        audioSource.PlayOneShot(sound_OK);
                        taskflag = false;
                    }
                }
                else
                {
                    same_target = true;
                    tasklogs[task_num] += ("select_target = " + select_target_id + ": " + test_time + "\n");
                    audioSource.PlayOneShot(sound_NG);
                }
            } else if (select_target_id == 999)
            {
                //audioSource.PlayOneShot(sound_START);
            }

            if (task_num == target_amount_select && output_flag == false)
            {
                output_flag = true;
                audioSource.PlayOneShot(sound_END);
                result_output();
                result_output_csv();
                //result_output_csv2();
                result_output_every("", streamWriter_gaze, true);
            }
        }

        if (error_output_flag)
        {
            error_output_flag = false;
            audioSource.PlayOneShot(sound_END);
            error_output();
            result_output_csv();
            //result_output_csv2();
            result_output_every("", streamWriter_gaze, true);
        }

        //if (laserswitch == false)
        //{
        //    Rlaser.GetComponent<SteamVR_LaserPointer>().enabled = false;
        //    Llaser.GetComponent<SteamVR_LaserPointer>().enabled = false;
        //}
        //else
        //{
        //    Rlaser.GetComponent<SteamVR_LaserPointer>().enabled = true;
        //    Llaser.GetComponent<SteamVR_LaserPointer>().enabled = true;
        //}
        if (lens_switch)
        {
            if (lens_flag)
            {
                Lens_Object.SetActive(true);
            }
            else
            {
                Lens_Object.SetActive(false);
            }
        }

        // Blink3();
        // Blink();
        //result_output_every("test,test,test.test", streamWriter_gaze, false);

        // Head（ヘッドマウンドディスプレイ）の情報を一時保管-----------
        //回転座標をクォータニオンで値を受け取る
        HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head);
        // 取得した値をクォータニオン → オイラー角に変換
        HMDRotation = HMDRotationQ.eulerAngles;
        //--------------------------------------------------------------

        // 視線関係のデータ取得
        // gaze_data.get_gaze_data();
        if(output_flag == false && taskflag == true) result_output_every(gaze_data.get_gaze_data2(), streamWriter_gaze, false);

        // 画面明度の変更
        if (colorgrading != null)
        {
            //colorgrading.brightness = 100.0f;
        }
    }

    public void result_output()
    {
        Debug.Log("data_input_start!!");

        //ファイル生成
        StreamWriter streamWriter = File.AppendText(filePath + ".txt");
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("target_pattorn:");
        for (int i = 0; i < target_amount_all - 5; i++)
        {
            streamWriter.WriteLine(tasknums[i]);
            //streamWriter.WriteLine(" ");
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("target_pattorn:");
        for (int i = 0; i < target_amount_select; i++)
        {
            streamWriter.WriteLine(tasknums[i]);
            //streamWriter.WriteLine(" ");
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");

        // 開始時間
        streamWriter.WriteLine("test_start_time: " + task_start_time[0]);
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");

        // 各タスクの計測を追記
        for (int i = 0; i < target_amount_select; i++)
        {
            streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_start: " + task_start_time[i]);
            streamWriter.WriteLine(tasklogs[i]);
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_end: " + task_end_time[i]);
            streamWriter.WriteLine("task_time: " + ( task_end_time[i] - task_start_time[i]));
        }

        //// 各タスクの計測を追記3
        //streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
        //for (int i = 0; i < tasklogs2.Count; i++)
        //{
        //    streamWriter.WriteLine(tasklogs2[i]);
        //}
        //streamWriter.WriteLine("\n-----------------------------------------------------------------------------------------\n");

        // 終了時間
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("test_end_time: " + task_end_time[target_amount_select-1]);

        // 後処理
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_input_end!!");
    }

    public void Blink3()
    {
        BlinkTime += Time.deltaTime;

        // 1回目
        if (RightBlink == 0 && BlinkSwitch == 0 && BlinkCount == 0)
        {
            BlinkSwitch = 1;
            BlinkCount = 1;
            BlinkTime = 0;
            Debug.Log("OK1");
        }

        // 2回目以降
        if (RightBlink == 0 && BlinkSwitch == 0 && BlinkCount > 0 && BlinkTime < 1.0f)
        {
            BlinkCount += 1;
            Debug.Log("OK=count" + BlinkCount);
        } else if (BlinkCount < 3 && BlinkTime > 1.0f)
        {
            BlinkCount = 0;
            Debug.Log("OK=count" + 0);
        }

        if (RightBlink > 0.3)
        {
            BlinkSwitch = 0;
            Debug.Log("OK=Switch" + 0);
        }

        // 3回以上瞬きをした場合
        if (BlinkCount > 2)
        {
            BlinkFlag = 1;
            Debug.Log("OKFlag");
        }
    }

    public void Blink()
    {
        BlinkTime += Time.deltaTime;

        if (RightBlink == 0 && BlinkSwitch == 0 && BlinkTime < 0.5)
        {
            BlinkCount += 1;
            BlinkTime = 0;
            BlinkSwitch = 1;
            Debug.Log("OK=count" + BlinkCount);
        } else if (BlinkTime > 1.0f)
        {
            BlinkCount = 0;
            BlinkTime = 0;
            BlinkFlag = 0;
            //Debug.Log("OK=count" + 0);
        } else if (BlinkTime > 0.3f)
        {
            BlinkFlag = 0;
        }

        if (RightBlink > 0.3 && RightBlink != 1)
        {
            BlinkSwitch = 0;
            //Debug.Log("OK=Switch" + 0);
        }

        // 3回以上瞬きをしあ場合
        if (BlinkCount > 1)
        {
            BlinkFlag = 1;
            BlinkCount = 0;
            BlinkTime = 0;
            //Debug.Log("OKFlag");
        }
    }

    public void result_output_csv()
    {
        Debug.Log("data_input_csv_start!!");

        //ファイル生成
        StreamWriter streamWriter = File.AppendText(filePath + ".csv");

        // 各タスクの計測を追記
        streamWriter.WriteLine("task,target,select,time,totaltime,totalerror");
        for (int i = 0; i < tasklogs2.Count; i++)
        {
            streamWriter.WriteLine(tasklogs2[i]);
        }

        // 後処理
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_input_end!!");
    }

    public void result_output_csv2()
    {
        Debug.Log("data_input_csv_start2!!");

        //ファイル生成
        StreamWriter streamWriter = File.AppendText(filePath + "_gaze_data.csv");

        // 各タスクの計測を追記
        streamWriter.WriteLine("timestamp,taskNo,gaze_x,gaze_y,pupil_r,pupil_l,blink_r,blink_l,hmd_x,hmd_y,hmd_z");
        for (int i = 0; i < tasklogs3.Count; i++)
        {
            streamWriter.WriteLine(tasklogs3[i]);
        }

        // 後処理
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_output_end2!!");
    }

    public void result_output_every(string data, StreamWriter streamWriter, bool endtask)
    {
        // これが呼び出されるたびに変数（第一引数の文字列）をcsvファイルに出力する
        // falseの場合は書き込み処理，trueの場合は閉じる処理
        if (endtask == false) streamWriter.WriteLine(data);

        if (endtask)
        {
            streamWriter.Close();
            Debug.Log("data_output_every!!");
        }
    }

    public void error_output()
    {
        Debug.Log("data_input_start!!");

        //ファイル生成
        StreamWriter streamWriter = File.AppendText(filePath + ".txt");
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("target_pattorn:");
        for (int i = 0; i < target_amount_all - 5; i++)
        {
            streamWriter.WriteLine(tasknums[i]);
            //streamWriter.WriteLine(" ");
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("target_pattorn:");
        for (int i = 0; i < target_amount_select; i++)
        {
            streamWriter.WriteLine(tasknums[i]);
            //streamWriter.WriteLine(" ");
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");

        // 開始時間
        streamWriter.WriteLine("test_start_time: " + task_start_time[0]);
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");

        // 各タスクの計測を追記
        for (int i = 0; i < task_num; i++)
        {
            streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_start: " + task_start_time[i]);
            streamWriter.WriteLine(tasklogs[i]);
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_end: " + task_end_time[i]);
            streamWriter.WriteLine("task_time: " + (task_end_time[i] - task_start_time[i]));
        }

        // 各タスクの計測を追記2
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
        streamWriter.WriteLine("task,target,select,time,totaltime,totalerror");
        for (int i = 0; i < tasklogs2.Count; i++)
        {
            streamWriter.WriteLine(tasklogs2[i]);
        }
        streamWriter.WriteLine("\n-----------------------------------------------------------------------------------------\n");


        // 終了時間
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("test_end_time: " + task_end_time[task_num - 1]);

        // 後処理
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_input_end!!");
    }

    void set_testpattern()
    {
        numbers = new List<int>();
        tasknums = new List<int>();

        for (int n = 0; n < target_amount_count; n++)
        {
            for (int i = 1; i <= target_amount_all; i++)
            {
                numbers.Add(i);
            }

            while (numbers.Count > 0)
            {

                int index = UnityEngine.Random.Range(0, numbers.Count);

                int ransu = numbers[index];

                if (ransu != 38)
                {
                    if (ransu != 63)
                    {
                        if (ransu != 88)
                        {
                            if (ransu != 113)
                            {
                                if (!(ransu == 13 && target_p_id == 3))
                                {
                                    tasknums.Add(ransu);
                                }
                            }
                        }
                    }
                }
                numbers.RemoveAt(index);
            }
        }
    }

    // アプリケーション終了時の処理を記述
    private void OnApplicationQuit()
    {
        // 視線データを保存したファイルを閉じる
        result_output_every("", streamWriter_gaze, true);
        Debug.Log("data_output_every!!");
    }
}