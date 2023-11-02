using System.Text.RegularExpressions;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Solo;
using Solo.Collections;
using Solo.Entities;

namespace MapEditor
{
    public class EditorConsole : SConsoleManager
    {
        public ConsoleOpenEvent ConsoleOpenEvent;
        protected EditorScene _scene;
        protected Heap _db;
        protected Heap _textures;
        protected Heap _sounds;
        protected Heap _materials;
        protected Heap _gos;
        protected string _path;
        public EditorConsole(GraphicsDeviceManager graphics, Camera camera, SpriteFont font, Settings settings, EditorScene scene) : base (graphics, camera, font, settings)
        {
            _scene = scene;
            _db = _scene.Db;   
            _textures = _db.GetHeap("textures");
            _sounds = _db.GetHeap("sounds");
            _materials = _db.GetHeap("materials");
            _gos = _db.GetHeap("gos");
        }

        public override void ParseString(string str)
        {
            string addTexture = @"^\+tex\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+$";
            string addTextureHelp = @"^\+tex$"; 
            string addSound = @"^\+sound\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+$";
            string addSoundHelp = @"^\+sound$";
            string addMaterial5 = @"^\+mat\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+\s\d+\.\d+\s\d+\.\d+\s[a-zA-Z0-9\-_]+$"; 
            string addMaterial4 = @"^\+mat\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+\s\d+\.\d+\s\d+\.\d+$";            
            string addMaterialHelp = @"^\+mat$";
            string addProp = @"^\+prop\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+$";          
            string addPropHelp = @"^\+prop$";
            string addBrush = @"^\+brush\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+$";          
            string addBrushHelp = @"^\+brush$";
            string addTrigger = @"^\+trig\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_]+\s[a-zA-Z0-9\-_,]+$";          
            string addTriggerHelp = @"^\+trig$"; 
            string makeNew = @"^\-new\s[a-zA-Z0-9\-_]+$";
            string makeNewHelp = @"^\-new$";
            string list = @"^\-list$";
            string open = @"^\-open\s[a-zA-Z0-9\-_]+$";
            string openHelp = @"^\-open$";
            string layer1f = @"^\-layer\s1f$";
            string layer = @"^\-layer\s[0-9]$";
            string layerHelp = @"^\-layer$";
            string gridm = @"^\-grid$";
            string gridp = @"^\+grid$";
            string gridon = @"^\-grid\son$";
            string gridoff = @"^\-grid\soff$";
            string pen = @"^\+pen\s[a-zA-Z0-9\-_]+$";
            string penHelp = @"^\+pen$";
            string penoff = @"^\-pen$";
            string del = @"^\-del$";
            string save = @"^\-save$";
                                

            base.ParseString(str);

            if (Regex.IsMatch(str, addTexture))
            {   
                string[] tmp = str.Split(' ');
                _textures.Add(tmp[1], tmp[2]);
                DbSave();
                SConsole.WriteLine("ok"); 
                return;
            }
            if (Regex.IsMatch(str, addTextureHelp))
            {   
                SConsole.WriteLine("+tex <name> <path>");             
                return;
            }
            if (Regex.IsMatch(str, addSound))
            {   
                string[] tmp = str.Split(' ');
                _sounds.Add(tmp[1], tmp[2]);
                DbSave();
                SConsole.WriteLine("ok"); 
                return;
            }
            if (Regex.IsMatch(str, addSoundHelp))
            {   
                SConsole.WriteLine("+sound <name> <path>");             
                return;
            }
            if (Regex.IsMatch(str, addMaterial5))
            {   
                string[] tmp = str.Split(' ');
                _materials.Add(tmp[1], new Heap());
                Heap material = _materials.GetHeap(tmp[1]);
                material.Add("texture", tmp[2]);
                string[] tmp2 = tmp[3].Split('.');
                material.Add("location", new Point(int.Parse(tmp2[0]), int.Parse(tmp2[1])));
                tmp2 = tmp[4].Split('.');
                material.Add("size", new Point(int.Parse(tmp2[0]), int.Parse(tmp2[1])));
                material.Add("sound", tmp[5]);
                DbSave();
                SConsole.WriteLine(material);
                SConsole.WriteLine("ok");  
                return;
            }
            if (Regex.IsMatch(str, addMaterial4))
            {   
                string[] tmp = str.Split(' ');
                _materials.Add(tmp[1], new Heap());
                Heap material = _materials.GetHeap(tmp[1]);
                material.Add("texture", tmp[2]);
                string[] tmp2 = tmp[3].Split('.');
                material.Add("location", new Point(int.Parse(tmp2[0]), int.Parse(tmp2[1])));
                tmp2 = tmp[4].Split('.');
                material.Add("size", new Point(int.Parse(tmp2[0]), int.Parse(tmp2[1])));
                DbSave();
                SConsole.WriteLine(material);
                SConsole.WriteLine("ok");  
                return;
            }
            if (Regex.IsMatch(str, addMaterialHelp))
            {   
                SConsole.WriteLine("+mat <name> <texture> <location<x.y>> <size<x.y>> <sound>");
                SConsole.WriteLine("+mat <name> <texture> <location<x.y>> <size<x.y>>");             
                return;
            }
            if (Regex.IsMatch(str, addProp))
            {   
                string[] tmp = str.Split(' ');
                Heap prop = new Heap();
                prop.Add("categoty", "prop");
                prop.Add("type", tmp[2]);
                prop.Add("material", tmp[3]);
                _gos.Add(tmp[1], prop);
                DbSave();
                SConsole.WriteLine(prop);
                SConsole.WriteLine("ok");  
                return;
            }
            if (Regex.IsMatch(str, addPropHelp))
            {   
                SConsole.WriteLine("+prop <name> <type> <material>");             
                return;
            }
            if (Regex.IsMatch(str, addBrush))
            {   
                string[] tmp = str.Split(' ');
                Heap brush = new Heap();
                brush.Add("categoty", "brush");
                brush.Add("type", tmp[2]);
                brush.Add("material", tmp[3]);
                _gos.Add(tmp[1], brush);
                DbSave();
                SConsole.WriteLine(brush);
                SConsole.WriteLine("ok");  
                return;
            }
            if (Regex.IsMatch(str, addBrushHelp))
            {   
                SConsole.WriteLine("+brush <name> <type> <material>");             
                return;
            }
            if (Regex.IsMatch(str, addTrigger))
            {   
                string[] tmp = str.Split(' ');
                Heap trigger = new Heap();
                trigger.Add("categoty", "trigger");
                trigger.Add("type", tmp[2]);
                trigger.Add("color", tmp[3]);
                _gos.Add(tmp[1], trigger);
                DbSave();
                SConsole.WriteLine(trigger);
                SConsole.WriteLine("ok");  
                return;
            }
            if (Regex.IsMatch(str, addTriggerHelp))
            {   
                SConsole.WriteLine("+trig <name> <type> <color<xx,xx,xx>>");             
                return;
            }
            if (Regex.IsMatch(str, makeNew))
            {  
                string[] tmp = str.Split(' ');
                string path = _scene.MapsDirectory + tmp[1] + ".heap";
                if (File.Exists(path))
                {
                    SConsole.WriteLine("The map \"" + path + "\" already exists: ");
                    return;
                }

                Heap map = new Heap();
                map.Add("name", "new-map");
                map.Add("description", "...");
                map.Add("camera-position", new Vector2(640, 360));
                map.Add("camera-focus", "");
                map.Add("textures", new Heap());
                map.Add("sounds", new Heap());
                map.Add("materials", new Heap());
                map.Add("gos", new Heap());
                map.Save(path);
                SConsole.WriteLine("New map created. Path: " + path);            
                return;
            }
            if (Regex.IsMatch(str, makeNewHelp))
            {   
                SConsole.WriteLine("-new <name>");             
                return;
            }
            if (Regex.IsMatch(str, list))
            {   
                string path = _scene.MapsDirectory;;
                string[] maps = Directory.GetFiles(path, "*.heap");
                foreach(string map in maps)
                {
                    SConsole.WriteLine(Path.GetFileName(map).Replace(".heap", "")); 
                }         
                return;
            }
            if (Regex.IsMatch(str, open))
            {   
                string[] tmp = str.Split(" ");
                string path = _scene.MapsDirectory + tmp[1] + ".heap";
                if (File.Exists(path))
                {
                    SConsole.WriteLine("Trying to open " + path);
                    _path = path;
                    ConsoleOpenEvent?.Invoke(path);         
                    return;
                }
                else
                {                    
                    SConsole.WriteLine("This map doesn't exist");
                    return;
                }
            }
            if (Regex.IsMatch(str, openHelp))
            {   
                SConsole.WriteLine("-open <name>");             
                return;
            }
            if (Regex.IsMatch(str, layer1f))
            {  
                string[] tmp = str.Split(' ');
                _scene.Layer =  1; 
                SConsole.WriteLine("Layer: " + _scene.Layer);             
                return;
            }
            if (Regex.IsMatch(str, layer))
            {  
                string[] tmp = str.Split(' ');
                _scene.Layer =  float.Parse("0," + tmp[1]); 
                SConsole.WriteLine("Layer: " + _scene.Layer);             
                return;
            }
            if (Regex.IsMatch(str, layerHelp))
            {   
                SConsole.WriteLine("-layer <0-9>");
                SConsole.WriteLine("current layer is " + _scene.Layer);              
                return;
            }
            if (Regex.IsMatch(str, gridm))
            {   
                int size = _scene.GridSize / 2;
                _scene.GridSize = size < 9? 8 : size;
                _scene.CursorScale = _scene.GridSize / 2;
                SConsole.WriteLine("Grid: " + _scene.GridSize);
                ((EditorScene)_scene).ResetGridTexture();             
                return;
            }
            if (Regex.IsMatch(str, gridp))
            {   
                int size = _scene.GridSize * 2;
                _scene.GridSize = size > 255? 256 : size;
                _scene.CursorScale = _scene.GridSize / 2;
                SConsole.WriteLine("Grid: " + _scene.GridSize); 
                ((EditorScene)_scene).ResetGridTexture();            
                return;
            }
            if (Regex.IsMatch(str, gridon))
            {   
                ((EditorScene)_scene).IsGrid = true;
                SConsole.WriteLine("Grid: вкл.");             
                return;
            }
            if (Regex.IsMatch(str, gridoff))
            {   
                ((EditorScene)_scene).IsGrid = false;
                SConsole.WriteLine("Grid: выкл.");             
                return;
            }
            if (Regex.IsMatch(str, pen))
            {  
                string[] tmp = str.Split(' ');
                if (_gos.GetHeap(tmp[1]) == null)
                {
                    SConsole.WriteLine("нет такого! Список пресетов:");
                    foreach (string k in _gos.GetHeapKeys())
                    {
                        SConsole.WriteLine(k);
                    }
                }
                else
                {
                    ((EditorScene)_scene).CurGOInfo = tmp[1];
                    ((EditorScene)_scene).EditorMode = 1;
                    SConsole.WriteLine("set pen: " + tmp[1]);
                }              
                return;
            }
            if (Regex.IsMatch(str, penHelp))
            {   
                SConsole.WriteLine("+pen <GO name>"); 
                SConsole.WriteLine("левая кнопка мыши - позиция. правая - размер");            
                return;
            }
            if (Regex.IsMatch(str, penoff))
            {   
                ((EditorScene)_scene).EditorMode = 0;
                SConsole.WriteLine("pen: off");             
                return;
            }
            if (Regex.IsMatch(str, save))
            {   
                ((EditorScene)_scene).Save(_path);
                SConsole.WriteLine(_path + " saved");             
                return;
            }
            if (Regex.IsMatch(str, del))
            {   
                ((EditorScene)_scene).EditorMode = 2;
                SConsole.WriteLine("pen: delete"); 
                SConsole.WriteLine("левая кнопка мыши - удалить");              
                return;
            }
        }

        public override void Help()
        {
            SConsole.WriteLine("-----editor commands list-----");
            SConsole.WriteLine("+tex  |  +sound  |  +material  |  +prop  |  +trig  | +brush");
            SConsole.WriteLine("-new  |  -open  |  -save*  |  -list");
            SConsole.WriteLine("-layer  |");
            SConsole.WriteLine("-grid  | +grid | -grid on  | -grid off");
            SConsole.WriteLine("+pen  | -pen");
            SConsole.WriteLine("-----default commands list-----");
            SConsole.WriteLine("-resolution   |   -resolutiob on/off |   -fullscreen on/off   |   -music            |   -music x.x  ");
            SConsole.WriteLine("-sound        |   -sound x.x         |   -opening on/off      |   -debug on/off     | -god on/off   ");
            SConsole.WriteLine("-reboot       |   -help              |   -log                 |   -reset-settings   | -save-settings");
            SConsole.WriteLine("-sound        |   +sound              ");
        }  

        protected void DbSave()
        {
            _db.Save("db.heap");
        }
    }

    public delegate void ConsoleOpenEvent(string path);
}