namespace AdvancedWorldGen.SpecialOptions._100kSpecial.Entities;

public class Shark : GlobalNPC
{
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		if (_100kWorld.Enabled && !spawnInfo.Water &&
		    ((spawnInfo.SpawnTileX < WorldGen.oceanDistance ||
		      spawnInfo.SpawnTileX > Main.maxTilesX - WorldGen.oceanDistance) &&
		     Main.tileSand[spawnInfo.SpawnTileType] && spawnInfo.SpawnTileY < Main.rockLayer) ||
		    (spawnInfo.SpawnTileType == 53 && WorldGen.oceanDepths(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY)))
		{
			pool.Add(NPCID.Shark, 1/8f);
		}
	}
}