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
    public GameObject Server;
    private receiver script;
    private results script2;
    public float dtime;
    public int Id;

    void Start()
    {
        script = Server.GetComponent<receiver>();
        script2 = Server.GetComponent<results>();
    }

    void Update()
    {
        if (Server.GetComponent<receiver>().enabled == false)
        {
            // 結果表示用の処理---------------------------------------------
            for (int i = 1; i < script2.csvDatas.Count; i++)
            {
                if (Id == int.Parse(script2.csvDatas[i][2]))
                {
                    this.GetComponent<Renderer>().material.color = Color.blue;
                }
            }
            //--------------------------------------------------------------

            return;
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
        }
        else
        {
            if (script.DwellTarget != this.gameObject) // このターゲットと注視中のターゲットが異なる場合
            {
                dtime = 0; // 累計注視時間を0にする
            }
        }
        //--------------------------------------------------------------


        if (dtime >= script.set_dtime) // 累計注視時間が設定した時間以上の場合，
        {
            script.selecting_target = this.gameObject; // 選択されたターゲットを更新
            script.select_target_id = Id; // 選択されたターゲットのIDを更新
            script.same_target = false; // ？？？
        }


        // 色変化の処理（要リファクタリング部分）-----------------------
        if (script.output_flag || Id == script.select_target_id)
        {
            this.GetComponent<Renderer>().material.color = script.target_color; //
        }
        else if (Id == 999)
        {
            this.GetComponent<Renderer>().material.color = Color.black; // ターゲットを黒色に変更
        }
        else if (script.DwellTarget != null)
        {
            if (script.DwellTarget.name == this.name)
            {
                this.GetComponent<Renderer>().material.color = script.select_color; // ターゲットの色を変更
            }
            else if (Id == script.tasknums[script.task_num] && script.taskflag)
            {
                this.GetComponent<Renderer>().material.color = Color.blue; // ターゲットを青色に変更
            }
            else
            {
                this.GetComponent<Renderer>().material.color = Color.white; // ターゲットを白色に変更
            }
        }
        else if (Id == script.tasknums[script.task_num] && script.taskflag)
        {
            this.GetComponent<Renderer>().material.color = Color.blue; // ターゲットを青色に変更
        }
        else
        {
            this.GetComponent<Renderer>().material.color = Color.white; // ターゲットを白色に変更
        }







        //if (script.controller_switch)
        //{
        //    if (script.output_flag) // セッション終了時
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color;
        //    }
        //    else if (Id == script.tasknums[script.task_num] && script.taskflag) // 提示したターゲットとIDが同じ，かつタスク中の場合
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.blue; // ターゲットを青色に変更
        //    }
        //    else if (Id == 999) // ターゲットが黒色のIDを持つ場合
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.black; // ターゲットを黒色に変更
        //    }
        //    else if (script.DwellTarget != null) // 注視中のターゲットが無い場合
        //    {
        //        if (script.DwellTarget.GetComponent<target_para_set>().Id == Id) // 注視中のターゲットのIDを更新
        //        {
        //            this.GetComponent<Renderer>().material.color = script.select_color; // ターゲットの色を変更
        //        }
        //    }
        //    else
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.white; // ターゲットを白色に変更
        //    }
        //}
        //else
        //{
        //    if (script.output_flag) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color; //
        //    }
        //    else if (Id == script.select_target_id) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color; //
        //    }
        //    else if (script.DwellTarget == null) //
        //    {
        //        if (script.taskflag == false && Id != 999) //
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.white; //
        //        }
        //        else if (Id == script.select_target_id) //
        //        {
        //            this.GetComponent<Renderer>().material.color = script.target_color; //
        //        }
        //        else if (Id == script.tasknums[script.task_num]) //
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.blue; //
        //        }
        //        else if (Id == 999) //
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.black; //
        //        }
        //        else
        //        {
        //            this.GetComponent<Renderer>().material.color = Color.white; //
        //        }
        //    }
        //    else if (script.DwellTarget.GetComponent<target_para_set>().Id == Id) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.select_color; //
        //    }
        //    else if (script.taskflag == false && Id != 999) //
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.white; //
        //    }
        //    else if (Id == script.select_target_id) //
        //    {
        //        this.GetComponent<Renderer>().material.color = script.target_color; //
        //    }
        //    else if (Id == script.tasknums[script.task_num]) //
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.blue; //
        //    }
        //    else if (Id == 999) //
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.black; //
        //    }
        //    else
        //    {
        //        this.GetComponent<Renderer>().material.color = Color.white; //
        //    }
        //}
    }

    private void OnTriggerEnter(Collider collider)
    {
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider collider)
    {
    }
}
