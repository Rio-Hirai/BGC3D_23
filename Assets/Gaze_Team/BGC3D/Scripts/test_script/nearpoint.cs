using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nearpoint : MonoBehaviour
{
    [SerializeField]
    private string tagName = "Enemy";        // インスペクターで変更可能

    public GameObject searchNearObj;         // 最も近いオブジェクト(public修飾子にすることで外部のクラスから参照できる)
    public GameObject oldNearObj;
    private float searchWaitTime = 0.02f;     // 検索の待機時間

    private float timer = 0f;                // 検索までの待機時間計測用

    public Vector3 targetpoint;

    public Vector3 oldcursorpoint;

    public GameObject Server;
    private receiver script;
    // Start is called before the first frame update
    void Start()
    {
        // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
        script = Server.GetComponent<receiver>();
        searchNearObj = Serch();
        Debug.Log(searchNearObj);
        Debug.Log(script.cursor_radious);
    }

    // Update is called once per frame
    void Update()
    {
        // 時間の経過に合わせて自動的に取得する場合

        // 時間を計測
        timer += Time.deltaTime;

        // 検索の待機時間を経過したら
        if (timer >= searchWaitTime)
        {

            // 指定したタグを持つゲームオブジェクトのうち、このゲームオブジェクトに最も近いゲームオブジェクト１つを取得
            searchNearObj = Serch();
            Debug.Log(searchNearObj);

            // 計測時間を初期化して、再検索
            searchWaitTime = 0;
        }


        this.transform.localScale = new Vector3(script.cursor_radious, script.cursor_radious, script.cursor_radious);
    }

    /// <summary>
    /// 指定されたタグの中で最も近いものを取得
    /// </summary>
    /// <returns></returns>
    private GameObject Serch()
    {
        // 最も近いオブジェクトの距離を代入するための変数
        float nearDistance = 0;
        script.cursor_radious = nearDistance;

        // 検索された最も近いゲームオブジェクトを代入するための変数
        GameObject searchTargetObj = null;

        // tagNameで指定されたTagを持つ、すべてのゲームオブジェクトを配列に取得
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

        // 取得したゲームオブジェクトが 0 ならnullを戻す(使用する場合にはnullでもエラーにならない処理にしておくこと)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objsから１つずつobj変数に取り出す
        foreach (GameObject obj in objs)
        {
            obj.GetComponent<Renderer>().material.color = Color.white;
            // objに取り出したゲームオブジェクトと、このゲームオブジェクトとの距離を計算して取得
            float distance = Vector3.Distance(obj.transform.position, transform.position);

            // nearDistanceが0(最初はこちら)、あるいはnearDistanceがdistanceよりも大きい値なら
            if (nearDistance == 0 || nearDistance > distance)
            {

                // nearDistanceを更新
                nearDistance = distance;

                // searchTargetObjを更新
                searchTargetObj = obj;
            }
        }

        //最も近かったオブジェクトを返す
        //searchTargetObj.GetComponent<Renderer>().material.color = Color.green;

        if (oldNearObj != searchTargetObj)
        {
            searchTargetObj.GetComponent<target_size_set>().dtime = 0;
        }
        searchTargetObj.GetComponent<target_size_set>().dtime += Time.deltaTime;
        if (searchTargetObj.GetComponent<target_size_set>().dtime >= script.set_dtime)
        {
            searchTargetObj.GetComponent<Renderer>().material.color = Color.green;
        }
        oldNearObj = searchTargetObj;

        targetpoint = searchTargetObj.transform.position;
        //script.cursor_radious = nearDistance*2 + script.target_size;
        //Debug.Log("dis1 = " + nearDistance + ", pos = " + this.transform.position);

        if (nearDistance < 1.0f )
        {
            //script.cursor_radious = nearDistance * 2 + script.target_size;
            script.cursor_radious = nearDistance;
        } else
        {
            script.cursor_radious = 1.0f;
        }

        //Debug.Log("dis2 = " + nearDistance + ", pos = " + this.transform.position);
        Debug.Log("dis3 = " + (this.transform.position.magnitude));
        return searchTargetObj;
    }
}
