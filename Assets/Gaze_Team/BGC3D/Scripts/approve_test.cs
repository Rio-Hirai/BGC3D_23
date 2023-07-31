using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class approve_test : MonoBehaviour
{
    public Transform head;

    private float lastRotationX; // �O���x���̉�]
    private float timer; // �^�C�}�[
    private float rotationThreshold = 10f; // ��]臒l�i�x�j
    private float timeThreshold = 0.3f; // ����臒l�i�b�j

    void Start()
    {
        // ������
        lastRotationX = head.rotation.eulerAngles.x;
        timer = 0;
    }

    void Update()
    {
        // ���Ԃ��X�V
        timer += Time.deltaTime;

        // �w�肵�����Ԃ��o�߂�����Ax���̉�]���`�F�b�N
        if (timer >= timeThreshold)
        {
            // ���݂�x���̉�]���擾
            float currentRotationX = head.rotation.eulerAngles.x;

            // �O��̉�]����̕ω����v�Z
            float rotationChange = Mathf.Abs(currentRotationX - lastRotationX);

            // �K�v�ɉ�����360�x���炷�i360�x�ȏ�̉�]������邽�߁j
            if (rotationChange > 180) rotationChange -= 360;

            // ��]��臒l�𒴂��Ă���ꍇ�A���b�Z�[�W���o��
            if (Mathf.Abs(rotationChange) > rotationThreshold)
            {
                Debug.Log("The x rotation changed by more than " + rotationThreshold + " degrees in " + timeThreshold + " seconds.");
            }

            // �O��̉�]�Ǝ��Ԃ��X�V
            lastRotationX = currentRotationX;
            timer = 0;
        }
    }
}
