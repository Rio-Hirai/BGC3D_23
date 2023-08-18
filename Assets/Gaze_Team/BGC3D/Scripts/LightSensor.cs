using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

public class LightSensor : MonoBehaviour
{
    [SerializeField] private Camera dispCamera;
    private Texture2D targetTexture;

    [SerializeField] private receiver server; // サーバと接続
    [System.NonSerialized] public float lightValue; // 画面の明度を格納するための変数


    IEnumerator Start()
    {
        var tex = dispCamera.targetTexture;
        targetTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);

        while (server.LightSensor_switch)
        {
            RenderTexture.active = dispCamera.targetTexture;// RenderTextureキャプチャ

            yield return new WaitForEndOfFrame();

            targetTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0); // カメラのピクセル情報を取得
            targetTexture.Apply();

            lightValue = GetLightValue(targetTexture); // 明度を取得

            //Debug.Log(lightValue); // 明度を表示する
        }
    }

    // 画像全体の明度計算
    public float GetLightValue(Texture2D tex)
    {
        var cols = tex.GetPixels(); // 画面全体のピクセル情報を取得

        // 平均色計算---------------------------------------------------
        Color avg = new Color(0, 0, 0);
        foreach (var col in cols)
        {
            avg += col;
        }
        avg /= cols.Length;
        //--------------------------------------------------------------

        return 0.299f * avg.r + 0.587f * avg.g + 0.114f * avg.b; // 明度を計算して返す
    }
}