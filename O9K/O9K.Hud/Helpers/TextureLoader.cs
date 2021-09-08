namespace O9K.Hud.Helpers;

using System;
using System.Collections.Generic;

using Core.Entities.Abilities.Base;
using Core.Entities.Heroes;
using Core.Entities.Units;
using Core.Logger;
using Core.Managers.Entity;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Players;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Renderer;

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
        RendererManager.LoadImage(id);
        RendererManager.LoadImage(id, AbilityImageType.Round);
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

            RendererManager.LoadImage(id);
            RendererManager.LoadImage(id, UnitImageType.RoundUnit);
            RendererManager.LoadImage(id, UnitImageType.MiniUnit);

            this.loaded.Add(id.ToString());
        }

        RendererManager.LoadImage("o9k.x", @"panorama\images\hud\reborn\ping_icon_retreat_psd.vtex_c");
        RendererManager.LoadImage("rune_arcane", ImageType.RoundAbility);
        RendererManager.LoadImage("rune_doubledamage", ImageType.RoundAbility);
        RendererManager.LoadImage("rune_haste", ImageType.RoundAbility);
        RendererManager.LoadImage("rune_invis", ImageType.RoundAbility);
        RendererManager.LoadImage("rune_regen", ImageType.RoundAbility);
        RendererManager.LoadImage("npc_dota_roshan", "npc_dota_hero_roshan", ImageType.Unit);
        RendererManager.LoadImage("npc_dota_roshan", "npc_dota_hero_roshan", ImageType.RoundUnit);

        RendererManager.LoadImage(AbilityId.item_bottle);
        RendererManager.LoadImage("item_bottle_arcane", ImageType.Ability);
        RendererManager.LoadImage("item_bottle_bounty", ImageType.Ability);
        RendererManager.LoadImage("item_bottle_doubledamage", ImageType.Ability);
        RendererManager.LoadImage("item_bottle_haste", ImageType.Ability);
        RendererManager.LoadImage("item_bottle_illusion", ImageType.Ability);
        RendererManager.LoadImage("item_bottle_invisibility", ImageType.Ability);
        RendererManager.LoadImage("item_bottle_regeneration", ImageType.Ability);
        RendererManager.LoadImage("item_bottle_water", ImageType.Ability);

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
                RendererManager.LoadImage(ability.TextureName, ImageType.Ability);
                RendererManager.LoadImage(ability.TextureName, ImageType.RoundAbility);
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
                RendererManager.LoadImage(hero.TextureName, ImageType.Unit);
                RendererManager.LoadImage(hero.TextureName, ImageType.RoundUnit);
                RendererManager.LoadImage(hero.TextureName, ImageType.MiniUnit);
            }
            else if (unit.IsUnit && !unit.IsCreep && !unit.IsBuilding)
            {
                RendererManager.LoadImage(unit.DefaultName, ImageType.Unit);
            }

            this.loaded.Add(unit.TextureName);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}