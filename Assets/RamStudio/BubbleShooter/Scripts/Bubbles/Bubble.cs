using System;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Bubbles
{
    [RequireComponent(typeof(Animation))]
    public class Bubble : MonoBehaviour
    {
        private const string Pop = nameof(Pop);
        private const string Appear = nameof(Appear);

        [SerializeField] private AnimationClip _appearClip;
        [SerializeField] private AnimationClip _popClip;
    
        private SpriteRenderer _spriteRenderer;
        private Animation _animation;

        public event Action<Bubble> Popped; 
        public BubbleColors Color { get; private set; }

        private void Awake()
        {
            _animation = GetComponent<Animation>();
            _animation.AddClip(_appearClip, Appear);
            _animation.AddClip(_popClip, Pop);
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    
        public void Init(BubbleColors color, Sprite sprite)
        {
            Color = color;
            _spriteRenderer.sprite = sprite;
            _animation.CrossFade(Appear);
        }

        public void OnPop()
        {
            _animation.CrossFade(Pop);
            Popped?.Invoke(this);
        }

        public void OnAnimationEnded() 
            => gameObject.SetActive(false);
    }
}