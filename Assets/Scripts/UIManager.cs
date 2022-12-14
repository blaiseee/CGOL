using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CGOL.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public GameObject heightInputParent;
        public GameObject widthInputParent;

        public Button btn_Restart;
        public Button btn_Generate;
        public Button btn_Play;

        public Slider slider_CellHeight;
        public Slider slider_CellWidth;
        public Slider slider_GenerationSpeed;

        public InputField inputfield_CellHeight;
        public InputField inputfield_CellWidth;
        public InputField inputfield_GenerationSpeed;

        public Text heightCellCount;
        public Text widthCellCount;

        public Text txt_Neighbors;

        public Text txt_State;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            slider_CellHeight.minValue = 8;
            slider_CellHeight.maxValue = 124;
            slider_CellWidth.minValue = 8;
            slider_CellWidth.maxValue = 124;
            slider_GenerationSpeed.minValue = 0.1f;
            slider_GenerationSpeed.maxValue = 1.0f;

            slider_CellHeight.value = 8;
            slider_CellWidth.value = 8;
            slider_GenerationSpeed.value = 0.1f;

            slider_CellHeight.onValueChanged.AddListener(delegate { SliderHeightValueChanged(); });
            slider_CellWidth.onValueChanged.AddListener(delegate { SliderWidthValueChanged(); });
            slider_GenerationSpeed.onValueChanged.AddListener(delegate { SliderGenSpeedValueChanged(); });

            inputfield_CellHeight.onValueChanged.AddListener(delegate { InputFieldHeightValueChanged(); });
            inputfield_CellWidth.onValueChanged.AddListener(delegate { InputFieldWidthValueChanged(); });
            inputfield_GenerationSpeed.onValueChanged.AddListener(delegate { InputFieldGenSpeedValueChanged(); });
        }

        #region Delegates

        private void SliderHeightValueChanged()
        {
            inputfield_CellHeight.text = slider_CellHeight.value.ToString();
        }

        private void SliderWidthValueChanged()
        {
            inputfield_CellWidth.text = slider_CellWidth.value.ToString();
        }

        private void SliderGenSpeedValueChanged()
        {
            inputfield_GenerationSpeed.text = slider_GenerationSpeed.value.ToString();
        }

        private void InputFieldHeightValueChanged()
        {
            slider_CellHeight.value = int.Parse(inputfield_CellHeight.text);
        }

        private void InputFieldWidthValueChanged()
        {
            slider_CellWidth.value = int.Parse(inputfield_CellWidth.text);
        }

        private void InputFieldGenSpeedValueChanged()
        {
            slider_GenerationSpeed.value = float.Parse(inputfield_GenerationSpeed.text);
        }

        #endregion


    }
}