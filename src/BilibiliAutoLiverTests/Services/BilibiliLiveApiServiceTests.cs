﻿using Bilibili.AspNetCore.Apis.Interface;

namespace BilibiliLiverTests.Services
{
    [TestClass()]
    public class BilibiliLiveApiServiceTests : BilibiliLiverTestsBase
    {
        private readonly IBilibiliLiveApiService _apiService;

        public BilibiliLiveApiServiceTests()
        {
            _apiService = (IBilibiliLiveApiService)ServiceProvider.GetService(typeof(IBilibiliLiveApiService));
            if (_apiService == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetLiveRoomInfoTest()
        {
            var info = await _apiService.GetMyLiveRoomInfo();
            Assert.IsNotNull(info);
        }

        [TestMethod()]
        public async Task GetLiveAreasTest()
        {
            var info = await _apiService.GetLiveAreas();
            Assert.IsNotNull(info);
        }

        [TestMethod()]
        public async Task GetHomePageLiveVersion()
        {
            var info = await _apiService.GetHomePageLiveVersion();
            Assert.IsNotNull(info);
        }

        [TestMethod()]
        public async Task StartLiveTest()
        {
            var info = await _apiService.GetMyLiveRoomInfo();
            var reslt = await _apiService.StartLive(info.room_id, 369);
            Assert.IsNotNull(reslt);
        }

        [TestMethod()]
        public async Task StopLiveTest()
        {
            var info = await _apiService.GetMyLiveRoomInfo();
            var reslt = await _apiService.StopLive(info.room_id);

            var liveRoomInfo = await _apiService.GetMyLiveRoomInfo();
            if (liveRoomInfo.live_status != 0)
            {
                Assert.Fail();
            }

            Assert.IsNotNull(reslt);
        }

        [TestMethod()]
        public async Task GetRoomPlayInfoTest()
        {
            var reslt = await _apiService.GetRoomPlayInfo(21614697);
            Assert.IsNotNull(reslt);
        }
    }
}