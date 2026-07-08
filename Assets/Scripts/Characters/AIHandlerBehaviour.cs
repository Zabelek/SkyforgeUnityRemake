using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AIHandlerBehaviour : MonoBehaviour
{
    //how namy times would a character try to find another spot near the enemy if the chosen one is taken
    protected const int RANDOM_POSITION_SEARCH_ITERATIONS = 20;
    //used so if the character performs any ability, it smoothly rotates towards the target. Ignores rotation speed
    protected const float TURN_ABILITY_TIME = 0.5f;
    //used so that characters see each other while calculating paths
    public static List<NavMeshAgent> RegisteredAgents = new();
    //Used if the character is stuck
    private Vector3 _lastPosition;
    private float _unstuckTimer, _repathTimer;
    private bool _agentActive;

    //FOR OLD MOVEMENT METHODS:
    //used in the agent to rigidbody vector method
    //protected const float SEPARATION_STRENGTH = 6;
    //protected const float GOAL_STRENGTH = 1.5f;
    //even if the two characters get too close (not intersecting with each other) they will try to find another path
    //protected const float PROXIMITY_ADD_VALUE = 0.6f;

    #region Variables
    [Header("AI Related Variables")]
    [SerializeField] protected MonsterBehaviour _character;
    [Tooltip("When the character in combat stays further to the player than this value, they will try to walk closer.")]
    [SerializeField] protected float _followCloseUpDistanceLimit;
    [Tooltip("Normally, the character would attack only when they're closer to the Player than Close Up Distance Limit. This value allows them to attach a bit further. WARNING: Make sure the hit box of character's attack is greater in size than the potential distance between the Character and Player!")]
    [SerializeField] protected float _attackRangeExtraLimit;
    [Tooltip("Overrides the normal character collision for some checks. If empty, the handler will replace it with the collision radius of the attached agent")]
    [SerializeField] protected float _collisionRadius;
    protected CharacterBehaviour _currentEnemy;
    protected Vector3 _originPoint;
    [Tooltip("Character can wander around within this distance from their spawn point")]
    [SerializeField] protected float _wanderRadius;
    [Tooltip("Character will spot enemies that would get closer than specified value")]
    [SerializeField] protected float _spotEnemyRadius;
    [Tooltip("How likely it is for the character to go to another point while out of combat")]
    [UnityEngine.Range(0, 100)]
    [SerializeField] protected int _wanderEagerness = 10;
    [Tooltip("The time in seconds between each reactions of the character to the environment. The lower is this value, the faster the character would react (may cost performance if set too low)")]
    [SerializeField] protected float _newTaskTimerMax;
    protected float _newTaskTimer;
    protected NavMeshAgent _agent;
    protected Rigidbody _rigidbody;
    //so the cutscenes are not broken by the handler scheduling new tasks
    protected bool _isActingInCutscene, _cutsceneEnablesKinematic;
    //used for smooth rotation while performing ability
    private float _currentAbilityTurnTimer;
    private Vector3 _currentAbilityTurnForwardVector;
    #endregion

    #region Mono
    protected void Start()
    {
        //in case setting character in the editor was ommited
        if (_character == null)
        {
            _character = GetComponent<MonsterBehaviour>();
        }
        if (_character != null)
        {
            _newTaskTimer = _newTaskTimerMax;
            _agent = GetComponent<NavMeshAgent>();
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _rigidbody = GetComponent<Rigidbody>();
            _originPoint = _character.transform.position;
            RegisteredAgents.Add(_agent);
        }
        _isActingInCutscene = false;
        _cutsceneEnablesKinematic = false;
        if (_collisionRadius == 0 && _agent!= null)
            _collisionRadius = _agent.radius;
        _agentActive = false;
    }
    protected virtual void FixedUpdate()
    {
        if (!Globals.Instance.IsCutscenePlaying)
        {
            var canMove = _character.CanMove();
            if (_character?.IsDead == false && canMove && !_isActingInCutscene)
            {
                _newTaskTimer -= Time.fixedDeltaTime;
                if (_newTaskTimer <= 0)
                {
                    _newTaskTimer = _newTaskTimerMax;
                    if (_character.IsInCombat)
                        HandleCombatMovement();
                    else
                        HandleIdleMovement();
                }
            }
            if (_character?.IsDead == true || _agent.path == null || !canMove)
                DeactivatenavmeshAgent();
            if (_isActingInCutscene)
            {
                //clearing cutscene values to free the character
                DeactivatenavmeshAgent();
            }
            //turning to enemy if ability is activated
            if(_currentAbilityTurnTimer>0)
            {
                _currentAbilityTurnTimer -= Time.fixedDeltaTime;
                if (_currentAbilityTurnTimer < 0)
                    _currentAbilityTurnTimer = 0;
                transform.forward = Vector3.Lerp(_currentAbilityTurnForwardVector, _character.transform.forward, _currentAbilityTurnTimer / TURN_ABILITY_TIME);
            }
        }
        ManageStuck();
        AgentToRigidbodyMovement();
    }
    private void ManageStuck()
    {
        if (_agentActive == true)
        {
            _unstuckTimer += Time.fixedDeltaTime;
            if (_unstuckTimer >= 2)
            {
                if ((_lastPosition - this.transform.position).magnitude < 0.2f)
                {
                    ActivateNavmeshAgent((this.transform.position - this.transform.forward));
                    _repathTimer = 2;
                }
                _lastPosition = this.transform.position;
                _unstuckTimer = 0;
            }
            if (_agent.velocity.magnitude == 0 && (_agent.destination - this.transform.position).magnitude<0.5f)
            {
                DeactivatenavmeshAgent();
            }
        }
        else
        {
            _unstuckTimer = 0;
        }
        if (_repathTimer > 0)
            _repathTimer -= Time.fixedDeltaTime;
    }
    private void AgentToRigidbodyMovement()
    {
        //new movement method, mot fully tested yet
        if(_rigidbody.isKinematic == false && _character.CanMove())
        {
            _agent.nextPosition = _rigidbody.position;
            var yValue = _agent.velocity.y;
            //basically turns off the physics when the characters move upwards. Without it, if they move too slow, they will stop completely due to gravity
            if (yValue <= 0)
                _rigidbody.linearVelocity = new Vector3(_agent.velocity.x, _rigidbody.linearVelocity.y, _agent.velocity.z);
            else
                _rigidbody.linearVelocity = _agent.velocity;
            transform.forward = Vector3.Lerp(transform.forward, _agent.velocity, Time.fixedDeltaTime * _agent.angularSpeed * Time.fixedDeltaTime);
        }
        //old movement method, overcomplicated and more buggy
        /*_agent.nextPosition = _rigidbody.position;
        if(!_rigidbody.isKinematic)
        {
            Vector3 goalDir = (_agent.steeringTarget - transform.position).normalized;
            Vector3 separationDir = Vector3.zero;
            foreach (var other in RegisteredAgents)
            {
                if (other != _agent)
                {
                    Vector3 diff = transform.position - other.transform.position;
                    float dist = diff.magnitude;
                    var realDist = dist - _agent.radius - other.radius;
                    if (realDist < PROXIMITY_ADD_VALUE && dist > 0)
                    {
                        //once the "too close" treshold is reached, the character will try to smoothly walk around the other one.
                        //The closer they get, the stronger is the force to avoid jumping from 0 to 1 each frame, to avoid shaking
                        separationDir += diff.normalized * (1 - (realDist / PROXIMITY_ADD_VALUE));
                    }
                }
            }
            Vector3 combinedDir = (goalDir * GOAL_STRENGTH + separationDir * SEPARATION_STRENGTH).normalized;
            Vector3 current = _rigidbody.linearVelocity;
            Vector3 target = combinedDir * _agent.speed;
            Vector3 appliedVector = Vector3.Lerp(current, target, Time.fixedDeltaTime * 10);
            _rigidbody.linearVelocity = new Vector3(appliedVector.x, _rigidbody.linearVelocity.y, appliedVector.z);
            transform.forward = Vector3.Lerp(transform.forward, appliedVector, Time.fixedDeltaTime * _agent.angularSpeed / 70);
        }*/
    }
    private void OnDisable()
    {
        RegisteredAgents.Remove(_agent);
    }
    #endregion

    #region Methods
    public virtual void CheckForNewEnemy()
    {
        if (_currentEnemy?.IsDead == true)
            _currentEnemy = null;
        if ((_currentEnemy == null) && _character.ActiveEnemies.Any())
            _currentEnemy = _character.ActiveEnemies[Random.Range(0, _character.ActiveEnemies.Count - 1)];
    }
    public void SetMovingDestinationForCutscene(Vector3 destination, bool enablesKinematic)
    {
        ActivateNavmeshAgent(destination);
        _isActingInCutscene = true;
        //if gravity would interrupt the cutscene
        _cutsceneEnablesKinematic = enablesKinematic;
        if (_cutsceneEnablesKinematic && TryGetComponent<Rigidbody>(out var rigidbody))
        {
            rigidbody.isKinematic = true;
        }
    }
    public void SetMovingDestinationForCutscene(Vector3 destination, bool enablesKinematic, float speed)
    {
        SetMovingDestinationForCutscene(destination, enablesKinematic);
        _agent.speed = speed;
    }
    protected virtual void HandleIdleMovement()
    {
        _agent.speed = _character.GetEffectiveMovementSpeed() * CharacterBehaviour.WALKING_SPEED_MOD;
        //check if enemy is in the patrol area.
        if (_character.Faction.IsAggresive)
        {
            var overlaps = Physics.OverlapSphere(_character.transform.position, _spotEnemyRadius);
            if (overlaps.Length > 0)
            {
                foreach (var hit in overlaps)
                {
                    if (CharacterBehaviour.FindEnemyCharacterInCollider(hit, _character, out CharacterBehaviour character) == true)
                    {
                        _currentEnemy = character;
                        _character.EnterCombat(_currentEnemy, false);
                        _currentEnemy.EnterCombat(_character, false);
                        break;
                    }
                }
            }
        }
        if (!_character.IsInCombat)
        {
            if (!_agentActive)
            {
                if (Random.Range(0, 100) < _wanderEagerness)
                {
                    var newDest = _originPoint + new Vector3(Random.Range(-_wanderRadius, _wanderRadius), 0f, Random.Range(-_wanderRadius, _wanderRadius));
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(newDest, out hit, _wanderRadius, NavMesh.AllAreas))
                    {
                        if (!AreCharactersInThePointAgentIncluded(hit.position))
                        {
                            ActivateNavmeshAgent(hit.position);
                        }
                    }
                    //adding another attempts may be worth considering later
                }
                //else if (Random.Range(0, 20) == 2)
                //{
                    //To implement later: set the character to play special idle animation if not walking around
                //}
            }
            _character.SpeakingBehaviour?.PerformIdleSound(0.0025f);
        }

    }
    protected virtual bool AreCharactersInThePoint(Vector3 point)
    {
        var overlaps = Physics.OverlapSphere(point, _collisionRadius);
        foreach (var hit in overlaps)
        {
            if (CharacterBehaviour.FindCharacterInCollider(hit, _character, out CharacterBehaviour hitCharacter) && hitCharacter != _currentEnemy)
            {
                return true;
            }
        }
        return false;
    }
    protected virtual bool AreCharactersInThePointAgentIncluded(Vector3 point)
    {
        var overlaps = Physics.OverlapSphere(point, _collisionRadius);
        foreach (var hit in overlaps)
        {
            if (CharacterBehaviour.FindCharacterInCollider(hit, _character, out CharacterBehaviour hitCharacter) && hitCharacter != _currentEnemy)
            {
                return true;
            }
        }
        foreach(var agent in RegisteredAgents)
        {
            if ((agent.destination - point).magnitude < _agent.radius + agent.radius)
                return true;
        }
        return false;
    }
    protected virtual void HandleCombatMovement()
    {
        _agent.speed = _character.GetEffectiveMovementSpeed();
        CheckForNewEnemy();
        if(_currentEnemy != null && !_currentEnemy.IsDead)
        {
            var dist = CheckEnemyDistance();
            if (dist > _followCloseUpDistanceLimit && _repathTimer<=0)
            {
                var newDest = SetUpNewDestination();
                ActivateNavmeshAgent(newDest);
            }
            //find a free spot if something ran into collision. Helps with crowds
            if (AreCharactersInThePoint(_character.transform.position))
            {
                var newDest = SetUpNewDestination();
                ActivateNavmeshAgent(newDest);
            }
            if (dist < _followCloseUpDistanceLimit + _attackRangeExtraLimit && TryPerformAttack())
            {
                TurnTowardsAbilityTarget();
            }
        }     
    }
    protected void ActivateNavmeshAgent(Vector3 vector)
    {
        if(vector != _character.transform.position)
        {
            _agent.destination = vector;
            _agentActive = true;
        }
    }
    protected void DeactivatenavmeshAgent()
    {
        if(_agent.isActiveAndEnabled)
        {
            _agent.destination = _character.transform.position;
            _agent.ResetPath();

        }
        if (_isActingInCutscene)
        {
            if (_cutsceneEnablesKinematic && TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.isKinematic = false;
            }
            _isActingInCutscene = false;
        }
        _agentActive = false;
    }
    protected float CheckEnemyDistance()
    {
        return (_character.transform.position - _currentEnemy.transform.position).magnitude;
    }
    protected virtual Vector3 SetUpNewDestination()
    {
        //closest possible destination
        var destVector = Vector3.Lerp(_character.transform.position, _currentEnemy.transform.position, 1 - (_followCloseUpDistanceLimit / (_character.transform.position - _currentEnemy.transform.position).magnitude));
        var overlaps = Physics.OverlapSphere(destVector, _collisionRadius);
        bool collision = false;
        int iterationsDone = 0;
        if (AreCharactersInThePoint(destVector))
            collision = true;
        //random near-enemy vector if the first one is taken, so that enemies don't try to go all into one spot, if approaching from one direction
        while (collision && iterationsDone < RANDOM_POSITION_SEARCH_ITERATIONS)
        {
            destVector = _currentEnemy.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * _followCloseUpDistanceLimit;
            overlaps = Physics.OverlapSphere(destVector, _collisionRadius);
            if (!AreCharactersInThePoint(destVector))
            {
                collision = false;
            }
            iterationsDone++;
        }
        if(!collision)
            return destVector;
        else
            return _character.transform.position;
    }
    public virtual float GetMovingSpeed()
    {
        return _agent.velocity.magnitude;
    }
    public virtual bool TryPerformAttack()
    {
        if(_character.CanAct())
        {
            //it can't be return _character.TryPerformAbility(_character.SpecialAttack), because it won't check for niemal attack and just return false!
            if (_character.SpecialAttack != null && _character.TryPerformAbility(_character.SpecialAttack))
                return true;
            else if (_character.BaseAttack != null && _character.TryPerformAbility(_character.BaseAttack))
                return true;
        }
        return false;
    }
    public void StopMovement()
    {
        DeactivatenavmeshAgent();
    }
    protected void TurnTowardsAbilityTarget()
    {
        _currentAbilityTurnTimer = TURN_ABILITY_TIME;
        _currentAbilityTurnForwardVector = -(_character.transform.position - _currentEnemy.transform.position).normalized;
    }
    #endregion
}
