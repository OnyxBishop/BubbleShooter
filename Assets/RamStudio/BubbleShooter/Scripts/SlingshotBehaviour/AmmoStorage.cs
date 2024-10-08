using System;
using System.Linq;
using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using Random = UnityEngine.Random;

namespace RamStudio.BubbleShooter.Scripts.SlingshotBehaviour
{
    public class AmmoStorage
    {
        private readonly BubbleSpawner _spawner;
        
        private readonly BubbleColors[] _availableColors = Enum.GetValues(typeof(BubbleColors))
            .Cast<BubbleColors>()
            .Where(color => color != BubbleColors.None)
            .ToArray();
        
        private int _initialCount;
        private int _currentValue;
        
        public AmmoStorage(BubbleSpawner spawner, int initialCount)
        {
            _spawner = spawner;
            _initialCount = initialCount;
        }

        public event Action<int> Changed;
        public bool HasBubbles => _initialCount > 0;

        public bool TryGet(out Bubble bubble)
        {
            bubble = null;

            if (!HasBubbles)
                return false;

            var randomColor = _availableColors[Random.Range(0, _availableColors.Length)];
            
            bubble = _spawner.Spawn(randomColor);;
            bubble.gameObject.layer = 6;
            _initialCount--;
            
            Changed?.Invoke(_initialCount);
            
            return true;
        }
    }
}