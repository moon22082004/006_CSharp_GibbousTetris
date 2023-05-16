﻿using SplashKitSDK;
using Timer = SplashKitSDK.Timer;

namespace GibbousTetris
{
    public class GameScene : Scene
    {
        private Button _homeButton;
        private Timer _timer;

        // Duration of time in seconds for each Move Down
        private int _timeOfSteps;

        private Tetromino _currentTetromino;
        private Tetromino _nextTetromino;

        private bool _wasMovedDown;

        public GameScene()
        {
            BlocksController.Instance.Reset();

            _timeOfSteps = (int)GameExecuter.Instance.Level;
            _homeButton = new ButtonCircle(Color.Gray, Color.Black, 50, 50, 20, "Home", Constants.MEDIA_FOLDER_LOCATION + "home.png");

            _timer = new Timer("Game Timer");
            _timer.Start();
            _wasMovedDown = false;

            _currentTetromino = TetrominoFactory.CreateTetromino();
            _nextTetromino = TetrominoFactory.CreateTetromino(17, 1);
        }

        public override void Update()
        {
            if (_homeButton.IsClicked)
            {
                GameExecuter.Instance.ChangeScene(Constants.HOME_SCENE);
            }

            BlocksController.Instance.Update();
            _currentTetromino.Update();

            // Most part of each (_timeOfSteps) second, for manipulate the tetrominno
            if ((_timer.Ticks % (1000 * _timeOfSteps)) <= (1000 * _timeOfSteps - 50))
            {
                _wasMovedDown = false;

                if (SplashKit.KeyTyped(KeyCode.UpKey))
                {
                    _currentTetromino.Rotate();
                }
                else if (SplashKit.KeyTyped(KeyCode.LeftKey)) 
                {
                    _currentTetromino.MoveLeft();
                }
                else if (SplashKit.KeyTyped(KeyCode.RightKey))
                {
                    _currentTetromino.MoveRight();
                }
                else if (SplashKit.KeyTyped(KeyCode.DownKey))
                {
                    _currentTetromino.MoveDown();
                }
                else if (SplashKit.KeyTyped(KeyCode.SpaceKey)) 
                {
                    _currentTetromino.MoveToBottom();
                }
            }
            // The rest of each (_timeOfSteps) second, for notify if the tetromino has been terminated, if not, move down automatedly
            else
            {
                if (!_wasMovedDown)
                {
                    _wasMovedDown = true;
                    _currentTetromino.MoveDown();
                }
            }

            // Process of terminating current tetromino and push next one into the game board
            if (!_currentTetromino.CanMoveDown)
            {
                BlocksController.Instance.TerminateTetromino(_currentTetromino.TheTetromino);
                _currentTetromino = _nextTetromino;
                _currentTetromino.SetPosition(5, 0);
                _nextTetromino = TetrominoFactory.CreateTetromino(17, 1);
            }
        }
        public override void Draw()
        {
            _homeButton.Draw();

            GameBoard.Instance.Draw();
            BlocksController.Instance.Draw();

            // Draw current and next tetrominos
            _currentTetromino.Draw();
            // Highlight the center point of the current tetromino
            SplashKit.FillCircle(Color.White, _currentTetromino.TheTetromino[0].X + (Constants.SIZE_OF_BLOCK / 2), _currentTetromino.TheTetromino[0].Y + (Constants.SIZE_OF_BLOCK / 2), 2);
            _nextTetromino.Draw();
          
            SplashKit.DrawText((_timer.Ticks / 1000).ToString(), Color.Black, 200, 20);
        }
    }
}