using UnityEngine;
using UnityEngine.UI;

namespace RamStudio.BubbleShooter.Scripts.GUI
{
    public class MenuButtons : MonoBehaviour
    {
        [SerializeField] private Button _startGame;
        [SerializeField] private Button _aboutGame;
        [SerializeField] private Button _closeGame;
        
        private void OnEnable()
        {
            _startGame.onClick.AddListener(OnButtonClicked);
            _aboutGame.onClick.AddListener(OnButtonClicked);
            _closeGame.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _startGame.onClick.RemoveListener(OnButtonClicked);
            _aboutGame.onClick.RemoveListener(OnButtonClicked);
            _closeGame.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            
        }
    }
}