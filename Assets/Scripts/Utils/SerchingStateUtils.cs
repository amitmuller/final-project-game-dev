using System.Collections;
using System.Linq;
using UnityEngine;
using EnemyAI;
using UnityEngine.Rendering.Universal;

namespace SerchingStateUtils
{
    public static class SerchingStateUtils
    {
        
        /// <summary>
        /// When at noise position, enable flashlight and rotate the light left/right for the given duration.
        /// </summary>
        public static void SearchInNoisePosition(EnemyAIController enemy, float searchDuration)
        {
            var light2D = enemy.GetComponentInChildren<Light2D>();
            if (light2D == null)
                return;
            
            // start looking left/right on the light transform only
            enemy.StartCoroutine(LookAroundLightCoroutine(light2D.transform, searchDuration));
        }

        private static IEnumerator LookAroundLightCoroutine(Transform lightTransform, float duration)
        {
            float elapsed = 0f;
            var initialLocalRotation = lightTransform.localRotation;
            const float maxAngle = 45f;
            while (elapsed < duration)
            {
                // oscillate between -maxAngle and +maxAngle around Z
                float angle = Mathf.Sin(elapsed * Mathf.PI * 2f / duration) * maxAngle;
                lightTransform.localRotation = initialLocalRotation * Quaternion.Euler(0f, 0f, angle);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // reset
            lightTransform.localRotation = initialLocalRotation;
            var light2D = lightTransform.GetComponent<Light2D>();
            if (light2D != null)
                light2D.enabled = false;
        }

    }
}