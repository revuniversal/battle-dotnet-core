module WoW
    module PVP =

        open BattleNet
        open UriBuilding

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
        
        type Leaderboard = Row list 
        
        let endpoint bracket (locale:Locale) apikey = 
            let game = Resource { key = Game.WOW.ToString().ToLower() }
            let api = Resource { key = "leaderboard" }
            let bracket = Resource { key = bracket }
            let locale = Query { key = "locale"; value = locale.ToString() }
            let key = Query { key = "apikey"; value = apikey }
            [game;api;bracket;locale;key]
    