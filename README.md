# Emu8080

C# Intel 8080 Emulator that doesn't work right. I don't know where or how to pick this up from, 
but I got pretty darn far so maybe some minute part might be useful. I *believe* there is 100% instruction coverage.

**Emu8080** is the code actually pertaining to emulation. I'm fairly sure I never did interrupts properly and didn't design this well.

**Debug8080** lets you load 8080 instructions from a file and step through, see what flags are set and the value of each register.

**Test8080** is the test suite to make sure each instruction works as it should.

**Play8080** is where the actual emulator would run and play a game (hardcoded to Space Invaders in this case), *if it worked*.
