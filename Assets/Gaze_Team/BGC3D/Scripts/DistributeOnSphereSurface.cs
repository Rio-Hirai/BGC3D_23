using UnityEngine;

public class DistributeOnSphereSurface : MonoBehaviour
{
    public GameObject[] objectsToDistribute; // 配置するオブジェクトの配列
    public GameObject baseSphere; // Base_Sphereオブジェクト
    public float targetSize = 0.25f; // ターゲットのサイズ
    public float spacing = 0.01f; // ターゲット間の間隔

    void Start()
    {
        DistributeObjects();
    }

    void DistributeObjects()
    {
        if (objectsToDistribute.Length != 25) // 5x5のグリッドのため、25のオブジェクトが必要
        {
            Debug.LogError("The number of objects to distribute should be 25.");
            return;
        }

        float sphereRadius = baseSphere.transform.localScale.x * 0.5f; // Base_Sphereの半径を取得
        Vector3 centerPosition = transform.position + transform.forward * sphereRadius;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;

        Vector3 start = centerPosition - 2 * (targetSize + spacing) * right - 2 * (targetSize + spacing) * up;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int index = i * 5 + j;
                Vector3 offset = j * (targetSize + spacing) * right + i * (targetSize + spacing) * up;
                Vector3 targetPosition = start + offset;

                // ターゲットの位置を球体の表面に移動
                Vector3 toCenter = (targetPosition - baseSphere.transform.position).normalized;
                targetPosition = baseSphere.transform.position + toCenter * sphereRadius;

                objectsToDistribute[index].transform.position = targetPosition;
                objectsToDistribute[index].transform.LookAt(baseSphere.transform.position);

                // ターゲットのサイズを設定
                objectsToDistribute[index].transform.localScale = new Vector3(targetSize, targetSize, targetSize);
            }
        }
    }
}
