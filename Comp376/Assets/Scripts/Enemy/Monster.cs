using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float health;
    [SerializeField] protected float speed;
    [SerializeField] private float attackRange;

    protected List<PathNode> path;
    protected Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }        
    }

    public void Initialize(List<PathNode> path, Transform target)
    {
        this.path = path;
        this.target = target;

        MoveAlongPath();
    }

    protected virtual async Task MoveAlongPath()
    {
        int currentNodeIndex = 0;

        float nodeSizeOffset = path[currentNodeIndex].nodeSize / 2;
        Vector2 randomPoint = Random.insideUnitCircle;
        Vector3 randomPathOffset = new Vector3(randomPoint.x, 0, randomPoint.y) * nodeSizeOffset;
        while ((this != null && currentNodeIndex + 1 != path.Count) && Vector3.Distance(transform.position, target.position) > target.localScale.x / 2 + attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[currentNodeIndex + 1].position + Vector3.up + randomPathOffset, Time.deltaTime * speed);

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
        while (this != null && Vector3.Distance(transform.position, target.position) > target.localScale.x / 2 + attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.up, Time.deltaTime * speed);
            await Task.Yield();
        }

        // attack the nexus
    }

    public void takeDamage(float incomingDamage)
    {
        health -= incomingDamage;
    }
}
