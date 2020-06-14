using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace AntiItemCheating
{
	[ApiVersion(2, 1)]
	public class MainPlugin : TerrariaPlugin
	{
		private HookHandler<EventArgs> updateHandler;
		private IList<IItemChecker> checkers;
		private IEnumerable<IItemChecker> validCheckers;

		public override string Name => "advanced item detector";
		public override string Author => "TOFOUT";
		public override string Description => "超进度物品检测";
		public MainPlugin(Main game) : base(game)
		{

		}

		public override void Initialize()
		{
			updateHandler = OnUpdate;
			ServerApi.Hooks.GameUpdate.Register(this, updateHandler);
			GetDataHandlers.PlayerSlot += OnItemSlot;

			checkers = new List<IItemChecker>
			{
				CheckerLoader.LoadBeforeEye(),
				CheckerLoader.LoadBeforeEvil(),
				CheckerLoader.LoadBeforeSkeletron(),
				CheckerLoader.LoadBeforeQueenBee(),
				CheckerLoader.LoadBeforeWall(),
				CheckerLoader.LoadBeforeMechAny(),
				CheckerLoader.LoadBeforeMechAll(),
				CheckerLoader.LoadBeforePlantera(),
				CheckerLoader.LoadBeforeGolem(),
				CheckerLoader.LoadBeforeSolar(),
				CheckerLoader.LoadBeforeStardust(),
				CheckerLoader.LoadBeforeVortex(),
				CheckerLoader.LoadBeforeNebula(),
				CheckerLoader.LoadBeforeMoon()
			};
		}

		protected override void Dispose(bool disposing)
		{
			ServerApi.Hooks.GameUpdate.Deregister(this, updateHandler);
			base.Dispose(disposing);
		}

		private void OnItemSlot(object sender, GetDataHandlers.PlayerSlotEventArgs args)
		{
			var player = TShock.Players[args.PlayerId];
			if (!player.IsLoggedIn || player.HasPermission("tshock.item.spawn") || player.HasPermission("tshock.item.give"))
			{
				return;
			}
			foreach (var checker in validCheckers)
			{
				if (checker.Contains(args.Type))
				{
					LogDetected(args);
					var reason = $"[i:{args.Type}]如果你是清白的,就请来解释一通";
					player.Ban(reason, true, "Server");
					args.Type = 0;
					args.Handled = true;
					player.Disconnect(reason);
				}
			}
		}

		private void OnUpdate(object args)
		{
			validCheckers = checkers.Where(checker => !checker.Obsolete);
			foreach (var item in Main.item)
			{
				if (item.active)
				{
					foreach (var checker in validCheckers)
					{
						if (checker.Contains(item.type))
						{
							item.active = false;
							NetMessage.SendData((int)PacketTypes.ItemOwner, -1, -1, null, item.whoAmI);
							goto end;
						}
					}
				end:;
				}
			}
		}

		private void LogDetected(GetDataHandlers.PlayerSlotEventArgs args)
		{
			var msg = $"{TShock.Players[args.PlayerId].Name} 有违规物品 {Lang.GetItemNameValue(args.Type)}({args.Type})), 已被插件自动封禁";
			TShock.Log.ConsoleInfo(msg);
		}

		private static class CheckerLoader
		{
			#region Eye
			internal static IItemChecker LoadBeforeEye()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedBoss1);
				checker.Add(ItemID.EoCShield);
				checker.Add(ItemID.EyeOfCthulhuPetItem);
				checker.Add(ItemID.EyeOfCthulhuBossBag);
				return checker;
			}
			#endregion
			#region Evil
			internal static IItemChecker LoadBeforeEvil()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedBoss2);
				//checker.Add(ItemID.ShadowScale);
				checker.Add(ItemID.WormScarf);
				checker.Add(ItemID.EaterOfWorldsPetItem);
				checker.Add(ItemID.EaterOfWorldsBossBag);

				//checker.Add(ItemID.TissueSample);
				checker.Add(ItemID.BrainOfConfusion);
				checker.Add(ItemID.BrainOfCthulhuPetItem);
				checker.Add(ItemID.BrainOfCthulhuBossBag);

				//checker.Add(ItemID.NightmarePickaxe, ItemID.DeathbringerPickaxe);

				//checker.Add(ItemID.Hellstone);
				//checker.Add(ItemID.HellstoneBar);
				//checker.Add(ItemID.MoltenHamaxe);
				//checker.Add(ItemID.MoltenPickaxe);
				//checker.Add(ItemID.MoltenHelmet);
				//checker.Add(ItemID.MoltenBreastplate);
				//checker.Add(ItemID.MoltenGreaves);
				return checker;
			}
			#endregion
			#region Skeletron
			internal static IItemChecker LoadBeforeSkeletron()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedBoss3);

				checker.Add(ItemID.NightsEdge);
				checker.Add(ItemID.ShadowKey);
				checker.Add(ItemID.CobaltShield);
				checker.Add(ItemID.ObsidianShield);
				checker.Add(ItemID.Muramasa);
				checker.Add(ItemID.BlueMoon);
				checker.Add(ItemID.Shotgun, ItemID.PhoenixBlaster);
				checker.Add(ItemID.MagicMissile);
				checker.Add(ItemID.HellwingBow);
				checker.Add(ItemID.Sunfury);

				return checker;
			}
			#endregion
			#region QueenBee
			internal static IItemChecker LoadBeforeQueenBee()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedQueenBee);
				checker.Add
				(
					ItemID.BeesKnees,
					ItemID.BeeKeeper,
					ItemID.QueenBeeBossBag,
					ItemID.QueenBeePetItem,
					ItemID.HiveBackpack
				);
				return checker;
			}
			#endregion
			#region Wall
			internal static IItemChecker LoadBeforeWall()
			{
				IItemChecker checker = new DefaultItemChecker(() => Main.hardMode);
				checker.Add(ItemID.RangerEmblem, ItemID.SorcererEmblem, ItemID.SummonerEmblem, ItemID.WarriorEmblem);

				checker.Add(ItemID.CobaltOre, ItemID.CobaltBar);
				checker.Add(ItemID.PalladiumOre, ItemID.PalladiumBar);
				checker.Add(ItemID.MythrilOre, ItemID.MythrilBar);
				checker.Add(ItemID.OrichalcumOre, ItemID.OrichalcumBar);
				checker.Add(ItemID.AdamantiteOre, ItemID.AdamantiteBar);
				checker.Add(ItemID.TitaniumOre, ItemID.TitaniumBar);
				
				checker.Add(ItemID.CobaltSword, ItemID.CobaltRepeater, ItemID.CobaltPickaxe, ItemID.CobaltDrill);
				checker.Add(ItemID.PalladiumSword, ItemID.PalladiumRepeater, ItemID.PalladiumPickaxe, ItemID.PalladiumDrill);
				checker.Add(ItemID.MythrilSword, ItemID.MythrilRepeater, ItemID.MythrilPickaxe, ItemID.MythrilDrill);
				checker.Add(ItemID.OrichalcumSword, ItemID.OrichalcumRepeater, ItemID.OrichalcumPickaxe, ItemID.OrichalcumDrill);
				checker.Add(ItemID.AdamantiteSword, ItemID.AdamantiteRepeater, ItemID.AdamantitePickaxe, ItemID.AdamantiteDrill);
				checker.Add(ItemID.TitaniumSword, ItemID.TitaniumRepeater, ItemID.TitaniumPickaxe, ItemID.TitaniumDrill);

				checker.Add(ItemID.CobaltMask, ItemID.CobaltBreastplate, ItemID.CobaltLeggings, ItemID.CobaltHat, ItemID.CobaltHelmet);
				checker.Add(ItemID.MythrilHood, ItemID.MythrilHat, ItemID.MythrilHelmet, ItemID.MythrilGreaves, ItemID.MythrilChainmail);
				checker.Add(ItemID.AdamantiteHelmet, ItemID.AdamantiteMask, ItemID.AdamantiteHeadgear, ItemID.AdamantiteLeggings, ItemID.AdamantiteBreastplate);

				checker.Add(ItemID.PalladiumHeadgear, ItemID.PalladiumHelmet, ItemID.PalladiumMask, ItemID.PalladiumLeggings, ItemID.PalladiumBreastplate);
				checker.Add(ItemID.OrichalcumMask, ItemID.OrichalcumHelmet, ItemID.OrichalcumHeadgear, ItemID.OrichalcumBreastplate, ItemID.OrichalcumLeggings);
				checker.Add(ItemID.TitaniumMask, ItemID.TitaniumHeadgear, ItemID.TitaniumHelmet, ItemID.TitaniumBreastplate, ItemID.TitaniumLeggings);

				checker.Add(ItemID.CobaltNaginata);
				checker.Add(ItemID.MythrilHalberd);
				checker.Add(ItemID.AdamantiteGlaive);
				checker.Add(ItemID.PalladiumPike);
				checker.Add(ItemID.OrichalcumHalberd);
				checker.Add(ItemID.TitaniumTrident);

				checker.Add(ItemID.OnyxBlaster);
				checker.Add(ItemID.DaedalusStormbow);
				checker.Add(ItemID.FetidBaghnakhs);
				checker.Add(ItemID.DartPistol);
				checker.Add(ItemID.DartRifle);
				checker.Add(ItemID.ChainGuillotines);

				checker.Add(ItemID.AnkhShield);

				return checker;
			}
			#endregion
			#region MechAny
			internal static IItemChecker LoadBeforeMechAny()
			{
				IItemChecker checker = new DefaultItemChecker(()=>NPC.downedMechBossAny);
				checker.Add(ItemID.HallowedBar);
				checker.Add(ItemID.HallowedGreaves, ItemID.HallowedHelmet, ItemID.HallowedHood, ItemID.HallowedMask, ItemID.HallowedPlateMail);
				checker.Add(ItemID.Excalibur, ItemID.HallowedRepeater, ItemID.Gungnir);
				checker.Add(ItemID.UnholyTrident);
				return checker;
			}
			#endregion
			#region MechAll
			internal static IItemChecker LoadBeforeMechAll()
			{
				var checker = new DefaultItemChecker(() => NPC.downedMechBoss1 & NPC.downedMechBoss2 & NPC.downedMechBoss3);
				checker.Add(ItemID.PickaxeAxe);
				return checker;
			}
			#endregion
			#region Plantera
			internal static IItemChecker LoadBeforePlantera()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedPlantBoss);
				checker.Add(ItemID.TheEyeOfCthulhu);
				checker.Add(ItemID.DeadlySphereStaff, ItemID.ToxicFlask);
				checker.Add(ItemID.SpectreBar, ItemID.SpectreHood, ItemID.SpectreMask);
				checker.Add(ItemID.ShroomiteBar, ItemID.ShroomiteBreastplate, ItemID.ShroomiteHelmet, ItemID.ShroomiteLeggings);
				return checker;
			}
			#endregion
			#region Golem
			internal static IItemChecker LoadBeforeGolem()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedGolemBoss);
				checker.Add(ItemID.Picksaw);
				checker.Add(ItemID.SunStone, ItemID.CelestialStone, ItemID.CelestialShell);
				checker.Add(ItemID.PossessedHatchet, ItemID.ShinyStone, ItemID.BeetleLeggings, ItemID.BeetleHusk, ItemID.BeetleHelmet);
				checker.Add(ItemID.InfluxWaver, ItemID.LaserMachinegun, ItemID.AntiGravityHook);
				return checker;
			}
			#endregion
			#region Solar
			internal static IItemChecker LoadBeforeSolar()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedTowerSolar);
				checker.Add(ItemID.FragmentSolar);
				checker.Add(ItemID.DayBreak);
				checker.Add(ItemID.SolarEruption);
				return checker;
			}
			#endregion
			#region Vortex
			internal static IItemChecker LoadBeforeVortex()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedTowerVortex);
				checker.Add(ItemID.FragmentVortex);
				checker.Add(ItemID.VortexBeater);
				checker.Add(ItemID.Phantasm);
				return checker;
			}
			#endregion
			#region Stardust
			internal static IItemChecker LoadBeforeStardust()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedTowerStardust);
				checker.Add(ItemID.FragmentStardust);
				checker.Add(ItemID.StardustCellStaff);
				checker.Add(ItemID.StardustDragonStaff);
				return checker;
			}
			#endregion
			#region Nebula
			internal static IItemChecker LoadBeforeNebula()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedTowerNebula);
				checker.Add(ItemID.FragmentNebula);
				checker.Add(ItemID.NebulaArcanum);
				checker.Add(ItemID.NebulaBlaze);
				return checker;
			}
			#endregion
			#region Moon
			internal static IItemChecker LoadBeforeMoon()
			{
				IItemChecker checker = new DefaultItemChecker(() => NPC.downedMoonlord);
				checker.Add(ItemID.LunarBar, ItemID.MoonlordBullet, ItemID.MoonlordArrow, ItemID.MoonLordBossBag);
				checker.Add(ItemID.WingsSolar, ItemID.WingsStardust, ItemID.WingsNebula, ItemID.WingsVortex);
				checker.Add(ItemID.SolarFlarePickaxe, ItemID.NebulaPickaxe, ItemID.StardustPickaxe, ItemID.VortexPickaxe);
				checker.Add(ItemID.SolarFlareHammer, ItemID.NebulaHammer, ItemID.StardustHammer, ItemID.VortexHammer);
				checker.Add(ItemID.Zenith, ItemID.Meowmere, ItemID.StarWrath, ItemID.LastPrism, ItemID.SDMG, ItemID.LunarFlareBook);
				checker.Add(ItemID.DrillContainmentUnit);
				return checker;
			}
			#endregion
		}
	}
}
