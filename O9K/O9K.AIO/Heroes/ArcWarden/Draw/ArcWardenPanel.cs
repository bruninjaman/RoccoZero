namespace O9K.AIO.Heroes.ArcWarden.Draw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Managers.Entity;
    using Core.Managers.Menu.Items;
    using Core.Managers.Renderer.Utils;

    using CustomUnitManager;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Input;
    using Divine.Input.EventArgs;
    using Divine.Numerics;
    using Divine.Renderer;

    using TargetManager;

    using UnitManager;

    using Utils;

    internal static class ArcWardenPanel
    {
        public static Lane lane;

        public static Unit9 cloneTarget;

        private static readonly int OptionsCount = Enum.GetNames(typeof(Lane)).Length;

        private static readonly Dictionary<Lane, Vector4> vector4PosClick = new();

        private static readonly Dictionary<Unit9, Vector4> enemyVector4PosClick = new();

        public static MenuSlider positionSliderX = new("position x", 0, 0, 5000);

        public static MenuSlider positionSliderY = new("position y", 600, 0, 5000);

        public static MenuSlider size = new("SIZE", 100, 50, 250);

        private static string UnitName
        {
            get
            {
                return _targetManager.Target?.BaseUnit.InternalName;
            }
        }

        private static TargetManager _targetManager;

        private static ArcWardenUnitManager _unitManager;

        private static float _inRectPosX;

        private static float _inRectPosY;

        public static void Init(TargetManager targetManager, IUnitManager unitManager)
        {
            _targetManager = targetManager;
            _unitManager = unitManager as ArcWardenUnitManager;
        }

        public static bool PushComboStatus { get; set; } = false;

        private static List<AbilityId> CloneItems { get; } =
            new()
            {
                AbilityId.item_hand_of_midas,
                AbilityId.item_tpscroll,
            };

        public static void OnMouseKeyDown(MouseEventArgs e)
        {
            if (e.MouseKey != MouseKey.Left)
            {
                return;
            }

            for (int i = 0; i < OptionsCount; i++)
            {
                if (e.Position.IsUnderRectangle(new RectangleF(vector4PosClick[(Lane)i].X - 5, vector4PosClick[(Lane)i].Y - 5,
                                                               vector4PosClick[(Lane)i].Z, vector4PosClick[(Lane)i].W)))
                {
                    lane = (Lane)i;
                }
            }

            foreach (var (hero, _) in enemyVector4PosClick.Where(keyValuePair => e.Position.IsUnderRectangle(new RectangleF(keyValuePair.Value.X - 5, keyValuePair.Value.Y - 5,
                                                                                                                            keyValuePair.Value.Z, keyValuePair.Value.W))))
            {
                cloneTarget = hero;
            }
        }

        public static void OnDraw()
        {
            if (GameManager.IsShopOpen)
            {
                return;
            }

            float scaling = RendererManager.Scaling;
            int optionsCount = OptionsCount;
            var positionX = positionSliderX;
            var positionY = positionSliderY;
            float sizeMenu = size * scaling;
            float indent = sizeMenu * 0.2f;
            float halfIndent = indent * 0.5f;
            float menuWidth = sizeMenu * optionsCount + indent * 2;
            float menuHeight = sizeMenu * 9.5f + indent * 2;

            //Draw main rect without ident
            var mainRect = new RectangleF(positionX - indent,
                                          positionY - indent,
                                          menuWidth + indent * 2,
                                          menuHeight + indent * 2);

            RendererManager.DrawFilledRectangle(mainRect, new Color(10, 10, 10, 255));

            //Draw rect with identst
            var rect = new RectangleF(positionX,
                                      positionY,
                                      menuWidth,
                                      menuHeight);

            // RendererManager.DrawFilledRectangle(rect, Color.Gray);

            vector4PosClick.Clear();

            _inRectPosX = rect.X;
            _inRectPosY = rect.Y;
            float rectHalfWidth = rect.Width * 0.5f;
            float imgSize = sizeMenu;
            float imgHalfSize = imgSize * 0.5f;

            //Draw Divine logo
            // var divineLogoRect = new RectangleF(inRectPosX + rectHalfWidth - imgHalfSize, inRectPosY, imgSize, imgSize);
            // RendererManager.DrawRectangle(divineLogoRect, Color.Gold);
            // inRectPosY += imgSize + indent;
            // inRectPosX = rect.X;

            //Draw text: "FARM STATUS"
            float farmStatusTextWidth = rect.Width;
            float farmStatusTextHeight = imgHalfSize;
            var farmStatusTextRect = new RectangleF(_inRectPosX, _inRectPosY, farmStatusTextWidth, farmStatusTextHeight);
            float farmStatusTextSize = sizeMenu * 0.4f;
            // RendererManager.DrawRectangle(farmStatusTextRect, Color.DarkKhaki);
            RendererManager.DrawText("FARM STATUS", farmStatusTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, farmStatusTextSize);
            _inRectPosY += farmStatusTextHeight + halfIndent;
            _inRectPosX = rect.X;

            //Draw farm status
            float farmStatusHeight = imgHalfSize * 1.2f;
            var farmStatusRect = new RectangleF(_inRectPosX, _inRectPosY, rect.Width, farmStatusHeight);
            float farmStatusSize = sizeMenu * 0.5f;
            string pushText;
            Color pushColor;

            if (PushComboStatus)
            {
                pushText = "ACTIVE";
                pushColor = Color.Green;
            }
            else
            {
                pushText = "NOT ACTIVE";
                pushColor = Color.Red;
            }

            // RendererManager.DrawFilledRectangle(farmStatusRect, new Color(255, 255, 255, 50));
            RendererManager.DrawText(pushText, farmStatusRect, pushColor, FontFlags.Center | FontFlags.VerticalCenter, farmStatusSize);

            _inRectPosY += farmStatusHeight + indent;
            _inRectPosX = rect.X;

            //Draw text: "LINE SELECT FARM"
            float lineTextWidth = rect.Width;
            float lineTextHeight = imgHalfSize;
            var lineTextRect = new RectangleF(_inRectPosX, _inRectPosY, lineTextWidth, lineTextHeight);
            float lineTextSize = farmStatusTextSize;

            RendererManager.DrawLine(
                                     new Vector2(lineTextRect.X, lineTextRect.Y),
                                     new Vector2(lineTextRect.X + lineTextRect.Width, lineTextRect.Y),
                                     Color.Gray);

            RendererManager.DrawText("LINE SELECT FARM", lineTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, lineTextSize);
            _inRectPosY += lineTextHeight + halfIndent;
            _inRectPosX = rect.X;

            //Draw modes for clone farm
            float buttonFontSize = sizeMenu * 0.35f;
            float buttonWidth = imgSize;
            float buttonHeight = imgHalfSize;

            for (int i = 0; i < optionsCount; i++)
            {
                vector4PosClick.Add((Lane)i, new Vector4(_inRectPosX, _inRectPosY, buttonWidth, buttonHeight));

                var rectBorderImage = new RectangleF(
                                                     _inRectPosX,
                                                     _inRectPosY,
                                                     buttonWidth,
                                                     buttonHeight);

                if ((Lane)i == lane)
                {
                    RendererManager.DrawRectangle(rectBorderImage, Color.Green);

                    RendererManager.DrawText(((Lane)i).ToString(), rectBorderImage, Color.Green, FontFlags.Center | FontFlags.VerticalCenter,
                                             buttonFontSize);
                }
                else
                {
                    RendererManager.DrawRectangle(rectBorderImage, Color.Red);

                    RendererManager.DrawText(((Lane)i).ToString(), rectBorderImage, Color.White, FontFlags.Center | FontFlags.VerticalCenter,
                                             buttonFontSize);
                }

                _inRectPosX += buttonWidth + indent * 0.68f;
            }

            _inRectPosX = rect.X;
            _inRectPosY += buttonHeight + indent;

            //Draw text: "ARC TARGET COMBO"
            float targetTextWidth = rect.Width;
            float targetTextHeight = imgHalfSize;
            var targetTextRect = new RectangleF(_inRectPosX, _inRectPosY, targetTextWidth, targetTextHeight);
            float targetTextSize = farmStatusTextSize;

            // RendererManager.DrawRectangle(targetTextRect, Color.DarkKhaki);
            RendererManager.DrawLine(
                                     new Vector2(targetTextRect.X, targetTextRect.Y),
                                     new Vector2(targetTextRect.X + targetTextRect.Width, targetTextRect.Y),
                                     Color.Gray);

            RendererManager.DrawText("ARC TARGET COMBO", targetTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, targetTextSize);
            _inRectPosY += targetTextHeight + halfIndent;
            _inRectPosX = rect.X;

            //Draw target image
            float targetImgWidth = imgSize * 2;
            float targetImgHeight = imgSize;
            var targetImgRect = new RectangleF(_inRectPosX + rectHalfWidth - imgSize, _inRectPosY, targetImgWidth, targetImgHeight);
            // RendererManager.DrawRectangle(targetImgRect, Color.Cyan);
            RendererManager.DrawImage(UnitName ?? "", targetImgRect, UnitImageType.Default, true);

            _inRectPosY += targetImgHeight + indent;
            _inRectPosX = rect.X;

            //Draw text: "CLONE COOLDOWN ITEMS"
            float cloneCDsTextWidth = rect.Width;
            float cloneCDsTextHeight = imgHalfSize;
            var cloneCDsTextRect = new RectangleF(_inRectPosX, _inRectPosY, cloneCDsTextWidth, cloneCDsTextHeight);
            float cloneCDsTextSize = farmStatusTextSize;

            RendererManager.DrawLine(
                                     new Vector2(cloneCDsTextRect.X, cloneCDsTextRect.Y),
                                     new Vector2(cloneCDsTextRect.X + cloneCDsTextRect.Width, cloneCDsTextRect.Y),
                                     Color.Gray);

            RendererManager.DrawText("CLONE COOLDOWN ITEMS", cloneCDsTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter,
                                     cloneCDsTextSize);

            _inRectPosY += cloneCDsTextHeight + indent;
            _inRectPosX = rect.X + rect.Width * 0.13f;

            //Draw clone mids and boots
            float itemImgWidth = rect.Width * 0.3f;
            float itemImgHeight = imgSize;
            float itemIndent = rect.Width * 0.13f;
            float itemCdTextSize = sizeMenu * 0.6f;

            //Draw text: "MIDAS"
            // var itemMidasTextWidth = rect.Width;
            // var itemMidasTextHeight = imgHalfSize;
            // var itemMidasTextRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemMidasTextHeight);
            // var itemMidasTextSize = buttonFontSize;
            // RendererManager.DrawRectangle(itemMidasTextRect, Color.MediumBlue);
            // RendererManager.DrawText("MIDAS", itemMidasTextRect, Color.MediumBlue, FontFlags.Center | FontFlags.VerticalCenter, itemMidasTextSize);
            // inRectPosX += itemImgWidth + itemIndent;

            //Draw text: "BOOTS"
            // var itemBootsTextWidth = rect.Width;
            // var itemBootsTextHeight = imgHalfSize;
            // var itemBootsTextRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemBootsTextHeight);
            // var itemBootsTextSize = buttonFontSize;
            // RendererManager.DrawRectangle(itemBootsTextRect, Color.MediumBlue);
            // RendererManager.DrawText("BOOTS", itemBootsTextRect, Color.MediumBlue, FontFlags.Center | FontFlags.VerticalCenter, itemBootsTextSize);
            // inRectPosY += itemBootsTextHeight + halfIndent;
            // inRectPosX = rect.X;

            var clone = EntityManager9.Units.FirstOrDefault(x => x.IsIllusion && x.IsHero && x.IsMyControllable);

            //Draw midas image
            var itemMidasRect = new RectangleF(_inRectPosX, _inRectPosY, itemImgWidth, itemImgHeight);
            var midas = (clone?.Abilities ?? Array.Empty<Ability9>()).FirstOrDefault(x => x.Id == CloneItems[0]);

            if (midas is not null)
            {
                var midasDrawer = new AbilityDrawer(midas);
                midasDrawer.Draw(new Rectangle9(itemMidasRect.X, itemMidasRect.Y, itemMidasRect.Width, itemMidasRect.Height), itemCdTextSize);
            }

            _inRectPosX += itemImgWidth + itemIndent;

            //Draw boots image
            var itemBootsRect = new RectangleF(_inRectPosX, _inRectPosY, itemImgWidth, itemImgHeight);
            var tp = (clone?.Abilities ?? Array.Empty<Ability9>()).FirstOrDefault(x => x.Id == CloneItems[1]);

            if (tp is not null)
            {
                var tpDrawer = new AbilityDrawer(tp);
                tpDrawer.Draw(new Rectangle9(itemBootsRect.X, itemBootsRect.Y, itemBootsRect.Width, itemBootsRect.Height), itemCdTextSize);
            }

            _inRectPosY += itemImgHeight + indent;

            _inRectPosX = rect.X;

            // Draw Enemy to choose
            InitTargetChooserForClone(rect, menuWidth, _inRectPosX, targetImgWidth, _inRectPosY, targetImgHeight, indent);

            foreach (var (unit, vector4) in enemyVector4PosClick)
            {
                var rectangleF = new RectangleF(vector4.X, vector4.Y, targetImgWidth, targetImgHeight);

                if (unit == cloneTarget)
                {
                    _unitManager.SetTargetForClone(unit);
                    RendererManager.DrawRectangle(rectangleF, Color.Green);

                    RendererManager.DrawImage(unit.Name ?? "", rectangleF, UnitImageType.Default, true);

                }
                else
                {
                    RendererManager.DrawRectangle(rectangleF, Color.Red);

                    RendererManager.DrawImage(unit.Name ?? "", rectangleF, UnitImageType.Default, true);
                }
            }
        }

        private static void InitTargetChooserForClone(RectangleF rect, float menuWidth, float inRectPosX, float targetImgWidth, float inRectPosY, float targetImgHeight, float indent)
        {
            if (EntityManager9.EnemyHeroes.Count < 5)
            {
                return;
            }

            if (enemyVector4PosClick.Count == 6)
            {
                return;
            }

            foreach (var unit in EntityManager9.EnemyHeroes)
            {
                if (rect.X + menuWidth < inRectPosX + targetImgWidth)
                {
                    inRectPosY += targetImgHeight + indent;
                    inRectPosX = rect.X;
                }

                enemyVector4PosClick.TryAdd(unit, new Vector4(inRectPosX, inRectPosY, targetImgWidth, targetImgHeight));

                inRectPosX += targetImgWidth + indent;
            }

            enemyVector4PosClick.TryAdd(EntityManager9.Owner, new Vector4(inRectPosX, inRectPosY, targetImgWidth, targetImgHeight));

        }
    }
}