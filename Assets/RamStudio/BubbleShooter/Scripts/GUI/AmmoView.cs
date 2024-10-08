using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    public class AmmoView : MonoBehaviour
    {
        [SerializeField] private IntValueView _countView;
        [SerializeField] private Image _bubbleImage;

        public IntValueView CountView => _countView;
    
        public void SetSprite(Sprite sprite) 
            => _bubbleImage.sprite = sprite;
    }
}