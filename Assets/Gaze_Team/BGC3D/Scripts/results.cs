using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR.Extras;
using Valve.VR;

public class results : MonoBehaviour
{
    // 実験用パラメータ
    private int test_id;
    public enum test_pattern_list
    {
        Bubble_Gaze_Cursor,
        Gaze_Raycast,
        Controller_Raycast
    }
    public test_pattern_list test_pattern = test_pattern_list.Bubble_Gaze_Cursor;
    private int target_p_id;
    public enum target_pattern_list
    {
        High_Density,
        High_Occlusion,
        Density_and_Occlusion
    }
    public target_pattern_list target_pattern = target_pattern_list.High_Density;
    public int task_num = 0; // タスクの番号
    public int select_target_id = 0; // 選択されたターゲットのID
    public List<string> tasklogs;

    public GameObject[] target_set;

    TextAsset csvFile; // CSVファイル
    public List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;

    void Start()
    {
        // モード管理
        switch (test_pattern.ToString())
        {
            case "Bubble_Gaze_Cursor":
                test_id = 1;
                break;
            case "Gaze_Raycast":
                test_id = 2;
                break;
            case "Controller_Raycast":
                test_id = 3;
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
                break;
            case "High_Occlusion":
                target_p_id = 2;
                break;
            case "Density_and_Occlusion":
                target_p_id = 3;
                break;
            default:
                target_p_id = 0;
                break;

        }


        target_set[target_p_id - 1].SetActive(true);

        csvFile = Resources.Load("testCSV") as TextAsset; // Resouces下のCSV読み込み
        StringReader reader = new StringReader(csvFile.text);

        int i = 0;
        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string line = reader.ReadLine(); // 一行ずつ読み込み
            csvDatas.Add(line.Split(',')); // , 区切りでリストに追加
            Debug.Log(csvDatas[i][0] + ", " + csvDatas[i][1] + ", " + csvDatas[i][2] + ", " + csvDatas[i][3] + ", " + csvDatas[i][4] + ", " + csvDatas[i][5]);
            i++;
            //if (csvDatas[i][5] != null)
            //{
            //    tasklogs.Add(csvDatas[i++][5]);
            //} else
            //{
            //    tasklogs.Add("-1");
            //    i++;
            //}
            
        }

        // csvDatas[行][列]を指定して値を自由に取り出せる
        //Debug.Log(csvDatas[0][1]);
    }

    // Update is called once per frame
    void Update()
    {
    }
}