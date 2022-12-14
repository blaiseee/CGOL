using System.Linq;
using UnityEngine;

namespace CGOL.Scripts
{
    public enum CellStateEnum : byte
    {
        Nothing,
        Dead,
        Alive
    }

    internal struct Index
    {
        internal int H;
        internal int W;
    }

    public class Cell : MonoBehaviour
    {
        public const int NumberOfNeightbors = 8;

        private Renderer _renderer;
        private int _cellState;

        private Index[] _neighbors;

        public int aliveCount;
        public int deadCount;

        public CellStateEnum CellState { get; set; }

        private bool _isAlive;
        public bool IsAlive
        {
            get { return _isAlive; }
            set
            {
                _isAlive = value;
                _cellState = _isAlive ? 1 : 0;
                _renderer.sharedMaterial = CGOLManager.CellMaterials[_cellState];
            }
        }

        public bool IsSumSet { get; set; }

        private int _sum;
        private int Sum
        {
            get
            {
                if(IsSumSet)
                {
                    return _sum;
                }

                CGOLManager.GameState = GameStateEnum.Invalid;
                return -1;

            }
            set
            {
                IsSumSet = true;
                _sum = value;
            }
        }

        public void Initialize(int h, int w, int gridHeight, int gridWidth)
        {
            int gridHeightOffset = gridHeight - 1;
            int gridWidthOffset = gridWidth - 1;

            int hPositiveOffset = h + 1;
            int hNegativeOffset = h - 1;
            int wPositiveOffset = w + 1;
            int wNegativeOffset = w - 1;

            _renderer = GetComponent<Renderer>();
            IsAlive = (Random.Range(0, int.MaxValue) & 0x1) == 0;
            CellState = CellStateEnum.Nothing;
            _neighbors = new Index[NumberOfNeightbors];

            _neighbors[0] = new Index
            {
                H = h,
                W = wNegativeOffset < 0 ? gridWidthOffset : wNegativeOffset
            };

            _neighbors[1] = new Index
            {
                H = hPositiveOffset > gridHeightOffset ? 0 : hPositiveOffset,
                W = wNegativeOffset < 0 ? gridWidthOffset : wNegativeOffset
            };

            _neighbors[2] = new Index
            {
                H = hPositiveOffset > gridHeightOffset ? 0 : hPositiveOffset,
                W = w
            };

            _neighbors[3] = new Index
            {
                H = hPositiveOffset > gridHeightOffset ? 0 : hPositiveOffset,
                W = wPositiveOffset > gridWidthOffset ? 0 : wPositiveOffset
            };

            _neighbors[4] = new Index
            {
                H = h,
                W = wPositiveOffset > gridWidthOffset ? 0: wPositiveOffset
            };

            _neighbors[5] = new Index
            {
                H = hNegativeOffset < 0 ? gridHeightOffset : hNegativeOffset,
                W = wPositiveOffset > gridWidthOffset ? 0 : wPositiveOffset
            };

            _neighbors[6] = new Index
            {
                H = hNegativeOffset < 0 ? gridHeightOffset : hNegativeOffset,
                W = w
            };

            _neighbors[7] = new Index
            {
                H = hNegativeOffset < 0 ? gridHeightOffset : hNegativeOffset,
                W = wNegativeOffset < 0 ? gridWidthOffset : wNegativeOffset
            };
        }

        public int CalculateCellSum(Cell[,] cells)
        {
            if (!IsSumSet)
            {
                Sum = _cellState + _neighbors.Sum(neighbor => cells[neighbor.H, neighbor.W]._cellState);
                UIManager.Instance.txt_Neighbors.text = "Neighbors: " + Sum.ToString();
            }


            return Sum;
        }

        private void OnMouseDown()
        {
            if (CGOLManager.GameState == GameStateEnum.AcceptInput)
                IsAlive = !IsAlive;
        }
    }
}
