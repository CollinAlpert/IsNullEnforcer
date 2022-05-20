# IsNullEnforcer
This Roslyn Analyzer was inspired by [this tweet](https://twitter.com/XavXander/status/1527478770063507462).\
It searches for conditions in your code which perform null checks using the "==" or the "!=" operator and forces you to use the "is" or "is not" patterns instead. Here's an example:

```c#
using System;

string? s = null;
if (s == null) {
    Console.WriteLine("Is null!")
}
```
The condition would be highlighted as an error and a code fix would be offered, changing the code to ``s is null``.


Disclaimer: I am not saying this is a good idea and/or should be included in every code base. This is just a demonstration to show what is possible.