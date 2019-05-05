using System.Collections.Generic;
using System.Linq;
using Smod2;
using Smod2.EventHandlers;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem.Events;
using scp4aiur;
namespace Finalbossassasinpillars
{
    partial class PlayersEvents : IEventHandlerSetRole, IEventHandlerWaitingForPlayers, IEventHandlerSetConfig, IEventHandlerSetSCPConfig, IEventHandlerPlayerHurt,
    IEventHandlerRoundEnd, IEventHandlerRoundStart
    {
        private Finalbossassasinpillars plugin;
        public PlayersEvents(Finalbossassasinpillars plugin)
        {
            this.plugin = plugin;
        }
        static Dictionary<string, float> Jugadores = new Dictionary<string, float>();
		static Dictionary<int, bool> Peanutpassive = new Dictionary<int, bool>();
		float var1 = 0;
        float var2 = 0;
        string MVP = "0";
        string Name = "nadie";

        public static IEnumerable<float> Teleport1(Player player)
        {
            yield return 2f;
            player.Teleport(PluginManager.Manager.Server.Map.GetSpawnPoints(Role.NTF_COMMANDER)[1], true);
        }
        public static IEnumerable<float> Teleport2(Player player)
        {
            yield return 2f;
            player.Teleport(PluginManager.Manager.Server.Map.GetSpawnPoints(Role.CHAOS_INSURGENCY)[1], true);
        }
		public static IEnumerable<float> Peanutpass(Player player)
		{
			while (true)
			{
				if (player.TeamRole.Role == Role.SCP_173)
				{
					player.SetGhostMode(true, true, false);
					yield return 5f;
					player.SetGhostMode(false);
				}
			}
		}
		public static IEnumerable<float> Bomb()
        {
            yield return 2f;
            PluginManager.Manager.Server.Map.AnnounceCustomMessage("Alert . Containment breach Detected . Automatic Self Destruction in 3 . 2 . 1");
            yield return 7f;           
            PluginManager.Manager.Server.Map.DetonateWarhead();
            yield return 1f;
            PluginManager.Manager.Server.Map.DetonateWarhead();
            yield return 1f;
            PluginManager.Manager.Server.Map.DetonateWarhead();
        }

        public void OnSetConfig(SetConfigEvent ev)
        {
            switch (ev.Key)
            {                                
                case "auto_warhead_start":
                    ev.Value = 1800;
                    break;
                case "auto_warhead_start_lock":
                    ev.Value = false;
                    break;
                case "default_item_classd":
                    ev.Value = new int[] { 25,24,25,25,30,14 };
                    break;
                case "default_item_scientist":
                    ev.Value = new int[] { 21,20,25,26,16,14 }; 
                    break;
                case "minimum_MTF_time_to_spawn":
                    ev.Value = 210;
                    break;
                case "maximum_MTF_time_to_spawn":
                    ev.Value = 210;
                    break;
            }
        }

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
			if (ev.Player.TeamRole.Role == Role.SCP_173)
			{
				Peanutpassive.Add(ev.Player.PlayerId, false);
			}
			if ((ev.Player.TeamRole.Role == Role.CHAOS_INSURGENCY)||(ev.Player.TeamRole.Role == Role.FACILITY_GUARD))
            {
                ev.Player.ChangeRole(Role.CLASSD);
                Timing.Run(Teleport1(ev.Player));
            }
            if((ev.Player.TeamRole.Role == Role.CLASSD)||(ev.Player.TeamRole.Role == Role.SCIENTIST))
            {
                Timing.Run(Teleport1(ev.Player));
            }
            if(ev.Player.TeamRole.Role == Role.SCP_173) { ev.Player.AddHealth((15000)); Timing.Run(Teleport2(ev.Player)); }
            if(ev.Player.TeamRole.Team != Team.SCP) { ev.Player.SetAmmo(AmmoType.DROPPED_5, 600); ev.Player.SetAmmo(AmmoType.DROPPED_7, 600);
                ev.Player.SetAmmo(AmmoType.DROPPED_9, 600);   }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
			Name = "nadie";
			MVP = "0";
			var2 = 0;
			var1 = 0;
		}

		public void OnSetSCPConfig(SetSCPConfigEvent ev)
        {
            ev.Ban049 = true;
            ev.Ban079 = true;
            ev.Ban106 = true;
            ev.Ban173 = true;
            ev.Ban939_53 = true;
            ev.Ban939_89 = true;
            ev.SCP173amount = 4;
        }

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
           if(ev.Player.TeamRole.Role == Role.SCP_173)
            {
                Jugadores[ev.Player.SteamId] = (Jugadores[ev.Player.SteamId] + ev.Damage);
				if((ev.Player.GetHealth() < 3000)&&(Peanutpassive[ev.Player.PlayerId] != true))
				{
					Peanutpassive[ev.Player.PlayerId] = true;
					Timing.Run(Peanutpass(ev.Player));
				}
			}
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            
            foreach (KeyValuePair<string, float> key in Jugadores)
            {
                var1 = key.Value;
                if (var1 > var2) { var2 = var1; MVP = key.Key; }
            }
            foreach (Player player in PluginManager.Manager.Server.GetPlayers())
            {
                if(player.SteamId == MVP)
                {
                    Name = player.Name;
                }
            }
            PluginManager.Manager.Server.Map.Broadcast(10, "El Mejor jugador ha sido  " + Name +" El daño que ha causado" + var2.ToString(),false);
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            Timing.Run(Bomb());
           foreach(Player player in PluginManager.Manager.Server.GetPlayers())
           {
                if(player.TeamRole.Role != Role.SCP_173)
                {
                    Jugadores.Add(player.SteamId, 0);
                }
                
           }
        }
    }
}
