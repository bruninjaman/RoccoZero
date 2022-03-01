using System.Text.Json;

using Divine.Entity;
using Divine.Entity.Entities.Components;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Network;
using Divine.Network.EventArgs;
using Divine.Network.GCSO;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;
using Divine.Zero.Helpers;
using Divine.Zero.Log;

using ShowEnemyRoles.Exstensions;

namespace ShowEnemyRoles
{
    internal sealed class Context
    {
        public Menu Menu { get; private set; }
        private Dictionary<string, List<LaneSelectonFlags>> RoleData = new Dictionary<string, List<LaneSelectonFlags>>()
        {
            ["Radiant"] = new List<LaneSelectonFlags>()
            {
                LaneSelectonFlags.HardSupport,
                LaneSelectonFlags.SafeLane,
                LaneSelectonFlags.SoftSupport,
                LaneSelectonFlags.MidLane,
                LaneSelectonFlags.OffLane
            },
            ["Dire"] = new List<LaneSelectonFlags>()
            {
                LaneSelectonFlags.OffLane,
                LaneSelectonFlags.SoftSupport,
                LaneSelectonFlags.HardSupport,
                LaneSelectonFlags.MidLane,
                LaneSelectonFlags.SafeLane
            }
        };
        private List<LaneSelectonFlags> EnemyRoles = new List<LaneSelectonFlags>();
        private Dictionary<LaneSelectonFlags, string> RoleNames = new Dictionary<LaneSelectonFlags, string>()
        {
            {LaneSelectonFlags.HardSupport, "Hard-Support"},
            {LaneSelectonFlags.SafeLane, "Safe-Lane"},
            {LaneSelectonFlags.MidLane, "Mid-Lane"},
            {LaneSelectonFlags.OffLane, "Off-Lane"},
            {LaneSelectonFlags.SoftSupport, "Soft-Support"}
        };
        private Color Color = Color.White;
        private RectangleF RoleRect;
        private RectangleF MainRect;
        private float Gap = 0f;
        private double Hue = 0d;
        private bool initDone = false;

        public Context()
        {
            Menu = new Menu();
            Menu.Enabled.ValueChanged += OnEnabledValueChanged;
            Menu.RGBMode.ValueChanged += OnRGBModeValueChanged;
            Menu.Width.ValueChanged += OnWidthValueChanged;
            Menu.Height.ValueChanged += OnHeightValueChanged;
            Menu.Gap.ValueChanged += OnGapValueChanged;
            Menu.RolesMovable.ValueChanged += OnRolesMovableValueChanged;

            RendererManager.LoadImageFromAssembly("ShowEnemyRoles.BG", "ShowEnemyRoles.Resources.bg.png");

            EnemyRoles = RoleData["Radiant"];
        }

