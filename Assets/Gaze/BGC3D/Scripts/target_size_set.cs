using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_size_set : MonoBehaviour
{
    public GameObject Server;
    private receiver script;
    //private int flag = 0;
    public float dtime;
    public float Id;
    //public GameObject targetObj;
    // Start is called before the first frame update
    void Start()
    {
        script = Server.GetComponent<receiver>();
        this.transform.localScale = new Vector3(script.target_size, script.target_size, script.target_size);
        this.name = "target_" + script.target_id;
        script.target_id++;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider collider)
    {
    //    flag = 1;
    //    if (script.select_flag_gaze == 0)
    //    {
    //        script.select_flag_gaze = 1;
    //    }
        //Debug.Log("OK");
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider collider)
    {
    }
}
