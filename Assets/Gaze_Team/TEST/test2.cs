using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test2 : MonoBehaviour
{
    public List<string> tasklogs;
    private string input_start_time;
    private string filePath;
    private float test_time;
    private StreamWriter streamWriter;
    private bool flag = false;
    public List<string> dummy;



    // Start is called before the first frame update
    void Start()
    {
        DateTime dt = DateTime.Now;
        input_start_time = dt.Month.ToString() + dt.Day.ToString() + dt.Hour.ToString() + dt.Minute.ToString() + dt.Second.ToString();
        filePath = Application.dataPath + "/BGS3D/Scripts/test_results/input-" + input_start_time;
        //filePath = Application.dataPath + "/BGS3D/Scripts/test_results/" + test_id + "_" + test_pattern + "_" + target_p_id + "_" + target_pattern + "_" + tester_id + "_" + tester_name + ".txt";

        // ƒƒOì¬
        tasklogs = new List<string>();
        dummy = new List<string>();

        streamWriter = File.AppendText(filePath + ".csv");
    }

    // Update is called once per frame
    void Update()
    {
        // ŠÔŒv‘ª
        test_time += Time.deltaTime;

        // ˆ—•‰‰×—p
        for (int i = 0; i < 75000; i++)
        {
            dummy.Add("int = " + i);
        }
        dummy.Clear();

        //if (flag == false) streamWriter.WriteLine(test_time + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time));
        result_output_every((test_time + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time) + "," + (test_time)), streamWriter);

        if (test_time > 15 && flag == false)
        {
            closefile(streamWriter);
            flag = true;
        }
    }

    public void result_output_every(string data, StreamWriter streamWriter)
    {
        if (flag == false) streamWriter.WriteLine(data);
    }

    public void closefile(StreamWriter streamWriter)
    {
        // Œãˆ—
        streamWriter.Close();
        Debug.Log("data_input_end!!");
    }
}
