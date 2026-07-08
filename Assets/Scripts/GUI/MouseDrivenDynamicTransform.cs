using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDrivenDynamicTransform : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    #region Variables
    public enum DragModType { Rotation, Location, Scale }
    public enum DragAxis { X, Y }
    [Tooltip("the type of modification done to the object on mouse drag")]
    public DragModType ModType;
    [Tooltip("Axis of the mouse movement that will be used")]
    public DragAxis MouseDragAxis;
    public Transform ObjectToRotate;
    public float TransformSpeed = 5;
    [Tooltip("Axis on which the object will be moved/rotated/scaled")]
    public Vector3 VectorOfTransform;
    private float _lastPos;
    #endregion

    #region Methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Made so that clicking in a new position of the screen won't make the object automatically moved by the distance of last grag release
        if (MouseDragAxis == DragAxis.Y)
        {
            _lastPos = eventData.position.y;
        }
        else
        {
            _lastPos = eventData.position.x;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Calculating the amount of drag
        float tempPos = 0;
        if (MouseDragAxis == DragAxis.Y)
        {
            tempPos = eventData.position.y - _lastPos;
            _lastPos = eventData.position.y;
        }
        else
        {
            tempPos = eventData.position.x - _lastPos;
            _lastPos = eventData.position.x;
        }
        //Applying it to the object depending on transform type
        if (ModType == DragModType.Rotation)
        {
            ObjectToRotate.Rotate(VectorOfTransform, -tempPos * TransformSpeed * Time.deltaTime, Space.World);
        }
        else if (ModType == DragModType.Location)
        {
            ObjectToRotate.Translate(VectorOfTransform * -tempPos * TransformSpeed * Time.deltaTime, Space.World);
        }
        else if (ModType == DragModType.Scale)
        {
            ObjectToRotate.localScale += VectorOfTransform * -tempPos * TransformSpeed * Time.deltaTime;
        }
    }
    #endregion
}
