using System.Collections.Generic;

using Divine;

using Ensage.Common.Extensions;
using Ensage.SDK.Abilities;

namespace TinkerCrappahilationPaid
{
    public class TargetClass
    {
        public Hero Hero;
        public float Health { get; set; }
        public float HealthAfterFirstCast { get; set; }
        public float HealthAfterFirstCastWithoutRange { get; set; }
        public float HealthWithoutRange { get; set; }
        public float DamageTaken { get; set; }
        public float DamageTakenFromFirstCast { get; set; }
        public float DamageTakenFromFirstCastWithoutRange { get; set; }
        public float DamageTakentWithoutRange { get; set; }
        public bool WillDieAfterFirstCast { get; set; }
        public List<ActiveAbility> AbilitiesForKillSteal { get; set; }

        public TargetClass(Hero hero)
        {
            Hero = hero;

            Health = hero.Health;
            var gotIt = false;
            if (Hero == null || hero.Inventory?.Items == null)
                return;
            foreach (var item in Hero.Inventory.Items)
            {
                var id = item.Id;
                switch (id)
                {
                    case AbilityId.item_faerie_fire:
                        if (!item.CanBeCasted())
                            continue;
                        if (gotIt)
                            continue;
                        gotIt = true;
                        Health += 85;
                        break;

                    case AbilityId.item_infused_raindrop:
                        if (!item.CanBeCasted())
                            continue;
                        Health += 120;
                        break;
                }
            }

            HealthAfterFirstCast = Health;
            HealthWithoutRange = Health;
            HealthAfterFirstCastWithoutRange = Health;

            DamageTaken = 0;
            DamageTakenFromFirstCast = 0;
            DamageTakenFromFirstCastWithoutRange = 0;
            DamageTakentWithoutRange = 0;
            
            AbilitiesForKillSteal = new List<ActiveAbility>();
        }
    }
}