namespace Debugger.Logger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Controls;

using Debugger.Menus;

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

using Button = Controls.Button;

internal class Log : ILog
{
    private const uint WM_LBUTTONDOWN = 0x0201;

    private const uint WM_MOUSEWHEEL = 0x020A;

    private const uint WM_RBUTTONDOWN = 0x0204;

    private readonly List<LogItem> displayList = new List<LogItem>();

    private readonly List<LogItem> items = new List<LogItem>();

    private Button clearButton;

    private MenuSlider itemsToSave;

    private Button jumpTopButton;

    private MenuSlider linesToShow;

    private readonly IMainMenu mainMenu;

    private ToggleButton overlayButton;

    private ToggleButton pauseButton;

    private MenuSlider positionX;

    private MenuSlider positionY;

    private MenuSwitcher hideSwitcher;

    private float screenSizeX;

    private int scrollPosition;

    private MenuSlider textSize;

    public Log(IMainMenu mainMenu)
    {
        this.mainMenu = mainMenu;
    }

    public int LoadPriority { get; } = 999;

    private int ScrollPosition
    {
        get => this.scrollPosition;

        set
        {
            if (value <= 0)
            {
                this.scrollPosition = 0;
            }
            else if (value >= this.items.Count)
            {
                this.scrollPosition = this.items.Count - 1;
            }
            else
            {
                this.scrollPosition = value;
            }
        }
    }

    public void Activate()
    {
        var menu = this.mainMenu.OverlaySettingsMenu;

        this.itemsToSave = menu.CreateSlider("Items to save", 100, 5, 500);
        this.linesToShow = menu.CreateSlider("Lines to show", 30, 10, 100);
        this.linesToShow.ValueChanged += this.LinesToShowOnPropertyChanged;
        this.textSize = menu.CreateSlider("Text size", 20, 10, 30);
        this.textSize.ValueChanged += this.TextSizeOnPropertyChanged;
        this.screenSizeX = HUDInfo.ScreenSize.X;
        this.positionX = menu.CreateSlider("Position x", (int)(this.screenSizeX * 0.75), 0, (int)this.screenSizeX);
        this.positionY = menu.CreateSlider("Position y", 100, 0, (int)HUDInfo.ScreenSize.Y);
        this.hideSwitcher = menu.CreateSwitcher("Hide");
        this.hideSwitcher.IsHidden = true;

        menu.CreateSlider("Position y", 100, 0, (int)HUDInfo.ScreenSize.Y);

        this.overlayButton = new ToggleButton(
            "Hide",
            "Show",
            new Vector2(this.screenSizeX - 100, this.positionY - 50),
            new Vector2(100, 30));

        this.overlayButton.Enabled = this.hideSwitcher;

        this.pauseButton = new ToggleButton(
            "Pause",
            "Continue",
            new Vector2(this.screenSizeX - 200, this.positionY - 50),
            new Vector2(100, 30));
        this.clearButton = new Button("Clear", new Vector2(this.screenSizeX - 300, this.positionY - 50), new Vector2(100, 30));
        this.jumpTopButton = new Button("^", new Vector2(this.screenSizeX - 400, this.positionY - 50), new Vector2(100, 30));

        this.TextSizeOnPropertyChanged(null, null);

        RendererManager.Draw += this.DrawingOnDraw;
        InputManager.WindowProc += this.GameOnWndProc;
    }

    public void Display(LogItem newItem)
    {
        if (!this.pauseButton.Enabled)
        {
            return;
        }

        if (this.items.Count > this.itemsToSave)
        {
            this.items.RemoveAt(0);
        }

        this.items.Add(newItem);

        if (this.ScrollPosition > 0)
        {
            this.ScrollPosition++;
        }

        this.UpdateOverlay();
    }

    public void Dispose()
    {
        this.linesToShow.ValueChanged -= this.LinesToShowOnPropertyChanged;
        this.positionY.ValueChanged += this.PositionYOnPropertyChanged;
        RendererManager.Draw -= this.DrawingOnDraw;
        InputManager.WindowProc -= this.GameOnWndProc;
    }

    public bool IsMouseUnderLog()
    {
        if (!this.overlayButton.Enabled)
        {
            return false;
        }

        var screenSize = RendererManager.ScreenSize;
        return GameManager.MouseScreenPosition.IsUnderRectangle(this.positionX - 20, this.positionY - 20, screenSize.X, screenSize.Y);
    }

