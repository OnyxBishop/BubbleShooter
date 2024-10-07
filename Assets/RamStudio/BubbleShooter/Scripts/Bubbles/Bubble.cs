using System;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animation))]
public class Bubble : MonoBehaviour
{
    private const int MinPoints = 50;
    private const int MaxPoints = 150;
    private const string Pop = nameof(Pop);
    private const string Appear = nameof(Appear);

    [SerializeField] private TMP_Text _offsetText;
    [SerializeField] private AnimationClip _appearClip;
    [SerializeField] private AnimationClip _popClip;
    
    private SpriteRenderer _spriteRenderer;
    private Animation _animation;
    
    public event Action<int> Popped;

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
        var points = Random.Range(MinPoints, MaxPoints);
        Popped?.Invoke(points);
        _animation.CrossFade(Pop);
    }

    public void TextMe(int column, int row)
    {
        _offsetText.text = string.Format($"{column}|{row}");
    }
    
    public void OnAnimationEnded() 
        => gameObject.SetActive(false);
}