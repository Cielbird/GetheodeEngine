# GetheodeEngine

A C# library that provides tools for constructing fictional languages. The end 
goal is an engine that automates the process of 
conlanging (constructing ficitonal languages) using generative grammar. It will 
aim to immitate the patterns of real languages as well as possible.

This is an early version of a project I have just begun developing.

*[Ġeþēode](https://en.wiktionary.org/wiki/geþeode)* `/jeˈθe͜oː.de/` is Old 
English for "language"

*Thanks to:*

Bruce Hayes and Eric Biggs for the ipa segment features csv data:
https://linguistics.ucla.edu/people/hayes/IP/#features

## File syntax

Comments can be added with `//` in any file.

### Phonemes

[What is a phoneme?](https://en.wikipedia.org/wiki/Phoneme)

Rules can be added to a Lect by using `Lect.ImportPhonemes(string filePath)`.

Each phoneme is written as:
```
[romanization] = [ipa]
```

Romanizations make it easier to read and write morphemes in the language you are
 creating. The romanization can be any UTF-8 string of characters.

Example:
```
a = æ
e = e
ee = iː
```

In cases where you wish the romanization to be the same as the IPA, you can
 leave out the romanization:
```
a = æ
e
ee = iː
```

### Rules
In the engine, "rules" refer to 
[phonological rules](https://en.wikipedia.org/wiki/Phonological_rule).

Rules can be added to a Lect by using `Lect.ImportRules(string filePath)`.

The synax of a rules file is as follows. Each rule is written on it's own line:

```
[input] -> [output] / [pre-context]_[post-context] 
```

To represent a word border in the context, use: `#`

Brackets and commas `{ , }` are used to represent **or**. *This doesn't work *
*in the output.* 

The following means "i **or** e becomes j, before an a **or** an o": 
```
{i, e} -> j / _{a, o}
```

`[input]`, `[output]`, `[pre-context]` and `[post-context]` are writen either 
using the IPA, or bracketted features: `[+voi ...]`

### Phonotactics

Phonotactics define how phonemes can or can't be arranged in morphemes. In the 
engine, phonotactics are represented by a network. A morpheme is built by 
traversing the network, starting at the `MORPH` node. Due to this, the `MORPH` 
node is required.

Phonotactics can be added to a Lect by using 
`Lect.ImportPhonotactics(string filePath)`.

Nodes are defined as such:
```
[keyword] = ...
```

The bar `|` can be used in right side of a definition to represent "or"

Here is an example of a possible phonotactics file (with only definitions):
```
MORPH = SYL SYL SYL
SYL = C V C

C = p|b|t|d|k|g
V = a|e|i|o|u
```

Some morphemes generated with this files (using `Lect.GenerateMorpheme()`) are:
```
pekbutpak
tudbidkop
gipgakpag
``` 

### Morphemes

[What is a morpheme?](https://en.wikipedia.org/wiki/Morpheme) 

The morpheme file is simple compared to the others. Morphemes to load are 
written on seperate lines, using the romanization of the phomenes defined in 
the phonemes file.

Morphemes can be added to a Lect by using 
`Lect.ImportMorphemes(string filePath)`.


```
// Nouns
gato
perro
perla

// Adjectives
con
sobre
sin
```