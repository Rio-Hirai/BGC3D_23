using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeCalibrator : MonoBehaviour
{
    public receiver server;

    void Start()
    {

    }

    void Update()
    {
        if (server.eye_calibration)
        {
            SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
            server.eye_calibration = false;
        }
    }
}
