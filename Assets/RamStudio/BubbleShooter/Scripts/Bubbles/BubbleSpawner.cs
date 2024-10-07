using System;
using System.Collections.Generic;
using System.Linq;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RamStudio.BubbleShooter.Scripts.Bubbles
{
    public class BubbleSpawner : MonoBehaviour
    {
        private BubblesPool _pool;
        private IReadOnlyList<Sprite> _sprites;

        private BubbleColors[] _availableColors = Enum.GetValues(typeof(BubbleColors))
            .Cast<BubbleColors>()
            .Where(color => color != BubbleColors.None)
            .ToArray();

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

        public Bubble GetLaunchBall()
        {
            var bubble = _pool.Get();

            var randomColor = _availableColors[Random.Range(0, _availableColors.Length)];
            bubble.Init(randomColor, GetSprite(randomColor));

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