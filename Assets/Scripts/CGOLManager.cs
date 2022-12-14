using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGOL.Scripts
{
    public enum GameStateEnum : byte
    {
        Invalid,
        Wait,
        AcceptInput,
        Run
    }

    public static class CGOLManager
    {
        public static GameStateEnum GameState;

        public static GameObject CellPrefab;
        public static Material[] CellMaterials;

        public static bool Initialize()
        {
            bool result = true;
            CellPrefab = Resources.Load<GameObject>("Prefabs/Cell");

            if (CellPrefab == null)
            {
                GameState = GameStateEnum.Invalid;
                Debug.LogError("Cell prefab is not found");
                result = false;
            }

            CellMaterials = new[]
            {
                Resources.Load<Material>("Materials/Dead"),
                Resources.Load<Material>("Materials/Alive")
            };

            for (int i = 0; i < CellMaterials.Length; i++)
            {
                if (CellMaterials[i] != null)
                    continue;

                GameState = GameStateEnum.Invalid;
                Debug.LogErrorFormat("Cell Material {0} is not found", i);
                result = false;
                break;
            }
            return result;
        }
    }
}