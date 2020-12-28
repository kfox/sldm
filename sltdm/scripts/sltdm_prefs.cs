package sltdm_prefs
{


function SLTDMGame::loadPrefs()
{
	// initialize prefs value
	$SLTDM::PrefsLoaded = 0;

	// initialize some other variables
	%path		= "prefs";
	%basename	= "SLTDMPrefs";
	%oldprefs	= %path @ "/" @ %basename @ ".old";
	%prefs		= %path @ "/" @ %basename @ ".cs";
	%default	= "scripts/" @ %basename @ ".default";

	%currentversion	= $SLTDM::Version;

	// load prefs file
	exec(%prefs);

	%loadedversion	= $SLTDM::PrefsLoaded;

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

			if (SLTDMGame::copyTextFile(%prefs, %oldprefs))
				echo("Backup prefs file saved to "@%oldprefs);
		}

		// create new prefs file from default
		// file included in distribution
		if (SLTDMGame::copyTextFile(%default, %prefs))
		{
			// created a new prefs file
			echo("Wrote a new prefs file to "@%prefs);
			exec(%prefs);
		}
		else
		{
			// failed to create a new prefs file
			warn("Unable to write "@%prefs@", using default pref settings");
			$SLTDM::PrefsLoaded = 0.1;
		}
	}
}

function SLTDMGame::copyTextFile(%sourceFileName, %destinationFileName)
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

function SLTDMGame::setPrefs(%game)
{
	echo (">>>>> begin setting SLTDM prefs");

	//
	// remove all items preferences
	//
	$SLTDM::Pref::RemoveAllItems = sltdmForceValidPrefValue($SLTDM::Pref::RemoveAllItems, 0, 1, 0);
	if ( $SLTDM::Pref::RemoveAllItems == 1 )
	{
		echo ("--> Removing all map items");
		sltdmDeleteNonGameObjectsFromMap("Item");
	}
	else
	{
		echo ("--> Preserving all map items");
	}

	//
	// allow inventory station access preferences
	//
	$SLTDM::Pref::InventoryStationAccess = sltdmForceValidPrefValue($SLTDM::Pref::InventoryStationAccess, 0, 8, 7);
	if ($SLTDM::Pref::InventoryStationAccess == 8)
	{
		echo ("--> Removing all Inventory Stations from map");
		sltdmDeleteNonGameObjectsFromMap("StaticShape");
	}
	else if ($SLTDM::Pref::InventoryStationAccess > 3)
	{
		echo ("--> Inventory Station access enabled; "@%game.describePrefs($SLTDM::Pref::InventoryStationAccess));
	}
	else
	{
		echo ("--> Inventory Station access disabled; "@%game.describePrefs($SLTDM::Pref::InventoryStationAccess));
	}

	//
	// inventory station access penalty preferences
	//
	$SLTDM::Pref::InventoryStationAccessPenalty = sltdmForceValidPrefValue($SLTDM::Pref::InventoryStationAccessPenalty, 0, 7, 7);
	%description = %game.describePrefs($SLTDM::Pref::InventoryStationAccessPenalty);
	if ( $SLTDM::Pref::InventoryStationAccessPenalty < 4 )
		echo ("--> Inventory access penalty disabled; "@%description);
	else
		echo ("--> Inventory access penalty enabled; "@%description);

	//
	// inventory station teleportation preferences
	//
	$SLTDM::Pref::InventoryStationTeleportation = sltdmForceValidPrefValue($SLTDM::Pref::InventoryStationTeleportation, 0, 7, 3);
	%description = %game.describePrefs($SLTDM::Pref::InventoryStationTeleportation);
	if ( $SLTDM::Pref::InventoryStationTeleportation < 4 )
		echo ("--> Inventory teleportation disabled; "@%description);
	else
		echo ("--> Inventory teleportation enabled; "@%description);

	//
	// whiteout grenades preferences
	//
	$SLTDM::Pref::AllowWhiteoutGrenades = sltdmForceValidPrefValue($SLTDM::Pref::AllowWhiteoutGrenades, 0, 7, 3);
	%description = %game.describePrefs($SLTDM::Pref::AllowWhiteoutGrenades);
	if ( $SLTDM::Pref::AllowWhiteoutGrenades < 4 )
	{
		echo ("--> Whiteout grenades disabled; "@%description);
		$InvBanList[SLTDM, "FlashGrenade"] = 1;
	}
	else
	{
		echo ("--> Whiteout grenades enabled; "@%description);
	}

	//
	// concussion grenades preferences
	//
	$SLTDM::Pref::AllowConcussionGrenades = sltdmForceValidPrefValue($SLTDM::Pref::AllowConcussionGrenades, 0, 7, 3);
	%description = %game.describePrefs($SLTDM::Pref::AllowConcussionGrenades);
	if ( $SLTDM::Pref::AllowConcussionGrenades < 4 )
	{
		echo ("--> Concussion grenades disabled; "@%description);
		$InvBanList[SLTDM, "ConcussionGrenade"] = 1;
	}
	else
	{
		echo ("--> Concussion grenades enabled; "@%description);
	}

	//
	// normal grenades preferences
	//
	$SLTDM::Pref::AllowNormalGrenades = sltdmForceValidPrefValue($SLTDM::Pref::AllowNormalGrenades, 0, 7, 3);
	%description = %game.describePrefs($SLTDM::Pref::AllowNormalGrenades);
	if ( $SLTDM::Pref::AllowNormalGrenades < 4 )
	{
		echo ("--> Normal grenades disabled; "@%description);
		$InvBanList[SLTDM, "Grenade"] = 1;
	}
	else
	{
		echo ("--> Normal grenades enabled; "@%description);
	}

	//
	// restrict armor preferences
	//
	$SLTDM::Pref::RestrictArmor = sltdmForceValidPrefValue($SLTDM::Pref::RestrictArmor, 1, 7, 7);
	switch ($SLTDM::Pref::RestrictArmor)
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
	$SLTDM::Pref::SpawnWithFaves = sltdmForceValidPrefValue($SLTDM::Pref::SpawnWithFaves, 0, 7, 3);
	%description = %game.describePrefs($SLTDM::Pref::SpawnWithFaves);
	if ( $SLTDM::Pref::SpawnWithFaves < 4 )
		echo ("--> Spawn with default loadout; "@%description);
	else
		echo ("--> Spawn using selected favorite; "@%description);

	echo ("<<<<< finished setting SLTDM prefs");
}

function SLTDMGame::describePrefs(%game, %value)
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

function sltdmForceValidPrefValue(%current, %lowest, %highest, %default)
{
	// is current value out of bounds?
	if (%current < %lowest || %current > %highest)
		return %default; // yes, return the default value
	else
		return %current; // no, use it
}


};
