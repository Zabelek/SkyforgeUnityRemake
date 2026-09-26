using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RestrictedAreaBehaviour : MonoBehaviour
{
    #region Variables
    private CapsuleCollider _collider;
    private List<Collider> _charactersInside;
    public bool IsActive { get; private set; }
    [Tooltip("margin used so that the teleport radius isn't too close to the check margin, as it may cause bugs")]
    [SerializeField] private float _safetyMargin;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _charactersInside = new();
    }
    protected virtual void FixedUpdate()
    {
        //for now logic works only for round areas
        var collidersToRemove = new List<Collider>();
        if(IsActive)
        {
            foreach (var collider in _charactersInside)
            {
                if(collider == null || collider.gameObject.IsDestroyed())
                {
                    collidersToRemove.Add(collider);
                }
                else
                {
                    var distance = (this.transform.position - collider.transform.position).magnitude;
                    if (distance > _collider.radius + _safetyMargin)
                    {
                        if (CharacterBehaviour.FindCharacterInCollider(collider, out var character) == true)
                        {
                            character.MovePosition(_collider.ClosestPoint(character.transform.position), false);
                        }
                    }
                }
            }
            //the mechanic works only on characters that enter the area while its active
            foreach (var potentialChar in Physics.OverlapSphere(this.transform.position, _collider.radius))
            {
                if ((this.transform.position - potentialChar.transform.position).magnitude < _collider.radius && !_charactersInside.Contains(potentialChar))
                {
                    if (CharacterBehaviour.FindCharacterInCollider(potentialChar, out var character) == true)
                    {
                        _charactersInside.Add(potentialChar);
                    }
                }
            }
            foreach(var collider in collidersToRemove)
            {
                _charactersInside.Remove(collider);
            }
        }
    }
    #endregion

    #region Methods
    public void Reset()
    {
        _charactersInside.Clear();
    }
    public void SetActive(bool active)
    {
        IsActive = active;
        if (active == false)
            Reset();
    }
    #endregion
}
