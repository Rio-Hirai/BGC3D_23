using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAverageFilter : MonoBehaviour
{
    private int windowSize = 10; // �ړ����ς̃E�B���h�E�T�C�Y
    private Queue<Vector3> values = new Queue<Vector3>(); // �E�B���h�E���̒l��ێ�
    private Vector3 sum = Vector3.zero; // �E�B���h�E���̒l�̍��v
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
    //    // �E�B���h�E�T�C�Y�𒴂����ꍇ�A�ł��Â��l����菜��
    //    if (values.Count >= windowSize)
    //    {
    //        sum -= values.Dequeue();
    //    }

    //    // �V�����l��ǉ�
    //    values.Enqueue(newValue);
    //    sum += newValue;

    //    return sum / values.Count; // ���ϒl���v�Z���ĕԂ�
    //}

    void Update()
    {
        //Vector3 res = filter(new Vector3(i++, i++, i++), 3);
    }
}
