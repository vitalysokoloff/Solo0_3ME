using Solo;

namespace MapEditor
{
    public class EditorLoader : MapLoader
        {
            public EditorLoader(Scene scene, string path) : base (scene, path) {}

            public override void MakeGameObjects()
            {   
                _scene.Camera.Position = _map.GetVector2("camera-position");
                if (_map.GetString("camera-focus") != "")
                {}
                ((EditorScene)_scene).Name = _map.GetString("name");
                ((EditorScene)_scene).Description = _map.GetString("description");
                SConsole.WriteLine("Name: " + ((EditorScene)_scene).Name);
                SConsole.WriteLine("Description: " + ((EditorScene)_scene).Description);
            }
        }
}
