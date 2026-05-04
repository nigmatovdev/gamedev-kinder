using UnityEngine;
using System.Collections;
using System;

public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 4f;

    Vector2Int gridPos;
    int cols, rows;
    float cellSize;
    Vector3 gridOrigin;
    Vector2Int[] obstacles;
    Vector2Int starPos;
    Vector2Int startPos;

    public void Setup(Vector2Int start, int cols, int rows, float cellSize,
                      Vector3 gridOrigin, Vector2Int[] obstacles, Vector2Int starPos)
    {
        this.startPos   = start;
        this.gridPos    = start;
        this.cols       = cols;
        this.rows       = rows;
        this.cellSize   = cellSize;
        this.gridOrigin = gridOrigin;
        this.obstacles  = obstacles;
        this.starPos    = starPos;
        transform.position = GridToWorld(start);
    }

    public void ResetToStart()
    {
        gridPos = startPos;
        transform.position = GridToWorld(startPos);
    }

    public IEnumerator MoveStep(Direction dir, Action<MoveResult> callback)
    {
        Vector2Int next = gridPos;
        switch (dir)
        {
            case Direction.Up:    next.y--; break;
            case Direction.Down:  next.y++; break;
            case Direction.Left:  next.x--; break;
            case Direction.Right: next.x++; break;
        }

        if (next.x < 0 || next.x >= cols || next.y < 0 || next.y >= rows)
        {
            yield return BounceAnim(next);
            callback(MoveResult.OutOfBounds);
            yield break;
        }

        foreach (var obs in obstacles)
        {
            if (obs == next)
            {
                yield return BounceAnim(next);
                callback(MoveResult.HitObstacle);
                yield break;
            }
        }

        yield return SmoothMove(GridToWorld(next));
        gridPos = next;

        callback(gridPos == starPos ? MoveResult.ReachedStar : MoveResult.Success);
    }

    IEnumerator SmoothMove(Vector3 target)
    {
        Vector3 from = transform.position;
        float duration = 1f / moveSpeed;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from, target, Mathf.SmoothStep(0, 1, t / duration));
            yield return null;
        }
        transform.position = target;
    }

    IEnumerator BounceAnim(Vector2Int toward)
    {
        Vector3 from   = transform.position;
        Vector3 midway = Vector3.Lerp(from, GridToWorld(toward), 0.3f);
        float dur = 0.15f;

        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from, midway, t / dur);
            yield return null;
        }
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(midway, from, t / dur);
            yield return null;
        }
        transform.position = from;
    }

    public Vector3 GridToWorld(Vector2Int pos) =>
        gridOrigin + new Vector3(pos.x * cellSize, -pos.y * cellSize, 0f);
}
