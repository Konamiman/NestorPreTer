# NestorPreTer for .NET #

NestorPreTer is a MSX-BASIC preinterpreter that I developed as a MSX-DOS application in Z80 assembler back in 1999. It has been [available for download in my MSX site](http://www.konamiman.com/msx/msx-e.html#nestorpreter) for a while, and it is part of [my MSX software repository](https://github.com/Konamiman/MSX/tree/master/SRC/NPR) as well.

This project is just a wrapper around the original MSX NestorPreTer program that uses the magic of [Z80.NET](https://github.com/Konamiman/Z80dotNet) to allow executing it on any modern .NET capable machine (such as Windows or Linux/Mac OS with Mono). I did this because I got an email from a MSX developer asking me if there was a way to use NestorPreTer without having to resort to a MSX machine or emulator. Well, there wasn't... but now there is.

For the nitty gritty details about what NestorPreTer does and how to use it, please refer to [the original manual of the program](NestorPreTer/Info/npr.txt). Just run `npr.exe` as you would run `npr.com` in your MSX, using the same command line parameters. You aren't constrained to the 8.3 format for file names (all the original calls to MSX-DOS functions are patched), but note however that the length of the command line must be 127 characters or less and that all file names must be ASCII.

And remember: if you like this project **[please consider donating!](http://www.konamiman.com/msx/msx-e.html#donate)** My kids need moar shoes!