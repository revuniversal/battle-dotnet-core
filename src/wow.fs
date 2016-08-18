module WoW

open BattleNet
open UriBuilding
open System
open Newtonsoft.Json

let getLocale locale = 
    match locale with
    | EN_US -> Query { key = "locale"; value = "en_us" }
    | EN_GB -> Query { key = "locale"; value = "en_gb" }

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

    let achievementEndpoint region id locale apikey =
        let protocol = Protocol Https
        let subdomain = Subdomain (getRegion region)
        let host = Host "api.battle.net"
        let game = Resource { key = "wow" }
        let api = Resource { key = "achievement" }
        let achievement = Resource { key = id }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [protocol; subdomain; host; game;api;achievement;locale;key]

    let achievement region achievement locale apikey = async {
        let endpoint = achievementEndpoint region achievement locale apikey
        let! json = get endpoint 
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
    
    let auctionEndpoint region realm locale apikey =
        let protocol = Protocol Https
        let subdomain = Subdomain (getRegion region)
        let host = Host "api.battle.net"
        let game = Resource { key = "wow" }
        let api = Resource { key = "auction" }
        let data = Resource { key = "data" }
        let realm = Resource { key = realm }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [protocol; subdomain; host; game;api;data;realm;locale;key]

    let getByUrl (uri:string)  = async {
        use http = new System.Net.Http.HttpClient()
        let! json = http.GetStringAsync(uri) |> Async.AwaitTask
        let data = JsonConvert.DeserializeObject<AuctionData>(json)
        return data}

    let auctionFile region realm locale apikey = async {
        let endpoint = auctionEndpoint region realm locale apikey
        let! json = get endpoint 
        let data = JsonConvert.DeserializeObject<Files>(json)
        return data}

    let auction file lastmodified = async {
        let modified = 
            match lastmodified with
            | Default -> int64 1
            | Specified (x) -> x

        let auctionData = 
            match modified = file.lastModified with
            | true -> None
            | false -> Some (getByUrl(file.url) |> Async.RunSynchronously) 

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

    let bossEndpoint region id locale apikey =
        let protocol = Protocol Https
        let subdomain = Subdomain (getRegion region)
        let host = Host "api.battle.net"
        let game = Resource { key = "wow" }
        let api = Resource { key = "boss" }
        let boss = Resource { key = id }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [protocol; subdomain; host; game;api;boss;locale;key]

    let bossesEndpoint region locale apikey =
        let protocol = Protocol Https
        let subdomain = Subdomain (getRegion region)
        let host = Host "api.battle.net"
        let game = Resource { key = "wow" }
        let api = Resource { key = "boss/" }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [protocol; subdomain; host; game;api;locale;key]

    let boss region bossId locale apikey = async {
        let endpoint = bossEndpoint region bossId locale apikey
        let! json = get endpoint
        let data = JsonConvert.DeserializeObject<Boss>(json)
        return data}

    let bosses region locale apikey = async {
        let endpoint = bossesEndpoint region locale apikey
        let! json = get endpoint
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
    let realmLeaderboardEndpoint region realm locale apikey = 
        let protocol = Protocol Https
        let subdomain = Subdomain (getRegion region)
        let host = Host "api.battle.net"
        let game = Resource { key = "wow" }
        let api = Resource { key = "challenge" }
        let realm = Resource { key = realm }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [protocol; subdomain; host; game;api;realm;locale;key]

    // ez mode
    let realmLeaderboard region realm locale apikey = async {
        let endpoint = realmLeaderboardEndpoint region realm locale apikey
        let! json = get endpoint
        let data = JsonConvert.DeserializeObject<ChallengeRealmLeaderboard>(json)
        return data}

    let regionLeaderboardEndpoint region locale apikey = 
        let protocol = Protocol Https
        let subdomain = Subdomain (getRegion region)
        let host = Host "api.battle.net"
        let game = Resource { key = "wow" }
        let api = Resource { key = "challenge" }
        let region = Resource { key = "region" }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [protocol; subdomain; host; game;api;region;locale;key]

    let regionLeaderboard region realm locale apikey = async {
        let endpoint = realmLeaderboardEndpoint region realm locale apikey
        let! json = get endpoint
        let data = JsonConvert.DeserializeObject<ChallengeRegionLeaderboard>(json)
        return data}
//module CharacterProfile =
//module GuildProfile =
//module Item =
//module Mount =
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
    let leaderboardEndpoint region bracket locale apikey = 
        let protocol = Protocol Https
        let subdomain = Subdomain (getRegion region)
        let host = Host "api.battle.net"
        let game = Resource { key = "wow" }
        let api = Resource { key = "leaderboard" }
        let bracket = Resource { key = bracket }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [protocol; subdomain; host; game; api; bracket; locale; key]

    // ez mode
    let leaderboard region bracket locale apikey = async {
        let endpoint = leaderboardEndpoint region bracket locale apikey
        let! json = get endpoint
        let data = JsonConvert.DeserializeObject<Leaderboard>(json)
        return data}
//module Quest =
//module RealmStatus =
//module Recipe =
//module Spell =
//module Zone =
//module DataResources =