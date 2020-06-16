using LeaderboardDataAccess;
using PMDemo.DataProvider;
using PMDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PMDemo.Controllers
{
    /// <summary>
    /// API service controller for rankings.
    /// </summary>
    public class RankingController : ApiController
    {
        private LeaderboardDataProvider LeaderboardDataProvider;
        public RankingController()
        {
            LeaderboardDataProvider = new LeaderboardDataProvider();
        }

        /// <summary>
        /// Gets all the rankings in the leaderboard database.
        /// </summary>
        /// <param name="name">Name of the leaderboard you want to retrieve.</param>
        /// <param name="size">Size of the leaderboard. Optional, entire size by default.</param>
        /// <param name="begin">Start of the leaderboard. Optional, starts at rank 1 at default.</param>
        /// <param name="ascending">Rating sort direction. Optional, Sorted by highest rating first by default.</param>
        public IEnumerable<RankingView> GetAllRankings()
        {
            return LeaderboardDataProvider.RetrieveRankingViews();
        }

        /// <summary>
        /// Gets all the rankings of a specified leaderboard name.
        /// </summary>
        /// <param name="name">Name of the leaderboard you want to retrieve.</param>
        /// <param name="size">Size of the leaderboard. Optional, entire size by default.</param>
        /// <param name="begin">Start of the leaderboard. Optional, starts at rank 1 at default.</param>
        /// <param name="ascending">Rating sort direction. Optional, Sorted by highest rating first by default.</param>
        public IEnumerable<RankingView> GetLeaderboardRankings(string name, bool ascending, int? size = null, int? begin = null)
        {
            return LeaderboardDataProvider.RetrieveLeaderboardRankings(name, ascending, size, begin);
        }

        /// <summary>
        /// Gets a ranking by ID in the leadership database.
        /// </summary>
        /// <param name="username">Username of the ranking you want to retrieve.</param>
        /// <param name="leaderboardName">Leaderboard of the ranking you want to retrieve.</param>
        public RankingView GetRanking(string username, string leaderboard)
        {
            return LeaderboardDataProvider.RetrieveLeaderboardRanking(username, leaderboard);
        }

        /// <summary>
        /// Creates or updates the ranking, if already exists, in the leaderboard database.
        /// </summary>
        /// <param name="ranking">Ranking that you intend to upsert.</param>
        public HttpResponseMessage UpsertRanking([FromBody] PMRanking ranking)
        {
            try
            {
                CheckArgumentExists(ranking.Username, nameof(ranking.Username));
                CheckArgumentExists(ranking.LeaderboardName, nameof(ranking.LeaderboardName));
                CheckRatingNegative(ranking.Rating);

                Ranking foundRanking = LeaderboardDataProvider.FindRanking(ranking.Username, ranking.LeaderboardName);
                if (null == foundRanking)
                {
                    Ranking newRanking = LeaderboardDataProvider.CreateRanking(ranking);
                    var message = Request.CreateResponse(HttpStatusCode.Created, ranking);
                    message.Headers.Location = new Uri(Request.RequestUri + newRanking.RankingID.ToString());
                    return message;
                }
                else
                {
                    LeaderboardDataProvider.UpdateRanking(foundRanking, ranking);
                    return Request.CreateResponse(HttpStatusCode.OK, ranking);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Deletes a ranking in the leaderboard database given an username and leaderboard pair.
        /// </summary>
        /// <param name="username">Username of the ranking you want to delete.</param>
        /// <param name="leaderboard">Leaderboard of the ranking you want to delete.</param>
        public HttpResponseMessage DeleteRanking(string username, string leaderboard)
        {
            try
            {
                Ranking ranking = LeaderboardDataProvider.DeleteRanking(username, leaderboard);
                return Request.CreateResponse(HttpStatusCode.OK, ranking);
            }
            catch (KeyNotFoundException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Checks if the passed in argument exists, throws exception if it doesn't.
        /// </summary>
        /// <param name="argument">Argument that is being checked.</param>
        /// <param name="argumentName">Name of the argument.</param>
        private void CheckArgumentExists<T>(T argument, string argumentName)
        {
            if (null == argument)
            {
                throw new ArgumentNullException($"{argumentName} cannot be null.");
            }
        }

        /// <summary>
        /// Checks if the ranking's rating is below zero.
        /// </summary>
        /// <param name="rating">The ranking's rating.</param>
        private void CheckRatingNegative(int rating)
        {
            if (rating < 0)
            {
                throw new InvalidOperationException($"Rating cannot be below zero.");
            }
        }

        /// <summary>
        /// Checks if there is a ranking with matching username and leaderboard pair. Throws exception if found.
        /// </summary>
        /// <param name="rankings">Rankings you wish to check from.</param>
        /// <param name="newRanking">Ranking you are intending to change.</param>
        //private void CheckDuplicateRanking(IEnumerable<PMRanking> rankings, PMRanking newRanking)
        //{
        //    PMRanking duplicateRanking = rankings.Where(x => x.Username == newRanking.Username && x.Leaderboard == newRanking.Leaderboard).FirstOrDefault();
        //    if (duplicateRanking != null)
        //    {
        //        throw new InvalidOperationException($"Ranking for {newRanking.Username} on {newRanking.Leaderboard} leaderboard already exists.");
        //    }
        //}
    }
}
