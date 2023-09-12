using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class target_para_set : MonoBehaviour
{
    public GameObject Server;   // サーバと接続
    private receiver script;    // サーバと接続
    private results script2;    // サーバと接続
    public float dtime;         // 注視時間
    public int Id;              // ターゲットID


    void Start()
    {
        script = Server.GetComponent<receiver>(); // サーバと接続

        if (script.target_size_mini_switch) // ターゲットの縮小化機能がオンの場合
        {
            float distance_of_camera_to_target = Vector3.Distance(script.head_obj.transform.position, this.transform.position); // ユーザとターゲット間の距離を計算
            float angleRadians = 1.0f * Mathf.Deg2Rad; // 角度をラジアンに変換
            float height = (Mathf.Tan(angleRadians) * distance_of_camera_to_target); // ターゲットの最小サイズを計算
            this.transform.localScale = new Vector3(height, height, height); // ターゲットのサイズを変更
        }

        if (script.target_a_id != 99) // ターゲットパターンが「ランダム」の場合
        {
            if (this.GetComponent<target_size_set>()) // ターゲットがtarget_size_setを持っている場合
            {
                this.GetComponent<target_size_set>().enabled = false; // target_size_setをオフ
            }
        }
    }

    void Update()
    {
        if (Server.GetComponent<receiver>().enabled == false) // サーバがオフの場合
        {
            // 結果表示用の処理---------------------------------------------
            for (int i = 1; i < script2.csvDatas.Count; i++) // ？？？
            {
                if (Id == int.Parse(script2.csvDatas[i][2])) this.GetComponent<Renderer>().material.color = Color.blue; // ？？？
            }
            //--------------------------------------------------------------

            return; // 終了
        }


        // 累積注視時間モード時の処理----------------------------------
        if (script.total_DwellTime_mode) // 累積注視時間モードがオンの時
        {
            if (script.DwellTarget != this.gameObject) // このターゲットと注視中のターゲットが異なる場合
            {
                if (dtime > 0) // 累計注視時間が0より大きい場合
                {
                    dtime -= Time.deltaTime; // 累計注視時間を非注視時間分減らす
                }
                else
                {
                    dtime = 0; // 累計注視時間を0にする
                }
            }

            if(Id != 999 && script.taskflag == false) // IDが999（＝黒いターゲット）かつ非タスク状態の場合
            {
                dtime = 0; // 累計注視時間を0にする．この処理のせいで非タスク中に練習を行えないため，要改良部分
            }
            else if (Id == 999 && script.taskflag == true) // ？？？
            {
                dtime = 0; // 累計注視時間を0に更新
            }
        }
        else
        {
            if (script.DwellTarget != this.gameObject) // このターゲットと注視中のターゲットが異なる場合
            {
                dtime = 0; // 累計注視時間を0に更新
            }
        }
        //--------------------------------------------------------------


        // 注視状態から選択状態への移行---------------------------------
        if (dtime >= script.set_dtime) // 累計注視時間が設定した時間以上の場合
        {
            script.selecting_target = this.gameObject; // 選択されたターゲットを更新
            script.select_target_id = Id; // 選択されたターゲットのIDを更新
            script.same_target = false; // ？？？
        }
        //--------------------------------------------------------------


        // タスクで選択したターゲットの情報を取得する処理---------------------
        if (script.gaze_data_switch) // 視線データの出力機能がオンの場合
        {
            if (script.tasknums[script.task_num] == Id) // タスクで提示されているターゲットのIDと同じIDを持っている場合
            {
                script.taskObject = this.gameObject; // タスクで提示されているターゲットを更新
            }
        }
        //--------------------------------------------------------------


        // 色変化の処理（リファクタリング済み）----------------------------
        float color_correction = 0.0f; // 明度補正用の変数を定義
        if (script.bright_correction_mode) color_correction = script.Brightness / 160.0f; // 明度補正モードがオンなら明度補正用のゲインを格納

        if (script.output_flag || Id == script.select_target_id) // 実験結果が出力された，また選択状態のターゲットのIDと同じIDを持っている場合
        {
            // this.GetComponent<Renderer>().material.color = script.target_color; // ターゲットの色を変更
            this.GetComponent<Renderer>().material.color = new Color(color_correction, 1, color_correction); // ターゲットの色を変更
        }
        else if (Id == 999) // IDが999（＝黒いターゲット）の場合
        {
            if (script.DwellTarget != null && script.DwellTarget.name == this.name)
            {
                // this.GetComponent<Renderer>().material.color = script.select_color; // ターゲットの色を変更
                this.GetComponent<Renderer>().material.color = new Color(script.select_color.r, script.select_color.g + color_correction * (1.0f - script.select_color.g), color_correction); // ターゲットの色を変更
            }
            else
            {
                // this.GetComponent<Renderer>().material.color = Color.black; // ターゲットを黒色に変更
                this.GetComponent<Renderer>().material.color = new Color(color_correction, color_correction, color_correction); // ターゲットの色を変更
            }
        }
        else if (script.DwellTarget != null) // 注視状態のターゲットが存在しない場合
        {
            if (script.DwellTarget.name == this.name) // 注視状態のターゲットの名前と同じ名前の場合．名前よりもIDの方がまだ一意性を担保できるので要リファクタリング
            {
                // this.GetComponent<Renderer>().material.color = script.select_color; // ターゲットの色を変更
                this.GetComponent<Renderer>().material.color = new Color(script.select_color.r, script.select_color.g + color_correction * (1.0f - script.select_color.g), color_correction); // ターゲットの色を変更
            }
            else if (script.target_p_id != 99 && Id == script.tasknums[script.task_num] && script.taskflag) // 提示IDと同じIDを持っている場合
            {
                // this.GetComponent<Renderer>().material.color = Color.blue; // ターゲットを青色に変更
                this.GetComponent<Renderer>().material.color = new Color(color_correction, color_correction, 1); // ターゲットの色を変更
            }
            else
            {
                this.GetComponent<Renderer>().material.color = Color.white; // ターゲットを白色に変更
            }
        }
        else if (script.target_p_id != 99 && Id == script.tasknums[script.task_num] && script.taskflag) // 提示IDと同じIDを持っている場合
        {
            // this.GetComponent<Renderer>().material.color = Color.blue; // ターゲットを青色に変更
            this.GetComponent<Renderer>().material.color = new Color(color_correction, color_correction, 1); // ターゲットの色を変更
        }
        else // 以上の条件に該当しない場合
        {
            this.GetComponent<Renderer>().material.color = Color.white; // ターゲットを白色に変更
        }
        //--------------------------------------------------------------
    }
}
