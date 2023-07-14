using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rev_sub : MonoBehaviour
{
    public float target_size;
    public float point_size;
    public int target_amount;
    public GameObject target_objects;
    public List<int> target_ids = new List<int>();
    public int target_id;
    public int select_flag;
    public int select_cnt;
    public int select_cnt_tmp;
    public GameObject target_object;
    public GameObject lens_object;
    public byte color_alpha;
    public float cursor_radious;

    public float LeftPupiltDiameter;
    public float RightPupiltDiameter;
    public int LeftPupiltDiameter_flag;
    public int RightPupiltDiameter_flag;

    public GameObject eyePoint;
    public int select_flag_2;
    private Vector3 old_position;
    public Vector3 old_eye_position;
    public Vector3 new_eye_position;
    public float pointvalue;

    public int point_OKNG;

    public int select_flag_gaze;
    public int select_flag_other;

    // íçéãéûä‘
    public float set_dtime = 0.6f;

    // êFê›íË
    public Color target_color;
    public Color cursor_color;

    void Start()
    {
        // É^Å[ÉQÉbÉgçÏê¨
        for (int i = 0; i < target_amount; i++)
        {
            float target_x = 0.0f;
            float target_y = 0.0f;
            float target_z = 0.0f;
            while (!(target_x > 0.5f || target_x < -0.5f))
            {
                target_x = Random.Range(-1.7f, 1.7f);
            }
            while (!(target_z > 0.5f || target_z < -0.5f))
            {
                target_z = Random.Range(-1.7f, 1.7f);
            }
            //target_x = Random.Range(-1.5f, 1.5f);
            target_y = Random.Range(-0.5f, 1.5f);
            //target_z = Random.Range(-1.5f, 1.5f);
            Instantiate(target_objects, new Vector3(target_x, target_y, target_z), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //IDêÿÇËë÷Ç¶
        if (target_ids.Count > 0)
        {
            target_id = target_ids[select_cnt];
            //lens_object.SetActive(true);
        }
        else
        {
            target_id = 0;
            //lens_object.SetActive(false);
        }

        // óUñ⁄èCê≥
        //if (Mathf.Abs(new_eye_position.magnitude - old_eye_position.magnitude) >= pointvalue)
        //{
        //    select_flag_2 = 1;
        //    point_OKNG = 1;
        //} else
        //{
        //    point_OKNG = 0;
        //}
        //Debug.Log(new_eye_position.magnitude - old_eye_position.magnitude + "," + new_eye_position.magnitude + "," + old_eye_position.magnitude);
        //old_position = eyePoint.transform.position;
    }
}
