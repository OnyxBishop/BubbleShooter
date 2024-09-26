using System;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.Common;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RamStudio.BubbleShooter.Scripts
{
    [Serializable]
    public class PhysicsTrajectoryPrediction
    {
        [SerializeField] private LineRenderer _lineRenderer;
        
        [Range(1,100)]
        [SerializeField] private int _physicSteps;

        [SerializeField] private float _timeStep = 0.1f;
        
        private Bubble _ghostBubble;
        private Rigidbody2D _rigidbody;
        private Rigidbody2D _ghostRb;
        private Transform _point;

        public void Init(Bubble bubble, Transform firePoint)
        {
            _point = firePoint;
            _rigidbody = bubble.GetComponent<Rigidbody2D>();
            _ghostBubble = Object.Instantiate(bubble, _point.transform.position, Quaternion.identity);
            _ghostRb = _ghostBubble.GetComponent<Rigidbody2D>();
            _ghostRb.Sleep();
            _ghostBubble.gameObject.layer = 8;
            _ghostBubble.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        public void PredictTrajectory(Vector3 bubbleVelocity)
        {
            if(!_rigidbody)
                return;
            
            var startPosition = _point.position;
            
            var trajectoryPoints = new List<Vector3>();
            bubbleVelocity.Normalize();
            var randVector3 = new Vector2(bubbleVelocity.x + Random.Range(0.5f, 1f), bubbleVelocity.y + Random.Range(0.5f, 1f));
            _ghostRb.WakeUp();
            _ghostRb.AddForce(randVector3, ForceMode2D.Impulse);

            Physics2D.simulationMode = SimulationMode2D.Script;
            
            for (var i = 0; i < _physicSteps; i++)
            {
                Physics2D.Simulate(_timeStep);
                trajectoryPoints.Add(_ghostRb.position);

                if (_ghostRb.IsTouchingLayers(1 << 7))
                    break;
            }

            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
            
            _ghostRb.velocity = Vector2.zero;
            _ghostRb.position = startPosition;

            _lineRenderer.positionCount = trajectoryPoints.Count;
            _lineRenderer.SetPositions(trajectoryPoints.ToArray());
        }
        
        // private float ComputeForce()
        // {
        //     float force = Mathf.Lerp(0,5, ); //t = max / min отдаление 
        // }
    }
}