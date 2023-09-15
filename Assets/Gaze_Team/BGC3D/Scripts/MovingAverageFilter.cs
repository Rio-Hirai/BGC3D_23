using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAverageFilter : MonoBehaviour
{
    private int windowSize = 15;                        // 移動平均のウィンドウサイズ
    private int windowMaxium = 16;                      // 最大ウィンドウサイズ
    private Vector3 sum = Vector3.zero;                 // ウィンドウ内の値の合計
    private Vector3[] stock_values = new Vector3[49];   // 移動平均フィルタ用の座標を格納するための変数
    private int index = 0;                              // stock_valuesの参照位置

    public Vector3 filter(Vector3 newvalue, int NowWindowSize) // 引数は，新しい値newvalueとウィンドウサイズNowWindowSize
    {
        sum = Vector3.zero; // 合計値を初期化

        windowSize = NowWindowSize; // ウィンドウサイズを更新
        if (windowSize > windowMaxium) windowSize = windowMaxium; // ウィンドウサイズが上限サイズを超えている場合は上限サイズに更新
        if (windowSize < 1) windowSize = 1; // ウィンドウサイズが1未満の場合は1に更新

        if (stock_values.Length < index + 1) index = 0; // 参照位置がオーバーフローした場合は0に更新

        stock_values[index] = newvalue; // 参照位置に新しい値を追加

        int index2 = index; // 参照位置をコピー

        index++; // 参照位置を次にずらす


        // ウィンドウサイズ内の値を合算する処理-----------------------------
        for (int i = 0; i < windowSize * 3; i++)
        {
            if (index2 < 0) index2 = stock_values.Length - 1; // 参照位置がオーバーフローした場合は配列最後尾に更新
            sum += stock_values[index2]; // 参照位置の値を加算
            index2--; // 参照位置を前にずらす
        }
        //--------------------------------------------------------------


        // 重み付け処理--------------------------------------------------
        int newvalue_gain = windowSize / 3; // ウィンドウサイズに合わせてゲインを追加
        sum += newvalue * newvalue_gain; // ゲインをかける
        //--------------------------------------------------------------

        return sum /= (windowSize + newvalue_gain); // フィルタをかけた値を返す
    }

    //public void set_value(Vector3 newvalue)
    //{
    //    if (stock_values.Length < index + 1)
    //    {
    //        index = 0;
    //    }

    //    stock_values[index++] = newvalue;
    //}

    //public Vector3 get_value(int NowWindowSize)
    //{
    //    sum = Vector3.zero;

    //    windowSize = NowWindowSize;
    //    if (windowSize > 7) windowSize = 7;
    //    if (windowSize < 1) windowSize = 1;

    //    int index2 = index;

    //    for (int i = 0; i < windowSize * windowSize; i++)
    //    {
    //        if (index2 < 0)
    //        {
    //            index2 = stock_values.Length - 1;
    //        }
    //        sum += stock_values[index2--];
    //    }

    //    return sum /= windowSize;
    //}
}
