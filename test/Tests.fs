module Test
open BattleNet
open Xunit
open WoW.PVP
open WoW.Achievement
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
    

