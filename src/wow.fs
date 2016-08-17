module WoW

open BattleNet
open UriBuilding
open Newtonsoft.Json

module PVP =
    type Row = {ranking: int; 
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
    
    type Leaderboard = { rows : Row list }
    
    // compose mode
    let leaderboardEndpoint bracket (locale:Locale) apikey = 
        let game = Resource { key = "wow" }
        let api = Resource { key = "leaderboard" }
        let bracket = Resource { key = bracket }
        let locale = match locale with
                        | EN_US -> Query { key = "locale"; value = "en_us" }
                        | EN_GB -> Query { key = "locale"; value = "en_gb" }

        let key = Query { key = "apikey"; value = apikey }
        [game;api;bracket;locale;key]

    // ez mode
    let leaderboard region bracket locale apikey = async {
        let endpoint = leaderboardEndpoint bracket locale apikey
        let! json = get endpoint region Protocol.Https
        let data = JsonConvert.DeserializeObject<Leaderboard>(json)
        return data
        }
       
    


 
   
        