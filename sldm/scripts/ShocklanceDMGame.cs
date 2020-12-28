// --------------------------------------------------------
// Shocklance Deathmatch mission type
// by a tiny fishie
// --------------------------------------------------------

// DisplayName = Shocklance Deathmatch

//--- GAME RULES BEGIN ---
//Lance everyone
//Don't get lanced
//Lance the leader for a bonus
//<spush><color:ffff00>Shocklance Deathmatch Mod (v2.1 - 20050506) by: <color:ee00ee>a tiny fishie<spop>
//<spush><color:ffff00>Get SLDM from <a:github.com/kfox/sldm>github.com/kfox/sldm</a><spop>
//--- GAME RULES END ---

$ShocklanceDM::Version = 2.1;
$ShocklanceDM::VersionString = "2.1";
$ShocklanceDM::LatestClientVersion = 1.01;

function ShocklanceDMGame::activatePackages(%game)
{
	Parent::activatePackages(%game);

	exec("scripts/sldm_prefs.cs");
	exec("scripts/sldm_inventory.cs");
	exec("scripts/sldm_delete.cs");
	exec("scripts/sldm_ai.cs");
	exec("scripts/sldm_vote.cs");
	exec("scripts/sldm_teleport.cs");
	exec("scripts/sldm_score.cs");
	exec("scripts/sldm_waypoint.cs");
	exec("scripts/sldm_main.cs");

	activatePackage(sldm_prefs);
	activatePackage(sldm_inventory);
	activatePackage(sldm_delete);
	activatePackage(sldm_vote);
	activatePackage(sldm_teleport);
	activatePackage(sldm_score);
	activatePackage(sldm_waypoint);
	activatePackage(sldm_ai);
	activatePackage(sldm_main);

	%game.state = "countdown";
}

function ShocklanceDMGame::deactivatePackages(%game)
{
	deactivatePackage(sldm_main);
	deactivatePackage(sldm_ai);
	deactivatePackage(sldm_waypoint);
	deactivatePackage(sldm_score);
	deactivatePackage(sldm_teleport);
	deactivatePackage(sldm_vote);
	deactivatePackage(sldm_delete);
	deactivatePackage(sldm_inventory);
	deactivatePackage(sldm_prefs);

	Parent::deactivatePackages(%game);
}
