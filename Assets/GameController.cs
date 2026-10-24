using System.Collections;
using UnityEngine;
using TicTacToe;
public class GameController : MonoBehaviour
{
    [SerializeField] float cellSize = 1.6f;
    [SerializeField] float aiDelay = 0.4f;

    GameState state;
    readonly MinimaxAI ai = new MinimaxAI();
    readonly Collider2D[] cellColliders = new Collider2D[9];

    Camera cam;
    Material lineMaterial;
    Transform marksRoot;
    bool aiThinking;
    string message = "";
    static readonly Color White = new Color(0.95f, 0.95f, 0.95f);
    static readonly Color HumanColor = new Color(0.3f, 0.8f, 1f);   // X
    static readonly Color AIColor = new Color(1f, 0.6f, 0.2f);      // O

    void Start()
    {
        cam = Camera.main;
        cam.orthographic = true;
        cam.orthographicSize = 3.5f;
        cam.transform.position = new Vector3(0, 0, -10);
        cam.backgroundColor = new Color(0.12f, 0.12f, 0.16f);

        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        marksRoot = new GameObject("Marks").transform;

        BuildGrid();
        BuildCells();
        NewGame();
    }

    void Update()
    {
        if (state == null || aiThinking || Rules.IsTerminal(state)) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(world);
        if (hit == null) return;

        int idx = System.Array.IndexOf(cellColliders, hit);
        if (idx < 0 || state.Board[idx] != Cell.Empty) return;

        PlayMove(idx); // ход человека
        UpdateMessage();

        if (!Rules.IsTerminal(state))
            StartCoroutine(AITurn());
    }

    IEnumerator AITurn()
    {
        aiThinking = true;
        UpdateMessage();

        yield return new WaitForSeconds(aiDelay);

        int move = ai.GetBestMove(state);
        PlayMove(move);

        aiThinking = false;
        Debug.Log($"ИИ выбрал клетку {move}. Узлов: {ai.NodesVisited}, время: {ai.ElapsedMs} мс");
        UpdateMessage();
    }

    void PlayMove(int idx)
    {
        Cell who = state.CurrentPlayer;
        state = Rules.ApplyMove(state, idx);

        if (who == Cell.Human) DrawX(idx);
        else DrawO(idx);
    }

    void NewGame()
    {
        StopAllCoroutines();
        aiThinking = false;
        state = new GameState();

        foreach (Transform child in marksRoot)
            Destroy(child.gameObject);

        UpdateMessage();
    }

    void UpdateMessage()
    {
        if (Rules.IsTerminal(state))
        {
            switch (Rules.GetWinner(state))
            {
                case Cell.AI: message = "Победил ИИ (O)"; break;
                case Cell.Human: message = "Вы победили!"; break;
                default: message = "Ничья"; break;
            }
        }
        else
        {
            message = state.CurrentPlayer == Cell.Human ? "Ваш ход (X)" : "ИИ думает...";
        }
    }


    Vector3 CellPos(int i)
    {
        float x = (i % 3 - 1) * cellSize;
        float y = (1 - i / 3) * cellSize;
        return new Vector3(x, y, 0);
    }

    void BuildGrid()
    {
        float half = 1.5f * cellSize;
        float offset = 0.5f * cellSize;

        for (int i = 0; i < 2; i++)
        {
            float o = (i == 0 ? -offset : offset);
            DrawLine(new Vector3(o, -half, 0), new Vector3(o, half, 0), 0.05f, White, transform);
            DrawLine(new Vector3(-half, o, 0), new Vector3(half, o, 0), 0.05f, White, transform);
        }
    }

    void BuildCells()
    {
        for (int i = 0; i < 9; i++)
        {
            var go = new GameObject($"Cell{i}");
            go.transform.SetParent(transform);
            go.transform.position = CellPos(i);

            var col = go.AddComponent<BoxCollider2D>();
            col.size = Vector2.one * cellSize;
            cellColliders[i] = col;
        }
    }

    void DrawX(int i)
    {
        Vector3 c = CellPos(i);
        float h = cellSize * 0.3f;
        DrawLine(c + new Vector3(-h, -h), c + new Vector3(h, h), 0.08f, HumanColor, marksRoot);
        DrawLine(c + new Vector3(-h, h), c + new Vector3(h, -h), 0.08f, HumanColor, marksRoot);
    }

    void DrawO(int i)
    {
        Vector3 c = CellPos(i);
        float r = cellSize * 0.3f;
        const int segments = 40;

        var go = new GameObject($"O{i}");
        go.transform.SetParent(marksRoot);
        var lr = go.AddComponent<LineRenderer>();
        SetupRenderer(lr, 0.08f, AIColor);
        lr.loop = true;
        lr.positionCount = segments;

        for (int s = 0; s < segments; s++)
        {
            float a = s * Mathf.PI * 2f / segments;
            lr.SetPosition(s, c + new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0));
        }
    }

    void DrawLine(Vector3 a, Vector3 b, float width, Color color, Transform parent)
    {
        var go = new GameObject("Line");
        go.transform.SetParent(parent);
        var lr = go.AddComponent<LineRenderer>();
        SetupRenderer(lr, width, color);
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
    }

    void SetupRenderer(LineRenderer lr, float width, Color color)
    {
        lr.useWorldSpace = true;
        lr.startWidth = lr.endWidth = width;
        lr.startColor = lr.endColor = color;
        lr.material = lineMaterial;
        lr.sortingOrder = 1;
    }

    // ---------- UI ----------

    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label) { fontSize = 26 };
        GUI.Label(new Rect(20, 20, 600, 40), message, style);

        if (GUI.Button(new Rect(20, 70, 180, 40), "Новая игра"))
            NewGame();
    }
}