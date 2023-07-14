using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target_posi_set : MonoBehaviour
{
    public GameObject Server;
    public GameObject Camera;
    public GameObject[] target_set;
    private receiver script;
    // Start is called before the first frame update
    void Start()
    {
        script = Server.GetComponent<receiver>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
