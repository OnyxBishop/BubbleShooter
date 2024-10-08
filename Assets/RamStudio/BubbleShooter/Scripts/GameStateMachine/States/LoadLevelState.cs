using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.Common.Enums;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.Interfaces;
using RamStudio.BubbleShooter.Scripts.Grid;
using RamStudio.BubbleShooter.Scripts.Services;

namespace RamStudio.BubbleShooter.Scripts.GameStateMachine.States
{
    public class LoadLevelState : IStateWithPayload<string>
    {
        private readonly StateMachine _stateMachine;
        private readonly SaveLoadService _saveLoadService;
        private readonly HexGrid _grid;
        private readonly string _levelId;

        public LoadLevelState(StateMachine stateMachine, SaveLoadService saveLoadService, HexGrid grid, string levelId)
        {
            _stateMachine = stateMachine;
            _saveLoadService = saveLoadService;
            _grid = grid;
            _levelId = levelId;
        }

        public void Enter(string levelId)
        {
            var bubbleArray = LoadGrid(levelId);
            _grid.SetBubblesTo(bubbleArray);
            
            _stateMachine.ChangeState<PlayerInputState>();
        }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        private BubbleColors[,] LoadGrid(string id)
        {
            var gridData = _saveLoadService.LoadGrid(id);
            var bubblesArray = gridData.BubblesArray;
            var array2D = bubblesArray.To2dArray(gridData.Columns, gridData.Rows);

            return array2D;
        }
    }
}