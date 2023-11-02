using System.Collections.Generic;
using System.Reflection.Metadata;
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
        public int EditorMode; 
        public int GridSize;
        public int CursorScale;
        public bool IsGrid;   

        protected string _stage;
        protected GUI _gui;
        protected Vector2 _position;
        protected Vector2 _m1;
        protected Vector2 _m2;
        protected bool _isPeen;
        protected Texture2D _cursor;
        protected Texture2D _grid;
        protected KeysInput _input; 
        protected Heap _gos;         

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
            _gos = Db.GetHeap("gos");
            GOsInfo = new Dictionary<string, Heap>();
            EditorMode = 0;
            GridSize = 32;
            CurGOInfo = "null";
            _cursor = Tools.MakeSolidColorTexture(_graphics, new Point(2, 2), Color.Green);
            CursorScale = GridSize / 2;
            _grid =  new Texture2D(graphicsDevice, 1280, 720);
            ResetGridTexture();

            _input = new KeysInput();
            _input.Add("left", new Key(Keys.Left));
            _input.Add("left", new Key(Keys.A));
            _input.Add("right", new Key(Keys.Right));
            _input.Add("right", new Key(Keys.D));
            _input.Add("up", new Key(Keys.Up));
            _input.Add("up", new Key(Keys.W));
            _input.Add("down", new Key(Keys.Down));
            _input.Add("down", new Key(Keys.S));
            _input.Add("m1", new Key(MouseButtons.Left));
            _input.Add("m2", new Key(MouseButtons.Right));
            
            _gui = new GUI();
            Page page = new Page();
            _gui.AddPage("main", page);
            Label label = new Label(new Rectangle(5, 5, 600, 20), new GUIStyle(_graphics, SConsole.Font), "...");
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
                _gui.GetPage("main").Get(0).SetText("Grid: " + GridSize + "  Mouse position: " + _position + " | " + (_position - new Vector2(GridSize / 2, GridSize / 2))  + " | " + (_position + new Vector2(GridSize / 2, GridSize / 2)) + "  Brush: " + CurGOInfo + "  Layer: " + Layer);

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

                    if (EditorMode == 1)
                    {
                        if (_input.IsPressed("m1"))
                        {                            
                            if (_gos.GetHeap(CurGOInfo).GetString("category") == "prop")
                            {
                                int number = GOsInfo.Count;
                                string name = CurGOInfo + number;
                                int c = 1;
                                while (GOs.ContainsKey(name))
                                {
                                    name += c;
                                    c++;
                                }
                                string type = _gos.GetHeap(CurGOInfo).GetString("type");
                                Heap newgo = new Heap();
                                newgo.Add("category", _gos.GetHeap(CurGOInfo).GetString("category"));
                                newgo.Add("type", type);
                                newgo.Add("material", _gos.GetHeap(CurGOInfo).GetString("material"));
                                newgo.Add("position", _position + Camera.LefUp);
                                newgo.Add("layer", Layer);
                                SMaterial material = Materials[_gos.GetHeap(CurGOInfo).GetString("material")];
                                Sprite sprite = new Sprite(material, Vector2.Zero, material.SourceRectangle.Size);
                                Prop prop = new Prop(_position + Camera.LefUp, sprite, new Collider(new SRectangle(material.SourceRectangle), Vector2.Zero), Layer);
                                prop.Name = name;
                                prop.Type = type;
                                GOs.Add(name, prop);                                
                                GOsInfo.Add(name, newgo);
                            }
                            else
                            {
                                _m1 = _position - new Vector2(GridSize / 2, GridSize / 2) + Camera.LefUp;
                                _isPeen = true;
                            }
                        }
                        if (_input.IsPressed("m2") && _isPeen)
                        {
                            if (_gos.GetHeap(CurGOInfo).GetString("category") == "brush")
                            {
                                _m2 = _position + new Vector2(GridSize / 2, GridSize / 2) + Camera.LefUp;
                                _isPeen = false;
                                int number = GOsInfo.Count;
                                string name = CurGOInfo + number;
                                int c = 1;
                                while (GOs.ContainsKey(name))
                                {
                                    name += c;
                                    c++;
                                }
                                string type = _gos.GetHeap(CurGOInfo).GetString("type");
                                Point location = new Point((int)_m1.X, (int)_m1.Y);
                                Point size = new Point((int)(_m2.X - _m1.X), (int)(_m2.Y - _m1.Y));
                                Heap newgo = new Heap();
                                newgo.Add("category", _gos.GetHeap(CurGOInfo).GetString("category"));
                                newgo.Add("type", type);
                                newgo.Add("material", _gos.GetHeap(CurGOInfo).GetString("material"));
                                newgo.Add("location", location);
                                newgo.Add("size", size);
                                newgo.Add("layer", Layer);
                                SMaterial material = Materials[_gos.GetHeap(CurGOInfo).GetString("material")];
                                Sprite sprite = new Sprite(material, Vector2.Zero, material.SourceRectangle.Size);
                                Brush brush = new Brush(material, new Rectangle(location, size));
                                brush.Name = name;
                                brush.Type = type;
                                brush.Layer = Layer;
                                GOs.Add(name, brush);                                
                                GOsInfo.Add(name, newgo);
                            }
                            else if (_gos.GetHeap(CurGOInfo).GetString("category") == "trigger")
                            {
                                _m2 = _position + new Vector2(GridSize / 2, GridSize / 2) + Camera.LefUp;
                                _isPeen = false;
                                int number = GOsInfo.Count;
                                string name = CurGOInfo + number;
                                int c = 1;
                                while (GOs.ContainsKey(name))
                                {
                                    name += c;
                                    c++;
                                }
                                string type = _gos.GetHeap(CurGOInfo).GetString("type");
                                string colorSource = _gos.GetHeap(CurGOInfo).GetString("color");
                                Point size = new Point((int)(_m2.X - _m1.X), (int)(_m2.Y - _m1.Y));
                                Heap newgo = new Heap();
                                string[] tmp = colorSource.Split(",");                                 
                                newgo.Add("category", _gos.GetHeap(CurGOInfo).GetString("category"));
                                newgo.Add("type", type);
                                newgo.Add("color", colorSource);
                                newgo.Add("position", _m1);
                                newgo.Add("size", size);
                                Trigger trigger = new Trigger(_m1, size);
                                trigger.Name = name;
                                trigger.Type = type;
                                trigger.Color = new Color(int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]));
                                GOs.Add(name, trigger);                                
                                GOsInfo.Add(name, newgo);
                            } 
                        }
                    }
                    else if (EditorMode == 2)
                    {
                        if (_input.IsPressed("m1"))
                        {
                            Rectangle rect = new Rectangle(Mouse.GetState().X + (int)Camera.LefUp.X, Mouse.GetState().Y + (int)Camera.LefUp.Y, 2, 2);
                            foreach(IGameObject go in _updatingGOs)
                            {
                                if (rect.Intersects(go.DrawRect) && go.Layer == Layer)
                                {
                                    string name = go.Name;
                                    GOs.Remove(name);
                                    GOsInfo.Remove(name);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsGrid)
            {
                _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                _spriteBatch.Draw(_grid, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }
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
                    SConsole.WriteLine("Игровых объектов создано:" + GOs.Count + "|" + GOsInfo.Count); 
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

        public void ResetGridTexture()
        {
            Color[] data = new Color[_grid.Width * _grid.Height];
            _grid.SetData(data);
            int w = 1280 / GridSize;
            int h = 720 / GridSize;
            for (int i = 1; i < w; i++)
            {
                int x = i * GridSize;
                Tools.DrawLine(_grid, Color.DarkOrange, new Vector2(x, 5), new Vector2(x, 715));
            }
            for (int i = 1; i < h; i++)
            {
                int y = i * GridSize;
                Tools.DrawLine(_grid, Color.DarkOrange, new Vector2(5, y), new Vector2(1275, y));
            }
        }

        public void Save(string path)
        {
            Heap map = Heap.Open(path);
            Heap gos = new Heap();
            foreach (string k in GOsInfo.Keys)
            {
                gos.Add(k, GOsInfo[k]);
            }
            map.Add("gos", gos);
            map.Save(path);
        }        
    }   
}