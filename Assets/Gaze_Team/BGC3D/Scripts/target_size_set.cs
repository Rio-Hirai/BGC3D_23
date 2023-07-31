using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class target_size_set : MonoBehaviour
{
    public receiver Server;
    public float dtime;

    void Start()
    {
        this.transform.localScale = new Vector3(Server.target_size, Server.target_size, Server.target_size);
        this.name = "target_" + Server.target_id;
        this.GetComponent<target_para_set>().Id = Server.target_id;
        Server.target_id++;
    }
}
