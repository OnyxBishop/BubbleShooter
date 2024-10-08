using System;
using JetBrains.Annotations;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common.Structs;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Grid
{
    public class HexCell
    {
        public HexCell(OffsetCoordinates coordinates, Vector2 worldPosition)
        {
            OffsetCoordinates = coordinates;
            WorldPosition = worldPosition;
            Bubble = null;
            Connectedness = 0;
        }

        public event Action<HexCell> Disconnected;

        public byte Connectedness { get; private set; }
        [CanBeNull] public Bubble Bubble { get; private set; }
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
            Bubble = null;
            Connectedness = 0;
        }

        public void ReplaceBubble(Bubble newBubble)
        {
            Bubble?.OnPop();
            Bubble = newBubble;
        }
        
        public void IncreaseConnectedness(byte value = 1) 
            => Connectedness += value;

        public void DecreaseConnectedness()
        {
            if (Connectedness <= 0) 
                return;
            
            Connectedness--;

            if (Connectedness == 0 && !IsEmpty)
                Disconnected?.Invoke(this);
        }
    }
}