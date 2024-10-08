using DG.Tweening;
using RamStudio.BubbleShooter.Scripts.GUI.Interfaces;
using TMPro;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    public class IntValueView : MonoBehaviour, IValueView<int>
    {
        [SerializeField] private TMP_Text _tmp;

        private int _currentValue;
    
        public void UpdateText(int amount) 
            => AnimateValueChange(amount);
    
        private void AnimateValueChange(int newValue)
        {
            DOTween.Kill(this);
        
            DOTween.To(() => _currentValue, x => 
                        _currentValue = x,
                    newValue, 1.3f)
                .OnUpdate(() => _tmp.text = newValue.ToString())
                .SetEase(Ease.OutQuad)
                .SetId(this)
                .OnComplete(() => _tmp.text = newValue.ToString());
        }
    }
}
