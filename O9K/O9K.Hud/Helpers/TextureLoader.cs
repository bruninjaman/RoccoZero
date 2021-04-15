namespace O9K.Hud.Helpers
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Heroes;
    using Core.Entities.Units;
    using Core.Logger;
    using Core.Managers.Entity;

    using Divine;

    using Modules;

    internal class TextureLoader : IHudModule
    {
        private readonly HashSet<string> loaded = new HashSet<string>
        {
            nameof(AbilityId.courier_take_stash_and_transfer_items),
            nameof(AbilityId.courier_transfer_items_to_other_player),
        };

        public void Activate()
        {
            this.LoadTextures();

            EntityManager9.UnitAdded += this.OnUnitAdded;
            EntityManager9.AbilityAdded += this.OnAbilityAdded;
        }

        public void Dispose()
        {
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
        }

        private void LoadAbilityTexture(AbilityId id)
        {
            RendererManager.LoadTexture(id);
            RendererManager.LoadTexture(id, AbilityTextureType.Round);
        }

        private void LoadTextures()
        {
            foreach (var player in EntityManager.GetEntities<Player>())
            {
                var id = player.SelectedHeroId;
                if (id == HeroId.npc_dota_hero_base)
                {
                    continue;
                }

                RendererManager.LoadTexture(id);
                RendererManager.LoadTexture(id, UnitTextureType.RoundUnit);
                RendererManager.LoadTexture(id, UnitTextureType.MiniUnit);

                this.loaded.Add(id.ToString());
            }

            RendererManager.LoadTexture("o9k.x", @"panorama\images\hud\reborn\ping_icon_retreat_psd.vtex_c");
            RendererManager.LoadTexture("rune_arcane", TextureType.RoundAbility);
            RendererManager.LoadTexture("rune_doubledamage", TextureType.RoundAbility);
            RendererManager.LoadTexture("rune_haste", TextureType.RoundAbility);
            RendererManager.LoadTexture("rune_invis", TextureType.RoundAbility);
            RendererManager.LoadTexture("rune_regen", TextureType.RoundAbility);
            RendererManager.LoadTexture("npc_dota_roshan", "npc_dota_hero_roshan", TextureType.Unit);
            RendererManager.LoadTexture("npc_dota_roshan", "npc_dota_hero_roshan", TextureType.RoundUnit);

            RendererManager.LoadTexture(AbilityId.item_bottle);
            RendererManager.LoadTexture("item_bottle_arcane", TextureType.Ability);
            RendererManager.LoadTexture("item_bottle_bounty", TextureType.Ability);
            RendererManager.LoadTexture("item_bottle_doubledamage", TextureType.Ability);
            RendererManager.LoadTexture("item_bottle_haste", TextureType.Ability);
            RendererManager.LoadTexture("item_bottle_illusion", TextureType.Ability);
            RendererManager.LoadTexture("item_bottle_invisibility", TextureType.Ability);
            RendererManager.LoadTexture("item_bottle_regeneration", TextureType.Ability);
            RendererManager.LoadTexture("item_bottle_water", TextureType.Ability);

            this.LoadAbilityTexture(AbilityId.item_smoke_of_deceit);
            this.LoadAbilityTexture(AbilityId.item_ward_sentry);
            this.LoadAbilityTexture(AbilityId.item_ward_observer);
        }

        private void OnAbilityAdded(Ability9 ability)
        {
            try
            {
                if (this.loaded.Contains(ability.TextureName))
                {
                    return;
                }

                if (!ability.IsTalent)
                {
                    RendererManager.LoadTexture(ability.TextureName, TextureType.Ability);
                    RendererManager.LoadTexture(ability.TextureName, TextureType.RoundAbility);
                }

                this.loaded.Add(ability.TextureName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUnitAdded(Unit9 unit)
        {
            try
            {
                if (this.loaded.Contains(unit.TextureName))
                {
                    return;
                }

                if (unit is Hero9 hero)
                {
                    RendererManager.LoadTexture(hero.TextureName, TextureType.Unit);
                    RendererManager.LoadTexture(hero.TextureName, TextureType.RoundUnit);
                    RendererManager.LoadTexture(hero.TextureName, TextureType.MiniUnit);
                }
                else if (unit.IsUnit && !unit.IsCreep)
                {
                    RendererManager.LoadTexture(unit.DefaultName, TextureType.Unit);
                }

                this.loaded.Add(unit.TextureName);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}