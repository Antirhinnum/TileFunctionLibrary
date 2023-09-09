using Terraria.ModLoader;
using TileFunctionLibrary.Common.Systems;

namespace TileFunctionLibrary;

/// <summary>
/// The mod class for the mod "Tile Functions Library".
/// </summary>
public sealed class TileFunctionLibrary : Mod
{
	/// <summary>
	/// See <see cref="CallSystem.HandleCalls(object[])"/> for details.
	/// </summary>
	public override object Call(params object[] args)
	{
		return CallSystem.HandleCalls(args);
	}
}