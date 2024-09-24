using System;
using System.Linq;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RamStudio.BubbleShooter.Scripts
{
    public class BubbleFactory
    {
        private Bubble _prefab;
        private Sprite[] _sprites;

        // public BubbleFactory()
        // {
        //     _prefab = Resources.Load<Bubble>(AssetPaths.Bubble);
        //     _sprites = Resources.LoadAll<Sprite>(AssetPaths.BubbleSprites);
        // }

        public Bubble Create(BubbleColors color)
        {
            _prefab = Resources.Load<Bubble>(AssetPaths.Bubble);
            _sprites = Resources.LoadAll<Sprite>(AssetPaths.BubbleSprites);
            var sprite = _sprites.FirstOrDefault(sprite => sprite.name == color.ToString());

            if (!sprite)
                throw new Exception();

            var instance = Object.Instantiate(_prefab);
            instance.Init(color);
            instance.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

            return instance;
        }
    }
}