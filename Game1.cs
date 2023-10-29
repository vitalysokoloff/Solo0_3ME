using Microsoft.Xna.Framework;
using Solo;
using Solo.Entities;

namespace MapEditor {
    public class SGame1 : SGame
    {
        public SGame1() : base(){ }

        protected override void Initialize()
        {
            base.Initialize();            
            _camera = new Camera(_graphics);
            _settings.Reset(_graphics, _camera);
            _camera.Position = new Vector2(300, 300);
            _currentScene = new EditorScene(_settings, _camera, Content, GraphicsDevice, _graphics);
            _console = new EditorConsole(_graphics, _camera, _font, _settings, (EditorScene)_currentScene);
            ((EditorConsole)_console).ConsoleOpenEvent += ((EditorScene)_currentScene).OpenMap;
        }

        protected override void LoadContent()
        {
            base.LoadContent();         
        }

        protected override void Update(GameTime gameTime)
        {  
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected void ChangingScenes(int value)
        {
            switch (value)
            {
                case -1:
                    Exit();
                    break;
                case 0:
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }    
    }
}