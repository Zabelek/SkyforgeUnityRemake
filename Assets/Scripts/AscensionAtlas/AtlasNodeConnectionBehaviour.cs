using System;
using UnityEngine;

public class AtlasNodeConnectionBehaviour : MonoBehaviour
{
    #region Variables
    public AtlasNodeBehaviour ParentNode { get; private set; }
    public AtlasNodeBehaviour ChildNode { get; private set; }
    [SerializeField] private SpriteRenderer _startSprite;
    [SerializeField] private SpriteRenderer _centerSprite;
    [SerializeField] private SpriteRenderer _endSprite;
    //need to properly rescale the connection's sprites
    private float _startWidth, _endWidth, _centerWidth;
    [SerializeField] private Sprite _regularStart, _regularCentr, _activeStart, _activeCenter;
    #endregion

    #region Mono
    protected void Awake()
    {
    }
    protected private void OnDestroy()
    {
        if(ParentNode!= null)
            ParentNode.OnActivation -= CheckActivation;
        if (ChildNode != null)
            ChildNode.OnActivation -= CheckActivation;
    }
    #endregion

    #region Methods
    public void DetermineConnectionBounds()
    {
        //Scale, place and rotate all connections' sprites. If you want to reuse this code, be aware that the Y is flipped with Z here
        _startWidth = _startSprite.GetComponent<SpriteRenderer>().bounds.size.x;
        _endWidth = _endSprite.GetComponent<SpriteRenderer>().bounds.size.x;
        _centerWidth = _centerSprite.GetComponent<SpriteRenderer>().bounds.size.z;
        _startSprite.transform.localScale = new Vector3(_startSprite.transform.localScale.x, _startSprite.transform.localScale.y / 2, _startSprite.transform.localScale.z);
        _endSprite.transform.localScale = new Vector3(_endSprite.transform.localScale.x, _endSprite.transform.localScale.y / 2, _endSprite.transform.localScale.z);
        Vector3 start = ParentNode.transform.position;
        Vector3 end = ChildNode.transform.position;
        start.y = 0;
        end.y = 0;
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0f, -90f, 0f);
        transform.position = ParentNode.transform.position;
        _startSprite.transform.localPosition = new Vector3(_startWidth * 0.25f, 0, 0);
        _endSprite.transform.localPosition = new Vector3(distance - _endWidth * 0.25f, 0, 0);
        float centerLength = distance - (_startWidth + _endWidth);
        _centerSprite.transform.localPosition = new Vector3((distance) / 2, 0f, 0f);
        _centerSprite.transform.localScale = new Vector3(_centerSprite.transform.localScale.x, centerLength / _centerWidth, _centerSprite.transform.localScale.z);
    }
    public void SetActivated(bool activated)
    {
        if(activated)
        {
            _startSprite.sprite = _activeStart;
            _endSprite.sprite = _activeStart;
            _centerSprite.sprite = _activeCenter;
        }
        else
        {
            _startSprite.sprite = _regularStart;
            _endSprite.sprite = _regularStart;
            _centerSprite.sprite = _regularCentr;
        }
    }
    public void SetNodes(AtlasNodeBehaviour parent, AtlasNodeBehaviour child)
    {
        //In case the connection already had different nodes. it should never be the case, but wonders happen when your PC hates you
        if(ParentNode != null)
            ParentNode.OnActivation -= CheckActivation;
        if (ChildNode != null)
            ChildNode.OnActivation -= CheckActivation;
        ParentNode = parent;
        ChildNode = child;
        ParentNode.OnActivation += CheckActivation;
        ChildNode.OnActivation += CheckActivation;
        DetermineConnectionBounds();
    }
    #endregion

    #region EventHandlers
    private void CheckActivation(object sender, EventArgs e)
    {
        SetActivated(ParentNode.IsActive && ChildNode.IsActive);
    }
    #endregion
}