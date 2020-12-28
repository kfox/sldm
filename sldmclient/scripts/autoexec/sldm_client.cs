// SLDM Client

// global variables
$SLDMClient::clientVersion = 1.01;
$SLDMClient::clientVersionString = "1.01";
$SLDMClient::serverVersion = 0;
$SLDMClient::serverVersionString = "";

// callback handler functions

function sldmClientJoin(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
	// register this client
	commandToServer('SLDMRegisterClient', $SLDMClient::clientVersion, $SLDMClient::clientVersionString);
}

function sldmVersion(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
	%version = detag(%a1);
	%versionString = detag(%a2);

	// show and store the server version info
	echo("SLDM server version: "@%versionString@" ("@%version@")");
	$SLDMClient::serverVersion = %version;
	$SLDMClient::serverVersionString = %versionString;
}

function sldmInvo(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
        %invos = detag(%a1);
        objectiveHud.yourInvos.setValue(%invos);
}

function sldmBonus(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
        %bonuses = detag(%a1);
        objectiveHud.yourBonuses.setValue(%bonuses);
}

function sldmStreak(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
        %streak = detag(%a1);
        objectiveHud.yourStreak.setValue(%streak);
}

// package begins

package sldm_client
{


function setupObjHud(%gameType)
{
	if (%gameType $= "ShocklanceDMGame")
	{
		// set separators
		objectiveHud.setSeparators("40 76 115 151 198");
		objectiveHud.disableHorzSeparator();

		//
		// TOP ROW, LEFT TO RIGHT
		//

		// Your score label ("SCORE")
		objectiveHud.scoreLabel = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "4 3";
			extent = "37 16";
			visible = "1";
			text = "SCORE";
		};
		// Your score
		objectiveHud.yourScore = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "43 3";
			extent = "30 16";
			visible = "1";
		};

		// Your kills label ("KILLS")
		objectiveHud.killsLabel = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "79 3";
			extent = "33 16";
			visible = "1";
			text = "KILLS";
		};
		// Your kills
		objectiveHud.yourKills = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "118 3";
			extent = "30 16";
			visible = "1";
		};

		// Your deaths label ("DEATHS")
		objectiveHud.deathsLabel = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "154 3";
			extent = "39 16";
			visible = "1";
			text = "DEATHS";
		};
		// Your deaths
		objectiveHud.yourDeaths = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "201 3";
			extent = "30 16";
			visible = "1";
		};

		//
		// BOTTOM ROW, LEFT TO RIGHT
		//

		// Your bonuses label ("BONUS")
		objectiveHud.bonusesLabel = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "4 19";
			extent = "37 16";
			visible = "1";
			text = "BONUS";
		};
		// Your bonuses
		objectiveHud.yourBonuses = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "43 19";
			extent = "30 16";
			visible = "1";
		};

		// Your invos label ("INVOS")
		objectiveHud.invosLabel = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "79 19";
			extent = "33 16";
			visible = "1";
			text = "INVOS";
		};
		// Your invos
		objectiveHud.yourInvos = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "118 19";
			extent = "30 16";
			visible = "1";
		};

		// Your streak label ("STREAK")
		objectiveHud.streakLabel = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "154 19";
			extent = "39 16";
			visible = "1";
			text = "STREAK";
		};
		// Your streak
		objectiveHud.yourStreak = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "201 19";
			extent = "30 16";
			visible = "1";
		};

		objectiveHud.add(objectiveHud.scoreLabel);
		objectiveHud.add(objectiveHud.yourScore);
		objectiveHud.add(objectiveHud.killsLabel);
		objectiveHud.add(objectiveHud.yourKills);
		objectiveHud.add(objectiveHud.bonusesLabel);
		objectiveHud.add(objectiveHud.yourBonuses);
		objectiveHud.add(objectiveHud.streakLabel);
		objectiveHud.add(objectiveHud.yourStreak);
		objectiveHud.add(objectiveHud.deathsLabel);
		objectiveHud.add(objectiveHud.yourDeaths);
		objectiveHud.add(objectiveHud.invosLabel);
		objectiveHud.add(objectiveHud.yourInvos);

		chatPageDown.setVisible(false);
	}
	else
		Parent::setupObjHud(%gameType);
}

function DispatchLaunchMode()
{
	echo("Adding message callbacks for ShocklanceDM...");

	addMessageCallback('MsgSLDMClientJoin', sldmClientJoin);
	addMessageCallback('MsgSLDMVersion', sldmVersion);
	addMessageCallback('MsgSLDMInvo', sldmInvo);
	addMessageCallback('MsgSLDMBonus', sldmBonus);
	addMessageCallback('MsgSLDMStreak', sldmStreak);

	Parent::DispatchLaunchMode();
}


};

// package ends

// activate the package
activatePackage(sldm_client);
