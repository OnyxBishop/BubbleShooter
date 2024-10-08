using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SO
{
    [CreateAssetMenu(menuName = "LevelCfg")]
    public class LevelConfiguration : ScriptableObject
    {
        [field: SerializeField] [Range(0,40)] public int BubblesLeftWinConditionPercent;
        [field: SerializeField] public int MaxPointsByBubble;
        [field: SerializeField] public int MinPointsByBubble;
        [field: SerializeField] public int BubbleFallSpeed;
        [field: SerializeField] public int BubbleThrowSpeed;
        [field: SerializeField] public int AmmoCount;
    }
}