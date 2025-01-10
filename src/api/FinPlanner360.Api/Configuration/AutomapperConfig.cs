﻿using AutoMapper;
using FinPlanner360.Api.ViewModels.Budget;
using FinPlanner360.Api.ViewModels.Category;
using FinPlanner360.Api.ViewModels.GeneralBudget;
using FinPlanner360.Api.ViewModels.Transaction;
using FinPlanner360.Business.Models;

namespace FinPlanner360.Api.Configuration;

public class AutomapperConfig : Profile
{
    public AutomapperConfig()
    {
        CreateMap<Category, CategoryViewModel>().ReverseMap();
        CreateMap<Category, CategoryUpdateViewModel>().ReverseMap();
        
        CreateMap<BudgetViewModel,Budget>();
        CreateMap<Budget, BudgetUpdateViewModel>().ReverseMap();
        CreateMap<Budget, BudgetViewModel>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(source => source.Category.Description));

        CreateMap<GeneralBudget, GeneralBudgetViewModel>().ReverseMap();

        CreateMap<Transaction, TransactionViewModel>().ReverseMap();
        CreateMap<Transaction, TransactionUpdateViewModel>().ReverseMap();
    }
}