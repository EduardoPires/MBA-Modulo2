﻿using FinPlanner360.Api.ViewModels.Dashboard;
using FinPlanner360.Business.Interfaces.Repositories;
using FinPlanner360.Business.Interfaces.Services;
using FinPlanner360.Business.Models;
using FinPlanner360.Business.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace FinPlanner360.Api.Controllers.V1
{
    [Authorize(Roles = "USER")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[Controller]")]
    public class DashboardController : MainController
    {
        private readonly ITransaction_Repository _transactionRepository;
        public DashboardController(ITransaction_Repository transactionRepository,
            IAppIdentityUser appIdentityUser,
            INotificationService notificationService) : base(appIdentityUser, notificationService)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet("Cards/{date:datetime?}")]
        [SwaggerOperation(Summary = "Cards de dashboard", Description = "Retorna informações financeiras resumidas do usuário")]
        [ProducesResponseType(typeof(List<CardSumaryViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CardSumaryViewModel>> GetCards(DateTime? date)
        {
            date = date.HasValue && date.Value != DateTime.MinValue && date.Value != DateTime.MaxValue 
                ? date.Value 
                : DateTime.Now;
            DateTime startDate = new DateTime(date.Value.Year, date.Value.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddSeconds(-1);

            ICollection<Transaction> transactions = await _transactionRepository.GetTransactionsByRangeAsync(startDate, endDate);

            if (transactions == null) { return GenerateResponse(null, HttpStatusCode.NotFound); }

            CardSumaryViewModel cardSumary = new CardSumaryViewModel
            {
                TotalIncome = transactions.Where(x => x.TransactionDate < date && x.Type == TransactionTypeEnum.Income).Sum(x => x.Amount),
                TotalExpense = transactions.Where(x => x.TransactionDate < date && x.Type == TransactionTypeEnum.Expense).Sum(x => x.Amount),
                TotalBalance = transactions.Where(x => x.TransactionDate < date).Sum(x => x.Type == TransactionTypeEnum.Expense ? (x.Amount * -1) : x.Amount),
                TotalIncomeToday = transactions.Where(x => x.TransactionDate == date && x.Type == TransactionTypeEnum.Income).Sum(x => x.Amount),
                TotalExpenseToday = transactions.Where(x => x.TransactionDate == date && x.Type == TransactionTypeEnum.Expense).Sum(x => x.Amount),
                FutureTotalIncome = transactions.Where(x => x.TransactionDate > date && x.Type == TransactionTypeEnum.Income).Sum(x => x.Amount),
                FutureTotalExpense = transactions.Where(x => x.TransactionDate > date && x.Type == TransactionTypeEnum.Expense).Sum(x => x.Amount)
            };

            return GenerateResponse(cardSumary, HttpStatusCode.OK);
        }
    }
}
