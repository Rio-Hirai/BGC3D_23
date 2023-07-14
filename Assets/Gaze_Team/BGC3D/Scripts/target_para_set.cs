using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class target_para_set : MonoBehaviour
{
    public GameObject Server;
    private receiver script;
    private results script2;
    public float dtime;
    public int Id;

    void Start()
    {
        script = Server.GetComponent<receiver>();
        script2 = Server.GetComponent<results>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dtime > 0 && script.cd_flag)
        {
            if (script.DwellTarget.GetComponent<target_para_set>().Id != Id)
            {
                dtime -= (script.DeltaTime / 3);
            }
        }
        else if (dtime < 0)
        {
            dtime = 0;
        }

        if (Server.GetComponent<receiver>().enabled == false)
        {
            // åãâ ï\é¶ópÇÃèàóù
            for (int i = 1; i < script2.csvDatas.Count; i++)
            {
                if (Id == int.Parse(script2.csvDatas[i][2]))
                {
                    this.GetComponent<Renderer>().material.color = Color.blue;
                }
            }
            
            return;
        }

        if (script.controller_switch)
        {
            if (script.output_flag)
            {
                this.GetComponent<Renderer>().material.color = script.target_color;
            }
            else if (Id == script.tasknums[script.task_num] && script.taskflag)
            {
                this.GetComponent<Renderer>().material.color = Color.blue;
            }
            else if (Id == 999)
            {
                this.GetComponent<Renderer>().material.color = Color.black;
            } else if (script.DwellTarget != null)
            {
                if (script.DwellTarget.GetComponent<target_para_set>().Id == Id)
                {
                    this.GetComponent<Renderer>().material.color = script.select_color;
                }
            }
            else
            {
                this.GetComponent<Renderer>().material.color = Color.white;
            }
        } else
        {
            if (script.output_flag)
            {
                this.GetComponent<Renderer>().material.color = script.target_color;
            }
            else if (Id == script.select_target_id)
            {
                this.GetComponent<Renderer>().material.color = script.target_color;
            }
            else if (script.DwellTarget == null)
            {
                if (script.taskflag == false && Id != 999)
                {
                    this.GetComponent<Renderer>().material.color = Color.white;
                }
                else if (Id == script.select_target_id)
                {
                    this.GetComponent<Renderer>().material.color = script.target_color;
                }
                else if (Id == script.tasknums[script.task_num])
                {
                    this.GetComponent<Renderer>().material.color = Color.blue;
                }
                else if (Id == 999)
                {
                    this.GetComponent<Renderer>().material.color = Color.black;
                }
                else
                {
                    this.GetComponent<Renderer>().material.color = Color.white;
                }
            }
            else if (script.DwellTarget.GetComponent<target_para_set>().Id == Id)
            {
                this.GetComponent<Renderer>().material.color = script.select_color;
            }
            else if (script.taskflag == false && Id != 999)
            {
                this.GetComponent<Renderer>().material.color = Color.white;
            }
            else if (Id == script.select_target_id)
            {
                this.GetComponent<Renderer>().material.color = script.target_color;
            }
            else if (Id == script.tasknums[script.task_num])
            {
                this.GetComponent<Renderer>().material.color = Color.blue;
            }
            else if (Id == 999)
            {
                this.GetComponent<Renderer>().material.color = Color.black;
            }
            else
            {
                this.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider collider)
    {
    }
}
