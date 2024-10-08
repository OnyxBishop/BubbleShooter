using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class SwitchSceneState : IStateWithPayload<SceneNames>
    {
        public void Enter(SceneNames sceneName) 
            => SceneSwitcher.ChangeToAsync(sceneName);

        public void Enter()
        {
        }

        public void Exit()
        {
        }
    }
}