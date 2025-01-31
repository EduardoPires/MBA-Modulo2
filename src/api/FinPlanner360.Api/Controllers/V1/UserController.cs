﻿using FinPlanner360.Api.Settings;
using FinPlanner360.Api.ViewModels.Report;
using FinPlanner360.Api.ViewModels.User;
using FinPlanner360.Business.Interfaces.Repositories;
using FinPlanner360.Business.Interfaces.Services;
using FinPlanner360.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;

namespace FinPlanner360.Api.Controllers.V1;

[Authorize(Roles = "USER")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[Controller]")]
public class UserController : MainController
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly AppSettings _appSettings;
    private readonly ILogger _logger;

    public UserController(ILogger<UserController> logger,
                          SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          IOptions<AppSettings> appSettings,
                          IUserRepository userRepository,
                          IAppIdentityUser appIdentityUser,
                          INotificationService notificationService) : base(appIdentityUser, notificationService)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _userRepository = userRepository;
        _appSettings = appSettings.Value;
    }


    /// <summary>
    /// Registra um novo usuário.
    /// </summary>
    /// <remarks>Cria um novo usuário com os dados fornecidos e retorna um token JWT.</remarks>
    /// <response code="200">Sucesso na operação!</response>
    /// <response code="400">Dados inconsistentes na requisição ao criar um novo usuário.</response>
    /// <response code="500">Erro interno de servidor.</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(LoginOutputViewModel), 200)]
    public async Task<ActionResult> RegisterAsync(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid) return GenerateResponse(ModelState);

        var identitiyUser = new IdentityUser
        {
            UserName = registerViewModel.Email,
            Email = registerViewModel.Email,
            EmailConfirmed = true
        };

        var registerResult = await _userManager.CreateAsync(identitiyUser, registerViewModel.Password);

        if (registerResult.Succeeded)
        {
            var user = new User
            {
                UserId = Guid.Parse(identitiyUser.Id),
                Email = registerViewModel.Email,
                Name = registerViewModel.Name,
                AuthenticationId = Guid.Parse(identitiyUser.Id)
            };

            try
            {
                await _userRepository.CreateAsync(user);

                var loginOutput = new LoginOutputViewModel
                {
                    Id = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    AccessToken = await GenerateJwt(user.Email)
                };

                return GenerateResponse(loginOutput);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(identitiyUser);

                _logger.LogError(ex, $"Erro ao tentar criar o usuário na base de dados: {ex.Message}");
                Notify("Não foi possível criar o usuário, tente novamente mais tarde.");
            }
        }
        else
        {
            foreach (var error in registerResult.Errors)
            {
                Notify(error.Description);
            }
        }

        return GenerateResponse();
    }


    /// <summary>
    /// Realiza o login do usuário.
    /// </summary>
    /// <remarks>Autentica o usuário e retorna um token JWT.</remarks>
    /// <response code="200">Sucesso na operação!</response>
    /// <response code="400">Dados inconsistentes na requisição ao logar o usuário.</response>
    /// <response code="500">Erro interno de servidor.</response>
    [AllowAnonymous]
    [HttpPost("login")]    
    [ProducesResponseType(typeof(LoginOutputViewModel), 200)]    
    public async Task<ActionResult> LoginAsync(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return GenerateResponse(ModelState);

        var loginResult = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, false, true);

        if (loginResult.Succeeded)
        {
            var user = await _userRepository.GetByEmailAsync(loginViewModel.Email);

            var loginOutput = new LoginOutputViewModel
            {
                Id = user.UserId,
                Name = user.Name,
                Email = user.Email,
                AccessToken = await GenerateJwt(user.Email)
            };

            return GenerateResponse(loginOutput);
        }
        else
        {
            Notify("Usuário não localizado com as credenciais informadas");
        }

        return GenerateResponse();
    }

    /// <summary>
    /// Exporta um relatório de usuários.
    /// </summary>
    /// <remarks>Gera e exporta um relatório contendo informações dos usuários em formato PDF ou XLSX. O tipo de arquivo deve ser especificado no parâmetro `tipoArquivo`.</remarks>
    /// <param name="tipoArquivo">O tipo de arquivo deve ser especificado como PDF ou XLSX</param>
    /// <response code="200">Sucesso na operação!</response>
    /// <response code="204">Sucesso na operação, porém sem conteúdo de resposta.</response>
    /// <response code="400">Dados inconsistentes na requisição ao listar os usuários.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="500">Erro interno de servidor.</response>
    [AllowAnonymous]
    [HttpGet("export-report")]
    [SwaggerOperation(Tags = new[] { "Transações" })]
    [ProducesResponseType(typeof(TransactionReportViewModel), 200)]
    public async Task<IActionResult> ExportReport(
       [Required(ErrorMessage = "O arquivo deve ser informado como tipo PDF ou XLSX")]
           [RegularExpression(@"^(pdf|Pdf|PDF|xlsx|Xlsx|XLSX)$", ErrorMessage = "O arquivo deve ser do tipo PDF ou XLSX.")] string tipoArquivo)
    {
    
        var users = (await _userRepository.FilterAsync(p => true)).Select(p => new
        {
            p.UserId,
            p.Name,
            p.Email,
        }).ToArray();

        byte[] fileBytes;
        string contentType;
        string fileName;

        switch (tipoArquivo.ToLower())
        {
            case "pdf":
                fileBytes = Reports.Fast.ReportService.GenerateReportPDF("Users", users);
                contentType = "application/pdf";
                fileName = "Usuarios.pdf";
                break;

            case "xlsx":
                fileBytes = Reports.Closed_Xml.ReportService.GenerateXlsxBytes("Categoria", users);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileName = "Usuarios.xlsx";
                break;

            default:
                return BadRequest("Tipo de arquivo inválido. Use 'pdf' ou 'xlsx'.");
        }

        return File(fileBytes, contentType, fileName);
    }


    private async Task<string> GenerateJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (ClaimTypes.Name, user.UserName),
            new (ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JwtSettings.Secret);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _appSettings.JwtSettings.Issuer,
            Audience = _appSettings.JwtSettings.Audience,
            Expires = DateTime.UtcNow.AddHours(_appSettings.JwtSettings.ExpirationInHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var encodedToken = tokenHandler.WriteToken(token);

        return encodedToken;
    }
}