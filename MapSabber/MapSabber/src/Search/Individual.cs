using System;
using System.Collections.Generic;

using SabberStoneCore.Model;

namespace MapSabber.Search
{
   class Individual
   {
      private static Random rnd = new Random();
      public static Individual GenerateRandomIndividual(List<Card> cardSet)
      {
         // Generate all card ids and permute them. Use this info to
         // construct counts.
         var available = new List<int>();
         for (int i=0; i<cardSet.Count; i++)
            for (int cnt=0; cnt<cardSet[i].MaxAllowedInDeck; cnt++)
               available.Add(i);

         for (int i=1; i<available.Count; i++)
			{
				int j = (int)(rnd.NextDouble() * i);
				int tmp = available[i];
				available[i] = available[j];
				available[j] = tmp;
			}

         var cardCounts = new int[cardSet.Count];
         for (int i=0; i<30; i++)
         {
            int cardId = available[i];
            cardCounts[cardId]++;
         }

         return new Individual(cardCounts, cardSet);
      }
   
      private int[] _cardCounts;
      private List<Card> _cardSet;
    
      public int ID { get; set; }
      public int ParentID { get; set; }
      public int WinCount { get; set; }
      public int TotalHealthDifference { get; set; }
      public int DamageDone { get; set; }
      public int NumTurns { get; set; }
      public int CardsDrawn { get; set; }
      public int HandSize { get; set; }
      public int ManaSpent { get; set; }
      public int ManaWasted { get; set; }
      public int StrategyAlignment { get; set; }
      public int Dust { get; set; }
      public int DeckManaSum { get; set; }
      public int DeckManaVariance { get; set; }
      public int NumMinionCards { get; set; }
      public int NumSpellCards { get; set; }
      
      public int Fitness { get; set; }
      public int[] Features { get; set; }

      public Individual(int[] cardCounts, List<Card> cardSet)
      {
         _cardCounts = cardCounts;
         _cardSet = cardSet;
         ParentID = -1;
      }

      public int GetStatByName(string name)
      {
         if (name.Equals("WinCount"))
            return WinCount;
         if (name.Equals("TotalHealthDifference"))
            return TotalHealthDifference;
         if (name.Equals("DamageDone"))
            return DamageDone;
         if (name.Equals("NumTurns"))
            return NumTurns;
         if (name.Equals("CardsDrawn"))
            return CardsDrawn;
         if (name.Equals("HandSize"))
            return HandSize;
         if (name.Equals("ManaSpent"))
            return ManaSpent;
         if (name.Equals("ManaWasted"))
            return ManaWasted;
         if (name.Equals("StrategyAlignment"))
            return StrategyAlignment;
         if (name.Equals("Dust"))
            return Dust;
         if (name.Equals("DeckManaSum"))
            return DeckManaSum;
         if (name.Equals("DeckManaVariance"))
            return DeckManaVariance;
         if (name.Equals("NumMinionCards"))
            return NumMinionCards;
         if (name.Equals("NumSpellCards"))
            return NumSpellCards;

         return Int32.MinValue;
      }

      // Generate a random individual via mutation
      public Individual Mutate()
      {
         int cardsInDeck = 30;
         var cardCounts = new int[_cardCounts.Length];
         Array.Copy(_cardCounts, cardCounts, cardCounts.Length);

         // Try swapping out cards in a cascading fashion.
         double taken = 0.0;
         while (taken < 0.5 && cardsInDeck > 0)
         {
            int cardNum = rnd.Next(cardsInDeck);

            // Find the cardNum'th card in the set.
            int cardId = 0;
            while (cardCounts[cardId] == 0 || cardNum-cardCounts[cardId] > 0)
            {
               cardNum -= cardCounts[cardId];
               cardId++;
            }

            // Remove this card
            cardCounts[cardId]--;
            cardsInDeck--;

            // Keep going?
            taken = rnd.NextDouble();
         }

         // Try putting cards back in until we have a full deck.
         while (cardsInDeck < 30)
         {
            int numTypesAvailable = 0;
            for (int i=0; i<cardCounts.Length; i++)
               if (cardCounts[i] < _cardSet[i].MaxAllowedInDeck)
                  numTypesAvailable++;

            // Find a random legal card.
            int typeSelected = rnd.Next(numTypesAvailable);
            int cardId = 0;
            while (cardCounts[cardId] == _cardSet[cardId].MaxAllowedInDeck || 
                   typeSelected > 0)
            {
               if (cardCounts[cardId] < _cardSet[cardId].MaxAllowedInDeck)
                  typeSelected--;
               cardId++;
            }
           
            // Add the card
            cardCounts[cardId]++;
            cardsInDeck++;
         }

         var result = new Individual(cardCounts, _cardSet);
         result.ParentID = ID;
         return result;
      }
   
      public List<string> GetCards()
      {
         var cards = new List<string>();
         for (int i=0; i<_cardCounts.Length; i++)
            for (int cnt=0; cnt<_cardCounts[i]; cnt++)
               cards.Add(_cardSet[i].Name);
         return cards;
      }

      public override string ToString()
      {
         return string.Join("", _cardCounts);
      }
   }
}
