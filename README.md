# Evealopalous-Player
Evealopalous was an Eve Online gambling site that allowed you to exchange in game credits for credit on their website to gamble for in-game credits and items.

One of the games on the site was an N X N grid where each cell contained a hidden value. You are told a head of time how many cells contain what values. For example in a 10x10 grid 80 of them could be worth 10, 15 worth 20, and 5 worth 100. You could pay a fixed amount to win the contents of the cell by clicking on it. Once a cell was clicked on the value is displayed for all players. This was a multiplayer game so other players would be playing on the same grid as you.

Given that the distribution of the values and the claimed values were known it is easy to calculate the expected value of clicking on a cell at any point.

This application would wait for other players to play the game and as soon as the grid had an expected value higher than the price of playing it would automatically play.

Unfortunately the game did not have a high volume of players and also experienced intermitent network issues so the bot never made significant amounts of money. Probably less than 5 million ISK.

The Evealopalous site is now defunct or has changed names.
