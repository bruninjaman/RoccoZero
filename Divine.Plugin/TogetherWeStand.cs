using Divine;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SDK.Extensions;
using Divine.SDK.Helpers;

using SharpDX;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CustomGame
{
    internal class TogetherWeStand
    {
        private readonly Sleeper Sleeper = new Sleeper();

        public static MenuSwitcher OnOff { get; set; }

        public static MenuToggleKey PressKey { get; set; }

        public static MenuSwitcher ExpensiveItems { get; set; }

        public static MenuSlider RadiusSearch { get; set; }


        private static readonly Hero localHero = EntityManager.LocalHero;

        public TogetherWeStand(Menu menu)
        {
            OnOff = menu.CreateSwitcher("Enable", true);
            PressKey = menu.CreateToggleKey("Pick Up Item Key", Key.D);
            ExpensiveItems = menu.CreateSwitcher("Pick up only expensive items", true);
            RadiusSearch = menu.CreateSlider("Radius Search", 300, 300, 2000);

            OnOff.ValueChanged += OnOff_ValueChanged;
        }

        private void OnOff_ValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
        {
            if (e.Value)
            {
                PressKey.ValueChanged += PressKey_ValueChanged;
            }
            else
            {
                PressKey.ValueChanged -= PressKey_ValueChanged;
            }

        }

        private Dictionary<ItemSlot, Item> StartingItems = new Dictionary<ItemSlot, Item>();

        private void PressKey_ValueChanged(MenuToggleKey toggleKey, ToggleKeyEventArgs e)
        {

            if (e.Value)
            {

                for (int i = 0; i <= 8; i++)
                {

                    var item = localHero.Inventory.GetItem((ItemSlot)i);

                    StartingItems.Add((ItemSlot)i, item);
                }
                RendererManager.Draw += RendererManager_Draw;
                UpdateManager.IngameUpdate += GameManager_GameUpdate;
            }
            else
            {
                StartingItems.Clear();
                RendererManager.Draw -= RendererManager_Draw;
                UpdateManager.IngameUpdate -= GameManager_GameUpdate;
            }
        }

        private void RendererManager_Draw()
        {
            var screenPosition = RendererManager.WorldToScreen(localHero.Position + new Vector3(0, 0, localHero.HealthBarOffset));
            RendererManager.DrawText("PickUp Working!", screenPosition, Color.Red, 30);
            RendererManager.DrawText(GameManager.MousePosition.ToString(), GameManager.MouseScreenPosition, Color.Red, "Arial", 25f);
            //Console.WriteLine(GameManager.MousePosition.ToString());
        }

        private void GameManager_GameUpdate()
        {
            if (Sleeper.Sleeping)
            {
                return;
            }

            Sleeper.Sleep(100);

            var rune = EntityManager.GetEntities<PhysicalItem>().FirstOrDefault(x => x.IsVisible && x.IsAlive && x.Distance2D(localHero.Position) <= RadiusSearch);


            foreach (var courier in EntityManager.GetEntities<Courier>().Where(x => x.IsAlive && x.IsAlly(localHero)))
            {
                var itemsCourier = courier.Inventory.GetFreeSlots((ItemSlot)0, (ItemSlot)8);
                if (itemsCourier.ToArray().Length != 0)
                {
                    courier.PickUp(rune);
                }
                else
                {

                    var PosCura = courier.Position;
                    var PosCuraNeed = new Vector3(-2319, -1872, 278);
                    courier.Move(PosCuraNeed, false);
                    Console.WriteLine(courier.Position + " == " + PosCuraNeed);
                    if (PosCuraNeed.Distance2D(PosCura) <= 200)
                    {
                        for (int i = 0; i <= 8; i++)
                        {


                            var item = courier.Inventory.GetItem((ItemSlot)i);
                            var Value = StartingItems[(ItemSlot)i];
                            if (Value == null)
                            {
                                item.Sell();
                            }
                        }
                    }

                }

            }

            if (localHero.Inventory.GetFreeSlots((ItemSlot)0, (ItemSlot)8).ToArray().Length != 0)
            {
                localHero.PickUp(rune);
            }
            else
            {

            }
        }
    }
}
