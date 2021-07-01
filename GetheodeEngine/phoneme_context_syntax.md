# Getheode phoneme context synatx:

## how to write contexts

any context: 
```
()
```

after or before the sound `/a/`: 
```
a()
()a
```
after either `/a/` or `/e/`, like an `or` statement:
```
[ae]()
```
word bounderies are marked with the charcter `#`

## sound tags

instead of writing long groups of similar ipa segments in brackets, use generic groupings:

after any vowel:
```
[+V]()
```
after any non-nasal consonant:
```
[+C-N]()
```

## sylable tags

sylable tags indicate any sylable related context (like stressed sylables, word final sylables, etc.) that belongs neither in the `before` or the `after` sections. they are put in the parentheses.

in any stressed syllable:
```
(+s)
```
in any unstressed, word-final sylable:
```
(-s+f)
```