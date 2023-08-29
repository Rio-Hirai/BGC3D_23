using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class approve_test : MonoBehaviour
{
    [SerializeField] private Transform head;// ���[�U�̓����ʒu
    [SerializeField] private receiver Server;

    private float lastRotationX;                // �O���x���̉�]
    private float timer;                        // �^�C�}�[
    private float rotationThreshold = 9.0f;      // ��]臒l�i�x�j
    private float timeThreshold_min = 0.20f;    // ����臒l�i�b�j
    private float timeThreshold_max = 0.40f;    // ����臒l�i�b�j


    // ���ʉ�-------------------------------------------------------
    public AudioSource audioSource;         // �����ݒ�
    public AudioClip sound_OK;              // �w���ʂ�̃^�[�Q�b�g��I���ł������̉�
    //--------------------------------------------------------------

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
        if (timer >= timeThreshold_min)
        {
            float currentRotationX = head.rotation.eulerAngles.x; // ���݂�x���̉�]���擾
            float rotationChange = Mathf.Abs(Mathf.Abs(currentRotationX) - Mathf.Abs(lastRotationX)); // �O��̉�]����̕ω����v�Z
            if (rotationChange > 180) rotationChange -= 360; // �K�v�ɉ�����360�x���炷�i360�x�ȏ�̉�]������邽�߁j


            // ��]��臒l�𒴂��Ă���ꍇ�A���b�Z�[�W���o��-----------------
            if ((Mathf.Abs(rotationChange) > rotationThreshold) && (lastRotationX < currentRotationX))
            {
                // �O��̉�]�Ǝ��Ԃ��X�V---------------------------------------
                lastRotationX = currentRotationX;
                timer = 0f;
                //--------------------------------------------------------------


                audioSource.PlayOneShot(sound_OK); // �����������̌��ʉ���炷
                Server.select_flag = true;
            }
            else if (timer >= timeThreshold_max)
            {
                // �O��̉�]�Ǝ��Ԃ��X�V---------------------------------------
                lastRotationX = currentRotationX;
                timer = 0f;
                //--------------------------------------------------------------
            }
            //--------------------------------------------------------------
        }
    }
}
