using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class NormalAI : MonoBehaviour
{
    public enum State
    {
        MoveToTarget,    // ターゲットに向けて前進
        StraightMove,    // 現在向きのまま前進
        RandomRotateMove,// ランダムに回転しつつ前進
        Stop             // 停止
    }

    [System.Serializable]
    public class ActionWeights
    {
        [Range(0, 1)] public float stop = 0.3f;
        [Range(0, 1)] public float randomRotate = 0.4f;
        [Range(0, 1)] public float straightMove = 0.3f;
        [Range(0, 1)] public float moveToTarget = 0f; // 攻撃遠距離モードのみ使用

        public void Normalize()
        {
            float sum = stop + randomRotate + straightMove + moveToTarget;
            if (sum > 0f)
            {
                stop /= sum;
                randomRotate /= sum;
                straightMove /= sum;
                moveToTarget /= sum;
            }
            else
            {
                stop = randomRotate = straightMove = moveToTarget = 0.25f;
            }
        }
    }

    [Header("基本移動")]
    [Tooltip("移動速度")]
    public float moveSpeed = 3f;
    [Tooltip("回転速度(度/秒)")]
    public float rotationSpeed = 180f;
    [Tooltip("攻撃モード開始距離")]
    public float attackRange = 8f;
    [Tooltip("待機モードの移動範囲(半径)")]
    public float idleRange = 5f;
    [Tooltip("攻撃モードへの切替検出距離")]
    public float detectRange = 12f;

    [Header("停止時間")]
    [Tooltip("待機モード 停止時間範囲(秒)")]
    public Vector2 idleStopTimeRange = new Vector2(3f, 6f);
    [Tooltip("攻撃モード 停止時間範囲(秒)")]
    public Vector2 attackStopTimeRange = new Vector2(0.5f, 1.5f);

    [Header("動作時間")]
    [Tooltip("待機モード 動作時間範囲(秒)")]
    public Vector2 idleMoveTimeRange = new Vector2(1f, 3f);
    [Tooltip("攻撃モード 動作時間範囲(秒)")]
    public Vector2 attackMoveTimeRange = new Vector2(1f, 2.5f);

    [Header("ランダム回転角度")]
    [Tooltip("ランダム回転の最大角度(+-度)")]
    public float maxRandomRotateAngle = 90f;

    [Header("行動比率")]
    [Tooltip("待機モード：停止・回転前進・直進")]
    public ActionWeights idleWeights = new ActionWeights { stop = 0.3f, randomRotate = 0.4f, straightMove = 0.3f };

    [Tooltip("攻撃モード遠距離：停止なし、ターゲット向き前進含む")]
    public ActionWeights attackFarWeights = new ActionWeights { stop = 0f, randomRotate = 0.4f, straightMove = 0.3f, moveToTarget = 0.3f };

    [Tooltip("攻撃モード近距離：停止あり")]
    public ActionWeights attackNearWeights = new ActionWeights { stop = 0.2f, randomRotate = 0.4f, straightMove = 0.4f };

    private Vector3 startPosition;
    private Vector3 moveDirection; 
    private State currentState;
    private float stateTimer;

    private Unit selfUnit;
    private Transform target;

    private bool inAttackMode = false;
    private Rigidbody rb;
    private Vector3 despos;

    void Awake()
    {
        startPosition = transform.position;
        selfUnit = GetComponent<Unit>();
        rb = GetComponent<Rigidbody>();

        idleWeights.Normalize();
        attackFarWeights.Normalize();
        attackNearWeights.Normalize();

        currentState = State.Stop;
        stateTimer = Random.Range(idleStopTimeRange.x, idleStopTimeRange.y);
        moveDirection = transform.forward;
    }

    void Update()
    {
        UpdateTarget();
        TryEnterAttackMode();

        // 接地していない場合は強制的に停止状態に
        if (rb.useGravity == true)
        {
            currentState = State.Stop;
            return;
        }
        if (inAttackMode)
            UpdateAttackMode();
        else
            UpdateIdleMode();
    }

    void UpdateTarget()
    {
        var candidates = FindObjectsOfType<Unit>()
            .Where(u => u != selfUnit && IsEnemyFaction(u.faction))
            .Select(u => u.transform);

        target = candidates
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .FirstOrDefault();
    }

    bool IsEnemyFaction(Unit.Faction faction)
    {
        if (selfUnit.faction == Unit.Faction.Enemy)
            return faction == Unit.Faction.Neutral || faction == Unit.Faction.Ally;
        if (selfUnit.faction == Unit.Faction.Ally)
            return faction == Unit.Faction.Enemy;
        return false;
    }

    void TryEnterAttackMode()
    {
        if (inAttackMode) return;

        if (target != null && Vector3.Distance(transform.position, target.position) <= detectRange)
        {
            inAttackMode = true;
            SetRandomState(false);
        }
    }

    void UpdateIdleMode()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SetRandomState(true);
        }

        ExecuteCurrentState(false);
    }

    void UpdateAttackMode()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SetRandomState(false);
        }

        ExecuteCurrentState(true);
    }

    State ChooseStateWithWeights(ActionWeights weights, bool isIdle)
    {
        float r = Random.value;

        if (isIdle)
        {
            if ( Vector3.Distance(transform.position, startPosition) > idleRange)
            {
                return State.MoveToTarget;
            }
            if (r < weights.stop) return State.Stop;
            r -= weights.stop;

            if (r < weights.randomRotate) return State.RandomRotateMove;
            r -= weights.randomRotate;

            return State.StraightMove;
        }
        else
        {
            if (weights.moveToTarget > 0f)
            {
                if (r < weights.moveToTarget) return State.MoveToTarget;
                r -= weights.moveToTarget;
            }

            if (r < weights.stop) return State.Stop;
            r -= weights.stop;

            if (r < weights.randomRotate) return State.RandomRotateMove;
            r -= weights.randomRotate;

            return State.StraightMove;
        }
    }

    void SetRandomState(bool isIdle)
    {
        if (isIdle)
        {
            currentState = ChooseStateWithWeights(idleWeights, true);

            if (currentState == State.Stop)
                stateTimer = Random.Range(idleStopTimeRange.x, idleStopTimeRange.y);
            else
                stateTimer = Random.Range(idleMoveTimeRange.x, idleMoveTimeRange.y);

            if (currentState == State.RandomRotateMove)
            {
                float randomAngle = Random.Range(-maxRandomRotateAngle, maxRandomRotateAngle);
                moveDirection = Quaternion.Euler(0, randomAngle, 0) * transform.forward;
            }
            else if (currentState == State.StraightMove)
            {
                moveDirection = transform.forward;
            }
        }
        else
        {
            if (target == null)
            {
                SetRandomState(true);
                return;
            }

            float distToTarget = Vector3.Distance(transform.position, target.position);
            bool isTargetNear = distToTarget <= attackRange;

            if (!isTargetNear)
            {
                currentState = ChooseStateWithWeights(attackFarWeights, false);
                stateTimer = Random.Range(attackMoveTimeRange.x, attackMoveTimeRange.y);

                if (currentState == State.RandomRotateMove)
                {
                    float randomAngle = Random.Range(-maxRandomRotateAngle, maxRandomRotateAngle);
                    moveDirection = Quaternion.Euler(0, randomAngle, 0) * transform.forward;
                }
                else if (currentState == State.StraightMove)
                {
                    moveDirection = transform.forward;
                }
                else if (currentState == State.MoveToTarget)
                {
                    Vector3 toTarget = (target.position - transform.position).normalized;
                    moveDirection = new Vector3(toTarget.x, 0, toTarget.z);
                }
            }
            else
            {
                currentState = ChooseStateWithWeights(attackNearWeights, false);

                if (currentState == State.Stop)
                    stateTimer = Random.Range(attackStopTimeRange.x, attackStopTimeRange.y);
                else
                    stateTimer = Random.Range(attackMoveTimeRange.x, attackMoveTimeRange.y);

                if (currentState == State.RandomRotateMove)
                {
                    float randomAngle = Random.Range(-maxRandomRotateAngle, maxRandomRotateAngle);
                    moveDirection = Quaternion.Euler(0, randomAngle, 0) * transform.forward;
                }
                else if (currentState == State.StraightMove)
                {
                    moveDirection = transform.forward;
                }
            }
        }
    }

    void ExecuteCurrentState(bool inAttack)
    {
        switch (currentState)
        {
            case State.MoveToTarget:
                Vector3 toTarget = (startPosition - transform.position).normalized;
                if (inAttack)
                    toTarget = (target.position - transform.position).normalized;
                if (!inAttack || (target != null && inAttack))
                    RotateTowards(toTarget);
                MoveForward();
                break;

            case State.StraightMove:
                MoveForward();
                break;

            case State.RandomRotateMove:
                RotateTowards(moveDirection);
                MoveForward();
                break;

            case State.Stop:
                // 停止は何もしない
                break;
        }
    }

    void RotateTowards(Vector3 direction)
    {
        Vector3 forwardFlat = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 dirFlat = new Vector3(direction.x, 0, direction.z).normalized;

        float angleDiff = Vector3.SignedAngle(forwardFlat, dirFlat, Vector3.up);
        float maxRotate = rotationSpeed * Time.deltaTime;
        float rotateAmount = Mathf.Clamp(angleDiff, -maxRotate, maxRotate);
        transform.Rotate(0, rotateAmount, 0);
    }

    void MoveForward()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

}
