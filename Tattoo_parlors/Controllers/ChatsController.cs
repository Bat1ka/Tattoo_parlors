using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tattoo_parlors.Data;
using Tattoo_parlors.Models;

namespace Tattoo_parlors.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class ChatsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ChatsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Chat(int? selectedUserId)
        {
            var currentUserName = User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(u => u.email == currentUserName);
            if (currentUser == null)
            {
                return Forbid();
            }

            // Начинаем запрос с общих сообщений
            IQueryable<MessageModel> query = _context.Message
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.timestamp);

            if (currentUser.role == 0)
            {
                // Пользователь видит только свои сообщения (отправленные или полученные)
                query = query.Where(m => m.senderId == currentUser.id || m.receiverId == currentUser.id);
            }
            else if (currentUser.role == 1)
            {
                if (selectedUserId.HasValue)
                {
                    // Администратор видит переписку только с выбранным пользователем.
                    query = query.Where(m =>
                        (m.senderId == selectedUserId.Value && m.receiverId == currentUser.id) ||
                        (m.senderId == currentUser.id && m.receiverId == selectedUserId.Value));
                }
                else
                {
                    // Если администратор не выбрал пользователя, то список сообщений оставляем пустым.
                    query = query.Where(m => false);
                }
            }

            var messages = query.ToList();

            // Дешифрование сообщений
            foreach (var msg in messages)
            {
                try
                {
                    msg.content = EncryptionHelper.Decrypt(msg.content);
                }
                catch (Exception ex)
                {
                    msg.content = "Ошибка расшифровки сообщения.";
                }
            }

            var viewModel = new ChatViewModel
            {
                Messages = messages,
                CurrentUserRole = currentUser.role,
                SelectedUserId = selectedUserId,
                CurrentUserId = currentUser.id
            };

            // Для администратора заполнить список пользователей, с которыми может вестись чат
            if (currentUser.role == 1)
            {
                viewModel.Users = _context.Users.Where(u => u.role == 0).ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SendMessage(ChatViewModel model)
        {
            var currentUserName = User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(u => u.email == currentUserName);
            if (currentUser == null)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(model.MessageContent))
            {
                ModelState.AddModelError("", "Сообщение не может быть пустым");
                // Для администратора передаём выбранного пользователя, чтобы сохранить фильтрацию диалога
                return currentUser.role == 1
                    ? RedirectToAction("Chat", new { selectedUserId = model.SelectedUserId })
                    : RedirectToAction("Chat");
            }

            var message = new MessageModel
            {
                content = EncryptionHelper.Encrypt(model.MessageContent),
                senderId = currentUser.id,
                timestamp = DateTime.Now
            };

            if (currentUser.role == 0)
            {
                // Если обычный пользователь – адресуем сообщение администратору.
                var admin = _context.Users.FirstOrDefault(u => u.role == 1);
                if (admin != null)
                {
                    message.receiverId = admin.id;
                }
            }
            else if (currentUser.role == 1)
            {
                if (model.SelectedUserId.HasValue)
                {
                    // Администратор отправляет сообщение выбранному пользователю
                    message.receiverId = model.SelectedUserId.Value;
                }
                else
                {
                    ModelState.AddModelError("", "Пожалуйста, выберите пользователя, которому хотите отправить сообщение.");
                    var users = _context.Users.Where(u => u.role == 0).ToList();
                    var messages = _context.Message
                        .Include(m => m.Sender)
                        .Include(m => m.Receiver)
                        .OrderBy(m => m.timestamp)
                        .ToList();
                    var viewModel = new ChatViewModel
                    {
                        Messages = messages,
                        Users = users,
                        CurrentUserRole = currentUser.role
                    };
                    return View("Chat", viewModel);
                }
            }

            _context.Message.Add(message);
            _context.SaveChanges();

            // После отправки сообщения, для администратора сохраняется выбранный пользователь
            return currentUser.role == 1
                ? RedirectToAction("Chat", new { selectedUserId = model.SelectedUserId })
                : RedirectToAction("Chat");
        }
    }

    // Класс для шифрования и дешифрования сообщений остается без изменений
    public static class EncryptionHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdef9876543210");

        public static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                        swEncrypt.Flush();
                        csEncrypt.FlushFinalBlock();
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}