    private void DrawingOnDraw()
    {
        this.overlayButton.Draw();
        if (!this.overlayButton.Enabled)
        {
            return;
        }

        this.pauseButton.Draw();
        this.clearButton.Draw();

        if (this.scrollPosition > 0)
        {
            this.jumpTopButton.Draw();
        }

        var backgroundColor = new Color(50, 50, 50, 200);
        var totalSize = this.displayList.Sum(x => x.Lines.Count + 2);
        if (totalSize > 0)
        {
            RendererManager.DrawFilledRectangle(
                new RectangleF(this.positionX - 20, this.positionY - 20, 2000, (totalSize * this.textSize) + 20),
                backgroundColor);
        }

        var selectedLine = (int)((GameManager.MouseScreenPosition.Y - this.positionY) / this.textSize);
        var offset = 0;

        foreach (var item in this.displayList)
        {
            if (!string.IsNullOrEmpty(item.FirstLine))
            {
                RendererManager.DrawText(
                    item.FirstLine,
                    new Vector2(this.positionX, this.positionY + (offset * this.textSize)),
                    item.Color,
                    "Arial",
                    this.textSize);
            }

            var startColor = item.Color;
            var endColor = Color.White;
            var time = Math.Min(GameManager.RawGameTime - item.Time, 1);

            var color = new Color(
                (int)(((endColor.R - startColor.R) * time) + startColor.R),
                (int)(((endColor.G - startColor.G) * time) + startColor.G),
                (int)(((endColor.B - startColor.B) * time) + startColor.B));

            foreach (var itemLine in item.Lines)
            {
                offset++;
                RendererManager.DrawText(
                    itemLine.Item1,
                    new Vector2(this.positionX, this.positionY + (offset * this.textSize)),
                    selectedLine == offset && this.IsMouseUnderLog() ? Color.Orange : color,
                    "Arial",
                    this.textSize);
            }

            offset += 2;
        }
    }

    private void GameOnWndProc(WindowProcEventArgs e)
    {
        if (e.Msg == WM_MOUSEWHEEL && this.IsMouseUnderLog())
        {
            var delta = (short)((e.WParam >> 16) & 0xFFFF);
            if (delta > 0)
            {
                this.ScrollPosition--;
            }
            else
            {
                this.ScrollPosition++;
            }

            this.UpdateOverlay();
            e.Process = false;
            return;
        }

        if (e.Msg == WM_LBUTTONDOWN)
        {
            if (this.overlayButton.IsMouseUnderButton())
            {
                this.overlayButton.Enabled = !this.overlayButton.Enabled;
                this.hideSwitcher.Value = this.overlayButton.Enabled;
                e.Process = false;
                return;
            }

            if (this.jumpTopButton.IsMouseUnderButton())
            {
                this.scrollPosition = 0;
                this.UpdateOverlay();
                e.Process = false;
                return;
            }

            if (this.clearButton.IsMouseUnderButton())
            {
                this.scrollPosition = 0;
                this.items.Clear();
                this.UpdateOverlay();
                e.Process = false;
                return;
            }

            if (this.pauseButton.IsMouseUnderButton())
            {
                this.pauseButton.Enabled = !this.pauseButton.Enabled;
                e.Process = false;
                return;
            }

            if (this.IsMouseUnderLog())
            {
                var line = (int)((GameManager.MouseScreenPosition.Y - this.positionY) / this.textSize);
                var offset = 0;

                foreach (var item in this.displayList)
                {
                    foreach (var itemLine in item.Lines)
                    {
                        if (++offset == line)
                        {
                            if (!string.IsNullOrEmpty(itemLine.Item2))
                            {
                                Clipboard.SetText(itemLine.Item2);
                            }

                            e.Process = false;
                            return;
                        }
                    }

                    offset += 2;
                }
            }
        }
    }

    private void LinesToShowOnPropertyChanged(MenuSlider slider, SliderEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            this.UpdateOverlay();
        });
    }

    private void PositionYOnPropertyChanged(MenuSlider slider, SliderEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            this.overlayButton.UpdateYPosition(this.positionY - 50);
            this.jumpTopButton.UpdateYPosition(this.positionY - 50);
            this.clearButton.UpdateYPosition(this.positionY - 50);
            this.pauseButton.UpdateYPosition(this.positionY - 50);
        });
    }

    private void TextSizeOnPropertyChanged(MenuSlider slider, SliderEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            this.overlayButton.UpdateSize(this.textSize);
            this.jumpTopButton.UpdateSize(this.textSize);
            this.clearButton.UpdateSize(this.textSize);
            this.pauseButton.UpdateSize(this.textSize);
        });
    }

    private void UpdateOverlay()
    {
        this.displayList.Clear();
        var totalLines = 0;

        foreach (var item in this.items.Reverse<LogItem>().Skip(this.scrollPosition))
        {
            this.displayList.Add(item);
            totalLines += item.Lines.Count + 2;

            if (totalLines >= this.linesToShow)
            {
                break;
            }
        }
    }
}