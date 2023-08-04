using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAverageFilter : MonoBehaviour
{
    private int windowSize = 10; // 移動平均のウィンドウサイズ
    private Vector3 sum = Vector3.zero; // ウィンドウ内の値の合計
    private Vector3[] stock_values = new Vector3[25];
    private int index = 0;

    public Vector3 filter(Vector3 newvalue, int NowWindowSize)
    {
        sum = Vector3.zero;

        windowSize = NowWindowSize;
        if (windowSize > 24) windowSize = 24;
        if (windowSize < 1) windowSize = 1;

        if (stock_values.Length < index + 1) index = 0;

        stock_values[index] = newvalue;

        int index2 = index;

        for (int i = 0; i < windowSize * 2; i++)
        {
            if (index2 < 0) index2 = stock_values.Length - 1;
            sum += stock_values[index2];
            index2--;
        }

        index++;

        int newvalue_gain = windowSize / 3;
        sum += newvalue * newvalue_gain;
        return sum /= (windowSize + newvalue_gain);
    }

    public void set_value(Vector3 newvalue)
    {
        if (stock_values.Length < index + 1)
        {
            index = 0;
        }

        stock_values[index++] = newvalue;
    }

    public Vector3 get_value(int NowWindowSize)
    {
        sum = Vector3.zero;

        windowSize = NowWindowSize;
        if (windowSize > 7) windowSize = 7;
        if (windowSize < 1) windowSize = 1;

        int index2 = index;

        for (int i = 0; i < windowSize * windowSize; i++)
        {
            if (index2 < 0)
            {
                index2 = stock_values.Length - 1;
            }
            sum += stock_values[index2--];
        }

        return sum /= windowSize;
    }
}
