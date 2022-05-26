using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.SDK.Service;
using log4net;
using PlaySharp.Toolkit.Logging;

namespace BAIO.UnitManager
{
    public class UnitManager : IEquatable<UnitManager>
    {
        public readonly BaseHero Main;
        public Unit Unit;
        public int Handle;
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UnitMovementManager UnitMovementManager { get; set; }

        public Ability Ability { get; set; }
        public Ability Ability2 { get; set; }
        public Ability Ability3 { get; set; }
        public Ability Ability4 { get; set; }

        public UnitManager(BaseHero main, Unit unit)
        {
            Main = main;
            Unit = unit;
            Handle = unit.Handle.Handle;
            Ability = unit.Spellbook.Spells.Count(x => x.Name != "lone_druid_spirit_bear_return" && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 0 ? unit.Spellbook.SpellQ : null;
            Ability2 = unit.Spellbook.Spells.Count(x => !x.Name.StartsWith("special_") && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 1 ? unit.Spellbook.SpellW : null;
            Ability3 = unit.Spellbook.Spells.Count(x => !x.Name.StartsWith("special_") && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 2 ? unit.Spellbook.SpellE : null;
            Ability4 = unit.Spellbook.Spells.Count(x => !x.Name.StartsWith("special_") && !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive)) > 3 ? unit.Spellbook.SpellR : null;

            if (unit.Name != "npc_dota_templar_assassin_psionic_trap")
            {
                UnitMovementManager = new UnitMovementManager(new EnsageServiceContext(unit));
            }
        }

        public static bool operator ==(UnitManager left, UnitManager right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnitManager left, UnitManager right)
        {
            return !Equals(left, right);
        }

        public bool Equals(UnitManager entity)
        {
            if (entity is null)
            {
                return false;
            }

            if (ReferenceEquals(this, entity))
            {
                return true;
            }

            return Handle == entity.Handle;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnitManager);
        }

        public override int GetHashCode()
        {
            return Handle;
        }
    }
}
