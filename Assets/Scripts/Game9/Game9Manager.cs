using UnityEngine;

public class Game9Manager : MonoBehaviour
{
    // ── References (set by Bootstrap) ─────────────────────────────────────────
    [HideInInspector] public Game9UIManager  uiManager;
    [HideInInspector] public NumberSpawner   spawner;
    [HideInInspector] public BasketController basket;

    // ── State ─────────────────────────────────────────────────────────────────
    public int   Score         { get; private set; }
    public float TimeLeft      { get; private set; }
    public int   CorrectAnswer { get; private set; }
    public bool  IsGameActive  { get; private set; }

    const float GameDuration = 60f;

    public float SpeedMultiplier
    {
        get
        {
            float elapsed = GameDuration - TimeLeft;
            return 1f + elapsed * 0.012f; // gentle ramp: ~72 % faster at 60 s
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────
    public void Initialize() => StartGame();

    public void StartGame()
    {
        Score     = 0;
        TimeLeft  = GameDuration;
        IsGameActive = true;

        GenerateQuestion();
        spawner.StartSpawning(this);
        uiManager.UpdateScore(Score);
        uiManager.UpdateTimer(TimeLeft);
        uiManager.HidePopups();
    }

    public void RestartGame()
    {
        spawner.ClearAll();
        StartGame();
    }

    public void GenerateQuestion()
    {
        int a = Random.Range(1, 11);
        int b = Random.Range(1, 11);
        CorrectAnswer = a + b;
        uiManager.UpdateQuestion($"{a} + {b} = ?");
        spawner.OnNewQuestion();
    }

    public void OnNumberCaught(int value)
    {
        if (!IsGameActive) return;

        if (value == CorrectAnswer)
        {
            Score += 10;
            uiManager.UpdateScore(Score);
            uiManager.ShowHappyEffect(basket.transform.position);
            
            if (Score >= 100)
            {
                EndGame(true);
                return;
            }
            GenerateQuestion();
        }
        else
        {
            // Wrong number penalty: increased and randomized
            int penalty = Random.Range(5, 11);
            Score -= penalty;
            uiManager.UpdateScore(Score);
            if (Score < 0)
            {
                EndGame(false);
                return;
            }
        }
        }

        void EndGame(bool won)
{
        IsGameActive = false;
        spawner.StopSpawning();
        string message = won ? "YOU WIN!" : "GAME OVER";
        if (!won && Score < 0) message = "YOU LOST!";
        uiManager.ShowGameOver(Score, message);
    }

    // ── Update loop ───────────────────────────────────────────────────────────
    void Update()
    {
        if (!IsGameActive) return;

        TimeLeft -= Time.deltaTime;
        uiManager.UpdateTimer(Mathf.Max(0f, TimeLeft));

        CheckCatches();
        CheckMisses();

        if (TimeLeft <= 0f)
        {
            EndGame(Score >= 100); // Or just game over based on current score
        }
    }

    void CheckCatches()
    {
        var numbers = spawner.GetActiveNumbers();
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            var fn = numbers[i];
            if (fn == null) { numbers.RemoveAt(i); continue; }
            if (fn.WasCaught) continue;

            Vector2 fnPos = fn.transform.position;
            Vector2 bPos  = basket.transform.position;

            // Catch zone: within basket width (±1.3) and at basket height (±0.55)
            if (Mathf.Abs(fnPos.x - bPos.x) < 1.3f &&
                Mathf.Abs(fnPos.y - bPos.y) < 0.55f)
            {
                fn.Catch();
                OnNumberCaught(fn.Value);
            }
        }
    }

    void CheckMisses()
    {
        var numbers = spawner.GetActiveNumbers();
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            var fn = numbers[i];
            if (fn == null) continue;
            
            // If the correct number falls below the basket height, it's a miss
            if (!fn.WasCaught && fn.Value == CorrectAnswer && fn.transform.position.y < -6f)
            {
                fn.Catch(); // Mark as 'caught' so we don't process it again
                int penalty = Random.Range(3, 8);
                Score -= penalty;
                uiManager.UpdateScore(Score);
                
                if (Score < 0)
{
                    EndGame(false);
                    return;
                }
                
                // Regenerate question if the correct one was missed
                GenerateQuestion();
                break;
                }
                }
                }
}
