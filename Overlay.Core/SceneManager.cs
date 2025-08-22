using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Overlay.Core
{
    public class SceneManager : ISceneManager
    {
        private Configuration _config = new Configuration();
        private string _currentSceneId = string.Empty;
        private readonly string _configPath;

        public SceneManager()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var overlayPath = Path.Combine(appDataPath, "OverlayApp");
            Directory.CreateDirectory(overlayPath);
            _configPath = Path.Combine(overlayPath, "config.json");

            // Load existing config or create default
            LoadConfiguration();
        }

        public Configuration LoadConfiguration()
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    var json = File.ReadAllText(_configPath);
                    var config = JsonSerializer.Deserialize<Configuration>(json);
                    if (config != null)
                    {
                        _config = config;
                    }
                }
                catch
                {
                    // If there's an error loading the config, use default
                    _config = new Configuration();
                }
            }
            else
            {
                // Create default config
                _config = CreateDefaultConfiguration();
                SaveConfiguration(_config);
            }

            return _config;
        }

        public void SaveConfiguration(Configuration config)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(_configPath, json);
            }
            catch
            {
                // Handle error in a real implementation
            }
        }

        public Scene? GetSceneById(string id)
        {
            return _config.Scenes.FirstOrDefault(s => s.Id == id);
        }

        public Scene? GetCurrentScene()
        {
            if (string.IsNullOrEmpty(_currentSceneId))
            {
                // Return first scene if no current scene is set
                return _config.Scenes.FirstOrDefault();
            }

            return GetSceneById(_currentSceneId);
        }

        public void SetCurrentScene(string sceneId)
        {
            _currentSceneId = sceneId;
        }

        private Configuration CreateDefaultConfiguration()
        {
            var config = new Configuration();

            // Add a more visible and styled text card - much larger now
            var textCard = new Card
            {
                Id = "card1",
                Type = CardType.Text,
                X = 100,
                Y = 100,
                Width = 1000,
                Height = 300,
                Opacity = 0.9,
                Text = "OVERLAY ACTIVE!",
                ForegroundColor = "#FFFFFF", // White text
                BackgroundColor = "#FF0000", // Red background
                OutlineColor = "#000000", // Black outline
                OutlineThickness = 4,
                TextAlignment = TextAlignment.Center
                // Note: We're setting font properties directly in the renderer now
            };

            var imageCard = new Card
            {
                Id = "card2",
                Type = CardType.Image,
                X = 300,
                Y = 450,
                Width = 500,
                Height = 500,
                Opacity = 0.8,
                ImagePath = "Assets/sample.png"
            };

            config.Cards.Add(textCard);
            config.Cards.Add(imageCard);

            // Add sample scene
            var scene = new Scene
            {
                Id = "scene1",
                Name = "Default Scene",
                CardIds = new List<string> { "card1", "card2" }
            };

            config.Scenes.Add(scene);

            return config;
        }
    }
}