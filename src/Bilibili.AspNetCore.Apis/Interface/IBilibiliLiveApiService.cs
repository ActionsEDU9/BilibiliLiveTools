﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Bilibili.AspNetCore.Apis.Models;

namespace Bilibili.AspNetCore.Apis.Interface
{
    public interface IBilibiliLiveApiService
    {
        /// <summary>
        /// 获取我的直播间信息
        /// </summary>
        /// <returns></returns>
        Task<MyLiveRoomInfo> GetMyLiveRoomInfo();

        /// <summary>
        /// 获取直播间信息
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<LiveRoomInfo> GetLiveRoomInfo(long roomId);

        /// <summary>
        /// 获取直播间分类
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 无需登录
        /// </remarks>
        Task<List<LiveAreaItem>> GetLiveAreas();

        /// <summary>
        /// 获取直播间信息
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        /// <remarks>
        /// 无需登录
        /// </remarks>
        Task<RoomPlayInfo> GetRoomPlayInfo(long roomId);

        /// <summary>
        /// 获取直播版本
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSec"></param>
        /// <returns></returns>
        Task<LiveVersionInfo> GetHomePageLiveVersion();

        /// <summary>
        /// 更新直播间分区
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        Task<bool> UpdateLiveRoomInfo(long roomId, string title, int areaId);

        /// <summary>
        /// 更新直播间公告
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<bool> UpdateRoomNews(long roomId, string content);

        /// <summary>
        /// 开始直播
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<StartLiveInfo> StartLive(long roomId, int areaId);

        /// <summary>
        /// 停止直播
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        Task<StopLiveInfo> StopLive(long roomId);
    }
}
