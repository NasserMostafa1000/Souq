using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreDataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminInfo",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    WhatsAppNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "NVARCHAR(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.ColorId);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    OrderStatusId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    MethodId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Method = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.MethodId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "VARCHAR(55)", maxLength: 55, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "ShipPrices",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "TINYINT", nullable: false),
                    GovernorateName = table.Column<string>(type: "NVARCHAR(55)", maxLength: 55, nullable: false),
                    Price = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipPrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    SizeId = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeName = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false),
                    SizeCategory = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.SizeId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "NVARCHAR(250)", maxLength: 250, nullable: false),
                    ProductPrice = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    CategoryId = table.Column<byte>(type: "TINYINT", nullable: false),
                    MoreDetails = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailOrAuthId = table.Column<string>(type: "VARCHAR(250)", maxLength: 250, nullable: false),
                    AuthProvider = table.Column<string>(type: "VARCHAR(85)", maxLength: 85, nullable: true),
                    RoleId = table.Column<byte>(type: "TINYINT", nullable: false),
                    PasswordHash = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    Salt = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET() AT TIME ZONE 'Egypt Standard Time'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductsDetails",
                columns: table => new
                {
                    ProductDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<byte>(type: "TINYINT", nullable: false),
                    SizeId = table.Column<byte>(type: "TINYINT", nullable: true),
                    Quantity = table.Column<int>(type: "INT", nullable: false),
                    ProductImage = table.Column<string>(type: "VARCHAR(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsDetails", x => x.ProductDetailsId);
                    table.ForeignKey(
                        name: "FK_ProductsDetails_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "ColorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsDetails_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "SizeId");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "NVARCHAR(25)", maxLength: 25, nullable: false),
                    SecondName = table.Column<string>(type: "NVARCHAR(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Clients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartId);
                    table.ForeignKey(
                        name: "FK_Carts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientsAddresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Governorate = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false),
                    St = table.Column<string>(type: "NVARCHAR(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsAddresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_ClientsAddresses_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "SYSDATETIMEOFFSET() AT TIME ZONE 'Egypt Standard Time'"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    ShippingCoast = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    OrderStatusId = table.Column<byte>(type: "TINYINT", nullable: false),
                    TransactionNumber = table.Column<string>(type: "NVARCHAR(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(350)", maxLength: 350, nullable: false),
                    RejectionReason = table.Column<string>(type: "NVARCHAR(1000)", maxLength: 1000, nullable: true),
                    PaymentMethodId = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_OrderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatus",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "MethodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartsDetails",
                columns: table => new
                {
                    CartDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    ProductDetailsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "INT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartsDetails", x => x.CartDetailsId);
                    table.ForeignKey(
                        name: "FK_CartsDetails_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartsDetails_ProductsDetails_ProductDetailsId",
                        column: x => x.ProductDetailsId,
                        principalTable: "ProductsDetails",
                        principalColumn: "ProductDetailsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductDetailsId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "INT", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailsId);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId");
                    table.ForeignKey(
                        name: "FK_OrderDetails_ProductsDetails_ProductDetailsId",
                        column: x => x.ProductDetailsId,
                        principalTable: "ProductsDetails",
                        principalColumn: "ProductDetailsId");
                });

            migrationBuilder.InsertData(
                table: "AdminInfo",
                columns: new[] { "Id", "Email", "PhoneNumber", "TransactionNumber", "WhatsAppNumber" },
                values: new object[] { (byte)1, "info@website.com", "+201098765432", "1234567890", "+201234567890" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { (byte)1, "ملابس" },
                    { (byte)2, "أحذية" },
                    { (byte)3, "إكسسوارات" },
                    { (byte)4, "مجوهرات" },
                    { (byte)5, "ساعات" },
                    { (byte)6, "هواتف" },
                    { (byte)7, "سيارات" },
                    { (byte)8, "دراجات" },
                    { (byte)9, "إلكترونيات" },
                    { (byte)10, "أثاث" },
                    { (byte)11, "مواد غذائية" },
                    { (byte)12, "كتب" },
                    { (byte)13, "ألعاب" },
                    { (byte)14, "رياضة" },
                    { (byte)15, "أدوات مكتبية" },
                    { (byte)16, "أجهزة منزلية" },
                    { (byte)17, "مستحضرات تجميل" },
                    { (byte)18, "منتجات العناية الشخصية" },
                    { (byte)19, "مطبخ" },
                    { (byte)20, "أدوات تنظيف" },
                    { (byte)21, "أدوية" },
                    { (byte)22, "معدات صناعية" },
                    { (byte)23, "حقائب" },
                    { (byte)24, "أدوات حدائق" },
                    { (byte)25, "إضاءة" },
                    { (byte)26, "أخري" }
                });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "ColorId", "ColorName" },
                values: new object[,]
                {
                    { (byte)1, "أحمر" },
                    { (byte)2, "أزرق" },
                    { (byte)3, "أخضر" },
                    { (byte)4, "أصفر" },
                    { (byte)5, "أسود" },
                    { (byte)6, "أبيض" },
                    { (byte)7, "رمادي" },
                    { (byte)8, "برتقالي" },
                    { (byte)9, "بنفسجي" },
                    { (byte)10, "وردي" },
                    { (byte)11, "بني" },
                    { (byte)12, "ذهبي" },
                    { (byte)13, "فضي" },
                    { (byte)14, "تركواز" },
                    { (byte)15, "نيلي" },
                    { (byte)16, "كحلي" },
                    { (byte)17, "عنابي" },
                    { (byte)18, "بيج" },
                    { (byte)19, "خردلي" },
                    { (byte)20, "فيروزي" },
                    { (byte)21, "زهري" },
                    { (byte)22, "أرجواني" },
                    { (byte)23, "لافندر" },
                    { (byte)24, "موف" },
                    { (byte)25, "ليموني" },
                    { (byte)26, "أخضر زيتي" },
                    { (byte)27, "أخضر فاتح" },
                    { (byte)28, "أزرق سماوي" },
                    { (byte)29, "أزرق ملكي" },
                    { (byte)30, "قرمزي" }
                });

            migrationBuilder.InsertData(
                table: "OrderStatus",
                columns: new[] { "OrderStatusId", "StatusName" },
                values: new object[,]
                {
                    { (byte)1, "قيد المعالجة" },
                    { (byte)2, "تم التأكيد" },
                    { (byte)3, "قيد الشحن" },
                    { (byte)4, "تم التوصيل" },
                    { (byte)5, "تم الإلغاء" },
                    { (byte)6, "تم الإرجاع" },
                    { (byte)7, "تم الرفض" }
                });

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "MethodId", "Method" },
                values: new object[,]
                {
                    { (byte)1, "المحفظة الإلكترونية" },
                    { (byte)2, "الدفع عند الاستلام" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { (byte)1, "Admin" },
                    { (byte)2, "Manager" },
                    { (byte)3, "User" },
                    { (byte)4, "Shipping Manager" }
                });

            migrationBuilder.InsertData(
                table: "ShipPrices",
                columns: new[] { "Id", "GovernorateName", "Price" },
                values: new object[,]
                {
                    { (byte)1, "القاهرة", 50.00m },
                    { (byte)2, "الجيزة", 55.00m },
                    { (byte)3, "الإسكندرية", 60.00m },
                    { (byte)4, "القليوبية", 45.00m },
                    { (byte)5, "الغربية", 48.00m },
                    { (byte)6, "الشرقية", 52.00m },
                    { (byte)7, "الدقهلية", 50.00m },
                    { (byte)8, "البحيرة", 58.00m },
                    { (byte)9, "المنوفية", 47.00m },
                    { (byte)10, "بني سويف", 53.00m },
                    { (byte)11, "الفيوم", 49.00m },
                    { (byte)12, "المنيا", 55.00m },
                    { (byte)13, "سوهاج", 57.00m },
                    { (byte)14, "أسيوط", 56.00m },
                    { (byte)15, "قنا", 60.00m },
                    { (byte)16, "الأقصر", 65.00m },
                    { (byte)17, "أسوان", 68.00m },
                    { (byte)18, "دمياط", 62.00m },
                    { (byte)19, "بورسعيد", 58.00m },
                    { (byte)20, "الإسماعيلية", 55.00m },
                    { (byte)21, "السويس", 60.00m },
                    { (byte)22, "شمال سيناء", 70.00m },
                    { (byte)23, "جنوب سيناء", 72.00m },
                    { (byte)24, "مرسى مطروح", 75.00m },
                    { (byte)25, "البحر الأحمر", 80.00m },
                    { (byte)26, "كفر الشيخ", 58.00m },
                    { (byte)27, "الوادي الجديد", 85.00m }
                });

            migrationBuilder.InsertData(
                table: "Sizes",
                columns: new[] { "SizeId", "SizeCategory", "SizeName" },
                values: new object[,]
                {
                    { (byte)1, "ملابس", "S" },
                    { (byte)2, "ملابس", "M" },
                    { (byte)3, "ملابس", "L" },
                    { (byte)4, "ملابس", "XL" },
                    { (byte)5, "ملابس", "XXL" },
                    { (byte)6, "ملابس", "XXXL" },
                    { (byte)7, "ملابس", "XXXXL" },
                    { (byte)8, "ملابس", "XXXXXL" },
                    { (byte)9, "حماله صدر", "A" },
                    { (byte)10, "حماله صدر", "B" },
                    { (byte)11, "حماله صدر", "C" },
                    { (byte)12, "حماله صدر", "D" },
                    { (byte)13, "حماله صدر", "E" },
                    { (byte)14, "حماله صدر", "F" },
                    { (byte)15, "بناطيل", "22" },
                    { (byte)16, "بناطيل", "23" },
                    { (byte)17, "بناطيل", "24" },
                    { (byte)18, "بناطيل", "25" },
                    { (byte)19, "بناطيل", "26" },
                    { (byte)20, "بناطيل", "27" },
                    { (byte)21, "بناطيل", "28" },
                    { (byte)22, "بناطيل", "29" },
                    { (byte)23, "بناطيل", "30" },
                    { (byte)24, "بناطيل", "31" },
                    { (byte)25, "بناطيل", "32" },
                    { (byte)26, "بناطيل", "33" },
                    { (byte)27, "بناطيل", "34" },
                    { (byte)28, "بناطيل", "35" },
                    { (byte)29, "بناطيل/احذيه", "36" },
                    { (byte)30, "بناطيل/احذيه", "37" },
                    { (byte)31, "بناطيل/احذيه", "38" },
                    { (byte)32, "بناطيل/احذيه", "39" },
                    { (byte)33, "بناطيل/احذيه", "40" },
                    { (byte)34, "بناطيل/احذيه", "41" },
                    { (byte)35, "بناطيل/احذيه", "42" },
                    { (byte)36, "بناطيل/احذيه", "43" },
                    { (byte)37, "بناطيل/احذيه", "44" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ClientId",
                table: "Carts",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartId",
                table: "CartsDetails",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartsDetails_ProductDetailsId",
                table: "CartsDetails",
                column: "ProductDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientsAddresses_ClientId",
                table: "ClientsAddresses",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Colors_ColorName",
                table: "Colors",
                column: "ColorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductDetailsId",
                table: "OrderDetails",
                column: "ProductDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientId",
                table: "Orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ColorId_productId",
                table: "ProductsDetails",
                columns: new[] { "ColorId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ColorId_SizeId_ProductId",
                table: "ProductsDetails",
                columns: new[] { "ColorId", "SizeId", "ProductDetailsId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductsDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsDetails_SizeId",
                table: "ProductsDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailOrAuthId",
                table: "Users",
                column: "EmailOrAuthId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminInfo");

            migrationBuilder.DropTable(
                name: "CartsDetails");

            migrationBuilder.DropTable(
                name: "ClientsAddresses");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ShipPrices");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductsDetails");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "OrderStatus");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
