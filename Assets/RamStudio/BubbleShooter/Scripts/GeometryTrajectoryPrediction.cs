using System;
using System.Collections;
using RamStudio.BubbleShooter.Scripts.Common;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RamStudio.BubbleShooter.Scripts
{
    [Serializable]
    public class GeometryTrajectoryPrediction
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LineRenderer _leftRedLine;
        [SerializeField] private LineRenderer _rightRedLine;

        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _leftWallX;
        [SerializeField] private float _rightWallX;
        [SerializeField] private int _lineLenght;
        [SerializeField] private float _offset = 0.5f;

        [SerializeField] private float _randX;
        [SerializeField] private float _randY;

        private Bubble _ghostBubble;
        private Bubble _currentBubble;

        private Rigidbody2D _ghostRb;

        public void Construct()
        {
            var prefab = Resources.Load<Bubble>(AssetPaths.Bubble);
            _ghostBubble = Object.Instantiate(prefab, _firePoint.transform.position, Quaternion.identity);
            _ghostRb = _ghostBubble.GetComponent<Rigidbody2D>();
            _ghostRb.Sleep();
            _ghostBubble.gameObject.layer = 8;
            _ghostBubble.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        public void Predict(Vector3 bubbleVelocity)
        {
            Vector3 gravity = new Vector3(0, -9.81f, 0);
            var currentPosition = _firePoint.position;
            var directionByMagnitude = Mathf.FloorToInt(Mathf.Lerp(200, 120, bubbleVelocity.magnitude / 128));
            
            var magnitude = bubbleVelocity.magnitude;
            var positions = new Vector3[directionByMagnitude];
            positions[0] = _firePoint.position;

            for (var i = 1; i < positions.Length; i++)
            {
                bubbleVelocity += gravity * Time.fixedDeltaTime;
                currentPosition += bubbleVelocity * _offset;

                if (currentPosition.x >= _rightWallX)
                {
                    bubbleVelocity.x = -bubbleVelocity.x;
                    currentPosition.x = _rightWallX;
                }
                else if (currentPosition.x <= _leftWallX)
                {
                    bubbleVelocity.x = -bubbleVelocity.x;
                    currentPosition.x = _leftWallX;
                }

                positions[i] = currentPosition;
            }

            if (magnitude > 125)
            {
                _randX = Random.Range(1f, 2f);
                _randY = Random.Range(0f, 2f);
                _leftRedLine.positionCount = positions.Length;
                _rightRedLine.positionCount = positions.Length;

                var start = positions[4];
                start.x -= _randX;
                _leftRedLine.SetPosition(0, start);

                start.x += _randX * 2;
                _rightRedLine.SetPosition(0, start);

                for (var i = 1; i < positions.Length; i++)
                {
                    Vector3 leftPosition = positions[i];
                    leftPosition.x -= _randX;

                    Vector3 rightPosition = positions[i];
                    rightPosition.x += _randX;

                    if (positions[i].x - _randX < _leftWallX)
                    {
                        leftPosition = _leftRedLine.GetPosition(i - 1);
                        leftPosition.x = _leftWallX - 0.5f;
                    }
                    else if (positions[i].x + _randX > _rightWallX)
                    {
                        rightPosition = _rightRedLine.GetPosition(i - 1);
                        rightPosition.x = _rightWallX + 0.5f;
                    }

                    _leftRedLine.SetPosition(i, leftPosition);
                    _rightRedLine.SetPosition(i, rightPosition);
                }
            }
            else
            {
                _leftRedLine.positionCount = 0;
                _rightRedLine.positionCount = 0;
            }

            _ghostBubble.transform.position = _firePoint.position;
            _lineRenderer.positionCount = positions.Length;
            _lineRenderer.SetPositions(positions);
        }

        private void ApplyPower(Vector3 bubbleVelocity, Vector3[] positions)
        {

        }
    }
}