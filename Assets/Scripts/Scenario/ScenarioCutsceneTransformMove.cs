using System;
using UnityEngine;

public class ScenarioCutsceneTransformMove : ScenarioCutsceneTransform
{
    #region Variables
    public Transform TObject;
    [SerializeField] protected Transform _initialPosition, _destinationPosition;
    private bool _clothTeleportDisabled;
    private CharacterBehaviour _character;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        _clothTeleportDisabled = false;
    }
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (Duration == 0)
        {
            if(TObject.TryGetComponent<CharacterBehaviour>(out CharacterBehaviour character)==true)
            {
                _character = character;
            }
            LockClothSimmulation(false);
            if (_character != null)
            {
                _character.MovePosition(_initialPosition.position);
                _character.MoveRotation(_initialPosition.eulerAngles);
                if(_character.TryGetComponent<Rigidbody>(out var rigidbody) == true)
                    rigidbody.linearVelocity = Vector3.zero;
            }
            else
            {
                TObject.transform.position = _initialPosition.position;
                TObject.transform.eulerAngles = _initialPosition.eulerAngles;
            }
            Finished = true;
            LockClothSimmulation(true);
            _clothTeleportDisabled = true;
        }
        else if (cutsceneTimer <= StartTimer + Duration)
        {
            if (!_clothTeleportDisabled)
                LockClothSimmulation(false);
            float percentDone = (cutsceneTimer - StartTimer) / Duration;
            var vector = Vector3.Lerp(_initialPosition.position, _destinationPosition.position, percentDone);
            var angle = Vector3.Lerp(_initialPosition.eulerAngles, _destinationPosition.eulerAngles, percentDone);
            if (_character != null)
            {
                _character.MovePosition(vector);
                _character.MoveRotation(angle);
            }
            else
            {
                TObject.transform.position = vector;
                TObject.transform.eulerAngles = angle;
            }

            if (!_clothTeleportDisabled)
            {
                LockClothSimmulation(true);
                _clothTeleportDisabled = true;
            }
        }
        else if (cutsceneTimer > StartTimer + Duration)
        {
            if (_character != null)
            {
                _character.MovePosition(_destinationPosition.position);
                _character.MoveRotation(_destinationPosition.eulerAngles);

            }
            else
            {
                TObject.transform.position = _destinationPosition.position;
                TObject.transform.eulerAngles = _destinationPosition.eulerAngles;
            }
            Finished = true;
        }
    }
    private void LockClothSimmulation(bool value)
    {
        var clothes = TObject.GetComponentsInChildren<Cloth>();
        if(clothes.Length>0)
        {
            foreach(var cloth in clothes)
            {
                cloth.enabled = value;
            }
        }
    }
    #endregion
}
