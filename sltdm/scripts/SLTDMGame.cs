// --------------------------------------------------------
// Shocklance Team Deathmatch mission type
// by a tiny fishie
// --------------------------------------------------------

// DisplayName = SLTDM

//--- GAME RULES BEGIN ---
//Lance the other team
//Don't get lanced
//Lance the enemy leader for a bonus
//<spush><color:ffff00>SLTDM (v1.0 - 20021107) by: <color:ee00ee>a tiny fishie<spop>
//<spush><color:ffff00>Get SLTDM and the SLTDMClient from <a:github.com/kfox/sldm>github.com/kfox/sldm</a><spop>
//--- GAME RULES END ---

$SLTDM::Version = 1.0;
$SLTDM::VersionString = "1.0";
$SLTDM::LatestClientVersion = 1.0;

function SLTDMGame::activatePackages(%game)
{
	exec("scripts/sltdm_prefs.cs");
	exec("scripts/sltdm_inventory.cs");
	exec("scripts/sltdm_delete.cs");
	exec("scripts/sltdm_ai.cs");
	exec("scripts/sltdm_vote.cs");
	exec("scripts/sltdm_teleport.cs");
	exec("scripts/sltdm_score.cs");
	exec("scripts/sltdm_waypoint.cs");
	exec("scripts/sltdm_main.cs");

	activatePackage(DefaultGame);
	activatePackage(sltdm_prefs);
	activatePackage(sltdm_inventory);
	activatePackage(sltdm_delete);
	activatePackage(sltdm_vote);
	activatePackage(sltdm_teleport);
	activatePackage(sltdm_score);
	activatePackage(sltdm_waypoint);
	activatePackage(sltdm_ai);
	activatePackage(sltdm_main);

	%game.state = "countdown";
}

function SLTDMGame::deactivatePackages(%game)
{
	deactivatePackage(sltdm_main);
	deactivatePackage(sltdm_ai);
	deactivatePackage(sltdm_waypoint);
	deactivatePackage(sltdm_score);
	deactivatePackage(sltdm_teleport);
	deactivatePackage(sltdm_vote);
	deactivatePackage(sltdm_delete);
	deactivatePackage(sltdm_inventory);
	deactivatePackage(sltdm_prefs);
	deactivatePackage(DefaultGame);
}
