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
    /// API service controller for leaderboards.
    /// </summary>
    public class LeaderboardController : ApiController
    {
        private LeaderboardDataProvider LeaderboardDataProvider;
        public LeaderboardController()
        {
            LeaderboardDataProvider = new LeaderboardDataProvider();
        }

        /// <summary>
        /// Finds all leaderboards in a leaderboard database.
        /// </summary>
        public IEnumerable<LeaderboardView> GetLeaderboards()
        {
            return LeaderboardDataProvider.RetrieveLeaderboards();
        }

        /// <summary>
        /// Creates a leaderboard in the leaderboard database.
        /// </summary>
        /// <param name="leaderboard">Leaderboard that you intend to create.</param>
        public HttpResponseMessage CreateLeaderboard([FromBody] Leaderboard leaderboard)
        {
            try
            {
                Leaderboard newLeaderboard = LeaderboardDataProvider.CreateLeaderboard(leaderboard);
                var message = Request.CreateResponse(HttpStatusCode.Created, leaderboard);
                message.Headers.Location = new Uri(Request.RequestUri + newLeaderboard.LeaderboardID.ToString());
                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
