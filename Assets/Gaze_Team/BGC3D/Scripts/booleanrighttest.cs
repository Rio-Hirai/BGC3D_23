using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;

public class booleanrighttest : MonoBehaviour
{

    //InteractUI�{�^���i�����ݒ�̓g���K�[�j��������Ă�̂��𔻒肷�邽�߂�Iui�Ƃ����֐���SteamVR_Actions.default_InteractUI���Œ�
    private SteamVR_Action_Boolean Iui = SteamVR_Actions.default_InteractUI;
    //���ʂ̊i�[�pBoolean�^�֐�interacrtui
    private Boolean interacrtui;

    //GrabGrip�{�^���i�����ݒ�͑��ʃ{�^���j��������Ă�̂��𔻒肷�邽�߂�Grab�Ƃ����֐���SteamVR_Actions.default_GrabGrip���Œ�
    private SteamVR_Action_Boolean GrabG = SteamVR_Actions.default_GrabGrip;
    //���ʂ̊i�[�pBoolean�^�֐�grapgrip
    private Boolean grapgrip;

    //1�t���[�����ɌĂяo�����Update���]�b�g
    void Update()
    {
        //���ʂ�GetState�Ŏ擾����interacrtui�Ɋi�[
        //SteamVR_Input_Sources.�@�햼�i����͉E�R���g���[���j
        interacrtui = Iui.GetState(SteamVR_Input_Sources.RightHand);
        //InteractUI��������Ă���Ƃ��ɃR���\�[����InteractUI�ƕ\��
        if (interacrtui)
        {
            Debug.Log("InteractUI");
        }

        //���ʂ�GetState�Ŏ擾����grapgrip�Ɋi�[
        //SteamVR_Input_Sources.�@�햼�i����͉E�R���g���[���j
        grapgrip = GrabG.GetState(SteamVR_Input_Sources.RightHand);
        //GrabGrip��������Ă���Ƃ��ɃR���\�[����GrabGrip�ƕ\��
        if (grapgrip)
        {
            Debug.Log("GrabGrip");
        }

    }
}
