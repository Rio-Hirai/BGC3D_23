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
    public enum test_pattern_list           // 新たな手法を追加したい場合はココに名前を追加する
    {
        Zero_Cursor,                        // カーソル無し（IDは0）
        Bubble_Gaze_Cursor_old,             // 旧BGC（IDは1）
        Bubble_Gaze_Cursor2_old,            // 旧BGC with RayCast（IDは2）
        Bubble_Gaze_Cursor_new,             // 新BGC（IDは5）
        Bubble_Gaze_Cursor_with_Gaze_Ray,   // 新BGC with 視線によるレイキャスト（IDは6）
        Bubble_Gaze_Cursor_with_nod,        // 新BGC with nod（IDは7）
        Gaze_Raycast,                       // 視線によるレイキャスト（IDは3）
        Gaze_Raycast_with_nod,              // 視線によるレイキャスト with nod（IDは8）
        Controller_Raycast                  // コントローラによるレイキャスト（IDは4）
    }
    [Header("実験条件_入力部分")]
    [Tooltip("使用手法")]
    public test_pattern_list test_pattern = test_pattern_list.Bubble_Gaze_Cursor_old;  // 手法切り替え用のリスト構造
    //--------------------------------------------------------------


    // ターゲット配置条件のリスト-----------------------------------
    public enum target_pattern_list         // 新たなターゲット配置条件を追加したい場合はココに名前を追加する
    {
        Null,                               // ターゲット無し（IDは0）
        High_Density,                       // 高密度条件（IDは1）
        High_Occlusion,                     // 高オクルージョン条件（IDは2）
        Density_and_Occlusion_5x5x5,        // 密度＆オクルージョン条件（IDは3）
        Density_and_Occlusion_5x5x3,        // 密度＆オクルージョン条件（IDは4）
        Small_5x5x5,                        // 密度＆オクルージョン条件（IDは5）
        Small_5x5x3,                        // 密度＆オクルージョン条件（IDは6）
        Study_1,                            //
        Flat_1,                             //
        TEST_16x3,                          // テスト用（IDは97）
        TEST_small_16x3,                    // テスト用（IDは98）
        Random,                             // ランダム配置（IDは99）
        Random_small                        // ランダム配置（IDは99）
    }
    [Tooltip("ターゲット配置条件")]
    public target_pattern_list target_pattern = target_pattern_list.High_Density;   // 条件切り替え用のリスト構造
    //--------------------------------------------------------------


    // パラメータテンプレートのリスト-----------------------------------
    public enum parameter_setting_templates // 新たなパラメータテンプレートを追加したい場合はココに名前を追加する
    {
        Null,                               // テンプレート未使用
        Study,                              // 実験時のパラメータ
        All_UI_Awake,                       // UIを表示したい時のパラメータ
        All_UI_Asleep,                      // UIを非表示したい時のパラメータ
        Debug                               // デバッグ時のパラメータ
    }
    [Tooltip("パラメータテンプレート")]
    public parameter_setting_templates parameter_setting = parameter_setting_templates.Null;   // 条件切り替え用のリスト構造
    //--------------------------------------------------------------


    // 調整用パラメータ--------------------------------------------
    [Tooltip("被験者のID")]
    public int tester_id;                   // 被験者のID
    [Tooltip("被験者の名前")]
    public string tester_name;              // 被験者の名前
    [Tooltip("注視時間")]
    public float set_dtime;                 // 注視時間
    [Tooltip("最大タスク時間")]
    public float TaskTime;                  // 最大タスク時間
    [Tooltip("ユーザとターゲット間の距離")]
    public float Depth;                     // ユーザとターゲット間の距離
    [Tooltip("画面明度")]
    [SerializeField, Range(-100.0f, 150.0f)]
    public int Brightness;                  // 画面明度（使用していない）
    //--------------------------------------------------------------


    // 各種機能切り替え---------------------------------------------
    [Tooltip("累積注視時間モードのオン・オフ")]
    public bool total_DwellTime_mode;       // 累積注視時間モードのオン・オフ
    [Tooltip("注視時間補正モードのオン・オフ")]
    public bool dtime_correction_mode;      // 注視時間補正モードのオン・オフ
    [Tooltip("視線情報出力機能のオン・オフ")]
    public bool gaze_data_switch;           // 視線情報出力機能のオン・オフ（実験以外ではオフにしておく）
    [Tooltip("タスクのタイムアウトのオン・オフ")]
    public bool TimeOut_switch;             // タスクでタイムアウトを行うか否かのフラグ
    [Tooltip("視線キャリブレーション")]
    public bool eye_calibration;            // キャリブレーションを行うためのフラグ（立てた瞬間にキャリブレーションが行われる）
    [Tooltip("ターゲット群の位置調整")]
    public bool target_pos_calibration;     // ターゲット群の位置調整を行うためのフラグ（立てた瞬間に位置調整が行われる）
    [Tooltip("バブルカーソルの表示・非表示")]
    public bool cursor_switch;              // バブルカーソルの表示・非表示
    [Tooltip("レイキャストの表示・非表示")]
    public bool raycast_switch;             // レイキャストの表示・非表示
    [Tooltip("ポインターの表示・非表示")]
    public bool pointer_switch;             // ポインターの表示・非表示
    [Tooltip("頷き選択機能のオン・オフ")]
    public bool approve_switch;             // 頷き選択機能のオン・オフ
    [Tooltip("コントローラの表示・非表示")]
    public bool controller_switch;          // コントローラの表示・非表示
    [Tooltip("コントローラのレイの表示・非表示")]
    public bool laserswitch;                // コントローラのレイの表示・非表示（まだレイを非表示にできない）
    [Tooltip("ターゲットの透明化")]
    public bool target_alpha_switch;        // ターゲットの透明化
    [Tooltip("ターゲットの縮小化")]
    public bool target_size_mini_switch;    // ターゲットの縮小化
    [Tooltip("明度計算機能のオン・オフ")]
    public bool LightSensor_switch;         // 明度計算機能のオン・オフ
    [Tooltip("明度補正のオン・オフ")]
    public bool bright_correction_mode;     // 明度補正のオン・オフ
    [Tooltip("動的移動平均フィルタのオン・オフ")]
    public bool MAverageFilter;             // 動的移動平均フィルタのオン・オフ
    [Tooltip("現在のタスクをスキップ")]
    public bool task_skip;                  // 現在のタスクをスキップ（フラグを立てた瞬間に１度だけ実行されfalseに戻る）
    [Tooltip("強制中断")]
    public bool error_output_flag;          // 強制中断（フラグを立てた瞬間に現時点での実験結果が出力される）
    [Tooltip("レンズの表示・非表示")]
    public bool lens_switch;                // レンズの表示・非表示（使っていない）
    [Tooltip("フリーモードのオン・オフ")]
    public bool free_mode;                  // フリーモードのオン・オフ
    [Tooltip("注視時間表示のオン・オフ")]
    public bool dtime_monitor_switch;       // 注視時間表示のオン・オフ
    //--------------------------------------------------------------


    // 色設定-------------------------------------------------------
    [Tooltip("選択確定時のターゲットの色")]
    public Color target_color;              // 選択確定時のターゲットの色
    [Tooltip("注視状態のターゲットの色")]
    public Color select_color;              // 注視状態のターゲットの色
    [Tooltip("バブルカーソルの色")]
    public Color cursor_color;              // バブルカーソルの色
    //--------------------------------------------------------------


    // 各種オブジェクト--------------------------------------------------
    [Header("各種オブジェクト・スクリプト")]
    public GameObject head_obj;             // 頭部（カメラ）オブジェクト
    public GameObject bubblegaze;           // Bubble_Gaze_Cursor1・2のオブジェクト（表示・非表示用）
    public GameObject gazeraycast;          // Gaze_Raycasのオブジェクト（表示・非表示用）
    public GameObject gazeraycast2;         // Bubble_Gaze_Cursor3のオブジェクト（表示・非表示用）
    public GameObject controller_R;         // 右コントローラ（表示・非表示用）
    public GameObject controller_L;         // 左コントローラ（表示・非表示用）
    public GameObject controller_Raycast;   // コントローラのレイキャスト機能（機能の無効化用）
    public GameObject dtime_monitor;        // コントローラのレイキャスト機能（機能の無効化用）
    public GameObject[] target_set;         // 配置条件ごとのターゲット群を保存するための配列（表示・非表示用）
    //--------------------------------------------------------------


    // 効果音-------------------------------------------------------
    public AudioSource audioSource;         // 音響設定
    public AudioClip sound_OK;              // 指示通りのターゲットを選択できた時の音
    public AudioClip sound_NG;              // エラーした時の音
    public AudioClip sound_END;             // タスクが終了した時の音
    //--------------------------------------------------------------


    // 各種スクリプト-----------------------------------------------
    public gaze_data_callback_v2 gaze_data; // 視線情報
    public LightSensor sensor;              // 画面の色彩情報
    private PostProcessVolume _postProcess; // ？？？
    private ColorGrading _colorGrading;     // ？？？
    //--------------------------------------------------------------


    // コントローラ関係----------------------------------------------
    public Valve.VR.SteamVR_Action_Boolean GrabG = SteamVR_Actions.default_GrabGrip;    // コントローラのトリガーボタン
    //--------------------------------------------------------------


    // データ出力関係------------------------------------------------
    [System.NonSerialized]
    public StreamWriter streamWriter_gaze;  // ファイル出力用
    [System.NonSerialized]
    public GameObject taskObject;           // 現在提示されているターゲット
    [System.NonSerialized]
    public string center_flag = "true";     // 黒いターゲットを注視している時の状態
    [System.NonSerialized]
    public string interval_flag = "true";   // 黒いターゲットを注視している時の状態
    //--------------------------------------------------------------


    // モニタ用変数-------------------------------------------------
    [System.NonSerialized]
    public int test_id;                     // 使用手法のID
    public int target_a_id;                 // 配置条件のID
    public int target_p_id;                 // 配置条件のID
    [Header("状況モニタ")]
    public int target_amount_all;           // ターゲットの総数
    public int target_amount_select;        // 選択する数
    public int target_amount_count;         // 繰り返し回数
    private string input_start_time;        // アプリケーションを実行した時の時間（ほぼ確実に一意に定まるのでファイル名に使用）
    public int task_num = 0;                // タスクの番号
    [System.NonSerialized]
    public string filePath;                 // 出力用ファイル名
    public float test_time = 0;             // 実験時間
    private float test_time_tmp;            // 前フレームまでの実験時間
    public List<int> tasknums;              // タスクの順番を格納するリスト
    private List<string> tasklogs;          // 実験結果を格納するリスト1（人間が見るため用で分析には殆ど使っていないので消してもいい）
    public List<string> tasklogs2;          // 実験結果を格納するリスト2
    private List<string> tasklogs3;         // 視線情報を格納するリスト
    private List<float> task_start_time;    // タスクが開始した時の時間（タスク間に休憩時間があるため必要性が低い）
    private List<float> task_end_time;      // タスクが終了した時の時間（タスク間に休憩時間があるため必要性が低い）
    private int logoutput_count = 0;        // そのタスク中のエラー数
    private string now_test_pattern;        // 現在の使用手法のパターン
    private string now_target_pattern;      // 現在のターゲット配置のパターン
    [System.NonSerialized]
    public float lightValue;                // 画面全体の明度
    [System.NonSerialized]
    public Vector3 HMDRotation;             // HMDの角度
    [System.NonSerialized]
    public float ab_dtime;                  // 絶対的な注視時間
    //--------------------------------------------------------------


    // ターゲット選択関係-----------------------------------------------
    public GameObject selecting_target;     // 選択状態のターゲット（主にinspectorでのモニタ用で無くても問題なし）
    public GameObject DwellTarget;          // 注視状態のターゲット（主に注視状態のターゲットの色の変更に使用）
    public GameObject RayTarget;            // レイキャストによって注視されているターゲット（Bubble Gaze Cursor with Raycastで使用）
    public int select_target_id = 0;        // 選択されたターゲットのID
    //--------------------------------------------------------------


    // ターゲットのパラメータ---------------------------------------------
    [Tooltip("バブルカーソルの大きさ")]
    [System.NonSerialized]
    public float cursor_radious;            // バブルカーソルの大きさ
    [System.NonSerialized]
    public int select_flag_2;               // ？？？
    [Tooltip("バブルカーソルの最大半径内に存在するターゲットの数")]
    public int cursor_count;                // バブルカーソル内に存在するターゲットの数
    [System.NonSerialized]
    public Vector3 old_eye_position;        // 以前の視線座標（瞬き選択用）
    [System.NonSerialized]
    public Vector3 new_eye_position;        // 新しい視線座標（瞬き選択用）
    //--------------------------------------------------------------


    // その他フラグ----------------------------------------------------
    public bool same_target;                // ？？？
    public bool session_flag;               // セッション中か否かを示す変数（trueだとセッション中）
    public bool taskflag;                   // タスク中か否かを示す変数（trueだとタスク中）
    public bool next_step__flag;            // ？？？（おそらくtaskflagで代替可能，要リファクタリング）
    public bool head_rot_switch;            // ？？？
    public bool select_flag;                // ？？？
    private int switch_flag = 0;            // ？？？
    public bool output_flag;                // タスクが全て完了したか否かを示す変数（trueだと完了）
    public Boolean grapgrip;                // 結果の格納用Boolean型関数grapgrip
    public Boolean trackpad;                // ？？？
    //--------------------------------------------------------------


    // ランダム配置関係------------------------------------------------
    [Header("ランダム配置条件のパラメータ")]
    [Tooltip("クローンするターゲット")]
    public GameObject target_objects;       // クローンするターゲット
    [System.NonSerialized]
    public int target_id;                   // クローンターゲットのID
    [Tooltip("注視状態のターゲットの大きさ")]
    public float target_size;               // 注視状態のターゲットの大きさ
    [Tooltip("クローンターゲットとユーザ間の距離")]
    public float target_distance;           // クローンターゲットとユーザ間の距離
    [Tooltip("クローンするターゲットの数")]
    public int target_amount;               // クローンするターゲットの数
    //--------------------------------------------------------------


    // 調整用パラメータ2--------------------------------------------
    [Header("調整用パラメータ2")]
    [Tooltip("サッケード運動に対する閾値1")]
    public float pointvalue;                // サッケード運動に対する閾値
    [Tooltip("サッケード運動に対する閾値2")]
    public float pointvalue2;               // 同上（ほぼ使っていない）
    //--------------------------------------------------------------


    // 瞬き関係-------------------------------------------------------
    [Header("瞬き関係")]
    public float LeftBlink;                 // 左のまぶたの開き具合格納用関数
    public float RightBlink;                // 右のまぶたの開き具合格納用関数
    public int BlinkFlag;                   // これがTrueになった瞬間にターゲット選択を確定させるように実装してあるので，瞬き関係はこれを弄るだけで十分．
    public int BlinkCount;                  // 瞬きの回数
    public int BlinkSwitch;                 // ？？？
    public float BlinkTime;                 // 瞬きの時間
    public float LeftPupiltDiameter;        // ？？？
    public float RightPupiltDiameter;       // ？？？
    public int LeftPupiltDiameter_flag;     // ？？？
    public int RightPupiltDiameter_flag;    // ？？？
    //--------------------------------------------------------------


    // Bubble Gaze Lens関係------------------------------------------
    [Header("Bubble Gaze Lens関係")]
    public GameObject Lens_Object;          // Bubble Gaze Lensのレンズオブジェクト（表示・非表示用，Bubble Gaze Lensが未実装なため使っていない）
    public bool lens_flag;                  // ？？？
    public bool lens_flag2;                 // ？？？
    //--------------------------------------------------------------


    void Start()
    {
        // 手法管理---------------------------------------------------
        switch (test_pattern.ToString()) // ココで手法毎にIDを割り振る（IDの順番とリスト構造の順番には関係がないため気にせず順番にIDを振ればいい）
        {
            case "Bubble_Gaze_Cursor_old":
                test_id = 1;
                break;
            case "Bubble_Gaze_Cursor2_old":
                test_id = 2;
                break;
            case "Gaze_Raycast":
                test_id = 3;
                break;
            case "Controller_Raycast":
                test_id = 4;
                break;
            case "Bubble_Gaze_Cursor_new":
                test_id = 5;
                break;
            case "Bubble_Gaze_Cursor_with_Gaze_Ray":
                test_id = 6;
                break;
            case "Bubble_Gaze_Cursor_with_nod":
                test_id = 7;
                break;
            case "Gaze_Raycast_with_nod":
                test_id = 8;
                break;
            default:
                test_id = 0;
                break;
        }
        //--------------------------------------------------------------


        // 各種手法のオブジェクトを非表示（以降の条件分岐で該当する手法のみ表示するため）
        bubblegaze.SetActive(false);
        gazeraycast.SetActive(false);
        controller_Raycast.SetActive(false);
        gazeraycast2.SetActive(false);
        //--------------------------------------------------------------
        //--------------------------------------------------------------


        // 該当する手法のオブジェクトを表示-----------------------------
        if (test_id == 0) // ？？？
        {
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
        }
        else if (test_id < 3) // ？？？
        {
            bubblegaze.SetActive(true); // Bubble Gaze Cursorを表示
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？

            if (test_id == 2) gazeraycast.SetActive(true); // 視線レイキャストを表示
        }
        else if (test_id == 3) // ？？？
        {
            raycast_switch = true; // ？？？
            gazeraycast.SetActive(true); // 視線レイキャストを表示
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
        }
        else if (test_id == 4) // ？？？
        {
            controller_switch = true; // コントローラの機能をオン
            controller_Raycast.SetActive(true); // ？？？
            controller_R.GetComponent<SteamVR_LaserPointer>().active = true; // ？？？
            controller_L.GetComponent<SteamVR_LaserPointer>().active = true; // ？？？
        }
        else if (test_id == 5 || test_id == 6 || test_id == 7) // ？？？
        {
            pointer_switch = true; // ？？？
            gazeraycast2.SetActive(true); // ？？？
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？

        }

        if (test_id == 7 || test_id == 8) // ？？？
        {
            set_dtime = 999; // ？？？
            approve_switch = true; // ？？？
            total_DwellTime_mode = false; // ？？？
        }
        //--------------------------------------------------------------
        //--------------------------------------------------------------


        // タスク条件管理-------------------------------------------------
        switch (target_pattern.ToString())      // ココで条件毎にIDを割り振りつつ，条件のパラメータを入力
        {
            case "High_Density":                // 高密度条件
                target_a_id = 1;                // 高密度条件のID
                target_p_id = 1;                // 高密度条件のID
                target_amount_all = 25;         // ターゲットの総数
                target_amount_select = 25;      // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 5.0f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                break;
            case "High_Occlusion":              // 高オクルージョン条件
                target_a_id = 2;                // 高オクルージョン条件のID
                target_p_id = 2;                // 高オクルージョン条件のID
                target_amount_all = 4;          // ターゲットの総数
                target_amount_select = 24;      // 選択（タスク）回数
                target_amount_count = 6;        // 繰り返し回数（ターゲットの総数が選択回数より少ない場合に使用する）
                Depth = 5.0f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                break;
            case "Density_and_Occlusion_5x5x5": // 密度＆オクルージョン条件
                target_a_id = 3;                // 密度＆オクルージョン条件のID
                target_p_id = 3;                // 密度＆オクルージョン条件のID
                target_amount_all = 124;        // ターゲットの総数
                target_amount_select = 25;      // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                break;
            case "Density_and_Occlusion_5x5x3": // 密度＆オクルージョン条件2
                target_a_id = 5;                // 密度＆オクルージョン条件のID
                target_p_id = 4;                // 密度＆オクルージョン条件2のID
                target_amount_all = 74;         // ターゲットの総数
                target_amount_select = 25;      // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                break;
            case "Small_5x5x5":                 // 密度＆オクルージョン条件
                target_a_id = 3;                // 高密度条件のID
                target_p_id = 5;                // 密度＆オクルージョン条件のID
                target_amount_all = 124;        // ターゲットの総数
                target_amount_select = 25;      // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                target_size_mini_switch = true;
                break;
            case "Small_5x5x3":                 // 密度＆オクルージョン条件
                target_a_id = 5;                // 密度＆オクルージョン条件2のID
                target_p_id = 6;                // 密度＆オクルージョン条件2のID
                target_amount_all = 74;         // ターゲットの総数
                target_amount_select = 25;      // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                target_size_mini_switch = true;
                break;
            case "Study_1":                     // 高密度条件
                target_a_id = 1;                // 高密度条件のID
                target_p_id = 1;                // 高密度条件のID
                target_amount_all = 25;         // ターゲットの総数
                target_amount_select = 25;      // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                break;
            case "Flat_1":                      // 高密度条件
                target_a_id = 6;                // 高密度条件のID
                target_p_id = 97;                // 高密度条件のID
                target_amount_all = 25;         // ターゲットの総数
                target_amount_select = 25;      // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                pointer_switch = false;         // ？？？
                break;
            case "TEST_16x3":                   // テスト用条件
                target_a_id = 4;                // 高密度条件のID
                target_p_id = 97;               // 密度＆オクルージョン条件2のID
                target_amount_all = 47;         // ターゲットの総数
                target_amount_select = 3;       // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                break;
            case "TEST_small_16x3":             // テスト用条件
                target_a_id = 4;                // 高密度条件のID
                target_p_id = 98;               // 密度＆オクルージョン条件2のID
                target_amount_all = 47;         // ターゲットの総数
                target_amount_select = 3;       // 選択（タスク）回数
                target_amount_count = 1;        // 繰り返し回数
                Depth = 3.5f;                   // 奥行き距離
                TaskTime = 60.0f;               // 最大タスク時間
                target_size_mini_switch = true;
                break;
            case "Random":                      // ランダム配置条件
                target_a_id = 99;               // 高密度条件のID
                target_p_id = 99;               // ランダム配置条件のID（配置条件を追加できるように99にしている）
                break;
            case "Random_small":                // ランダム配置条件
                target_a_id = 99;               // 高密度条件のID
                target_p_id = 99;               // ランダム配置条件のID（配置条件を追加できるように99にしている）
                target_size_mini_switch = true; // ？？？
                break;
            default:
                target_a_id = 0;                // 高密度条件のID
                target_p_id = 0;                // ？？？
                target_amount_all = 0;          // ？？？
                target_amount_select = 0;       // ？？？
                target_amount_count = 0;        // ？？？
                break;
        }
        //--------------------------------------------------------------


        // パラメータ条件管理-----------------------------------------------
        switch (parameter_setting.ToString()) // ココでパラメータ条件を毎にパラメータを調整
        {
            case "Study":                   // 実験時
                gaze_data_switch = true;    // 視線情報を取得
                TimeOut_switch = true;      // 長引いたタスクをタイムアウト
                LightSensor_switch = true;  // 画面明度を取得
                break;
            case "Debug":                   // デバッグ時
                gaze_data_switch = false;   // 視線情報の取得は無し
                TimeOut_switch = false;     // タイムアウトは無し
                LightSensor_switch = false; // 画面明度の取得は無し
                break;
            case "All_UI_Awake":            // UI系を全て表示する場合
                cursor_switch = true;       // カーソルを表示
                raycast_switch = true;      // レイを表示
                break;
            case "All_UI_Asleep":           // UI系を全て非表示する場合
                cursor_switch = false;      // カーソルを非表示
                raycast_switch = false;     // レイを非表示
                break;
            default:                        // テンプレート未使用時（Inspectorの設定をそのまま使用）
                break;
        }
        //--------------------------------------------------------------


        for (int i = 0; i < target_set.Length; i++) target_set[i].SetActive(false); // 表示されているターゲット群を全て非表示;


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

        input_start_time = dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString(); // ファイル名に使用する月日時分を保存
        filePath = Application.dataPath + "/Gaze_Team/BGC3D/Scripts/test_results/" + target_pattern + "__test_id = " + test_id + "__" + "target_p_id = " + target_p_id + "__" + "tester_id  = " + tester_id + "__" + tester_name + "__" + Brightness + "__" + input_start_time; // ファイル名を作成．秒単位の時間をファイル名に入れているため重複・上書きの可能性はほぼない
        streamWriter_gaze = File.AppendText(filePath + "_gaze_data.csv"); // 視線情報用のcsvファイルを作成

        if (gaze_data_switch) result_output_every ("fulltimestamp,timestamp,taskNo,target_id,target_x,target_y,target_z,gaze_x,gaze_y,pupil_r,pupil_l,blink_r,blink_l,hmd_x,hmd_y,hmd_z,LightValue,center,interval", streamWriter_gaze, false); // gaze_data_switchがtrue＝視線情報保存状態の場合はファイルを生成して書き込む．視線情報に先立って表のタイトルを追記．
        //--------------------------------------------------------------


        // ログ作成-----------------------------------------------------
        tasklogs = new List<string>(); // 実験データを保存するリストを初期化（データ分析をする分には消してもいい）
        task_start_time = new List<float>(); // タスクが開始された時の時間を保存するリストを初期化
        task_end_time = new List<float>(); // タスクが終了した時の時間を保存するリストを初期化（データ分析をする分には消してもいい）
        //--------------------------------------------------------------


        // 明度操作------------------------------------------------------
        _postProcess = this.GetComponent<PostProcessVolume>(); // PostProcessVolumeを取得
        foreach (PostProcessEffectSettings item in _postProcess.profile.settings) // PostProcessVolume内のエフェクトを検索
        {
            if (item as ColorGrading) _colorGrading = item as ColorGrading; // PostProcessVolume内のエフェクトからColor Gradingを検索して取得
        }
        //--------------------------------------------------------------
    }


    void Update()
    {
        if (free_mode == false) // ？？？
        {
            // method_change(); // 使用している手法を変更


            // コントローラの表示・非表示について-----------------------------
            if (controller_switch) // controller_switchがオンの場合
            {
                controller_R.gameObject.SetActive(true); // 右コントローラを表示
                controller_L.gameObject.SetActive(true); // 左コントローラを表示
            }
            else // controller_switchがオフの場合
            {
                controller_R.gameObject.SetActive(false); // 右コントローラを非表示
                controller_L.gameObject.SetActive(false); // 左コントローラを非表示
            }
            //--------------------------------------------------------------


            // ターゲット位置の調整------------------------------------------
            grapgrip = GrabG.GetState(SteamVR_Input_Sources.Any); // ？？？
            if (target_a_id == 0 || target_a_id == 99) grapgrip = false; // ？？？

            if (grapgrip || target_pos_calibration) // ？？？
            {
                target_set[target_a_id - 1].SetActive(true); // 指定した配置条件のターゲット群を表示する

                Vector3 forward = Vector3.Scale(head_obj.transform.forward, new Vector3(1, 0, 1)).normalized; // ユーザ（カメラ）の前方方向を取得
                Vector3 newPosition = head_obj.transform.position + forward * Depth; // ユーザ（カメラ）の位置をターゲット群の新しい位置に設定
                newPosition.y = head_obj.transform.position.y; // ターゲット群とユーザ（カメラ）の高さを同じにする
                target_set[target_a_id - 1].transform.position = newPosition; // ターゲット群を新しい位置に移動
                target_set[target_a_id - 1].transform.LookAt(head_obj.transform.position); // ターゲット群をユーザ（カメラ）の方向に向ける
                Vector3 rotation = target_set[target_a_id - 1].transform.eulerAngles; // ターゲット群が逆を向いてしまうので180度回転させる
                rotation.y += 180; // ？？？
                target_set[target_a_id - 1].transform.eulerAngles = rotation; // ？？？

                target_pos_calibration = false; // 機能フラグをリセット
            }
            //--------------------------------------------------------------


            if(_colorGrading) _colorGrading.brightness.value = Brightness; // 明度を更新


            //--------------------------------------------------------------
            if (test_id != 0) // 何らかの手法が選択されている場合
            {
                test_time += Time.deltaTime; // タスク時間を更新


                // タスクの推移管理---------------------------------------------
                if (select_target_id == 999 && taskflag == false) // ？？？
                {
                    taskflag = true; // ？？？
                    tasklogs.Add(""); // ？？？
                    task_start_time.Add(test_time); // ？？？
                    test_time_tmp = test_time; // ？？？
                    logoutput_count = 0; // ？？？
                    session_flag = true; // ？？？
                    selecting_target = null; // ？？？
                    select_target_id = -1; // ？？？
                }
                //--------------------------------------------------------------


                // 視線情報を取得する場合に視線が中央に存在するか否かを取得する処理-----
                if (gaze_data_switch) // 視線情報を取得する場合
                {
                    if (DwellTarget != null && DwellTarget.name != "ScreenSphere" && DwellTarget.GetComponent<target_para_set>().Id ==999) // 注視しているターゲットが存在し，かつそれが黒いターゲットでない場合
                    {
                        center_flag = "false"; // center_flagを"false"に更新
                    }
                    else
                    {
                        center_flag = "true"; // center_flagを"true"に更新
                    }
                }
                //--------------------------------------------------------------


                // タスクの状態チェック-----------------------------------------
                if (taskflag ==true && target_p_id != 97) // タスク中の場合
                {
                    if (TimeOut_switch == true && test_time - task_start_time[task_num] > 60.0f) task_skip = true; // 1分以上選択できなかった場合にタスクをスキップ

                    // ターゲットの選択が行われた時の処理-------------------------
                    if ((select_target_id != -1 && select_target_id != 999 && same_target == false))
                    {
                        tasklogs2.Add((task_num + 1) + "," + tasknums[task_num] + "," + select_target_id + "," + (test_time - test_time_tmp)); // タスク番号・選択すべきだったターゲット・選択されたターゲット・その選択に要した時間を追記
                        test_time_tmp = test_time;

                        if ((select_target_id == tasknums[task_num])) // 正しいターゲットを選択した時の処理
                        {
                            same_target = true; // ？？？
                            task_skip = false; // フラグを初期化

                            tasklogs[task_num] += ("select_target = " + select_target_id + ": " + test_time + "\n"); // 選択したターゲットのIIDとタスク完了時の時間を追記
                            tasklogs2[tasklogs2.Count - 1] += ("," + (test_time - task_start_time[task_num]) + "," + logoutput_count); // そのタスクの総時間とエラー数を追記

                            if (task_num < target_amount_select) // まだタスクが残っている場合
                            {
                                task_end_time.Add(test_time); // タスクが終了した時の時間を保存
                                task_num++; // タスクを次に進める
                                test_time_tmp = 0; // タスク時間を初期化
                                audioSource.PlayOneShot(sound_OK); // 正解した時の効果音を鳴らす
                                taskflag = false; // 非タスク中にする
                            }
                        }
                        else if (output_flag == false)// 間違えたターゲットを選択した時の処理
                        {
                            logoutput_count++; // 間違えた数をカウント

                            same_target = true; // ？？？
                            tasklogs[task_num] += ("select_target = " + select_target_id + ": " + test_time + "\n"); // ログを出力
                            audioSource.PlayOneShot(sound_NG); // 間違えた時の効果音を鳴らす
                            if (selecting_target) selecting_target.GetComponent<target_para_set>().dtime = 0; // ？？？
                        }
                    }
                    else if (task_skip && target_p_id != 97) // タスクをスキップした場合
                    {
                        tasklogs2.Add((task_num + 1) + "," + tasknums[task_num] + "," + select_target_id + "," + (test_time - test_time_tmp)); // タスク番号・選択すべきだったターゲット・選択されたターゲット・その選択に要した時間を追記
                        test_time_tmp = test_time; // 経過時間を更新

                        same_target = true; // ？？？
                        task_skip = false; // フラグを初期化

                        tasklogs[task_num] += ("select_target = " + "skip" + ": " + test_time + "\n"); // 選択したターゲットのIDとタスク完了時の時間を追記
                        tasklogs2[tasklogs2.Count - 1] += ("," + (test_time - task_start_time[task_num]) + "," + logoutput_count + ",skip"); // そのタスクの総時間とエラー数を追記

                        if (task_num < target_amount_select) // まだタスクが残っている場合
                        {
                            task_end_time.Add(test_time); // タスクが終了した時の時間を保存
                            task_num++; // タスクを次に進める
                            test_time_tmp = 0; // タスク時間を初期化
                            audioSource.PlayOneShot(sound_NG); // 正解した時の効果音を鳴らす
                            taskflag = false; // 非タスク中にする
                        }
                    }
                    //--------------------------------------------------------------


                    // セッションが終了するか，強制中断を行った時の処理--------------
                    if (task_num == target_amount_select && output_flag == false) // セッションが終了するか，強制中断を行った場合
                    {
                        output_flag = true; // 出力済みにする
                        audioSource.PlayOneShot(sound_END); // セッション終了時の音を鳴らす
                        taskflag = false; // ？？？
                        free_mode = true; // フリーモードをオン
                        result_output(); // 実験結果をテキスト形式で出力
                        result_output_csv(); // 実験結果をcsv形式で出力
                        result_output_every("", streamWriter_gaze, true); // 視線情報をcsv形式で出力
                    }
                    //--------------------------------------------------------------
                }
                else if (taskflag == true && target_p_id == 97)
                {
                    if (test_time - task_start_time[task_num] > TaskTime + 1.0f) task_skip = true; // 1分以上選択できなかった場合にタスクをスキップ

                    if (test_time - task_start_time[task_num] > 1.0f)
                    {
                        interval_flag = "false";
                    }
                    else
                    {
                        interval_flag = "true";
                    }

                    // ターゲットの選択が行われた時の処理-------------------------
                    if (task_skip) // タスクをスキップした場合
                    {
                        tasklogs2.Add((task_num + 1) + "," + tasknums[task_num] + "," + select_target_id + "," + (test_time - test_time_tmp)); // タスク番号・選択すべきだったターゲット・選択されたターゲット・その選択に要した時間を追記
                        test_time_tmp = test_time; // 経過時間を更新

                        same_target = true; // よくわからないフラグをTrueに
                        task_skip = false; // フラグを初期化

                        tasklogs[task_num] += ("select_target = " + select_target_id + ": " + test_time + "\n"); // 選択したターゲットのIDとタスク完了時の時間を追記
                        tasklogs2[tasklogs2.Count - 1] += ("," + (test_time - task_start_time[task_num]) + "," + logoutput_count); // そのタスクの総時間とエラー数を追記

                        if (task_num < target_amount_select) // まだタスクが残っている場合
                        {
                            taskflag = true; // ？？？
                            tasklogs.Add(""); // ？？？
                            task_start_time.Add(test_time); // ？？？
                            test_time_tmp = test_time; // ？？？
                            logoutput_count = 0; // ？？？
                            session_flag = true; // ？？？
                            selecting_target = null; // ？？？
                            select_target_id = -1; // ？？？

                            task_end_time.Add(test_time); // タスクが終了した時の時間を保存
                            task_num++; // タスクを次に進める
                            test_time_tmp = 0; // タスク時間を初期化
                            audioSource.PlayOneShot(sound_OK); // 正解した時の効果音を鳴らす
                        }
                    }
                    //--------------------------------------------------------------


                    // セッションが終了するか，強制中断を行った時の処理--------------
                    if (task_num == target_amount_select && output_flag == false) // セッションが終了するか，強制中断を行った場合
                    {
                        output_flag = true; // 出力済みにする
                        audioSource.PlayOneShot(sound_END); // セッション終了時の音を鳴らす
                        taskflag = false; // ？？？
                        free_mode = true; // フリーモードをオン
                        result_output(); // 実験結果をテキスト形式で出力
                        result_output_csv(); // 実験結果をcsv形式で出力
                        result_output_every("", streamWriter_gaze, true); // 視線情報をcsv形式で出力
                    }
                    //--------------------------------------------------------------
                }
                //--------------------------------------------------------------


                // タスクを中断する際の処理-------------------------------------
                if (error_output_flag) // 強制中断した場合
                {
                    error_output_flag = false; // 出力済みにする
                    audioSource.PlayOneShot(sound_END); // セッション終了時の音を鳴らす
                    error_output(); // 実験結果をテキスト形式で出力
                    result_output_csv(); // 視線情報をcsv形式で出力
                    result_output_every("", streamWriter_gaze, true); // 視線情報をcsv形式で出力
                }
                //--------------------------------------------------------------
            }


            Quaternion HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head); //回転座標をクォータニオンで値を受け取る，旧式らしいので要修正
            HMDRotation = HMDRotationQ.eulerAngles; // 取得した値をクォータニオン → オイラー角に変換
            lightValue = sensor.lightValue; // 画面全体の明度情報を更新
            if (gaze_data_switch) if (output_flag == false && taskflag == true) result_output_every(gaze_data.get_gaze_data(), streamWriter_gaze, false); // 視線関係のデータを取得＆書き出し
        }
        else
        {
            // フリーモードの場合の処理を記述するスペース----------------------
            // hogehoge
            // hogehoge
            // hogehoge
            //--------------------------------------------------------------
        }

    }
    //--------------------------------------------------------------


    // 手法を変更する関数---------------------------------------------
    private void method_change()
    {
        // コントローラのタッチパッドで使用手法を変更する処理-------------------
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
        if (now_test_pattern != test_pattern.ToString() || switch_flag == 1) // ？？？
        {
            now_test_pattern = test_pattern.ToString(); // ？？？


            // ？？？----------------------------------------------------------
            if (DwellTarget != null) // ？？？
            {
                DwellTarget.GetComponent<Renderer>().material.color = Color.white; // ？？？
            }
            //--------------------------------------------------------------


            // パラメータ初期化-------------------------------------------------
            controller_switch = false; // ？？？
            bubblegaze.SetActive(false); // ？？？
            gazeraycast.SetActive(false); // ？？？
            controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            //--------------------------------------------------------------


            // パラメータ更新---------------------------------------------------
            if (test_id == 0) // ？？？
            {
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            }
            else if (test_id < 3) // ？？？
            {
                bubblegaze.SetActive(true); // ？？？
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？

                if (test_id == 2) // ？？？
                {
                    gazeraycast.SetActive(true); // ？？？
                }
            }
            else if (test_id == 3) // ？？？
            {
                gazeraycast.SetActive(true); // ？？？
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            }
            else if (test_id == 4) // ？？？
            {
                controller_switch = true; // ？？？
                controller_R.GetComponent<SteamVR_LaserPointer>().active = true; // ？？？
                controller_L.GetComponent<SteamVR_LaserPointer>().active = true; // ？？？
            }
            else if (test_id == 5) // ？？？
            {
                gazeraycast2.SetActive(true); // ？？？
                controller_R.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
                controller_L.GetComponent<SteamVR_LaserPointer>().active = false; // ？？？
            }

            if (test_id == 2) // ？？？
            {
                bubblegaze.GetComponent<Collider>().enabled = false; // ？？？
            }
            else
            {
                bubblegaze.GetComponent<Collider>().enabled = true; // ？？？
            }
            //--------------------------------------------------------------
        }
        //--------------------------------------------------------------


        // ？？？--------------------------------------------------------
        if (now_target_pattern != target_pattern.ToString()) // ？？？
        {
            now_target_pattern = target_pattern.ToString(); // ？？？
        }
        //--------------------------------------------------------------


        // ？？？--------------------------------------------------------
        if (lens_switch) // ？？？
        {
            if (lens_flag) // ？？？
            {
                Lens_Object.SetActive(true); // ？？？
            }
            else // ？？？
            {
                Lens_Object.SetActive(false); // ？？？
            }
        }
        //--------------------------------------------------------------
    }
    // private void method_change() 終了-------------------------------


    // 結果を出力する関数----------------------------------------------
    public void result_output()
    {
        Debug.Log("data_input_start!!"); // 確認メッセージを出力


        //ファイル生成-------------------------------------------------
        StreamWriter streamWriter = File.AppendText(filePath + ".txt"); // ？？？
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------"); // ？？？
        streamWriter.WriteLine("target_pattorn:"); // ？？？
        //--------------------------------------------------------------


        // ？？？----------------------------------------------------------
        for (int i = 0; i < target_amount_all - 5; i++) // ？？？
        {
            streamWriter.WriteLine(tasknums[i]); // ？？？
            //streamWriter.WriteLine(" ");
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------"); // ？？？
        streamWriter.WriteLine("target_pattorn:"); // ？？？
        for (int i = 0; i < target_amount_select; i++) // ？？？
        {
            streamWriter.WriteLine(tasknums[i]); // ？？？
        }
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------"); // ？？？
        //--------------------------------------------------------------


        // 開始時間-----------------------------------------------------
        streamWriter.WriteLine("test_start_time: " + task_start_time[0]); // ？？？
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------"); // ？？？
        //--------------------------------------------------------------


        // 各タスクの計測を追記-----------------------------------------
        for (int i = 0; i < target_amount_select; i++) // ？？？
        {
            streamWriter.WriteLine("-----------------------------------------------------------------------------------------\n"); // ？？？
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_start: " + task_start_time[i]); // ？？？
            streamWriter.WriteLine(tasklogs[i]); // ？？？
            streamWriter.WriteLine("task=" + (i + 1) + "_select=" + tasknums[i] + "_end: " + task_end_time[i]); // ？？？
            streamWriter.WriteLine("task_time: " + ( task_end_time[i] - task_start_time[i])); // ？？？
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
        streamWriter.WriteLine("-----------------------------------------------------------------------------------------"); // ？？？
        streamWriter.WriteLine("test_end_time: " + task_end_time[target_amount_select-1]); // ？？？
        //--------------------------------------------------------------


        // 後処理-------------------------------------------------------
        streamWriter.Flush(); // ？？？
        streamWriter.Close(); // ファイルを閉じる
        Debug.Log("data_input_end!!"); // 確認メッセージを出力
        //--------------------------------------------------------------
    }
    // public void result_output() 終了-----------------------------


    // 実験結果をcsv形式で出力する関数------------------------------
    public void result_output_csv()
    {
        Debug.Log("data_input_csv_start!!"); // 確認メッセージを出力
        StreamWriter streamWriter = File.AppendText(filePath + ".csv"); //ファイル生成


        // 各タスクの計測を追記-----------------------------------------
        streamWriter.WriteLine("task,target,select,time,totaltime,totalerror"); // 表のタイトルを出力
        for (int i = 0; i < tasklogs2.Count; i++) streamWriter.WriteLine(tasklogs2[i]); // 実験結果を出力
        //--------------------------------------------------------------


        // 後処理-------------------------------------------------------
        streamWriter.Flush(); // ？？？
        streamWriter.Close(); // ファイルを閉じる
        Debug.Log("data_input_end!!"); // 確認メッセージを出力
        //--------------------------------------------------------------
    }
    // public void result_output_csv() 終了--------------------------


    // 視線情報をcsv形式で出力する関数（処理が重いので使っていない）-
    public void result_output_csv2()
    {
        Debug.Log("data_input_csv_start2!!"); // 確認メッセージを出力
        StreamWriter streamWriter = File.AppendText(filePath + "_gaze_data.csv"); //ファイル生成


        // 各タスクの計測を追記----------------------------------------
        streamWriter.WriteLine("timestamp,taskNo,gaze_x,gaze_y,pupil_r,pupil_l,blink_r,blink_l,hmd_x,hmd_y,hmd_z,LightValue"); // 表のタイトルを出力
        for (int i = 0; i < tasklogs3.Count; i++) streamWriter.WriteLine(tasklogs3[i]); // 実験結果を出力
        //--------------------------------------------------------------


        // 後処理-------------------------------------------------------
        streamWriter.Flush(); // ？？？
        streamWriter.Close(); // ファイルを閉じる
        Debug.Log("data_output_end2!!"); // 確認メッセージを出力
        //--------------------------------------------------------------
    }
    // public void result_output_csv2() 終了------------------------


    // 引数をファイルに出力する関数---------------------------------
    public void result_output_every(string data, StreamWriter streamWriter, bool endtask) // これが呼び出されるたびに変数（第一引数の文字列）をcsvファイルに出力
    {
        if (endtask == false) streamWriter.WriteLine(data); // falseの場合は書き込み処理，trueの場合は閉じる処理


        // 閉じる処理---------------------------------------------------
        if (endtask)
        {
            streamWriter.Close(); // ファイルを閉じる
            Debug.Log("data_output_every!!"); // 確認メッセージを出力
        }
        //--------------------------------------------------------------
    }
    // public void result_output_every() 終了-----------------------


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
    // public void error_output() 終了------------------------------


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
                                if (!(ransu == 13 && (target_a_id == 3 || target_a_id == 4 || target_a_id == 5)))
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
    // void set_testpattern() 終了----------------------------------


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
    //private void random_target_set() 終了-------------------------


    // アプリケーション終了時の処理を行う関数-----------------------
    private void OnApplicationQuit()
    {
        result_output_every("", streamWriter_gaze, true);   // 視線データを保存したファイルを閉じる
        Debug.Log("data_output_every!!");                   // 確認メッセージを出力
    }
    // private void OnApplicationQuit() 終了------------------------


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
    // public void Blink3() 終了------------------------------------


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
    // public void Blink() 終了-------------------------------------
}
