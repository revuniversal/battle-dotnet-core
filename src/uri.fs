module UriBuilding

    open System
    open System.Text
    open BattleNet
    
    type Protocol = Https | Http
    type Subdomain = Subdomain of string
    type QueryParameter = { key:string; value:string }
    type ResourceParameter = { key: string }
    
    type UriPart = 
        | Protocol of Protocol
        | Subdomain of Subdomain
        | Resource of ResourceParameter 
        | Query of QueryParameter

    type Uri = Protocol * Subdomain * ResourceParameter list * QueryParameter list
    type Endpoint = UriPart list

    let addPart (orig:string) (parameter:UriPart) =
        match parameter with
        | Resource (x) -> orig + "/" + x.key
        | Query (y) ->  orig + "&" + y.key + "=" + y.value
        | _ -> ""

    let getUri protocol region host endpoint = 
        let protocol = match protocol with
                        | Https -> "https://"
                        | Http -> "http://"

        let region = match region with
                        | US -> "us"
                        | EU -> "eu"
                        | KR -> "kr"
                        | TW -> "tw"
                        | CN -> "cn"

        let isResource part = 
            match part with
            | Resource (x) -> true
            | _ -> false

        let isQuery part = 
            match part with
            | Resource (x) -> true
            | _ -> false

        let resourceParameters = endpoint |> List.filter isResource
        let queryParameters = endpoint |> List.filter isQuery

        let resource = resourceParameters |> List.fold (fun param -> addPart param ) ""
        let query = queryParameters |> List.fold (fun param -> addPart param ) ""

        let uri = protocol + region + "." + host + resource + "?" + query
        uri

    let get endpoint region protocol = async {
        let host = "api.battle.net"
        let uri = getUri protocol region host endpoint
        use http = new System.Net.Http.HttpClient()
        let! json = http.GetStringAsync(uri) |> Async.AwaitTask
        return json
        }

