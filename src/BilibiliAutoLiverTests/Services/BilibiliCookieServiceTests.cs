﻿using System.Diagnostics;
using Bilibili.AspNetCore.Apis.Interface;
using Bilibili.AspNetCore.Apis.Models;
using Bilibili.AspNetCore.Apis.Models.Base;

namespace BilibiliLiverTests.Services
{
    [TestClass()]
    public class BilibiliCookieServiceTests : BilibiliLiverTestsBase
    {
        private readonly IBilibiliCookieService _cookieService;
        private readonly IBilibiliAccountApiService _accountService;

        public BilibiliCookieServiceTests()
        {
            _cookieService = (IBilibiliCookieService)ServiceProvider.GetService(typeof(IBilibiliCookieService));
            if (_cookieService == null)
            {
                Assert.Fail();
            }
            _accountService = (IBilibiliAccountApiService)ServiceProvider.GetService(typeof(IBilibiliAccountApiService));
            if (_accountService == null)
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task SaveTest()
        {
            var scanResult = await GetQrCodeResult();
            await _cookieService.SaveCookie(scanResult.Cookies, scanResult.Data.refresh_token);
        }

        [TestMethod()]
        public async Task AllTest()
        {
            var cookies = await _cookieService.GetCookies();
            var cookieString = await _cookieService.GetString();

            bool hasCookie = await _cookieService.HasCookie();
            Assert.IsTrue(hasCookie);



            cookies = await _cookieService.GetCookies(true);
            cookieString = await _cookieService.GetString(true);

            var csrf = await _cookieService.GetCsrf();
            var userId = await _cookieService.GetUserId();

            string token = await _cookieService.GetRefreshToken();
            Assert.IsNotNull(token);
        }

        public async Task<ResultModel<QrCodeScanResult>> GetQrCodeResult()
        {
            QrCodeUrl qrCode = await _accountService.GenerateQrCode();

            byte[] qrCodeBytes = qrCode.GetBytes();
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                var result = await _accountService.QrCodeScanStatus(qrCode.qrcode_key);
                if (result.Data.status == QrCodeStatus.Scaned)
                {
                    return result;
                }
                if (result.Data.status == QrCodeStatus.Expired)
                {
                    throw new Exception("二维码已过期");
                }
                Debug.WriteLine($"等待扫描中...耗时:{sw.ElapsedMilliseconds / 1000}s");
                await Task.Delay(1000);
            }
        }

        [TestMethod()]
        public async Task CookieNeedToRefreshTest()
        {
            var result = await _accountService.CookieNeedToRefresh();
        }

        [TestMethod()]
        public async Task RefreshCookieTest()
        {
            UserInfo userInfo = await _accountService.LoginByCookie();

            await _accountService.RefreshCookie();

            userInfo = await _accountService.LoginByCookie();
        }


        [TestMethod()]
        public async Task GetTest()
        {
            string cookieText = await _cookieService.GetString();
            Assert.IsTrue(!string.IsNullOrEmpty(cookieText));
        }

        [TestMethod()]
        public async Task GetCsrfTest()
        {
            string csrf = await _cookieService.GetCsrf();
            Assert.IsNotNull(csrf);
        }

        [TestMethod()]
        public async Task GetUserIdTest()
        {
            string userid = await _cookieService.GetUserId();
            Assert.IsNotNull(userid);
        }
    }
}