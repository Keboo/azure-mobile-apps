﻿// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Datasync.Common.Test;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Datasync.Integration.Test.Server
{
    public class TableController_Tests : BaseTest
    {
        [Theory]
        [InlineData("tables/movies/id-001", null)]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=true", null)]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=0", null)]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=somevalue", null)]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=1.0", null)]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=9.0.0", null)]
        [InlineData("tables/movies/id-001", "true")]
        [InlineData("tables/movies/id-001", "0")]
        [InlineData("tables/movies/id-001", "somevalue")]
        [InlineData("tables/movies/id-001", "1.0")]
        [InlineData("tables/movies/id-001", "9.0.0")]
        public async Task ZumoVersion_MissingOrInvalid_BadRequest(string relativeUri, string headerValue)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://localhost/{relativeUri}")
            };
            if (headerValue != null)
            {
                request.Headers.Add("ZUMO-API-VERSION", headerValue);
            }

            var response = await MovieHttpClient.SendAsync(request).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.DeserializeContentAsync<Dictionary<string, object>>().ConfigureAwait(false);
            Assert.True(result?.ContainsKey("title") ?? false);
            Assert.True(result?.ContainsKey("detail") ?? false);
        }

        [Theory]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=3.0", null)]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=3.0.0", null)]
        [InlineData("tables/movies/id-001?ZUMO-API-VERSION=3.0.1", null)]
        [InlineData("tables/movies/id-001", "3.0")]
        [InlineData("tables/movies/id-001", "3.0.0")]
        [InlineData("tables/movies/id-001", "3.0.1")]
        public async Task ZumoVersion_V3_0_OK(string relativeUri, string headerValue)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://localhost/{relativeUri}")
            };
            if (headerValue != null)
            {
                request.Headers.Add("ZUMO-API-VERSION", headerValue);
            }

            var response = await MovieHttpClient.SendAsync(request).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
