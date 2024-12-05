using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using P3AddNewFunctionalityDotNetCore.Data;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class TestControllerTests
    {
        public class LambazonControllerTests
        {
            [Fact]
            public async Task GET_retrieves_Product_page()
            {
                await using var application = new WebApplicationFactory<Program>();
                using var client = application.CreateClient();

                var response = await client.GetAsync("/Product");
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                
            }

        }
    }
}
    