using Microsoft.AspNetCore.Mvc;
using DLForum.Models;
using System.Diagnostics;

namespace DLForum.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            switch (statusCode)
            {
                case 404:
                    errorViewModel.Message = "Страница не найдена. Возможно, она была удалена или перемещена.";
                    break;
                case 500:
                    errorViewModel.Message = "Внутренняя ошибка сервера. Мы уже работаем над её устранением.";
                    break;
                case 403:
                    errorViewModel.Message = "Доступ запрещен. У вас нет прав для просмотра этой страницы.";
                    break;
                case 401:
                    errorViewModel.Message = "Требуется авторизация для доступа к этой странице.";
                    break;
                case 503:
                    errorViewModel.Message = "Сервис временно недоступен. Пожалуйста, попробуйте позже.";
                    break;
                default:
                    errorViewModel.Message = "Произошла непредвиденная ошибка. Мы уже работаем над её устранением.";
                    break;
            }

            return View("Error", errorViewModel);
        }

        [Route("Error/Database")]
        public IActionResult DatabaseError()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = "Ошибка подключения к базе данных. Пожалуйста, попробуйте позже."
            };

            return View("Error", errorViewModel);
        }

        [Route("Error/Timeout")]
        public IActionResult TimeoutError()
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = "Превышено время ожидания ответа от сервера. Пожалуйста, попробуйте позже."
            };

            return View("Error", errorViewModel);
        }
    }
} 