using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float health;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected int currencyOnKill;

    protected List<PathNode> path;
    protected Nexus target;

    public int damage;
    public int pushPower;
    private float baseSpeed;
    private int tier = 1;
    [SerializeField] private float tierMultiplier = 1.4f;

    // Start is called before the first frame update
    void Start()
    {
        baseSpeed = speed;
    }

    private void Update()
    {
        
    }

    public void Initialize(List<PathNode> path, Nexus target, int tier = 1)
    {
        this.path = path;
        this.target = target;
        SetTier(tier);

        MoveAlongPath();
    }

    private void SetTier(int tier)
    {
        this.tier = tier;
        float multi = Mathf.Pow(tierMultiplier, tier - 1);
        baseSpeed = speed = Mathf.RoundToInt(speed * multi);
        health = Mathf.RoundToInt(health * multi);
        currencyOnKill = Mathf.RoundToInt(currencyOnKill * multi);
    }

    public void ApplySlow(float modifier)
    {
        speed = modifier * baseSpeed;
    }

    public void RemoveSlow() {
        speed = baseSpeed;
    }

    protected virtual async Task MoveAlongPath()
    {
        int currentNodeIndex = 0;

        float nodeSizeOffset = path[currentNodeIndex].nodeSize / 2;
        Vector2 randomPoint = Random.insideUnitCircle;
        Vector3 randomPathOffset = new Vector3(randomPoint.x, 0, randomPoint.y) * nodeSizeOffset;
        while ((this != null && currentNodeIndex + 1 != path.Count) && Vector3.Distance(transform.position, target.nexusBase.position) > target.nexusBase.localScale.x / 2 + attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[currentNodeIndex + 1].position + Vector3.up + randomPathOffset, Time.deltaTime * speed);
            transform.LookAt(path[currentNodeIndex + 1].position + Vector3.up + randomPathOffset);


            if (Vector3.Distance(transform.position, path[currentNodeIndex + 1].position + Vector3.up + randomPathOffset) < 0.5f)
            {
                currentNodeIndex++;
                randomPoint = Random.insideUnitCircle;
                randomPathOffset = new Vector3(randomPoint.x, 0, randomPoint.y) * nodeSizeOffset;
            }

            await Task.Yield();
        }

        await AttackTarget();
    }

    async Task AttackTarget()
    {
        // move in range
        while (this != null && Vector3.Distance(transform.position, target.nexusBase.position) > target.nexusBase.localScale.x / 2 + attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.nexusBase.position + Vector3.up, Time.deltaTime * speed);
            await Task.Yield();
        }

        // attack the nexus
        if (gameObject != null)
        {
            target.TakeDamage(tier);
            Destroy(gameObject);
        }
    }

    public bool TakeDamage(float incomingDamage)
    {
        health -= incomingDamage;
        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    private void Die()
    {
        CurrencyManager.Instance.AddCurrency(currencyOnKill);
        Destroy(gameObject);
    }
}
