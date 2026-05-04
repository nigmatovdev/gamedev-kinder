using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;          // New Input System UI module

[DefaultExecutionOrder(-100)]
public class Game4Bootstrap : MonoBehaviour
{
    void Awake()
    {
        SetupCamera();
        EnsureEventSystem();

        var gmGO   = new GameObject("GameManager");
        var cqGO   = new GameObject("CommandQueue");
        var uiGO   = new GameObject("UIManager");
        var gridGO = new GameObject("Grid");

        var gm   = gmGO.AddComponent<GameManager>();
        var cq   = cqGO.AddComponent<CommandQueue>();
        var ui   = uiGO.AddComponent<UIManager>();
        var grid = gridGO.AddComponent<GridBuilder>();

        var charGO          = new GameObject("Character");
        var charSR          = charGO.AddComponent<SpriteRenderer>();
        charSR.sprite       = SpriteFactory.CreateCharacterSprite();
        charSR.sortingOrder = 5;
        charGO.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        var mover = charGO.AddComponent<PlayerMover>();

        gm.commandQueue = cq;
        gm.uiManager    = ui;
        gm.playerMover  = mover;
        gm.gridBuilder  = grid;

        grid.Initialize();
        ui.Initialize(gm, cq);
        gm.StartGame();
    }

    static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.orthographicSize = 5.5f;
        cam.backgroundColor  = new Color(0.40f, 0.78f, 0.96f);
    }

    // The project uses New Input System (activeInputHandler = 1).
    // StandaloneInputModule only handles the legacy input — buttons stay deaf.
    // InputSystemUIInputModule is the correct module for com.unity.inputsystem.
    static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;

        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<InputSystemUIInputModule>();   // ← New Input System UI
    }
}
