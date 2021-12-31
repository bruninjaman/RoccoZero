using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Zero.Helpers;
using Divine.Zero.Log;

using Overwolf.Controls;
using Overwolf.Controls.EventArgs;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Overwolf.Renderer
{
    internal sealed class MainWindowOld
    {
        private Vector2 ScreenSize;

        private readonly Core.Menu MainMenu;

        private bool IsKeyDown;

        //private bool trigger;

        //private Vector2 posOffset;

        private bool initDone;

        private RectangleF windowRect;
        private Vector2 posOffset;

        private Button close_button;
        private Toggler settings_switcher;
        private string TextureBackground;

        public MainWindowOld(Context context)
        {
            ScreenSize = RendererManager.ScreenSize;
            MainMenu = context.Menu;
            LoadCFG();

            close_button = new Button("panorama/images/control_icons/x_close_png.vtex_c");
            close_button.Click += Close_button_Click;

            settings_switcher = new Toggler("panorama/images/control_icons/gear_png.vtex_c", 0.8f);
            settings_switcher.ValueChanged += Settings_switcher_ValueChanged;

            RendererManager.LoadImageFromAssembly("Overwolf.Frontground", "Overwolf.Resources.frontground.png");

            RendererManager.LoadImageFromAssembly("Overwolf.Default", "Overwolf.Resources.default_background.png");
            RendererManager.LoadImageFromAssembly("Overwolf.Booba", "Overwolf.Resources.booba_background.png");

            var fileNames = new List<string>() { "Default", "Booba" };

            var resourceDir = Path.Combine(Directories.Resources, @"Textures\Overwolf");
            if (!Directory.Exists(resourceDir))
            {
                Directory.CreateDirectory(resourceDir);
            }

            foreach (var file in Directory.GetFiles(Path.Combine(Directories.Resources, @"Textures\Overwolf")))
            {
                var extension = Path.GetExtension(file);
                if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                {
                    continue;
                }
                var fileName = Path.GetFileNameWithoutExtension(file);
                fileName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fileName.ToLower());
                fileNames.Add(fileName);
                RendererManager.LoadImageFromFile($"Overwolf.{fileName}", file);
            }

            MainMenu.OverwolfBackGround = MainMenu.RootMenu.CreateSelector("Select Background", fileNames.ToArray()).SetTooltip("To use a custom background, rename and move the required \".png\" \nfile along the path \"Divine\\Resources\\Textures\\Overwolf\" to get \n\"Divine\\Resources\\Textures\\Overwolf\\background.png\"");

            MainMenu.OverwolfToggleKey.ValueChanged += OverwolfToggleKey_ValueChanged;
            MainMenu.OverwolfWindowSize.ValueChanged += OverwolfWindowSize_ValueChanged;
            MainMenu.OverwolfBackGround.ValueChanged += OverwolfBackGround_ValueChanged;

            //NetworkManager.NetMessageReceived += NetworkManager_NetMessageReceived;
        }

        private void NetworkManager_NetMessageReceived(Divine.Network.EventArgs.NetMessageReceivedEventArgs e)
        {
            if (e.Protobuf.Name == "CNETMsg_Tick" || e.Protobuf.Name == "CSVCMsg_PacketEntities") return;
            Console.WriteLine($"NetMessageReceived: " +
                $"\n\tName:{e.Protobuf.Name}" +
                $"\n\tId:{e.Protobuf.Id}" +
                $"\n\tMessageID:{e.Protobuf.MessageId}" +
                $"\n\tMessage:\n{e.Protobuf.ToJson()}");
        }

        private void Close_button_Click(Button button, Controls.EventArgs.ButtonEventArgs e)
        {
            MainMenu.OverwolfToggleKey.Value = false;
        }

        private void Settings_switcher_ValueChanged(Toggler button, TooglerEventArgs e)
        {
            Console.WriteLine("Миразил, пошёл нахуй!");
        }

        private void OverwolfBackGround_ValueChanged(MenuSelector selector, SelectorEventArgs e)
        {
            TextureBackground = e.NewValue;
        }

        private void OverwolfWindowSize_ValueChanged(MenuSlider slider, SliderEventArgs e)
        {
            if (!initDone)
            {
                return;
            }
            SetDefaultPos(e.NewValue, false);
        }

        private void OverwolfToggleKey_ValueChanged(MenuToggleKey toggleKey, ToggleKeyEventArgs e)
        {
            if (e.Value)
            {
                RendererManager.Draw += RendererManager_Draw;
                InputManager.MouseKeyDown += InputManager_MouseKeyDown;
                InputManager.MouseKeyUp += InputManager_MouseKeyUp;
                InputManager.MouseMove += InputManager_MouseMove;
            }
            else
            {
                RendererManager.Draw -= RendererManager_Draw;
                InputManager.MouseKeyDown -= InputManager_MouseKeyDown;
                InputManager.MouseKeyUp -= InputManager_MouseKeyUp;
                InputManager.MouseMove -= InputManager_MouseMove;
            }
        }

        private void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (!e.Process || !IsKeyDown)
            {
                return;
            }

            windowRect = new RectangleF(e.Position.X - posOffset.X, e.Position.Y - posOffset.Y, windowRect.Width, windowRect.Height);
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left || !e.Process || IsKeyDown)
            {
                return;
            }

            var windowHeaderRect = new RectangleF(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height * 0.13f);
            if (e.Position.IsUnderRectangle(windowHeaderRect))
            {
                posOffset = new Vector2(e.Position.X - windowRect.X, e.Position.Y - windowRect.Y);
                IsKeyDown = true;
            }
        }

        private void InputManager_MouseKeyUp(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left || !IsKeyDown)
            {
                return;
            }

            var windowHeaderRect = new RectangleF(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height * 0.13f);
            if (e.Position.IsUnderRectangle(windowHeaderRect))
            {
                IsKeyDown = false;
                SaveCFG();
            }
        }

        private void RendererManager_Draw()
        {

            RendererManager.DrawImage($"Overwolf.{TextureBackground}", windowRect);
            //RendererManager.DrawFilledRectangle(windowHeaderRect, new Color(0, 0, 0, 127));
            RendererManager.DrawImage("Overwolf.Frontground", windowRect);
            float buttonSize = windowRect.Height * 0.0557f;
            close_button.SetSize(new RectangleF(windowRect.X + windowRect.Width - buttonSize - (windowRect.Height * 0.0079f), windowRect.Y + (windowRect.Height * 0.0078f), buttonSize, buttonSize));
            close_button.Draw();

            settings_switcher.SetRectangle(new RectangleF(windowRect.X + (windowRect.Width - buttonSize * 2) - (windowRect.Height * 0.0157f), windowRect.Y + (windowRect.Height * 0.0078f), buttonSize, buttonSize));
            settings_switcher.Draw();

            float gap = windowRect.Height * 0.0095f;
            float teamGap = (windowRect.Height * 0.076f);
            var firstPos = new Vector2(windowRect.X + (windowRect.Width * 0.0325f), windowRect.Y + (windowRect.Height * 0.1904f));
            var roleSize = windowRect.Height * 0.056f;

            for (int i = 1; i <= 10; i++)
            {

                var roleRect = new RectangleF(firstPos.X, firstPos.Y, roleSize, roleSize);
                //RendererManager.DrawFilledRectangle(roleRect, new Color(0, 0, 0, 127));
                RendererManager.DrawImage("panorama/images/rank_tier_icons/handicap/midlaneicon_psd.vtex_c", roleRect, ImageType.Default, true);

                var heroRect = new RectangleF(roleRect.X + roleRect.Width + (windowRect.Width * 0.0175f), roleRect.Y, roleSize * 1.6f, roleSize);
                //RendererManager.DrawFilledRectangle(heroRect, new Color(0, 0, 0, 127));
                RendererManager.DrawImage(HeroId.npc_dota_hero_axe, heroRect, UnitImageType.Default, true);

                var nameRect = new RectangleF(heroRect.X + heroRect.Width + (windowRect.Width * 0.0059f), heroRect.Y, (windowRect.Width * 0.1f), roleSize);
                //RendererManager.DrawFilledRectangle(nameRect, new Color(0, 0, 0, 127));
                RendererManager.DrawText("RoccoZero", nameRect, Color.White, "Roboto", FontFlags.Left | FontFlags.VerticalCenter, (windowRect.Height * 0.03f));

                var rankRect = new RectangleF(nameRect.X + nameRect.Width + (windowRect.Width * 0.0018f) + (((windowRect.Width * 0.0388f) - roleSize) * 0.5f) - (((roleSize * 1.25f) - roleSize) * 0.5f), heroRect.Y - (((roleSize * 1.25f) - roleSize) * 0.5f), roleSize * 1.25f, roleSize * 1.25f);
                //RendererManager.DrawFilledRectangle(rankRect, new Color(0, 0, 0, 127));
                RendererManager.DrawImage("panorama/images/rank_tier_icons/rank7_psd.vtex_c", rankRect, ImageType.Default, true);
                RendererManager.DrawImage("panorama/images/rank_tier_icons/pip5_psd.vtex_c", rankRect, ImageType.Default, true);

                var totalMatсhesRect = new RectangleF(rankRect.X + rankRect.Width + (windowRect.Width * 0.0131f), heroRect.Y, (windowRect.Width * 0.0629f), roleSize);
                //RendererManager.DrawFilledRectangle(totalMatсhesRect, new Color(0, 0, 0, 127));
                RendererManager.DrawText("1488", totalMatсhesRect, Color.White, "Roboto", FontFlags.Top | FontFlags.Center, (windowRect.Height * 0.024f));
                RendererManager.DrawText("50%", totalMatсhesRect, Color.White, "Roboto", FontFlags.Bottom | FontFlags.Center, (windowRect.Height * 0.018f));

                //lastGameRect = new RectangleF(totalMatсhesRect.X + totalMatсhesRect.Width /*+ (windowRect.Width * 0.0175f)*/, totalMatсhesRect.Y , (windowRect.Width * 0.289f), roleSize);
                //RendererManager.DrawFilledRectangle(lastGameRect, new Color(0, 0, 0, 127));
                var lastGamesWidth = (windowRect.Width * 0.289f) - ((roleSize * 1.6f * 5) + (windowRect.Width * 0.004f * 4));
                var lastGameRect = new RectangleF(totalMatсhesRect.X + totalMatсhesRect.Width + (lastGamesWidth * 0.5f), totalMatсhesRect.Y, roleSize * 1.6f, roleSize);

                for (int k = 1; k <= 5; k++)
                {

                    //RendererManager.DrawFilledRectangle(lastGameRect, new Color(0, 0, 0, 127));
                    RendererManager.DrawImage(HeroId.npc_dota_hero_axe, lastGameRect, UnitImageType.Default, true);
                    lastGameRect.X += lastGameRect.Width + (windowRect.Width * 0.004f);
                }
                lastGameRect = new RectangleF(totalMatсhesRect.X + totalMatсhesRect.Width /*+ (windowRect.Width * 0.0175f)*/, totalMatсhesRect.Y, (windowRect.Width * 0.289f), roleSize);

                var lastMonthTotalMatсhesRect = new RectangleF(lastGameRect.X + lastGameRect.Width, heroRect.Y, (windowRect.Width * 0.0629f), roleSize);
                //RendererManager.DrawFilledRectangle(lastMonthTotalMatсhesRect, new Color(0, 0, 0, 127));
                RendererManager.DrawText("1488", lastMonthTotalMatсhesRect, Color.White, "Roboto", FontFlags.Top | FontFlags.Center, (windowRect.Height * 0.024f));
                RendererManager.DrawText("50%", lastMonthTotalMatсhesRect, Color.White, "Roboto", FontFlags.Bottom | FontFlags.Center, (windowRect.Height * 0.018f));

                var mostPlayedRect = new RectangleF(lastMonthTotalMatсhesRect.X + lastMonthTotalMatсhesRect.Width /*+ (windowRect.Width * 0.0175f)*/, lastMonthTotalMatсhesRect.Y, (windowRect.Width * 0.1557f), roleSize);
                //RendererManager.DrawFilledRectangle(mostPlayedRect, new Color(0, 0, 0, 127));
                var mostPlayedWidth = (windowRect.Width * 0.1557f) - (roleSize * 1.5f * 3);
                mostPlayedRect = new RectangleF(lastMonthTotalMatсhesRect.X + lastMonthTotalMatсhesRect.Width + (mostPlayedWidth * 0.5f), lastMonthTotalMatсhesRect.Y + roleSize * 0.1f, roleSize * 0.8f, roleSize * 0.8f);

                for (int k = 1; k <= 3; k++)
                {

                    RendererManager.DrawFilledRectangle(new RectangleF(mostPlayedRect.X, mostPlayedRect.Y, mostPlayedRect.Width * 1.5f, mostPlayedRect.Height), new Color(0, 0, 0, 127));
                    RendererManager.DrawImage(HeroId.npc_dota_hero_axe, mostPlayedRect, UnitImageType.MiniUnit, true);
                    var mostPlayedStatRect = new RectangleF(mostPlayedRect.X + mostPlayedRect.Width, mostPlayedRect.Y, mostPlayedRect.Width * 0.7f, roleSize);
                    //RendererManager.DrawFilledRectangle(mostPlayedStatRect, new Color(0, 0, 0, 127));
                    var mostPlayedStatTopRect = new RectangleF(mostPlayedStatRect.X, mostPlayedStatRect.Y + (windowRect.Height * 0.008f), mostPlayedStatRect.Width, mostPlayedStatRect.Height * 0.5f);
                    RendererManager.DrawText("1488", mostPlayedStatTopRect, Color.White, "Roboto", FontFlags.Bottom | FontFlags.Center, (windowRect.Height * 0.016f));
                    var mostPlayedStatBotRect = new RectangleF(mostPlayedStatRect.X, mostPlayedStatRect.Y + (mostPlayedStatRect.Height * 0.5f), mostPlayedStatRect.Width, mostPlayedStatRect.Height * 0.5f);
                    RendererManager.DrawText("50%", mostPlayedStatBotRect, Color.White, "Roboto", FontFlags.Top | FontFlags.Center, (windowRect.Height * 0.012f));

                    mostPlayedRect.X += (mostPlayedRect.Width * 1.7f);
                }
                mostPlayedRect = new RectangleF(lastMonthTotalMatсhesRect.X + lastMonthTotalMatсhesRect.Width /*+ (windowRect.Width * 0.0175f)*/, lastMonthTotalMatсhesRect.Y, (windowRect.Width * 0.1557f), roleSize);

                firstPos.Y += (roleSize + gap);
                if (i == 5)
                {
                    firstPos.Y += teamGap;
                }
            }
        }

        private void SetDefaultPos(float scaleValue, bool useDefaultPos)
        {
            var scaling = RendererManager.ScalingNew * (scaleValue * 0.01f);
            var scaledWidth = (1920 * 0.62f * scaling);
            var scaledHeight = (1080 * 0.6f * scaling);
            if (useDefaultPos)
            {
                windowRect = new RectangleF((ScreenSize.X - scaledWidth) * 0.5f, (ScreenSize.Y - scaledHeight) * 0.5f, scaledWidth, scaledHeight);
            }
            else
            {
                windowRect = new RectangleF(windowRect.X, windowRect.Y, scaledWidth, scaledHeight);
            }
        }

        private void SaveCFG()
        {
            try
            {
                var json = JsonSerializer.Serialize<RectangleF>(windowRect, new JsonSerializerOptions() { WriteIndented = true });
                File.WriteAllText(Path.Combine(Directories.Config, @"Plugins\Overwolf\Overwolf.json"), json);
            }
            catch (Exception e)
            {
                LogManager.Error(e);
            }
        }

        private void LoadCFG()
        {
            try
            {
                var cfgDir = Path.Combine(Directories.Config, @"Plugins\Overwolf");
                if (!Directory.Exists(cfgDir))
                {
                    Directory.CreateDirectory(cfgDir);
                }
                var cfgFile = Path.Combine(cfgDir, @"Overwolf.json");
                if (File.Exists(cfgFile))
                {
                    var json = File.ReadAllText(cfgFile);
                    windowRect = JsonSerializer.Deserialize<RectangleF>(json, new JsonSerializerOptions() { WriteIndented = true });
                }
                else
                {
                    SetDefaultPos(MainMenu.OverwolfWindowSize.Value, true);
                    SaveCFG();
                }
                initDone = true;
            }
            catch (Exception e)
            {
                LogManager.Error(e);
            }
        }
    }
}
