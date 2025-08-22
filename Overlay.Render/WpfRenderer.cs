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
using System.Windows.Media.Animation;

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
            // Create a canvas to hold our text with optional background
            var textCanvas = new Canvas
            {
                Width = card.Width,
                Height = card.Height
            };

            // Create a rectangle for the background if specified
            if (!string.IsNullOrEmpty(card.BackgroundColor))
            {
                try
                {
                    var background = new Rectangle
                    {
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(card.BackgroundColor)),
                        Opacity = card.Opacity,
                        Width = card.Width,
                        Height = card.Height
                    };
                    textCanvas.Children.Add(background);
                }
                catch
                {
                    // If color parsing fails, continue without background
                }
            }

            // Determine font size based on text size option
            double fontSize = card.TextSize switch
            {
                TextSize.Small => 48,
                TextSize.Large => 96,
                _ => 72 // Medium
            };

            // Create the text block
            var textBlock = new TextBlock
            {
                Text = card.Text ?? string.Empty,
                FontFamily = new FontFamily(card.Font?.Family ?? "Impact"), // Changed to Impact as default
                FontSize = card.Font?.Size ?? fontSize,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(card.ForegroundColor ?? "#FF69B4")), // Pink as default
                TextAlignment = ConvertTextAlignment(card.TextAlignment),
                TextWrapping = TextWrapping.NoWrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Center the text within the canvas
            textBlock.Measure(new Size(card.Width, card.Height));
            textBlock.Arrange(new Rect(0, 0, card.Width, card.Height));
            
            // Set position to center the text block within the canvas
            Canvas.SetLeft(textBlock, (card.Width - textBlock.ActualWidth) / 2);
            Canvas.SetTop(textBlock, (card.Height - textBlock.ActualHeight) / 2);

            // Add outline effect if specified
            if (card.HasOutline)
            {
                // Determine contrasting color (black or white)
                var primaryColor = (Color)ColorConverter.ConvertFromString(card.ForegroundColor ?? "#FF69B4");
                var contrastingColor = IsColorLight(primaryColor) ? Colors.Black : Colors.White;
                
                // For outline effect, we'll create multiple text blocks slightly offset
                // Create 8 points around the text for a thicker outline
                for (int i = 0; i < 8; i++)
                {
                    var outlineTextBlock = new TextBlock
                    {
                        Text = card.Text ?? string.Empty,
                        FontFamily = new FontFamily(card.Font?.Family ?? "Impact"),
                        FontSize = card.Font?.Size ?? fontSize,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(contrastingColor),
                        TextAlignment = ConvertTextAlignment(card.TextAlignment),
                        TextWrapping = TextWrapping.NoWrap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    // Position the outline text blocks around the main text in a circle
                    double angle = (i * 45) * Math.PI / 180; // 8 points, 45 degrees apart
                    double offsetX = Math.Cos(angle) * 3; // 3 pixels offset for thicker outline
                    double offsetY = Math.Sin(angle) * 3;

                    // Center the outline text block and apply offset
                    outlineTextBlock.Measure(new Size(card.Width, card.Height));
                    outlineTextBlock.Arrange(new Rect(0, 0, card.Width, card.Height));
                    
                    Canvas.SetLeft(outlineTextBlock, (card.Width - outlineTextBlock.ActualWidth) / 2 + offsetX);
                    Canvas.SetTop(outlineTextBlock, (card.Height - outlineTextBlock.ActualHeight) / 2 + offsetY);

                    textCanvas.Children.Add(outlineTextBlock);
                }
            }

            // Create effects collection for multiple effects
            var effects = new List<Effect>();
            
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
                effects.Add(effect);
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
                effects.Add(effect);
            }

            // Add glow effect if specified (or if it's the default)
            if (card.HasGlow)
            {
                var glowEffect = new DropShadowEffect
                {
                    Color = (Color)ColorConverter.ConvertFromString(card.ForegroundColor ?? "#FF69B4"),
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = 25,
                    Opacity = 0.7
                };
                effects.Add(glowEffect);
            }

            // Apply effects (combine if multiple)
            if (effects.Count == 1)
            {
                textBlock.Effect = effects[0];
            }
            else if (effects.Count > 1)
            {
                // For multiple effects, we'll need to use a different approach
                // In WPF, we can't directly combine effects, so we'll prioritize glow over drop shadow
                textBlock.Effect = effects.FirstOrDefault(e => e is DropShadowEffect && 
                    ((DropShadowEffect)e).BlurRadius > 15) ?? effects[0];
            }

            // Add text to canvas (main text block added last so it's on top)
            textCanvas.Children.Add(textBlock);

            // Set position and size
            Canvas.SetLeft(textCanvas, card.X);
            Canvas.SetTop(textCanvas, card.Y);
            Canvas.SetZIndex(textCanvas, card.ZIndex);

            canvas.Children.Add(textCanvas);

            // Handle zoom animation if enabled
            if (card.UseZoomAnimation)
            {
                ApplyZoomAnimation(textCanvas);
            }

            // Handle pulsating effect if enabled
            if (card.IsPulsating)
            {
                ApplyPulsatingEffect(textBlock, card);
            }

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

        private bool IsColorLight(Color color)
        {
            // Calculate luminance using the formula: (0.299*R + 0.587*G + 0.114*B) / 255
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5;
        }

        private void ApplyZoomAnimation(Canvas targetCanvas)
        {
            // Create a scale transform for the zoom effect
            var scaleTransform = new ScaleTransform(0.1, 0.1); // Start at 10% size
            targetCanvas.RenderTransform = scaleTransform;
            targetCanvas.RenderTransformOrigin = new Point(0.5, 0.5); // Scale from center

            // Create animation for scaling
            var scaleXAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0.1,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)), // 500ms zoom
                EasingFunction = new System.Windows.Media.Animation.CubicEase()
            };

            var scaleYAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0.1,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new System.Windows.Media.Animation.CubicEase()
            };

            // Start the animation
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
        }

        private void ApplyPulsatingEffect(TextBlock textBlock, Card card)
        {
            // Get the pulse speed (default to medium if not specified)
            double speed = card.PulseSpeed?.ToLower() switch
            {
                "fast" => 0.5,    // 0.5 seconds per cycle
                "slow" => 2.0,    // 2 seconds per cycle
                _ => 1.0          // 1 second per cycle (medium)
            };

            // Create a scale transform for the pulsating size effect
            var scaleTransform = new ScaleTransform(1.0, 1.0);
            textBlock.RenderTransform = scaleTransform;
            textBlock.RenderTransformOrigin = new Point(0.5, 0.5);

            // Create storyboard for pulsating animation
            var storyboard = new Storyboard();

            // Scale X animation
            var scaleXAnimation = new System.Windows.Media.Animation.DoubleAnimationUsingKeyFrames();
            scaleXAnimation.Duration = new Duration(TimeSpan.FromSeconds(speed));
            scaleXAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Scale Y animation
            var scaleYAnimation = new System.Windows.Media.Animation.DoubleAnimationUsingKeyFrames();
            scaleYAnimation.Duration = new Duration(TimeSpan.FromSeconds(speed));
            scaleYAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Glow effect animation (if glow is enabled)
            DropShadowEffect? glowEffect = null;
            System.Windows.Media.Animation.DoubleAnimation? glowAnimation = null;

            if (card.HasGlow)
            {
                // Get or create glow effect
                glowEffect = textBlock.Effect as DropShadowEffect ?? new DropShadowEffect
                {
                    Color = (Color)ColorConverter.ConvertFromString(card.ForegroundColor ?? "#FFFFFF"),
                    Direction = 0,
                    ShadowDepth = 0,
                    BlurRadius = 25,
                    Opacity = 0.7
                };

                if (textBlock.Effect == null)
                    textBlock.Effect = glowEffect;

                // Create glow animation
                glowAnimation = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 0.3,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(speed)),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
            }

            // Add keyframes for smooth sine wave pulsing
            for (int i = 0; i <= 10; i++)
            {
                double progress = (double)i / 10;
                double sineValue = Math.Sin(progress * 2 * Math.PI - Math.PI / 2); // Start at minimum
                double scale = 1.0 + (sineValue * 0.2); // Scale between 0.8 and 1.2

                var keyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(progress * speed));

                scaleXAnimation.KeyFrames.Add(new System.Windows.Media.Animation.LinearDoubleKeyFrame(scale, keyTime));
                scaleYAnimation.KeyFrames.Add(new System.Windows.Media.Animation.LinearDoubleKeyFrame(scale, keyTime));
            }

            // Set up storyboard
            Storyboard.SetTarget(scaleXAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

            Storyboard.SetTarget(scaleYAnimation, scaleTransform);
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

            storyboard.Children.Add(scaleXAnimation);
            storyboard.Children.Add(scaleYAnimation);

            // Start the animations
            storyboard.Begin();

            // Start glow animation if applicable
            if (glowAnimation != null && glowEffect != null)
            {
                glowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, glowAnimation);
            }
        }
    }
}