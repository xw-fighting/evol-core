﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evol.Domain.Dto;
using Microsoft.AspNetCore.Mvc;
using Evol.Common;
using Evol.TMovie.Website.Models.CinemaViewModels;
using Evol.TMovie.Domain.QueryEntries;
using Evol.TMovie.Domain.QueryEntries.Parameters;
using Evol.TMovie.Website.Models;

namespace Evol.TMovie.Website.ApiControllers
{
    [Produces("application/json")]
    [Route("api/Cinema")]
    public class CinemaApiController : Controller
    {
        public ICinemaQueryEntry CinemaQuery { get; private set; }

        public IScheduleQueryEntry ScheduleQuery { get; private set; }

        public CinemaApiController(ICinemaQueryEntry cinemaQueryEntry, IScheduleQueryEntry screeningQuery)
        {
            CinemaQuery = cinemaQueryEntry;
            ScheduleQuery = screeningQuery;
        }

        /// <summary>
        /// 分页获取影院信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Cinema/Paged")]
        public async Task<IPaged<CinemaViewModel>> Paged(int pageIndex = 1, int pageSize = 10, CinemaQueryParameter param = null)
        {
            var result = await CinemaQuery.PagedAsync(param, pageIndex, pageSize);
            var paged = result.MapPaged<CinemaViewModel>();
            return paged;
        }

        /// <summary>
        /// 获取影院信息
        /// </summary>
        /// <param name="id">cinemaId</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<CinemaViewModel> GetById(Guid id)
        {
            var item = await CinemaQuery.FindAsync(id);
            var value = item.Map<CinemaViewModel>();
            return value;
        }

        /// <summary>
        /// 获取上映电影
        /// </summary>
        /// <param name="id">cinemaId</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Route("api/Cinema/Show")]
        public async Task<ShowMovieViewModel> Screening(Guid id)
        {
            var items = await CinemaQuery.SelectShowingMoiveAsync(id);
            var value = items.Map<ShowMovieViewModel>();
            return value;
        }

        /// <summary>
        /// 获取排片计划
        /// </summary>
        /// <param name="id">cinemaId</param>
        /// <param name="movieId">movieId</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Route("api/Cinema/Sched")]  
        public async Task<List<ScheduleViewModel>> Schedule(Guid id, Guid movieId)
        {
            var item = await ScheduleQuery.SelectAsync(new ScheduleQueryParameter { CinemaId = id  });
            var values = item.MapList<ScheduleViewModel>();
            return values;
        }



    }
}