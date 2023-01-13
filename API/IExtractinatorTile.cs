using Terraria;
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

	///// <summary>
	///// Determines if this tile should accept a certain item as input.
	///// </summary>
	///// <param name="input">The item to check.</param>
	///// <returns>
	///// <see langword="true"/> if this <see cref="IExtractinatorTile"/> should accept <paramref name="input"/> as input, <see langword="false"/> otherwise.<br/>
	///// By default, returns <see langword="true"/> if <c><see cref="ItemID.Sets.ExtractinatorMode"/>[<paramref name="input"/>.type] &gt;= 0</c>.
	///// </returns>
	//bool AcceptsInput(in Item input)
	//{
	//	return ItemID.Sets.ExtractinatorMode[input.type] >= 0;
	//}
}