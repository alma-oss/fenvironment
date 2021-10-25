namespace Lmc.Environment

[<AutoOpen>]
module internal Regex =
    open System.Text.RegularExpressions

    // http://www.fssnip.net/29/title/Regular-expression-active-pattern
    let internal (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ])
        else None
