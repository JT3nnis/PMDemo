using LeaderboardDataProvider;
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
        /// <summary>
        /// Gets all rankings in the leadership database.
        /// </summary>
        public IEnumerable<Ranking> GetAllRankings()
        {
            using (LeaderboardDBEntities entities = new LeaderboardDBEntities())
            {
                return entities.Rankings.ToList();
            }
        }

        /// <summary>
        /// Gets a ranking by ID in the leadership database.
        /// </summary>
        /// <param name="id">ID of the ranking you want to retrieve.</param>
        public Ranking GetRankingByID(int id)
        {
            using (LeaderboardDBEntities entities = new LeaderboardDBEntities())
            {
                return entities.Rankings.FirstOrDefault(d => d.ID == id);
            }
        }

        /// <summary>
        /// Gets all the rankings of a specified leaderboard name.
        /// </summary>
        /// <param name="name">Name of the leaderboard you want to retrieve.</param>
        /// <param name="size">Size of the leaderboard. Optional, entire size by default.</param>
        /// <param name="begin">Start of the leaderboard. Optional, starts at rank 1 at default.</param>
        /// <param name="ascending">Rating sort direction. Optional, Sorted by highest rating first by default.</param>
        public IEnumerable<RankingView> GetLeaderboardRankings(string name, int? size = null, int? begin = null, bool ascending = true)
        {
            using (LeaderboardDBEntities entities = new LeaderboardDBEntities())
            {
                IEnumerable<Ranking> rankings = entities.Rankings.AsEnumerable().OrderBy(x => x.Rating).Where(x => x.Leaderboard == name).Reverse().ToList();
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
                    return ConvertRankingsToRankingViews(rankings.Reverse(), begin);
                }
            }
        }

        /// <summary>
        /// Creates or updates the ranking, if already exists, in the leaderboard database.
        /// </summary>
        /// <param name="ranking">Ranking that you intend to upsert.</param>
        public HttpResponseMessage UpsertRanking([FromBody] Ranking ranking)
        {
            try
            {
                CheckArgumentExists(ranking.Username, nameof(ranking.Username));
                CheckArgumentExists(ranking.Leaderboard, nameof(ranking.Leaderboard));
                CheckRatingNegative(ranking.Rating);

                using (LeaderboardDBEntities entities = new LeaderboardDBEntities())
                {
                    Ranking foundRanking = entities.Rankings.Where(x => x.Username == ranking.Username && x.Leaderboard == ranking.Leaderboard).FirstOrDefault();
                    if (foundRanking != null)
                    {
                        foundRanking.Rating = ranking.Rating;
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, foundRanking);
                    }
                    else
                    {
                        entities.Rankings.Add(ranking);
                        entities.SaveChanges();

                        var message = Request.CreateResponse(HttpStatusCode.Created, ranking);
                        message.Headers.Location = new Uri(Request.RequestUri + ranking.ID.ToString());
                        return message;
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Updates a ranking in the leaderboard database.
        /// </summary>
        /// <param name="id">ID of the ranking you want to update.</param>
        /// <param name="ranking">Ranking that you intend to update.</param>
        public HttpResponseMessage UpdateRanking(int id, [FromBody] Ranking ranking)
        {
            try
            {
                CheckArgumentExists(ranking.Username, nameof(ranking.Username));
                CheckArgumentExists(ranking.Leaderboard, nameof(ranking.Leaderboard));
                CheckRatingNegative(ranking.Rating);

                using (LeaderboardDBEntities entities = new LeaderboardDBEntities())
                {
                    Ranking foundRanking = entities.Rankings.Where(x => x.ID == id).FirstOrDefault();
                    if (null == foundRanking)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Ranking with ID: {id} was not found.");
                    }

                    CheckDuplicateRanking(entities.Rankings, ranking);

                    foundRanking.Username = ranking.Username;
                    foundRanking.Leaderboard = ranking.Leaderboard;
                    foundRanking.Rating = ranking.Rating;
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, foundRanking);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        /// <summary>
        /// Deletes a ranking in the leaderboard database.
        /// </summary>
        /// <param name="id">ID of the ranking you want to delete.</param>
        public HttpResponseMessage DeleteRanking(int id)
        {
            try
            {
                using (LeaderboardDBEntities entities = new LeaderboardDBEntities())
                {
                    Ranking foundRanking = entities.Rankings.Where(x => x.ID == id).FirstOrDefault();
                    if (null == foundRanking)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Ranking with ID: {id} was not found.");
                    }

                    entities.Rankings.Remove(foundRanking);
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, foundRanking);
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
                using (LeaderboardDBEntities entities = new LeaderboardDBEntities())
                {
                    Ranking foundRanking = entities.Rankings.Where(x => x.Username == username && x.Leaderboard == leaderboard).FirstOrDefault();
                    if (null == foundRanking)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Ranking for {username} on {leaderboard} leaderboard was not found.");
                    }

                    entities.Rankings.Remove(foundRanking);
                    entities.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, foundRanking);
                }
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
        private void CheckDuplicateRanking(IEnumerable<Ranking> rankings, Ranking newRanking)
        {
            Ranking duplicateRanking = rankings.Where(x => x.Username == newRanking.Username && x.Leaderboard == newRanking.Leaderboard).FirstOrDefault();
            if (duplicateRanking != null)
            {
                throw new InvalidOperationException($"Ranking for {newRanking.Username} on {newRanking.Leaderboard} leaderboard already exists.");
            }
        }

        /// <summary>
        /// Converts a Ranking model into a Ranking View Model.
        /// </summary>
        /// <param name="ranking">Ranking model you are converting.</param>
        /// <param name="index">Index of ranking.</param>
        /// <param name="begin">Beginning index of the rankings.</param>
        private RankingView ConvertRankingToRankingView(Ranking ranking, int index, int? begin = 0)
        {
            if (null == begin)
            {
                begin = 0;
            }

            RankingView rankingView = new RankingView();
            rankingView.Rank = index + 1 + (int)begin;
            rankingView.Username = ranking.Username;
            rankingView.Rating = ranking.Rating;

            return rankingView;
        }

        /// <summary>
        /// Converts an enumerable set of Ranking models into a Ranking View Models.
        /// </summary>
        /// <param name="rankings">Ranking models you are converting.</param>
        /// <param name="begin">Beginning index of the rankings.</param>
        private IEnumerable<RankingView> ConvertRankingsToRankingViews(IEnumerable<Ranking> rankings, int? begin = 0)
        {
            IList<RankingView> rankingViews = new List<RankingView>();
            IList<Ranking> RankingList = rankings.ToList();
            for (int i = 0; i < RankingList.Count; i++)
            {
                RankingView rankingView = ConvertRankingToRankingView(RankingList[i], i, begin);
                rankingViews.Add(rankingView);
            }

            return rankingViews;
        }
    }
}
