using System;
using DaftAppleGames.Attributes;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather
{
    [Serializable]
    public class TimeCurve
    {
        [BoxGroup("Curve Settings")] [SerializeField] private AnimationCurve timeCurve;
        [BoxGroup("Curve Settings")] [Range(-10, 10)] [SerializeField] private float multiplier = 1.0f;
        public float Evaluate(float time)
        {
            return timeCurve.Evaluate(time) * multiplier;
        }
    }
}