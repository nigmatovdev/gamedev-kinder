using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    GameManager   gm;
    CommandQueue  cq;
    Canvas        canvas;

    // Top HUD
    TextMeshProUGUI levelLabel;
    TextMeshProUGUI hintLabel;

    // Command queue
    Transform commandRow;
    List<GameObject> queueIcons = new List<GameObject>();

    // Popups
    GameObject winPopup;
    GameObject retryPopup;

    // Control buttons
    Button runBtn;
    Button clearBtn;
    List<Button> actionButtons = new List<Button>();

    // Arrow icon sprites cached
    Sprite[] arrowSprites = new Sprite[4];

    public void Initialize(GameManager gameManager, CommandQueue commandQueue)
    {
        gm = gameManager;
        cq = commandQueue;

        for (int i = 0; i < 4; i++)
            arrowSprites[i] = SpriteFactory.CreateArrowSprite((Direction)i);

        BuildCanvas();
    }

    // ── Public API ────────────────────────────────────────────────────────────
    public void LoadLevel(int lvlNum, int total)
    {
        levelLabel.text = $"Level {lvlNum} / {total}";
        hintLabel.text  = $"Hint: {gm.CurrentLevel.hint}";
        RefreshCommandQueue();
        HidePopups();
        SetButtonsInteractable(true);
    }

    public void RefreshCommandQueue()
    {
        foreach (var icon in queueIcons) Destroy(icon);
        queueIcons.Clear();

        foreach (var dir in cq.Commands)
        {
            var icon = new GameObject("CmdIcon");
            icon.transform.SetParent(commandRow, false);

            var rect = icon.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(44, 44);

            var img = icon.AddComponent<Image>();
            img.sprite               = arrowSprites[(int)dir];
            img.preserveAspect       = true;

            queueIcons.Add(icon);
        }
    }

    public void ShowWin(bool hasNext)
    {
        winPopup.SetActive(true);
        retryPopup.SetActive(false);

        var nextBtn = winPopup.transform.Find("NextBtn")?.GetComponent<Button>();
        if (nextBtn != null)
            nextBtn.gameObject.SetActive(hasNext);
    }

    public void ShowRetry()
    {
        retryPopup.SetActive(true);
        winPopup.SetActive(false);
    }

    public void HidePopups()
    {
        if (winPopup)   winPopup.SetActive(false);
        if (retryPopup) retryPopup.SetActive(false);
    }

    public void SetButtonsInteractable(bool interactable)
    {
        foreach (var b in actionButtons) b.interactable = interactable;
    }

    // ── Canvas Construction ───────────────────────────────────────────────────
    void BuildCanvas()
    {
        var cvGO = new GameObject("Canvas_Game4");
        canvas = cvGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = cvGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;

        cvGO.AddComponent<GraphicRaycaster>();

        // ── Background panel ──────────────────────────────────────────────
        var bgPanel = MakePanel(cvGO.transform, "BgPanel",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(0, -1920), new Vector2(1080, 1920),
            new Color(0.40f, 0.78f, 0.96f));

        // ── Title bar ─────────────────────────────────────────────────────
        var titleBar = MakePanel(cvGO.transform, "TitleBar",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            Vector2.zero, new Vector2(0, 110),
            new Color(0.13f, 0.55f, 0.82f));

        var titleTMP = MakeText(titleBar.transform, "Title", "Code the Path",
            new Vector2(0.1f, 0), new Vector2(0.7f, 1), 52, FontStyles.Bold, Color.white);

        levelLabel = MakeText(titleBar.transform, "LevelLabel", "Level 1 / 3",
            new Vector2(0.7f, 0), new Vector2(1f, 1), 36, FontStyles.Normal,
            new Color(1f, 0.95f, 0.5f));

        // ── Hint bar ──────────────────────────────────────────────────────
        var hintBar = MakePanel(cvGO.transform, "HintBar",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -110), new Vector2(0, 55),
            new Color(0.98f, 0.88f, 0.40f, 0.85f));

        hintLabel = MakeText(hintBar.transform, "Hint", "Hint: Right × 4!",
            Vector2.zero, Vector2.one, 30, FontStyles.Italic,
            new Color(0.2f, 0.1f, 0f));

        // ── Bottom control panel ──────────────────────────────────────────
        var ctrlPanel = MakePanel(cvGO.transform, "ControlPanel",
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0f),
            Vector2.zero, new Vector2(0, 340),
            new Color(0.15f, 0.15f, 0.25f, 0.92f));

        // Command queue scroll area
        var queueArea = MakePanel(ctrlPanel.transform, "QueueArea",
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1f),
            new Vector2(0, -4), new Vector2(-20, 80),
            new Color(0.08f, 0.08f, 0.18f, 0.8f));

        var queueLabel = MakeText(queueArea.transform, "QLabel", "Commands:",
            new Vector2(0, 0), new Vector2(0.25f, 1), 26, FontStyles.Bold,
            new Color(0.8f, 0.8f, 1f));

        var scrollContent = new GameObject("CommandRow");
        scrollContent.transform.SetParent(queueArea.transform, false);
        var scrollRect = scrollContent.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.24f, 0);
        scrollRect.anchorMax = new Vector2(1f,    1);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;

        var hLayout = scrollContent.AddComponent<HorizontalLayoutGroup>();
        hLayout.childAlignment       = TextAnchor.MiddleLeft;
        hLayout.spacing              = 6;
        hLayout.childForceExpandWidth  = false;
        hLayout.childForceExpandHeight = false;
        hLayout.padding              = new RectOffset(6, 6, 6, 6);

        commandRow = scrollContent.transform;

        // Arrow button row
        var arrowRow = new GameObject("ArrowRow");
        arrowRow.transform.SetParent(ctrlPanel.transform, false);
        var arrowRowRect = arrowRow.AddComponent<RectTransform>();
        arrowRowRect.anchorMin        = new Vector2(0, 1);
        arrowRowRect.anchorMax        = new Vector2(1, 1);
        arrowRowRect.pivot            = new Vector2(0.5f, 1f);
        arrowRowRect.anchoredPosition = new Vector2(0, -88);
        arrowRowRect.sizeDelta        = new Vector2(0, 120);

        var arrowHLayout = arrowRow.AddComponent<HorizontalLayoutGroup>();
        arrowHLayout.childAlignment       = TextAnchor.MiddleCenter;
        arrowHLayout.spacing              = 16;
        arrowHLayout.childForceExpandWidth  = false;
        arrowHLayout.childForceExpandHeight = false;

        // Arrow buttons
        var dirColors = new Color[]
        {
            new Color(0.18f, 0.72f, 0.30f), // Up
            new Color(0.18f, 0.40f, 0.90f), // Down
            new Color(0.95f, 0.56f, 0.10f), // Left
            new Color(0.90f, 0.20f, 0.50f)  // Right
        };
        string[] labels = { "UP", "DOWN", "LEFT", "RIGHT" };

        for (int i = 0; i < 4; i++)
        {
            Direction dir = (Direction)i;
            var btn = MakeButton(arrowRow.transform, labels[i], dirColors[i],
                new Vector2(100, 100));
            int captured = i;
            btn.onClick.AddListener(() => gm.AddCommand((Direction)captured));
            actionButtons.Add(btn);
        }

        // Run + Clear buttons row
        var actionRow = new GameObject("ActionRow");
        actionRow.transform.SetParent(ctrlPanel.transform, false);
        var actionRowRect = actionRow.AddComponent<RectTransform>();
        actionRowRect.anchorMin        = new Vector2(0, 1);
        actionRowRect.anchorMax        = new Vector2(1, 1);
        actionRowRect.pivot            = new Vector2(0.5f, 1f);
        actionRowRect.anchoredPosition = new Vector2(0, -216);
        actionRowRect.sizeDelta        = new Vector2(0, 110);

        var actionHLayout = actionRow.AddComponent<HorizontalLayoutGroup>();
        actionHLayout.childAlignment       = TextAnchor.MiddleCenter;
        actionHLayout.spacing              = 24;
        actionHLayout.childForceExpandWidth  = false;
        actionHLayout.childForceExpandHeight = false;

        runBtn = MakeButton(actionRow.transform, "RUN",
            new Color(0.10f, 0.75f, 0.35f), new Vector2(220, 90));
        runBtn.onClick.AddListener(() => gm.RunCommands());
        GetTMP(runBtn).fontSize = 38;
        actionButtons.Add(runBtn);

        clearBtn = MakeButton(actionRow.transform, "CLEAR",
            new Color(0.80f, 0.22f, 0.22f), new Vector2(220, 90));
        clearBtn.onClick.AddListener(() => gm.ClearCommands());
        GetTMP(clearBtn).fontSize = 34;
        actionButtons.Add(clearBtn);

        // Back to menu button
        var menuBtn = MakeButton(actionRow.transform, "MENU",
            new Color(0.4f, 0.4f, 0.55f), new Vector2(130, 90));
        menuBtn.onClick.AddListener(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamesMenu"));
        GetTMP(menuBtn).fontSize = 28;

        // ── Win Popup ─────────────────────────────────────────────────────
        winPopup = BuildPopup(cvGO.transform, "WinPopup",
            new Color(0.10f, 0.65f, 0.25f),
            "You Win!",
            new Color(1f, 0.95f, 0.3f));

        var winNextBtn = MakeButton(winPopup.transform, "Next Level",
            new Color(0.9f, 0.75f, 0.0f), new Vector2(280, 85), "NextBtn");
        winNextBtn.onClick.AddListener(() => gm.NextLevel());

        var winRetryBtn = MakeButton(winPopup.transform, "Play Again",
            new Color(0.3f, 0.5f, 0.9f), new Vector2(240, 75));
        winRetryBtn.onClick.AddListener(() => gm.RestartLevel());

        winPopup.SetActive(false);

        // ── Retry Popup ───────────────────────────────────────────────────
        retryPopup = BuildPopup(cvGO.transform, "RetryPopup",
            new Color(0.75f, 0.18f, 0.15f),
            "Oops! Try Again",
            Color.white);

        var retryBtn = MakeButton(retryPopup.transform, "Retry",
            new Color(0.95f, 0.55f, 0.1f), new Vector2(260, 85));
        retryBtn.onClick.AddListener(() => gm.RestartLevel());

        var menuBtn2 = MakeButton(retryPopup.transform, "Menu",
            new Color(0.4f, 0.4f, 0.55f), new Vector2(220, 75));
        menuBtn2.onClick.AddListener(() =>
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamesMenu"));

        retryPopup.SetActive(false);
    }

    // ── UI Helpers ────────────────────────────────────────────────────────────
    RectTransform MakePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta, Color color)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin        = anchorMin;
        rect.anchorMax        = anchorMax;
        rect.pivot            = pivot;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta        = sizeDelta;
        var img   = go.AddComponent<Image>();
        img.color = color;
        return rect;
    }

    TextMeshProUGUI MakeText(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, float fontSize,
        FontStyles style, Color color)
    {
        var go   = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = new Vector2(8, 4);
        rect.offsetMax = new Vector2(-8, -4);

        var tmp             = go.AddComponent<TextMeshProUGUI>();
        tmp.text            = text;
        tmp.fontSize        = fontSize;
        tmp.fontStyle       = style;
        tmp.color           = color;
        tmp.alignment       = TextAlignmentOptions.MidlineLeft;
        tmp.overflowMode    = TextOverflowModes.Ellipsis;
        return tmp;
    }

    Button MakeButton(Transform parent, string label, Color color,
        Vector2 size, string goName = null)
    {
        var go   = new GameObject(goName ?? label + "_Btn");
        go.transform.SetParent(parent, false);

        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = size;

        var img  = go.AddComponent<Image>();
        img.sprite              = SpriteFactory.CreateRoundRect(
            (int)size.x, (int)size.y, 16, color, DarkenColor(color, 0.3f));
        img.type                = Image.Type.Simple;
        img.preserveAspect      = false;

        var btn  = go.AddComponent<Button>();
        var cols = ColorBlock.defaultColorBlock;
        cols.normalColor      = Color.white;
        cols.highlightedColor = new Color(1.1f, 1.1f, 1.1f);
        cols.pressedColor     = new Color(0.8f, 0.8f, 0.8f);
        btn.colors            = cols;
        btn.targetGraphic     = img;

        var textGO  = new GameObject("Label");
        textGO.transform.SetParent(go.transform, false);
        var textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var tmp         = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text        = label;
        tmp.fontSize    = 30;
        tmp.fontStyle   = FontStyles.Bold;
        tmp.color       = Color.white;
        tmp.alignment   = TextAlignmentOptions.Center;

        return btn;
    }

    GameObject BuildPopup(Transform parent, string name, Color bgColor,
        string titleText, Color titleColor)
    {
        var popup = new GameObject(name);
        popup.transform.SetParent(parent, false);
        var popupRect         = popup.AddComponent<RectTransform>();
        popupRect.anchorMin   = new Vector2(0.1f, 0.3f);
        popupRect.anchorMax   = new Vector2(0.9f, 0.7f);
        popupRect.offsetMin   = Vector2.zero;
        popupRect.offsetMax   = Vector2.zero;

        var img = popup.AddComponent<Image>();
        img.color = bgColor;

        var vLayout = popup.AddComponent<VerticalLayoutGroup>();
        vLayout.childAlignment       = TextAnchor.MiddleCenter;
        vLayout.spacing              = 18;
        vLayout.padding              = new RectOffset(20, 20, 20, 20);
        vLayout.childForceExpandWidth  = true;
        vLayout.childForceExpandHeight = false;

        // Title text
        var titleGO  = new GameObject("Title");
        titleGO.transform.SetParent(popup.transform, false);
        var titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(0, 80);
        var titleTMP        = titleGO.AddComponent<TextMeshProUGUI>();
        titleTMP.text       = titleText;
        titleTMP.fontSize   = 52;
        titleTMP.fontStyle  = FontStyles.Bold;
        titleTMP.color      = titleColor;
        titleTMP.alignment  = TextAlignmentOptions.Center;
        var titleLayoutEl   = titleGO.AddComponent<LayoutElement>();
        titleLayoutEl.preferredHeight = 80;

        return popup;
    }

    static Color DarkenColor(Color c, float amount) =>
        new Color(c.r - amount, c.g - amount, c.b - amount, c.a);

    static TextMeshProUGUI GetTMP(Button btn) =>
        btn.GetComponentInChildren<TextMeshProUGUI>();
}
