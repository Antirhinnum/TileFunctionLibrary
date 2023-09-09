using Terraria.ID;
using Terraria.ModLoader;

namespace TileFunctionLibrary.API;

/// <summary>
/// Any <see cref="ModTile"/> that implements this interface will function as vanilla's Extractinator (<see cref="TileID.Extractinator"/>) tile.
/// </summary>
public interface IExtractinatorTile
{
	/// <summary>
	/// If <see langword="true"/>, players can place items with a value set in <see cref="ItemID.Sets.ExtractinatorMode"/> into this tile to recieve loot.<br/>
	/// Defaults to <see langword="true"/>.
	/// </summary>
	bool ShouldFunctionAsExtractinator => true;

#if !TML_2022_09
	/// <summary>
	/// If <see langword="true"/>, the loot table for the <see cref="TileID.ChlorophyteExtractinator"/> will be used instead of the normal <see cref="TileID.Extractinator"/> loot table.<br/>
	/// Does nothing unless <see cref="ShouldFunctionAsExtractinator"/> is <see langword="true"/>.<br/>
	/// Defaults to <see langword="false"/>.
	/// </summary>
	bool UseChlorophyteExtractinatorLootTable => false;

	/// <summary>
	/// If <see langword="true"/>, players can trade items at this tile, similarly to the <see cref="TileID.ChlorophyteExtractinator"/>.<br/>
	/// Defaults to <see langword="false"/>.
	/// </summary>
	bool ShouldDoChlorophyteExtractinatorItemTrades => false;
#endif

#if TML_2022_09

	/// <summary>
	/// If <see langword="true"/>, Smart Select will bring up extractables or tradable items when used on this tile.<br/>
	/// Defaults to <c><see cref="ShouldFunctionAsExtractinator"/></c>.
	/// </summary>
	bool ShouldBeExtractinatorSmartSelectTarget => ShouldFunctionAsExtractinator;

#else
	/// <summary>
	/// If <see langword="true"/>, Smart Select will bring up extractables or tradable items when used on this tile.<br/>
	/// Defaults to <c><see cref="ShouldFunctionAsExtractinator"/> || <see cref="ShouldDoChlorophyteExtractinatorItemTrades"/></c>.
	/// </summary>
	bool ShouldBeExtractinatorSmartSelectTarget => ShouldFunctionAsExtractinator || ShouldDoChlorophyteExtractinatorItemTrades;
#endif
}