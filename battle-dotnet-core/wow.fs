module WoW

open BattleNet
open UriBuilding
open System

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

    let achievementUri region id locale apikey = createUri region locale apikey ["wow"; "achievement"; id]
    let achievement region id locale apikey = achievementUri region id locale apikey |> get<Achievement>      
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
    
    let auctionUri region realm locale apikey = createUri region locale apikey ["wow"; "auction"; "data"; realm]
    let auctionFile region realm locale apikey = auctionUri region realm locale apikey |> get<Files> 
    let auction file lastmodified = async {
        let getAuctionData = httpget >> deserialize<AuctionData>

        let modified = 
            match lastmodified with
            | Default -> int64 1
            | Specified (x) -> x

        let auctionData = 
            match modified = file.lastModified with
            | true -> None
            | false -> 
            Some (getAuctionData(file.url) |> Async.RunSynchronously) 

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

    let bossUri region id locale apikey = createUri region locale apikey ["wow"; "boss"; id]
    let bossesUri region locale apikey = createUri region locale apikey ["wow"; "boss/"]
    let boss region bossId locale apikey = bossUri region bossId locale apikey |> get<Boss>
    let bosses region locale apikey = bossesUri region locale apikey |> get<BossData>
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

    let realmLeaderboardUri region realm locale apikey = createUri region locale apikey ["wow"; "challenge"; realm]
    let regionLeaderboardUri region locale apikey = createUri region locale apikey ["wow"; "challenge"; "region"]
    let realmLeaderboard region realm locale apikey = get<ChallengeRealmLeaderboard>(realmLeaderboardUri region realm locale apikey)
    let regionLeaderboard region realm locale apikey = get<ChallengeRegionLeaderboard>(regionLeaderboardUri region locale apikey) 
//module CharacterProfile =
//module GuildProfile =
module Item =
    type BonusStat = { stat:int; amount:int}
    type Damage = { min:int; max:int; exactMin:double; exactMax:double;}
    type WeaponInfo = { damage: Damage; weaponSpeed:double; dps:double; }
    type ItemSource = { sourceId:int; sourceType:string; }
    type BonusSummary = { 
        defaultBonusLists:Object list; 
        changeBonusLists: Object list;
        bonusChances:Object list}
    type Item = {
        id:int;
        disenchantingSkillRank:int;
        description:string;
        name:string;
        icon:string;
        stackable:int;
        itemBind:int;
        bonusStats: BonusStat list;
        itemSpells: Object list;
        buyPrice:int;
        itemClass:int;
        itemSubClass:int;
        containerSlots:int;
        weaponInfo:WeaponInfo;
        inventoryType:int;
        equippable:bool;
        itemLevel:int;
        maxCount:int;
        maxDurability:int;
        minFactionId:int;
        minReputation:int;
        quality:int;
        sellPrice:int;
        requiredSkillLevel:int;
        requiredLevel:int;
        requiredSkillRank:int;
        itemSource:ItemSource;
        baseArmor:int;
        hasSockets:bool;
        isAuctionable:bool;
        armor:int;
        displayInfold:int;
        nameDescription:string;
        nameDescriptionColor:int;
        upgradable:bool;
        heroicTooltip:bool;
        context:string;
        bonusLists: Object list;
        availableContexts:string list;
        bonusSummary:BonusSummary;
        artifactId:int}
    type SetBonus = { description:string; threshold:int; }
    type ItemSet = { id:int; name:string; setBonuses:SetBonus list; items: int list;}

    let itemUri region itemId locale apikey = createUri region locale apikey ["wow";"item"; itemId]
    let itemSetUri region setId locale apikey = createUri region locale apikey ["wow";"item"; "set"; setId]
    let item region itemId locale apikey = itemUri region itemId locale apikey |> get<Item>
    let itemSet region setId locale apikey = itemSetUri region setId locale apikey |> get<ItemSet>
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

    let mountUri region locale apikey = createUri region locale apikey ["wow";"mount/"]
    let mounts region locale apikey = mountUri region locale apikey |> get<Mounts>
module Pet =
    type Stats = {
        speciesId: int;
        breedId: int;
        petQualityId: int;
        level: int;
        health: int;
        power: int;
        speed: int;}
    type Pet = {
        canBattle: bool;
        creatureId: int;
        name: string;
        family: string;
        icon: string;
        qualityId: int;
        stats: Stats;
        strongAgainst: string list;
        typeId: int;
        weakAgainst: string list;}
    type Pets = { pets: Pet list; }
    type PetAbility = {
        id: int;
        name: string;
        icon: string;
        cooldown: int;
        rounds: int;
        petTypeId: int;
        isPassive: int;
        hideHints: int;}
    type SpeciesAbility = {
        slot: int;
        order: int;
        requiredLevel: int;
        id: int;
        name: string;
        icon: string;
        cooldown: int;
        rounds: int;
        petTypeId: int;
        isPassive: bool;
        hideHints: bool;}
    type Species = {
        speciesId: int;
        petTypeId: int;
        creatureId: int;
        name: string;
        canBattle: string;
        icon: string;
        description: string;
        source: string;
        abilities: SpeciesAbility list;}

    let petsUri region locale apikey = createUri region locale apikey ["wow"; "pet/";]
    let petAbilityUri region abilityId locale apikey = createUri region locale apikey ["wow"; "pet"; "ability"; abilityId;]
    let petSpeciesUri region speciesId locale apikey = createUri region locale apikey ["wow"; "pet"; "species"; speciesId;]
    let petStatsUri region speciesId level breedId qualityId locale apikey = 
        let uri = createUri region locale apikey ["wow"; "pet"; "stats"; speciesId;]
        { uri with query = [{key = "level"; value = level};
                                {key = "breedId"; value = breedId};
                                {key = "qualityId"; value = qualityId};
                                getLocale locale;
                                { key = "apikey"; value = apikey };]} 

    let pets region locale apikey = petsUri region locale apikey |> get<Pets>
    let petAbility region abilityId locale apikey = petAbilityUri region abilityId locale apikey |> get<PetAbility>
    let petSpecies region speciesId locale apikey = petSpeciesUri region speciesId locale apikey |> get<Species>
    let petStats region speciesId level breedId qualityId locale apikey = petStatsUri region speciesId level breedId qualityId locale apikey |> get<Stats>
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

    let leaderboardUri region bracket locale apikey = createUri region locale apikey ["wow"; "leaderboard"; bracket]
    let leaderboard region bracket locale apikey = leaderboardUri region bracket locale apikey |> get<Leaderboard>
module Quest =
    type Quest = {
        id: int;
        title: string;
        reqLevel: int;
        suggestedPartyMembers: int;
        category: string;
        level: int}

    let questUri region questId locale apikey = createUri region locale apikey ["wow";"quest"; questId;]
    let quest region questId locale apikey = questUri region questId locale apikey |> get<Quest>   
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

    let realmUri region locale apikey = createUri region locale apikey ["wow";"realm"; "status";]
    let realm region locale apikey = realmUri region locale apikey |> get<Realms>
module Recipe =
    type Recipe = { 
        id: int;
        name: string;
        profession: string;
        icon: string}

    let recipeUri region recipeId locale apikey = createUri region locale apikey ["wow";"recipe"; recipeId;]
    let recipe region recipeId locale apikey = recipeUri region recipeId locale apikey |> get<Recipe>
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

    let spellUri region spellId locale apikey = createUri region locale apikey ["wow";"recipe"; spellId;]
    let spell region spellId locale apikey = spellUri region spellId locale apikey |> get<Spell>       
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
        patch:string;}
    type Zones = { zones: Zone list }

    let zonesUri region locale apikey = createUri region locale apikey ["wow";"zone/";]
    let zoneUri region zoneId locale apikey = createUri region locale apikey ["wow";"zone"; zoneId;]
    let zones region locale apikey = zonesUri region locale apikey |> get<Zones>
    let zone region zoneId locale apikey = zoneUri region zoneId locale apikey |> get<Zone>
//module DataResources =