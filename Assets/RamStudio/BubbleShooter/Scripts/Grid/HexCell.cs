using JetBrains.Annotations;
using RamStudio.BubbleShooter.Scripts.Common.Structs;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts
{
    public class HexCell
    {
        public HexCell(OffsetCoordinates coordinates, Vector2 worldPosition)
        {
            OffsetCoordinates = coordinates;
            WorldPosition = worldPosition;
            Bubble = null;
        }

        [CanBeNull] public Bubble Bubble { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsEmpty => Bubble == null;
        public Vector2 WorldPosition { get; }
        public OffsetCoordinates OffsetCoordinates { get; }
        
        public void SetBubble(Bubble bubble)
        {
            if (Bubble is not null)
                PopBubble();

            bubble.transform.position = WorldPosition;
            Bubble = bubble;
        }

        public void PopBubble()
        {
            Bubble?.OnPop();
            IsConnected = false;
            Bubble = null;
        }

        public void MarkAsConnected()
        {
            if (IsConnected)
                return;

            IsConnected = true;
        }
    }
}