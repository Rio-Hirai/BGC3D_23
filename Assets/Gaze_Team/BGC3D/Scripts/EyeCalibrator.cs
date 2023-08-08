using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeCalibrator : MonoBehaviour
{
    [SerializeField] private receiver server;

    void Update()
    {
        if (server.eye_calibration)
        {
            SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
            server.eye_calibration = false;
        }
    }
}
