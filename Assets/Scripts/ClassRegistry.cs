using UnityEngine;

public class ClassRegistry : MonoBehaviour
{
    [Tooltip("Every new class has to be referenced here")]
    public HeroClassSO[] RegisteredClasses;
}
