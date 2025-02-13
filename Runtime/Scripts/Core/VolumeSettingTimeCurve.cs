using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather
{
    public enum VolumeSetting { ExposureCompensation };

    [Serializable]
    public class VolumeSettingTimeCurve
    {
        [SerializeField] private VolumeSetting volumeSetting;
        [SerializeField] private TimeCurve timeCurve;

        public VolumeSetting VolumeSetting => volumeSetting;

        public float EvaluateCurve(float time)
        {
            return timeCurve.Evaluate(time);
        }
    }
}