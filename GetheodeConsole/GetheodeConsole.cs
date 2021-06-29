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
        Phoneme a = new Phoneme('a');
        a.AddRealization("none", 'a');
        a.AddRealization("none", 'e');
        english_lang.Lects[0].Phonology.AddPhoneme(a);
        Phoneme b = new Phoneme('b');
        b.AddRealization("none", 'b');
        b.AddRealization("none", 'p');
        english_lang.Lects[0].Phonology.AddPhoneme(b);

        print(english_lang);
    }
}