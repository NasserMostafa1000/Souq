using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Entities;

namespace StoreBusinessLayer.Orders
{
    public class OrdersBL
    {
      private readonly  AppDbcontext _Context;

        public OrdersBL(AppDbcontext context)
        {
            _Context = context;
        }
        public async Task<int> PostOrder(OrdersDtos.ClientOrders.PostOrderReq req, int ClientId)
        {
            try
            {
                // إنشاء الطلب الجديد
                var newOrder = new StoreDataAccessLayer.Entities.Orders
                {
                    ClientId = ClientId,
                    // الحالة الافتراضية قيد المعالجة
                    OrderStatusId = 1,
                    PaymentMethodId = req.PaymentMethodId,
                    ShippingCoast = req.ShippingCoast,
                    TotalAmount = req.TotalPrice,
                    // قد يكون null إذا كانت طريقة الدفع "عند استلام المنتج"
                    TransactionNumber = req.TransactionNumber,
                    Address = req.Address,
                };

                // إضافة الطلب إلى قاعدة البيانات
                await _Context.Orders.AddAsync(newOrder);
                await _Context.SaveChangesAsync();

                // إرسال رسالة شكر مع تعليمات متابعة الطلب
                var client = await _Context.Clients
                    .Include(c => c.User) // التأكد من تحميل الـ User مع العميل
                    .FirstOrDefaultAsync(c => c.ClientId == ClientId);
                if (client != null)
                {
                    string customerName = client.FirstName;
                    string orderNumber = newOrder.OrderId.ToString();
                    string message = $@"
عزيزي {customerName}،

شكراً لك على طلبك من سوق البلد! تم استلام طلبك بنجاح ونحن بصدد معالجته.
رقم طلبك هو: {orderNumber}.

يمكنك متابعة حالة طلبك عبر حسابك الشخصي على موقعنا الإلكتروني.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                    // التحقق من البريد الإلكتروني وإرسال الإشعار
                    var clientEmail = client.User?.EmailOrAuthId; // التأكد من الحصول على البريد الإلكتروني أو معرّف المصادقة
                    if (!string.IsNullOrEmpty(clientEmail))
                    {
                        // إرسال الرسالة إلى العميل
                        await NotificationServices.NotificationsCreator.SendNotification(
                            "شكرًا لطلبك في سوق البلد",
                            message,
                            clientEmail,  // البريد الإلكتروني الفعلي
                            "gmail");  // يمكنك استبدال "Gmail" بأي مزود إشعار آخر إن كان مختلفًا
                    }
                }

                return newOrder.OrderId;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public async Task<int>PostOrderDetail(OrdersDtos.ClientOrders.PostOrderDetailsReq req)
        {
            try
            {
            var newOrderDetails = new OrderDetails
            {
                ProductDetailsId = req.ProductDetailsId,
                UnitPrice=req.UnitPrice,
                Quantity=req.Quantity,
                OrderId=req.OrderId

            };
             await  _Context.OrderDetails.AddAsync(newOrderDetails);
                await _Context.SaveChangesAsync();
                return newOrderDetails.OrderDetailsId;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }
        private List<OrderDetails> PostListOfOrderDetails(List<OrdersDtos.ClientOrders.PostOrderDetailsReq> details,int OrderId)
        {
            return details.Select(detail => new OrderDetails
            {
                ProductDetailsId = detail.ProductDetailsId,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                OrderId= OrderId,
            }).ToList();
        }
        public async Task<bool> PostListOfOrdersDetails(List<OrdersDtos.ClientOrders.PostOrderDetailsReq> req,int OrderId)
        {
           
            try
            {
                List<OrderDetails> Details = PostListOfOrderDetails(req, OrderId);
                await _Context.OrderDetails.AddRangeAsync(Details);
               int RowsAffected= await _Context.SaveChangesAsync();
                return RowsAffected > 0;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }
        public async Task<string>GetOrderStatusNameById(int ID)
        {
            var OrderStatus = await _Context.OrderStatus.FirstOrDefaultAsync(Os => Os.OrderStatusId == ID);
            if(OrderStatus!=null)
            {
                return OrderStatus.StatusName;
            }

            return "";
        }
        public async Task<List<OrdersDtos.ClientOrders.GetOrdersByClientIdReq>> GetOrdersByClientId(int ClientId)
        {
            var orders = await _Context.Orders
                .Where(O => O.ClientId == ClientId).OrderByDescending(O=>O.OrderId)
                .Select(O => new OrdersDtos.ClientOrders.GetOrdersByClientIdReq
                {
                    OrderId = O.OrderId,
                    TotalAmount = O.TotalAmount+O.ShippingCoast,
                    OrderStatus = O.OrderStatusId.ToString(), 
                    ShippingCoast=O.ShippingCoast,
                    RejectionReason = O.RejectionReason,
                    OrderDate = O.CreatedAt
                })
                .ToListAsync(); 

            foreach (var order in orders)
            {
                order.OrderStatus = await GetOrderStatusNameById(int.Parse(order.OrderStatus));
            }

            return orders;
        }

        public async Task<List<OrdersDtos.ClientOrders.GetOrderDetailsInSpecificOrderReq>> GetOrderDetailsInSpecificOrder(int OrderId)
        {
            var orderDetailsList = await _Context.OrderDetails
                .Where(detail => detail.OrderId == OrderId)
                .Include(detail => detail.ProductDetails)
                    .ThenInclude(pd => pd.Product)
                .Include(detail => detail.ProductDetails)
                    .ThenInclude(pd => pd.Color)
                .Include(detail => detail.ProductDetails)
                    .ThenInclude(pd => pd.Size)
                .Select(detail => new OrdersDtos.ClientOrders.GetOrderDetailsInSpecificOrderReq
                {
                    ProductId = detail.ProductDetails.ProductId,
                    ProductName = detail.ProductDetails.Product.ProductName,
                    ImagePath = detail.ProductDetails.ProductImage,
                    Quantity = detail.Quantity,
                    ColorName = detail.ProductDetails.Color.ColorName,
                    SizeName = detail.ProductDetails.Size!.SizeName,
                    UnitPrice = detail.UnitPrice,
                    TotalAmount = detail.Quantity * detail.UnitPrice
                })
                .ToListAsync();

            return orderDetailsList;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        //                                                    Admin Section
        //-------------------------------------------------------------------------------------------------------------------------
        public async Task<List<OrdersDtos.AdminOrders.GetOrdersReq>>GetOrders(int PageNumber)
        {
            var orders = await _Context.Orders
        .OrderByDescending(O => O.OrderId)
        .Include(O => O.Client)
        .Include(O => O.OrderStatus)
        .Include(O => O.PaymentMethod)
        .Select(O => new OrdersDtos.AdminOrders.GetOrdersReq
        {
            OrderId = O.OrderId,
            CreatedAt = O.CreatedAt,
            TotalAmount = O.TotalAmount,
            PaymentMethod = O.PaymentMethod.Method,
            TransactionNumber = O.TransactionNumber,
            ShippingCoast = O.ShippingCoast,
            RejectionReason = O.RejectionReason,
            Address = O.Address!,
            FullName = O.Client.FirstName + " " + O.Client.SecondName,
            ClientPhone = O.Client.PhoneNumber!,
            OrderStatus = O.OrderStatus.StatusName
        })
        .Paginate(PageNumber)  
        .ToListAsync();
            if(orders!=null)
            {
             return orders!;
            }
            return new List<OrdersDtos.AdminOrders.GetOrdersReq>();

        }

        public async Task<OrdersDtos.AdminOrders.GetOrdersReq?> FindOrder(int OrderId)
        {
            var order = await _Context.Orders
                .Include(O => O.Client)
                .Include(O => O.OrderStatus)
                .Include(O => O.PaymentMethod)
                .Where(O => O.OrderId == OrderId)
                .Select(O => new OrdersDtos.AdminOrders.GetOrdersReq
                {
                    OrderId = O.OrderId,
                    CreatedAt = O.CreatedAt,
                    TotalAmount = O.TotalAmount,
                    PaymentMethod = O.PaymentMethod.Method,
                    TransactionNumber = O.TransactionNumber,
                    ShippingCoast=O.ShippingCoast,
                    Address = O.Address!,
                    FullName = O.Client.FirstName + " " + O.Client.SecondName,
                    ClientPhone = O.Client.PhoneNumber!,
                    OrderStatus = O.OrderStatus.StatusName
                })
                .FirstOrDefaultAsync();

            return order;
        }
        private  int GetOrderStatusId(string statusName)
        {
            var orderStatusDict = new Dictionary<string, int>
        {
            { "قيد المعالجة", 1 },
            { "تم التأكيد", 2 },
            { "قيد الشحن", 3 },
            { "تم التوصيل", 4 },
            { "تم الإلغاء", 5 },
            { "تم الإرجاع", 6 },
            { "تم الرفض", 7 }
        };

            return orderStatusDict.TryGetValue(statusName, out int statusId) ? statusId : 0;
        }
        //use when we return an order
        private async Task<bool> ProcessOfReturningOrders(int OrderId)
        {
            var OrderDetails = await _Context.OrderDetails
                .Where(Od => Od.OrderId == OrderId)
                .ToListAsync();

            foreach (var orderDetail in OrderDetails)
            {
                var productDetail = await _Context.ProductDetails
                    .FirstOrDefaultAsync(Pd => Pd.ProductDetailsId == orderDetail.ProductDetailsId);

                if (productDetail != null)
                {
                    productDetail.Quantity += orderDetail.Quantity; // استرجاع الكمية
                }
            }

            await _Context.SaveChangesAsync();
            return true;
        }
        //use when we Confirm Order
        private async Task<bool> ProcessOfConfirmingOrders(int OrderId)
        {
            var OrderDetails = await _Context.OrderDetails
                .Where(Od => Od.OrderId == OrderId)
                .ToListAsync();

            foreach (var orderDetail in OrderDetails)
            {
                var productDetail = await _Context.ProductDetails
                    .FirstOrDefaultAsync(Pd => Pd.ProductDetailsId == orderDetail.ProductDetailsId);

                if (productDetail != null)
                {
                    if (productDetail.Quantity >= orderDetail.Quantity) // التأكد من توفر الكمية
                    {
                        productDetail.Quantity -= orderDetail.Quantity; // خصم الكمية
                    }
                    else
                    {
                        throw new Exception("الكميه غير متوفره"); // فشل العملية بسبب عدم توفر كمية كافية
                    }
                }
            }

            await _Context.SaveChangesAsync();
            return true;
        }
        private string MessageBasedOnTheStatus(string statusName, string customerName, string orderNumber, string rejectionReason = "")
        {
            switch (statusName)
            {
                case "قيد المعالجة":
                    return $@"
عزيزي {customerName}،

نحن بصدد معالجة طلبك رقم {orderNumber} في سوق البلد. سنقوم بالمتابعة معك وتحديث حالته في أقرب وقت ممكن.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                case "تم التأكيد":
                    return $@"
عزيزي {customerName}،

نحن سعداء بإبلاغك أن طلبك رقم {orderNumber} في سوق البلد قد تم تأكيده بنجاح.
   سيتم تجهيز طلبك في أقرب وقت ممكن , يمكنك متابعه حاله طلبك من الموقع الرسمي❤.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                case "قيد الشحن":
                    return $@"
عزيزي {customerName}،

تم شحن طلبك رقم {orderNumber} في سوق البلد بنجاح. سيتم تسليمه قريباً إلى العنوان الذي قمت بتحديده.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                case "تم التوصيل":
                    return $@"
عزيزي {customerName}،

نحن سعيدون بإبلاغك أن طلبك رقم {orderNumber} في سوق البلد قد تم توصيله بنجاح إلى العنوان المحدد.

إذا كنت بحاجة إلى أي مساعدة إضافية، لا تتردد في التواصل مع فريق الدعم لدينا.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                case "تم الإلغاء":
                    return $@"
عزيزي {customerName}،

نأسف لإبلاغك أنه تم إلغاء طلبك رقم {orderNumber} في سوق البلد. إذا كنت بحاجة إلى معرفة المزيد من التفاصيل أو كان لديك أي استفسار، يمكنك التواصل مع فريق الدعم.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                case "تم الإرجاع":
                    return $@"
عزيزي {customerName}،

لقد تم إرجاع طلبك رقم {orderNumber} في سوق البلد بنجاح. إذا كنت بحاجة إلى المزيد من المساعدة أو لديك أي استفسار، لا تتردد في التواصل مع فريق الدعم لدينا.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                case "تم الرفض":
                    return $@"
عزيزي {customerName}،

نأسف لإبلاغك أن طلبك رقم {orderNumber} في سوق البلد قد تم رفضه. السبب في ذلك هو: {rejectionReason}.
إذا كنت بحاجة إلى أي مساعدة إضافية أو توضيحات، لا تتردد في التواصل معنا.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";

                default:
                    return $@"
عزيزي {customerName}،

تم تحديث حالة طلبك رقم {orderNumber} في سوق البلد.
إذا كنت بحاجة إلى المزيد من المعلومات أو المساعدة، لا تتردد في التواصل مع فريق الدعم لدينا.

مع أطيب التحيات،
فريق الدعم الفني
سوق البلد";
            }
        }

        public async Task<bool> UpdateOrderStatusByName(string statusName, int OrderId, string RejectionReason = "")
        {
            int StatusId = GetOrderStatusId(statusName);

            // العمليات الخاصة بتغير الكمية أو استرجاعها تبقى كما هي
            if (statusName == "تم الإرجاع")
            {
                await ProcessOfReturningOrders(OrderId);
            }
            else if (statusName == "تم التأكيد")
            {
                await ProcessOfConfirmingOrders(OrderId);
            }

            // تحديث حالة الطلب بالإضافة إلى سبب الرفض في حالة "تم الرفض"
            var order = await _Context.Orders
                .Include(o => o.Client) // تحميل العميل المرتبط بالطلب
                .ThenInclude(c => c.User) // تحميل المستخدم المرتبط بالعميل
                .FirstOrDefaultAsync(o => o.OrderId == OrderId);
            if (order != null)
            {
                order.RejectionReason = null!;  // تنظيف سبب الرفض
                order.OrderStatusId = (byte)StatusId;

                if (statusName == "تم الرفض")
                {
                    order.RejectionReason = RejectionReason;  // تخصيص سبب الرفض إذا كانت الحالة "تم الرفض"
                }

                _Context.Orders.Update(order);
                int RowsAffected = await _Context.SaveChangesAsync();

                {
                    string customerName = order.Client?.FirstName;  // استبدالها بالقيمة الفعلية
                    string orderNumber = order.OrderId.ToString();  // استبدالها بالقيمة الفعلية

                    string message = MessageBasedOnTheStatus(statusName, customerName, orderNumber, RejectionReason);

                    // التحقق من البريد الإلكتروني وإرسال الإشعار بناءً عليه
                    var clientEmail = order.Client?.User?.EmailOrAuthId;  // التأكد من الحصول على البريد الإلكتروني أو معرّف المصادقة

                    if (!string.IsNullOrEmpty(clientEmail))
                    {
                        // إذا كان البريد الإلكتروني موجودًا، أرسل الإشعار
                        await NotificationServices.NotificationsCreator.SendNotification(
                            $"تحديث حالة الطلب - {statusName}",
                            message,
                            clientEmail,  // البريد الإلكتروني الفعلي
                            "gmail");  // يمكنك استبدال "Gmail" بأي مزود إشعار آخر إن كان مختلفًا
                    }

                    return true;
                }
            }
            return false;
        }

        public async Task<List<OrdersDtos.AdminOrders.GetOrdersDetailsReq>> GetOrderDetails(int orderId)
        {
            var orderDetailsDto = await _Context.OrderDetails
                .Include(O => O.ProductDetails)
                    .ThenInclude(Pd => Pd.Product)
                .Include(O => O.ProductDetails.Size)
                .Include(O => O.ProductDetails.Color)
                .Where(O => O.OrderId == orderId)
                .Select(O => new OrdersDtos.AdminOrders.GetOrdersDetailsReq
                {
                    ProductId = O.ProductDetails.Product.ProductId,
                    ProductName = O.ProductDetails.Product.ProductName,
                    UnitPrice = O.UnitPrice,
                    ColorName = O.ProductDetails.Color.ColorName,
                    SizeName = O.ProductDetails.Size.SizeName,
                    Quantity = O.Quantity,
                    TotalPrice = O.UnitPrice * O.Quantity
                })
                .ToListAsync(); 

            return orderDetailsDto;
        }

    }
}
