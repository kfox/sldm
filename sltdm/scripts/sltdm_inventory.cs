package sltdm_inventory
{


function SLTDMGame::initInventory()
{
	// Never allow these in player inventory
	$InvBanList[SLTDM, "TurretOutdoorDeployable"] = 1;
	$InvBanList[SLTDM, "TurretIndoorDeployable"] = 1;
	$InvBanList[SLTDM, "ElfBarrelPack"] = 1;
	$InvBanList[SLTDM, "MortarBarrelPack"] = 1;
	$InvBanList[SLTDM, "PlasmaBarrelPack"] = 1;
	$InvBanList[SLTDM, "AABarrelPack"] = 1;
	$InvBanList[SLTDM, "MissileBarrelPack"] = 1;
	$InvBanList[SLTDM, "ShieldPack"] = 1;
	$InvBanList[SLTDM, "AmmoPack"] = 1;
	$InvBanList[SLTDM, "CloakingPack"] = 1;
	$InvBanList[SLTDM, "RepairPack"] = 1;
	$InvBanList[SLTDM, "Mine"] = 1;
	$InvBanList[SLTDM, "Blaster"] = 1;
	$InvBanList[SLTDM, "Mortar"] = 1;
	$InvBanList[SLTDM, "Disc"] = 1;
	$InvBanList[SLTDM, "Chaingun"] = 1;
	$InvBanList[SLTDM, "MissileLauncher"] = 1;
	$InvBanList[SLTDM, "GrenadeLauncher"] = 1;
	$InvBanList[SLTDM, "SniperRifle"] = 1;
	$InvBanList[SLTDM, "ELFGun"] = 1;
	$InvBanList[SLTDM, "Plasma"] = 1;
	$InvBanList[SLTDM, "CameraGrenade"] = 1;
	$InvBanList[SLTDM, "MotionSensorDeployable"] = 1;
	$InvBanList[SLTDM, "PulseSensorDeployable"] = 1;
	$InvBanList[SLTDM, "SatchelCharge"] = 1;
	$InvBanList[SLTDM, "SensorJammerPack"] = 1;
	$InvBanList[SLTDM, "InventoryDeployable"] = 1;
	$InvBanList[SLTDM, "Beacon"] = 1;
}

function SLTDMGame::equip(%game, %player)
{
	for(%i =0; %i<$InventoryHudCount; %i++)
		%player.client.setInventoryHudItem($InventoryHudData[%i, itemDataName], 0, 1);

	%player.client.clearBackpackIcon();

	%player.setArmor("Light");

	%player.setInventory("ShockLance", 1);
	%player.setInventory("EnergyPack", 1);
	%player.setInventory("TargetingLaser", 1);
	%player.setInventory("FlareGrenade", 5);
	%player.setInventory("RepairKit", 1);

	%player.use("ShockLance");
	%player.weaponCount = 1;
}

function serverCmdSetClientFav(%client, %text)
{
	// Disallow certain armors
	switch ($SLTDM::Pref::RestrictArmor)
	{
		case 1:
			%text = setField(%text, 1, "Scout");
		case 2:
			%text = setField(%text, 1, "Assault");
		case 3:
			if (getField(%text, 1) $= "Juggernaut")
				%text = setField(%text, 1, "Assault");
		case 4:
			%text = setField(%text, 1, "Juggernaut");
		case 5:
			if (getField(%text, 1) $= "Assault")
				%text = setField(%text, 1, "Scout");
		case 6:
			if (getField(%text, 1) $= "Scout")
				%text = setField(%text, 1, "Assault");
		case 7:
			// do nothing - all armors available
	}

	// call the original func
	Parent::serverCmdSetClientFav(%client, %text);
}

function checkInventory(%client, %text)
{
	%list = getField( %text, 0 ) TAB getField( %text, 1 );
	%list = %list TAB "Weapon" TAB "Shocklance";

	for( %i = 5; %i < getFieldCount( %text ); %i = %i + 2 )
	{
		switch$ (getField(%text, %i - 1))
		{
			case Weapon:
				%list = %list TAB "Weapon" TAB "INVALID";

			case Pack:
				%list = %list TAB "Pack" TAB "Energy Pack";

			case Grenade:
				%list = %list TAB "Grenade";

				%grenade = "Flare Grenade";

				switch$ (getField(%text, %i))
				{
					case "Grenade":
						if ($SLTDM::Pref::AllowNormalGrenades > 3)
							%grenade = "Grenade";

					case "Whiteout Grenade":
						if ($SLTDM::Pref::AllowWhiteoutGrenades > 3)
							%grenade = "Whiteout Grenade";

					case "Concussion Grenade":
						if ($SLTDM::Pref::AllowConcussionGrenades > 3)
							%grenade = "Concussion Grenade";
				}

				%list = %list TAB %grenade;

			case Mine:
				%list = %list TAB "Mine" TAB "INVALID";
		}
	}

	return %list;
}

function InventoryScreen::updateHud(%this, %client, %tag)
{
	%armor = getArmorDatablock(%client, $NameToInv[%client.favorites[0]]);

	// remove inventory hud lines
	for (%x = 0; %x < 9; %x++)
		messageClient(%client, 'RemoveLineHud', "", 'inventoryScreen', %x);
	//Create - ARMOR - List
	%armorList = %client.favorites[0];
	for ( %y = 0; $InvArmor[%y] !$= ""; %y++ )
		if ( $InvArmor[%y] !$= %client.favorites[0] )
			%armorList = %armorList TAB $InvArmor[%y];

	// tell the client their armor options
	messageClient(%client, 'SetLineHud', "", %tag, 0, "Armor:", %armorList, armor, 1);

	// tell the client their weapon options
	messageClient(%client, 'SetLineHud', "", %tag, 1, "Weapon:", "Shocklance", weapon, 2);

	// tell the client their pack options
	messageClient(%client, 'SetLineHud', "", %tag, 2, "Pack:", "Energy Pack", pack, 3);

	%favorite = %client.favorites[getField(%client.grenadeIndex, 0)];
	if ( %favorite $= empty ||
	     %favorite $= invalid ||
	    (%favorite $= "Whiteout Grenade" && $SLTDM::Pref::AllowWhiteoutGrenades < 4) ||
	    (%favorite $= "Concussion Grenade" && $SLTDM::Pref::AllowConcussionGrenades < 4) ||
	    (%favorite $= "Grenade" && $SLTDM::Pref::AllowNormalGrenades < 4) ||
	     %favorite $= "Deployable Camera")
		%favorite = "Flare Grenade";

	%list = %favorite;

	if (%favorite !$= "Flare Grenade")
		%list = %list TAB "Flare Grenade";

	if (%favorite !$= "Whiteout Grenade" && $SLTDM::Pref::AllowWhiteoutGrenades > 3)
		%list = %list TAB "Whiteout Grenade";

	if (%favorite !$= "Concussion Grenade" && $SLTDM::Pref::AllowConcussionGrenades > 3)
		%list = %list TAB "Concussion Grenade";

	if (%favorite !$= "Grenade" && $SLTDM::Pref::AllowNormalGrenades > 3)
		%list = %list TAB "Grenade";

	// tell the client their grenade options
	messageClient(%client, 'SetLineHud', "", %tag, 3, "Grenade:", %list, grenade, 4);
}

function stationTrigger::onEnterTrigger(%data, %obj, %colObj)
{
	//make sure it's a player object, and that that object is still alive
	if (%colObj.getDataBlock().className !$= "Armor" || %colObj.getState() $= "Dead")
		return;

	%colObj.inStation = true;
	commandToClient(%colObj.client,'setStationKeys', true);
	if (Game.stationOnEnterTrigger(%data, %obj, %colObj))
	{
		//verify station.team is team associated and isn't on player's team
		if ((%obj.mainObj.team != %colObj.client.team) && (%obj.mainObj.team != 0))
		{
			//%obj.station.playAudio(2, StationAccessDeniedSound);
			messageClient(%colObj.client, 'msgStationDenied', '\c2Access Denied -- Wrong team.~wfx/powered/station_denied.wav');
		}
		else if (%obj.disableObj.isDisabled())
		{
			messageClient(%colObj.client, 'msgStationDisabled', '\c2Station is disabled.');
		}
		else if (!%obj.mainObj.isPowered())
		{
			messageClient(%colObj.client, 'msgStationNoPower', '\c2Station is not powered.');
		}
		else if (%obj.station.notDeployed)
		{
			messageClient(%colObj.client, 'msgStationNotDeployed', '\c2Station is not deployed.');
		}
		else if ($SLTDM::Pref::InventoryStationTeleportation > 3)
		{
			// check for penalty
			Game.inventoryPenalty(%colObj);

			// buy favorites if invo access enabled
			if ($SLTDM::Pref::InventoryStationAccess > 3)
			{
				buyFavorites(%colObj.client);
				%colObj.client.player.selectWeaponSlot(0);
			}

			// teleport!
			Game.activateTeleporter(%obj, %colObj);
		}
		else if ($SLTDM::Pref::InventoryStationAccess < 4)
		{
			messageClient(%colObj.client, 'msgStationDenied', '\c2Access Denied -- Inventory Station Access is disabled.~wfx/powered/station_denied.wav');
		}
		else if (%obj.station.triggeredBy $= "")
		{
			// check for penalty
			Game.inventoryPenalty(%colObj);

			if (%obj.station.getDataBlock().setPlayersPosition(%obj.station, %obj, %colObj))
			{
				messageClient(%colObj.client, 'CloseHud', "", 'inventoryScreen');
				commandToClient(%colObj.client, 'TogglePlayHuds', true);
				%obj.station.triggeredBy = %colObj;
				%obj.station.getDataBlock().stationTriggered(%obj.station, 1);
				%colObj.station = %obj.station;
				%colObj.lastWeapon = ( %colObj.getMountedImage($WeaponSlot) == 0 ) ? "" : %colObj.getMountedImage($WeaponSlot).getName().item;
				%colObj.unmountImage($WeaponSlot);
			}
		}
	}
}

function SLTDMGame::inventoryPenalty(%game, %obj)
{
	// penalize players for invo access
	// if penalty is enabled AND
	// they are at less than full health
	// AND they do NOT have a repkit with them
	if ($SLTDM::Pref::InventoryStationAccessPenalty > 3 &&
	    ((%obj.getDamagePercent() > 0) || !%obj.getInventory("RepairKit")))
	{
		messageClient(%obj.client, '', '\c2You have been penalized for inventory station access.~wfx/misc/warning_beep.wav');
		%game.updateInvoScores(%obj.client);
	}
}

function SLTDMGame::stationOnEnterTrigger(%game, %data, %obj, %colObj)
{
	// just call the default for now... maybe get fancy in the future
	DefaultGame::stationOnEnterTrigger(%game, %data, %obj, %colObj);
}

function buyFavorites(%client)
{
	%client.player.clearInventory();
	%client.setWeaponsHudClearAll();
	%cmt = $CurrentMissionType;

	%curArmor = %client.player.getDatablock();
	%curDmgPct = getDamagePercent(%curArmor.maxDamage, %client.player.getDamageLevel());

	// armor
	%client.armor = $NameToInv[%client.favorites[0]];

	switch ($SLTDM::Pref::RestrictArmor)
	{
		case 1:
			%client.armor = "Light";
		case 2:
			%client.armor = "Medium";
		case 3:
			if (%client.armor $= "Heavy")
				%client.armor = "Medium";
		case 4:
			%client.armor = "Heavy";
		case 5:
			if (%client.armor $= "Medium")
				%client.armor = "Light";
		case 6:
			if (%client.armor $= "Light")
				%client.armor = "Medium";
		case 7:
			// anything goes
	}

	%client.player.setArmor(%client.armor);
	%newArmor = %client.player.getDataBlock();

	%client.player.setDamageLevel(%curDmgPct * %newArmor.maxDamage);

	// weapon
	%client.player.setInventory("ShockLance", 1);
	%client.player.weaponCount = 1;

	// pack
	%client.player.setInventory("EnergyPack", 1);

	// grenades
	for ( %i = 0; %i < getFieldCount( %client.grenadeIndex ); %i++ )
	{
		if ( !($InvBanList[%cmt, $NameToInv[%client.favorites[getField( %client.grenadeIndex, %i )]]]) )
			%client.player.setInventory( $NameToInv[%client.favorites[getField( %client.grenadeIndex,%i )]], 30 );
		else
			%client.player.setInventory("FlareGrenade", 30);
	}

	%client.player.lastGrenade = $NameToInv[%client.favorites[getField( %client.grenadeIndex,%i )]];

	// miscellaneous stuff -- Repair Kit, Beacons, Targeting Laser
	if ( !($InvBanList[%cmt, RepairKit]) )
		%client.player.setInventory("RepairKit", 1);
	if ( !($InvBanList[%cmt, Beacon]) )
		%client.player.setInventory("Beacon", 400);
	if ( !($InvBanList[%cmt, TargetingLaser]) )
		%client.player.setInventory("TargetingLaser", 1);
}


};
