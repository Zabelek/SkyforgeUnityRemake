using UnityEngine;

public class ScenarioCutsceneTransformSpawn : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Spawn Related Variables")]
    public GameObject ObjectToSpawn;
    [Tooltip("If the object is already placed on the scene, but it's just deactivated")]
    public bool JustActivate;
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (ObjectToSpawn != null)
        {
            if(JustActivate)
            {
                ObjectToSpawn.gameObject.SetActive(true);
            }
            else
            {
                GameObject.Instantiate(ObjectToSpawn.gameObject, ObjectToSpawn.transform.position, ObjectToSpawn.transform.rotation).gameObject.SetActive(true);
            }
        }
        Finished = true;
    }
    #endregion
}
