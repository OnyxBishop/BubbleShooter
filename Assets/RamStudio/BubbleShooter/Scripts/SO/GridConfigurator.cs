using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.SO
{
    [CreateAssetMenu(menuName = "GridData")]
    public class GridConfigurator : ScriptableObject
    {
        [SerializeField] private GridDataEditor _gridDataEditor;
        [SerializeField] private BubbleColors[] _bubbles;

        public BubbleColors[] Bubbles => _bubbles;

        private void OnValidate()
        {
            InitializeBubbles();
        }

        private void InitializeBubbles()
        {
            if (_gridDataEditor == null)
            {
                _bubbles = null;
                return;
            }

            var rows = _gridDataEditor.MaxRows;
            var columns = _gridDataEditor.MaxColumns;
            var total = rows * columns;
            
            if (_bubbles == null || _bubbles.Length != total)
            {
                _bubbles = new BubbleColors[total];

                for (var i = 0; i < total; i++)
                    _bubbles[i] = BubbleColors.None;
            }
        }
    }
}