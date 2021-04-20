using System;
using System.Linq;
using System.Threading.Tasks;

using Divine.BeAware.Data;
using Divine.BeAware.Helpers;
using Divine.BeAware.MenuManager.PartialMapHack;
using Divine.BeAware.ShowMeMore;
using Divine.BeAware.ShowMeMore.MoreInformation;
using Divine.SDK.Extensions;
using Divine.SDK.Managers.Log;

using SharpDX;

namespace Divine.BeAware.Entities
{
    internal sealed class UnitMonitor
    {
        private readonly PartialMapHackMenu PartialMapHackMenu;

        private readonly MiranaArrow MiranaArrow;

        private readonly InvokerSunStrike InvokerSunStrike;

        private readonly KunkkaTorrent KunkkaTorrent;

        private readonly LeshracSplitEarth LeshracSplitEarth;

        private readonly LinaLightStrikeArray LinaLightStrikeArray;

        //private readonly IllusionShow IllusionShow;

        private readonly Verification Verification;

        private static readonly Log Log = LogManager.GetCurrentClassLogger();

        public UnitMonitor(Common common)
        {
            PartialMapHackMenu = common.MenuConfig.PartialMapHackMenu;

            Verification = common.Verification;

            MiranaArrow = common.MiranaArrow;
            InvokerSunStrike = common.InvokerSunStrike;
            KunkkaTorrent = common.KunkkaTorrent;
            LeshracSplitEarth = common.LeshracSplitEarth;
            LinaLightStrikeArray = common.LinaLightStrikeArray;

            //IllusionShow = common.IllusionShow;

            EntityManager.EntityAdded += OnEntityAdded;
        }

        public void Dispose()
        {
            EntityManager.EntityAdded -= OnEntityAdded;
        }

        private Hero FindHeroWithSpells(AbilityId abilityId)
        {
            return EntityManager.GetEntities<Hero>().FirstOrDefault(x => !x.IsIllusion && x.Spellbook.Spells.Any(v => v.Id == abilityId));
        }

        private bool IsIgnoreHero(Vector3 position)
        {
            foreach (var hero in EntityManager.GetEntities<Hero>())
            {
                if (hero.IsIllusion)
                {
                    continue;
                }

                if (hero.HeroId == HeroId.npc_dota_hero_invoker)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnEntityAdded(EntityAddedEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    var entity = e.Entity;
                    if (!entity.IsValid || entity is not Unit unit)
                    {
                        return;
                    }

                    /*if (IllusionShow.Entity(unit))
                    {
                        return;
                    }*/

                    if (unit.ClassId != ClassId.CDOTA_BaseNPC)
                    {
                        return;
                    }

                    if (MiranaArrow.Entity(unit))
                    {
                        return;
                    }

                    var hero = unit.Owner as Hero;
                    if (hero != null)
                    {
                        if (InvokerSunStrike.Entity(unit, hero))
                        {
                            return;
                        }

                        if (KunkkaTorrent.Entity(unit, hero))
                        {
                            return;
                        }

                        if (LeshracSplitEarth.Entity(unit, hero))
                        {
                            return;
                        }

                        if (LinaLightStrikeArray.Entity(unit, hero))
                        {
                            return;
                        }

                        var units = UnitDictionaries.Units.FirstOrDefault(x => hero.HeroId == x.Key);
                        if (units.Key > 0)
                        {
                            Spells(hero, units.Value, unit.Position);
                            return;
                        }
                    }

                    var unitDayVision = UnitDictionaries.UnitsDayVision.FirstOrDefault(x => unit.DayVision == x.Key);
                    if (unitDayVision.Key > 0)
                    {
                        var abilityId = unitDayVision.Value;
                        Spells(FindHeroWithSpells(abilityId), abilityId, unit.Position);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        private void Spells(Hero hero, AbilityId abilityId, Vector3 position)
        {
            try
            {
                var isDangerousSpell = DangerousAbility.DangerousSpells.Contains(abilityId);
                if (hero == null)
                {
                    Verification.EntityVerification(position, "npc_dota_hero_default", abilityId, 0, isDangerousSpell);
                    return;
                }

                if (hero.IsAlly(EntityManager.LocalHero))
                {
                    return;
                }

                if (!hero.IsVisible || PartialMapHackMenu.WhenIsVisibleItem && isDangerousSpell)
                {
                    Verification.EntityVerification(position, hero.Name, abilityId, hero.Player.Id + 1, isDangerousSpell);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}