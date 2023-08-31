using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class ScriptSwitch : MonoBehaviour
{
    [SerializeField] private receiver server;

    // Update is called once per frame
    void Update()
    {
        //--------------------------------------------------------------
        if (server.approve_switch)
        {
            server.head_obj.GetComponent<AngularVelocityCalculator>().enabled = true;
            server.GetComponent<approve_test>().enabled = true;
        }
        else
        {
            server.head_obj.GetComponent<AngularVelocityCalculator>().enabled = false;
            server.GetComponent<approve_test>().enabled = false;
        }
        //--------------------------------------------------------------


        //--------------------------------------------------------------
        if (server.gaze_data_switch)
        {
            server.gaze_data.enabled = true;
            server.GetComponent<gaze_data_output>().enabled = true;
        }
        else
        {
            server.gaze_data.enabled = false;
            server.GetComponent<gaze_data_output>().enabled = false;
        }
        //--------------------------------------------------------------


        //--------------------------------------------------------------
        if (server.LightSensor_switch)
        {
            server.GetComponent<LightSensor>().enabled = true;
        }
        else
        {
            server.GetComponent<LightSensor>().enabled = false;
        }
        //--------------------------------------------------------------


        //--------------------------------------------------------------
        if (server.dtime_monitor_switch)
        {
            server.dtime_monitor.GetComponent<dtime_output>().enabled = true;
        }
        else
        {
            server.dtime_monitor.GetComponent<dtime_output>().enabled = false;
        }
        //--------------------------------------------------------------


        //--------------------------------------------------------------
        if (server.MAverageFilter)
        {
            server.gazeraycast2.GetComponent<MovingAverageFilter>().enabled = true;
        }
        else
        {
            server.gazeraycast2.GetComponent<MovingAverageFilter>().enabled = false;
        }
        //--------------------------------------------------------------
    }
}
