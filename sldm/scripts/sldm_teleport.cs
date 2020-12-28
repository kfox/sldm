package sldm_teleport
{


function ShocklanceDMGame::activateTeleporter(%game, %obj, %colObj)
{
	messageClient(%colObj.client, '', '\c2Teleporter activated.~wfx/misc/diagnostic_on.wav');
	%game.teleportPlayer(%colObj);

	%pos = %obj.position;
	%poof = new ParticleEmissionDummy(Teleport)
	{
		position	= getWord(%pos,0) SPC getWord(%pos,1) SPC getWord(%pos,2) + 2.2 SPC 1;
		rotation	= "1 0 0 0";
		scale		= "4.0 4.0 4.0";
		dataBlock	= "defaultEmissionDummy";
		emitter		= "TeleportEmitter";
		velocity	= "0";
	};

	%obj.mainObj.playAudio($ActivateSound, "TeleportSound");
}

function ShocklanceDMGame::teleportPlayer(%game, %obj)
{
	%obj.setCloaked(true);
	%obj.schedule(500, "setCloaked", false);
	%rot = getWords(%obj.getTransform(),3, 6);
	%pos = %game.pickPlayerSpawn(%obj);
	%obj.setTransform(getWord(%pos,0) SPC getWord(%pos,1) SPC getWord(%pos,2) SPC %rot);

	%pos = %obj.position;
	%poof = new ParticleEmissionDummy(Teleport)
	{
		position	= getWord(%pos,0) SPC getWord(%pos,1) SPC getWord(%pos,2) + 0.8 SPC 1;
		rotation	= "1 0 0 0";
		scale		= "30.0 30.0 30.0";
		dataBlock	= "defaultEmissionDummy";
		emitter		= "MaterializeEmitter";
		velocity	= "4";
	};

	%obj.playAudio($ActivateSound, "MaterializeSound");
}


};

datablock EffectProfile(TeleportEffect)
{
	effectname = "misc/diagnostic_on";
	minDistance = 5.0;
	maxDistance = 7.5;
};

datablock AudioProfile(TeleportSound)
{
	fileName = "fx/misc/diagnostic_on.wav";
	description = AudioClose3d;
	preload = true;
	effect = TeleportEffect;
};

datablock EffectProfile(MaterializeEffect)
{
	effectname = "misc/SHIELDH1";
	minDistance = 5.0;
	maxDistance = 7.5;
};

datablock AudioProfile(MaterializeSound)
{
	fileName = "fx/misc/SHIELDH1.WAV";
	description = AudioClose3d;
	preload = true;
	effect = MaterializeEffect;
};

datablock ParticleData(TeleportChaff)
{
	dragCoefficient		= 0.1;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 2500;
	lifetimeVarianceMS	= 350;
	textureName		= "special/LightFalloffMono";
	colors[0]		= "0.05 0.05 0.05 1";
	colors[1]		= "0.1 0.1 0.13 1";
	colors[2]		= "0.12 0.12 0.2 1";
	colors[3]		= "0.2 0.2 0.3 1";
	sizes[0]		= 0.5;
	sizes[1]		= 1.0;
	sizes[2]		= 2.5;
	sizes[3]		= 4.5;
	times[0]		= 0.0;
	times[1]		= 0.5;
	times[2]		= 0.9;
	times[3]		= 1.5;
};

datablock ParticleData(MaterializeChaff)
{
	dragCoefficient		= 0.2;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.5;
	constantAcceleration	= 0.1;
	lifetimeMS		= 1500;
	lifetimeVarianceMS	= 200;
	textureName		= "special/LightFalloffMono";
	colors[0]		= "0.2 0.2 0.3 1";
	colors[1]		= "0.12 0.12 0.2 1";
	colors[2]		= "0.1 0.1 0.13 1";
	colors[3]		= "0.05 0.05 0.05 1";
	sizes[0]		= 2.5;
	sizes[1]		= 2.0;
	sizes[2]		= 1.0;
	sizes[3]		= 0.5;
	times[0]		= 0.0;
	times[1]		= 0.5;
	times[2]		= 0.7;
	times[3]		= 1.1;
};

datablock ParticleEmitterData(TeleportEmitter)
{
	ejectionPeriodMS	= 5;
	periodVarianceMS	= 3;
	ejectionVelocity	= 1.1;
	velocityVariance	= 1.0;
	ejectionOffset		= 0;
	thetaMin		= 0;
	thetaMax		= 180;
	phiReferenceVel		= 0;
	phiVariance		= 360;
	overrideAdvances	= false;
	orientParticles		= false;
	lifetimeMS		= 350;
	particles		= "TeleportChaff";
};

datablock ParticleEmitterData(MaterializeEmitter)
{
	ejectionPeriodMS	= 3;
	periodVarianceMS	= 1;
	ejectionVelocity	= 2.1;
	velocityVariance	= 1.0;
	ejectionOffset		= 0;
	thetaMin		= 0;
	thetaMax		= 180;
	phiReferenceVel		= 0;
	phiVariance		= 360;
	overrideAdvances	= false;
	orientParticles		= true;
	lifetimeMS		= 150;
	particles		= "MaterializeChaff";
};
