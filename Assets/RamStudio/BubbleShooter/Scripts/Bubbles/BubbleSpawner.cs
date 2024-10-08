using System;
using System.Collections.Generic;
using System.Linq;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Bubbles
{
    public class BubbleSpawner : MonoBehaviour
    {
        private BubblesPool _pool;
        private IReadOnlyList<Sprite> _sprites;
        
        public void Init(BubblesPool pool, IReadOnlyList<Sprite> sprites)
        {
            _pool = pool;
            _sprites = sprites;
        }

        public Bubble Spawn(BubbleColors color)
        {
            var bubble = _pool.Get();
            bubble.Init(color, GetSprite(color));

            return bubble;
        }
        
        private Sprite GetSprite(BubbleColors color)
        {
            var sprite = _sprites.FirstOrDefault(sprite => sprite.name == color.ToString());

            if (!sprite)
                throw new Exception($"Cannot found sprite for color {color.ToString()}");

            return sprite;
        }
    }
}