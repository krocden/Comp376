using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    [SerializeField] private float ballSpeed = 3f;
    [SerializeField] private GameObject currentPathsVisual;
    [SerializeField] private GameObject nextWavePathsVisual;
    [SerializeField] private float newBallCooldown = 1000f;

    [SerializeField] private float pathTrailHeight = 1f;
    private float timeSinceLastBall;

    public List<List<PathNode>> paths = new List<List<PathNode>>();
    public List<List<PathNode>> nextWavePaths = new List<List<PathNode>>();

    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    private bool displayBalls = true;

    public void SetPathNodes(List<List<PathNode>> nodes, List<List<PathNode>> nextWaveNodes)
    {
        tokenSource.Cancel();
        paths = nodes;
        nextWavePaths = nextWaveNodes;
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
                MoveTroughPaths(paths, false, tokenSource.Token);
                MoveTroughPaths(nextWavePaths, true, tokenSource.Token);
                timeSinceLastBall = 0;
            }
        }

    }

    private async void MoveTroughPaths(List<List<PathNode>> paths, bool isNextWavePath, CancellationToken cancellationToken)
    {
        foreach (List<PathNode> nodes in paths)
        {
            foreach (PathNode node in nodes)
            {
                if (node.parentNode != null)
                    MoveTroughNode(node, Vector3.Distance(node.position, node.parentNode.position), isNextWavePath, cancellationToken);
            }
        }
    }

    private async Task MoveTroughNode(PathNode node, float distance, bool isNextWavePath, CancellationToken cancellationToken)
    {
        GameObject ball = Instantiate(isNextWavePath ? nextWavePathsVisual : currentPathsVisual, transform);
        ball.transform.position = node.parentNode.position + (Vector3.up * pathTrailHeight);
        ball.name = node.parentNode.ToString();

        ball.GetComponent<MeshRenderer>().enabled = displayBalls;
        ball.GetComponent<TrailRenderer>().enabled = displayBalls;

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

    private void OnEnable()
    {
        GameStateManager.Instance.onGameStateChanged.AddListener((currentGameState) => UpdateVisuals(currentGameState));
    }

    private void OnDisable()
    {
        GameStateManager.Instance.onGameStateChanged.RemoveListener(UpdateVisuals);
        tokenSource.Cancel();
    }

    void UpdateVisuals(GameState currentGameState)
    {
        displayBalls = currentGameState == GameState.Planning;
    }
}
