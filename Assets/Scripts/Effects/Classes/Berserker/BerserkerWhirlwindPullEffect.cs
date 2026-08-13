using UnityEngine;
using UnityEngine.AI;

public class BerserkerWhirlwindPullEffect : GameplayEffectBehaviour
{
    #region Variables
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _duration;
    public float ArcHeight = 1f;
    public float MinDistance = 3f;
    //if the character is too close to the berserker, this variable will be set to zero which means it'll just be immobilized for a short time.
    private bool _active;
    private Rigidbody _rigidbody;
    [HideInInspector] public Vector3 BerserkerPosition;
    #endregion

    #region Methods
    public override void OnApply(CharacterBehaviour character)
    {
        character.SetCanMove(false);
        _duration = TimeLeft;
        float dist = Vector3.Distance(BerserkerPosition, character.transform.position);
        if (dist <= MinDistance)
            return;
        _startPos = character.transform.position;
        Vector3 destinationDirection = character.transform.position - BerserkerPosition;
        _targetPos = BerserkerPosition + destinationDirection.normalized * MinDistance;
        ArcHeight = (destinationDirection.magnitude / 15);
        _active = true;
        _rigidbody = character.GetComponent<Rigidbody>();
        if (_rigidbody)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }
    }
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        if (!_active)
            return;
        if (TimeLeft < 0)
            TimeLeft = 0;
        Vector3 pos = Vector3.Lerp(_targetPos, _startPos, TimeLeft / _duration);
        pos.y += Mathf.Sin((TimeLeft / _duration) * Mathf.PI) * ArcHeight;
        character.transform.position = pos;
    }
    public override void OnRemove(CharacterBehaviour character)
    {
        _active = false;
        character.SetCanMove(true);
        if (_rigidbody!= null)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = Vector3.zero;
        }
        NavMesh.SamplePosition(character.transform.position, out var hit, 5, NavMesh.AllAreas);
        character.transform.position = hit.position;
        base.OnRemove(character);
    }
    #endregion
}