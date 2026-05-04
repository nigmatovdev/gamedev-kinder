using UnityEngine;

// Attached to a single "Game4Bootstrap" GameObject in the scene.
// Creates all game objects and wires every reference at runtime.
[DefaultExecutionOrder(-100)]
public class Game4Bootstrap : MonoBehaviour
{
    void Awake()
    {
        SetupCamera();

        // ── Create manager GameObjects ──────────────────────────────────────
        var gmGO    = new GameObject("GameManager");
        var cqGO    = new GameObject("CommandQueue");
        var uiGO    = new GameObject("UIManager");
        var gridGO  = new GameObject("Grid");

        var gm   = gmGO.AddComponent<GameManager>();
        var cq   = cqGO.AddComponent<CommandQueue>();
        var ui   = uiGO.AddComponent<UIManager>();
        var grid = gridGO.AddComponent<GridBuilder>();

        // ── Create character ───────────────────────────────────────────────
        var charGO = new GameObject("Character");
        charGO.transform.position = Vector3.zero;
        var charSR        = charGO.AddComponent<SpriteRenderer>();
        charSR.sprite     = SpriteFactory.CreateCharacterSprite();
        charSR.sortingOrder = 5;
        charGO.transform.localScale = new Vector3(0.85f, 0.85f, 1f);

        var mover = charGO.AddComponent<PlayerMover>();

        // ── Wire references ────────────────────────────────────────────────
        gm.commandQueue = cq;
        gm.uiManager    = ui;
        gm.playerMover  = mover;
        gm.gridBuilder  = grid;

        // ── Initialize subsystems ──────────────────────────────────────────
        grid.Initialize();
        ui.Initialize(gm, cq);

        // ── Start game ─────────────────────────────────────────────────────
        gm.StartGame();
    }

    static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.orthographicSize  = 5.5f;
        cam.backgroundColor   = new Color(0.40f, 0.78f, 0.96f);
    }
}
