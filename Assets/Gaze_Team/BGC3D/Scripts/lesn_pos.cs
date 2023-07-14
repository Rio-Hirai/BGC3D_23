using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lesn_pos : MonoBehaviour
{
    public GameObject Server;
    public GameObject head_point;
    public GameObject gaze_point;
    public float head_gain = 1.0f;
    public float gaze_gain = 1.0f;
    private receiver script;
    // Start is called before the first frame update
    void Start()
    {
        script = Server.GetComponent<receiver>();
    }

    // Update is called once per frame
    void Update()
    {
        if (script.lens_flag)
        {
            //this.transform.position = (head_point.transform.position * head_gain + gaze_point.transform.position * gaze_gain) / (head_gain + gaze_gain);
            this.transform.position = (head_point.transform.position * head_gain + script.selecting_target.transform.position * gaze_gain) / (head_gain + gaze_gain);
            script.lens_flag2 = false;
        }
    }
}
