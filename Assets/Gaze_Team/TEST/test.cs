using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test : MonoBehaviour
{
    public List<string> tasklogs;
    public List<string> dummy;
    private string input_start_time;
    private string filePath;
    private float test_time;
    private bool flag = false;

    // Start is called before the first frame update
    void Start()
    {
        DateTime dt = DateTime.Now;
        input_start_time = dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
        filePath = Application.dataPath + "/BGS3D/Scripts/test_results/add-" + input_start_time;
        //filePath = Application.dataPath + "/BGS3D/Scripts/test_results/" + test_id + "_" + test_pattern + "_" + target_p_id + "_" + target_pattern + "_" + tester_id + "_" + tester_name + ".txt";

        // ÉçÉOçÏê¨
        tasklogs = new List<string>();
        dummy = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        // éûä‘åvë™
        test_time += Time.deltaTime;

        for ( int i = 0; i < 75000; i++ )
        {
            dummy.Add("int = " + i);
        }
        dummy.Clear();


        //if(flag == false) adddata();

        if(test_time > 150 && flag == false)
        {
            result_output();
            flag = true;
        }
    }

    public void result_output()
    {
        StreamWriter streamWriter = File.AppendText(filePath + ".csv");

        for (int i = 0; i < tasklogs.Count; i++)
        {
            streamWriter.WriteLine(tasklogs[i]);
        }

        // å„èàóù
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("data_input_end!!");
    }

    public void adddata()
    {
        tasklogs.Add(test_time + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time));
    }
}
