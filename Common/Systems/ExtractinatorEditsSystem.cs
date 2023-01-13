using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TileFunctionLibrary.API;

namespace TileFunctionLibrary.Common.Systems;

/// <summary>
/// Implements the functionality of the <see cref="IExtractinatorTile"/> interface.
/// </summary>
internal sealed class ExtractinatorEditsSystem : ModSystem
{
	public override void Load()
	{
		IL.Terraria.Player.PlaceThing_ItemInExtractinator += AllowInterfacedModTilesToExtractinate;
	}

	private void AllowInterfacedModTilesToExtractinate(ILContext il)
	{
		ILCursor c = new(il);

		// Match (C#):
		//	... && Main.tile[tileTargetX, tileTargetY].type == 219 && ...
		// Match (IL):
		//	stloc.0
		//	ldloca.s 0
		//	call instance uint16& Terraria.Tile::get_type()
		//	ldind.u2
		//	ldc.i4 219
		//	bne.un IL_01e9
		// Replace with (C#):
		//	... && (Main.tile[tileTargetX, tileTargetY].type == 219 || TileLoader.GetTile(Main.tile[tileTargetX, tileTargetY].type) is IExtractinatorTile extractinator && extractinator.ShouldFunctionAsExtractinator) && ...
		// tileIndex is the retrieved Tile reference.

		MethodInfo tileGetType = typeof(Tile).GetProperty("type", BindingFlags.NonPublic | BindingFlags.Instance).GetMethod;
		int tileIndex = -1;
		if (!c.TryGotoNext(MoveType.After,
			i => i.MatchStloc(out tileIndex),
			i => i.MatchLdloca(tileIndex),
			i => i.MatchCall(tileGetType),
			i => i.MatchLdindU2(),
			i => i.MatchLdcI4(TileID.Extractinator)
			))
		{
			throw new Exception("AllowInterfacedModTilesToExtractinate patch #1 failed.");
		}

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
}