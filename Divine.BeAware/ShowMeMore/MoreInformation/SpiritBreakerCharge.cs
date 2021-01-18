using System;
using System.Linq;
using System.Threading.Tasks;

using Divine.BeAware.MenuManager.ShowMeMore;
using Divine.BeAware.MenuManager.ShowMeMore.MoreInformation;
using Divine.Helpers;
using Divine.SDK.Extensions;
using Divine.SDK.Managers.Log;

using SharpDX;

namespace Divine.BeAware.ShowMeMore.MoreInformation
{
    internal sealed class SpiritBreakerCharge : Base
    {
        private readonly SpiritBreakerChargeMenu SpiritBreakerChargeMenu;

        private bool OnMinimap;

        private bool ColorScreen;

        private Vector3 startChargePosition;

        private Vector3 endChargePosition;

        public SpiritBreakerCharge(Common common) : base(common)
        {
            SpiritBreakerChargeMenu = MoreInformationMenu.SpiritBreakerChargeMenu;

            RendererManager.LoadTexture(HeroId.npc_dota_hero_spirit_breaker, UnitTextureType.MiniUnit);

            RendererManager.Draw += RendererManagerOnDraw;
        }

        public void Dispose()
        {
            RendererManager.Draw -= RendererManagerOnDraw;
        }

        private void RendererManagerOnDraw()
        {
            if (ColorScreen)
            {
                var color = new Color(
                SpiritBreakerChargeMenu.RedItem.Value,
                SpiritBreakerChargeMenu.GreenItem.Value,
                SpiritBreakerChargeMenu.BlueItem.Value,
                SpiritBreakerChargeMenu.AlphaItem.Value);

                var screenSize = RendererManager.ScreenSize;
                RendererManager.DrawFilledRectangle(new RectangleF(0, 0, screenSize.X, screenSize.Y), Color.Zero, color, 0);
            }

            if (OnMinimap)
            {
                var startPos = startChargePosition.WorldToMinimap();
                var endPos = endChargePosition.WorldToMinimap();
                RendererManager.DrawLine(startPos, endPos, Color.WhiteSmoke, 1);
                RendererManager.DrawTexture(@"mini_heroes\npc_dota_hero_spirit_breaker.png", new RectangleF(startPos.X - 11, startPos.Y - 13, 24, 24));
            }
        }

        public override bool Modifier(Unit unit, Modifier modifier, bool isHero)
        {
            if (!SpiritBreakerChargeMenu.EnableItem || modifier.Name != "modifier_spirit_breaker_charge_of_darkness_vision")
            {
                return false;
            }

            Charge(unit, modifier, isHero);
            return true;
        }

        private async void Charge(Unit unit, Modifier modifier, bool isHero)
        {
            try
            {
                if (unit.Team != LocalHero.Team)
                {
                    return;
                }

                var effectName = "particles/units/heroes/hero_spirit_breaker/spirit_breaker_charge_target.vpcf";
                if (isHero)
                {
                    if (LocalHero.Handle == unit.Handle)
                    {
                        ColorScreen = true;
                    }

                    effectName = "materials/ensage_ui/particles/spirit_breaker_charge_target.vpcf";

                    var position = unit.Position;
                    var pos = Pos(position, SpiritBreakerChargeMenu.OnWorldItem);
                    var minimapPos = MinimapPos(position, SpiritBreakerChargeMenu.OnMinimapItem);

                    Verification.InfoVerification(pos, minimapPos, unit.Name, AbilityId.spirit_breaker_charge_of_darkness, 0, SpiritBreakerChargeMenu.SideMessageItem, SpiritBreakerChargeMenu.SoundItem);

                    if (SpiritBreakerChargeMenu.WriteOnChatItem)
                    {
                        DisplayMessage(unit);
                    }
                }

                ParticleManager.CreateOrUpdateParticle($"ChargeUnit", effectName, EntityManager.GetEntityByHandle(unit.Handle), ParticleAttachment.OverheadFollow);

                var spiritBreaker = EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsIllusion && x.HeroId == HeroId.npc_dota_hero_spirit_breaker);
                var speed = spiritBreaker.GetAbilityById(AbilityId.spirit_breaker_charge_of_darkness).GetAbilitySpecialDataWithTalent(spiritBreaker, "movement_speed");

                var rawGameTime = GameManager.RawGameTime;
                var firstIsVisible = false;

                do
                {
                    var isVisible = spiritBreaker.IsVisible;
                    if (isVisible)
                    {
                        rawGameTime = GameManager.RawGameTime;
                        firstIsVisible = true;
                    }

                    if (firstIsVisible)
                    {
                        startChargePosition = spiritBreaker.Position.Extend(unit.Position, (GameManager.RawGameTime - rawGameTime) * speed);
                        endChargePosition = unit.Position;
                        DrawLine("Charge", startChargePosition, endChargePosition, 150, 185, Color.DarkRed);

                        if (SpiritBreakerChargeMenu.OnMinimapItem)
                        {
                            OnMinimap = true;
                        }

                        if (!isVisible)
                        {
                            DrawRange("Charge", startChargePosition, 100, Color.Red, 180);
                        }
                        else
                        {
                            DrawRangeRemove("Charge");
                        }
                    }

                    await Task.Delay(50);
                }
                while (modifier.IsValid);

                ColorScreen = false;
                OnMinimap = false;

                ParticleManager.RemoveParticle("ChargeUnit");
                DrawRangeRemove("Charge");
                DrawLineRemove("Charge");
                startChargePosition = Vector3.Zero;
            }
            catch (Exception e)
            {
                LogManager.Error(e);
            }
        }

        private void DisplayMessage(Unit unit)
        {
            switch (MenuConfig.LanguageItem)
            {
                case "EN":
                    {
                        DisplayMessage($"say_team Carefully, Spirit Breaker charge on {unit.GetDisplayName()}");

                    }
                    break;

                case "RU":
                    {
                        DisplayMessage($"say_team Осторожно, Spirit Breaker разгон на {unit.GetDisplayName()}", true);
                    }
                    break;
            }
        }
    }
}