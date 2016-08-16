namespace BattleDotCore 

open System
open System.Text
module BattleNet = 
    type Game = WOW | D3 | SC2
    type Locale = EN_US | EN_GB
    type Region = US | EU | KR | TW | CN

open BattleNet
    
module UriBuilding =
    type Protocol = Https | Http
    type resourceParameter = { key: string }
    type queryParameter = { key:string; value:string }
    type Parameter = 
        | Resource of resourceParameter 
        | Query of queryParameter

    type Endpoint = List<Parameter>

    let addParameter (orig:string) (parameter:Parameter) =
        match parameter with
        | Resource (x) -> orig + "/" + x.key
        | Query (y) ->  orig + "&" + y.key + "=" + y.value

    let getUri endpoint region host protocol = 
        let region = match region with
                        | US -> "us"
                        | EU -> "eu"
                        | KR -> "kr"
                        | TW -> "tw"
                        | CN -> "cn"

        let protocol = match protocol with
                        | Https -> "https://"
                        | Http -> "http://"

        let uri = protocol + region + "." + host
        let resourceParameters = endpoint |> List.filter (fun param -> match param with| Resource (x) -> true | Query (y) ->  false )
        let queryParameters = endpoint |> List.filter (fun param -> match param with | Resource (x) -> false |Query (y) -> true)

        let resource = resourceParameters |> List.fold (fun param -> addParameter param ) ""
        let query = queryParameters |> List.fold (fun param -> addParameter param ) ""

        uri + resource + "?" + query

    let get endpoint region protocol = async {
        let host = "api.battle.net"
        let uri = getUri endpoint region host protocol
        use http = new System.Net.Http.HttpClient()
        let! json = http.GetStringAsync(uri) |> Async.AwaitTask
        return json
        }



namespace WorldOfWarcraft    
module PVP = 
    type Row = { ranking: int; 
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
    
    type Leaderboard = List<Row>