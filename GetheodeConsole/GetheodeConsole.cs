using System;
using GetheodeEngine;

class GetheodeConsole
{
    static void print(object s)
    {
        Console.WriteLine(s);
    }

    static void Main()
    {
        Language english_lang = new Language("english");
        Lect scottish = new Lect("scottish_english");
        scottish.Phonology.AddRule("[] -> [-voi] / [-voi]_");
        english_lang.Lects.Add(scottish);
        print(english_lang);
    }
}