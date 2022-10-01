using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    [SerializeField] private float ballSpeed = 3f;
    [SerializeField] private GameObject ball;
    [SerializeField] private float newBallCooldown = 3f;
    private float timeSinceLastBall;

    public List<List<PathNode>> paths = new List<List<PathNode>>();

    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public void SetPathNodes(List<List<PathNode>> nodes)
    {
        tokenSource.Cancel();
        this.paths = nodes;
        tokenSource = new CancellationTokenSource();
    }

    private void Update()
    {
        if (this.paths.Count > 0)
        {
            timeSinceLastBall += Time.deltaTime;
            if (timeSinceLastBall >= newBallCooldown) {
                foreach (List<PathNode> path in paths)
                {
                    MoveTroughPath(path, tokenSource.Token);
                }
                timeSinceLastBall = 0;
            }
        }

    }

    private async void MoveTroughPath(List<PathNode> nodes, CancellationToken cancellationToken)
    {
        int currentNodeIndex = 0;

        GameObject ball = GameObject.Instantiate(this.ball, transform);
        ball.transform.position = nodes[currentNodeIndex].position + Vector3.up; 

        while (currentNodeIndex < nodes.Count - 1)
        {
            if (cancellationToken.IsCancellationRequested) {
                GameObject.Destroy(ball);
                return;
            }


            ball.transform.position = Vector3.MoveTowards(ball.transform.position, nodes[currentNodeIndex + 1].position + Vector3.up, Time.deltaTime * ballSpeed);
            await Task.Yield();

            if (ball.transform.position == nodes[currentNodeIndex + 1].position + Vector3.up)
                currentNodeIndex++;
        }

        GameObject.Destroy(ball);
    }
}
