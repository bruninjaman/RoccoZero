namespace O9K.AIO.Heroes.ArcWarden.Draw
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.Managers.Entity;
    using Core.Managers.Menu.Items;
    using Core.Managers.Renderer.Utils;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Game;
    using Divine.Numerics;
    using Divine.Renderer;

    using Modes;

    using Utils;

    public static class ArcWardenPanel
    {
        public static Lane lane;

        // private static readonly int _optionsCount = Enum.GetNames(typeof(Lane)).Length;

        private static readonly Dictionary<Lane, Vector4> vector4PosClick = new();

        public static Vector2 SizePanel;

        public static MenuSlider positionSliderX = new("position x", 0, 0, 5000);

        public static MenuSlider positionSliderY = new("position y", 600, 0, 5000);

        public static MenuSlider size = new("SIZE", 100, 50, 250);

        public static string unitName { get; set; }

        public static bool pushComboStatus { get; set; } = false;

        public static bool cloneComboStatus { get; set; } = false;
        
        private static List<AbilityId> CloneItems { get; } =
            new()
            {
                AbilityId.item_hand_of_midas,
                AbilityId.item_tpscroll,
            };
        // public static void OnMouseKeyDown(MouseEventArgs e)
        // {
        //     if (e.MouseKey != MouseKey.Left)
        //     {
        //         return;
        //     }
        //
        //     for (var i = 0; i < _optionsCount; i++)
        //     {
        //         if (e.Position.IsUnderRectangle(new RectangleF(vector4PosClick[(Lane)i].X - 5, vector4PosClick[(Lane)i].Y - 5, vector4PosClick[(Lane)i].Z, vector4PosClick[(Lane)i].W)))
        //         {
        //             lane = (Lane)i;
        //         }
        //     }
        // }

        public static void OnDraw()
        {
            unitName = ArcWardenComboMode.StaticTargetManager.Target?.BaseUnit.InternalName;

            if (GameManager.IsShopOpen)
            {
                return;
            }

            var scaling = RendererManager.Scaling;
            var optionsCount = 4;
            var positionX = positionSliderX;
            var positionY = positionSliderY;
            var sizeMenu = size * scaling;
            var indent = sizeMenu * 0.2f;
            var halfIndent = indent * 0.5f;
            var menuWidth = sizeMenu * optionsCount + indent;
            var menuHeight = sizeMenu * 5f + indent;

            //Draw main rect without ident
            var mainRect = new RectangleF(
                positionX - indent,
                positionY - indent,
                menuWidth + indent,
                menuHeight + indent);

            RendererManager.DrawFilledRectangle(mainRect, new Color(10, 10, 10, 100));

            //Draw rect with identst
            var rect = new RectangleF(
                positionX,
                positionY,
                menuWidth,
                menuHeight);

            // RendererManager.DrawFilledRectangle(rect, Color.Gray);

            vector4PosClick.Clear();

            var inRectPosX = rect.X;
            var inRectPosY = rect.Y;
            var rectHalfWidth = rect.Width * 0.5f;
            var imgSize = sizeMenu;
            var imgHalfSize = imgSize * 0.5f;

            //Draw Divine logo
            // var divineLogoRect = new RectangleF(inRectPosX + rectHalfWidth - imgHalfSize, inRectPosY, imgSize, imgSize);
            // RendererManager.DrawRectangle(divineLogoRect, Color.Gold);
            // inRectPosY += imgSize + indent;
            // inRectPosX = rect.X;

            //Draw text: "FARM STATUS"
            var farmStatusTextWidth = rect.Width;
            var farmStatusTextHeight = imgHalfSize;
            var farmStatusTextRect = new RectangleF(inRectPosX, inRectPosY, farmStatusTextWidth, farmStatusTextHeight);
            var farmStatusTextSize = sizeMenu * 0.35f;
            // // RendererManager.DrawRectangle(farmStatusTextRect, Color.DarkKhaki);
            RendererManager.DrawText("CLONE STATUS", farmStatusTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, farmStatusTextSize);
            inRectPosY += farmStatusTextHeight + halfIndent;
            inRectPosX = rect.X;
            //
            // //Draw farm status
            var farmStatusHeight = imgHalfSize * 1.2f;
            var farmStatusRect = new RectangleF(inRectPosX, inRectPosY, rect.Width, farmStatusHeight);
            var farmStatusSize = sizeMenu * 0.5f;
            string cloneStatus;
            Color pushColor;
            
            if (cloneComboStatus)
            {
                cloneStatus = "ACTIVE";
                pushColor = Color.Green;
            }
            else
            {
                cloneStatus = "IDLE";
                pushColor = Color.Red;
            }
            //
            // // RendererManager.DrawFilledRectangle(farmStatusRect, new Color(255, 255, 255, 50));
            RendererManager.DrawText(cloneStatus, farmStatusRect, pushColor, FontFlags.Center | FontFlags.VerticalCenter, farmStatusSize);
            
            inRectPosY += farmStatusHeight + indent;
            inRectPosX = rect.X;
            //
            // //Draw text: "LINE SELECT FARM"
            // var lineTextWidth = rect.Width;
            // var lineTextHeight = imgHalfSize;
            // var lineTextRect = new RectangleF(inRectPosX, inRectPosY, lineTextWidth, lineTextHeight);
            // var lineTextSize = farmStatusTextSize;
            //
            // RendererManager.DrawLine(
            //     new Vector2(lineTextRect.X, lineTextRect.Y),
            //     new Vector2(lineTextRect.X + lineTextRect.Width, lineTextRect.Y),
            //     Color.Gray);
            //
            // RendererManager.DrawText("LINE SELECT FARM", lineTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, lineTextSize);
            // inRectPosY += lineTextHeight + halfIndent;
            // inRectPosX = rect.X;
            //
            // //Draw modes for clone farm
            // var buttonFontSize = sizeMenu * 0.35f;
            // var buttonWidth = imgSize;
            // var buttonHeight = imgHalfSize;
            //
            // for (var i = 0; i < optionsCount; i++)
            // {
            //     vector4PosClick.Add((Lane)i, new Vector4(inRectPosX, inRectPosY, buttonWidth, buttonHeight));
            //
            //     var rectBorderImage = new RectangleF(
            //         inRectPosX,
            //         inRectPosY,
            //         buttonWidth,
            //         buttonHeight);
            //
            //     if ((Lane)i == lane)
            //     {
            //         RendererManager.DrawRectangle(rectBorderImage, Color.Green);
            //
            //         RendererManager.DrawText(((Lane)i).ToString(), rectBorderImage, Color.Green, FontFlags.Center | FontFlags.VerticalCenter, buttonFontSize);
            //     }
            //     else
            //     {
            //         RendererManager.DrawRectangle(rectBorderImage, Color.Red);
            //
            //         RendererManager.DrawText(((Lane)i).ToString(), rectBorderImage, Color.White, FontFlags.Center | FontFlags.VerticalCenter, buttonFontSize);
            //     }
            //
            //     inRectPosX += buttonWidth + indent * 0.68f;
            // }
            //
            // inRectPosX = rect.X;
            // inRectPosY += buttonHeight + indent;

            //Draw text: "ARC TARGET COMBO"
            var targetTextWidth = rect.Width;
            var targetTextHeight = imgHalfSize;
            var targetTextRect = new RectangleF(inRectPosX, inRectPosY, targetTextWidth, targetTextHeight);
            var targetTextSize = farmStatusTextSize;

            // RendererManager.DrawRectangle(targetTextRect, Color.DarkKhaki);
            RendererManager.DrawLine(
                new Vector2(targetTextRect.X, targetTextRect.Y),
                new Vector2(targetTextRect.X + targetTextRect.Width, targetTextRect.Y),
                Color.Gray);

            RendererManager.DrawText("ARC TARGET COMBO", targetTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, targetTextSize);
            inRectPosY += targetTextHeight + halfIndent;
            inRectPosX = rect.X;

            //Draw target image
            var targetImgWidth = imgSize * 2;
            var targetImgHeight = imgSize;
            var targetImgRect = new RectangleF(inRectPosX + rectHalfWidth - imgSize, inRectPosY, targetImgWidth, targetImgHeight);
            // RendererManager.DrawRectangle(targetImgRect, Color.Cyan);
            RendererManager.DrawImage(unitName ?? "", targetImgRect, UnitImageType.Default, true);

            inRectPosY += targetImgHeight + indent;
            inRectPosX = rect.X;

            //Draw text: "CLONE COOLDOWN ITEMS"
            var cloneCDsTextWidth = rect.Width;
            var cloneCDsTextHeight = imgHalfSize;
            var cloneCDsTextRect = new RectangleF(inRectPosX, inRectPosY, cloneCDsTextWidth, cloneCDsTextHeight);
            var cloneCDsTextSize = farmStatusTextSize;

            RendererManager.DrawLine(
                new Vector2(cloneCDsTextRect.X, cloneCDsTextRect.Y),
                new Vector2(cloneCDsTextRect.X + cloneCDsTextRect.Width, cloneCDsTextRect.Y),
                Color.Gray);

            RendererManager.DrawText("CLONE COOLDOWN ITEMS", cloneCDsTextRect, Color.White, FontFlags.Center | FontFlags.VerticalCenter, cloneCDsTextSize);
            inRectPosY += cloneCDsTextHeight + indent;
            inRectPosX = rect.X + rect.Width * 0.13f;

            //Draw clone mids and boots
            var itemImgWidth = rect.Width * 0.3f;
            var itemImgHeight = imgSize;
            var itemIndent = rect.Width * 0.13f;
            var itemCdTextSize = sizeMenu * 0.6f;

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

            var clone = EntityManager9.Units.Where(x => x.IsIllusion && x.IsHero && x.IsMyControllable).FirstOrDefault();

            //Draw midas image
            var itemMidasRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemImgHeight);
            var midas = clone?.Abilities.Where(x => x.Id == CloneItems[0]).FirstOrDefault();

            if (midas is not null)
            {
                var midasDrawer = new AbilityDrawer(midas);
                midasDrawer.Draw(new Rectangle9(itemMidasRect.X, itemMidasRect.Y, itemMidasRect.Width, itemMidasRect.Height), itemCdTextSize);
            }

            inRectPosX += itemImgWidth + itemIndent;

            //Draw boots image
            var itemBootsRect = new RectangleF(inRectPosX, inRectPosY, itemImgWidth, itemImgHeight);
            var tp = clone?.Abilities.Where(x => x.Id == CloneItems[1]).FirstOrDefault();

            if (tp is not null)
            {
                var tpDrawer = new AbilityDrawer(tp);
                tpDrawer.Draw(new Rectangle9(itemBootsRect.X, itemBootsRect.Y, itemBootsRect.Width, itemBootsRect.Height), itemCdTextSize);
            }
        }
    }
}