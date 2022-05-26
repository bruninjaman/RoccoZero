using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using log4net;
using PlaySharp.Toolkit.Logging;

namespace BAIO.UnitManager
{
    public class Updater : IDisposable
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly BaseHero _main;

        private readonly Dictionary<int, UnitManager> allUnits = new Dictionary<int, UnitManager>();

        private IUpdateHandler Handler;

        public IEnumerable<UnitManager> AllUnits
        {
            get
            {
                return allUnits.Values.Where(x =>
                    x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team == _main.Context.Owner.Team && (!(x.Unit is Hero) || x.Unit.IsIllusion) &&
                    x.Unit.IsControllable && x.Unit.CanMove());
            }
        }

        public Updater(BaseHero main)
        {
            _main = main;

            try
            {
                foreach (var unit in EntityManager<Unit>.Entities)
                {
                    if (unit.Team == main.Context.Owner.Team && unit.IsControllable && unit.IsControllableByPlayer(ObjectManager.LocalPlayer) &&
                        (!(unit is Hero) || unit.IsIllusion)
                        && unit.NetworkName != "CDOTA_BaseNPC_Clinkz_Skeleton_Army" && unit.NetworkName != "CDOTA_Unit_ZeusCloud"
                        && unit.NetworkName != "CDOTA_Unit_Courier" && unit.Name != "npc_dota_wraith_king_skeleton_warrior"  &&
                        unit.Name != "npc_dota_juggernaut_healing_ward" && !unit.Name.Contains("npc_dota_pugna_nether_ward")
                        && !unit.Name.Contains("npc_dota_shadow_shaman_ward")
                        && unit != ObjectManager.LocalHero)
                    {
                        allUnits[unit.Handle.Handle] = new UnitManager(_main, unit);
                        //Log.Debug($"new unit found");
                    }
                }

                Handler = UpdateManager.Subscribe(OnUpdate);
            }
            catch
            {
                //Ignore
            }

            EntityManager<Unit>.EntityRemoved += EntityManagerOnEntityRemoved;
        }

        private void OnUpdate()
        {
            foreach (var unit in EntityManager<Unit>.Entities.Where(x =>
                x != null && x.IsValid && x.IsAlly(_main.Context.Owner) && x.IsControllable &&
                x.IsControllableByPlayer(ObjectManager.LocalPlayer)
                && (!(x is Hero) || x.IsIllusion)
                && x.NetworkName != "CDOTA_BaseNPC_Clinkz_Skeleton_Army" && x.NetworkName != "CDOTA_Unit_ZeusCloud"
                && x.NetworkName != "CDOTA_Unit_Courier" && x.Name != "npc_dota_wraith_king_skeleton_warrior" &&
                x.Name != "npc_dota_juggernaut_healing_ward"
                && !x.Name.Contains("npc_dota_pugna_nether_ward") && !x.Name.Contains("npc_dota_shadow_shaman_ward")
                && x != ObjectManager.LocalHero)
            )
            {
                if (!allUnits.ContainsKey(unit.Handle.Handle))
                {
                    allUnits[unit.Handle.Handle] = new UnitManager(_main, unit);
                    //Log.Debug($"new unit added");
                }
            }
        }

        private void EntityManagerOnEntityRemoved(object sender, Unit unit)
        {
            var handle = unit.Handle.Handle;
            if (!allUnits.ContainsKey(handle))
            {
                return;
            }

            if (allUnits.ContainsKey(handle))
            {
                allUnits.Remove(handle);
            }
        }

        public void Dispose()
        {
            EntityManager<Unit>.EntityRemoved -= EntityManagerOnEntityRemoved;
            UpdateManager.Unsubscribe(OnUpdate);
            allUnits.Clear();
        }
    }

}
