module UrlTests 

open Xunit
open BattleNet
open WoW.PVP
open WoW.Achievement
open WoW.Auction
open WoW.Boss
open WoW.ChallengeMode
open UriBuilding
open Newtonsoft.Json

let apikey = "testKey"

[<Fact>]    
let ``Leaderboard endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/leaderboard/3v3?&locale=en_us&apikey=testKey"
    let endpoint = leaderboardEndpoint "3v3" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Achievement endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/achievement/2144?&locale=en_us&apikey=testKey"
    let endpoint = achievementEndpoint "2144" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Auction endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/auction/data/zuljin?&locale=en_us&apikey=testKey"
    let endpoint = auctionEndpoint "zuljin" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Boss endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/boss/24723?&locale=en_us&apikey=testKey"
    let endpoint = bossEndpoint "24723" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Bosses endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/boss/?&locale=en_us&apikey=testKey"
    let endpoint = bossesEndpoint EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Challenge mode realm leaderboard endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/challenge/zuljin?&locale=en_us&apikey=testKey"
    let endpoint =  realmLeaderboardEndpoint "zuljin" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(sample, uri)

[<Fact>]    
let ``Challenge mode region leaderboard endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/challenge/region?&locale=en_us&apikey=testKey"
    let endpoint =  regionLeaderboardEndpoint EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(sample, uri)

    
