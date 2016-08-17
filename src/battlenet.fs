module BattleNet

type Game = WOW | D3 | SC2
type Locale = EN_US | EN_GB
type Region = US | EU | KR | TW | CN

let getRegion region = 
     match region with
     | US -> "us"
     | EU -> "eu"
     | KR -> "kr"
     | TW -> "tw"
     | CN -> "cn"