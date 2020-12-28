// SLTDM Client

// global variables
$SLTDMClient::clientVersion = 1.0;
$SLTDMClient::clientVersionString = "1.0";
$SLTDMClient::serverVersion = 0;
$SLTDMClient::serverVersionString = "";

// callback handler functions

function sltdmClientJoin(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
	error("Received SLTDM join msg from server");
	// register this client
	commandToServer('SLTDMRegisterClient', $SLTDMClient::clientVersion, $SLTDMClient::clientVersionString);
}

function sltdmVersion(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
	%version = detag(%a1);
	%versionString = detag(%a2);

	// show and store the server version info
	echo("SLTDM server version: "@%versionString@" ("@%version@")");
	$SLTDMClient::serverVersion = %version;
	$SLTDMClient::serverVersionString = %versionString;
}

function sltdmSwapTeamLines()
{
	// if player's team is on the bottom of the objective hud,
	// swap them so that it's on top
	%bLeft = "GuiTextObjHudLeftProfile";
	%bCenter = "GuiTextObjHudCenterProfile";
	%gLeft = "GuiTextObjGreenLeftProfile";
	%gCenter = "GuiTextObjGreenCenterProfile";

	%teamOneY = getWord(objectiveHud.teamName[1].position, 1);
	%teamTwoY = getWord(objectiveHud.teamName[2].position, 1);
	if(%teamOneY > %teamTwoY)
	{
		// if team one was on the second line, now it'll be on the first
		%newTop = 1;
		%newBottom = 2;
	}
	else
	{
		// if team one was on the first line, now it'll be on the second
		%newTop = 2;
		%newBottom = 1;
	}

	%nameX = firstWord(objectiveHud.teamName[1].position);
	objectiveHud.teamName[1].position = %nameX SPC %teamTwoY;
	objectiveHud.teamName[2].position = %nameX SPC %teamOneY;
	objectiveHud.teamName[%newTop].setProfile(%gLeft);
	objectiveHud.teamName[%newBottom].setProfile(%bLeft);

	%scoreX = firstWord(objectiveHud.teamScore[1].position);
	objectiveHud.teamScore[1].position = %scoreX SPC %teamTwoY;
	objectiveHud.teamScore[2].position = %scoreX SPC %teamOneY;
	objectiveHud.teamScore[%newTop].setProfile(%gCenter);
	objectiveHud.teamScore[%newBottom].setProfile(%bCenter);

	%leaderX = firstWord(objectiveHud.leaderName[1].position);
	objectiveHud.leaderName[1].position = %leaderX SPC %teamTwoY;
	objectiveHud.leaderName[2].position = %leaderX SPC %teamOneY;
	objectiveHud.leaderName[%newTop].setProfile(%gLeft);
	objectiveHud.leaderName[%newBottom].setProfile(%bLeft);
}

function sltdmAddTeam(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
        %teamNum = detag(%a1);
        %teamName = detag(%a2);
        %score = detag(%a3);
        %leader = detag(%a4);

	if (%score $= "")
		%score = "0";

        objectiveHud.teamName[%teamNum].setValue(%teamName);
        objectiveHud.teamScore[%teamNum].setValue(%score);
        objectiveHud.leaderName[%teamNum].setValue(%leader);
}

function sltdmTeamLeaderIs(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
	%teamNum = detag(%a1);
	%leader = detag(%a2);

	objectiveHud.leaderName[%teamNum].setValue(%leader);
}

function sltdmCheckTeamLines(%msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6)
{
	%team = detag(%a1);

	if (%team == 1)
		%other = 2;
	else if (%team == 2)
		%other = 1;
	else
		return;

	%tY = getWord(objectiveHud.teamName[%team].position, 1);
	%oY = getWord(objectiveHud.teamName[%other].position, 1);

	// if player's team is lower on objective hud than the
	// other team is, switch them
	if (%tY > %oY)
		sltdmSwapTeamLines();
}


// package begins

package sltdm_client2
{


function setupObjHud(%gameType)
{
	if (%gameType $= "SLTDMGame")
	{
		// set separators
		objectiveHud.setSeparators("75 100 144");
		objectiveHud.enableHorzSeparator();

		// Team names
		objectiveHud.teamName[1] = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "4 3";
			extent = "68 16";
			visible = "1";
		};

		objectiveHud.teamName[2] = new GuiTextCtrl() {
			profile = "GuiTextObjHudLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "4 19";
			extent = "68 16";
			visible = "1";
		};

		// Team scores
		objectiveHud.teamScore[1] = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "78 3";
			extent = "22 16";
			visible = "1";
		};

		objectiveHud.teamScore[2] = new GuiTextCtrl() {
			profile = "GuiTextObjHudCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "78 19";
			extent = "22 16";
			visible = "1";
		};

		// Team leader labels
		objectiveHud.leaderLabel[1] = new GuiTextCtrl() {
			profile = "GuiTextObjGreenCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "103 3";
			extent = "41 16";
			visible = "1";
			text = "LEADER";
		};

		objectiveHud.leaderLabel[2] = new GuiTextCtrl() {
			profile = "GuiTextObjHudCenterProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "103 19";
			extent = "41 16";
			visible = "1";
			text = "LEADER";
		};

		// Team leaders
		objectiveHud.leaderName[1] = new GuiTextCtrl() {
			profile = "GuiTextObjGreenLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "149 3";
			extent = "102 16";
			visible = "1";
		};

		objectiveHud.leaderName[2] = new GuiTextCtrl() {
			profile = "GuiTextObjHudLeftProfile";
			horizSizing = "right";
			vertSizing = "bottom";
			position = "149 19";
			extent = "102 16";
			visible = "1";
		};

		for(%i = 1; %i <= 2; %i++)
		{
			objectiveHud.add(objectiveHud.teamName[%i]);
			objectiveHud.add(objectiveHud.teamScore[%i]);
			objectiveHud.add(objectiveHud.leaderLabel[%i]);
			objectiveHud.add(objectiveHud.leaderName[%i]);
		}

		chatPageDown.setVisible(false);
	}
	else
		Parent::setupObjHud(%gameType);
}


};

// package ends


// package begins

package sltdm_client1
{


function DispatchLaunchMode()
{
	echo("Adding message callbacks for SLTDM...");

	addMessageCallback('MsgClientJoin', sltdmClientJoin);
	addMessageCallback('MsgSLTDMCheckTeamLines', sltdmCheckTeamLines);
	addMessageCallback('MsgSLTDMAddTeam', sltdmAddTeam);
	addMessageCallback('MsgSLTDMTeamLeaderIs', sltdmTeamLeaderIs);
	addMessageCallback('MsgSLTDMVersion', sltdmVersion);

	Parent::DispatchLaunchMode();

	echo("Setting up SLTDM objective HUD...");
	activatePackage(sltdm_client2);
}


};

// package ends

// activate the package
activatePackage(sltdm_client1);
