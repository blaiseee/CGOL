using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CGOL.Scripts
{
    public class GridGenerator : MonoBehaviour
    {
        [Range(8, 124)]
        public int width;

        [Range(8, 124)]
        public int height;

        [Range(0.1f, 1.0f)]
        public float generationGap;

        private static readonly Vector3 CellScale = Vector3.one * 0.5f;

        private Cell[,] _cells;
        private int _height, _width;
        private bool _isRunCoroutineFinished;

        private Transform _transform;

        private Scene _scene;

        private void Awake()
        {
            _transform = transform;

            if (CGOLManager.Initialize())
            {
                CGOLManager.GameState = GameStateEnum.Wait;
            }
        }

        void Start()
        {
            _scene = SceneManager.GetActiveScene();
            _isRunCoroutineFinished = true;
            if (CGOLManager.GameState == GameStateEnum.Invalid)
                return;
            
            UIManager.Instance.btn_Generate.onClick.AddListener(PopulateGrid);
        }

        //private void Update()
        //{
        //    if(Input.GetKeyDown(KeyCode.Space))
        //    {
        //        if (CGOLManager.GameState == GameStateEnum.AcceptInput && _isRunCoroutineFinished)
        //        {
        //            CGOLManager.GameState = GameStateEnum.Run;
        //            _isRunCoroutineFinished = false;
        //            StartCoroutine(Run());
        //        }

        //        else if (CGOLManager.GameState == GameStateEnum.Run)
        //            CGOLManager.GameState = GameStateEnum.AcceptInput;
        //    }
        //}

        private void LateUpdate()
        {
            _height = (int)UIManager.Instance.slider_CellHeight.value;
            _width = (int)UIManager.Instance.slider_CellWidth.value;
        }

        private void PopulateGrid()
        {
            _cells = new Cell[_height, _width];
            var offset = new Vector3Int
            {
                x = _width - Mathf.FloorToInt(0.5f * (_width - 1) + 1.0f),
                y = _height - Mathf.FloorToInt(0.5f * (_height - 1) + 1.0f)
            };

            for (int h = 0; h < _height; h++)
            {
                for (int w = 0; w < _width; w++)
                {
                    _cells[h, w] = Instantiate(CGOLManager.CellPrefab, _transform).GetComponent<Cell>();

#if UNITY_EDITOR
                    _cells[h, w].gameObject.name = "Cell (" + h + "," + w + ")";
#endif

                    Transform cellTransform = _cells[h, w].transform;
                    cellTransform.position = new Vector2(w - offset.x, h - offset.y) * 1.0f;
                    cellTransform.rotation = Quaternion.identity;
                    cellTransform.localScale = CellScale;

                    _cells[h, w].Initialize(h, w, _height, _width);
                }
            }

            CGOLManager.GameState = GameStateEnum.AcceptInput;
            CameraController.SetupCamera.Invoke(_width, _height);

            SetUIAfterCellGeneration();
        }

        private void UpdateCells()
        {
            for(int h = 0; h < _height; h++)
            {
                for(int w = 0; w < _width; w++)
                {
                    var sum = _cells[h, w].CalculateCellSum(_cells);
                    switch(sum)
                    {
                        case 3:
                            _cells[h, w].CellState = _cells[h, w].IsAlive ? CellStateEnum.Nothing : CellStateEnum.Alive;
                            break;

                        case 4:
                            _cells[h, w].CellState = CellStateEnum.Nothing;
                            break;

                        default:
                            _cells[h, w].CellState = _cells[h, w].IsAlive ? CellStateEnum.Dead : CellStateEnum.Nothing;
                            break;
                    }
                }
            }
        }

        private void ApplyCellUpdate()
        {
            for(int h = 0; h < _height; h++)
            {
                for(int w = 0; w < _width; w++)
                {
                    if (_cells[h, w].CellState == CellStateEnum.Dead)
                        _cells[h, w].IsAlive = false;


                    else if (_cells[h, w].CellState == CellStateEnum.Alive)
                        _cells[h, w].IsAlive = true;

                    _cells[h, w].IsSumSet = false;
                }
            }
        }

        private IEnumerator Run()
        {
            while(CGOLManager.GameState == GameStateEnum.Run)
            {
                UpdateCells();
                ApplyCellUpdate();
                yield return new WaitForSeconds(UIManager.Instance.slider_GenerationSpeed.value);
            }
            _isRunCoroutineFinished = true;
        }

        #region UI

        public void Button_Start()
        {
            if (CGOLManager.GameState == GameStateEnum.AcceptInput && _isRunCoroutineFinished)
            {
                UIManager.Instance.txt_State.text = "Pause";
                CGOLManager.GameState = GameStateEnum.Run;
                _isRunCoroutineFinished = false;
                StartCoroutine(Run());
            }

            else if (CGOLManager.GameState == GameStateEnum.Run)
            {
                UIManager.Instance.txt_State.text = "Play";
                CGOLManager.GameState = GameStateEnum.AcceptInput;
            }
        }

        public void Button_Restart()
        {
            SceneManager.LoadScene(_scene.buildIndex);
        }

        private void SetUIAfterCellGeneration()
        {
            UIManager.Instance.btn_Generate.gameObject.SetActive(false);
            UIManager.Instance.btn_Restart.gameObject.SetActive(true);
            UIManager.Instance.heightInputParent.gameObject.SetActive(false);
            UIManager.Instance.widthInputParent.gameObject.SetActive(false);

            UIManager.Instance.heightCellCount.enabled = true;
            UIManager.Instance.widthCellCount.enabled = true;

            UIManager.Instance.heightCellCount.text = _height.ToString();
            UIManager.Instance.widthCellCount.text = _width.ToString();
        }

        #endregion

    }
}
