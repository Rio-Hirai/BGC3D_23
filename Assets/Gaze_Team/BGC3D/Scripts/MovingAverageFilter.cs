using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAverageFilter : MonoBehaviour
{
    public int windowSize = 10; // �ړ����ς̃E�B���h�E�T�C�Y
    public Queue<Vector3> values = new Queue<Vector3>(); // �E�B���h�E���̒l��ێ�
    public Vector3 sum = Vector3.zero; // �E�B���h�E���̒l�̍��v
    public Vector3[] stock_values = new Vector3[25];
    public int index = 0;
    private int i = 0;

    public void value_stock(Vector3 newvalue)
    {
        sum = new Vector3(0, 0, 0);

        if (stock_values.Length < index + 1)
        {
            index = 0;
        }

        stock_values[index++] = newvalue;

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

        sum /= windowSize;
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
        value_stock(new Vector3(i++, i++, i++));
    }
}
