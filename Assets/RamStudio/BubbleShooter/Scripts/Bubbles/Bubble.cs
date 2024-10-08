using System;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Bubbles
{
    [RequireComponent(typeof(Animator))]
    public class Bubble : MonoBehaviour
    {
        private readonly int Pop = Animator.StringToHash(nameof(Pop));
        private readonly int Appear = Animator.StringToHash(nameof(Appear));
        
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;

        public event Action<Bubble> Popped; 
        public BubbleColors Color { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Init(BubbleColors color, Sprite sprite)
        {
            Color = color;
            _spriteRenderer.sprite = sprite;
        }

        public void OnPop()
        {
            _animator.SetTrigger(Pop);
            Popped?.Invoke(this);
        }

        public void OnAnimationEnded()
        {
            gameObject.SetActive(false);
            gameObject.transform.localScale = Vector3.one;
        }
    }
}