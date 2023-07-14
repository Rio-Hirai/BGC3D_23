using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class red_result : MonoBehaviour
{
    public GameObject Server;
    private receiver script;

    public int result_para = 0;
    // Start is called before the first frame update
    void Start()
    {
        script = Server.GetComponent<receiver>();
    }

    // Update is called once per frame
    void Update()
    {

        this.GetComponent<Renderer>().material.color = new Color(255/255, (255 - (255 / script.tester_id * result_para)) / 255, (255 - (255 / script.tester_id * result_para)) / 255);
        float gre = (255 - (255 / script.tester_id * result_para)) / 255;
        UnityEngine.Debug.Log(gre);
    }
}
