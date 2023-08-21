using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class approve_test : MonoBehaviour
{
    [SerializeField] private Transform head;// ���[�U�̓����ʒu

    private float lastRotationX;            // �O���x���̉�]
    private float timer;                    // �^�C�}�[
    private float rotationThreshold = 10f;  // ��]臒l�i�x�j
    private float timeThreshold = 0.3f;     // ����臒l�i�b�j

    void Start()
    {
        // ������-------------------------------------------------------
        lastRotationX = head.rotation.eulerAngles.x;
        timer = 0;
        //--------------------------------------------------------------
    }

    void Update()
    {
        timer += Time.deltaTime; // ���Ԃ��X�V

        // �w�肵�����Ԃ��o�߂�����Ax���̉�]���`�F�b�N
        if (timer >= timeThreshold)
        {
            float currentRotationX = head.rotation.eulerAngles.x; // ���݂�x���̉�]���擾

            float rotationChange = Mathf.Abs(currentRotationX - lastRotationX); // �O��̉�]����̕ω����v�Z

            if (rotationChange > 180) rotationChange -= 360; // �K�v�ɉ�����360�x���炷�i360�x�ȏ�̉�]������邽�߁j


            // ��]��臒l�𒴂��Ă���ꍇ�A���b�Z�[�W���o��-----------------
            if (Mathf.Abs(rotationChange) > rotationThreshold)
            {
                Debug.Log("The x rotation changed by more than " + rotationThreshold + " degrees in " + timeThreshold + " seconds.");
            }
            //--------------------------------------------------------------


            // �O��̉�]�Ǝ��Ԃ��X�V---------------------------------------
            lastRotationX = currentRotationX;
            timer = 0;
            //--------------------------------------------------------------
        }
    }
}
