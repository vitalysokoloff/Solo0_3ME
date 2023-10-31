using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Solo;
using Solo.Collections;
using Solo.Entities;
using Solo.Input;

namespace MapEditor {
    public class EditorScene : Scene
    {
        public Heap Db;
        public Dictionary<string, Heap> GOsInfo;
        public string Name;
        public string Description;
        public float Layer;
        public string CurGOInfo;
        public int EditorMode; // это для (1)добавление игровых объектов или их (2)удаления. 0 - ничего
        public int GridSize;
        public int CursorScale;   

        protected string _stage;
        protected GUI _gui;
        protected Vector2 _position;
        protected Texture2D _cursor;
        protected KeysInput _input;             

        public EditorScene(Settings settings, Camera camera, ContentManager content, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics) : 
            base (settings, camera, content, graphicsDevice, graphics)
        {             
            Db = Heap.Open("db.heap");
            if (Db.GetHeap("textures") == null)
            {
                Db.Add("textures", new Heap());
            }
            if (Db.GetHeap("sounds") == null)
            {
                Db.Add("sounds", new Heap());
            }
            if (Db.GetHeap("materials") == null)
            {
                Db.Add("materials", new Heap());
            }
            if (Db.GetHeap("gos") == null)
            {
                Db.Add("gos", new Heap());
            }
            Db.Save("db.heap");
            _stage = "...";
            GOsInfo = new Dictionary<string, Heap>();
            EditorMode = 0;
            GridSize = 32;
            CurGOInfo = "null";
            _cursor = Tools.MakeSolidColorTexture(_graphics, new Point(2, 2), Color.Green);
            CursorScale = GridSize / 2;

            _input = new KeysInput();
            _input.Add("left", new Key(Keys.Left));
            _input.Add("left", new Key(Keys.A));
            _input.Add("right", new Key(Keys.Right));
            _input.Add("right", new Key(Keys.D));
            _input.Add("up", new Key(Keys.Up));
            _input.Add("up", new Key(Keys.W));
            _input.Add("down", new Key(Keys.Down));
            _input.Add("down", new Key(Keys.S));
            
            _gui = new GUI();
            Page page = new Page();
            _gui.AddPage("main", page);
            Label label = new Label(new Rectangle(5, 5, 400, 20), new GUIStyle(_graphics, SConsole.Font), "...");
            page.Add(label);
            _gui.SetPage("main");
        }

        public override void Update(GameTime gameTime)
        {            
            base.Update(gameTime);
            if (_stage == "генерация коллайдеров...")
            {
                foreach (string k in GOs.Keys)
                    {
                        GOs[k].Collider.SetTexture(_graphics);
                    }
            }
            if (_isContentLoaded)
            {
                _gui.Update(gameTime);
                MouseState state = Mouse.GetState();
                int x = (state.X / GridSize) * GridSize + GridSize / 2;
                int y = (state.Y / GridSize) * GridSize + GridSize / 2;
                _position = new Vector2(x , y );
                _gui.GetPage("main").Get(0).SetText("Grid: " + GridSize + "  Mouse position: " + _position + "  Brush: " + CurGOInfo + "  Layer: " + Layer);

                if (!SConsole.Configs.GetBool("gui-lock"))
                {
                    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Vector2 delta = new Vector2(0, 0);
                    if (_input.IsDown("left"))
                        delta.X -= GridSize;
                    if (_input.IsDown("right"))
                        delta.X += GridSize;
                    if (_input.IsDown("up"))
                        delta.Y -= GridSize;
                    if (_input.IsDown("down"))
                        delta.Y += GridSize;                
                    Camera.Position += delta;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (_isContentLoaded)
            {
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                _gui.Draw(gameTime, _spriteBatch);
                _spriteBatch.Draw(_cursor, _position, new Rectangle(0, 0, 2, 2), Color.White * 0.2f, 0f, new Vector2(1, 1), new Vector2(CursorScale, CursorScale), SpriteEffects.None, 1f);
                _spriteBatch.End();
            }
        }

        public override void ChangingScene(int number)
        {
            ChangeScene?.Invoke(number);
        }

        public void OpenMap(string path)
        {
            _isContentLoaded = false;
            MapLoader loader = new EditorLoader(this, path);
            loader.StageEvent += OnLoad;
            Thread load = new Thread(new ThreadStart(loader.Load));
            load.Start();
            _stage = "загрузка...";
        }

        public override void OnLoad(int number)
        {            
            switch (number)
            {
                default:
                    _stage = "Загрузка...";
                    break;
                case 0:
                    _stage = "Чтение файла...";
                    break;
                case 1:
                    _stage = "Загрузка текстур...";
                    break;
                case 2:
                    _stage = "Загрузка звуков...";
                    break;
                case 3:
                    _stage = "Генерация матералов...";
                    break;
                case 4:
                    _stage = "Создание игровых объектов...";
                    break;
                case 5:
                    SConsole.WriteLine("Игровых объектов создано:" + GOs.Count); 
                    _stage = "Генерация коллайдеров";                                     
                    _isContentLoaded = true;
                    SConsole.WriteLine("Done");
                    break;                
            }
            SConsole.WriteLine(_stage);
        }
        public override void WhileLoadDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            _spriteBatch.DrawString(SConsole.Font, _stage, new Vector2(10, 10), Color.White);
            _spriteBatch.End();
        }        
    }   
}