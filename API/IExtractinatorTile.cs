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
}