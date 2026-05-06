namespace PipePuzzle
{
    using UnityEngine;
    using UnityEngine.UI;

    public class PipePiece : MonoBehaviour
    {
        public enum PipeType { Straight, Corner, TJunction }
        public PipeType pipeType;
        
        // Connections: 0 = Top, 1 = Right, 2 = Bottom, 3 = Left
        public bool[] connections = new bool[4];
        public int rotationIndex = 0; 

        private PipeManager manager;

        public void Init(PipeType type, int initialRotation, PipeManager mgr)
        {
            pipeType = type;
            manager = mgr;
            
            SetBaseConnections();
            
            // Apply initial rotation
            for (int i = 0; i < initialRotation; i++)
            {
                RotateInternal();
            }
        }

        private void SetBaseConnections()
        {
            connections = new bool[4];
            if (pipeType == PipeType.Straight)
            {
                // Sprite is horizontal by default
                connections[1] = true;
                connections[3] = true;
            }
            else if (pipeType == PipeType.Corner)
            {
                // Sprite is Top-Right corner by default
                connections[0] = true;
                connections[1] = true;
            }
            else if (pipeType == PipeType.TJunction)
            {
                // T-Junction: Top, Right, Bottom
                connections[0] = true;
                connections[1] = true;
                connections[2] = true;
            }
        }

        public void OnPipeClick()
        {
            Debug.Log("Pipe clicked: " + gameObject.name);
            RotateInternal();
            manager.OnPipeRotated();
        }

        private void RotateInternal()
        {
            rotationIndex = (rotationIndex + 1) % 4;
            transform.Rotate(0, 0, -90f);
            
            bool last = connections[3];
            for (int i = 3; i > 0; i--)
            {
                connections[i] = connections[i - 1];
            }
            connections[0] = last;
        }

        public bool HasConnection(int side)
        {
            return connections[side];
        }
    }
}
