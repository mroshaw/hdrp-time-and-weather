#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DaftAppleGames.Samples
{
    public class SliderValue : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private TMP_Text valueText;

        private Slider _slider;

        private void OnEnable()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(UpdateSliderText);
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(UpdateSliderText);
        }

        private void Start()
        {
            valueText.text = $"{_slider.value:0.00}";
        }

        public void UpdateSliderText(float value)
        {
            valueText.text = $"{value:0.00}";
        }
    }
}