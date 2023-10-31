using Microsoft.Xna.Framework;
using Solo;
using Solo.Collections;
using Solo.Entities;

namespace MapEditor
{
    public class EditorLoader : MapLoader
        {
            public EditorLoader(Scene scene, string path) : base (scene, path) {}

            public override void MakeGameObjects()
            {   
                _scene.Camera.Position = _map.GetVector2("camera-position");                
                ((EditorScene)_scene).Name = _map.GetString("name");
                ((EditorScene)_scene).Description = _map.GetString("description");
                SConsole.WriteLine("Name: " + ((EditorScene)_scene).Name);
                SConsole.WriteLine("Description: " + ((EditorScene)_scene).Description);
                SConsole.WriteLine("Создание игровых объектов...");
                Heap gos = _map.GetHeap("gos");
                foreach (string k in gos.GetHeapKeys())
                {
                    Heap go = gos.GetHeap(k);
                    ((EditorScene)_scene).GOsInfo.Add(k, go);
                    if (go.GetString("category") == "prop")
                    {
                        AddGO(k, GetProp(go));                        
                    }
                    if (go.GetString("category") == "trigger")
                    {
                        AddGO(k, GetTrigger(go));                          
                    }
                    if (go.GetString("category") == "brush")
                    {
                        AddGO(k, GetBrush(go));                          
                    }
                }
                if (_map.GetString("camera-focus") != "")
                {
                    _scene.Camera.Focus = _scene.GOs[_map.GetString("camera-focus")];
                }
            }

            protected IGameObject GetProp(Heap go)
            {
                SMaterial material = _scene.Materials[go.GetString("material")];
                Vector2 position = go.GetVector2("position");
                float layer = go.GetFloat("layer");
                Prop prop = new Prop(position, 
                    new Sprite(material, 
                    Vector2.Zero, 
                    material.SourceRectangle.Size), 
                    new Collider(new SRectangle(material.SourceRectangle), Vector2.Zero), 
                    layer
                );
                return prop;
            }

            protected IGameObject GetTrigger(Heap go)
            {                
                Vector2 position = go.GetVector2("position");
                Point size = go.GetPoint("size");
                string colorSource = go.GetString("color");
                Trigger trigger = new Trigger(position, size);
                string[] tmp = colorSource.Split(",");
                trigger.Color = new Color(int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2]));
                return trigger;
            }

            protected IGameObject GetBrush(Heap go)
            {
                SMaterial material = _scene.Materials[go.GetString("material")];
                Point location = go.GetPoint("location");
                Point size = go.GetPoint("size");
                float layer = go.GetFloat("layer");
                Brush brush = new Brush(material, new Rectangle(location, size));
                brush.Layer = layer;
                return brush;
            }
        }
}
