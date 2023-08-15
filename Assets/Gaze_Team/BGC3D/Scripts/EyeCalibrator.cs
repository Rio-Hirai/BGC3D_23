using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeCalibrator : MonoBehaviour
{
    [SerializeField] private receiver server; // �T�[�o�ڑ�

    void Update()
    {
        // �����̃L�����u���[�V��������---------------------------------
        // �����inspector������蓮��eye_calibration��True�ɂ��邱�ƂŎ��s���邪�C��Script��eye_calibration��ύX���邱�Ƃł����s�͉\
        if (server.eye_calibration) // �����̃L�����u���[�V�����p�t���O��True�̏ꍇ
        {
            SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero); // �����̃L�����u���[�V���������s
            server.eye_calibration = false; // �����̃L�����u���[�V�����p�t���O��False�ɍX�V
        }
        //--------------------------------------------------------------
    }
}
