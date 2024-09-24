using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float FlyPower;
    public BubbleColors Color;

    public void Init(BubbleColors color)
    {
        Color = color;
    }

    public void SetFlyPower(float power)
    {
        FlyPower = power;
    }
}