﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace Foundation.Test.Api.ApiControllers.Tests
{
    [TestClass]
    public class WebApiTests
    {
        [TestMethod]
        [TestCategory("WebApi")]
        public virtual async Task WebApiShouldReturnOk()
        {
            using (TestEnvironment testEnvironment = new TestEnvironment())
            {
                OAuthToken token = testEnvironment.Server.Login("ValidUserName", "ValidPassword");

                HttpClient client = testEnvironment.Server.GetHttpClient(token: token);

                HttpResponseMessage response = await client.GetAsync("api/customers/1/orders");

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
