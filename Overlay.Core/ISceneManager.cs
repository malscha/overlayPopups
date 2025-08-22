using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overlay.Core
{
    public interface ISceneManager
    {
        Configuration LoadConfiguration();
        void SaveConfiguration(Configuration config);
        Scene? GetSceneById(string id);
        Scene? GetCurrentScene();
        void SetCurrentScene(string sceneId);
    }
}