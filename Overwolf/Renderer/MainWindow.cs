using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;
using Divine.Zero.Helpers;
using Divine.Zero.Log;

using Overwolf.Controls;
using Overwolf.Controls.EventArgs;
using Overwolf.Core;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

using Menu = Overwolf.Core.Menu;

namespace Overwolf.Renderer
{
    internal sealed class MainWindow
    {
        private readonly Context Context;
        private CoreMain CoreMain;
        private readonly Vector2 ScreenSize;
        private readonly Menu MainMenu;
        private readonly Button close_button;
        private readonly Toggler settings_switcher;
        private string TextureBackground;
        private bool initDone;
        private RectangleF windowRect;
        private RectangleF customRect;
        private Vector2 posOffset;
        private bool IsKeyDown;
        private RectangleF originalRect;
        private RectangleF windowHeaderRect;
        private Dictionary<int, bool> IsNormalized = new Dictionary<int, bool>();
        private Dictionary<int, Dictionary<int, RectangleF>> LastGamesPositions = new Dictionary<int, Dictionary<int, RectangleF>>();

        public MainWindow(Context context)
        {
            Context = context;
            CoreMain = Context.CoreMain;
            ScreenSize = RendererManager.ScreenSize;
            MainMenu = context.Menu;

            LoadCFG();
            //close_button = new Button("panorama/images/control_icons/x_close_png.vtex_c");
            //close_button.Click += Close_button_Click;

            //settings_switcher = new Toggler("panorama/images/control_icons/gear_png.vtex_c", 0.8f);
            //settings_switcher.ValueChanged += Settings_switcher_ValueChanged;

            var fileNames = new List<string>() { "Default 1", "Default 2", "Booba" };

            var resourceDir = Path.Combine(Directories.Resources, @"Textures\Overwolf");
            if (!Directory.Exists(resourceDir))
            {
                Directory.CreateDirectory(resourceDir);
            }

            RendererManager.LoadImageFromAssembly("Overwolf.Logo", "Overwolf.Resources.OW.png");
            RendererManager.LoadImageFromAssembly("Overwolf.Default 1", "Overwolf.Resources.default_bg_1_1080p.png");
            RendererManager.LoadImageFromAssembly("Overwolf.Default 2", "Overwolf.Resources.default_bg_2.png");
            RendererManager.LoadImageFromAssembly("Overwolf.Booba", "Overwolf.Resources.booba_bg.png");
            RendererManager.LoadImageFromAssembly("Overwolf.AddInfoBG", "Overwolf.Resources.add_info_bg.png");
            RendererManager.LoadImage("Overwolf.MatchLose", "panorama/images/status_icons/timer_ring_psd.vtex_c", new ImageProperties { ColorTint = Color.Red }, ImageType.Default);
            RendererManager.LoadImage("Overwolf.MatchWin", "panorama/images/status_icons/timer_ring_psd.vtex_c", new ImageProperties { ColorTint = Color.LimeGreen }, ImageType.Default);

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

            MainMenu.OverwolfBackGround = MainMenu.RootMenu.CreateSelector("Select Background", fileNames.ToArray())
                .SetTooltip("To use a custom background, rename and move the required \".png\"" +
                "\nfile along the path \"Divine\\Resources\\Textures\\Overwolf\" to get" +
                "\n\"Divine\\Resources\\Textures\\Overwolf\\background.png\"");

            MainMenu.OverwolfToggleKey.ValueChanged += OverwolfToggleKey_ValueChanged;
            MainMenu.OverwolfBackGround.ValueChanged += OverwolfBackGround_ValueChanged;
        }

        private void Settings_switcher_ValueChanged(Toggler button, TooglerEventArgs e)
        {
        }

        private void Close_button_Click(Button button, Controls.EventArgs.ButtonEventArgs e)
        {
        }

        private void OverwolfToggleKey_ValueChanged(Divine.Menu.Items.MenuToggleKey toggleKey, ToggleKeyEventArgs e)
        {

            if (e.Value)
            {
                RendererManager.Draw += RendererManager_Draw;
                UpdateManager.Update += UpdateManager_Update;
                InputManager.MouseKeyDown += InputManager_MouseKeyDown;
                InputManager.MouseKeyUp += InputManager_MouseKeyUp;
                InputManager.MouseMove += InputManager_MouseMove;
            }
            else
            {
                RendererManager.Draw -= RendererManager_Draw;
                UpdateManager.Update -= UpdateManager_Update;
                InputManager.MouseKeyDown -= InputManager_MouseKeyDown;
                InputManager.MouseKeyUp -= InputManager_MouseKeyUp;
                InputManager.MouseMove -= InputManager_MouseMove;
            }
        }

        private void UpdateManager_Update()
        {

        }

