using System.Collections;
using System.Collections.Generic;
using Uduino;
using UnityEngine;

public class Haptic_Feedback : MonoBehaviour
{
    [SerializeField] Server_Haptic server;
    UduinoManager ud; // The instance of Uduino is initialized here

    [Range(0, 255)] public int Max_Power;
    [Range(0, 255)] public int Min_Power;

    [Range(0, 255)] public int blinkpower_11 = 100;
    [Range(0, 255)] public int blinkpower_10 = 100;
    [Range(0, 255)] public int blinkpower_9 = 100;
    [Range(0, 255)] public int blinkpower_6 = 100;
    // Start is called before the first frame update
    void Start()
    {
        UduinoManager.Instance.pinMode(11, PinMode.PWM);
        UduinoManager.Instance.pinMode(10, PinMode.PWM);
        UduinoManager.Instance.pinMode(9, PinMode.PWM);
        UduinoManager.Instance.pinMode(6, PinMode.PWM);
    }

    // Update is called once per frame
    void Update()
    {
        // test_haptic();

        if (server.Haptic_Feedback)
        {
            blinkpower_11 = (int)server.Moved_Power;
            Phantom();
        }
        else
        {
            blinkpower_11 = Min_Power;
        }

        //UduinoManager.Instance.analogWrite(11, blinkpower_11);
    }

    private void test_haptic()
    {
        UduinoManager.Instance.analogWrite(11, blinkpower_11);
        UduinoManager.Instance.analogWrite(10, blinkpower_10);
        UduinoManager.Instance.analogWrite(9, blinkpower_9);
        UduinoManager.Instance.analogWrite(6, blinkpower_6);
    }

    private void Phantom()
    {
        int front = (int)server.Moved_Power;
        int back = 255 - (int)server.Moved_Power + Min_Power;

        if (back < Min_Power)
        {
            back = Min_Power;
        }

        UduinoManager.Instance.analogWrite(11, back);
        UduinoManager.Instance.analogWrite(10, front);
        UduinoManager.Instance.analogWrite(9, front);
        UduinoManager.Instance.analogWrite(6, back);
    }
}
