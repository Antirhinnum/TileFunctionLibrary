using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TileFunctionLibrary.API;

#if TML_2022_09

using IL_Player = IL.Terraria.Player;

#else
using Terraria.GameContent;
#endif

namespace TileFunctionLibrary.Common.Systems;

/// <summary>
/// Implements the functionality of the <see cref="IExtractinatorTile"/> interface.
/// </summary>
internal sealed class ExtractinatorEditsSystem : ModSystem
{
	public override void Load()
	{
		IL_Player.PlaceThing_ItemInExtractinator += AllowInterfacedModTilesToExtractinate;

#if !TML_2022_09
		On_Player.ExtractinatorUse += ForceInterfacedModTilesToUseChlorophyteLootTable;
		On_Player.TryGettingItemTraderFromBlock += MakeChlorophyteExtractinatorTradesAvailable;
#endif
	}

	private void AllowInterfacedModTilesToExtractinate(ILContext il)
	{
		ILCursor c = new(il);

		// Match (C#):
		//	Tile targetBlock = Main.tile[tileTargetX, tileTargetY];
		// Match (IL):
		//	ldsflda valuetype Terraria.Tilemap Terraria.Main::tile
		//	ldsfld int32 Terraria.Player::tileTargetX
		//	ldsfld int32 Terraria.Player::tileTargetY
		//	call instance valuetype Terraria.Tile Terraria.Tilemap::get_Item(int32, int32)
		//	stloc tileIndex
		// Need the tile var index

		MethodInfo tilemapGetItem = typeof(Tilemap).GetProperty("Item", new Type[] { typeof(int), typeof(int) }).GetMethod;
		int tileIndex = -1;
		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdsflda<Main>(nameof(Main.tile)),
			i => i.MatchLdsfld<Player>(nameof(Player.tileTargetX)),
			i => i.MatchLdsfld<Player>(nameof(Player.tileTargetY)),
			i => i.MatchCall<Tilemap>("get_Item"),
			i => i.MatchStloc(out tileIndex)
			))
		{
			throw new Exception("AllowInterfacedModTilesToExtractinate patch #1 failed.");
		}

		// Match (C#):
		//	...  && (tile.type == 219 || tile.type == 642)) {
		// Match (IL):
		//	ldloca.s tileIndex
		//	call instance uint16& Terraria.Tile::get_type()
		//	ldind.u2
		//	ldc.i4 219
		//	beq.s LABEL
		//	ldloca.s tileIndex
		//	call instance uint16& Terraria.Tile::get_type()
		//	ldind.u2
		//	ldc.i4 642
		// Replace with (C#):
		//	... && (tile.type == 219 || tile.type == 642 || (TileLoader.GetTile(tile.type) is IExtractinatorTile extractinator && extractinator.ShouldFunctionAsExtractinator))) {
		// tileIndex is the retrieved Tile reference.

		MethodInfo tileGetType = typeof(Tile).GetProperty("type", BindingFlags.NonPublic | BindingFlags.Instance).GetMethod;
		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchLdloca(tileIndex),
			i => i.MatchCall(tileGetType),
			i => i.MatchLdindU2(),
			i => i.MatchLdcI4(TileID.Extractinator)
#if !TML_2022_09
			,
			i => i.MatchBeq(out _),
			i => i.MatchLdloca(tileIndex),
			i => i.MatchCall(tileGetType),
			i => i.MatchLdindU2(),
			i => i.MatchLdcI4(TileID.ChlorophyteExtractinator)
#endif
			))
		{
			throw new Exception("AllowInterfacedModTilesToExtractinate patch #2 failed.");
		}

		// Right before the bne.un.s
		c.Emit(OpCodes.Ceq); // If this fails (returns 0), then we need to check modded tiles.
		c.Emit(OpCodes.Ldloca, tileIndex);
		c.EmitDelegate(CheckModdedTiles); // This returns 1 if vanilla succeeded or if the ModTile succeeded.
		c.Emit(OpCodes.Ldc_I4, (int)1); // Cast needed, this opcode expects an int.
										// Use vanilla's bne to check if CheckModdedTiles == true (1)
	}

	private static bool CheckModdedTiles(bool vanillaSuccess, ref Tile tile)
	{
		return vanillaSuccess || (TileLoader.GetTile(tile.TileType) is IExtractinatorTile extractinator && extractinator.ShouldFunctionAsExtractinator);
	}

#if !TML_2022_09
	private void ForceInterfacedModTilesToUseChlorophyteLootTable(On_Player.orig_ExtractinatorUse orig, Player self, int extractType, int extractinatorBlockType)
	{
		if (TileLoader.GetTile(extractinatorBlockType) is IExtractinatorTile extractinator && extractinator.UseChlorophyteExtractinatorLootTable)
		{
			extractinatorBlockType = TileID.ChlorophyteExtractinator;
		}

		orig(self, extractType, extractinatorBlockType);
	}

	private ItemTrader MakeChlorophyteExtractinatorTradesAvailable(On_Player.orig_TryGettingItemTraderFromBlock orig, Tile targetBlock)
	{
		if (TileLoader.GetTile(targetBlock.TileType) is IExtractinatorTile extractinator && extractinator.ShouldDoChlorophyteExtractinatorItemTrades)
		{
			return ItemTrader.ChlorophyteExtractinator;
		}

		return orig(targetBlock);
	}
#endif
}