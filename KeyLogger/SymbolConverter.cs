using System;
using System.Collections.Generic;
using System.Text;

namespace KeyLogger
{
    public enum Language {USA = 1033, RU = 1049, UA = 1058};

    public static class SymbolConverter
    {

        public static string GetSymbol(string symbol, Language lang)
        {
            string res = symbol;
            if (lang == Language.RU || lang == Language.UA)
            {
                switch (res)
                {
                    case "Q": res = "Й"; break;
                    case "W": res = "Ц"; break;
                    case "E": res = "У"; break;
                    case "R": res = "К"; break;
                    case "T": res = "Е"; break;
                    case "Y": res = "Н"; break;
                    case "U": res = "Г"; break;
                    case "I": res = "Ш"; break;
                    case "O": res = "Щ"; break;
                    case "P": res = "З"; break;
                    case "[": res = "Х"; break;
                    case "]": res = "Ъ"; break;
                    case "A": res = "Ф"; break;
                    case "S": res = "Ы"; break;
                    case "D": res = "В"; break;
                    case "F": res = "А"; break;
                    case "G": res = "П"; break;
                    case "H": res = "Р"; break;
                    case "J": res = "О"; break;
                    case "K": res = "Л"; break;
                    case "L": res = "Д"; break;
                    case ";": res = "Ж"; break;
                    case "'": res = "Э"; break;
                    case "Z": res = "Я"; break;
                    case "X": res = "Ч"; break;
                    case "C": res = "С"; break;
                    case "V": res = "М"; break;
                    case "B": res = "И"; break;
                    case "N": res = "Т"; break;
                    case "M": res = "Ь"; break;
                    case ",": res = "Б"; break;
                    case ".": res = "Ю"; break;
                    case "/": res = "."; break;
                }
            }
            return res;
        }
    }
}
