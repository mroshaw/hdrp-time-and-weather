using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather
{
    [Serializable]
    public class TimeCurve
    {
        [SerializeField] private AnimationCurve timeCurve;
        [Range(-10, 10)] [SerializeField] private float multiplier = 1.0f;
        public float Evaluate(float time)
        {
            return timeCurve.Evaluate(GetNormalizedTime(time)) * multiplier;
        }

        private float GetNormalizedTime(float time)
        {
            return time / 23.99f;
        }
    }
}