using System;
using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts.Grid;

namespace RamStudio.BubbleShooter.Scripts.Bubbles
{
    public class BubblesStorage : IDisposable
    {
        private readonly HexGrid _hexGrid;
        private List<Bubble> _bubbles = new List<Bubble>();
        
        public BubblesStorage(HexGrid hexGrid)
        {
            _hexGrid = hexGrid;
            _hexGrid.Initialized += OnGridInitialized;
        }

        public int Count => _bubbles.Count;
        
        public void Dispose()
        {
            _hexGrid.Initialized -= OnGridInitialized;
        }
        
        private void OnGridInitialized(IReadOnlyList<Bubble> firstRow)
        {
            _bubbles = new List<Bubble>(firstRow.Count);
            
            foreach (var bubble in firstRow)
            {
                bubble.Popped += OnBubblePopped;
                _bubbles.Add(bubble);
            }
        }
        
        private void OnBubblePopped(Bubble bubble)
        {
            bubble.Popped -= OnBubblePopped;
            _bubbles.Remove(bubble);
        }
    }
}