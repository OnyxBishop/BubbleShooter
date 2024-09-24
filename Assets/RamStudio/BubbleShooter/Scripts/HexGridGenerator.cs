using System.Collections.Generic;
using RamStudio.BubbleShooter.Scripts;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    //[SerializeField] private Transform[] _bubblePrefabs;
    [SerializeField] private Vector2 _startPosition = Vector2.zero;
    [SerializeField] private List<Bubble> _bubbles = new List<Bubble>();
    
    [SerializeField] private float _radius = 0.5f;
    [SerializeField] private int _red;
    [SerializeField] private int _green;
    [SerializeField] private int _blue;

    private int _sizeX, _sizeY;
    private float _offsetX, _offsetY;
    
    private int[,] _formation;
    private BubbleFactory _factory = new BubbleFactory();
    
    public void Build()
    {
        _formation = new int[,]
        {
            { _red, _red, _red, _red, _red, _red, _red, _red },
            { _red, _green, _green, _green, _green, _green, _red, _red },
            { _red, _green, _blue, _blue, _blue, _green, _red, _red },
            { _red, _green, _green, _green, _green, _green, _red, _red },
            { _red, _red, _red, _red, _red, _red, _red, _red },
        };

        _sizeX = _formation.GetLength(1);
        _sizeY = _formation.GetLength(0);

        var unitLength = _radius / (Mathf.Sqrt(3) / 2);
        
        _offsetX = unitLength * Mathf.Sqrt(3);
        _offsetY = unitLength * 1.5f;

        for (int y = 0; y < _sizeY; y++)
        {
            for (int x = 0; x < _sizeX; x++)
            {
                var bubbleType = _formation[y, x];
                var hexPosition = HexOffset(x, y);
                var position = new Vector3(hexPosition.x, hexPosition.y, 0);
                var newBubble = _factory.Create((BubbleColors)bubbleType);
                
                newBubble.transform.position = position;
                newBubble.transform.SetParent(transform);
                _bubbles.Add(newBubble);
            }
        }
    }

    public void Clear()
    {
        if (_bubbles.Count == 0)
            return;

        for (var i = 0; i < _bubbles.Count; i++)
            DestroyImmediate(_bubbles[i].gameObject);
        
        _bubbles.Clear();
    }

    private Vector2 HexOffset(int x, int y)
    {
        var gridPosition = y % 2 == 0 
            ? new Vector2(x * _offsetX, y * -_offsetY) 
            : new Vector2((x + 0.5f) * _offsetX, y * -_offsetY);
        
        return gridPosition + _startPosition;
    }
}