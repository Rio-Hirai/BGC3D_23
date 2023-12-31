using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class focus_BGC_v2 : MonoBehaviour
{
    private FocusInfo FocusInfo;                            // レイ（視線）と衝突したターゲットの情報を格納する変数
    private readonly float MaxDistance = 20;                // レイの最大長
    private static EyeData_v2 eyeData = new EyeData_v2();   // 各種視線情報を格納する変数
    private bool eye_callback_registered = false;           // callback関係
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };// ？？？

    public receiver script;                                 // サーバ接続
    public GameObject pointer;                              // ポインタ
    public GameObject objectName_now;                       // 現在のターゲット
    public GameObject objectName_new;                       // 新しいターゲット
    [SerializeField] private string tagName = "Targets";    // 注視可能対象の選定．インスペクタで変更可能

    private void Start()
    {
        // SRanipal_Eye_Frameworkが正常に動いていない場合の処理---------
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
        //--------------------------------------------------------------
    }

    private void Update()
    {
        if (script.test_id == 6) // 使用手法が「Bubble_Gaze_Cursor_with_Gaze_Ray」の場合
        {
            // 視線情報を取得-----------------------------------------------
            if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING && SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

            if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
            {
                SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = true;
            }
            else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
            {
                SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                eye_callback_registered = false;
            }
            //--------------------------------------------------------------


            // オブジェクト選択---------------------------------------------
            foreach (GazeIndex index in GazePriority)
            {
                Ray GazeRay; // レイの情報
                bool eye_focus; // レイと衝突しているターゲットの有無
                int dart_board_layer_id = LayerMask.NameToLayer(tagName); // 指定したレイヤの番号を取得


                // 視線の方向情報を取得-----------------------------------------
                if (eye_callback_registered)
                {
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
                }
                else
                {
                    eye_focus = SRanipal_Eye_v2.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));
                }
                //--------------------------------------------------------------


                // レイとターゲットの衝突処理-----------------------------------
                if (eye_focus) // レイに衝突しているターゲットが存在する場合
                {
                    // pointer.transform.position = FocusInfo.point; // ポインタオブジェクトの位置を更新

                    objectName_new = FocusInfo.collider.gameObject; // レイと衝突しているターゲットを変数に格納
                    script.RayTarget = objectName_new; // レイと衝突しているターゲットを更新
                }
                else
                {
                }
                //--------------------------------------------------------------
            }
            //if (script.test_id == 6) 終了---------------------------------


            // オブジェクト選択---------------------------------------------
            if (objectName_now != objectName_new) script.DwellTarget = objectName_new; //注視しているオブジェクトを更新

            if (objectName_new != null) // オブジェクトが空でない場合
            {
                if (script.DwellTarget == objectName_new) // ？？？
                {
                    objectName_new.GetComponent<target_para_set>().dtime += Time.deltaTime * (objectName_new.GetComponent<target_para_set>().dtime * objectName_new.GetComponent<target_para_set>().dtime + 1.0f); // 注視中のオブジェクトの総連続注視時間を追加
                    objectName_now = objectName_new; //注視しているオブジェクトを更新
                }
            }
            //--------------------------------------------------------------


            //--------------------------------------------------------------
            if (script.pointer_switch) // ？？？
            {
                pointer.SetActive(true); // ？？？
            }
            else // ？？？
            {
                pointer.SetActive(false); // ？？？
            }
            //--------------------------------------------------------------
        }
    }

    private void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
        }
    }

    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        eyeData = eye_data; // 各種視線情報を更新
    }
}