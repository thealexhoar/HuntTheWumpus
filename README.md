HuntTheWumpus
=============

One of the Tesla High School Hunt the Wumpus groups.

This implementation is done with .net, C# and XNA.

Contributed to by Alex Hoar, Dylan Katz, Martin Berger, Paul Siicu, Divya Cherukupalli, Kostya Yatsuk.

This brach is maintained by Dylan, and contains experimental features for the highscore tracking system.

### Differences

Loading system works in a different thread in order to not disrupt the GUI if something takes too long for some reason.

Material is in place for a networked global score system. There is an additional project called `ScoreServer`, which proccesses highscores, saves them, and pushes the top ten to the client. Within the client there is code that sends highscore data to the server.
