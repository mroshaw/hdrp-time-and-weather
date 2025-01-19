using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather
{
    [CreateAssetMenu(fileName = "TimeProbeScenarios", menuName = "Daft Apple Games/Time And Weather/Time Probe Scenarios")]
    public class TimeProbeScenarios : ScriptableObject
    {
        public List<TimeScenario> timeScenarios;

        public string GetScenario(int hourToFind)
        {
            // Find the scenario with the closest startTime to timeToFind
            TimeScenario closestScenario = timeScenarios
                .OrderBy(ts => Math.Abs(ts.startHour - hourToFind))
                .ThenBy(ts => ts.startHour) // Resolve ties by choosing the smaller startTime
                .FirstOrDefault();

            return closestScenario?.scenarioName;
        }

        [Serializable]
        public class TimeScenario
        {
            public int startHour;
            public string scenarioName;
        }
    }
}