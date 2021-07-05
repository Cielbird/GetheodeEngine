using System;
using System.Collections.Generic;
using GetheodeEngine;

class GetheodeConsole
{
    static void print(object s)
    {
        Console.WriteLine(s);
    }

    static void Main()
    {
        print((IPAChar)"t" + (IPAChar)"[+voi]");

        List<IPAChar> tzeer = new List<IPAChar>()
        {
            (IPAChar)"t",
            (IPAChar)"z",
            (IPAChar)"i",
            (IPAChar)"r",
        };
        new PhonologicalRule("[-syl] -> [-voi] / [-voi]_").ApplyToMorpheme(tzeer);


        Language english_lang = new Language("english");
        Lect scottish = new Lect("scottish_english");
        scottish.Phonology.AddRule("[] -> [-voi] / [-voi]_");
        english_lang.Lects.Add(scottish);
        print(english_lang);
    }
}