using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    public class BackToMenuButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
    
        private void OnButtonClicked()
        {
            SceneSwitcher.ChangeToAsync(SceneNames.Menu);
        }
    }
}
