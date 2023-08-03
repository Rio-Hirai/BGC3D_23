using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAverageFilter : MonoBehaviour
{
    private int windowSize = 10; // 移動平均のウィンドウサイズ
    private Queue<Vector3> values = new Queue<Vector3>(); // ウィンドウ内の値を保持
    private Vector3 sum = Vector3.zero; // ウィンドウ内の値の合計
    private Vector3[] stock_values = new Vector3[25];
    private int index = 0;
    private int i = 0;

    public Vector3 filter(Vector3 newvalue, int NowWindowSize)
    {
        sum = Vector3.zero;

        windowSize = NowWindowSize;
        if (windowSize > 5) windowSize = 5;

        if (stock_values.Length < index + 1)
        {
            index = 0;
        }

        stock_values[index] = newvalue;

        int index2 = index;

        for (int k = 0; k < windowSize; k++)
        {
            if (index2 < 0)
            {
                index2 = stock_values.Length - 1;
            }
            sum += stock_values[index2];
            index2--;
        }

        index++;
        return sum /= windowSize;
    }

    //public Vector3 Filter(Vector3 newValue)
    //{
    //    // ウィンドウサイズを超えた場合、最も古い値を取り除く
    //    if (values.Count >= windowSize)
    //    {
    //        sum -= values.Dequeue();
    //    }

    //    // 新しい値を追加
    //    values.Enqueue(newValue);
    //    sum += newValue;

    //    return sum / values.Count; // 平均値を計算して返す
    //}

    void Update()
    {
        //Vector3 res = filter(new Vector3(i++, i++, i++), 3);
    }
}
