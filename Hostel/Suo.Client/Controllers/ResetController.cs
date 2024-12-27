using JWTCoding;
using Microsoft.AspNetCore.Mvc;
using Suo.Client.Data.RabbitMqService;
using Suo.Client.Data.Services;
using Suo.Client.Extentions;
using Suo.Client.Models;

namespace Suo.Client.Controllers
{
    public class ResetController : Controller
    {
        public static string key = "8asdq728das412zxcq14asd";

        public UserService UserService;
        public RabbitService RabbitService;

        private readonly AppConfiguration _config;
        public ErrorConfirm errorConfirm = new ErrorConfirm();
        public ResetController(UserService userService, AppConfiguration config, RabbitService rabbitService) 
        {
            UserService = userService;
            RabbitService = rabbitService;
            _config = config;
        }

        public IActionResult Index()
        {
            errorConfirm.TelegramLink = _config.TelegramLink;
            return View("Index", errorConfirm);
        }

        [HttpPost]
        public async Task<IActionResult> Reset(string phonenumber)
        {

            if (phonenumber != null)
            {
                if ((await UserService.Check(phonenumber)) == null)
                {
                    errorConfirm.Message = "Пользователь не найден, проверьте ваш номер телефона или пройдите регистрацию.";
                    return View("Index", errorConfirm);
                }
                else
                {
                    //Сюда надо добавить отправку сообщения
                    var users = await UserService.Check(phonenumber);
                    var teleusers = UserService.UserInfo(users.UserId);

                    var token = (new Token()).GenerateToken(users.UserId);
                    token = Token.Encrypt(token, key);
                    var link = $"Перейдите по ссылке для восстановления пароля: ⬇⬇⬇ \n{AppConfigGlobals.IdentityServer}/reset/resetpassword/reset/{token}";
                    await RabbitService.SendMessageToTgBot(new MessageModelForTg() { TelegrammUserId = int.Parse(teleusers.TelegramUserId), Message = link });

                    return Redirect("https://t.me/SUO1_Bot");
                }
            }
            else
            {
                errorConfirm.Message = "Введите номер телефона.";

                return View("Index", errorConfirm);
            }
        }
    }
}
