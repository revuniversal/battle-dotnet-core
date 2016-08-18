module UriBuilding

open System
open System.Text
open BattleNet

type Protocol = Https | Http
type QueryParameter = { key:string; value:string }
type ResourceParameter = { key:string }
type UriPart = 
    | Protocol of Protocol
    | Subdomain of string
    | Host of string
    | Resource of ResourceParameter 
    | Query of QueryParameter

type Endpoint = UriPart list

let addPart orig parameter =
    match parameter with
    | Protocol (p) -> 
        match p with
        | Https -> "https://" + orig
        | Http -> "http://" + orig
    | Subdomain (s) -> orig + s + "."
    | Host (h) -> orig + h
    | Resource (x) -> orig + "/" + x.key
    | Query (y) ->  orig + "&" + y.key + "=" + y.value
    
let getCombinedParts uriParts predicate = 
    uriParts 
    |> List.filter predicate
    |> List.fold addPart ""



let getUri endpoint = 
    let isProtocol part = match part with Protocol (x) -> true | _ -> false
    let isSubdomain part = match part with Subdomain (x) -> true | _ -> false
    let isHost part = match part with Host (x) -> true | _ -> false
    let isResource part = match part with Resource (x) -> true | _ -> false
    let isQuery part = match part with Query (x) -> true | _ -> false

    let protocol = getCombinedParts endpoint isProtocol
    let subdomain = getCombinedParts endpoint isSubdomain
    let host = getCombinedParts endpoint isHost
    let resource = getCombinedParts endpoint isResource 
    let query = getCombinedParts endpoint isQuery
    protocol + subdomain + host + resource + "?" + query

let get endpoint = async {
    let uri = getUri endpoint
    use http = new System.Net.Http.HttpClient()
    let! json = http.GetStringAsync(uri) |> Async.AwaitTask
    return json}