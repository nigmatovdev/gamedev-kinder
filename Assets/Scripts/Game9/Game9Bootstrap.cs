using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

// Runs automatically when Game9 scene loads — no GUID wiring needed.
[DefaultExecutionOrder(-100)]
public class Game9Bootstrap : MonoBehaviour
{
    // ── Auto-entry point ──────────────────────────────────────────────────────
    // RuntimeInitializeOnLoadMethod fires after every scene load.
    // It creates this bootstrap if the scene is Game9 and it isn't already present
    // (e.g. placed manually in the scene hierarchy).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoRun()
    {
        if (SceneManager.GetActiveScene().name != "Game9") return;
#if UNITY_2023_1_OR_NEWER
        if (FindFirstObjectByType<Game9Bootstrap>() != null) return;
#else
        if (FindObjectOfType<Game9Bootstrap>() != null) return;
#endif
        new GameObject("Game9Bootstrap").AddComponent<Game9Bootstrap>();
    }

    // ── MonoBehaviour entry point ─────────────────────────────────────────────
    void Awake()
    {
        SetupCamera();
        EnsureEventSystem();
        BuildScene();
    }

    // ── Scene construction ────────────────────────────────────────────────────
    void BuildScene()
    {
        // Background
        var bgGO = new GameObject("Background");
        var bgSR = bgGO.AddComponent<SpriteRenderer>();
        bgSR.sprite       = Game9SpriteFactory.CreateBackground();
        bgSR.sortingOrder = -10;
        bgGO.transform.localScale = new Vector3(22f, 14f, 1f);

        // Core systems
        var gmGO      = new GameObject("GameManager");
        var uiGO      = new GameObject("UIManager");
        var spawnerGO = new GameObject("NumberSpawner");

        var gm      = gmGO.AddComponent<Game9Manager>();
        var ui      = uiGO.AddComponent<Game9UIManager>();
        var spawner = spawnerGO.AddComponent<NumberSpawner>();

        // Basket
        var basketGO = new GameObject("Basket");
        var basketSR = basketGO.AddComponent<SpriteRenderer>();
        basketSR.sprite       = Game9SpriteFactory.CreateBasket();
        basketSR.sortingOrder = 5;
        basketGO.transform.position = new Vector3(0f, -4.2f, 0f);
        var basket = basketGO.AddComponent<BasketController>();

        // Wire
        gm.uiManager = ui;
        gm.spawner   = spawner;
        gm.basket    = basket;

        ui.Initialize(gm);
        gm.Initialize();
    }

    // ── Camera ────────────────────────────────────────────────────────────────
    static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.orthographicSize = 5.5f;
        cam.backgroundColor  = new Color(0.53f, 0.81f, 0.98f);
    }

    // ── EventSystem ───────────────────────────────────────────────────────────
    static void EnsureEventSystem()
    {
#if UNITY_2023_1_OR_NEWER
        if (FindFirstObjectByType<EventSystem>() != null) return;
#else
        if (FindObjectOfType<EventSystem>() != null) return;
#endif
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<InputSystemUIInputModule>();
    }
}
