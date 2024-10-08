using System;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour.TrajectoryPrediction;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SlingshotBehaviour
{
    public class SlingshotConnector : IDisposable
    {
        private readonly Slingshot _slingshot;
        private readonly Trajectory _trajectory;
        private readonly SlingshotStripsView _slingshotStripsView;

        public SlingshotConnector(Slingshot slingshot, Trajectory trajectory, SlingshotStripsView slingshotStripsView)
        {
            _slingshot = slingshot;
            _trajectory = trajectory;
            _slingshotStripsView = slingshotStripsView;

            _slingshot.Reloaded += OnReloaded;
            _trajectory.Aiming += OnAiming;
            _trajectory.HasContacts += OnTrajectoryHasContacts;
        }

        public void Dispose()
        {
            _slingshot.Reloaded -= OnReloaded;
            _trajectory.Aiming -= OnAiming;
            _trajectory.HasContacts -= OnTrajectoryHasContacts;
        }

        private void OnTrajectoryHasContacts(IReadOnlyList<Vector2> trajectory, float force)
        {
            _slingshot.Shoot(trajectory, force);
            _slingshotStripsView.RevertToFirePoint();
        }
        
        private void OnReloaded(Bubble bubble)
        {
            _slingshotStripsView.SetBubble(bubble);
        }

        private void OnAiming(Vector2 position)
        {
            _slingshotStripsView.SetLines(position);
        }
    }
}