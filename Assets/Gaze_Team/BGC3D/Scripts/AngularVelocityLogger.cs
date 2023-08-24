using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityLogger : MonoBehaviour
{
    // ���炩���ߌv�Z�X�N���v�g���A�^�b�`���Ă���
    [SerializeField] private AngularVelocityCalculator _calculator;

    private void LateUpdate()
    {
        // ��]�����ǂ���
        if (_calculator.IsRotating)
        {
            // ��]���Ă���ꍇ�́A�p���x�Ɖ�]�����o��
            print($"�p���x = {_calculator.AngularVelocity}, ��]�� = {_calculator.Axis}");
        }
        else
        {
            // ��]���Ă��Ȃ��ꍇ
            print("��]���Ă��Ȃ�");
        }
    }
}
