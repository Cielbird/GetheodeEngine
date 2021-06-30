# Getheode phoneme context synatx:

## how to use symbols

symbols:
- `C` : consonant
- `V` : vowel

any context: 
```
_
```

after a consonant: 
```
S_
```

before symbol: 
```
_S
```

not after symbol: 
```
!(S)_
```

not before A, but after B: 
```
B_!(A)
```

## how to use modifiers (and matchers, and tags)

modifiers are used to specify the type of consonant or vowel.
modifiers are placed in are placed in square brackets: `[]` in the following structure:

```
(Symbol) [ (Modifier)(Matcher)(Tag) ]

```
Some examples include:
```
C[p=bilab] //Any bilabial consonant
```
multiple modifiers can be listed with commas (this acts like an `and` statement):
```
C[m=plos, v=true] //Any voiced plossive consonant
```
multiple tags can be applied to a modifier with a `|` which indicates "or"
```
C[m=plos|fric] //Any plossive or fricative consonant
```
any tag can be inverted to include everything but the tag with `!`:
```
V[h=!open] //any vowel that is not open
```

below are a list of all the modifiers and tags:
- `C` :
  - `p` : place of articulation
    - `bilab`
    - `labdent`
    - `dent`
    - `alv`
    - `postalv`
    - `palatal`
    - `velar`
    - `uvular`
    - `pharyn`
    - `glottal`
  - `m` : manner of articulation
    - `plos`
    - `fric`
    - `...`
  - `v` : voice
    - `true`
    - `false`
- `V` :
  - `h` : hieght
    - `close`
    - `...`
    - `open`
  - `f` : frontedness
    - `back`
    - `...`
    - `front`
  - `r` : rounded
    - `true`
    - `false`

matchers are usually a simple equals: `=`. But other possible modifiers are:

- `>`
- `<`
- `>=`
- `<=`

example:
```
V[h>midclose] //any vowel higher than mid-close
```