module WoW

open BattleNet
open UriBuilding
open System

let getLocale locale = 
    match locale with
    | EN_US -> { Key = "locale"; Value = "en_us" }
    | EN_GB -> { Key = "locale"; Value = "en_gb" }

let createUri region locale apikey resources =
    let locale = getLocale locale 
    let key = { Key = "apikey"; Value = apikey }
    {   
        Scheme = Https;
        Subdomains = [getRegion region];
        Host = "api.battle.net"
        Resources = resources;
        File = "";
        Extension = "";
        Query = [locale;key];
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
    type Npc = { Id:int; Name:string; UrlSlug:string; }
    type Location = { Id:int; Name:string; }
    type Boss = {
        Id:int;
        Name:string;
        UrlSlug:string;
        Description:string;
        ZoneId:int;
        AvailableInNormalMode:bool;
        AvailableInHeroicMode:bool;
        Health:int;
        HeroicHealth:int;
        Level:int;
        HeroicLevel:int;
        JournalId:int;
        Npcs: Npc list}
    type BossData = { Bosses: Boss list}

    let bossUri region id locale apikey = createUri region locale apikey ["wow"; "boss"; id]
    let bossesUri region locale apikey = createUri region locale apikey ["wow"; "boss/"]
    let boss region bossId locale apikey = bossUri region bossId locale apikey |> get<Boss>
    let bosses region locale apikey = bossesUri region locale apikey |> get<BossData>
module ChallengeMode =
    type Realm = { Name:string;
        Slug:string;
        Battlegroup:string;
        Locale:string;
        Timezone:string;
        ConnectedRealms:string list;}// _

    type Criteria = { Time:int; 
        hours:int;
        minutes:int;
        seconds:int;
        milliseconds:int;
        isPositive:bool;}

    type Map = { Id:int;
        name:string;
        slug:string;
        hasChallengeMode:bool;
        bronzeCriteria:Criteria;
        silverCriteria:Criteria;
        goldCriteria:Criteria;}

    type Time = { Time:int;
        hours:int;
        minutes:int;
        seconds:int;
        milliseconds:int;
        isPositive:bool;}

    type Spec = { Name:string;
        role:string;
        backgroundImage:string;
        icon:string;
        description:string;
        order:int;}

    type Character = { Name:string;
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
        
    type Member = {Character:Character; Spec:Spec; }
    type Group = {Ranking:int;
        time:Time;
        date:string;
        medal:string;
        faction:string;
        isRecurring:bool;
        members:Member list;}

    type RealmChallenge = { Realm:Realm; Map:Map; Groups:Group list; }
    type RegionChallenge = { Map:Map; Groups:Group list; }
    type ChallengeRealmLeaderboard = { Challenge: RealmChallenge list; }
    type ChallengeRegionLeaderboard = {Challenge: RegionChallenge list; }

    let realmLeaderboardUri region realm locale apikey = createUri region locale apikey ["wow"; "challenge"; realm]
    let regionLeaderboardUri region locale apikey = createUri region locale apikey ["wow"; "challenge"; "region"]
    let realmLeaderboard region realm locale apikey = get<ChallengeRealmLeaderboard>(realmLeaderboardUri region realm locale apikey)
    let regionLeaderboard region realm locale apikey = get<ChallengeRegionLeaderboard>(regionLeaderboardUri region locale apikey) 
module CharacterProfile =
    type Achievements = {
        AchievementsCompleted: int list;
        AchievementsCompletedTimestamp: Object list;
        Criteria: int list;
        CriteriaQuantity: Object list;
        CriteriaTimestamp: Object list;
        CriteriaCreated: Object list;
    }
    type CharacterProfile = {
        LastModified: int64;
        Name: string;
        Realm: string;
        Battlegroup: string;
        ``Class``: int;
        Race: int;
        Gender: int;
        Level: int;
        AchievementPoints: int;
        Thumbnail: string;
        CalcClass: string;
        Faction: int;
        Achievements: Achievements option;
        TotalHonorableKills: int;        
    }
    let characterProfileUri region name realm locale apiKey = 
        let uri = createUri region locale apiKey ["wow"; "character"; realm; name;]
        {uri with Query = [{Key = "fields"; Value = "achievements"}; 
                            getLocale locale;
                            { Key = "apikey"; Value = apiKey };]}                            
    //let characterProfile region name realm locale apiKey = characterProfileUri region name realm locale apiKey |> get<CharacterProfile>
//module GuildProfile =
module Item =
    type BonusStat = { Stat:int; Amount:int}
    type Damage = { Min:int; Max:int; ExactMin:double; ExactMax:double;}
    type WeaponInfo = { Damage: Damage; WeaponSpeed:double; Dps:double; }
    type ItemSource = { SourceId:int; SourceType:string; }
    type BonusSummary = { 
        DefaultBonusLists:Object list; 
        ChangeBonusLists: Object list;
        BonusChances:Object list}
    type Item = {
        Id:int;
        DisenchantingSkillRank:int;
        Description:string;
        Name:string;
        Icon:string;
        Stackable:int;
        ItemBind:int;
        BonusStats: BonusStat list;
        ItemSpells: Object list;
        BuyPrice:int;
        ItemClass:int;
        ItemSubClass:int;
        ContainerSlots:int;
        WeaponInfo:WeaponInfo;
        InventoryType:int;
        Equippable:bool;
        ItemLevel:int;
        MaxCount:int;
        MaxDurability:int;
        MinFactionId:int;
        MinReputation:int;
        Quality:int;
        SellPrice:int;
        RequiredSkillLevel:int;
        RequiredLevel:int;
        RequiredSkillRank:int;
        ItemSource:ItemSource;
        BaseArmor:int;
        HasSockets:bool;
        IsAuctionable:bool;
        Armor:int;
        DisplayInfold:int;
        NameDescription:string;
        NameDescriptionColor:int;
        Upgradable:bool;
        HeroicTooltip:bool;
        Context:string;
        BonusLists: Object list;
        AvailableContexts:string list;
        BonusSummary:BonusSummary;
        ArtifactId:int}
    type SetBonus = { Description:string; Threshold:int; }
    type ItemSet = { Id:int; Name:string; SetBonuses:SetBonus list; Items: int list;}

    let itemUri region itemId locale apikey = createUri region locale apikey ["wow";"item"; itemId]
    let itemSetUri region setId locale apikey = createUri region locale apikey ["wow";"item"; "set"; setId]
    let item region itemId locale apikey = itemUri region itemId locale apikey |> get<Item>
    let itemSet region setId locale apikey = itemSetUri region setId locale apikey |> get<ItemSet>
module Mount =
    type Mount = { 
        Name:string;
        SpellId:int;
        CreatureId:int;
        ItemId:int;
        QualityId:int;
        Icon:string;
        IsGround:bool;
        IsFlying:bool;
        IsAquatic:bool;
        IsJumping:bool}
    type Mounts = { Mounts: Mount list}

    let mountUri region locale apikey = createUri region locale apikey ["wow";"mount/"]
    let mounts region locale apikey = mountUri region locale apikey |> get<Mounts>
module Pet =
    type Stats = {
        SpeciesId: int;
        BreedId: int;
        PetQualityId: int;
        Level: int;
        Health: int;
        Power: int;
        Speed: int;}
    type Pet = {
        CanBattle: bool;
        CreatureId: int;
        Name: string;
        Family: string;
        Icon: string;
        QualityId: int;
        Stats: Stats;
        StrongAgainst: string list;
        TypeId: int;
        WeakAgainst: string list;}
    type Pets = { Pets: Pet list; }
    type PetAbility = {
        Id: int;
        Name: string;
        Icon: string;
        Cooldown: int;
        Rounds: int;
        PetTypeId: int;
        IsPassive: int;
        HideHints: int;}
    type SpeciesAbility = {
        Slot: int;
        Order: int;
        RequiredLevel: int;
        Id: int;
        Name: string;
        Icon: string;
        Cooldown: int;
        Rounds: int;
        PetTypeId: int;
        IsPassive: bool;
        HideHints: bool;}
    type Species = {
        SpeciesId: int;
        PetTypeId: int;
        CreatureId: int;
        Name: string;
        CanBattle: string;
        Icon: string;
        Description: string;
        Source: string;
        Abilities: SpeciesAbility list;}

    let petsUri region locale apikey = createUri region locale apikey ["wow"; "pet/";]
    let petAbilityUri region abilityId locale apikey = createUri region locale apikey ["wow"; "pet"; "ability"; abilityId;]
    let petSpeciesUri region speciesId locale apikey = createUri region locale apikey ["wow"; "pet"; "species"; speciesId;]
    let petStatsUri region speciesId level breedId qualityId locale apikey = 
        let uri = createUri region locale apikey ["wow"; "pet"; "stats"; speciesId;]
        { uri with Query = [{Key = "level"; Value = level};
                                {Key = "breedId"; Value = breedId};
                                {Key = "qualityId"; Value = qualityId};
                                getLocale locale;
                                { Key = "apikey"; Value = apikey };]} 

    let pets region locale apikey = petsUri region locale apikey |> get<Pets>
    let petAbility region abilityId locale apikey = petAbilityUri region abilityId locale apikey |> get<PetAbility>
    let petSpecies region speciesId locale apikey = petSpeciesUri region speciesId locale apikey |> get<Species>
    let petStats region speciesId level breedId qualityId locale apikey = petStatsUri region speciesId level breedId qualityId locale apikey |> get<Stats>
module PVP =
    type Row = {Ranking: int; 
        Rating: int; 
        Name: string; 
        RealmId:int;
        RealmName:string; 
        RealmSlug:string; 
        RaceId:int;
        ClassId:int;
        SpecId:int;
        FactionId:int;
        GenderId:int;
        SeasonWins:int;
        SeasonLosses:int;
        WeeklyWins:int;
        WeeklyLosses:int}    
    type Leaderboard = { Rows : Row list }

    let leaderboardUri region bracket locale apikey = createUri region locale apikey ["wow"; "leaderboard"; bracket]
    let leaderboard region bracket locale apikey = leaderboardUri region bracket locale apikey |> get<Leaderboard>
module Quest =
    type Quest = {
        Id: int;
        Title: string;
        ReqLevel: int;
        SuggestedPartyMembers: int;
        Category: string;
        Level: int}

    let questUri region questId locale apikey = createUri region locale apikey ["wow";"quest"; questId;]
    let quest region questId locale apikey = questUri region questId locale apikey |> get<Quest>   
module RealmStatus =
    type Realm = {
        ``Type``: string;
        Population: string;
        Queue: bool;
        Status: bool;
        Name: string;
        Slug: string;
        Battlegroup: string
        Locale: string;
        Timezome: string;
        ConnectedRealms: string list}
    type Realms = {Realms:Realm list}

    let realmUri region locale apikey = createUri region locale apikey ["wow";"realm"; "status";]
    let realm region locale apikey = realmUri region locale apikey |> get<Realms>
module Recipe =
    type Recipe = { 
        Id: int;
        Name: string;
        Profession: string;
        Icon: string}

    let recipeUri region recipeId locale apikey = createUri region locale apikey ["wow";"recipe"; recipeId;]
    let recipe region recipeId locale apikey = recipeUri region recipeId locale apikey |> get<Recipe>
module Spell =
    type Spell = {
        Id: int;
        Name: string;
        Icon: string;
        Description: string;
        Range: string;
        PowerCost: string;
        CastTime: string;
        Cooldown: string}

    let spellUri region spellId locale apikey = createUri region locale apikey ["wow";"recipe"; spellId;]
    let spell region spellId locale apikey = spellUri region spellId locale apikey |> get<Spell>       
module Zone =
    open Boss
    type Zone = {
        Id:int;
        Name:string;
        UrlSlug:string;
        Description:string;
        Location:Location;
        ExpansionId:int;
        NumPlayers:int;
        IsDungeon:bool;
        IsRaid:bool;
        AdvisedMinLevel:int;
        AdvisedMaxLevel:int;
        AdvisedHeroicMinLevel:int;
        AdvisedHeroicMaxLevel:int;
        AvailableModes:Object list;
        LfgNormalMinGearLevel:int;
        LfgHeroicMinGearLevel:int;
        Floors:int;
        Bosses: Boss list;
        Patch:string;}
    type Zones = { Zones: Zone list }

    let zonesUri region locale apikey = createUri region locale apikey ["wow";"zone/";]
    let zoneUri region zoneId locale apikey = createUri region locale apikey ["wow";"zone"; zoneId;]
    let zones region locale apikey = zonesUri region locale apikey |> get<Zones>
    let zone region zoneId locale apikey = zoneUri region zoneId locale apikey |> get<Zone>
//module DataResources =