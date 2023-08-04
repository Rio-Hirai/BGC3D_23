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
    // 使用手法のリスト---------------------------------------------
    public enum test_pattern_list       // 新たな手法を追加したい場合はココに名前を追加する
    {
        Zero_Cursor,                    // カーソル無し（IDは0）
        Bubble_Gaze_Cursor1,            // BGC（IDは1）
        Bubble_Gaze_Cursor2,            // BGC with RayCast（IDは2）
        Bubble_Gaze_Cursor3,            // BGC_new（コレがBubble Gaze Cursor内で一番性能高い．IDは3）
        Gaze_Raycast,                   // 視線によるレイキャスト（IDは4）
        Controller_Raycast              // コントローラによるレイキャスト（IDは5）
    }
    public test_pattern_list test_pattern = test_pattern_list.Bubble_Gaze_Cursor1;  // 手法切り替え用のリスト構造
    //--------------------------------------------------------------


    // ターゲット配置条件のリスト-----------------------------------
    public enum target_pattern_list     // 新たなターゲット配置条件を追加したい場合はココに名前を追加する
    {
        Null,                           // ターゲット無し（IDは0）
        High_Density,                   // 高密度条件（IDは1）
        High_Occlusion,                 // 高オクルージョン条件（IDは2）
        Density_and_Occlusion,          // 密度＆オクルージョン条件（IDは3）
        Density_and_Occlusion2,          // 密度＆オクルージョン条件2（IDは4）
        Random                          // ランダム配置（IDは99）
    }
    public target_pattern_list target_pattern = target_pattern_list.High_Density;   // 条件切り替え用のリスト構造
    //--------------------------------------------------------------


    // 調整用パラメータ--------------------------------------------
    public int tester_id;               // 被験者のID
    public string tester_name;          // 被験者の名前
    public float set_dtime;             // 注視時間
    public float Depth;                 // ユーザとターゲット間の距離
    public float pointvalue;            // サッケード運動に対する閾値
    public float pointvalue2;           // 同上（ほぼ使っていない）
    [SerializeField, Range(-100, 100)] public int Brightness;   // 画面明度（使用していない）
    //--------------------------------------------------------------


    // 各種機能切り替え---------------------------------------------
    public bool total_DwellTime_mode;   // 累積注視時間モードのオン・オフ
    public bool gaze_data_switch;       // 視線情報出力機能のオン・オフ（実験以外ではオフにしておく）
    public bool eye_calibration;        // キャリブレーションを行うためのフラグ（立てた瞬間にキャリブレーションが行われる）
    public bool target_pos__calibration;// ターゲット群の位置調整を行うためのフラグ（立てた瞬間に位置調整が行われる）
    public bool cursor_switch;          // バブルカーソルの表示・非表示
    public bool bubblegaze_switch;      // ？？？（要リファクタリング）
    public bool gazeraycast_switch;     // ？？？（要リファクタリング）
    public bool controller_switch;      // コントローラの表示・非表示（まだコントローラを非表示にできない）
    public bool laserswitch;            // コントローラのレイの表示・非表示（まだレイを非表示にできない）
    public bool target_alpha_switch;    // ターゲットの透明化
    public bool MAverageFilter;         // 動的移動平均Fフィルタのオン・オフ
    public bool task_skip;              // 現在のタスクをスキップする
    public bool error_output_flag;      // 強制中断用
    public bool lens_switch;            // レンズの表示・非表示（使っていない）
    //--------------------------------------------------------------


    // 色設定-------------------------------------------------------
    public Color target_color;          // 選択確定時のターゲットの色
    public Color select_color;          // 注視状態のターゲットの色
    public Color cursor_color;          // バブルカーソルの色
    //--------------------------------------------------------------


    // 各種オブジェクト--------------------------------------------
    public GameObject head_obj;         // 頭部（カメラ）オブジェクト
    public GameObject bubblegaze;       // Bubble_Gaze_Cursor1・2のオブジェクト（表示・非表示用）
    public GameObject gazeraycast;      // Gaze_Raycasのオブジェクト（表示・非表示用）
    public GameObject gazeraycast2;     // Bubble_Gaze_Cursor3のオブジェクト（表示・非表示用）
    public GameObject controller_R;     // 右コントローラ（表示・非表示用）
    public GameObject controller_L;     // 左コントローラ（表示・非表示用）
    public GameObject controller_Raycast;// 左コントローラ（表示・非表示用）
    public GameObject Lens_Object;      // Bubble Gaze Lensのレンズオブジェクト（表示・非表示用，Bubble Gaze Lensが未実装なため使っていない）
    public GameObject[] target_set;     // 配置条件ごとのターゲット群を保存するための配列（表示・非表示用）
                                        //--------------------------------------------------------------


    // 効果音-------------------------------------------------------
    public AudioSource audioSource;     // 音響設定
    public AudioClip sound_OK;          // 指示通りのターゲットを選択できた時の音
    public AudioClip sound_NG;          // エラーした時の音
    public AudioClip sound_END;         // タスクが終了した時の音
    //--------------------------------------------------------------


    // 各種スクリプト-----------------------------------------------
    public gaze_data gaze_data;         // 各種自然情報を取得
    public LightSensor sensor;          // 画面の色彩情報
    //--------------------------------------------------------------


    // モニタ用変数-------------------------------------------------
    public int test_id;                 // 使用手法のID
    public int target_p_id;             // 配置条件のID
    public int target_amount_all;       // ターゲットの総数
    public int target_amount_select;    // 選択する数
    public int target_amount_count;     // 繰り返し回数
    private string input_start_time;    // タスク開始時間
    public int task_num = 0;            // タスクの番号
    private string filePath;            // 出力用ファイル名
    public float test_time = 0;         // 実験時間
    private float test_time_tmp;        // 前フレームまでの実験時間
    public List<int> tasknums;          // タスクの順番を格納するリスト
    private List<string> tasklogs;      // 実験結果を格納するリスト1（分析で殆ど使っていないので消してもいい）
    public List<string> tasklogs2;      // 実験結果を格納するリスト2
    private List<string> tasklogs3;     // 視線情報を格納するリスト
    private List<float> task_start_time;// タスクが開始した時の時間（タスク間に休憩時間があるため必要性が低い）
    private List<float> task_end_time;  // タスクが終了した時の時間（タスク間に休憩時間があるため必要性が低い）
    private int logoutput_count = 0;    // そのタスク中のエラー数
    private string now_test_pattern;    // 現在の使用手法のパターン
    private string now_target_pattern;  // 現在のターゲット配置のパターン
    public float lightValue;            // 画面全体の明度
    [System.NonSerialized] public Vector3 HMDRotation;  // HMDの角度
    //--------------------------------------------------------------


    // ターゲット選択関係-------------------------------------------
    public GameObject selecting_target; // 選択状態のターゲット（主にinspectorでのモニタ用で無くても問題なし）
    public GameObject DwellTarget;      // 注視状態のターゲット（主に注視状態のターゲットの色の変更に使用）
    public GameObject RayTarget;        // レイキャストによって注視されているターゲット（Bubble Gaze Cursor with Raycastで使用）
    public int select_target_id = 0;    // 選択されたターゲットのID
    //--------------------------------------------------------------


    // その他フラグ-------------------------------------------------
    public bool same_target;            // ？？？
    public bool session_flag;           // セッション中か否かを示す変数（trueだとセッション中）
    public bool taskflag;               // タスク中か否かを示す変数（trueだとタスク中）
    public bool next_step__flag;        // ？？？（おそらくtaskflagで代替可能，要リファクタリング）
    public bool output_flag;            // タスクが全て完了したか否かを示す変数（trueだと完了）
    public Boolean grapgrip;            // 結果の格納用Boolean型関数grapgrip
    public Boolean trackpad;            // ？？？
    private int switch_flag = 0;        // ？？？
    //--------------------------------------------------------------


    // ランダム配置関係---------------------------------------------
    public GameObject target_objects;   // クローンするターゲット
    public int target_id;               // クローンターゲットのID
    public float target_size;           // 注視状態のターゲットの大きさ
    public float target_distance;       // クローンターゲットとユーザ間の距離
    public int target_amount;           // クローンするターゲットの数
    //--------------------------------------------------------------


    // 瞬き関係-----------------------------------------------------
    public float LeftBlink;             // 左のまぶたの開き具合格納用関数
    public float RightBlink;            // 右のまぶたの開き具合格納用関数
    public int BlinkFlag;               // これがTrueになった瞬間にターゲット選択を確定させるように実装してあるので，瞬き関係はこれを弄るだけで十分．
    public int BlinkCount;              // 瞬きの回数
    public int BlinkSwitch;             // ？？？
    public float BlinkTime;             // 瞬きの時間
    public float LeftPupiltDiameter;    // ？？？
    public float RightPupiltDiameter;   // ？？？
    public int LeftPupiltDiameter_flag; // ？？？
    public int RightPupiltDiameter_flag;// ？？？
    //--------------------------------------------------------------


    // ターゲットのパラメータ---------------------------------------
    public float cursor_radious;        // ？？？
    public int select_flag_2;           // ？？？
    public int cursor_count;            // バブルカーソル内に存在するターゲットの数
    [System.NonSerialized] public Vector3 old_eye_position; // 以前の視線座標（瞬き選択用）
    [System.NonSerialized] public Vector3 new_eye_position; // 新しい視線座標（瞬き選択用）
    //--------------------------------------------------------------


    // Bubble Gaze Lens関係-----------------------------------------
    public bool lens_flag;              // ？？？
    public bool lens_flag2;             // ？？？
    //--------------------------------------------------------------

    private StreamWriter streamWriter_gaze; // ファイル出力用
    private SteamVR_Action_Boolean GrabG = SteamVR_Actions.default_GrabGrip;    // コントローラボタン

    void Start()
    {
        // 手法管理---------------------------------------------------
        switch (test_pattern.ToString()) // ココで手法毎にIDを割り振る
        {
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
            case "Bubble_Gaze_Cursor3":
                test_id = 5;
                break;
            default:
                test_id = 0;
                break;
        }

        bubblegaze.SetActive(false);
        gazeraycast.SetActive(false);
        controller_Raycast.SetActive(false);
        gazeraycast2.SetActive(false);

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
            controller_Raycast.SetActive(true);
            controller_R.GetComponent<SteamVR_LaserPointer>().active = true;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = true;
        }
        else if (test_id == 5)
        {
            gazeraycast2.SetActive(true);
            bubblegaze_switch = false;
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false;
        }
        //--------------------------------------------------------------


        // タスク条件管理-----------------------------------------------
        switch (target_pattern.ToString()) // ココで条件毎にIDを割り振りつつ，条件のパラメータを入力
        {
            case "High_Density":            // 高密度条件
                target_p_id = 1;            // 高密度条件のID
                target_amount_all = 25;     // ターゲットの総数
                target_amount_select = 25;  // 選択（タスク）回数
                target_amount_count = 1;    // 繰り返し回数
                Depth = 5.0f;               // 奥行き距離
                break;
            case "High_Occlusion":          // 高オクルージョン条件
                target_p_id = 2;            // 高オクルージョン条件のID
                target_amount_all = 4;      // ターゲットの総数
                target_amount_select = 24;  // 選択（タスク）回数
                target_amount_count = 6;    // 繰り返し回数（ターゲットの総数が選択回数より少ない場合に使用する）
                Depth = 5.0f;               // 奥行き距離
                break;
            case "Density_and_Occlusion":   // 密度＆オクルージョン条件
                target_p_id = 3;            // 密度＆オクルージョン条件のID
                target_amount_all = 124;    // ターゲットの総数
                target_amount_select = 25;  // 選択（タスク）回数
                target_amount_count = 1;    // 繰り返し回数
                Depth = 3.5f;               // 奥行き距離
                break;
            case "Density_and_Occlusion2":  // 密度＆オクルージョン条件2
                target_p_id = 4;            // 密度＆オクルージョン条件2のID
                target_amount_all = 48;     // ターゲットの総数
                target_amount_select = 25;  // 選択（タスク）回数
                target_amount_count = 1;    // 繰り返し回数
                Depth = 3.5f;               // 奥行き距離
                break;
            case "Random":                  // ランダム配置条件
                target_p_id = 99;           // ランダム配置条件のID（配置条件を追加できるように99にしている）
                break;
            default:
                target_p_id = 0;
                target_amount_all = 0;
                target_amount_select = 0;
                target_amount_count = 0;
                break;
        }
        //--------------------------------------------------------------


        // 表示されているターゲット群を全て非表示-----------------------
        for (int i = 0; i < target_set.Length; i++) {
            target_set[i].SetActive(false);
        }
        //--------------------------------------------------------------


        // ランダム配置条件の場合の処理---------------------------------
        if (target_p_id == 99)
        {
            random_target_set(); // ランダムにターゲットを配置
        }
        else
        {
            set_testpattern(); // ターゲットの初期化
        }
        //--------------------------------------------------------------


        //ファイル名作成------------------------------------------------
        DateTime dt = DateTime.Now; // 時間を保存

        input_start_time = dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
        filePath = Application.dataPath + "/Gaze_Team/BGC3D/Scripts/test_results/" + "test_id = " + test_id + "___" + "target_p_id = " + target_p_id + "___" + "tester_id  = " + tester_id + "___" + tester_name + "___" + input_start_time;
        streamWriter_gaze = File.AppendText(filePath + "_gaze_data.csv");

        if (gaze_data_switch)result_output_every ("timestamp,taskNo,gaze_x,gaze_y,pupil_r,pupil_l,blink_r,blink_l,hmd_x,hmd_y,hmd_z,LightValue", streamWriter_gaze, false); // gaze_data_switchがtrue＝視線情報保存状態の場合はファイルを生成して書き込む
        //--------------------------------------------------------------


        // ログ作成（データ分析をする分には消してもいい）---------------
        tasklogs = new List<string>();
        task_start_time = new List<float>();
        task_end_time = new List<float>();
        //--------------------------------------------------------------
    }


    void Update()
    {
        method_change(); // 使用している手法を変更


        // ターゲット位置の調整---------------------------------------
        grapgrip = SteamVR_Actions.default_GrabGrip.GetState(SteamVR_Input_Sources.Any);
        if (grapgrip || target_pos__calibration)
        {
            if (target_p_id != 99) target_set[target_p_id - 1].SetActive(true); // 指定した配置条件のターゲット群を表示する

            Camera mainCamera = Camera.main;
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 cameraForward = mainCamera.transform.forward;
            target_set[target_p_id - 1].transform.position = cameraPosition + cameraForward * Depth; // ユーザの正面にターゲット群を配置
            target_set[target_p_id - 1].transform.LookAt(2 * target_set[target_p_id - 1].transform.position - mainCamera.transform.position); // ターゲット群をユーザに向ける
            target_set[target_p_id - 1].transform.rotation = Quaternion.Euler(0, target_set[target_p_id - 1].transform.rotation.eulerAngles.y, target_set[target_p_id - 1].transform.rotation.eulerAngles.z); // ターゲット群の角度を微調整
            //target_set[target_p_id - 1].transform.position = new Vector3(target_set[target_p_id - 1].transform.position.x, cameraPosition.y, target_set[target_p_id - 1].transform.position.y); // ターゲット群の位置を微調整

            target_pos__calibration = false; // 機能フラグをリセット
        }
        //--------------------------------------------------------------


        test_time += Time.deltaTime; // タスク時間を更新


        // タスクの推移管理---------------------------------------------
        if (select_target_id == 999 && taskflag == false)
        {
            taskflag = true;
            tasklogs.Add("");
            task_start_time.Add(test_time);
            test_time_tmp = test_time;
            logoutput_count = 0;
            session_flag = true;
        }
        //--------------------------------------------------------------


        // タスクの状態チェック-----------------------------------------
        if (taskflag)
        {
            // ターゲットの選択が行われた時の処理---------------------
            if ((select_target_id != -1 && select_target_id != 999 && same_target == false) || task_skip)
            {
                tasklogs2.Add((task_num + 1) + "," + tasknums[task_num] + "," + select_target_id + "," + (test_time - test_time_tmp));
                test_time_tmp = test_time;

                if ((select_target_id == tasknums[task_num]) || task_skip)　// 正しいターゲットを選択した時の処理
                {
                    same_target = true;
                    task_skip = false;

                    tasklogs[task_num] += ("select_target = " + select_target_id + ": " + test_time + "\n");
                    tasklogs2[tasklogs2.Count - 1] += ("," + (test_time - task_start_time[task_num]) + "," + logoutput_count); // そのタスクの総時間とエラー数を追記

                    if (task_num < target_amount_select)
                    {
                        task_end_time.Add(test_time); // タスクが終了した時の時間を保存
                        task_num++; // タスクを次に進める
                        test_time_tmp = 0; // タスク時間を初期化
                        audioSource.PlayOneShot(sound_OK); // 正解した時の効果音を鳴らす
                        taskflag = false; // 非タスク中にする
                    }
                }
                else　// 間違えたターゲットを選択した時の処理
                {
                    logoutput_count++; // 間違えた数をカウント

                    same_target = true;
                    tasklogs[task_num] += ("select_target = " + select_target_id + ": " + test_time + "\n");
                    audioSource.PlayOneShot(sound_NG); // 間違えた時の効果音を鳴らす
                }
            }
            //--------------------------------------------------------------


            // セッションが終了するか，強制中断を行った時の処理-------------
            if (task_num == target_amount_select && output_flag == false)
            {
                output_flag = true;
                audioSource.PlayOneShot(sound_END);
                result_output();
                result_output_csv();
                //result_output_csv2();
                result_output_every("", streamWriter_gaze, true);
            }
            //--------------------------------------------------------------
        }
        //--------------------------------------------------------------


        // タスクを中断する際の処理-------------------------------------
        if (error_output_flag)
        {
            error_output_flag = false;
            audioSource.PlayOneShot(sound_END);
            error_output();
            result_output_csv();
            //result_output_csv2();
            result_output_every("", streamWriter_gaze, true);
        }
        //--------------------------------------------------------------


        // Head（ヘッドマウンドディスプレイ）の情報を一時保管-----------
        Quaternion HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head); //回転座標をクォータニオンで値を受け取る，旧式らしいので要修正
        HMDRotation = HMDRotationQ.eulerAngles; // 取得した値をクォータニオン → オイラー角に変換
        //--------------------------------------------------------------


        lightValue = sensor.lightValue; // 画面全体の明度情報を更新

        if (gaze_data_switch) if (output_flag == false && taskflag == true) result_output_every(gaze_data.get_gaze_data2(), streamWriter_gaze, false); // 視線関係のデータを取得
    }
    //--------------------------------------------------------------


    // 手法を変更する関数
    private void method_change()
    {
        // ？？？-------------------------------------------------------
        if (SteamVR_Actions.default_SnapTurnLeft.GetStateDown(SteamVR_Input_Sources.Any) && switch_flag == 0)
        {
            Debug.Log("LEFT");
            if (test_id > 1)
            {
                test_id -= 1;
            }
            else
            {
                test_id = 5;
            }
            switch_flag = 1;
        }
        else if (SteamVR_Actions.default_SnapTurnRight.GetStateDown(SteamVR_Input_Sources.Any) && switch_flag == 0)
        {
            Debug.Log("RIGHT");
            if (test_id < 5)
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
        //--------------------------------------------------------------


        //trackpad = SteamVR_Actions.default_Teleport.GetStateDown(SteamVR_Input_Sources.Any);


        // ？？？-------------------------------------------------------
        if (now_test_pattern != test_pattern.ToString() || switch_flag == 1)
        {
            now_test_pattern = test_pattern.ToString();


            // ？？？-------------------------------------------------------
            if (DwellTarget != null)
            {
                DwellTarget.GetComponent<Renderer>().material.color = Color.white;
            }
            //--------------------------------------------------------------


            // パラメータ初期化---------------------------------------------
            bubblegaze_switch = false;
            gazeraycast_switch = false;
            controller_switch = false;
            bubblegaze.SetActive(false);
            gazeraycast.SetActive(false);
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false;
            //--------------------------------------------------------------


            // パラメータ更新-----------------------------------------------
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
            else if (test_id == 5)
            {
                gazeraycast2.SetActive(true);
                bubblegaze_switch = false;
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false;
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false;
            }

            if (test_id == 2)
            {
                bubblegaze.GetComponent<Collider>().enabled = false;
            }
            else
            {
                bubblegaze.GetComponent<Collider>().enabled = true;
            }
            //--------------------------------------------------------------
        }
        //--------------------------------------------------------------


        // ？？？-------------------------------------------------------
        if (now_target_pattern != target_pattern.ToString())
        {
            now_target_pattern = target_pattern.ToString();
        }
        //--------------------------------------------------------------


        // ？？？-------------------------------------------------------
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
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // 結果を出力する関数-------------------------------------------
    public void result_output()
    {
        Debug.Log("data_input_start!!"); // 確認メッセージを出力


        //ファイル生成-------------------------------------------------
        StreamWriter streamWriter = File.AppendText(filePath + ".txt");
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("target_pattorn:");
        //--------------------------------------------------------------


        // ？？？-------------------------------------------------------
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
        //--------------------------------------------------------------


        // 開始時間-----------------------------------------------------
        streamWriter.WriteLine("test_start_time: " + task_start_time[0]);
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        //--------------------------------------------------------------


        // 各タスクの計測を追記-----------------------------------------
        for (int i = 0; i < target_amount_select; i++)
        {
            streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_start: " + task_start_time[i]);
            streamWriter.WriteLine(tasklogs[i]);
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_end: " + task_end_time[i]);
            streamWriter.WriteLine("task_time: " + ( task_end_time[i] - task_start_time[i]));
        }
        //--------------------------------------------------------------


        //// 各タスクの計測を追記3
        //streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
        //for (int i = 0; i < tasklogs2.Count; i++)
        //{
        //    streamWriter.WriteLine(tasklogs2[i]);
        //}
        //streamWriter.WriteLine("\n-----------------------------------------------------------------------------------------\n");


        // 終了時間-----------------------------------------------------
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("test_end_time: " + task_end_time[target_amount_select-1]);
        //--------------------------------------------------------------


        // 後処理-------------------------------------------------------
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_input_end!!"); // 確認メッセージを出力
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // 実験結果をcsv形式で出力する関数------------------------------
    public void result_output_csv()
    {
        Debug.Log("data_input_csv_start!!"); // 確認メッセージを出力
        StreamWriter streamWriter = File.AppendText(filePath + ".csv"); //ファイル生成


        // 各タスクの計測を追記-----------------------------------------
        streamWriter.WriteLine("task,target,select,time,totaltime,totalerror");
        for (int i = 0; i < tasklogs2.Count; i++)
        {
            streamWriter.WriteLine(tasklogs2[i]);
        }
        //--------------------------------------------------------------


        // 後処理-------------------------------------------------------
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_input_end!!"); // 確認メッセージを出力
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // 視線情報をcsv形式で出力する関数（処理が重いので使っていない）-
    public void result_output_csv2()
    {
        Debug.Log("data_input_csv_start2!!"); // 確認メッセージを出力
        StreamWriter streamWriter = File.AppendText(filePath + "_gaze_data.csv"); //ファイル生成


        // 各タスクの計測を追記----------------------------------------
        streamWriter.WriteLine("timestamp,taskNo,gaze_x,gaze_y,pupil_r,pupil_l,blink_r,blink_l,hmd_x,hmd_y,hmd_z,LightValue");
        for (int i = 0; i < tasklogs3.Count; i++)
        {
            streamWriter.WriteLine(tasklogs3[i]);
        }
        //--------------------------------------------------------------


        // 後処理-------------------------------------------------------
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_output_end2!!"); // 確認メッセージを出力
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // 引数をファイルに出力する関数---------------------------------
    public void result_output_every(string data, StreamWriter streamWriter, bool endtask) // これが呼び出されるたびに変数（第一引数の文字列）をcsvファイルに出力
    {
        if (endtask == false) streamWriter.WriteLine(data); // falseの場合は書き込み処理，trueの場合は閉じる処理


        // 閉じる処理---------------------------------------------------
        if (endtask)
        {
            streamWriter.Close();
            Debug.Log("data_output_every!!"); // 確認メッセージを出力
        }
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // 実験を途中で中断した場合にログを出力するための関数-----------
    public void error_output()
    {
        Debug.Log("data_input_start!!"); // 確認メッセージを出力

        //ファイル生成--------------------------------------------------
        StreamWriter streamWriter = File.AppendText(filePath + ".txt");
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("target_pattorn:");
        //--------------------------------------------------------------


        // ？？？-------------------------------------------------------
        for (int i = 0; i < target_amount_all - 5; i++)
        {
            streamWriter.WriteLine(tasknums[i]);
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("target_pattorn:");
        for (int i = 0; i < target_amount_select; i++)
        {
            streamWriter.WriteLine(tasknums[i]);
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        //--------------------------------------------------------------


        // 開始時間-----------------------------------------------------
        streamWriter.WriteLine("test_start_time: " + task_start_time[0]);
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        //--------------------------------------------------------------


        // 各タスクの計測を追記-----------------------------------------
        for (int i = 0; i < task_num; i++)
        {
            streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_start: " + task_start_time[i]);
            streamWriter.WriteLine(tasklogs[i]);
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_end: " + task_end_time[i]);
            streamWriter.WriteLine("task_time: " + (task_end_time[i] - task_start_time[i]));
        }
        //--------------------------------------------------------------


        // 各タスクの計測を追記2----------------------------------------
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n");
        streamWriter.WriteLine("task,target,select,time,totaltime,totalerror");
        for (int i = 0; i < tasklogs2.Count; i++)
        {
            streamWriter.WriteLine(tasklogs2[i]);
        }
        streamWriter.WriteLine("\n-----------------------------------------------------------------------------------------\n");
        //--------------------------------------------------------------


        // 終了時間-----------------------------------------------------
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
        streamWriter.WriteLine("test_end_time: " + task_end_time[task_num - 1]);
        //--------------------------------------------------------------

        // 後処理-------------------------------------------------------
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_input_end!!"); // 確認メッセージを出力
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // タスク（選択するターゲット）を生成する関数-------------------
    void set_testpattern()
    {
        List<int> numbers = new List<int>();
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
    //--------------------------------------------------------------


    // ランダム配置条件のためのターゲット生成と配置を行う関数-------
    private void random_target_set()
    {
        target_id = 0; // ターゲットに割り振るIDを初期化
        target_objects.SetActive(true); // クローンするターゲットを表示


        // ？？？-------------------------------------------------------
        for (int i = 0; i < target_amount; i++)
        {
            Vector3 randomDirection = UnityEngine.Random.onUnitSphere; // カメラからランダムな方向を取得
            float randomDistance = UnityEngine.Random.Range(Depth, target_distance + Depth); // ランダムな距離を計算
            Vector3 objectPosition = Camera.main.transform.position + randomDirection * randomDistance; // オブジェクトの位置を決定
            Instantiate(target_objects, objectPosition, Quaternion.identity); // オブジェクトを複製して配置
        }
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // アプリケーション終了時の処理を行う関数-----------------------
    private void OnApplicationQuit()
    {
        result_output_every("", streamWriter_gaze, true);   // 視線データを保存したファイルを閉じる
        Debug.Log("data_output_every!!");                   // 確認メッセージを出力
    }
    //--------------------------------------------------------------


    // 瞬き3回で選択行為を行う関数----------------------------------
    public void Blink3()
    {
        BlinkTime += Time.deltaTime;


        // 1回目--------------------------------------------------------
        if (RightBlink == 0 && BlinkSwitch == 0 && BlinkCount == 0)
        {
            BlinkSwitch = 1;
            BlinkCount = 1;
            BlinkTime = 0;
            Debug.Log("OK1");
        }
        //--------------------------------------------------------------


        // 2回目以降----------------------------------------------------
        if (RightBlink == 0 && BlinkSwitch == 0 && BlinkCount > 0 && BlinkTime < 1.0f)
        {
            BlinkCount += 1;
            Debug.Log("OK=count" + BlinkCount);
        }
        else if (BlinkCount < 3 && BlinkTime > 1.0f)
        {
            BlinkCount = 0;
            Debug.Log("OK=count" + 0);
        }
        //--------------------------------------------------------------


        // ？？？-------------------------------------------------------
        if (RightBlink > 0.3)
        {
            BlinkSwitch = 0;
            Debug.Log("OK=Switch" + 0);
        }
        //--------------------------------------------------------------


        // 3回以上瞬きをした場合----------------------------------------
        if (BlinkCount > 2)
        {
            BlinkFlag = 1;
            Debug.Log("OKFlag");
        }
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------


    // 瞬きを3回行うと選択になる関数--------------------------------
    public void Blink()
    {
        BlinkTime += Time.deltaTime;


        // ？？？-------------------------------------------------------
        if (RightBlink == 0 && BlinkSwitch == 0 && BlinkTime < 0.5)
        {
            BlinkCount += 1;
            BlinkTime = 0;
            BlinkSwitch = 1;
            Debug.Log("OK=count" + BlinkCount);
        }
        else if (BlinkTime > 1.0f)
        {
            BlinkCount = 0;
            BlinkTime = 0;
            BlinkFlag = 0;
        }
        else if (BlinkTime > 0.3f)
        {
            BlinkFlag = 0;
        }
        //--------------------------------------------------------------


        // ？？？-------------------------------------------------------
        if (RightBlink > 0.3 && RightBlink != 1)
        {
            BlinkSwitch = 0;
        }
        //--------------------------------------------------------------


        // 3回以上瞬きをした場合----------------------------------------
        if (BlinkCount > 1)
        {
            BlinkFlag = 1;
            BlinkCount = 0;
            BlinkTime = 0;
        }
        //--------------------------------------------------------------
    }
    //--------------------------------------------------------------
}
