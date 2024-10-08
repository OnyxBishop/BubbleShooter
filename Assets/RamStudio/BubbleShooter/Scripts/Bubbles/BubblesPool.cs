using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RamStudio.BubbleShooter.Scripts.Bubbles
{
    public class BubblesPool
    {
        private readonly Bubble _prefab;
        private readonly Transform _container;
        
        private List<Bubble> _bubbles;

        public BubblesPool(Bubble prefab, Transform container, int initialCount)
        {
            _prefab = prefab;
            _container = container;
            
            Fill(initialCount);
        }
        
        public Bubble Get()
        {
            foreach (var bubble in _bubbles.Where(bubble => !bubble.isActiveAndEnabled))
            {
                bubble.gameObject.SetActive(true);
                return bubble;
            }

            var newBubble = Create();
            newBubble.gameObject.SetActive(true);
            return newBubble;
        }
        
        private void Fill(int count)
        {
            _bubbles = new List<Bubble>(count);
            
            for (var i = 0; i < count; i++)
                Create();
        }
        
        private Bubble Create()
        {
            var bubble = Object.Instantiate(_prefab, _container);
            bubble.gameObject.SetActive(false);
            _bubbles.Add(bubble);

            return bubble;
        }
    }
}