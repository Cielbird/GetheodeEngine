using System;
using GetheodeEngine;

class GetheodeConsole
{
    static void print(object s)
    {
        Console.WriteLine(s);
    }

    static void Main(string[] args)
    {
        Language english_lang = new Language("english");
        english_lang.Lects.Add(new Lect("scottish_english"));

        print(english_lang);
    }

}