module UriBuilding

open Newtonsoft.Json

type Scheme = Https | Http
type Subdomain = string
type Host = string
type Query = { key:string; value:string }
type Resource = string
type File = string
type Extension = string

type Uri = {
    scheme:Scheme;
    subdomains:Subdomain list;
    host: Host;
    resources: Resource list;
    file: File;
    extension: Extension;
    query: Query list;}

let buildUri uri =  
    let scheme = 
        match uri.scheme with
        | Http -> "http://"
        | Https -> "https://"

    let subdomain = uri.subdomains |> List.fold (fun orig s -> orig + s.ToString() + ".") ""
    let resource = uri.resources |> List.fold (fun orig r -> orig + "/" + r.ToString()) ""
    let queryString = 
        uri.query 
        |> List.map (fun q -> "&" + q.key + "=" + q.value)
        |> List.fold (fun orig q -> orig + q ) ""

    let query = 
        match queryString with 
        | "" -> ""
        | _ -> "?" + queryString

    let fileName = uri.file + "." + uri.extension
    let file =
        match fileName with
        | "." -> ""
        | _ -> "/" + fileName

    scheme + subdomain + uri.host + resource + file + query

let httpget (url:string) = async {
    use http = new System.Net.Http.HttpClient()
    let! json = http.GetStringAsync(url) |> Async.AwaitTask
    return json}

let deserialize<'T> (json:Async<string>) = async{
    let! unwrappedJson = json
    return JsonConvert.DeserializeObject<'T>(unwrappedJson)}

let get<'T> = buildUri >> httpget >> deserialize<'T>
