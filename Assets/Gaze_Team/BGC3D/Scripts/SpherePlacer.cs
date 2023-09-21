using UnityEngine;

public class SpherePlacer : MonoBehaviour
{
    public GameObject[] objectsToDistribute; // 配置するオブジェクトの配列
    public Camera mainCamera; // カメラの参照

    void Start()
    {
        PlaceSpheres();
    }

    void PlaceSpheres()
    {
        float distance = 3.5f; // カメラからの距離
        float sphereSize = 0.1f; // 球体の大きさ
        float spacing = 0.12f; // 球体間の距離
        int rows = 7;
        int columns = 7;

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 centerPosition = mainCamera.transform.position + cameraForward * distance; // カメラの正面の位置

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = i * 7 + j;

                float offsetX = (j - columns / 2) * (sphereSize + spacing);
                float offsetY = (i - rows / 2) * (sphereSize + spacing);

                Vector3 position = centerPosition + mainCamera.transform.right * offsetX + mainCamera.transform.up * offsetY;

                objectsToDistribute[index].transform.position = position;
                objectsToDistribute[index].transform.localScale = Vector3.one * sphereSize;
            }
        }
    }
}
