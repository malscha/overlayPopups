using Overlay.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Overlay.Render
{
    public class WpfRenderer : IRenderer
    {
        private Dictionary<string, DispatcherTimer> _strobingTimers = new Dictionary<string, DispatcherTimer>();

        public void RenderCard(Card card, Canvas canvas)
        {
            switch (card.Type)
            {
                case CardType.Text:
                    RenderTextCard(card, canvas);
                    break;
                case CardType.Image:
                    RenderImageCard(card, canvas);
                    break;
            }
        }

        public void RenderScene(Scene scene, Configuration config, Canvas canvas)
        {
            ClearCanvas(canvas);

            foreach (var cardId in scene.CardIds)
            {
                var card = config.Cards.FirstOrDefault(c => c.Id == cardId);
                if (card != null)
                {
                    RenderCard(card, canvas);
                }
            }
        }

        public void ClearCanvas(Canvas canvas)
        {
            // Stop all strobing timers
            foreach (var timer in _strobingTimers.Values)
            {
                timer.Stop();
            }
            _strobingTimers.Clear();

            canvas.Children.Clear();
        }

        private void RenderTextCard(Card card, Canvas canvas)
        {
            // Create a grid to hold our text with optional background
            var grid = new Grid();

            // Create a rectangle for the background if specified
            if (!string.IsNullOrEmpty(card.BackgroundColor))
            {
                try
                {
                    var background = new Rectangle
                    {
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(card.BackgroundColor)),
                        Opacity = card.Opacity
                    };
                    grid.Children.Add(background);
                }
                catch
                {
                    // If color parsing fails, continue without background
                }
            }

            // Create the text block
            var textBlock = new TextBlock
            {
                Text = card.Text ?? string.Empty,
                FontFamily = new FontFamily(card.Font?.Family ?? "Arial"),
                FontSize = card.Font?.Size ?? 72,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(card.ForegroundColor ?? "#FFFFFF")),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = ConvertTextAlignment(card.TextAlignment),
                TextWrapping = TextWrapping.NoWrap
            };

            // Add drop shadow effect if specified
            if (!string.IsNullOrEmpty(card.DropShadowColor))
            {
                var effect = new DropShadowEffect
                {
                    Color = (Color)ColorConverter.ConvertFromString(card.DropShadowColor),
                    Direction = 315,
                    ShadowDepth = 0, // No depth for a more even shadow
                    BlurRadius = 15,
                    Opacity = 1.0
                };
                textBlock.Effect = effect;
            }
            else
            {
                // Default drop shadow
                var effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 0,
                    BlurRadius = 15,
                    Opacity = 1.0
                };
                textBlock.Effect = effect;
            }

            // Add text to grid
            grid.Children.Add(textBlock);

            // Set position and size
            Canvas.SetLeft(grid, card.X);
            Canvas.SetTop(grid, card.Y);
            Canvas.SetZIndex(grid, card.ZIndex);

            // Set the grid size
            grid.Width = card.Width;
            grid.Height = card.Height;

            canvas.Children.Add(grid);

            // Handle strobing effect if enabled
            if (card.IsStrobing)
            {
                StartStrobingEffect(textBlock, card.Id);
            }
        }

        private void StartStrobingEffect(TextBlock textBlock, string cardId)
        {
            // Stop any existing timer for this card
            if (_strobingTimers.ContainsKey(cardId))
            {
                _strobingTimers[cardId].Stop();
                _strobingTimers.Remove(cardId);
            }

            // Create a new timer for strobing
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500); // Strobe every 500ms
            bool isBright = false;

            timer.Tick += (sender, e) => {
                isBright = !isBright;
                if (isBright)
                {
                    textBlock.Foreground = new SolidColorBrush(Colors.Purple); // Bright purple
                }
                else
                {
                    textBlock.Foreground = new SolidColorBrush(Colors.White); // White
                }
            };

            timer.Start();
            _strobingTimers[cardId] = timer;
        }

        private void RenderImageCard(Card card, Canvas canvas)
        {
            var image = new Image
            {
                Opacity = card.Opacity,
                Stretch = Stretch.Fill
            };

            if (!string.IsNullOrEmpty(card.ImagePath))
            {
                try
                {
                    // Check if the file exists
                    if (File.Exists(card.ImagePath))
                    {
                        // Use BitmapImage with proper caching and decode settings
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(card.ImagePath, UriKind.RelativeOrAbsolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad; // Cache the image data
                        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bitmap.EndInit();
                        bitmap.Freeze(); // Freeze to improve performance

                        image.Source = bitmap;
                    }
                    else
                    {
                        // If the file doesn't exist, create a placeholder
                        CreatePlaceholderImage(image, card);
                    }
                }
                catch
                {
                    // Handle error by creating a placeholder
                    CreatePlaceholderImage(image, card);
                }
            }
            else
            {
                // If no path is specified, create a placeholder
                CreatePlaceholderImage(image, card);
            }

            image.Width = card.Width;
            image.Height = card.Height;

            Canvas.SetLeft(image, card.X);
            Canvas.SetTop(image, card.Y);
            Canvas.SetZIndex(image, card.ZIndex);

            canvas.Children.Add(image);
        }

        private void CreatePlaceholderImage(Image image, Card card)
        {
            // Create a DrawingImage as a placeholder
            var drawingGroup = new DrawingGroup();

            // Draw a red background
            drawingGroup.Children.Add(new GeometryDrawing(
                new SolidColorBrush(Colors.Red),
                new Pen(new SolidColorBrush(Colors.Yellow), 2),
                new RectangleGeometry(new Rect(0, 0, card.Width, card.Height))
            ));

            // Draw text
            var text = "IMAGE";
            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                24,
                new SolidColorBrush(Colors.White),
                1.0); // dpi
            drawingGroup.Children.Add(new GeometryDrawing(
                new SolidColorBrush(Colors.White),
                null,
                formattedText.BuildGeometry(new Point((card.Width - formattedText.Width) / 2, (card.Height - formattedText.Height) / 2))
            ));

            image.Source = new DrawingImage(drawingGroup);
        }

        private System.Windows.TextAlignment ConvertTextAlignment(Overlay.Core.TextAlignment alignment)
        {
            return alignment switch
            {
                Overlay.Core.TextAlignment.Center => System.Windows.TextAlignment.Center,
                Overlay.Core.TextAlignment.Right => System.Windows.TextAlignment.Right,
                _ => System.Windows.TextAlignment.Left
            };
        }
    }
}