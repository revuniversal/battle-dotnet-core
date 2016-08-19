module UrlTests 

open Xunit
open BattleNet
open WoW.PVP
open WoW.Achievement
open WoW.Auction
open WoW.Boss
open WoW.ChallengeMode
open WoW.Mount
open WoW.Recipe
open WoW.Quest
open WoW.Realm
open WoW.Spell
open UriBuilding
open Newtonsoft.Json

let apikey = "testKey"

[<Fact>]    
let ``Leaderboard endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/leaderboard/3v3?&locale=en_us&apikey=testKey"
    let endpoint = leaderboardUri US "3v3" EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Achievement endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/achievement/2144?&locale=en_us&apikey=testKey"
    let endpoint = achievementUri US "2144" EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Auction endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/auction/data/zuljin?&locale=en_us&apikey=testKey"
    let endpoint = auctionUri US "zuljin" EN_US apikey
    let uri = getUrl  endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Boss endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/boss/24723?&locale=en_us&apikey=testKey"
    let endpoint = bossUri US "24723" EN_US apikey
    let uri = getUrl  endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Bosses endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/boss/?&locale=en_us&apikey=testKey"
    let endpoint = bossesUri US EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Challenge mode realm leaderboard endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/challenge/zuljin?&locale=en_us&apikey=testKey"
    let endpoint =  realmLeaderboardUri US "zuljin" EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Challenge mode region leaderboard endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/challenge/region?&locale=en_us&apikey=testKey"
    let endpoint =  regionLeaderboardUri US EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Mount endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/mount/?&locale=en_us&apikey=testKey"
    let endpoint =  mountUri US EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Recipe endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/recipe/33994?&locale=en_us&apikey=testKey"
    let endpoint =  recipeUri US "33994" EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Quest endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/quest/13146?&locale=en_us&apikey=testKey"
    let endpoint =  questUri US "13146" EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Realm endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/realm/status?&locale=en_us&apikey=testKey"
    let endpoint =  realmUri US EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Zone endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/zone/4131?&locale=en_us&apikey=testKey"
    let endpoint =  zoneUri US "4131" EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Zones endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/zone/?&locale=en_us&apikey=testKey"
    let endpoint =  zonesUri US EN_US apikey
    let uri = getUrl endpoint
    Assert.Equal(sample, uri)