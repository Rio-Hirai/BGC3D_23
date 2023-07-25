using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSensor : MonoBehaviour
{
    public Camera dispCamera;
    private Texture2D targetTexture;

    public float lightValue;

    // Use this for initialization
    IEnumerator Start()
    {
        var tex = dispCamera.targetTexture;
        targetTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);

        while (true)
        {
            // RenderTextureキャプチャ
            RenderTexture.active = dispCamera.targetTexture;

            yield return new WaitForEndOfFrame();

            targetTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            targetTexture.Apply();


            // 照度を取得する
            lightValue = GetLightValue(targetTexture);

            // 照度を表示する
            //Debug.Log(lightValue);
        }
    }

    // 画像全体の照度計算
    float GetLightValue(Texture2D tex)
    {
        var cols = tex.GetPixels();

        // 平均色計算
        Color avg = new Color(0, 0, 0);
        foreach (var col in cols)
        {
            avg += col;
        }
        avg /= cols.Length;

        // 照度計算
        return 0.299f * avg.r + 0.587f * avg.g + 0.114f * avg.b;
    }
}