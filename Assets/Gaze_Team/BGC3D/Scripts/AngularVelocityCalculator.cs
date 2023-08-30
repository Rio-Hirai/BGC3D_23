using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityCalculator : MonoBehaviour
{
    public bool IsRotating { get; private set; }        // ��]�����Ă��邩�ǂ���
    public float AngularVelocity { get; private set; }  // �p���x[deg/s]
    public Vector3 Axis { get; private set; }           // ��]��
    private Quaternion _prevRotation;                   // �O�t���[���̎p��
    [SerializeField] private receiver Server;           // �H�H�H

    private void Start()
    {
        _prevRotation = transform.rotation; // �H�H�H
    }

    private void Update()
    {
        if (Server.approve_switch) // �H�H�H
        {
            var rotation = transform.rotation; // ���݃t���[���̎p�����擾
            var diffRotation = Quaternion.Inverse(_prevRotation) * rotation; // �O�t���[������̉�]�ʂ����߂�
            diffRotation.ToAngleAxis(out var angle, out var axis); // ��]�����p�x�Ǝ��i���[�J����ԁj�����߂�

            IsRotating = !Mathf.Approximately(angle, 0); // ��]�p�x��0�ȊO�Ȃ��]���Ă���Ƃ݂Ȃ�
            AngularVelocity = angle / Time.deltaTime; // ��]�p�x����p���x���v�Z
            Axis = rotation * axis; // ���[�J����Ԃ̉�]�������[���h��Ԃɕϊ�


            //--------------------------------------------------------------
            if (AngularVelocity > 7.0f)
            {
                Server.head_rot_switch = true;
            }
            else
            {
                Server.head_rot_switch = false;
            }
            //--------------------------------------------------------------


            _prevRotation = rotation; // �O�t���[���̎p�����X�V
        }
    }
}