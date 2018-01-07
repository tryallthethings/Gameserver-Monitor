/*
	Copyright (c) 2004 Cory Nelson

	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the
	"Software"), to deal in the Software without restriction, including
	without limitation the rights to use, copy, modify, merge, publish,
	distribute, sublicense, and/or sell copies of the Software, and to
	permit persons to whom the Software is furnished to do so, subject to
	the following conditions:

	The above copyright notice and this permission notice shall be included
	in all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
	TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
	SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

/*
	Changed by Franz Pentenrieder
*/

#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	/// <summary>
	/// Gameserver types
	/// </summary>
	public enum GameType
	{
		#region Ase
        /// <summary>
        /// SAMP
        /// </summary>
        GtaSA = GameProtocol.Samp,
		/// <summary>
		/// Multi-Theft Auto
		/// </summary>
		MultiTheftAuto = GameProtocol.Ase,
		/// <summary>
		/// Multi-Theft Auto: Vice City
		/// </summary>
		MultiTheftAutoViceCity = GameProtocol.Ase,
		/// <summary>
		/// Age of Empires 2
		/// </summary>
		AgeOfEmpires2 = GameProtocol.Ase,
		/// <summary>
		/// Age of Empires 2: The Conquerors
		/// </summary>
		AgeOfEmpires2TheConquerors = GameProtocol.Ase,
		/// <summary>
		/// Soldat
		/// </summary>
		Soldat = GameProtocol.Ase,
		/// <summary>
		/// Devastation
		/// </summary>
		Devastation = GameProtocol.Ase,
		/// <summary>
		/// Purge Jihad
		/// </summary>
		PurgeJihad = GameProtocol.Ase,
		/// <summary>
		/// Universal Combat
		/// </summary>
		UniversalCombat = GameProtocol.Ase,
		/// <summary>
		/// Xpand Rally
		/// </summary>
		XpandRally = GameProtocol.Ase,
		/// <summary>
		/// Far Cry
		/// </summary>
		FarCry = GameProtocol.Ase,
		/// <summary>
		/// Medal of Honor: Pacific Assault
		/// </summary>
		MedalOfHonorPacificAssault = GameProtocol.Ase,
		/// <summary>
		/// Chaser
		/// </summary>
		Chaser = GameProtocol.Ase,
		/// <summary>
		/// Chrome
		/// </summary>
		Chrome = GameProtocol.Ase,
		#endregion
		#region Quake3
		/// <summary>
		/// Quake 3
		/// </summary>
		Quake3 = GameProtocol.Quake3,
		/// <summary>
		/// Call of Duty
		/// </summary>
		CallOfDuty = GameProtocol.Quake3,
		/// <summary>
		/// Call of Duty 4
		/// </summary>
		CallOfDuty4 = GameProtocol.Quake3,
		/// <summary>
		/// Star Wars Jedi Knight: Jedi Academy
		/// </summary>
		JediKnightJediAcademy = GameProtocol.Quake3,
		/// <summary>
		/// Call of Duty: United Offensive
		/// </summary>
		CallOfDutyUnitedOffensive = GameProtocol.Quake3,
		/// <summary>
		/// Star Trek: Voyager - Elite Force
		/// </summary>
		StarTrekVoyagerEliteForce = GameProtocol.Quake3,
		/// <summary>
		/// Star Trek: Voyager - Elite Force 2
		/// </summary>
		StarTrekVoyagerEliteForce2 = GameProtocol.Quake3,
		/// <summary>
		/// Soldier of Fortune 2
		/// </summary>
		SoldierOfFortune2 = GameProtocol.Quake3,
		/// <summary>
		/// Return to Castle Wolfenstein
		/// </summary>
		ReturnToCastleWolfenstein = GameProtocol.Quake3,
		/// <summary>
		/// Wolfenstein: Enemy Territory
		/// </summary>
		WolfensteinEnemyTerritory = GameProtocol.Quake3,
		/// <summary>
		/// Star Wars Jedi Knight 2: Jedi Outcast
		/// </summary>
		JediKnightJediOutcast = GameProtocol.Quake3,
		/// <summary>
		/// Soldier of Fortune
		/// </summary>
		SoldierOfFortune = GameProtocol.Quake3,
		/// <summary>
		/// Daikatana
		/// </summary>
		Daikatana = GameProtocol.Quake3,
		#endregion
		#region Half-Life
		/// <summary>
		/// Half-Life
		/// </summary>
		HalfLife = GameProtocol.HalfLife,
		/// <summary>
		/// Counter-Strike
		/// </summary>
		CounterStrike = GameProtocol.HalfLife,
		/// <summary>
		/// Counter-Strike v 48 protocol
		/// </summary>
        CounterStrikev48 = GameProtocol.HalfLife, 
            //due to dproto trouble, query protocol even for 48 servers should be HL's one. Normally - Source
		/// <summary>
		/// Counter-Strike: Condition Zero
		/// </summary>
		CounterStrikeConditionZero = GameProtocol.HalfLife,
		/// <summary>
		/// Day of Defeat
		/// </summary>
		DayOfDefeat = GameProtocol.HalfLife,
		/// <summary>
		/// Gunman Chronicles
		/// </summary>
		GunmanChronicles = GameProtocol.HalfLife,
		#endregion
		#region GameSpy
		/// <summary>
		/// Doom 3
		/// </summary>
		Doom3  = GameProtocol.Doom3,
		/// <summary>
		/// Unreal Tournament 2003
		/// </summary>
		UnrealTournament2003 = GameProtocol.GameSpy,
		/// <summary>
		/// Unreal Tounrmanet 2004
		/// </summary>
		UnrealTournament2004 = GameProtocol.GameSpy,
		/// <summary>
		/// Unreal 2 XMP
		/// </summary>
		Unreal2XMP = GameProtocol.GameSpy,
		/// <summary>
		/// Medal of Honor: Allied Assault
		/// </summary>
		MedalOfHonorAlliedAssault = GameProtocol.GameSpy,
		/// <summary>
		/// Medal of Honor: Breakthrough
		/// </summary>
		MedalOfHonorBreakthrough = GameProtocol.GameSpy,
		/// <summary>
		/// Medal of Honor: Spearhead
		/// </summary>
		MedalOfHonorSpearhead = GameProtocol.GameSpy,
		/// <summary>
		/// GameSpy (Generic Protocol)
		/// </summary>
		GameSpy = GameProtocol.GameSpy,
		/// <summary>
		/// Unreal Tournament
		/// </summary>
		UnrealTournament = GameProtocol.GameSpy,
		/// <summary>
		/// Descent 3
		/// </summary>
		Descent3 = GameProtocol.GameSpy,
		/// <summary>
		/// Battlefield 1942
		/// </summary>
		Battlefield1942 = GameProtocol.GameSpy,
		/// <summary>
		/// Postal 2
		/// </summary>
		Postal2 = GameProtocol.GameSpy,
		/// <summary>
		/// Deus Ex
		/// </summary>
		DeusEx = GameProtocol.GameSpy,
		/// <summary>
		/// IL-2 Sturmovik
		/// </summary>
		IL2Sturmovik = GameProtocol.GameSpy,
		/// <summary>
		/// IL-2 Sturmovik: Forgotten Battles
		/// </summary>
		IL2SturmovikForgottenBattles = GameProtocol.GameSpy,
		/// <summary>
		/// Heretic 2
		/// </summary>
		Heretic2 = GameProtocol.GameSpy,
		/// <summary>
		/// IGI-2: Covert Strike
		/// </summary>
		IGI2CovertStrike = GameProtocol.GameSpy,
		/// <summary>
		/// Gore
		/// </summary>
		Gore = GameProtocol.GameSpy,
		/// <summary>
		/// Vietcong
		/// </summary>
		Vietcong = GameProtocol.GameSpy,
		/// <summary>
		/// Serious Sam
		/// </summary>
		SeriousSam = GameProtocol.GameSpy,
		/// <summary>
		/// Serious Sam: The Second Encounter
		/// </summary>
		SeriousSamTheSecondEncounter = GameProtocol.GameSpy,
		/// <summary>
		/// Aliens vs. Predators 2
		/// </summary>
		AliensVsPredators2 = GameProtocol.GameSpy,
		/// <summary>
		/// No One Lives Forever
		/// </summary>
		NoOneLivesForever = GameProtocol.GameSpy,
		/// <summary>
		/// No One Lives Forever 2
		/// </summary>
		NoOneLivesForever2 = GameProtocol.GameSpy,
		/// <summary>
		/// Shogo
		/// </summary>
		Shogo = GameProtocol.GameSpy,
		/// <summary>
		/// Codename: Eagle
		/// </summary>
		CodenameEagle = GameProtocol.GameSpy,
		/// <summary>
		/// Giants: Citizen Kabuto
		/// </summary>
		GiantsCitizenKabuto = GameProtocol.GameSpy,
		/// <summary>
		/// Global Operations
		/// </summary>
		GlobalOperations = GameProtocol.GameSpy,
		/// <summary>
		/// Nerf ArenaBlast
		/// </summary>
		NerfArenaBlast = GameProtocol.GameSpy,
		/// <summary>
		/// RalliSport Challenge
		/// </summary>
		RalliSportChallenge = GameProtocol.GameSpy,
		/// <summary>
		/// Rally Masters
		/// </summary>
		RallyMasters = GameProtocol.GameSpy,
		/// <summary>
		/// Command and Conquer: Renegade
		/// </summary>
		CommandAndConquerRenegade = GameProtocol.GameSpy,
		/// <summary>
		/// Rune
		/// </summary>
		Rune = GameProtocol.GameSpy,
		/// <summary>
		/// Sin
		/// </summary>
		Sin = GameProtocol.GameSpy,
		/// <summary>
		/// Tactical Ops
		/// </summary>
		TacticalOps = GameProtocol.GameSpy,
		/// <summary>
		/// Unreal
		/// </summary>
		Unreal = GameProtocol.GameSpy,
		/// <summary>
		/// Wheel of Time
		/// </summary>
		WheelOfTime = GameProtocol.GameSpy,
		/// <summary>
		/// Deadly Dozen: Pacific Theater
		/// </summary>
		DeadlyDozenPacificTheater = GameProtocol.GameSpy,
		/// <summary>
		/// Dirt Track Racing 2
		/// </summary>
		DirtTrackRacing2 = GameProtocol.GameSpy,
		/// <summary>
		/// Drakan: Order of the Flame
		/// </summary>
		DrakanOrderOfTheFlame = GameProtocol.GameSpy,
		/// <summary>
		/// F1 2002
		/// </summary>
		F12002 = GameProtocol.GameSpy,
		/// <summary>
		/// Iron Storm
		/// </summary>
		IronStorm = GameProtocol.GameSpy,
		/// <summary>
		/// James Bond: Nightfire
		/// </summary>
		JamesBondNightfire = GameProtocol.GameSpy,
		/// <summary>
		/// Kingpin
		/// </summary>
		Kingpin = GameProtocol.GameSpy,
		/// <summary>
		/// Need for Speed: Hot Pursuit 2
		/// </summary>
		NeedForSpeedHotPursuit2 = GameProtocol.GameSpy,
		/// <summary>
		/// Redline
		/// </summary>
		Redline = GameProtocol.GameSpy,
		/// <summary>
		/// Turok 2
		/// </summary>
		Turok2 = GameProtocol.GameSpy,
		/// <summary>
		/// Tron 2.0
		/// </summary>
		Tron2 = GameProtocol.GameSpy,
		/// <summary>
		/// Tony Hawk's Pro Skater 3
		/// </summary>
		TonyHawksProSkater3 = GameProtocol.GameSpy,
		/// <summary>
		/// Tony Hawk's Pro Skater 4
		/// </summary>
		TonyHawksProSkater4 = GameProtocol.GameSpy,
		/// <summary>
		/// V8 Supercar Challenge
		/// </summary>
		V8SupercarChallenge = GameProtocol.GameSpy,
		/// <summary>
		/// Team Factor
		/// </summary>
		TeamFactor = GameProtocol.GameSpy,
		/// <summary>
		/// Rainbow Six
		/// </summary>
		RainbowSix = GameProtocol.GameSpy,
		/// <summary>
		/// Rainbow Six: Rogue Spear
		/// </summary>
		RainbowSixRogueSpear = GameProtocol.GameSpy,
		/// <summary>
		/// Nitro Family
		/// </summary>
		NitroFamily = GameProtocol.GameSpy,
		/// <summary>
		/// Rise of Nations
		/// </summary>
		RiseOfNations = GameProtocol.GameSpy,
		/// <summary>
		/// Contract J.A.C.K.
		/// </summary>
		ContractJACK = GameProtocol.GameSpy,
		/// <summary>
		/// Homeworld 2
		/// </summary>
		Homeworld2 = GameProtocol.GameSpy,
		/// <summary>
		/// Breed
		/// </summary>
		Breed = GameProtocol.GameSpy2,
		/// <summary>
		/// Operation Flashpoint: Resistance
		/// </summary>
		OperationFlashPointResistance = GameProtocol.GameSpy,
		/// <summary>
		/// Star Trek: Bridge Commander
		/// </summary>
		StarTrekBridgeCommander = GameProtocol.GameSpy,
		/// <summary>
		/// Tribes: Vengeance
		/// </summary>
		TribesVengeance = GameProtocol.GameSpy,
		/// <summary>
		/// Tony Hawk's Underground 2
		/// </summary>
		TonyHawksUnderground2 = GameProtocol.GameSpy,
		#endregion
		#region Gamespy v2
		/// <summary>
		/// Star Wars: Battlefront
		/// </summary>
		StarWarsBattlefront = GameProtocol.GameSpy2,
		/// <summary>
		/// GameSpy 2 (Generic Protocol)
		/// </summary>
		GameSpy2 = GameProtocol.GameSpy2,
		/// <summary>
		/// Battlefield Vietnam
		/// </summary>
		BattlefieldVietnam = GameProtocol.GameSpy2,
		/// <summary>
		/// Painkiller
		/// </summary>
		Painkiller = GameProtocol.GameSpy2,
		/// <summary>
		/// Halo: Combat Evolved
		/// </summary>
		HaloCombatEvolved = GameProtocol.GameSpy2,
		/// <summary>
		/// America's Army
		/// </summary>
		AmericasArmy = GameProtocol.GameSpy2,
		/// <summary>
		/// Neverwinter Nights
		/// </summary>
		NeverwinterNights = GameProtocol.GameSpy2,
		/// <summary>
		/// Operation Flashpoint
		/// </summary>
		OperationFlashpoint = GameProtocol.GameSpy2,
		#endregion
		#region Hl-Source
		/// <summary>
		/// Source Engine (Generic Protocol)
		/// </summary>
		Source = GameProtocol.Source,
		/// <summary>
		/// Half-Life 2
		/// </summary>
		HalfLife2 = GameProtocol.Source,
		/// <summary>
		/// Team Fortress 2
		/// </summary>
		TeamFortress2 = GameProtocol.Source,
		/// <summary>
		/// Counter-Strike: Source
		/// </summary>
		CounterStrikeSource = GameProtocol.Source,
        /// <summary>
        /// Left4Dead
        /// </summary>
        Left4Dead = GameProtocol.Source,

        Left4Dead2 = GameProtocol.Source,
		#endregion
		/// <summary>
		/// Not listed game
		/// </summary>
		Unknown = GameProtocol.None
	}

	/// <summary>
	/// Gameserver protocol
	/// </summary>
	public enum GameProtocol
    {
        /// <summary>
        /// GtaSA
        /// </summary>
        Samp,
		/// <summary>
		/// The all seeing eye
		/// </summary>
		Ase,
		/// <summary>
		/// Doom3
		/// </summary>
		Doom3,
		/// <summary>
		/// Halflife and HL-Mods
		/// </summary>
		HalfLife,
		/// <summary>
		/// Quake3 and Q3-Mods
		/// </summary>
		Quake3,
		/// <summary>
		/// Halflife Source
		/// </summary>
		Source,
		/// <summary>
		/// Gamespy v1
		/// </summary>
		GameSpy,
		/// <summary>
		/// Gamespy v2
		/// </summary>
		GameSpy2,
		/// <summary>
		/// Unkown
		/// </summary>
		None
	}


}
