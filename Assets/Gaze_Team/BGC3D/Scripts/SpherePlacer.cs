using UnityEngine;

public class SpherePlacer : MonoBehaviour
{
    public GameObject[] objectsToDistribute; // 配置するオブジェクトの配列
    public Camera mainCamera; // カメラの参照
    public Transform centerPoint; // 球体の中心点

    void Start()
    {
        PlaceSpheres();
    }

    void PlaceSpheres()
    {
        float distance = 3.5f; // カメラからの距離
        float sphereSize = 0.1f; // 球体の大きさ
        float spacing = 0.24f; // 球体間の距離
        int rows = 5;
        int columns = 5;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int index = i * 5 + j;

                float offsetX = (j - columns / 2) * (sphereSize + spacing);
                float offsetY = (i - rows / 2) * (sphereSize + spacing);

                Vector3 position = new Vector3(offsetX, offsetY, 0) + centerPoint.position;
                position = position.normalized * distance; // 中心点から3.5mの位置に配置

                objectsToDistribute[index].transform.position = position;
                objectsToDistribute[index].transform.localScale = Vector3.one * sphereSize;
            }
        }
    }
}
