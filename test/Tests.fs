module Test
open BattleNet
open Xunit
open WoW.PVP

[<Fact>]    
let test = 
    let number1 = 1
    let number2 = 1
    Assert.Equal(number1, number2)