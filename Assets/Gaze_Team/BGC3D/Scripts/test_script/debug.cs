using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
    public GameObject Server;
    private receiver script;

    void Start()
    {
        script = Server.GetComponent<receiver>();
    }
    // Update is called once per frame
    void Update()
    {
    //    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.A))
    //    {
    //        //script.pash_in = 2;
    //        //script.goal_in = 1;
    //    }
    }
}
