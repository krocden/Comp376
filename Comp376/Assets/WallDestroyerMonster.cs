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
        Vector2 randomPoint = Random.insideUnitCircle;
        Vector3 randomPathOffset = new Vector3(randomPoint.x, 0, randomPoint.y) * nodeSizeOffset;
        while (this != null && currentNodeIndex + 1 != path.Count)
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
