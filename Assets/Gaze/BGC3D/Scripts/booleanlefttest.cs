using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;

public class booleanlefttest : MonoBehaviour
{

    //InteractUI�{�^����������Ă�̂��𔻒肷�邽�߂�Iui�Ƃ����֐���SteamVR_Actions.default_InteractUI���Œ�
    private SteamVR_Action_Boolean Iui = SteamVR_Actions.default_InteractUI;
    //���ʂ̊i�[�pBoolean�^�֐�interacrtui
    private Boolean interacrtui;

    //1�t���[�����ɌĂяo�����Update���]�b�g
    void Update()
    {
        //���ʂ�GetState�Ŏ擾����interacrtui�Ɋi�[
        //SteamVR_Input_Sources.�@�햼�i����͍��R���g���[���j
        interacrtui = Iui.GetState(SteamVR_Input_Sources.RightHand);
        //interacrtui�̒��g���m�F
        //Debug.Log(interacrtui);
    }
}
