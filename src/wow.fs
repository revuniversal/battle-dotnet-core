module WoW

open BattleNet
open UriBuilding
open System
open Newtonsoft.Json

let getLocale locale = 
    match locale with
    | EN_US -> { key = "locale"; value = "en_us" }
    | EN_GB -> { key = "locale"; value = "en_gb" }

let createUri region locale apikey resources =
    let locale = getLocale locale 
    let key = { key = "apikey"; value = apikey }
    {   
        scheme = Https;
        subdomains = [getRegion region];
        host = "api.battle.net"
        resources = resources;
        file = "";
        extension = "";
        query = [locale;key];
    }

module Achievement =

    type ToolTipParams = {timewalkerLevel:int}
    type RewardItem = {
        id:int;
        name:string;
        icon:string;
        quality:int;
        itemlevel:int;
        tooltipParams:ToolTipParams;
        stats:Object list;
        armor:int;
        context:string;
        bonusLists:Object list}
    type Criterion = {
        id:int;
        descripton:string;
        orderIndex:int;
        max:int;}
    type Achievement = {
        id:int;
        title:string;
        points:int;
        description:string;
        reward:string;
        rewardItems:RewardItem list;
        icon:string;
        criteria:Criterion list;
        accountWide:bool;
        factionId:int;}

    let achievementUri region id locale apikey =
        createUri region locale apikey ["wow"; "achievement"; id]

    let achievement region achievement locale apikey = async {
        let uri = achievementUri region achievement locale apikey
        let! json = get uri 
        let data = JsonConvert.DeserializeObject<Achievement>(json)
        return data}

module Auction =
    type Realm = { name:string; slug:string; }
    type Modifier = { ``type``:int; value:int; }
    type File = { url:string; lastModified:Int64 }
    type Files = { files: File list }
    type BonusList = { bonusListId:int }
    type Auction = {
        auc:int;
        item:int;
        owner:string;
        ownerRealm:string;
        bid:Int64;
        buyout:Int64;
        quantity:int;
        timeLeft:string;
        rand:int;
        seed:Int64;
        context:int;
        modifiers: Modifier list;
        petSpeciesId: Nullable<int>;
        petBreedId: Nullable<int>;
        petLevel: Nullable<int>;
        petQualityId: Nullable<int>;
        bonusLists: BonusList list}
    type AuctionData = { realms: Realm list; auctions: Auction list }
    type LastModified = Default | Specified of Int64
    
    let auctionUri region realm locale apikey =
        createUri region locale apikey ["wow"; "auction"; "data"; realm]

    let auctionFile region realm locale apikey = async {
        let uri = auctionUri region realm locale apikey
        let! json = get uri 
        let data = JsonConvert.DeserializeObject<Files>(json)
        return data}

    let auction file lastmodified = async {
        let getAuctionData (url:string)  = async {
            let! json = getByUrl(url)
            let data = JsonConvert.DeserializeObject<AuctionData>(json)
            return data}

        let modified = 
            match lastmodified with
            | Default -> int64 1
            | Specified (x) -> x

        let auctionData = 
            match modified = file.lastModified with
            | true -> None
            | false -> Some (getAuctionData(file.url) |> Async.RunSynchronously) 

        return auctionData}

module Boss =
    type Npc = { id:int; name:string; urlSlug:string; }
    type Location = { id:int; name:string; }
    type Boss = {
        id:int;
        name:string;
        urlSlug:string;
        description:string;
        zoneId:int;
        availableInNormalMode:bool;
        availableInHeroicMode:bool;
        health:int;
        heroicHealth:int;
        level:int;
        heroicLevel:int;
        journalId:int;
        npcs: Npc list}
    type BossData = { bosses: Boss list}

    let bossUri region id locale apikey =
        createUri region locale apikey ["wow"; "boss"; id]

    let bossesUri region locale apikey =
        createUri region locale apikey ["wow"; "boss/"]

    let boss region bossId locale apikey = async {
        let uri = bossUri region bossId locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Boss>(json)
        return data}

    let bosses region locale apikey = async {
        let uri = bossesUri region locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<BossData>(json)
        return data}


module ChallengeMode =
    type Realm = { name:string;
        slug:string;
        battlegroup:string;
        locale:string;
        timezone:string;
        connected_realms:string list;}

    type Criteria = { time:int; 
        hours:int;
        minutes:int;
        seconds:int;
        milliseconds:int;
        isPositive:bool;}

    type Map = { id:int;
        name:string;
        slug:string;
        hasChallengeMode:bool;
        bronzeCriteria:Criteria;
        silverCriteria:Criteria;
        goldCriteria:Criteria;}

    type Time = { time:int;
        hours:int;
        minutes:int;
        seconds:int;
        milliseconds:int;
        isPositive:bool;}

    type Spec = { name:string;
        role:string;
        backgroundImage:string;
        icon:string;
        description:string;
        order:int;}

    type Character = { name:string;
        realm:string;
        battlegroup:string;
        ``class``:int;
        race:int;
        gender:int;
        level:int;
        achievementPoints:int;
        thumbnail:string;
        spec:Spec;
        guild:string;
            guildRealm:string;
        lastModified:int;}
        
    type Member = {character:Character; spec:Spec; }
    type Group = {ranking:int;
        time:Time;
        date:string;
        medal:string;
        faction:string;
        isRecurring:bool;
        members:Member list;}

    type RealmChallenge = { realm:Realm; map:Map; groups:Group list; }
    type RegionChallenge = { map:Map; groups:Group list; }
    type ChallengeRealmLeaderboard = { challenge: RealmChallenge list; }
    type ChallengeRegionLeaderboard = {challenge: RegionChallenge list; }
    // compose mode
    let realmLeaderboardUri region realm locale apikey = 
        createUri region locale apikey ["wow"; "challenge"; realm]

    // ez mode
    let realmLeaderboard region realm locale apikey = async {
        let uri = realmLeaderboardUri region realm locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<ChallengeRealmLeaderboard>(json)
        return data}

    let regionLeaderboardUri region locale apikey = 
        createUri region locale apikey ["wow"; "challenge"; "region"]

    let regionLeaderboard region realm locale apikey = async {
        let uri = realmLeaderboardUri region realm locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<ChallengeRegionLeaderboard>(json)
        return data}
