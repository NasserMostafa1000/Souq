using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using StoreBusinessLayer.Interfaces;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;

namespace StoreBusinessLayer.Users
{
    public class GoogleLogin : ILogin
    {
        private readonly AppDbcontext _context;

        public GoogleLogin(AppDbcontext context)
        {
            _context = context;
        }

        public async Task<StoreDataAccessLayer.Entities.Users> LoginWithProviderAsync(string email ,string token, string password ) 
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new Exception("هناك خطأ في المصادقه مع جوجل يرجي المحاوله مره اخري بعد ثواني");
            }
            try
            {
                var payload = await ValidateGoogleTokenAsync(token);
                if (payload == null)
                {
                    throw new Exception("خطأ في رمز جوجل");
                }
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailOrAuthId == payload.Email);
                  //if User Exists Return it

                if (user != null)
                {
                    return user;
                }
                //if does not, create new user and return it
                else
                {
                    var newUser = new StoreDataAccessLayer.Entities.Users
                    {
                        EmailOrAuthId = payload.Email, 
                        AuthProvider = "Google", 
                        //User Role=3
                        RoleId=3
                    };
                    await _context.Users.AddAsync(newUser);
                    await _context.SaveChangesAsync();

                    var newClient = new Client
                    {
                        FirstName = payload.Name.Split(" ")[0],
                        SecondName = payload.Name.Split(" ")[1],
                        UserId = newUser.UserId
                    };

                    await _context.Clients.AddAsync(newClient);
                    await _context.SaveChangesAsync();
                    string message = $@"
مرحبًا {newClient.FirstName} {newClient.SecondName},

لقد تم إنشاء حسابك بنجاح في سوق البلد. يمكنك الآن الوصول إلى جميع خدماتنا باستخدام بريدك الإلكتروني: {newClient.User.EmailOrAuthId}.

لا تتردد في الاتصال بنا إذا كنت بحاجة إلى أي مساعدة.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                    await NotificationServices.NotificationsCreator.SendNotification(
                        "تم إنشاء حسابك بنجاح",  // العنوان
                        message,  // الرسالة الفعلية
                        newClient.User.EmailOrAuthId,  // البريد الإلكتروني الخاص بالعميل
                        "gmail"  // مزود الإشعار (يمكن تغييره حسب الخدمة المستخدمة)
                    );



                    return newUser;
                }
            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء
                  throw new Exception( ex.Message.ToString());
                
            }
        }
        private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { "1002692311708-dv44b5us60jlovbgdcv87rbuvgfs01vo.apps.googleusercontent.com" } // ضع هنا clientId الخاص بك من جوجل
                };

                // تحقق من التوكن
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return payload;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
     
    }
}
