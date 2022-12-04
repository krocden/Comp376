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
            transform.position = Vector3.MoveTowards(transform.position, path[currentNodeIndex + 1].position + Vector3.up * 2, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, path[currentNodeIndex + 1].position + Vector3.up * 2) < 0.5f)
            {
                currentNodeIndex++;
            }

            await Task.Yield();
        }

#if !UNITY_EDITOR
        Destroy(gameObject);
#else
        DestroyImmediate(gameObject);
#endif    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            NotificationManager.Instance.PlayPositionnalNotification(NotificationType.FortificationsDestroyed, transform.position, true);
            WallSegment wall = collision.collider.GetComponent<WallSegment>();
            wall.SetEmptyWall();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Wall"))
        {
            NotificationManager.Instance.PlayPositionnalNotification(NotificationType.FortificationsDestroyed, transform.position, true);
            WallSegment wall = col.GetComponent<WallSegment>();
            wall.SetEmptyWall();
        }
    }
}
