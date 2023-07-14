using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class LookAt : MonoBehaviour
            {
                private Ray ray;
                public int LengthOfRay = 25;

                public Image Eye_Image;

                public Canvas canvas;

                public RectTransform canvasRect;


                private void Start()
                {
                    canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

                    canvasRect = canvas.GetComponent<RectTransform>();

                    Eye_Image = GameObject.Find("EyeImage").GetComponent<Image>();

                }


                private void Update()
                {
                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal);

                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);

                    RaycastHit hit;

                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay, out hit))
                    {
                        Eye_Image.GetComponent<RectTransform>().anchoredPosition = hit.point;

                        string objectName = hit.collider.gameObject.name;
                        Debug.Log(objectName + ":" + hit.point.ToString("F2"));
                    }

                }
            }

        }
    }
}