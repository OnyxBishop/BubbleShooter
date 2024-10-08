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

        public AmmoStorage(BubbleSpawner spawner, int initialCount)
        {
            _spawner = spawner;
            _initialCount = initialCount;

            ChooseNextColor();
        }

        public event Action<int, BubbleColors> Changed;
        public bool HasBubbles => _initialCount > 0;
        public BubbleColors CurrentColor { get; private set; }
        public int Count => _initialCount;

        public bool TryGet(out Bubble bubble)
        {
            bubble = null;

            if (!HasBubbles)
                return false;

            bubble = _spawner.Spawn(CurrentColor);
            bubble.gameObject.layer = 6;
            
            _initialCount--;
            ChooseNextColor();

            Changed?.Invoke(_initialCount, CurrentColor);

            return true;
        }

        private void ChooseNextColor()
        {
            CurrentColor = _availableColors[Random.Range(0, _availableColors.Length)];
        }
    }
}