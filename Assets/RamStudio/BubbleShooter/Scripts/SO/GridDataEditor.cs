using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SO
{
    [CreateAssetMenu(menuName = "Config", order = 51)]
    public class GridDataEditor : ScriptableObject
    {
        [Header("Board")]
        [SerializeField] private int _maxRows;
        [SerializeField] private int _maxColumns;

        public int MaxRows => _maxRows;
        public int MaxColumns => _maxColumns;
    }
}