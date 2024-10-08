using RamStudio.BubbleShooter.Scripts.Bubbles;
using RamStudio.BubbleShooter.Scripts.Common;
using RamStudio.BubbleShooter.Scripts.GameStateMachine;
using RamStudio.BubbleShooter.Scripts.GameStateMachine.States;
using RamStudio.BubbleShooter.Scripts.Grid;
using RamStudio.BubbleShooter.Scripts.Services;
using RamStudio.BubbleShooter.Scripts.Services.DataSavers;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour;
using RamStudio.BubbleShooter.Scripts.SlingshotBehaviour.TrajectoryPrediction;
using RamStudio.BubbleShooter.Scripts.SO;
using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts
{
    public class CompositionRoot : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private string _levelId;

        [SerializeField] private LevelConfiguration _levelConfiguration;

        [Header("Slingshot behaviour")]
        [SerializeField] private Slingshot _slingshot;

        [SerializeField] private Trajectory _trajectory;
        [SerializeField] private SlingshotStripsView _slingshotStripsView;

        [Header("Objects")]
        [SerializeField] private InputService _inputService;

        [SerializeField] private LevelBuilder _levelBuilder;
        [SerializeField] private BubbleSpawner _spawner;
        [SerializeField] private int _initialPoolCount;

        [Header("GUI")]
        [SerializeField] private GameplayHUD _gameplayHUD;

        private HexGrid _hexGrid;
        private BubblesStorage _bubblesStorage;

        private ScoreStorage _scoreStorage;
        private StoragePresenter _storagePresenter;
        private ValueView _valueView;

        private SlingshotConnector _slingshotConnector;
        private AmmoStorage _ammoStorage;
        private SaveLoadService _saveLoadService;

        private void Awake()
        {
            InitSpawner();
            InitSaveLoadSystem();
            InitBoard();
            InitSlingshot();
            CreateGUI();
            InitStorage();
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
            _hexGrid = _levelBuilder.Build(_spawner);
            _bubblesStorage = new BubblesStorage(_hexGrid);
        }

        private void InitSlingshot()
        {
            _slingshotConnector = new SlingshotConnector(_slingshot, _trajectory, _slingshotStripsView);

            _ammoStorage = new AmmoStorage(_spawner, _levelConfiguration.AmmoCount);
            _ammoStorage.TryGet(out var bubble);

            _slingshot.Reload(bubble);
            _trajectory.Init(_inputService, _hexGrid);
            _slingshotStripsView.Init(_trajectory.FirePointPosition);

            var footerPoint = GetTopCenterWorldPosition(_gameplayHUD.Footer);
            var slingshotPosition = (Vector2)footerPoint + Vector2.up;

            _slingshot.transform.position = slingshotPosition;
        }

        private void InitStorage()
        {
            _scoreStorage = new ScoreStorage(_saveLoadService);
            _storagePresenter = new StoragePresenter(_scoreStorage, _valueView);
        }

        private void InitGameStateMachine()
        {
            var gameStateMachine = new StateMachine();

            var loadLevelState = new LoadLevelState(gameStateMachine, _saveLoadService, _hexGrid, _levelId);
            var playerInputState = new PlayerInputState(gameStateMachine, _slingshot, _inputService);
            var ballInFlightState = new BallFlightState(gameStateMachine, _slingshot, _hexGrid);
            var checkClusterState = new CheckColorClusterState(gameStateMachine, _hexGrid, _slingshot, _scoreStorage,
                _levelConfiguration);
            var reloadSlingshotState =
                new ReloadSlingshotState(gameStateMachine, _slingshot, _slingshotConnector, _ammoStorage);
            var checkEndConditionState =
                new CheckEndConditionState(gameStateMachine, _levelConfiguration, _bubblesStorage, _ammoStorage);
            var gameEndState = new GameEndState(gameStateMachine, _gameplayHUD, _scoreStorage, _storagePresenter);

            gameStateMachine.AddStates(loadLevelState, playerInputState, ballInFlightState, checkClusterState,
                reloadSlingshotState, checkEndConditionState, gameEndState);

            gameStateMachine.ChangeState<LoadLevelState, string>(_levelId);
        }

        private void CreateGUI()
        {
            SetWalls(_hexGrid.Bounds.Left, _hexGrid.Bounds.Right, _gameplayHUD, out var rightWallRect);
            SetScoreBoard(rightWallRect);
        }

        private void SetScoreBoard(RectTransform rightWallRect)
        {
            var prefab = Resources.Load<ValueView>(AssetPaths.ValueView);
            var offset = new Vector2(25f, -50);

            _valueView = Instantiate(prefab, rightWallRect, false);
            _valueView.GetComponent<RectTransform>().anchoredPosition = offset;
        }

        private void SetWalls(Vector2 leftWallPosition, Vector2 rightWallPosition, GameplayHUD hud,
            out RectTransform rightWallRect)
        {
            var camera = Camera.main;
            var wallPrefab = Resources.Load<GameObject>(AssetPaths.Wall);

            Vector3 screenPointLeft = RectTransformUtility.WorldToScreenPoint(camera, leftWallPosition);
            Vector3 screenPointRight = RectTransformUtility.WorldToScreenPoint(camera, rightWallPosition);

            var container = hud.Content;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPointLeft, camera,
                out var leftLocal);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPointRight, camera,
                out var rightWallPos);

            Instantiate(wallPrefab, hud.Content, false).GetComponent<RectTransform>().anchoredPosition = leftLocal;
            rightWallRect = Instantiate(wallPrefab, hud.Content, false).GetComponent<RectTransform>();
            rightWallRect.anchoredPosition = rightWallPos;
        }

        private Vector3 GetTopCenterWorldPosition(RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var topLeft = corners[1];
            var topRight = corners[2];
            var topCenter = (topLeft + topRight) / 2f;

            return topCenter;
        }
    }
}