using System.Collections;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SlingshotBehaviour
{
    public class SlingshotStripsView : MonoBehaviour
    {
        [Header("Lines View")]
        [SerializeField] private LineRenderer _leftStrip;

        [SerializeField] private LineRenderer _rightStrip;
        [SerializeField] private Transform _leftStripEnd;
        [SerializeField] private Transform _rightStripEnd;
        [SerializeField] private float _elasticSpeed;
        [SerializeField] private AnimationCurve _revertCurve;

        private Vector2 _firePointPosition;
        private Bubble _currentBubble;

        public void Init(Vector2 firePointPosition)
        {
            _firePointPosition = firePointPosition;
            SetLines(_firePointPosition);
        }

        public void SetBubble(Bubble bubble)
            => _currentBubble = bubble;

        public void SetLines(Vector2 position)
        {
            _leftStrip.SetPosition(0, position);
            _leftStrip.SetPosition(1, _leftStripEnd.position);

            _rightStrip.SetPosition(0, position);
            _rightStrip.SetPosition(1, _rightStripEnd.position);

            if (_currentBubble == null)
                return;

            _currentBubble.transform.position = position;
            _currentBubble.transform.up = (_firePointPosition - position).normalized;
        }

        public void RevertToFirePoint()
        {
            _currentBubble = null;
            var startPosition = (Vector2)_leftStrip.GetPosition(0);

            StartCoroutine(RevertCoroutine(startPosition));
        }

        private IEnumerator RevertCoroutine(Vector2 from)
        {
            var targetPosition = _firePointPosition;
            var currentPos = from;
            var duration = 1f;
            var elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime * _elasticSpeed;
                
                currentPos = Vector2.LerpUnclamped(currentPos, targetPosition,
                    _revertCurve.Evaluate(elapsedTime / duration));
                
                SetLines(currentPos);

                yield return null;
            }

            SetLines(targetPosition);
        }
    }
}