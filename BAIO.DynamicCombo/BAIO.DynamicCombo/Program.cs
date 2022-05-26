using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAIO;
using Ensage;
using Ensage.SDK.Service;
using Ensage.SDK.Service.Metadata;

namespace BAIO.DynamicCombo
{
    public class Program
    {
        public static HashSet<HeroId> SupportedHeroes()
        {
            return new HashSet<HeroId>
            {
                HeroId.npc_dota_hero_base
            };
        }
    }
}
