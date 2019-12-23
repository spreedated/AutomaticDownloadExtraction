Public Class SpecialChars
    Public Enum Chars
        Checkmark
        Crossmark
    End Enum
    Private Shared CharArray As String() = {"\u2713", "\u2717"}
    Public Shared Function GetChar(ByVal [Char] As Chars) As Char
        Return System.Text.RegularExpressions.Regex.Unescape(CharArray([Char]))
    End Function

End Class
