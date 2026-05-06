using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumberSpawner : MonoBehaviour
{
    readonly List<FallingNumber> _active = new List<FallingNumber>();

    Game9Manager _gm;
    Coroutine    _loop;
    float        _camHalfW;
    bool         _running;

    const float BaseInterval = 1.8f; // seconds between waves

    // ── Public API ────────────────────────────────────────────────────────────
    public void StartSpawning(Game9Manager gm)
    {
        _gm       = gm;
        var cam   = Camera.main;
        _camHalfW = cam != null ? cam.orthographicSize * cam.aspect * 0.82f : 3.8f;
        _running  = true;

        StopLoop();
        _loop = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        _running = false;
        StopLoop();
    }

    public void ClearAll()
    {
        foreach (var fn in _active)
            if (fn != null) Destroy(fn.gameObject);
        _active.Clear();
    }

    // Called by GameManager when a new question is generated — wipe old numbers
    // so the player only sees numbers relevant to the current question.
    public void OnNewQuestion() => ClearAll();

    public List<FallingNumber> GetActiveNumbers()
    {
        _active.RemoveAll(fn => fn == null);
        return _active;
    }

    // ── Coroutine ─────────────────────────────────────────────────────────────
    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(0.4f); // short warm-up
        while (_running)
        {
            SpawnWave();
            float interval = BaseInterval / _gm.SpeedMultiplier;
            yield return new WaitForSeconds(Mathf.Max(0.6f, interval));
        }
    }

    void SpawnWave()
    {
        int correct = _gm.CorrectAnswer;

        // Build 3 unique wrong answers in the valid sum range (2–20)
        var decoys = new List<int>();
        int tries  = 0;
        while (decoys.Count < 3 && tries++ < 40)
        {
            int v = Random.Range(2, 21);
            if (v != correct && !decoys.Contains(v)) decoys.Add(v);
        }

        var pool = new List<int> { correct };
        pool.AddRange(decoys);
        Shuffle(pool);

        float spacing = _camHalfW * 2f / (pool.Count + 1);
        float baseX   = -_camHalfW + spacing;

        for (int i = 0; i < pool.Count; i++)
        {
            float x = baseX + i * spacing + Random.Range(-0.3f, 0.3f);
            float y = 6.5f + Random.Range(-0.5f, 1.5f); // Randomized spawn height
            Spawn(pool[i], new Vector3(x, y, 0f));
        }
}

    void Spawn(int value, Vector3 pos)
    {
        var go = new GameObject("Num_" + value);
        go.transform.position = pos;

        var sr      = go.AddComponent<SpriteRenderer>();
        sr.sprite       = Game9SpriteFactory.CreateNumberBall(value);
        sr.sortingOrder = 4;

        float speed = 2.6f * _gm.SpeedMultiplier;
        var   fn    = go.AddComponent<FallingNumber>();
        fn.Setup(value, speed);

        _active.Add(fn);
    }

    void StopLoop()
    {
        if (_loop != null) { StopCoroutine(_loop); _loop = null; }
    }

    static void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
