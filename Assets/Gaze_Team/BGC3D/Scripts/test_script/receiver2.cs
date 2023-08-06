using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Valve.VR.Extras;

public class receiver2 : MonoBehaviour
{
    public float target_size;
    public float target_distance;
    public float point_size;
    public int target_amount;
    public GameObject target_objects;
    //public List<int> target_ids = new List<int>();
    //public int target_id;

    // 誘目対策
    public float pointvalue;
    //public float pointvalue2;

    // 注視時間
    public float set_dtime = 0.6f;

    // 色設定
    public Color target_color;
    public Color cursor_color;

    // 各種機能切り替え
    public bool laserswitch;
    public bool lens_switch;
    public bool cursor_switch;
    public bool target_alpha_switch;

    //レーザー
    public GameObject Rlaser;
    public GameObject Llaser;

    // クローンID
    public float target_id;
    public GameObject target_clone;

    public int select_flag;
    public int select_cnt;
    public int select_cnt_tmp;
    public byte color_alpha;
    public float cursor_radious;

    public float LeftPupiltDiameter;
    public float RightPupiltDiameter;
    public int LeftPupiltDiameter_flag;
    public int RightPupiltDiameter_flag;

    //public GameObject eyePoint;
    public int select_flag_2;
    public Vector3 old_eye_position;
    public Vector3 new_eye_position;
    public int select_flag_gaze;
    public int select_flag_other;
    public GameObject head_obj;

    // Bubble Gaze Lens用
    public GameObject Lens_Object;
    public bool lens_flag;

    void Start()
    {
        target_id = 0;
        // ターゲット作成
        for (int i = 0; i < target_amount; i++)
        {
            float target_x = 0.0f;
            float target_y = 0.0f;
            float target_z = 0.0f;
            while (!(target_x > target_distance || target_x < -target_distance))
            {
                target_x = Random.Range(-(target_distance + 1.0f), target_distance + 1.0f);
            }
            while (!(target_z > target_distance || target_z < -target_distance))
            {
                target_z = Random.Range(-(target_distance + 1.0f), target_distance + 1.0f);
            }
            //target_x = Random.Range(-1.5f, 1.5f);
            target_y = Random.Range(-1.0f, 2.2f);
            //target_z = Random.Range(-1.5f, 1.5f);
            Instantiate(target_objects, new Vector3(target_x, target_y, target_z), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (laserswitch == false)
        //{
        //    Rlaser.GetComponent<SteamVR_LaserPointer>().enabled = false;
        //    Llaser.GetComponent<SteamVR_LaserPointer>().enabled = false;
        //}
        //else
        //{
        //    Rlaser.GetComponent<SteamVR_LaserPointer>().enabled = true;
        //    Llaser.GetComponent<SteamVR_LaserPointer>().enabled = true;
        //}
        if (lens_switch)
        {
            if (lens_flag)
            {
                Lens_Object.SetActive(true);
            }
            else
            {
                Lens_Object.SetActive(false);
            }
        }
    }
}