using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

public class LightSensor : MonoBehaviour
{
    [SerializeField] private Camera dispCamera;
    private Texture2D targetTexture;

    [SerializeField] private receiver server; // �T�[�o�Ɛڑ�
    [System.NonSerialized] public float lightValue; // ��ʂ̖��x���i�[���邽�߂̕ϐ�


    IEnumerator Start()
    {
        var tex = dispCamera.targetTexture;
        targetTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);

        while (server.LightSensor_switch)
        {
            RenderTexture.active = dispCamera.targetTexture;// RenderTexture�L���v�`��

            yield return new WaitForEndOfFrame();

            targetTexture.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0); // �J�����̃s�N�Z�������擾
            targetTexture.Apply();

            lightValue = GetLightValue(targetTexture); // ���x���擾

            //Debug.Log(lightValue); // ���x��\������
        }
    }

    // �摜�S�̖̂��x�v�Z
    public float GetLightValue(Texture2D tex)
    {
        var cols = tex.GetPixels(); // ��ʑS�̂̃s�N�Z�������擾

        // ���ϐF�v�Z---------------------------------------------------
        Color avg = new Color(0, 0, 0);
        foreach (var col in cols)
        {
            avg += col;
        }
        avg /= cols.Length;
        //--------------------------------------------------------------

        return 0.299f * avg.r + 0.587f * avg.g + 0.114f * avg.b; // ���x���v�Z���ĕԂ�
    }
}