        private void OnRGBModeValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                UpdateManager.CreateGameUpdate(30, OnUpdate);
            }
            else
            {
                UpdateManager.DestroyUpdate(OnUpdate);
                Color = Color.White;
            }
        }

        private void OnWidthValueChanged(MenuSlider slider, SliderEventArgs e)
        {
            RoleRect.Width = e.NewValue;
            UpdateMainRect();
        }

        private void OnHeightValueChanged(MenuSlider slider, SliderEventArgs e)
        {
            RoleRect.Height = e.NewValue;
            UpdateMainRect();
        }

        private void OnGapValueChanged(MenuSlider slider, SliderEventArgs e)
        {
            Gap = e.NewValue;
            UpdateMainRect();
        }

        private void UpdateMainRect()
        {
            RoleRect.X = 500f;
            RoleRect.Y = 500f;
            MainRect = new RectangleF(RoleRect.X - 10f, RoleRect.Y - 10f, (RoleRect.Width * 5f) + (Gap * 4) + 20f, RoleRect.Height + 20f);
        }

        private void OnRolesMovableValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                InputManager.MouseKeyDown += OnMouseKeyDown;
                InputManager.MouseKeyUp += OnMouseKeyUp;
                InputManager.MouseMove += OnMouseMove;
            }
            else
            {
                InputManager.MouseKeyDown -= OnMouseKeyDown;
                InputManager.MouseKeyUp -= OnMouseKeyUp;
                InputManager.MouseMove -= OnMouseMove;
            }
        }

        private void OnMouseMove(MouseMoveEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnMouseKeyUp(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnMouseKeyDown(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnEnabledValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                RendererManager.Draw += OnDraw;
                NetworkManager.GCSOMessageUpdate += OnGCSOMessageUpdate;
            }
            else
            {
                RendererManager.Draw -= OnDraw;
                NetworkManager.GCSOMessageUpdate -= OnGCSOMessageUpdate;
            }
        }

        private void OnUpdate()
        {
            if (!initDone || !Menu.RGBMode.Value) return;

            if (Hue < 360)
            {
                var tempColor = ColorExstensions.ColorFromHSV(Hue, 1d, 1d);
                Color = new Color(tempColor.R, tempColor.G, tempColor.B, tempColor.A);
                Hue++;
            }
            else
            {
                Hue = 0;
            }
        }

        private void OnDraw()
        {
            if (!initDone || EnemyRoles == null) return;

            RoleRect.X = 500f;
            RoleRect.Y = 500f;
            //var rect = new RectangleF(500,500, float.MaxValue, float.MaxValue);
            if (Menu.RolesMovable.Value)
            {
                RendererManager.DrawFilledRectangle(MainRect, new Color(70, 70, 70, 100));
            }

            foreach (var role in EnemyRoles)
            {

                if (Menu.Borders.Value)
                {
                    //RendererManager.DrawImage("ShowEnemyRoles.BG", RoleRect);
                    RendererManager.DrawRectangle(RoleRect, Color);
                }

                var textSize = RendererManager.MeasureText(RoleNames[role], "Tahoma", Menu.TextSize.Value);
                //RoleRect.Width = textSize.X + (textSize.Y * 0.5f);
                //RoleRect.Height = textSize.Y;
                DrawTextExstensions.DrawTextWithShadow(
                    RoleNames[role],
                    RoleRect,
                    Color,
                    "Tahoma",
                    FontFlags.Center | FontFlags.VerticalCenter,
                    Menu.TextSize.Value,
                    true);

                RoleRect.X += RoleRect.Width + Gap;
            }
        }

        private void SetDefaultPos()
        {

        }

        private void SaveCFG()
        {
            try
            {
                var roleRectJson = JsonSerializer.Serialize(RoleRect);
                var mainRectJson = JsonSerializer.Serialize(MainRect);
                //var screenSizeJson = JsonSerializer.Serialize(RendererManager.ScreenSize);
                File.WriteAllLines(
                    Path.Combine(Directories.Config, @"Plugins\ShowEnemyRoles\ShowEnemyRoles.json"),
                    new List<string>()
                    {
                        roleRectJson,
                        mainRectJson
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
                var cfgDir = Path.Combine(Directories.Config, @"Plugins\ShowEnemyRoles");
                if (!Directory.Exists(cfgDir))
                {
                    Directory.CreateDirectory(cfgDir);
                }
                var cfgFile = Path.Combine(cfgDir, @"ShowEnemyRoles.json");
                if (File.Exists(cfgFile))
                {
                    var cfgJson = File.ReadAllLines(cfgFile);

                    RoleRect = JsonSerializer.Deserialize<RectangleF>(cfgJson[0]);
                    MainRect = JsonSerializer.Deserialize<RectangleF>(cfgJson[1]);

                    if (RoleRect.IsEmpty || MainRect.IsEmpty)
                    {
                        SetDefaultPos();
                        SaveCFG();
                    }
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

        private void OnGCSOMessageUpdate(GCSOMessageUpdateEventArgs e)
        {
            var lobbyDataProto = e.Protobuf;
            if (lobbyDataProto.MessageId != GCSOMessageId.CSODOTALobby) return;

            var lobbyDataJson = lobbyDataProto.ToJson();
            var members = lobbyDataJson["all_members"].AsArray();

            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                var team = i < 4 ? RoleData["Radiant"] : RoleData["Dire"];
                team[i] = (LaneSelectonFlags)member["lane_selection_flags"].GetValue<int>();
            }

            var isRadiant = EntityManager.LocalPlayer?.Team == Team.Radiant ? true : false;
            if (isRadiant)
                EnemyRoles = RoleData["Radiant"];
            else
                EnemyRoles = RoleData["Dire"];

        }

        internal void Dispose()
        {
            RendererManager.Draw -= OnDraw;
            UpdateManager.DestroyUpdate(OnUpdate);
            NetworkManager.GCSOMessageUpdate -= OnGCSOMessageUpdate;
            Menu.Enabled.ValueChanged -= OnEnabledValueChanged;
        }
    }
}