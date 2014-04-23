HuntTheWumpus
=============

One of the Tesla High School Hunt the Wumpus groups.

This implementation is done with .net, C# and XNA.

Contributed to by Alex Hoar, Dylan Katz, Martin Berger, Paul Siicu, Divya Cherukupalli, Kostya Yatsuk.

This brach is maintained by Dylan, and contains experimental features for the highscore tracking system.

### Differences

Loading system works in a different thread in order to not disrupt the GUI if something takes too long for some reason.

There is a networked high score table, so you can see how you stack up against the rest of the world.

The client for this is integrated with the regular high score class, so that the API remains consistant.

The additional `.py` file is the score server, which hosts the global high score board, and sends data to the client.

There still needs to be testing, and the networked stuff is far from finished.
