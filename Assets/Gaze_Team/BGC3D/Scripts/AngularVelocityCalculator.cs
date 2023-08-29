using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngularVelocityCalculator : MonoBehaviour
{
    // ��]�����Ă��邩�ǂ���
    public bool IsRotating { get; private set; }

    // �p���x[deg/s]
    public float AngularVelocity { get; private set; }

    // ��]��
    public Vector3 Axis { get; private set; }

    // �O�t���[���̎p��
    private Quaternion _prevRotation;

    [SerializeField]
    private receiver Server;

    private void Start()
    {
        _prevRotation = transform.rotation;
    }

    private void Update()
    {
        if (Server.approve_switch)
        {
            // ���݃t���[���̎p�����擾
            var rotation = transform.rotation;

            // �O�t���[������̉�]�ʂ����߂�
            var diffRotation = Quaternion.Inverse(_prevRotation) * rotation;
            // ��]�����p�x�Ǝ��i���[�J����ԁj�����߂�
            diffRotation.ToAngleAxis(out var angle, out var axis);

            // ��]�p�x��0�ȊO�Ȃ��]���Ă���Ƃ݂Ȃ�
            IsRotating = !Mathf.Approximately(angle, 0);

            // ��]�p�x����p���x���v�Z
            AngularVelocity = angle / Time.deltaTime;
            // ���[�J����Ԃ̉�]�������[���h��Ԃɕϊ�
            Axis = rotation * axis;

            if (AngularVelocity > 7.0f)
            {
                Server.head_rot_switch = true;
            }
            else
            {
                Server.head_rot_switch = false;
            }

            // �O�t���[���̎p�����X�V
            _prevRotation = rotation;
        }
    }
}