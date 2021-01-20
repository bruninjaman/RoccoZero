using System.Reflection;

using Divine;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SDK.Extensions;

using SharpDX;

namespace Manabars
{
    internal sealed class ItemBar 
    {
        private readonly MenuSwitcher ItemEnableChangedValue;
        private readonly MenuSwitcher ItemAllyChangedValue;
        private readonly MenuSwitcher ItemСourierChangedValue;

        public ItemBar(Menu menu)
        {
            var MenuItem = menu.CreateMenu("ItemBar");

            ItemEnableChangedValue = MenuItem.CreateSwitcher("Enable");


            ItemAllyChangedValue = MenuItem.CreateSwitcher("Allies");
            ItemСourierChangedValue = MenuItem.CreateSwitcher("Сourier");


            var testImage = Assembly.GetExecutingAssembly().GetManifestResourceStream("Manabars.ItemOver.png");

            RendererManager.LoadTexture("Manabars.ItemOver.png", testImage);


            ItemEnableChangedValue.ValueChanged += ItemBarHide;

        }

        private void ItemBarHide(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value == true)
            {
                RendererManager.Draw += OnRendererManagerDraw;
            }
            else
            {
                RendererManager.Draw -= OnRendererManagerDraw;
            }
        }

        private void OnRendererManagerDraw()
        {
            var scaling = RendererManager.Scaling;

            var localHero = EntityManager.LocalHero;
            if (localHero == null)
            {
                return;
            }

            foreach (var unit in EntityManager.GetEntities<Unit>())
            {
                if (unit is not Hero && (!ItemСourierChangedValue || unit is not Courier))
                {
                    continue;
                }

                if (!unit.IsVisible || unit.IsIllusion || unit == localHero)
                {
                    continue;
                }

                if (!ItemAllyChangedValue && unit.IsAlly(localHero))
                {
                    continue;
                }


                var screenPosition = GameManager.WorldToScreen(unit.Position + new Vector3(0, 0, unit.HealthBarOffset));

                for (var i = 0; i < 6; i++)
                {
                    var item = unit.Inventory.GetItem((ItemSlot)i);
                    if (item == null)
                    {
                        continue;
                    }

                    Vector2 newScreenPosition = new Vector2(screenPosition.X + (i * (66 * scaling)), screenPosition.Y);
                    var rect2 = new RectangleF(newScreenPosition.X - (100 * scaling), newScreenPosition.Y - (180 * scaling), 65 * scaling, 53 * scaling);

                    if (i >= 3)
                    {
                        rect2 = new RectangleF(newScreenPosition.X - (298 * scaling), newScreenPosition.Y - (127 * scaling), 65 * scaling, 53 * scaling);
                    }

                    var textureName = $@"items\{item.Name}.png";
                    RendererManager.LoadTextureFromResource(textureName);
                    RendererManager.DrawTexture(textureName, rect2, 0.94f);

                    var positionItem = new Vector2(screenPosition.X - 45 + (i * (66 * scaling)), screenPosition.Y - 90);
                    var cooldown = item.Cooldown;
                    if (cooldown != 0)
                    {
                        RendererManager.DrawText(((int)cooldown).ToString(), positionItem, Color.White, "Impact", 20);
                    }
                }

                var pos = new Vector2(screenPosition.X - 102 * scaling, screenPosition.Y - 157 * scaling);
                var start1 = pos + new Vector2(0, 30 * scaling);
                var end1 = pos + new Vector2(200 * scaling, 30 * scaling);
                RendererManager.DrawLine(start1, end1, Color.Black, 4 * scaling);
                pos = new Vector2(screenPosition.X - 35 * scaling, screenPosition.Y - 180 * scaling);
                for (var i = 0; i < 2; i++)
                {
                    var start2 = pos + new Vector2(i * 67.2f * scaling, 0);
                    var end2 = pos + new Vector2(i * 67.2f * scaling, 108 * scaling);
                    RendererManager.DrawLine(start2, end2, Color.Black, 4 * scaling);
                }
            }
        }
    }
}
