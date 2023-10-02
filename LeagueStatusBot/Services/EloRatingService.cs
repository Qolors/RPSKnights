using System;

namespace LeagueStatusBot.Services;

public class EloRating
{
    private const int KFactor = 32; // The maximum change in rating. Adjust this as needed.

    public static double CalculateExpectedOutcome(double ratingA, double ratingB)
    {
        return 1.0 / (1.0 + Math.Pow(10, (ratingB - ratingA) / 400.0));
    }

    public static double UpdateRating(double currentRating, double actualOutcome, double expectedOutcome)
    {
        return currentRating + KFactor * (actualOutcome - expectedOutcome);
    }

    public static void UpdateRatings(ref double ratingA, ref double ratingB, double outcomeA)
    {
        // Calculate expected outcomes for each player
        double expectedOutcomeA = CalculateExpectedOutcome(ratingA, ratingB);
        double expectedOutcomeB = 1 - expectedOutcomeA; // Since it's a two-player game

        // Update ratings based on actual outcomes
        ratingA = UpdateRating(ratingA, outcomeA, expectedOutcomeA);
        ratingB = UpdateRating(ratingB, 1 - outcomeA, expectedOutcomeB);
    }
}