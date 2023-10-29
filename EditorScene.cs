using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Solo;
using Solo.Collections;
using Solo.Entities;

namespace MapEditor {
    public class EditorScene : Scene
    {
        public Heap Db;
        public string Name;
        public string Description;

        protected string _stage;        

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
        }

        public override void Update(GameTime gameTime)
        {            
            base.Update(gameTime);
            if (_stage == "генерация коллайдеров")
            {
                foreach (string k in GOs.Keys)
                    {
                        GOs[k].Collider.SetTexture(_graphics);
                    }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
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
                    _stage = "загрузка...";
                    break;
                case 0:
                    _stage = "чтение файла...";
                    break;
                case 1:
                    _stage = "загрузка текстур...";
                    break;
                case 2:
                    _stage = "загрузка звуков...";
                    break;
                case 3:
                    _stage = "Генерауия матералов...";
                    break;
                case 4:
                    _stage = "Создание игровых объектов...";
                    break;
                case 5:
                    _stage = "генерация коллайдеров";                    
                    _isContentLoaded = true;
                    SConsole.WriteLine("Done");
                    break;
            }
        }
        public override void WhileLoadDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            _spriteBatch.DrawString(SConsole.Font, _stage, new Vector2(10, 10), Color.White);
            _spriteBatch.End();
        }        
    }    
}