using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.GameStateMachine;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.States;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.Services.DataSavers;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour.TrajectoryPrediction;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts
{
    public class CompositionRoot : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private string _levelId;
        [SerializeField] private int _shootCount;

        [Header("Slingshot behaviour")]
        [SerializeField] private Slingshot _slingshot;
        [SerializeField] private Trajectory _trajectory;
        [SerializeField] private SlingshotStripsView _slingshotStripsView;

        [Header("Objects")]
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private GridBuilder _gridBuilder;
        [SerializeField] private BubbleSpawner _spawner;
        [SerializeField] private int _initialPoolCount;

        [Header("GUI")]
        [SerializeField] private ReturnButton _returnButton;
        
        private HexGrid _hexGrid;
        private SlingshotConnector _slingshotConnector;
        private SaveLoadService _saveLoadService;

        private void Awake()
        {
            InitSpawner();
            InitSaveLoadSystem();
            InitBoard();
            InitSlingshot();
            InitGameStateMachine();
        }

        private void InitSpawner()
        {
            var bubblePrefab = Resources.Load<Bubble>(AssetPaths.Bubble);
            var sprites = Resources.LoadAll<Sprite>(AssetPaths.BubbleSprites);
            var pool = new BubblesPool(bubblePrefab, _spawner.transform, _initialPoolCount);

            _spawner.Init(pool, sprites);
        }

        private void InitSaveLoadSystem()
        {
            var serializer = new JsonSerializer();
            var prefsDataService = new PrefsDataService(serializer);
            var fileDataService = new FileDataService(serializer);
            _saveLoadService = new SaveLoadService(fileDataService, prefsDataService);
        }

        private void InitBoard()
        {
            _hexGrid = _gridBuilder.Build(_spawner, _slingshot);
        }

        private void InitSlingshot()
        {
            _slingshotConnector = new SlingshotConnector(_slingshot, _trajectory, _slingshotStripsView);

            _slingshot.Reload(_spawner.GetLaunchBall());
            _trajectory.Init(_inputSystem, _hexGrid);
            _slingshotStripsView.Init(_trajectory.FirePointPosition);
        }

        private void InitGameStateMachine()
        {
            var gameStateMachine = new StateMachine();

            var loadLevelState = new LoadLevelState(gameStateMachine, _saveLoadService, _hexGrid, _levelId);
            var playerInputState = new PlayerInputState(gameStateMachine, _slingshot, _inputSystem);
            var ballInFlightState = new BallFlightState(gameStateMachine, _slingshot, _hexGrid);
            var checkClusterState = new CheckColorClusterState(gameStateMachine, _hexGrid, _slingshot);
            var checkFloatingBubblesState = new CheckFloatingBubblesState(gameStateMachine, _hexGrid);
            var reloadSlingshotState = new ReloadSlingshotState(gameStateMachine, _slingshot, _slingshotConnector,
                _spawner, _shootCount);
            var gameOverState = new GameOverState(gameStateMachine); // add UI
            var levelCompleteState = new LevelCompleteState(gameStateMachine); // add UI

            gameStateMachine.AddStates(loadLevelState, playerInputState, ballInFlightState, checkClusterState,
                checkFloatingBubblesState, reloadSlingshotState, gameOverState, levelCompleteState);

            gameStateMachine.ChangeState<LoadLevelState>();
        }
    }
}