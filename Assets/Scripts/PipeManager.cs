namespace PipePuzzle
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;

    public class PipeManager : MonoBehaviour
    {
        public static PipeManager Instance;

        public GameObject pipePrefab;
        public Transform gridParent;
        public GridLayoutGroup gridLayout;
        
        public Sprite straightSprite;
        public Sprite cornerSprite;
        
        public UnityEngine.UI.Image sourceImage;
        public UnityEngine.UI.Image plantImage;
        
        private int rows = 5;
        private int cols = 5;
        private PipePiece[,] grid;
        private Vector2Int sourcePos = new Vector2Int(0, 2);
        private Vector2Int plantPos = new Vector2Int(4, 2);

        private int currentLevel = 0;

        void Awake()
        {
            Instance = this;
        }

        public void StartGame(int level)
        {
            Debug.Log("Starting Level: " + level);
            currentLevel = level;
            GenerateLevel(level);
        }

        void GenerateLevel(int level)
        {
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }

            grid = new PipePiece[cols, rows];
            
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    GameObject go = Instantiate(pipePrefab, gridParent);
                    go.name = "Pipe_" + x + "_" + y;
                    PipePiece piece = go.GetComponent<PipePiece>();
                    
                    PipePiece.PipeType type = PipePiece.PipeType.Straight;
                    if (level == 0) type = PipePiece.PipeType.Straight;
                    else if (level == 1) type = (x + y) % 2 == 0 ? PipePiece.PipeType.Corner : PipePiece.PipeType.Straight;
                    else type = Random.value > 0.65f ? PipePiece.PipeType.Corner : PipePiece.PipeType.Straight;

                    piece.Init(type, Random.Range(0, 4), this);
go.GetComponent<UnityEngine.UI.Image>().sprite = (type == PipePiece.PipeType.Straight) ? straightSprite : cornerSprite;
                    
                    UnityEngine.UI.Button btn = go.GetComponent<UnityEngine.UI.Button>();
                    if (btn == null) btn = go.AddComponent<UnityEngine.UI.Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(piece.OnPipeClick);

                    grid[x, y] = piece;
                }
            }
            
            OnPipeRotated();
        }

        public void OnPipeRotated()
        {
            if (grid == null) return;

            // Reset colors
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (grid[x, y] != null)
                        grid[x, y].GetComponent<UnityEngine.UI.Image>().color = Color.white;
                }
            }

            HashSet<Vector2Int> connected = GetConnectedPath();
            
            foreach (var pos in connected)
            {
                grid[pos.x, pos.y].GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.8f, 1f);
            }

            if (connected.Contains(plantPos) && grid[plantPos.x, plantPos.y].HasConnection(1))
            {
                Debug.Log("Level Complete!");
                UIManager.Instance.ShowWin();
                plantImage.color = Color.green;
            }
            else
            {
                plantImage.color = Color.white;
            }
        }

        HashSet<Vector2Int> GetConnectedPath()
        {
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            if (grid[sourcePos.x, sourcePos.y].HasConnection(3)) 
            {
                queue.Enqueue(sourcePos);
                visited.Add(sourcePos);
            }

            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            while (queue.Count > 0)
            {
                Vector2Int curr = queue.Dequeue();
                
                for (int i = 0; i < 4; i++)
                {
                    if (grid[curr.x, curr.y].HasConnection(i))
                    {
                        Vector2Int next = curr + new Vector2Int(dx[i], dy[i]);
                        if (next.x >= 0 && next.x < cols && next.y >= 0 && next.y < rows)
                        {
                            int oppositeSide = (i + 2) % 4;
                            if (!visited.Contains(next) && grid[next.x, next.y].HasConnection(oppositeSide))
                            {
                                visited.Add(next);
                                queue.Enqueue(next);
                            }
                        }
                    }
                }
            }
            return visited;
        }
    }
}
