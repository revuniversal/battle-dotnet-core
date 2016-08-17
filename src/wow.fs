module WoW

open BattleNet
open UriBuilding
open System
open Newtonsoft.Json

let getLocale locale = 
    match locale with
    | EN_US -> Query { key = "locale"; value = "en_us" }
    | EN_GB -> Query { key = "locale"; value = "en_gb" }

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
    let leaderboardEndpoint bracket locale apikey = 
        let game = Resource { key = "wow" }
        let api = Resource { key = "leaderboard" }
        let bracket = Resource { key = bracket }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [game;api;bracket;locale;key]

    // ez mode
    let leaderboard region bracket locale apikey = async {
        let endpoint = leaderboardEndpoint bracket locale apikey
        let! json = get endpoint region Protocol.Https
        let data = JsonConvert.DeserializeObject<Leaderboard>(json)
        return data}
       
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
        factionId:int;
    }

    let achievementEndpoint id locale apikey =
        let game = Resource { key = "wow" }
        let api = Resource { key = "achievement" }
        let achievement = Resource { key = id }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [game;api;achievement;locale;key]

    let achievement region achievement locale apikey = async {
        let endpoint = achievementEndpoint achievement locale apikey
        let! json = get endpoint region Protocol.Https
        let data = JsonConvert.DeserializeObject<Achievement>(json)
        return data}

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

    let bossEndpoint id locale apikey =
        let game = Resource { key = "wow" }
        let api = Resource { key = "boss" }
        let boss = Resource { key = id }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [game;api;boss;locale;key]

    let bossesEndpoint locale apikey =
        let game = Resource { key = "wow" }
        let api = Resource { key = "boss/" }
        let locale = getLocale locale 
        let key = Query { key = "apikey"; value = apikey }
        [game;api;locale;key]

    let boss region bossId locale apikey = async {
        let endpoint = bossEndpoint bossId locale apikey
        let! json = get endpoint region Protocol.Https
        let data = JsonConvert.DeserializeObject<Boss>(json)
        return data}

    let bosses region locale apikey = async {
        let endpoint = bossesEndpoint locale apikey
        let! json = get endpoint region Protocol.Https
        let data = JsonConvert.DeserializeObject<BossData>(json)
        return data}
