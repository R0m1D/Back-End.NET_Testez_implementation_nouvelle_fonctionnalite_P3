using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using P3AddNewFunctionalityDotNetCore.Controllers;
using System.Threading.Tasks;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Microsoft.AspNetCore.Http;

public class AccountControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        // Mock UserManager
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        // Mock SignInManager
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        _signInManagerMock = new Mock<SignInManager<IdentityUser>>(
            _userManagerMock.Object,
            httpContextAccessorMock.Object,
            userClaimsPrincipalFactoryMock.Object,
            null,
            null,
            null,
            null
        );

        // Create Controller
        _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldRedirectToReturnUrl()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Name = "validUser",
            Password = "validPassword",
            ReturnUrl = "/Admin/Index"
        };

        var user = new IdentityUser { UserName = loginModel.Name };

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginModel.Name))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(sm => sm.PasswordSignInAsync(user, loginModel.Password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Be(loginModel.ReturnUrl);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturnViewWithError()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Name = "invalidUser",
            Password = "invalidPassword"
        };

        _userManagerMock
            .Setup(um => um.FindByNameAsync(loginModel.Name))
            .ReturnsAsync((IdentityUser)null); // User not found

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(loginModel);
        _controller.ModelState[""].Errors.Should().Contain(e => e.ErrorMessage == "Invalid name or password");
    }

    [Fact]
    public async Task Login_InvalidModelState_ShouldReturnView()
    {
        // Arrange
        var loginModel = new LoginModel();
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.Login(loginModel);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(loginModel);
    }
}
