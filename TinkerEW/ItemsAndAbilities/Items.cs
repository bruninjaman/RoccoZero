using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.Helpers;

namespace TinkerEW.ItemsAndAbilities.Items;

internal partial class Items
{
    private readonly Sleeper UpdateSleeper = new();
    public readonly SoulRing soulRing;
    public readonly GhostScepter ghostScepter;
    public readonly EternalShroud eternalShroud;
    public readonly VeilOfDiscord veilOfDiscord;
    public readonly Blink blink;
    public readonly Dagon dagon;
    public readonly EtherealBlade etherealBlade;
    public readonly ScytheOfVyse scytheOfVyse;
    public readonly ShivasGuard shivasGuard;
    public readonly Orchid orchid;
    public readonly Bloodthorn bloodthorn;
    public readonly RodOfAtos rodOfAtos;
    public readonly GlimmerCape glimmerCape;
    public readonly GuardianGreaves guardianGreaves;
    public readonly LotusOrb lotusOrb;
    public readonly Nullifier nullifier;

    public Items()
    {
        soulRing = new SoulRing();
        ghostScepter = new GhostScepter();
        eternalShroud = new EternalShroud();
        veilOfDiscord = new VeilOfDiscord();
        blink = new Blink();
        dagon = new Dagon();
        etherealBlade = new EtherealBlade();
        scytheOfVyse = new ScytheOfVyse();
        shivasGuard = new ShivasGuard();
        orchid = new Orchid();
        bloodthorn = new Bloodthorn();
        rodOfAtos = new RodOfAtos();
        glimmerCape = new GlimmerCape();
        guardianGreaves = new GuardianGreaves();
        lotusOrb = new LotusOrb();
        nullifier = new Nullifier();
    }

    public void Update()
    {
        if (!UpdateSleeper.Sleeping)
        {
            var localHero = EntityManager.LocalHero;

            if (localHero == null)
                return;

            //Update items here
            soulRing.Update(localHero.GetItemById(AbilityId.item_soul_ring));
            ghostScepter.Update(localHero.GetItemById(AbilityId.item_ghost));
            eternalShroud.Update(localHero.GetItemById(AbilityId.item_eternal_shroud));
            veilOfDiscord.Update(localHero.GetItemById(AbilityId.item_veil_of_discord));
            blink.Update(localHero.Inventory?.MainItems.FirstOrDefault(x => x.Id == AbilityId.item_blink
            || x.Id == AbilityId.item_overwhelming_blink
            || x.Id == AbilityId.item_swift_blink
            || x.Id == AbilityId.item_arcane_blink));
            dagon.Update(localHero.Inventory?.MainItems.FirstOrDefault(x => LocalizationHelper.LocalizeAbilityName(x.Name) == "Dagon"));
            etherealBlade.Update(localHero.GetItemById(AbilityId.item_ethereal_blade));
            scytheOfVyse.Update(localHero.GetItemById(AbilityId.item_sheepstick));
            shivasGuard.Update(localHero.GetItemById(AbilityId.item_shivas_guard));
            orchid.Update(localHero.GetItemById(AbilityId.item_orchid));
            bloodthorn.Update(localHero.GetItemById(AbilityId.item_bloodthorn));
            rodOfAtos.Update(localHero.GetItemById(AbilityId.item_rod_of_atos));
            glimmerCape.Update(localHero.GetItemById(AbilityId.item_glimmer_cape));
            guardianGreaves.Update(localHero.GetItemById(AbilityId.item_guardian_greaves));
            lotusOrb.Update(localHero.GetItemById(AbilityId.item_lotus_orb));
            nullifier.Update(localHero.GetItemById(AbilityId.item_nullifier));

            UpdateSleeper.Sleep(500);
        }
    }
}
