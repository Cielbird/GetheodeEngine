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
        List<IPAChar> tzeer = new List<IPAChar>()
        {
            (IPAChar)"t",
            (IPAChar)"z",
            (IPAChar)"i",
            (IPAChar)"r",
        };


        Language english_lang = new Language("english");
        Lect scottish = new Lect("scottish_english");
        scottish.Phonology.AddRule("[-syl] -> [-voi] / [-voi]_");

        scottish.Phonology.AddPhoneme((IPAChar)"t");
        scottish.Phonology.AddPhoneme((IPAChar)"z");
        scottish.Phonology.AddPhoneme((IPAChar)"i", "ee");
        scottish.Phonology.AddPhoneme((IPAChar)"r");
        scottish.Phonology.AddPhoneme((IPAChar)"s");

        scottish.AddMorpheme("tzeer");

        List<IPAChar> tzirSurfRep = scottish.GetSurfaceRepresentation(scottish.Lexicon.GetMorphemes()[0]);

        english_lang.Lects.Add(scottish);
        print(english_lang);
    }
}