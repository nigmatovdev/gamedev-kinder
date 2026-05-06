using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Game9UIManager : MonoBehaviour
{
    Game9Manager _gm;

    TextMeshProUGUI _questionText;
    TextMeshProUGUI _scoreText;
    TextMeshProUGUI _timerText;
    TextMeshProUGUI _gameOverTitleText;
    GameObject      _gameOverPopup;
    TextMeshProUGUI _gameOverScoreText;

    // ── Init ──────────────────────────────────────────────────────────────────
    public void Initialize(Game9Manager gm)
    {
        _gm = gm;
        BuildCanvas();
    }

    // ── Public API ────────────────────────────────────────────────────────────
    public void UpdateQuestion(string q)    => _questionText.text = q;
    public void UpdateScore(int score)      => _scoreText.text    = $"Score: {score}";
    public void HidePopups()                => _gameOverPopup.SetActive(false);

    public void UpdateTimer(float t)
    {
        int secs = Mathf.CeilToInt(t);
        _timerText.text  = $"{secs}s";
        _timerText.color = secs <= 10
            ? new Color(1f, 0.28f, 0.28f)
            : Color.white;
    }

    public void ShowGameOver(int finalScore, string message = "Time's Up!")
    {
        _gameOverTitleText.text = message;
        _gameOverScoreText.text = $"Score: {finalScore}";
        _gameOverPopup.SetActive(true);
    }

    public void ShowHappyEffect(Vector3 worldPos)
        => StartCoroutine(HappyEffectCo(worldPos));

    // ── Canvas builder ────────────────────────────────────────────────────────
    void BuildCanvas()
    {
        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Top HUD strip ─────────────────────────────────────────────────────
        var hud     = MakeRT(canvasGO.transform, "HUD");
        hud.anchorMin       = new Vector2(0, 1);
        hud.anchorMax       = new Vector2(1, 1);
        hud.pivot           = new Vector2(0.5f, 1f);
        hud.sizeDelta       = new Vector2(0, 140);
        hud.anchoredPosition = Vector2.zero;
        var hudImg  = hud.gameObject.AddComponent<Image>();
        hudImg.color = new Color(0f, 0f, 0f, 0.45f);

        // Question text – large, centred inside HUD
        _questionText = AddTMP(hud, "3 + 2 = ?", 76, Color.white, FontStyles.Bold);
        StretchRT(_questionText.rectTransform);

        // ── Score (below HUD, top-left) ───────────────────────────────────────
        _scoreText = AddTMP(canvasGO.transform, "Score: 0", 52,
                            new Color(1f, 0.95f, 0.38f));
        var sRT = _scoreText.rectTransform;
        sRT.anchorMin        = new Vector2(0, 1);
        sRT.anchorMax        = new Vector2(0, 1);
        sRT.pivot            = new Vector2(0, 1);
        sRT.sizeDelta        = new Vector2(420, 65);
        sRT.anchoredPosition = new Vector2(18, -148);
        _scoreText.alignment = TextAlignmentOptions.Left;

        // ── Timer (below HUD, top-right) ──────────────────────────────────────
        _timerText = AddTMP(canvasGO.transform, "60s", 52, Color.white);
        var tRT = _timerText.rectTransform;
        tRT.anchorMin        = new Vector2(1, 1);
        tRT.anchorMax        = new Vector2(1, 1);
        tRT.pivot            = new Vector2(1, 1);
        tRT.sizeDelta        = new Vector2(230, 65);
        tRT.anchoredPosition = new Vector2(-18, -148);
        _timerText.alignment = TextAlignmentOptions.Right;

        // ── Back button (bottom-left, always visible) ─────────────────────────
        var backBtn = MakeButton(canvasGO.transform, "Back",
            new Color(0.55f, 0.55f, 0.55f, 0.92f),
            new Color(0.35f, 0.35f, 0.35f));
        var backRT = backBtn.GetComponent<RectTransform>();
        backRT.anchorMin        = new Vector2(0, 0);
        backRT.anchorMax        = new Vector2(0, 0);
        backRT.pivot            = new Vector2(0, 0);
        backRT.sizeDelta        = new Vector2(180, 78);
        backRT.anchoredPosition = new Vector2(18, 18);
        backBtn.onClick.AddListener(() => SceneManager.LoadScene("GamesMenu"));
        SetBtnLabel(backBtn, "Back", 44);

        // ── Game-over popup ───────────────────────────────────────────────────
        BuildGameOverPopup(canvasGO.transform);
    }

    void BuildGameOverPopup(Transform parent)
    {
        _gameOverPopup = new GameObject("GameOverPopup");
        _gameOverPopup.transform.SetParent(parent, false);

        var popRT = _gameOverPopup.AddComponent<RectTransform>();
        popRT.anchorMin = new Vector2(0.5f, 0.5f);
        popRT.anchorMax = new Vector2(0.5f, 0.5f);
        popRT.pivot     = new Vector2(0.5f, 0.5f);
        popRT.sizeDelta = new Vector2(800, 600); // Increased size for extra button

        var popImg  = _gameOverPopup.AddComponent<Image>();
        popImg.sprite = Game9SpriteFactory.CreateRoundRect(800, 600, 45,
            new Color(0.10f, 0.20f, 0.45f, 0.97f),
            new Color(0.25f, 0.55f, 1.00f));
        popImg.type = Image.Type.Simple;

        // Title
        _gameOverTitleText = AddTMP(_gameOverPopup.transform, "Time's Up!", 84,
                                   Color.white, FontStyles.Bold);
        var titleRT = _gameOverTitleText.rectTransform;
        titleRT.anchorMin        = new Vector2(0.5f, 0.5f);
        titleRT.anchorMax        = new Vector2(0.5f, 0.5f);
        titleRT.pivot            = new Vector2(0.5f, 0.5f);
        titleRT.sizeDelta        = new Vector2(700, 110);
        titleRT.anchoredPosition = new Vector2(0, 180);

        // Final score
        _gameOverScoreText = AddTMP(_gameOverPopup.transform, "Score: 0", 68,
                                    new Color(1f, 0.88f, 0.25f), FontStyles.Bold);
        var fRT = _gameOverScoreText.rectTransform;
        fRT.anchorMin        = new Vector2(0.5f, 0.5f);
        fRT.anchorMax        = new Vector2(0.5f, 0.5f);
        fRT.pivot            = new Vector2(0.5f, 0.5f);
        fRT.sizeDelta        = new Vector2(600, 90);
        fRT.anchoredPosition = new Vector2(0, 70);

        // Restart button (Play Again)
        var restartBtn = MakeButton(_gameOverPopup.transform, "PlayAgain",
            new Color(0.18f, 0.72f, 0.28f),
            new Color(0.08f, 0.48f, 0.12f));
        var rRT = restartBtn.GetComponent<RectTransform>();
        rRT.anchorMin        = new Vector2(0.5f, 0.5f);
        rRT.anchorMax        = new Vector2(0.5f, 0.5f);
        rRT.pivot            = new Vector2(0.5f, 0.5f);
        rRT.sizeDelta        = new Vector2(440, 96);
        rRT.anchoredPosition = new Vector2(0, -60);
        restartBtn.onClick.AddListener(() => _gm.RestartGame());
        SetBtnLabel(restartBtn, "Play Again", 58);

        // Main Menu button
        var menuBtn = MakeButton(_gameOverPopup.transform, "MainMenu",
            new Color(0.6f, 0.2f, 0.2f),
            new Color(0.4f, 0.1f, 0.1f));
        var mRT = menuBtn.GetComponent<RectTransform>();
        mRT.anchorMin        = new Vector2(0.5f, 0.5f);
        mRT.anchorMax        = new Vector2(0.5f, 0.5f);
        mRT.pivot            = new Vector2(0.5f, 0.5f);
        mRT.sizeDelta        = new Vector2(440, 96);
        mRT.anchoredPosition = new Vector2(0, -180);
        menuBtn.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        SetBtnLabel(menuBtn, "Main Menu", 58);

        _gameOverPopup.SetActive(false);
    }

    // ── Happy effect (world-space star burst) ─────────────────────────────────
    IEnumerator HappyEffectCo(Vector3 worldPos)
    {
        var go = new GameObject("HappyFX");
        go.transform.position = worldPos + Vector3.up * 0.6f;

        var sr      = go.AddComponent<SpriteRenderer>();
        sr.sprite       = Game9SpriteFactory.CreateStarBurst();
        sr.sortingOrder = 20;

        float t = 0f;
        while (t < 0.55f)
        {
            t += Time.deltaTime;
            float p = t / 0.55f;
            go.transform.localScale = Vector3.one * (1f + p * 2.5f);
            sr.color = new Color(1f, 0.9f, 0f, 1f - p);
            yield return null;
        }
        Destroy(go);
    }

    // ── Builder helpers ───────────────────────────────────────────────────────
    static RectTransform MakeRT(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<RectTransform>();
    }

    static TextMeshProUGUI AddTMP(Transform parent, string text, float size,
        Color color, FontStyles style = FontStyles.Normal)
    {
        var go  = new GameObject("Lbl");
        go.transform.SetParent(parent, false);
        var tmp       = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = size;
        tmp.color     = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;
        return tmp;
    }

    static void StretchRT(RectTransform rt)
    {
        rt.anchorMin        = Vector2.zero;
        rt.anchorMax        = Vector2.one;
        rt.sizeDelta        = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    static Button MakeButton(Transform parent, string name,
        Color fill, Color border)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var img  = go.AddComponent<Image>();
        img.sprite = Game9SpriteFactory.CreateRoundRect(200, 80, 18, fill, border);
        img.type   = Image.Type.Simple;
        var btn  = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var cb = new ColorBlock
        {
            normalColor      = Color.white,
            highlightedColor = new Color(0.88f, 1.00f, 0.88f),
            pressedColor     = new Color(0.70f, 0.90f, 0.70f),
            selectedColor    = Color.white,
            disabledColor    = new Color(0.5f, 0.5f, 0.5f),
            colorMultiplier  = 1f,
            fadeDuration     = 0.1f
        };
        btn.colors = cb;
        return btn;
    }

    static void SetBtnLabel(Button btn, string text, float size)
    {
        var lbl = AddTMP(btn.transform, text, size, Color.white, FontStyles.Bold);
        StretchRT(lbl.rectTransform);
    }
}
