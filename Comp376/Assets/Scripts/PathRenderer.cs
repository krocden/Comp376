using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    [SerializeField] private float ballSpeed = 3f;
    [SerializeField] private GameObject ball;
    [SerializeField] private float newBallCooldown = 1000f;

    [SerializeField] private float pathTrailHeight = 1f;
    private float timeSinceLastBall;

    public List<List<PathNode>> paths = new List<List<PathNode>>();

    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public void SetPathNodes(List<List<PathNode>> nodes)
    {
        tokenSource.Cancel();
        paths = nodes;
        tokenSource = new CancellationTokenSource();
        timeSinceLastBall = newBallCooldown;
    }

    private void Update()
    {
        if (paths.Count > 0)
        {
            timeSinceLastBall += Time.deltaTime;
            if (timeSinceLastBall > newBallCooldown)
            {
                MoveTroughPaths(paths, tokenSource.Token);
                timeSinceLastBall = 0;
            }
        }

    }

    private async void MoveTroughPaths(List<List<PathNode>> paths, CancellationToken cancellationToken)
    {
        foreach (List<PathNode> nodes in paths)
        {
            foreach (PathNode node in nodes)
            {
                Debug.Log("WAZA");
                if (node.parentNode != null)
                    MoveTroughNode(node, Vector3.Distance(node.position, node.parentNode.position), cancellationToken);
            }
        }
    }

    private async Task MoveTroughNode(PathNode node, float distance, CancellationToken cancellationToken)
    {
        GameObject ball = Instantiate(this.ball, transform);
        ball.transform.position = node.parentNode.position + (Vector3.up * pathTrailHeight);
        ball.name = node.parentNode.ToString();

        while (Vector3.Distance(ball.transform.position, node.position + (Vector3.up * pathTrailHeight)) > 0.1f)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                DestroyBall(ball);
                return;
            }

            ball.transform.position = Vector3.MoveTowards(ball.transform.position, node.position + (Vector3.up * pathTrailHeight), Time.deltaTime * ballSpeed * distance);
            await Task.Yield();
        }

        DestroyBall(ball);
    }

    private void DestroyBall(GameObject ball)
    {
        GameObject.Destroy(ball);
    }

    private void OnApplicationQuit()
    {
        tokenSource.Cancel();
    }

    private void OnDisable()
    {
        tokenSource.Cancel();
    }
}