        private void InputManager_MouseMove(MouseMoveEventArgs e)
        {
            if (!e.Process || !IsKeyDown)
            {
                return;
            }

            windowRect = new RectangleF(e.Position.X - posOffset.X, e.Position.Y - posOffset.Y, windowRect.Width, windowRect.Height);
            customRect = new RectangleF(
                windowRect.X - (windowRect.Width * 0.0225f),
                windowRect.Y - (windowRect.Height * 0.04f),
                windowRect.Width + (windowRect.Width * 0.0450f),
                windowRect.Height + (windowRect.Height * 0.08f));
            windowHeaderRect = new RectangleF(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height * 0.05f);
            LastGamesPositions.Clear();
        }

        private void InputManager_MouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left || !e.Process || IsKeyDown)
            {
                return;
            }

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

            if (e.Position.IsUnderRectangle(windowHeaderRect))
            {
                IsKeyDown = false;
                SaveCFG();
            }
        }

        private void RendererManager_Draw()
        {
            if (!initDone) return;

            //Draw background
            if (TextureBackground == "Default 1")
            {
                RendererManager.DrawImage($"Overwolf.{TextureBackground}", customRect);
            }
            else
            {
                RendererManager.DrawImage($"Overwolf.{TextureBackground}", windowRect);
            }
            //

            //RendererManager.DrawRectangle(windowHeaderRect, Color.Black);

            float gap = windowRect.Height * 0.017f;

            //Logo & Text
            var logoRect = new RectangleF(windowRect.X + (gap * 2f), windowRect.Y + gap, windowRect.Height * 0.034f, windowRect.Height * 0.034f);
            RendererManager.DrawImage(
                "Overwolf.Logo",
                logoRect);

            var overwolfTextRect = new RectangleF(logoRect.X + logoRect.Width + gap, windowRect.Y + gap, windowRect.Width, windowRect.Height);
            RendererManager.DrawText(
                "OVERWOLF DIVINE",
                overwolfTextRect,
                Color.White,
                "Lato",
                windowRect.Height * 0.025f);
            //

            //Radiant Team Text
            var radiantTeamRect = new RectangleF(logoRect.X, logoRect.Y + logoRect.Height + gap, windowRect.Width, windowRect.Height);
            RendererManager.DrawText(
                "Radiant",
                radiantTeamRect,
                Color.LimeGreen,
                "Lato",
                windowRect.Height * 0.025f);
            //

            //Most Played Text
            var prevTextSize = RendererManager.MeasureText("Radiant", "Lato", windowRect.Height * 0.025f);
            var mostPlayedRect = new RectangleF(radiantTeamRect.X + prevTextSize.X + (windowRect.Width * 0.28f), radiantTeamRect.Y, windowRect.Width, windowRect.Height);
            RendererManager.DrawText(
                "Most Played",
                mostPlayedRect,
                Color.White,
                "Lato",
                windowRect.Height * 0.025f);
            //

            //Matches Text
            prevTextSize = RendererManager.MeasureText("Most Played", "Lato", windowRect.Height * 0.025f);
            var matchesRect = new RectangleF(mostPlayedRect.X + prevTextSize.X + (windowRect.Width * 0.07f), radiantTeamRect.Y, windowRect.Width, windowRect.Height);
            RendererManager.DrawText(
                "Matches",
                matchesRect,
                Color.White,
                "Lato",
                windowRect.Height * 0.025f);
            //

            //Win Percent Text
            prevTextSize = RendererManager.MeasureText("Matches", "Lato", windowRect.Height * 0.025f);
            var winPercentRect = new RectangleF(matchesRect.X + prevTextSize.X + (windowRect.Width * 0.04f), radiantTeamRect.Y, windowRect.Width, windowRect.Height);
            RendererManager.DrawText(
                "Win(%)",
                winPercentRect,
                Color.White,
                "Lato",
                windowRect.Height * 0.025f);
            //

            //Last Games Text
            prevTextSize = RendererManager.MeasureText("Win(%)", "Lato", windowRect.Height * 0.025f);
            var lastGamestRect = new RectangleF(winPercentRect.X + prevTextSize.X + (windowRect.Width * 0.157f), radiantTeamRect.Y, windowRect.Width, windowRect.Height);
            RendererManager.DrawText(
                "Last Games",
                lastGamestRect,
                Color.White,
                "Lato",
                windowRect.Height * 0.025f);
            //

            var firstPos = new Vector2(windowRect.X + (gap * 2f), windowRect.Y + (windowRect.Height * 0.094f) + gap);
            var roleSize = windowRect.Height * 0.05f;
            for (int i = 1; i <= 5; i++)
            {
                var containsKey = CoreMain.playerTable.ContainsKey(i - 1);
                //Role Icon
                var roleRect = new RectangleF(firstPos.X, firstPos.Y, roleSize, roleSize);

                if (containsKey)
                {
                    //RendererManager.DrawFilledRectangle(roleRect, new Color(0, 0, 0, 127));
                    var roleModRect = roleRect;
                    var roleImage = $"panorama/images/rank_tier_icons/handicap/{CoreMain.playerTable[i - 1].laneSelectonFlags}icon_psd.vtex_c";
                    if (CoreMain.playerTable[i - 1].laneSelectonFlags == Data.Data.LaneSelectonFlags.Unknown)
                    {
                        roleImage = "panorama/images/control_icons/question_mark_circle_psd.vtex_c";
                        roleModRect = new RectangleF(roleRect.X + ((roleSize * 0.25f) * 0.5f), roleRect.Y + ((roleSize * 0.25f) * 0.5f), roleSize * 0.75f, roleSize * 0.75f);
                    }
                    RendererManager.DrawImage(roleImage, roleModRect, ImageType.Default, true);
                }
                //

                //Party Indicator
                //

                //Rank Icon
                var rankRect = new RectangleF(roleRect.X + roleRect.Width + (gap * 3), roleRect.Y - ((roleSize * 0.25f) * 0.5f), roleSize * 1.25f, roleSize * 1.25f);

                if (containsKey)
                {
                    var rankNum = CoreMain.playerTable[i - 1].rankTier / 10;
                    var pipNum = CoreMain.playerTable[i - 1].rankTier % 10;
                    var rank = $"panorama/images/rank_tier_icons/rank{rankNum}_psd.vtex_c";
                    var pip = $"panorama/images/rank_tier_icons/pip{pipNum}_psd.vtex_c";

                    //RendererManager.DrawFilledRectangle(rankRect, new Color(0, 0, 0, 127));
                    RendererManager.DrawImage(rank, rankRect, ImageType.Default, true);
                    if (pipNum != 0)
                        RendererManager.DrawImage(pip, rankRect, ImageType.Default, true);
                }
                //

                //Player Name
                var nameRect = new RectangleF(rankRect.X + rankRect.Width + gap, firstPos.Y, (windowRect.Width * 0.16f), roleSize);

                if (containsKey)
                {
                    var textSize = RendererManager.MeasureText(CoreMain.playerTable[i - 1].name, "Lato", windowRect.Height * 0.025f);

                    if (textSize.X >= nameRect.Width && (!IsNormalized.ContainsKey(i - 1) || !IsNormalized[i - 1]))
                    {
                        CoreMain.playerTable[i - 1].name = CoreMain.playerTable[i - 1].name.Remove(CoreMain.playerTable[i - 1].name.Length);
                        IsNormalized[i - 1] = false;
                    }
                    else
                    {
                        if (IsNormalized.ContainsKey(i - 1) && IsNormalized[i - 1])
                        {
                            CoreMain.playerTable[i - 1].name = CoreMain.playerTable[i - 1].name.Insert(CoreMain.playerTable[i - 1].name.Length, "...");
                            IsNormalized[i - 1] = true;
                        }
                    }
                    //RendererManager.DrawFilledRectangle(nameRect, new Color(0, 0, 0, 127));
                    RendererManager.DrawText(
                        CoreMain.playerTable[i - 1].name,
                        nameRect,
                        Color.White,
                        "Lato",
                        FontFlags.Left | FontFlags.VerticalCenter,
                        windowRect.Height * 0.025f);
                }
                //

                //Most Played
                var mostPlayedIconRect = new RectangleF(nameRect.X + nameRect.Width + gap, firstPos.Y, roleSize, roleSize);
                var mostPlayedTextRect = new RectangleF(mostPlayedIconRect.X + mostPlayedIconRect.Width, firstPos.Y, roleSize * 1.25f, roleSize);
                for (int k = 1; k <= 3; k++)
                {
                    if (containsKey
                        && !CoreMain.playerTable[i - 1].isAnonymous
                        && CoreMain.playerTable[i - 1].mostPlayed.Count > 0
                        && CoreMain.playerTable[i - 1].mostPlayed.Count >= k)
                    {
                        //RendererManager.DrawFilledRectangle(mostPlayedIconRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawImage(CoreMain.playerTable[i - 1].mostPlayed[k - 1].heroId, mostPlayedIconRect, UnitImageType.MiniUnit, true);
                        //RendererManager.DrawFilledRectangle(mostPlayedTextRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"{CoreMain.playerTable[i - 1].mostPlayed[k - 1].matchCount}",
                            mostPlayedTextRect,
                            Color.White,
                            "Lato",
                            FontFlags.Center | FontFlags.Top,
                            windowRect.Height * 0.02f);
                        RendererManager.DrawText(
                            $"{(uint)Math.Round(((float)CoreMain.playerTable[i - 1].mostPlayed[k - 1].winCount / (float)CoreMain.playerTable[i - 1].mostPlayed[k - 1].matchCount) * 100f)}%",
                            mostPlayedTextRect,
                            Color.White,
                            "Lato",
                            FontFlags.Center | FontFlags.Bottom,
                            windowRect.Height * 0.017f);
                    }
                    if (k != 3)
                    {
                        mostPlayedIconRect.X += mostPlayedIconRect.Width + mostPlayedTextRect.Width;
                        mostPlayedTextRect.X += mostPlayedTextRect.Width + mostPlayedIconRect.Width;
                    }
                }

                if (containsKey && CoreMain.playerTable[i - 1].isAnonymous)
                {
                    var matchesHiddenTextRect = new RectangleF(nameRect.X + nameRect.Width + gap, firstPos.Y, (mostPlayedIconRect.Width + mostPlayedTextRect.Width) * 3, roleSize);
                    RendererManager.DrawText(
                        "Hidden",
                        matchesHiddenTextRect,
                        Color.White,
                        "Lato",
                        FontFlags.Center | FontFlags.VerticalCenter,
                        windowRect.Height * 0.03f);
                }
                //

                //Matches
                var matchesTextRect = new RectangleF(mostPlayedTextRect.X + mostPlayedTextRect.Width + gap, firstPos.Y, (windowRect.Width * 0.058f), roleSize);
                if (containsKey)
                {
                    //RendererManager.DrawFilledRectangle(matchesTextRect, new Color(0, 0, 0, 127));
                    var matchesText = $"{CoreMain.playerTable[i - 1].matchCount}";
                    if (CoreMain.playerTable[i - 1].isAnonymous)
                        matchesText = "Hidden";

                    RendererManager.DrawText(
                        matchesText,
                        matchesTextRect,
                        Color.White,
                        "Lato",
                        FontFlags.Center | FontFlags.VerticalCenter,
                        windowRect.Height * 0.03f);
                }
                //

                //Win Percent
                var winPercentsTextRect = new RectangleF(matchesTextRect.X + matchesTextRect.Width + gap, firstPos.Y, (windowRect.Width * 0.1f), roleSize);
                if (containsKey)
                {
                    //RendererManager.DrawFilledRectangle(winPercentsTextRect, new Color(0, 0, 0, 127));
                    var winPercentsText = $"{CoreMain.playerTable[i - 1].winPercent}%";
                    if (CoreMain.playerTable[i - 1].isAnonymous)
                        winPercentsText = "Hidden";

                    RendererManager.DrawText(
                        winPercentsText,
                        winPercentsTextRect,
                        Color.White,
                        "Lato",
                        FontFlags.Center | FontFlags.VerticalCenter,
                        windowRect.Height * 0.03f);
                }
                //

                //Last Games
                var LastGameIconRect = new RectangleF(winPercentsTextRect.X + winPercentsTextRect.Width + gap + (windowRect.Width * 0.01f), firstPos.Y, roleSize, roleSize);
                for (int k = 1; k <= 8; k++)
                {
                    if (containsKey && CoreMain.playerTable[i - 1].recentMatches.Count >= k)
                    {
                        //RendererManager.DrawFilledRectangle(LastGameIconRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawImage(CoreMain.playerTable[i - 1].recentMatches[k - 1].heroId, LastGameIconRect, UnitImageType.RoundUnit, true);
                        var ringColor = "Overwolf.MatchLose";
                        if (CoreMain.playerTable[i - 1].recentMatches[k - 1].wonMatch)
                            ringColor = "Overwolf.MatchWin";
                        RendererManager.DrawImage(ringColor, LastGameIconRect, ImageType.Default, true);

                        if (!LastGamesPositions.ContainsKey(i - 1))
                            LastGamesPositions.Add(i - 1, new Dictionary<int, RectangleF>());

                        if (!LastGamesPositions[i - 1].ContainsKey(k - 1))
                            LastGamesPositions[i - 1].Add(k - 1, LastGameIconRect);
                    }
                    if (k != 8)
                    {
                        LastGameIconRect.X += mostPlayedIconRect.Width + gap;
                    }
                }
                //

                firstPos.Y += (roleSize + gap);
            }
            firstPos.Y += (windowRect.Height * 0.03f);
            //

            //Dire Team Text
            var direTeamRect = new RectangleF(logoRect.X, firstPos.Y, windowRect.Width, windowRect.Height);
            RendererManager.DrawText(
                "Dire",
                direTeamRect,
                Color.Red,
                "Lato",
                windowRect.Height * 0.025f);
            //

            firstPos.Y += gap + (windowRect.Height * 0.025f);

            for (int i = 6; i <= 10; i++)
            {
                var containsKey = CoreMain.playerTable.ContainsKey(i - 1);
                //Role Icon
                var roleRect = new RectangleF(firstPos.X, firstPos.Y, roleSize, roleSize);

                if (containsKey)
                {
                    //RendererManager.DrawFilledRectangle(roleRect, new Color(0, 0, 0, 127));
                    var roleModRect = roleRect;
                    var roleImage = $"panorama/images/rank_tier_icons/handicap/{CoreMain.playerTable[i - 1].laneSelectonFlags}icon_psd.vtex_c";
                    if (CoreMain.playerTable[i - 1].laneSelectonFlags == Data.Data.LaneSelectonFlags.Unknown)
                    {
                        roleImage = "panorama/images/control_icons/question_mark_circle_psd.vtex_c";
                        roleModRect = new RectangleF(roleRect.X + ((roleSize * 0.25f) * 0.5f), roleRect.Y + ((roleSize * 0.25f) * 0.5f), roleSize * 0.75f, roleSize * 0.75f);
                    }
                    RendererManager.DrawImage(roleImage, roleModRect, ImageType.Default, true);
                }
                //

                //Party Indicator
                //

                //Rank Icon
                var rankRect = new RectangleF(roleRect.X + roleRect.Width + (gap * 3), roleRect.Y - ((roleSize * 0.25f) * 0.5f), roleSize * 1.25f, roleSize * 1.25f);

                if (containsKey)
                {
                    var rankNum = CoreMain.playerTable[i - 1].rankTier / 10;
                    var pipNum = CoreMain.playerTable[i - 1].rankTier % 10;
                    var rank = $"panorama/images/rank_tier_icons/rank{rankNum}_psd.vtex_c";
                    var pip = $"panorama/images/rank_tier_icons/pip{pipNum}_psd.vtex_c";

                    //RendererManager.DrawFilledRectangle(rankRect, new Color(0, 0, 0, 127));
                    RendererManager.DrawImage(rank, rankRect, ImageType.Default, true);
                    if (pipNum != 0)
                        RendererManager.DrawImage(pip, rankRect, ImageType.Default, true);
                }
                //

                //Player Name
                var nameRect = new RectangleF(rankRect.X + rankRect.Width + gap, firstPos.Y, (windowRect.Width * 0.16f), roleSize);

                if (containsKey)
                {
                    var textSize = RendererManager.MeasureText(CoreMain.playerTable[i - 1].name, "Lato", windowRect.Height * 0.025f);

                    if (textSize.X >= nameRect.Width && (!IsNormalized.ContainsKey(i - 1) || !IsNormalized[i - 1]))
                    {
                        CoreMain.playerTable[i - 1].name = CoreMain.playerTable[i - 1].name.Remove(CoreMain.playerTable[i - 1].name.Length);
                        IsNormalized[i - 1] = false;
                    }
                    else
                    {
                        if (IsNormalized.ContainsKey(i - 1) && IsNormalized[i - 1])
                        {
                            CoreMain.playerTable[i - 1].name = CoreMain.playerTable[i - 1].name.Insert(CoreMain.playerTable[i - 1].name.Length, "...");
                            IsNormalized[i - 1] = true;
                        }
                    }
                    //RendererManager.DrawFilledRectangle(nameRect, new Color(0, 0, 0, 127));
                    RendererManager.DrawText(
                        CoreMain.playerTable[i - 1].name,
                        nameRect,
                        Color.White,
                        "Lato",
                        FontFlags.Left | FontFlags.VerticalCenter,
                        windowRect.Height * 0.025f);
                }
                //

                //Most Played
                var mostPlayedIconRect = new RectangleF(nameRect.X + nameRect.Width + gap, firstPos.Y, roleSize, roleSize);
                var mostPlayedTextRect = new RectangleF(mostPlayedIconRect.X + mostPlayedIconRect.Width, firstPos.Y, roleSize * 1.25f, roleSize);
                for (int k = 1; k <= 3; k++)
                {
                    if (containsKey
                        && !CoreMain.playerTable[i - 1].isAnonymous
                        && CoreMain.playerTable[i - 1].mostPlayed.Count > 0
                        && CoreMain.playerTable[i - 1].mostPlayed.Count >= k)
                    {
                        //RendererManager.DrawFilledRectangle(mostPlayedIconRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawImage(CoreMain.playerTable[i - 1].mostPlayed[k - 1].heroId, mostPlayedIconRect, UnitImageType.MiniUnit, true);
                        //RendererManager.DrawFilledRectangle(mostPlayedTextRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"{CoreMain.playerTable[i - 1].mostPlayed[k - 1].matchCount}",
                            mostPlayedTextRect,
                            Color.White,
                            "Lato",
                            FontFlags.Center | FontFlags.Top,
                            windowRect.Height * 0.02f);
                        RendererManager.DrawText(
                            $"{(uint)Math.Round(((float)CoreMain.playerTable[i - 1].mostPlayed[k - 1].winCount / (float)CoreMain.playerTable[i - 1].mostPlayed[k - 1].matchCount) * 100f)}%",
                            mostPlayedTextRect,
                            Color.White,
                            "Lato",
                            FontFlags.Center | FontFlags.Bottom,
                            windowRect.Height * 0.017f);
                    }
                    if (k != 3)
                    {
                        mostPlayedIconRect.X += mostPlayedIconRect.Width + mostPlayedTextRect.Width;
                        mostPlayedTextRect.X += mostPlayedTextRect.Width + mostPlayedIconRect.Width;
                    }
                }

                if (containsKey && CoreMain.playerTable[i - 1].isAnonymous)
                {
                    var matchesHiddenTextRect = new RectangleF(nameRect.X + nameRect.Width + gap, firstPos.Y, (mostPlayedIconRect.Width + mostPlayedTextRect.Width) * 3, roleSize);
                    RendererManager.DrawText(
                        "Hidden",
                        matchesHiddenTextRect,
                        Color.White,
                        "Lato",
                        FontFlags.Center | FontFlags.VerticalCenter,
                        windowRect.Height * 0.03f);
                }
                //

                //Matches
                var matchesTextRect = new RectangleF(mostPlayedTextRect.X + mostPlayedTextRect.Width + gap, firstPos.Y, (windowRect.Width * 0.058f), roleSize);
                if (containsKey)
                {
                    //RendererManager.DrawFilledRectangle(matchesTextRect, new Color(0, 0, 0, 127));
                    var matchesText = $"{CoreMain.playerTable[i - 1].matchCount}";
                    if (CoreMain.playerTable[i - 1].isAnonymous)
                        matchesText = "Hidden";

                    RendererManager.DrawText(
                        matchesText,
                        matchesTextRect,
                        Color.White,
                        "Lato",
                        FontFlags.Center | FontFlags.VerticalCenter,
                        windowRect.Height * 0.03f);
                }
                //

                //Win Percent
                var winPercentsTextRect = new RectangleF(matchesTextRect.X + matchesTextRect.Width + gap, firstPos.Y, (windowRect.Width * 0.1f), roleSize);
                if (containsKey)
                {
                    //RendererManager.DrawFilledRectangle(winPercentsTextRect, new Color(0, 0, 0, 127));
                    var winPercentsText = $"{CoreMain.playerTable[i - 1].winPercent}%";
                    if (CoreMain.playerTable[i - 1].isAnonymous)
                        winPercentsText = "Hidden";

                    RendererManager.DrawText(
                        winPercentsText,
                        winPercentsTextRect,
                        Color.White,
                        "Lato",
                        FontFlags.Center | FontFlags.VerticalCenter,
                        windowRect.Height * 0.03f);
                }
                //

                //Last Games
                var LastGameIconRect = new RectangleF(winPercentsTextRect.X + winPercentsTextRect.Width + gap + (windowRect.Width * 0.01f), firstPos.Y, roleSize, roleSize);
                for (int k = 1; k <= 8; k++)
                {
                    if (containsKey && CoreMain.playerTable[i - 1].recentMatches.Count >= k)
                    {
                        //RendererManager.DrawFilledRectangle(LastGameIconRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawImage(CoreMain.playerTable[i - 1].recentMatches[k - 1].heroId, LastGameIconRect, UnitImageType.RoundUnit, true);
                        var ringColor = "Overwolf.MatchLose";
                        if (CoreMain.playerTable[i - 1].recentMatches[k - 1].wonMatch)
                            ringColor = "Overwolf.MatchWin";
                        RendererManager.DrawImage(ringColor, LastGameIconRect, ImageType.Default, true);

                        if (!LastGamesPositions.ContainsKey(i - 1))
                            LastGamesPositions.Add(i - 1, new Dictionary<int, RectangleF>());

                        if (!LastGamesPositions[i - 1].ContainsKey(k - 1))
                            LastGamesPositions[i - 1].Add(k - 1, LastGameIconRect);
                    }
                    if (k != 8)
                    {
                        LastGameIconRect.X += mostPlayedIconRect.Width + gap;
                    }
                }
                //

                firstPos.Y += (roleSize + gap);
            }

            foreach (var (i, positions) in LastGamesPositions)
            {
                foreach (var (k, LastGameIconRect) in positions)
                {
                    if (GameManager.MouseScreenPosition.IsUnderRectangle(LastGameIconRect))
                    {
                        //Additional Info Background
                        var additionalInfoRect = new RectangleF(
                            LastGameIconRect.X + (LastGameIconRect.Width * 0.5f) - (windowRect.Width * 0.15f),
                            LastGameIconRect.Y + (LastGameIconRect.Width * 0.5f),
                            (windowRect.Width * 0.15f),
                            (windowRect.Width * 0.15f));
                        RendererManager.DrawImage("Overwolf.AddInfoBG", additionalInfoRect);
                        //

                        //Localized Name
                        var localizedName = LocalizationHelper.LocalizeName(CoreMain.playerTable[i].recentMatches[k].heroId);
                        var localizedNameSize = RendererManager.MeasureText(
                            localizedName,
                            "Lato",
                            FontWeight.ExtraBold,
                            (additionalInfoRect.Height * 0.085f));
                        var localizedNameRect = new RectangleF(
                            additionalInfoRect.X + gap,
                            additionalInfoRect.Y + gap,
                            additionalInfoRect.Width - (gap * 2),
                            localizedNameSize.Y);
                        //RendererManager.DrawFilledRectangle(localizedNameRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            localizedName,
                            localizedNameRect,
                            new Color(66, 66, 66),
                            "Lato",
                            FontWeight.ExtraBold,
                            FontFlags.Left | FontFlags.Top,
                            (additionalInfoRect.Height * 0.085f));
                        //

                        //XPM
                        var miniGap = gap * 0.4f;
                        var xpmSize = RendererManager.MeasureText(
                            $"XPM - {CoreMain.playerTable[i].recentMatches[k].XPPerMin}",
                            "Lato",
                            FontWeight.Bold,
                            (additionalInfoRect.Height * 0.075f));
                        var xpmRect = new RectangleF(
                            localizedNameRect.X,
                            localizedNameRect.Y + localizedNameSize.Y + miniGap,
                            localizedNameRect.Width,
                            xpmSize.Y);
                        //RendererManager.DrawFilledRectangle(xpmRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"XPM - {CoreMain.playerTable[i].recentMatches[k].XPPerMin}",
                            xpmRect,
                            new Color(66, 66, 66, 179),
                            "Lato",
                            FontWeight.Bold,
                            FontFlags.Left | FontFlags.Top,
                            (additionalInfoRect.Height * 0.075f));
                        //

                        //GPM
                        var gpmSize = RendererManager.MeasureText(
                            $"GPM - {CoreMain.playerTable[i].recentMatches[k].goldPerMin}",
                            "Lato",
                            FontWeight.Bold,
                            (additionalInfoRect.Height * 0.075f));
                        var gpmRect = new RectangleF(
                            xpmRect.X,
                            xpmRect.Y + xpmSize.Y + miniGap,
                            localizedNameRect.Width,
                            gpmSize.Y);
                        //RendererManager.DrawFilledRectangle(gpmRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"GPM - {CoreMain.playerTable[i].recentMatches[k].goldPerMin}",
                            gpmRect,
                            new Color(66, 66, 66, 179),
                            "Lato",
                            FontWeight.Bold,
                            FontFlags.Left | FontFlags.Top,
                            (additionalInfoRect.Height * 0.075f));
                        //

                        //KDA
                        var kdaSize = RendererManager.MeasureText(
                            $"KDA - {CoreMain.playerTable[i].recentMatches[k].kills}/{CoreMain.playerTable[i].recentMatches[k].deaths}/{CoreMain.playerTable[i].recentMatches[k].assists}",
                            "Lato",
                            FontWeight.Bold,
                            (additionalInfoRect.Height * 0.075f));
                        var kdaRect = new RectangleF(
                            gpmRect.X,
                            gpmRect.Y + gpmSize.Y + miniGap,
                            localizedNameRect.Width,
                            kdaSize.Y);
                        //RendererManager.DrawFilledRectangle(kdaRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"KDA - {CoreMain.playerTable[i].recentMatches[k].kills}/{CoreMain.playerTable[i].recentMatches[k].deaths}/{CoreMain.playerTable[i].recentMatches[k].assists}",
                            kdaRect,
                            new Color(66, 66, 66, 179),
                            "Lato",
                            FontWeight.Bold,
                            FontFlags.Left | FontFlags.Top,
                            (additionalInfoRect.Height * 0.075f));
                        //

                        //Creep Stat
                        var csSize = RendererManager.MeasureText(
                            $"CS - {CoreMain.playerTable[i].recentMatches[k].lastHits}/{CoreMain.playerTable[i].recentMatches[k].denies}",
                            "Lato",
                            FontWeight.Bold,
                            (additionalInfoRect.Height * 0.075f));
                        var csRect = new RectangleF(
                            kdaRect.X,
                            kdaRect.Y + kdaSize.Y + miniGap,
                            localizedNameRect.Width,
                            csSize.Y);
                        //RendererManager.DrawFilledRectangle(csRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"CS - {CoreMain.playerTable[i].recentMatches[k].lastHits}/{CoreMain.playerTable[i].recentMatches[k].denies}",
                            csRect,
                            new Color(66, 66, 66, 179),
                            "Lato",
                            FontWeight.Bold,
                            FontFlags.Left | FontFlags.Top,
                            (additionalInfoRect.Height * 0.075f));
                        //

                        //Duration
                        uint mins = CoreMain.playerTable[i].recentMatches[k].duration / 60;
                        uint secs = CoreMain.playerTable[i].recentMatches[k].duration % 60;
                        var duration = string.Format(@"{0:D2}:{1:D2}", mins, secs);
                        var durSize = RendererManager.MeasureText(
                            $"Duration - {duration}",
                            "Lato",
                            FontWeight.Bold,
                            (additionalInfoRect.Height * 0.075f));
                        var durRect = new RectangleF(
                            csRect.X,
                            csRect.Y + csSize.Y + miniGap,
                            localizedNameRect.Width,
                            durSize.Y);
                        //RendererManager.DrawFilledRectangle(durRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"Duration - {duration}",
                            durRect,
                            new Color(66, 66, 66, 179),
                            "Lato",
                            FontWeight.Bold,
                            FontFlags.Left | FontFlags.Top,
                            (additionalInfoRect.Height * 0.075f));
                        //

                        //Impact
                        var impSize = RendererManager.MeasureText(
                            $"Impact - {CoreMain.playerTable[i].recentMatches[k].performanceRating}",
                            "Lato",
                            FontWeight.Bold,
                            (additionalInfoRect.Height * 0.075f));
                        var impRect = new RectangleF(
                            durRect.X,
                            durRect.Y + durSize.Y + miniGap,
                            localizedNameRect.Width,
                            impSize.Y);
                        //RendererManager.DrawFilledRectangle(impRect, new Color(0, 0, 0, 127));
                        RendererManager.DrawText(
                            $"Impact - {CoreMain.playerTable[i].recentMatches[k].performanceRating}",
                            impRect,
                            new Color(66, 66, 66, 179),
                            "Lato",
                            FontWeight.Bold,
                            FontFlags.Left | FontFlags.Top,
                            (additionalInfoRect.Height * 0.075f));
                        //

                        //RendererManager.DrawImage("panorama/images/status_icons/information_psd.vtex_c", LastGameIconRect, ImageType.Default, true);
                    }
                }
            }

        }

        private void OverwolfBackGround_ValueChanged(MenuSelector selector, SelectorEventArgs e)
        {
            TextureBackground = e.NewValue;
        }

        private void SetDefaultPos()
        {
            var scaledHeight = RendererManager.ScreenSize.Y * 0.6953703f;
            var scaledWidth = scaledHeight * 1.7777774f;

            scaledWidth = Math.Min(scaledWidth, RendererManager.ScreenSize.X * 0.6953125f);
            scaledHeight = scaledWidth / 1.7777774f;

            windowRect = new RectangleF((ScreenSize.X - scaledWidth) * 0.5f, (ScreenSize.Y - scaledHeight) * 0.5f, scaledWidth, scaledHeight);
            customRect = new RectangleF(
                windowRect.X - (windowRect.Width * 0.0225f),
                windowRect.Y - (windowRect.Height * 0.04f),
                windowRect.Width + (windowRect.Width * 0.0450f),
                windowRect.Height + (windowRect.Height * 0.08f));
            windowHeaderRect = new RectangleF(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height * 0.05f);
        }

        private void SaveCFG()
        {
            try
            {
                var windowRectJson = JsonSerializer.Serialize(windowRect);
                var aspectRatioJson = JsonSerializer.Serialize(RendererManager.AspectRatio);
                var screenSizeJson = JsonSerializer.Serialize(RendererManager.ScreenSize);
                File.WriteAllLines(
                    Path.Combine(Directories.Config, @"Plugins\Overwolf\Overwolf.json"),
                    new List<string>()
                    {
                        windowRectJson,
                        aspectRatioJson,
                        screenSizeJson
                    });
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
                    var cfgJson = File.ReadAllLines(cfgFile);

                    windowRect = JsonSerializer.Deserialize<RectangleF>(cfgJson[0]);
                    var aspectRatio = JsonSerializer.Deserialize<Vector2>(cfgJson[1]);
                    var screenSize = JsonSerializer.Deserialize<Vector2>(cfgJson[2]);
                    if (aspectRatio.X != RendererManager.AspectRatio.X
                        || aspectRatio.Y != RendererManager.AspectRatio.Y
                        || screenSize.X != RendererManager.ScreenSize.X
                        || screenSize.Y != RendererManager.ScreenSize.Y)
                    {
                        SetDefaultPos();
                        SaveCFG();
                    }
                    originalRect = windowRect;
                    customRect = new RectangleF(
                        windowRect.X - (windowRect.Width * 0.0225f),
                        windowRect.Y - (windowRect.Height * 0.04f),
                        windowRect.Width + (windowRect.Width * 0.0450f),
                        windowRect.Height + (windowRect.Height * 0.08f));
                    windowHeaderRect = new RectangleF(windowRect.X, windowRect.Y, windowRect.Width, windowRect.Height * 0.05f);
                }
                else
                {
                    SetDefaultPos();
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