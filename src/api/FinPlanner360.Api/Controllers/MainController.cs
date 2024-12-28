﻿using FinPlanner360.Busines.Interfaces.Services;
using FinPlanner360.Busines.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FinPlanner360.Api.Controllers
{
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public Guid UserId { get; set; }
        public bool IsAuthenticated { get; set; }

        protected MainController(INotificationService notificationService)
        {
            _notificationService = notificationService;

            //if (appUser.IsAuthenticated())
            //{
            //    UsuarioId = appUser.GetUserId();
            //    UsuarioAutenticado = true;
            //}
        }

        //protected bool OperacaoValida()
        //{
        //    return !_notificador.TemNotificacao();
        //}

        protected ActionResult GenerateResponse(object result = null, int statusCode = StatusCodes.Status200OK)
        {
            if (!_notificationService.HasNotification())
            {
                return new JsonResult(result)
                {
                    StatusCode = statusCode, 
                    Value = new
                    {
                        success = true, 
                        result = result
                    }
                };
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificationService.GetNotifications().Select(n => n.Message)
            });
        }

        protected void Notify(string message)
        {
            _notificationService.Handle(new Notification(message));
        }

        //protected ActionResult CustomResponse(ModelStateDictionary modelState)
        //{
        //    if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
        //    return CustomResponse();
        //}

        //protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        //{
        //    var erros = modelState.Values.SelectMany(e => e.Errors);
        //    foreach (var erro in erros)
        //    {
        //        var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
        //        NotificarErro(errorMsg);
        //    }
        //}

        //protected void NotificarErro(string mensagem)
        //{
        //    _notificador.Handle(new Notificacao(mensagem));
        //}
    }
}
