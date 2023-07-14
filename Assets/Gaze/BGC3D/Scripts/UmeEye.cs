using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;



namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {


            public class UmeEye : MonoBehaviour

            {
                private static EyeData eyeData;
                private static VerboseData verboseData;
                private float pupilDiameterLeft, pupilDiameterRight, pupilDiameterCombined;
                private Vector2 pupilPositionLeft, pupilPositionRight, pupilPositionCombined;
                private float eyeOpenLeft, eyeOpenRight, eyeOpenCombined;
                private Vector3 gaze_direction_right, gaze_direction_left, gaze_direction_combined;
                private Vector3 gaze_origin_right, gaze_origin_left, gaze_origin_combined;
                private float convergence_distance;

                //
                private Ray ray;
                private FocusInfo focusInfo;
                public float radius = 5.0f;
                public float maxradius = 5.0f;
                private float LeftOpenness, RightOpenness;

                private Vector3 origin;
                private Vector3 direction;


                public ParticleSystem hitRight;
                public ParticleSystem hitLeft;

                // Use this for initialization
                void Start()
                {

                }

                // Update is called once per frame
                void Update()
                {

                   // SRanipal_Eye.GetEyeOpenness
                    SRanipal_Eye.GetVerboseData(out verboseData);


                    // �ڂ̊J���(0�`1�ŕ]��)
                    eyeOpenLeft = eyeData.verbose_data.left.eye_openness;
                    eyeOpenRight = eyeData.verbose_data.right.eye_openness;
                    eyeOpenCombined = eyeData.verbose_data.combined.eye_data.eye_openness; //�Ȃɂ���H���0

                    // �����̕����x�N�g��(�e�ϐ�-1�`1�j�@��SystemOrigin�����_�ɂ��Ă�悤�ȋC������
                    //�E����W�n�Ȃ̂�x�������t�Ȃ̂ɒ���
                    //�E�E�������ꂼ��SystemOrigin����̕����x�N�g���𐄒肵�Ă�H
                    gaze_direction_left = eyeData.verbose_data.left.gaze_direction_normalized;
                    gaze_direction_right = eyeData.verbose_data.right.gaze_direction_normalized;
                    gaze_direction_combined = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;

                    // �����̎n�_���W(�p���̒��S�jSystemOrigin��(0,0,0)�E����W�n 
                    gaze_origin_left = eyeData.verbose_data.left.gaze_origin_mm;
                    gaze_origin_right = eyeData.verbose_data.right.gaze_origin_mm;
                    gaze_origin_combined = eyeData.verbose_data.combined.eye_data.gaze_origin_mm; //��

                    // ���E�a�i���a�j
                    pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;
                    pupilDiameterRight = eyeData.verbose_data.right.pupil_diameter_mm;
                    pupilDiameterCombined = eyeData.verbose_data.combined.eye_data.pupil_diameter_mm; //�Ȃɂ���H���0

                    // ���E�̈ʒu���W(0�`1�Ő��K���ς݁j
                    pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
                    pupilPositionRight = eyeData.verbose_data.right.pupil_position_in_sensor_area;
                    pupilPositionCombined = eyeData.verbose_data.combined.eye_data.pupil_position_in_sensor_area; //�Ȃɂ���H���0

                    //�t�s�����H �܂���������ĂȂ��炵���c
                    convergence_distance = eyeData.verbose_data.combined.convergence_distance_mm;

                    //Focus//�������Ă�ꏊ�ɃG�t�F�N�g�Ƃ��悳��
                    SRanipal_Eye.Focus(GazeIndex.COMBINE, out ray, out focusInfo, radius, maxradius);
                    SRanipal_Eye.Focus(GazeIndex.COMBINE, out ray, out focusInfo, maxradius);
                    SRanipal_Eye.Focus(GazeIndex.COMBINE, out ray, out focusInfo);

                    //GetEyeOpenness
                    SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out LeftOpenness);

                    //GetGazeRay
                    SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out origin, out direction);
                    SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out ray);

                    //GetPupilPosition
                    SRanipal_Eye.GetPupilPosition(EyeIndex.LEFT, out pupilPositionLeft);

                    //

                    //Debug.Log("c" + gaze_direction_combined);
                    //Debug.Log("R" + gaze_direction_right);
                    //Debug.Log("L" + gaze_direction_left);

                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {

                    }

                }
            }

        }
    }
}