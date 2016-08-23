module UriBuilding

open Newtonsoft.Json
open Newtonsoft.Json.Converters
type Scheme = Https | Http
type Subdomain = string
type Host = string
type Query = { Key:string; Value:string }
type Resource = string
type File = string
type Extension = string

type Uri = {
    Scheme:Scheme;
    Subdomains:Subdomain list;
    Host: Host;
    Resources: Resource list;
    File: File;
    Extension: Extension;
    Query: Query list;}

let buildUri uri =  
    let scheme = 
        match uri.Scheme with
        | Http -> "http://"
        | Https -> "https://"

    let subdomain = uri.Subdomains |> List.fold (fun orig s -> orig + s.ToString() + ".") ""
    let resource = uri.Resources |> List.fold (fun orig r -> orig + "/" + r.ToString()) ""
    let queryString = 
        uri.Query 
        |> List.map (fun q -> "&" + q.Key + "=" + q.Value)
        |> List.fold (fun orig q -> orig + q ) ""

    let query = 
        match queryString with 
        | "" -> ""
        | _ -> "?" + queryString

    let fileName = uri.File + "." + uri.Extension
    let file =
        match fileName with
        | "." -> ""
        | _ -> "/" + fileName

    scheme + subdomain + uri.Host + resource + file + query

let httpget (url:string) = async {
    use http = new System.Net.Http.HttpClient()
    let! json = http.GetStringAsync(url) |> Async.AwaitTask
    return json}

let deserialize<'T> (json:Async<string>) = async{
    let! unwrappedJson = json
    return JsonConvert.DeserializeObject<'T>(unwrappedJson)}

let get<'T> = buildUri >> httpget >> deserialize<'T>
