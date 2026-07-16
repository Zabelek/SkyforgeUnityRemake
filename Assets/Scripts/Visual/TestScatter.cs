using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestScatter : MonoBehaviour
{
    public GameObject Prefab;
    public int Count = 100;
    public Vector3 Size = new Vector3(10, 5, 10);
    public bool IncludeRotation = false;
    public bool IncludeScale = false;

#if UNITY_EDITOR
    [ContextMenu("Scatter")]
    void Scatter()
    {
        // Delete old children
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < Count; i++)
        {
            Vector3 localPos = new Vector3(
                Random.Range(-Size.x / 2, Size.x / 2),
                Random.Range(-Size.y / 2, Size.y / 2),
                Random.Range(-Size.z / 2, Size.z / 2)
            );

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(Prefab);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = localPos;
            if(IncludeRotation)
                obj.transform.localRotation = Random.rotation;
        }

        EditorUtility.SetDirty(gameObject);
    }
#endif

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Size);
    }
}
