using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestScatter : MonoBehaviour
{
    #region Variables
    [Tooltip("A prefab to be scattered")]
    public GameObject Prefab;
    [Tooltip("A target count of scattered objects")]
    public int Count = 100;
    [Tooltip("Terrain on which the objects will be scattered. The center of the scattering game object is always in the center.")]
    public Vector3 Size = new Vector3(10, 5, 10);
    [Tooltip("Rotation variation of scattered objects")]
    public bool IncludeRotation = false;
    [Tooltip("Scale variation of scattered objects (not implemented yet)")]
    public bool IncludeScale = false;
    #endregion

#if UNITY_EDITOR
    [ContextMenu("Scatter")]
    public void Scatter()
    {
        // Delete old children from previous scatter
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
