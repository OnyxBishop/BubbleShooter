using System;
using System.Collections;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SlingshotBehaviour
{
    [SelectionBase]
    public class Slingshot : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _movementCurve;
        
        public event Action<Bubble> Reloaded;
        public event Action<Bubble> Shot;
        public event Action BallLanded;

        public Bubble ActiveBubble { get; private set; }
        
        public void Reload(Bubble bubble)
        {
            ActiveBubble = bubble;
            Reloaded?.Invoke(bubble);
        }

        public void Shoot(IReadOnlyList<Vector2> contactPoints, float force)
        {
            Shot?.Invoke(ActiveBubble);
            StartCoroutine(ShootCoroutine(contactPoints, force));
        }

        private IEnumerator ShootCoroutine(IReadOnlyList<Vector2> points, float force)
        {
            var duration = CalculateDuration(force);
            
            for (var i = 0; i < points.Count - 1; i++)
            {
                var startPosition = points[i];
                var endPosition = points[i + 1];
                var segmentDuration = duration / (points.Count - 1);
                var elapsedTime = 0f;

                while (elapsedTime < segmentDuration)
                {
                    var t = elapsedTime / segmentDuration;
                    var curveValue = _movementCurve.Evaluate(t);
                    var newPosition = Vector3.LerpUnclamped(startPosition, endPosition, curveValue);

                    ActiveBubble.transform.position = newPosition;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                
                ActiveBubble.transform.position = endPosition;
            }
            
            BallLanded?.Invoke();
        }
        
        private float CalculateDuration(float force)
        {
            var minDuration = 0.35f;
            var maxDuration = 1f;
            return Mathf.Lerp(maxDuration, minDuration, force);
        }
    }
}