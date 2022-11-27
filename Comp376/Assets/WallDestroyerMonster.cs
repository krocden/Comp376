using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WallDestroyerMonster : Monster
{
    protected override async Task MoveAlongPath()
    {
        int currentNodeIndex = 0;

        float nodeSizeOffset = path[currentNodeIndex].nodeSize / 2;
        while ((this != null && currentNodeIndex + 1 != path.Count) && Vector3.Distance(transform.position, target.nexusBase.position) > target.nexusBase.localScale.x / 2 + attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, path[currentNodeIndex + 1].position + Vector3.up, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, path[currentNodeIndex + 1].position + Vector3.up) < 0.5f)
            {
                currentNodeIndex++;
            }

            await Task.Yield();
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            WallSegment wall = collision.collider.GetComponent<WallSegment>();
            wall.SetEmptyWall();
        }
    }
}
