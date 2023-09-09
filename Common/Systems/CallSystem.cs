using Terraria.ID;
using Terraria.ModLoader;
using TileFunctionLibrary.API;

namespace TileFunctionLibrary.Common.Systems;

/// <summary>
/// Handles all <see cref="Mod.Call(object[])"/> for this mod.
/// </summary>
public sealed class CallSystem : ModSystem
{
	internal static object HandleCalls(object[] args)
	{
		switch (args)
		{
			// Returns true if players can place extractible items into tiles of this type for loot.
			case ["IsTileExtractinator", int tileType]:
			{
				return tileType == TileID.Extractinator
#if !TML_2022_09
					|| tileType == TileID.ChlorophyteExtractinator 
#endif
					|| (TileLoader.GetTile(tileType) is IExtractinatorTile extractinatorTile && extractinatorTile.ShouldFunctionAsExtractinator);
			}

#if !TML_2022_09
			// Returns true if players can use the given tile type as a Chlorophyte Extractinator.
			// Which aspect is checked depends on whatToCheck:
			// 0: If the given tile uses the Chlorophyte Extractinator's loot table.
			// 1: If the given tile performs the Chlorophyte Extractinator's item trades.
			// Any other value: Returns false unless tileType is TileID.ChlorophyteExtractinator.
			case ["IsTileChlorophyteExtractinator", int tileType, int whatToCheck]:
			{
				if (tileType == TileID.ChlorophyteExtractinator)
				{
					return true;
				}

				if (TileLoader.GetTile(tileType) is not IExtractinatorTile extractinatorTile)
				{
					return false;
				}

				return whatToCheck switch
				{
					0 => extractinatorTile.UseChlorophyteExtractinatorLootTable,
					1 => extractinatorTile.ShouldDoChlorophyteExtractinatorItemTrades,
					_ => false
				};
			} 
#endif
		}

		return null;
	}
}