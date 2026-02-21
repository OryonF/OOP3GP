# Object Oriented Programming 3 Group Project (Group 1)

Course project for OOP3.

## Chosen game: 2 player Durak

Durak is a Russian card game where the objective is to be the first to empty one's own hand.

### Setup
- All cards with values of 2 through 5 (inclusive) are removed from the standard deck, leaving only 36 cards
- After shuffling the deck, players are dealt 6 cards each. The trump suit is decided at the start of the game by revealing the bottom card from the deck.
- The turn-up is placed face up, with the remainder of the deck face down on top of it at a right angle.
- The first attacker is whoever has the lowest-value trump suit card in their hand.
- For our purposes, if neither player has a trump card in their starting hand, the human player will go before the computer player

### Rounds
- If the attacker plays a card that is not of the trump suit, the defender must defend with a card of either:
- - the same suit as the attacking card with a higher value
- - a card of the trump suit with any value
- If the attacker plays a card of the trump suit, the defender can only defend with a trump suit card with a higher value
- On a successful defense, the attacker may declare a new attack if they can play a card of the same value as any attacking card previously played in the given round.
- There can be up to 6 attacks per round
- If the defending player does not play a defending card, they must add all cards played during the attack to their hand, and the attacking player goes again.
- If the attacking player does not play a new attacking card, all cards played during the attack go to a discard pile and the defender will attack next turn

### End of turn
- After the battle is resolved, players with less than 6 cards in hand draw new cards from the deck until they have 6 cards in hand or the deck is empty.
- The attacker draws cards first, followed by the defender. After both players have drawn, roles are swapped if the defender was successful in the last round.

### End of game
- The first player to empty their hand wins immediately. The other player is the "Durak" (fool)