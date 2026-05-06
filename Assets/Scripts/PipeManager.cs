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
        public Sprite tJunctionSprite;
        
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
            
            // Randomize vertical positions for source and plant
            if (level > 0)
            {
                sourcePos = new Vector2Int(0, Random.Range(0, rows));
                plantPos = new Vector2Int(4, Random.Range(0, rows));
            }
            else
            {
                sourcePos = new Vector2Int(0, 2);
                plantPos = new Vector2Int(4, 2);
            }

            UpdateVisualPositions();
            GenerateLevel(level);
        }

        private void UpdateVisualPositions()
        {
            float stride = 155f; // cellSize(150) + spacing(5)
            
            // sourcePos.y is grid row. row 0 is top, row 4 is bottom.
            // UI Y=0 is middle. row 2 is UI Y=0.
            float sourceY = (2 - sourcePos.y) * stride;
            sourceImage.rectTransform.anchoredPosition = new Vector2(-460f, sourceY);

            float plantY = (2 - plantPos.y) * stride;
            plantImage.rectTransform.anchoredPosition = new Vector2(460f, plantY);
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
                    
                    PipePiece.PipeType type;
                    Vector2Int pos = new Vector2Int(x, y);

                    // Ensure source and plant positions always have Straight pipes to connect properly
                    if (pos == sourcePos || pos == plantPos)
                    {
                        type = PipePiece.PipeType.Straight;
                    }
                    else if (level == 0)
                    {
                        type = PipePiece.PipeType.Straight;
                    }
                    else
                    {
                        // Higher chance for TJunction and Straight to make it easier to connect
                        float rand = Random.value;
                        if (rand < 0.45f) type = PipePiece.PipeType.Straight;
                        else if (rand < 0.75f) type = PipePiece.PipeType.Corner;
                        else type = PipePiece.PipeType.TJunction;
                    }

                    piece.Init(type, Random.Range(0, 4), this);
                    
                    Sprite spriteToUse = straightSprite;
                    if (type == PipePiece.PipeType.Corner) spriteToUse = cornerSprite;
                    else if (type == PipePiece.PipeType.TJunction) spriteToUse = tJunctionSprite;
                    
                    go.GetComponent<UnityEngine.UI.Image>().sprite = spriteToUse;
                    
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
