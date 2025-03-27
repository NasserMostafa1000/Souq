using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreBusinessLayer.Users;
using StoreDataAccessLayer.Entities;
using StoreDataAccessLayer;
using static StoreBusinessLayer.Clients.ClientsDtos;
using StoreBusinessLayer.Orders;

namespace StoreBusinessLayer.Clients
{
    public class ClientsBL
    {
        public AppDbcontext _DbContext;
        public UsersBL _UsersBL;

        public ClientsBL(AppDbcontext Context, UsersBL Users)
        {
            _DbContext = Context;
            _UsersBL = Users;
        }
        public async Task SendNotificationToUser(int userId, string message)
        {
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null && !string.IsNullOrEmpty(user.EmailOrAuthId))
            {
                // إرسال إشعار عبر البريد الإلكتروني أو أي وسيلة أخرى.
                await NotificationServices.NotificationsCreator.SendNotification(
                    "تحديث في حسابك - سوق البلد", // العنوان
                    message, // الرسالة المخصصة
                    user.EmailOrAuthId, // البريد الإلكتروني أو معرّف المصادقة
                    "Gmail" // يمكنك استبدال هذه الوسيلة حسب الحاجة
                );
            }
        }
        public async Task<int> AddNewClient(ClientsDtos.PostClientReq Dto)
        {
            try
            {
                int UserId = await _UsersBL.PostUser(
                    new UsersDtos.PostUserReq
                    {
                        EmailOrAuthId = Dto.Email,
                        Password = Dto.Password,
                        AuthProvider = "Online Store"
                    }, 3); // user role id => 3 (client role)

                Client NewClient = new Client
                {
                    FirstName = Dto.FirstName,
                    SecondName = Dto.SecondName,
                    PhoneNumber = Dto.PhoneNumber,
                    UserId = UserId
                };

                await _DbContext.Clients.AddAsync(NewClient);
                await _DbContext.SaveChangesAsync();

                // إرسال إشعار للعميل عند إضافة حساب جديد
                string message = $"مرحبًا {Dto.FirstName} {Dto.SecondName},\n\nلقد تم إنشاء حسابك بنجاح في سوق البلد. يمكنك الآن الوصول إلى جميع خدماتنا باستخدام بريدك الإلكتروني: {Dto.Email}.\n\nلا تتردد في الاتصال بنا إذا كنت بحاجة إلى أي مساعدة.";

                await SendNotificationToUser(UserId, message);

                return NewClient.ClientId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public async Task<bool> GetClientName(string FirstName, string LastName, int ClientId)
        {
            var Client = await _DbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == ClientId);
            if (Client != null)
            {
                Client.FirstName = FirstName;
                Client.SecondName = LastName;
                _DbContext.Clients.Update(Client);
                await _DbContext.SaveChangesAsync();

                // إرسال إشعار عند تحديث الاسم
                string message = $"مرحبًا {FirstName} {LastName},\n\nلقد تم تحديث اسمك في حسابك في سوق البلد بنجاح.\n\nإذا كنت بحاجة إلى أي مساعدة، لا تتردد في الاتصال بنا.";

                await SendNotificationToUser(Client.UserId, message);

                return true;
            }
            return false;
        }
        public async Task<bool> AddOrUpdatePhoneToClientById(int id, string phoneNumber)
        {
            try
            {
                var targetClient = await _DbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
                if (targetClient == null)
                    return false;
                targetClient.PhoneNumber = phoneNumber;
                _DbContext.Update(targetClient);
                await _DbContext.SaveChangesAsync();
                string message = $"مرحبًا {targetClient.FirstName} {targetClient.SecondName},\n\nلقد تم تحديث رقم الهاتف الخاص بك في حسابك في سوق البلد إلى: {phoneNumber}.\n\nإذا كنت بحاجة إلى أي مساعدة، لا تتردد في الاتصال بنا.";

                await SendNotificationToUser(targetClient.UserId, message);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<int> AddNewAddress(ClientsDtos.PostAddressReq req, int ClientId)
        {
            var Address = new Address
            {
                Governorate = req.Governorate,
                St = req.street,
                City = req.City,
                ClientId = ClientId,
            };

            await _DbContext.Addresses.AddAsync(Address);
            await _DbContext.SaveChangesAsync();
            var client = await _DbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == ClientId);
            if (client != null)
            {
                string message = $"مرحبًا {client.FirstName} {client.SecondName},\n\nلقد تم إضافة عنوان جديد إلى حسابك في سوق البلد: \n\nمحافظة: {req.Governorate}\nمدينة: {req.City}\nالشارع: {req.street}\n\nإذا كنت بحاجة إلى تعديل أو حذف هذا العنوان، يمكنك القيام بذلك من خلال حسابك.";

                await SendNotificationToUser(client.UserId, message);
            }

            return Address.AddressId;
        }
        public async Task<bool> UpdateClientEmail(int ClientId, string newEmail)
        {
            var client = await _DbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == ClientId);
            if (client != null)
            {
                client.User.EmailOrAuthId = newEmail;
                _DbContext.Clients.Update(client);
                await _DbContext.SaveChangesAsync();
                string message = $"مرحبًا {client.FirstName} {client.SecondName},\n\nلقد تم تحديث البريد الإلكتروني الخاص بحسابك في سوق البلد إلى: {newEmail}.\n\nإذا كنت بحاجة إلى أي مساعدة، لا تتردد في الاتصال بنا.";
                await SendNotificationToUser(client.UserId, message);

                return true;
            }
            return false;
        }
        public async Task<int> GetClientIdByUserId(int UserId)
        {
            var client = await _DbContext.Clients.FirstOrDefaultAsync(Client => Client.UserId == UserId);
            return client!.ClientId;
        }
        public async Task<bool> AddGeneralManagerOrShippingManager(string FirstName, string LastName, int UserId)
        {
            await _DbContext.Clients.AddAsync(new Client { FirstName = FirstName, SecondName = LastName, UserId = UserId });
            int RowsAffected = await _DbContext.SaveChangesAsync();
            return RowsAffected > 0;
        }

        public async Task<string?> GetClientPhoneNumberById(int ClientId)
        {
            var client = await _DbContext.Clients.FirstOrDefaultAsync(Client => Client.ClientId == ClientId);
            return client!.PhoneNumber;
        }
        public async Task<Dictionary<int, string>> GetClientAddresses(int ClientId)
        {
            Dictionary<int, string> AddressesDic = await _DbContext.Addresses
                .Where(ad => ad.ClientId == ClientId)
                .ToDictionaryAsync(ad => ad.AddressId,
                    ad => ad.Governorate + "-" + " مدينه " + ad.City + "  شارع " + ad.St);

            return AddressesDic ?? new Dictionary<int, string>();
        }

        // دالة للحصول على بيانات العميل
        public async Task<ClientsDtos.GetClientReq> GetClientById(int ClientId)
        {
            var Client = await _DbContext.Clients.Where(c => c.ClientId == ClientId).Include(c => c.Addresses).FirstOrDefaultAsync();
            if (Client != null)
            {
                return new ClientsDtos.GetClientReq
                {
                    FirstName = Client.FirstName,
                    LastName = Client.SecondName,
                    ClientAddresses = await GetClientAddresses(ClientId),
                    PhoneNumber = Client.PhoneNumber
                };
            }
            return null!;
        }

        // دالة لاسترجاع العملاء
        public async Task<List<GetClientsReq>> GetClientsAsync(int PageNum)
        {
            var clients = await _DbContext.Clients.Include(C => C.User).Where(C => C.User.RoleId == 3)
                .Select(c => new GetClientsReq
                {
                    FullName = c.FirstName + " " + c.SecondName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.User.EmailOrAuthId,
                    Password = c.User.PasswordHash, // تأكد من ضرورة إرجاع كلمة المرور وفقًا لمتطلبات الأمان
                    AuthProvider = c.User.AuthProvider
                })
                .Paginate(PageNum).ToListAsync();

            return clients ?? new List<GetClientsReq>();
        }
    }
}
