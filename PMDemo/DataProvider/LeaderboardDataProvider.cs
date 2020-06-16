using LeaderboardDataAccess;
using PMDemo.Controllers;
using PMDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMDemo.DataProvider
{
    public class LeaderboardDataProvider
    {
        private LeaderboardDBEntities LeaderboardDBEntities;

        public LeaderboardDataProvider()
        {
            LeaderboardDBEntities = new LeaderboardDBEntities();
        }

        /// <summary>
        /// Finds all rankings in a leaderboard database.
        /// </summary>
        public IEnumerable<Ranking> RetrieveRankings()
        {
            return LeaderboardDBEntities.Rankings.ToList();
        }

        /// <inheritdoc cref="RankingController.GetLeaderboardRankings(string, bool, int?, int?)"/>
        public IEnumerable<RankingView> RetrieveLeaderboardRankings(string name, bool ascending, int? size = null, int? begin = null)
        {
            IEnumerable<Ranking> rankings = LeaderboardDBEntities.Rankings.AsEnumerable().OrderBy(x => x.Rating).Where(x => x.Leaderboard.LeaderboardName.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Reverse().ToList();
            if (begin != null)
            {
                rankings = rankings.Skip((int)begin);
            }

            if (size != null)
            {
                rankings = rankings.Take((int)size);
            }

            if (ascending)
            {
                return ConvertRankingsToRankingViews(rankings, begin);
            }
            else
            {
                return ConvertRankingsToRankingViews(rankings.Reverse(), begin, ascending);
            }
        }

        /// <inheritdoc cref="RankingController.GetRanking(string, string)"/>
        public RankingView RetrieveLeaderboardRanking(string username, string leaderboardName)
        {
            IEnumerable<RankingView> rankingViews = RetrieveLeaderboardRankings(leaderboardName, true);
            return rankingViews.Where(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase) && x.LeaderboardName.Equals(leaderboardName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Finds a ranking in a leaderboard based off their username.
        /// </summary>
        /// <param name="username">Username that is being queried.</param>
        /// <param name="leaderboardName">Name of the leaderboard queried.</param>
        public Ranking FindRanking(string username, string leaderboardName)
        {
            Ranking foundRanking = LeaderboardDBEntities.Rankings.Where(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase) && x.Leaderboard.LeaderboardName.Equals(leaderboardName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            return foundRanking;
        }

        /// <summary>
        /// Creates a ranking in the leaderboard database.
        /// </summary>
        /// <param name="ranking">Ranking that you are creating.</param>
        public Ranking CreateRanking(PMRanking ranking)
        {
            try
            {
                Ranking newRanking = new Ranking();
                newRanking.Username = ranking.Username;
                newRanking.Rating = ranking.Rating;
                Leaderboard leaderboard = LeaderboardDBEntities.Leaderboards.Where(x => x.LeaderboardName.Equals(ranking.LeaderboardName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (null == leaderboard)
                {
                    Leaderboard newLeaderboard = new Leaderboard();
                    newLeaderboard.LeaderboardName = ranking.LeaderboardName;
                    LeaderboardDBEntities.Leaderboards.Add(newLeaderboard);
                    leaderboard = newLeaderboard;
                }

                newRanking.Leaderboard = leaderboard;
                newRanking.LeaderboardID = leaderboard.LeaderboardID;
                LeaderboardDBEntities.Rankings.Add(newRanking);
                LeaderboardDBEntities.SaveChanges();
                return newRanking;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a ranking in the leaderboard database.
        /// </summary>
        /// <param name="foundRanking">Ranking that you intend to update.</param>
        /// <param name="ranking">Values of the ranking you intend to change.</param>
        public Ranking UpdateRanking(Ranking foundRanking, PMRanking ranking)
        {
            try
            {
                Ranking newRanking = new Ranking();
                newRanking.RankingID = foundRanking.RankingID;
                newRanking.Username = ranking.Username;
                newRanking.Rating = ranking.Rating;
                Leaderboard leaderboard = LeaderboardDBEntities.Leaderboards.Where(x => x.LeaderboardName.Equals(ranking.LeaderboardName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                newRanking.Leaderboard = leaderboard;
                newRanking.LeaderboardID = leaderboard.LeaderboardID;
                LeaderboardDBEntities.Entry(foundRanking).CurrentValues.SetValues(newRanking);
                LeaderboardDBEntities.SaveChanges();
                return foundRanking;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes a ranking in the leaderboard database given an username and leaderboard pair.
        /// </summary>
        /// <param name="username">Username of the ranking you want to delete.</param>
        /// <param name="leaderboard">Leaderboard of the ranking you want to delete.</param>
        public Ranking DeleteRanking(string username, string leaderboard)
        {
            try
            {
                Ranking foundRanking = LeaderboardDBEntities.Rankings.Where(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase) && x.Leaderboard.LeaderboardName.Equals(leaderboard, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (null == foundRanking)
                {
                    throw new KeyNotFoundException($"Ranking for {username} on {leaderboard} leaderboard was not found.");
                }

                LeaderboardDBEntities.Rankings.Remove(foundRanking);
                LeaderboardDBEntities.SaveChanges();
                return foundRanking;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <inheritdoc cref="LeaderboardController.GetLeaderboards"/>
        public IEnumerable<LeaderboardView> RetrieveLeaderboards()
        {
            return ConvertLeaderboardsToLeaderboardViews(LeaderboardDBEntities.Leaderboards.ToList());
        }

        /// <inheritdoc cref="LeaderboardController.CreateLeaderboard(Leaderboard)"/>
        public Leaderboard CreateLeaderboard(Leaderboard leaderboard)
        {
            try
            {
                Leaderboard foundLeaderboard = LeaderboardDBEntities.Leaderboards.Where(x => x.LeaderboardName.Equals(leaderboard.LeaderboardName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                if (foundLeaderboard != null)
                {
                    throw new NotImplementedException($"{leaderboard.LeaderboardName} leaderboard already exists.");
                }

                LeaderboardDBEntities.Leaderboards.Add(leaderboard);
                LeaderboardDBEntities.SaveChanges();
                return leaderboard;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Checks if there is a ranking with matching username and leaderboard pair. Throws exception if found.
        /// </summary>
        /// <param name="rankings">Rankings you wish to check from.</param>
        /// <param name="username">Ranking username you are intending to change.</param>
        /// <param name="leaderboardName">Ranking leaderboard you are intending to change.</param>
        public void CheckDuplicateRanking(IEnumerable<Ranking> rankings, string username, string leaderboardName)
        {
            Ranking duplicateRanking = rankings.Where(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase) && x.Leaderboard.LeaderboardName.Equals(leaderboardName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (duplicateRanking != null)
            {
                throw new InvalidOperationException($"Ranking for {username} on {leaderboardName} leaderboard already exists.");
            }
        }

        /// <summary>
        /// Converts a Ranking model into a Ranking View Model.
        /// </summary>
        /// <param name="ranking">Ranking model you are converting.</param>
        /// <param name="index">Index of ranking.</param>
        /// <param name="begin">Beginning index of the rankings.</param>
        /// <param name="ascending">Sort direction of the rankings.</param>
        /// <param name="count">Size of the queried rankings.</param>
        private RankingView ConvertRankingToRankingView(Ranking ranking, int index, int? begin = 0, bool ascending = true, int? count = 0)
        {
            if (null == begin)
            {
                begin = 0;
            }

            RankingView rankingView = new RankingView();
            if (ascending)
            {
                rankingView.Rank = index + 1 + (int)begin;
            }
            else
            {
                rankingView.Rank = (int)count + (int)begin - index;
            }
            rankingView.Username = ranking.Username;
            rankingView.Rating = ranking.Rating;
            rankingView.LeaderboardName = ranking.Leaderboard.LeaderboardName;

            return rankingView;
        }

        /// <summary>
        /// Converts an enumerable set of Ranking models into a Ranking View Models.
        /// </summary>
        /// <param name="rankings">Ranking models you are converting.</param>
        /// <param name="begin">Beginning index of the rankings.</param>
        /// <param name="asccending">Sort direction of the rankings.</param>
        private IEnumerable<RankingView> ConvertRankingsToRankingViews(IEnumerable<Ranking> rankings, int? begin = 0, bool asccending = true)
        {
            IList<RankingView> rankingViews = new List<RankingView>();
            IList<Ranking> RankingList = rankings.ToList();
            for (int i = 0; i < RankingList.Count; i++)
            {
                RankingView rankingView = ConvertRankingToRankingView(RankingList[i], i, begin, asccending, RankingList.Count);
                rankingViews.Add(rankingView);
            }

            return rankingViews;
        }

        /// <summary>
        /// Converts a Leaderboard model into a Ranking View Model.
        /// </summary>
        /// <param name="leaderboard">Leaderboard model you are converting.</param>
        private LeaderboardView ConvertLeaderboardToLeaderboardView(Leaderboard leaderboard)
        {
            LeaderboardView leaderboardView = new LeaderboardView();
            leaderboardView.LeaderboardName = leaderboard.LeaderboardName;
            return leaderboardView;
        }

        /// <summary>
        /// Converts an enumerable set of Leaderboard models into a Leaderboard View Models.
        /// </summary>
        /// <param name="leaderboards">Leaderboards models you are converting.</param>
        private IEnumerable<LeaderboardView> ConvertLeaderboardsToLeaderboardViews(IEnumerable<Leaderboard> leaderboards)
        {
            IList<LeaderboardView> leaderboardViews = new List<LeaderboardView>();
            foreach (Leaderboard leaderboard in leaderboards)
            {
                LeaderboardView leaderboardView = ConvertLeaderboardToLeaderboardView(leaderboard);
                leaderboardViews.Add(leaderboardView);
            }

            return leaderboardViews;
        }
    }
}