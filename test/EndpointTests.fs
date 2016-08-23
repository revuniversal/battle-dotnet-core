module EndpointTests

open BattleNet
open Xunit
open WoW.PVP
open WoW.Achievement
open WoW.Auction
open WoW.Boss
open WoW.ChallengeMode

open WoW.CharacterProfile
let apikey = "vftjkwdyvev3p4m9jrnfxgsdu2dz68yd"

[<Fact>]
let ``Leaderboard request deserializes``() =
    let ladder =  leaderboard US "3v3" EN_US apikey |> Async.RunSynchronously
    Assert.NotEmpty(ladder.Rows)
    Assert.True(ladder.Rows.Length > 4000)

[<Fact>]
let ``Achievement request deserializes``() =
    let achievement =  achievement US "2144" EN_US apikey |> Async.RunSynchronously
    Assert.True(achievement.title = "What a Long, Strange Trip It's Been")

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
let ``Boss request deserializes``() =
    let boss =  boss US "24723" EN_US apikey |> Async.RunSynchronously
    Assert.True(boss.Name = "Selin Fireheart")

[<Fact>]
let ``Bosses request deserializes``() =
    let bossData =  bosses US  EN_US apikey |> Async.RunSynchronously
    Assert.NotEmpty(bossData.Bosses)

[<Fact>]
let ``Challenge mode realm leaderboard request deserializes``() =
    let leaderboard =  realmLeaderboard US "zuljin" EN_US apikey |> Async.RunSynchronously
    Assert.NotEmpty(leaderboard.Challenge)

[<Fact>]
let ``Challenge mode region leaderboard request deserializes``() =
    let leaderboard =  regionLeaderboard US "zuljin" EN_US apikey |> Async.RunSynchronously
    Assert.NotEmpty(leaderboard.Challenge)
