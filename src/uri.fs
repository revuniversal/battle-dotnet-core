module UriBuilding

open System
open System.Text
open BattleNet

type Protocol = Https | Http
type QueryParameter = { key:string; value:string }
type ResourceParameter = { key:string }
type UriPart = 
    | Protocol of Protocol
    | Resource of ResourceParameter 
    | Query of QueryParameter
type Endpoint = UriPart list

let addPart orig parameter =
    match parameter with
    | Resource (x) -> orig + "/" + x.key
    | Query (y) ->  orig + "&" + y.key + "=" + y.value
    | _ -> ""
    
let getCombinedParts uriParts predicate = 
    uriParts 
    |> List.filter predicate
    |> List.fold addPart ""

let getProtocol protocol =
    match protocol with
    | Https -> "https://"
    | Http -> "http://"

let getUri protocol region host endpoint = 
    let isResource part = match part with Resource (x) -> true | _ -> false
    let isQuery part = match part with Query (x) -> true | _ -> false
    let resource = getCombinedParts endpoint isResource 
    let query = getCombinedParts endpoint isQuery
    getProtocol protocol + getRegion region + "." + host + resource + "?" + query

let get endpoint region protocol = async {
    let host = "api.battle.net"
    let uri = getUri protocol region host endpoint
    use http = new System.Net.Http.HttpClient()
    let! json = http.GetStringAsync(uri) |> Async.AwaitTask
    return json}