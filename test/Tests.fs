module Test
open BattleNet
open Xunit
open WoW.PVP
open WoW.Achievement
open WoW.Auction
open WoW.Boss
open UriBuilding
open Newtonsoft.Json

let apikey = "vftjkwdyvev3p4m9jrnfxgsdu2dz68yd"

[<Fact>]    
let ``Leaderboard endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/leaderboard/3v3?&locale=en_us&apikey=vftjkwdyvev3p4m9jrnfxgsdu2dz68yd"
    let endpoint = leaderboardEndpoint "3v3" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(uri, sample)

[<Fact>]
let ``Leaderboard request deserializes``() =
    let ladder =  leaderboard US "3v3" EN_US apikey |> Async.RunSynchronously
    Assert.NotEmpty(ladder.rows)
    Assert.True(ladder.rows.Length > 4000)

[<Fact>]    
let ``Achievement endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/achievement/2144?&locale=en_us&apikey=vftjkwdyvev3p4m9jrnfxgsdu2dz68yd"
    let endpoint = achievementEndpoint "2144" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(uri, sample)

[<Fact>]
let ``Achievement request deserializes``() =
    let achievement =  achievement US "2144" EN_US apikey |> Async.RunSynchronously
    Assert.True(achievement.title = "What a Long, Strange Trip It's Been")

[<Fact>]    
let ``Auction endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/auction/data/zuljin?&locale=en_us&apikey=vftjkwdyvev3p4m9jrnfxgsdu2dz68yd"
    let endpoint = auctionEndpoint "zuljin" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(uri, sample)

[<Fact>]
let ``Auction request deserializes``() =
    let auctionData =  auctionFile US "zuljin" EN_US apikey |> Async.RunSynchronously
    Assert.NotEmpty(auctionData.files)

[<Fact>]
let ``Auction data is old``() =
    let files = auctionFile US "zuljin" EN_US apikey |> Async.RunSynchronously
    let file = files.files.Head
    let data1 = auction file Default |> Async.RunSynchronously
    let data2 = auction file (Specified file.lastModified) |> Async.RunSynchronously
    Assert.True((data2 = None) = true)

[<Fact>]
let ``Auction data is new``() =
    let files = auctionFile US "zuljin" EN_US apikey |> Async.RunSynchronously
    let file = files.files.Head
    let data = auction file Default |> Async.RunSynchronously

    match data with
    | Some (x) -> 
        Assert.True(true)
    | None -> 
        Assert.True(false)

[<Fact>]    
let ``Boss endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/boss/24723?&locale=en_us&apikey=vftjkwdyvev3p4m9jrnfxgsdu2dz68yd"
    let endpoint = bossEndpoint "24723" EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(uri, sample)

[<Fact>]
let ``Boss request deserializes``() =
    let boss =  boss US "24723" EN_US apikey |> Async.RunSynchronously
    Assert.True(boss.name = "Selin Fireheart")

[<Fact>]    
let ``Bosses endpoint url is built``() = 
    let sample = @"https://us.api.battle.net/wow/boss/?&locale=en_us&apikey=vftjkwdyvev3p4m9jrnfxgsdu2dz68yd"
    let endpoint = bossesEndpoint  EN_US apikey
    let uri = getUri Https US "api.battle.net" endpoint
    Assert.Equal(uri, sample)

[<Fact>]
let ``Bosses request deserializes``() =
    let bossData =  bosses US  EN_US apikey |> Async.RunSynchronously
    Assert.NotEmpty(bossData.bosses)
    