//module CharacterProfile =
//module GuildProfile =
//module Item =
module Mount =
    type Mount = { 
        name:string;
        spellId:int;
        creatureId:int;
        itemId:int;
        qualityId:int;
        icon:string;
        isGround:bool;
        isFlying:bool;
        isAquatic:bool;
        isJumping:bool}
    type Mounts = { mounts: Mount list}

    let mountUri region locale apikey = 
        createUri region locale apikey ["wow";"mount/"]

    let mounts region locale apikey = async {
        let uri = mountUri region locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Mounts>(json)
        return data
        }

//module Pet =
module PVP =
    type Row = {ranking: int; 
        rating: int; 
        name: string; 
        realmId:int;
        realmName:string; 
        realmSlug:string; 
        raceId:int;
        classId:int;
        specId:int;
        factionId:int;
        genderId:int;
        seasonWins:int;
        seasonLosses:int;
        weeklyWins:int;
        weeklyLosses:int}
    
    type Leaderboard = { rows : Row list }
    
    // compose mode
    let leaderboardUri region bracket locale apikey = 
        createUri region locale apikey ["wow"; "leaderboard"; bracket]

    // ez mode
    let leaderboard region bracket locale apikey = async {
        let uri = leaderboardUri region bracket locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Leaderboard>(json)
        return data}
module Quest =
    type Quest = {
        id: int;
        title: string;
        reqLevel: int;
        suggestedPartyMembers: int;
        category: string;
        level: int}

    let questUri region questId locale apikey = 
        createUri region locale apikey ["wow";"quest"; questId;]

    let quest region questId locale apikey = async {
        let uri = questUri region questId locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Quest>(json)
        return data
        }        
module RealmStatus =
    type Realm = {
        ``type``: string;
        population: string;
        queue: bool;
        status: bool;
        name: string;
        slug: string;
        battlegroup: string
        locale: string;
        timezome: string;
        connected_realms: string list}
    type Realms = {realms:Realm list}

    let realmUri region locale apikey = 
        createUri region locale apikey ["wow";"realm"; "status";]

    let realm region locale apikey = async {
        let uri = realmUri region locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Realms>(json)
        return data
        }

module Recipe =
    type Recipe = { 
        id: int;
        name: string;
        profession: string;
        icon: string}

    let recipeUri region recipeId locale apikey = 
        createUri region locale apikey ["wow";"recipe"; recipeId;]

    let recipe region recipeId locale apikey = async {
        let uri = recipeUri region recipeId locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Recipe>(json)
        return data
        }

module Spell =
    type Spell = {
        id: int;
        name: string;
        icon: string;
        description: string;
        range: string;
        powerCost: string;
        castTime: string;
        cooldown: string}

    let spellUri region spellId locale apikey = 
        createUri region locale apikey ["wow";"recipe"; spellId;]

    let spell region spellId locale apikey = async {
        let uri = spellUri region spellId locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Spell>(json)
        return data
        }        
module Zone =
    open Boss
    type Zone = {
        id:int;
        name:string;
        urlSlug:string;
        description:string;
        location:Location;
        expansionId:int;
        numPlayers:int;
        isDungeon:bool;
        isRaid:bool;
        advisedMinLevel:int;
        advisedMaxLevel:int;
        advisedHeroicMinLevel:int;
        advisedHeroicMaxLevel:int;
        availableModes:Object list;
        lfgNormalMinGearLevel:int;
        lfgHeroicMinGearLevel:int;
        floors:int;
        bosses: Boss list;
        patch:string;
    }
    type Zones = { zones: Zone list }

    let zonesUri region locale apikey = 
        createUri region locale apikey ["wow";"zone/";]

    let zones region locale apikey = async {
        let uri = zonesUri region locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Zones>(json)
        return data
        } 

    let zoneUri region zoneId locale apikey = 
        createUri region locale apikey ["wow";"zone"; zoneId;]

    let zone region zoneId locale apikey = async {
        let uri = zoneUri region zoneId locale apikey
        let! json = get uri
        let data = JsonConvert.DeserializeObject<Zone>(json)
        return data
        } 
//module DataResources =