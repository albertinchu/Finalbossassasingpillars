using Smod2;
using Smod2.Attributes;
using scp4aiur;
namespace Finalbossassasinpillars
{
	[PluginDetails(
		author = "Albertinchu",
		name = "Finalbossassasinpillars",
		description = "4 SCPS 173 con 18000 de vida aproximadamente, muchos jugadores...",
		id = "albertinchu.gamemode.Finalbossassasinpillars",
		version = "1.0.0",
		SmodMajor = 3,
		SmodMinor = 0,
		SmodRevision = 0
		)]
	public class Finalbossassasinpillars : Plugin
	{

		public override void OnDisable()
		{
			this.Info("Finalbossassasinpillars - Desactivado");
		}

		public override void OnEnable()
		{
			Info("Finalbossassasinpillars - activado.");
		}

		public override void Register()
		{
			GamemodeManager.Manager.RegisterMode(this);
			Timing.Init(this);
			this.AddEventHandlers(new PlayersEvents(this));

		}
		public void RefreshConfig()
		{


		}
	}

}

