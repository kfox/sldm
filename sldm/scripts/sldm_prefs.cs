package sldm_prefs
{


function ShocklanceDMGame::loadPrefs()
{
	// initialize prefs value
	$ShocklanceDM::PrefsLoaded = 0;

	// initialize some other variables
	%path		= "prefs";
	%basename	= "ShocklanceDMPrefs";
	%oldprefs	= %path @ "/" @ %basename @ ".old";
	%prefs		= %path @ "/" @ %basename @ ".cs";
	%default	= "scripts/" @ %basename @ ".default";

	%currentversion	= $ShocklanceDM::Version;

	// load prefs file
	exec(%prefs);

	%loadedversion	= $ShocklanceDM::PrefsLoaded;

	if (%loadedversion != %currentversion)
	{
		// prefs not loaded or invalid
		if (%loadedversion == 0)
			echo(%prefs@" not found - writing new file");
		else
		{
			// backup the old prefs file
			// and write a new one
			echo(%prefs@" is an old version (old="@%loadedversion@", current="@%currentversion@"), writing new file");

			if (ShocklanceDMGame::copyTextFile(%prefs, %oldprefs))
				echo("Backup prefs file saved to "@%oldprefs);
		}

		// create new prefs file from default
		// file included in distribution
		if (ShocklanceDMGame::copyTextFile(%default, %prefs))
		{
			// created a new prefs file
			echo("Wrote a new prefs file to "@%prefs);
			exec(%prefs);
		}
		else
		{
			// failed to create a new prefs file
			warn("Unable to write "@%prefs@", using default pref settings");
			$ShocklanceDM::PrefsLoaded = 0.1;
		}
	}
}

function ShocklanceDMGame::copyTextFile(%sourceFileName, %destinationFileName)
{
	// create new filehandles
	%sourceFile = new FileObject();
	%destinationFile = new FileObject();

	// if the source is readable and destination is writable...
	if (%sourceFile.openForRead(%sourceFileName) &&
	    %destinationFile.openForWrite(%destinationFileName))
	{
		// do a line-by-line copy
		while (!%sourceFile.isEOF())
			%destinationFile.writeLine(%sourceFile.readLine());

		// close the filehandles
		%sourceFile.close();
		%destinationFile.close();
	}
	else
		return false; // failure

	// success
	return true;
}

function ShocklanceDMGame::setPrefs(%game)
{
	echo (">>>>> begin setting ShocklanceDM prefs");

	//
	// remove all items preferences
	//
	$ShocklanceDM::Pref::RemoveAllItems = %game.forceValidPrefValue($ShocklanceDM::Pref::RemoveAllItems, 0, 1, 0);
	if ( $ShocklanceDM::Pref::RemoveAllItems == 1 )
	{
		echo ("--> Removing all map items");
		sldmDeleteNonGameObjectsFromMap("Item");
	}
	else
	{
		echo ("--> Preserving all map items");
	}

	//
	// allow inventory station access preferences
	//
	$ShocklanceDM::Pref::InventoryStationAccess = %game.forceValidPrefValue($ShocklanceDM::Pref::InventoryStationAccess, 0, 8, 7);
	if ($ShocklanceDM::Pref::InventoryStationAccess == 8)
	{
		echo ("--> Removing all Inventory Stations from map");
		sldmDeleteNonGameObjectsFromMap("StaticShape");
	}
	else if ($ShocklanceDM::Pref::InventoryStationAccess > 3)
	{
		echo ("--> Inventory Station access enabled; "@%game.describePrefs($ShocklanceDM::Pref::InventoryStationAccess));
	}
	else
	{
		echo ("--> Inventory Station access disabled; "@%game.describePrefs($ShocklanceDM::Pref::InventoryStationAccess));
	}

	//
	// inventory station access penalty preferences
	//
	$ShocklanceDM::Pref::InventoryStationAccessPenalty = %game.forceValidPrefValue($ShocklanceDM::Pref::InventoryStationAccessPenalty, 0, 7, 7);
	%description = %game.describePrefs($ShocklanceDM::Pref::InventoryStationAccessPenalty);
	if ( $ShocklanceDM::Pref::InventoryStationAccessPenalty < 4 )
		echo ("--> Inventory access penalty disabled; "@%description);
	else
		echo ("--> Inventory access penalty enabled; "@%description);

	//
	// inventory station teleportation preferences
	//
	$ShocklanceDM::Pref::InventoryStationTeleportation = %game.forceValidPrefValue($ShocklanceDM::Pref::InventoryStationTeleportation, 0, 7, 3);
	%description = %game.describePrefs($ShocklanceDM::Pref::InventoryStationTeleportation);
	if ( $ShocklanceDM::Pref::InventoryStationTeleportation < 4 )
		echo ("--> Inventory teleportation disabled; "@%description);
	else
		echo ("--> Inventory teleportation enabled; "@%description);

	//
	// whiteout grenades preferences
	//
	$ShocklanceDM::Pref::AllowWhiteoutGrenades = %game.forceValidPrefValue($ShocklanceDM::Pref::AllowWhiteoutGrenades, 0, 7, 3);
	%description = %game.describePrefs($ShocklanceDM::Pref::AllowWhiteoutGrenades);
	if ( $ShocklanceDM::Pref::AllowWhiteoutGrenades < 4 )
	{
		echo ("--> Whiteout grenades disabled; "@%description);
		$InvBanList[ShocklanceDM, "FlashGrenade"] = 1;
	}
	else
	{
		echo ("--> Whiteout grenades enabled; "@%description);
	}

	//
	// concussion grenades preferences
	//
	$ShocklanceDM::Pref::AllowConcussionGrenades = %game.forceValidPrefValue($ShocklanceDM::Pref::AllowConcussionGrenades, 0, 7, 3);
	%description = %game.describePrefs($ShocklanceDM::Pref::AllowConcussionGrenades);
	if ( $ShocklanceDM::Pref::AllowConcussionGrenades < 4 )
	{
		echo ("--> Concussion grenades disabled; "@%description);
		$InvBanList[ShocklanceDM, "ConcussionGrenade"] = 1;
	}
	else
	{
		echo ("--> Concussion grenades enabled; "@%description);
	}

	//
	// normal grenades preferences
	//
	$ShocklanceDM::Pref::AllowNormalGrenades = %game.forceValidPrefValue($ShocklanceDM::Pref::AllowNormalGrenades, 0, 7, 3);
	%description = %game.describePrefs($ShocklanceDM::Pref::AllowNormalGrenades);
	if ( $ShocklanceDM::Pref::AllowNormalGrenades < 4 )
	{
		echo ("--> Normal grenades disabled; "@%description);
		$InvBanList[ShocklanceDM, "Grenade"] = 1;
	}
	else
	{
		echo ("--> Normal grenades enabled; "@%description);
	}

	//
	// restrict armor preferences
	//
	$ShocklanceDM::Pref::RestrictArmor = %game.forceValidPrefValue($ShocklanceDM::Pref::RestrictArmor, 1, 7, 7);
	switch ($ShocklanceDM::Pref::RestrictArmor)
	{
		case 1:
			echo ("--> Only Scout armor available");
		case 2:
			echo ("--> Only Assault armor available");
		case 3:
			echo ("--> Only Scout and Assault armors available");
		case 4:
			echo ("--> Only Juggernaut armor available");
		case 5:
			echo ("--> Only Scout and Juggernaut armors available");
		case 6:
			echo ("--> Only Assault and Juggernaut armors available");
		case 7:
			echo ("--> All armors available");
	}

	//
	// spawn with faves preferences
	//
	$ShocklanceDM::Pref::SpawnWithFaves = %game.forceValidPrefValue($ShocklanceDM::Pref::SpawnWithFaves, 0, 7, 3);
	%description = %game.describePrefs($ShocklanceDM::Pref::SpawnWithFaves);
	if ( $ShocklanceDM::Pref::SpawnWithFaves < 4 )
		echo ("--> Spawn with default loadout; "@%description);
	else
		echo ("--> Spawn using selected favorite; "@%description);

	echo ("<<<<< finished setting ShocklanceDM prefs");
}

function ShocklanceDMGame::describePrefs(%game, %value)
{
	%description = "unknown value";

	switch (%value)
	{
		case 0:
			%description = "No in-game change possible";
		case 1:
			%description = "May be enabled/disabled by vote only";
		case 2:
			%description = "May be enabled/disabled by admin only";
		case 3:
			%description = "May be enabled/disabled by vote or admin";
		case 4:
			%description = "No in-game change possible";
		case 5:
			%description = "May be enabled/disabled by vote only";
		case 6:
			%description = "May be enabled/disabled by admin only";
		case 7:
			%description = "May be enabled/disabled by vote or admin";
	}

	return %description;
}

function ShocklanceDMGame::forceValidPrefValue(%game, %current, %lowest, %highest, %default)
{
	// is current value out of bounds?
	if (%current < %lowest || %current > %highest)
		return %default; // yes, return the default value
	else
		return %current; // no, use it
}


};
