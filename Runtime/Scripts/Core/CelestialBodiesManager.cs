using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

using UnityEngine.Rendering.HighDefinition;

namespace DaftAppleGames.TimeAndWeather
{
    [ExecuteAlways]
    public class CelestialBodiesManager : MonoBehaviour
    {
        [System.Serializable]
        public class CelestialBodyData
        {
            public Light light;
            public HDAdditionalLightData hdLightData;
            public Transform transform;
            public float evaluatedIntensity;
            public float fadeFactor;
            public float shadowFadeFactor;
            public bool shadowEnabled;

            public float volumetricMultiplier = 1f;

            public CelestialBodyData(Light light)
            {
                if (light != null)
                {
                    this.light = light;
                    transform = this.light.transform;
                    hdLightData = this.light.GetComponent<HDAdditionalLightData>();
                    evaluatedIntensity = this.light.intensity;
                }
                else
                {
                    this.light = null;
                    transform = null;
                    hdLightData = null;
                    evaluatedIntensity = 1f;
                }

                fadeFactor = 1f;
                shadowFadeFactor = 1f;
                shadowEnabled = false;
            }

            public float Evaluate(float fadeStart, float fadeEnd)
            {
                var angle = transform.eulerAngles.x;
                fadeFactor = GetHorizonMultiplier(angle, fadeStart, fadeEnd);

                shadowFadeFactor = fadeFactor;

                evaluatedIntensity = light.intensity * fadeFactor;

                return evaluatedIntensity;
            }



            public void ApplyFade(bool shadowsEnabled)
            {
                if (light == null)
                    return;

                hdLightData.EnableShadows(shadowsEnabled);

                hdLightData.lightDimmer = fadeFactor;
                hdLightData.shadowDimmer = shadowFadeFactor;
                hdLightData.volumetricDimmer = fadeFactor * volumetricMultiplier;
                hdLightData.volumetricShadowDimmer = shadowFadeFactor;
            }

            public void ApplyFade()
            {
                ApplyFade(shadowEnabled);
            }
        }

        [Tooltip("Controls the angle used as the body orbits")]
        [BoxGroup("Settings")]  [SerializeField] private Vector2 startEndDecreaseAngle = new Vector3(-15f, -20f);

        [Tooltip("Added Celestial bodies to manage here. e.g. sun and moon")]
        [BoxGroup("Settings")] [SerializeField] private List<Light> celestialBodies = new List<Light>();

        private List<CelestialBodyData> _bodiesData;

        private void Update()
        {
            EvaluateBodies();
            SortBodies();
            ApplyFade();
        }

        private void EvaluateBodies()
        {
            foreach (var body in _bodiesData)
            {
                body.Evaluate(startEndDecreaseAngle.x, startEndDecreaseAngle.y);
            }
        }

        private void SortBodies()
        {
            _bodiesData.Sort((CelestialBodyData a, CelestialBodyData b) => a.evaluatedIntensity > b.evaluatedIntensity ? -1 : 1);
        }

        private void ApplyFade()
        {
            // remap the two first bodies to a non overlapping sawtooth pattern
            if (_bodiesData.Count > 1)
            {
                _bodiesData[0].fadeFactor = _bodiesData[0].fadeFactor * 2f - 1f;

                _bodiesData[1].fadeFactor = Mathf.Clamp01(-_bodiesData[0].fadeFactor);
                _bodiesData[0].fadeFactor = Mathf.Clamp01(_bodiesData[0].fadeFactor);

                _bodiesData[0].shadowEnabled = _bodiesData[0].fadeFactor > 0;
                _bodiesData[1].shadowEnabled = _bodiesData[1].fadeFactor > 0;
            }

            for (int i = 0; i < _bodiesData.Count; i++)
            {
                // Disable light on bodies after two first ones
                if (i > 1)
                {
                    _bodiesData[i].fadeFactor = 0;
                    _bodiesData[i].shadowEnabled = false;
                }

                _bodiesData[i].shadowFadeFactor = _bodiesData[i].fadeFactor;

                // Only the first body has shadow enabled
                _bodiesData[i].ApplyFade();
            }
        }

        private void OnValidate()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            _bodiesData = new List<CelestialBodyData>();
            foreach (Light currLight in celestialBodies)
            {
                _bodiesData.Add(new CelestialBodyData(currLight));
            }
        }

        // Returns a float between 0 and 1
        public static float GetHorizonMultiplier(float angle, float fadeStart, float fadeEnd)
        {
            angle %= 360f;

            // after this the angle should be in the range [0;360] for all cases
            if (angle < 0f)
                angle += 360f;

            // Range [-180, 180]
            if (angle > 180f)
                angle -= 360;

            var sign = Mathf.Sign(angle);
            var abs = Mathf.Abs(angle);
            // Mirror over 90� to make it symmetrical
            if (abs > 90f)
                abs = 90f - abs;

            // Angle is now symmetric, ranging from -90� bellow the ground, 0� at the horizon an 90� at top
            angle = abs * sign;

            float factor = Mathf.Clamp01(Mathf.InverseLerp(fadeEnd, fadeStart, angle));

            return factor;
        }
    }
}