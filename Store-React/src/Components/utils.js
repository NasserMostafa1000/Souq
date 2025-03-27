import API_BASE_URL, { ServerPath } from "./Constant";
import * as signalR from "@microsoft/signalr";
let connection = null; // تعريف الاتصال كمتغير عام
export const SendSignalMessageForOrders = async (message) => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${ServerPath}/orderHub`, {
      withCredentials: false, // مهم جداً للسماح بالـ CORS
    }) // استبدل `your-api-url` برابط API الخاص بك
    .withAutomaticReconnect() // يعيد الاتصال تلقائيًا عند الانقطاع
    .build();

  try {
    await connection.start();
    await connection.invoke("SendMessage", message);
    console.log("📤 Message sent:", message);
  } catch (error) {
    console.error("❌ Connection failed:", error);
  } finally {
    await connection.stop();
  }
};
export const startListeningToMessages = async (onMessageReceived) => {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(`${ServerPath}/orderHub`, {
        withCredentials: false, // مهم جداً للسماح بالـ CORS
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }

  try {
    await connection.start();
    connection.on("ReceiveMessage", (message) => {
      if (onMessageReceived) {
        onMessageReceived(message);
      }
    });
  } catch (error) {
    console.error("❌ Connection failed:", error);
  }
};

export const stopListeningToMessages = () => {
  if (connection) {
    connection.stop();
    connection = null;
  }
};
export default function getDeliveryDate() {
  const today = new Date();
  today.setDate(today.getDate() + 7);
  return today.toLocaleDateString("ar-EG", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  });
}

export const egyptianGovernorates = [
  "القاهرة",
  "الجيزة",
  "الإسكندرية",
  "القليوبية",
  "الغربية",
  "الشرقية",
  "الدقهلية",
  "البحيرة",
  "المنوفية",
  "بني سويف",
  "الفيوم",
  "المنيا",
  "سوهاج",
  "أسيوط",
  "قنا",
  "الأقصر",
  "أسوان",
  "دمياط",
  "بورسعيد",
  "الإسماعيلية",
  "السويس",
  "شمال سيناء",
  "جنوب سيناء",
  "مرسى مطروح",
  "البحر الأحمر",
  "كفر الشيخ",
  "الوادي الجديد",
];
export function getRoleFromToken(token) {
  if (!token) return null;
  try {
    // فك تشفير التوكن (JWT)
    const payload = JSON.parse(atob(token.split(".")[1]));

    return payload.role || null;
  } catch (error) {
    console.error("Invalid token:", error);
    return null;
  }
}
export const colors = [
  { ColorId: 1, ColorName: "أحمر" },
  { ColorId: 2, ColorName: "أزرق" },
  { ColorId: 3, ColorName: "أخضر" },
  { ColorId: 4, ColorName: "أصفر" },
  { ColorId: 5, ColorName: "أسود" },
  { ColorId: 6, ColorName: "أبيض" },
  { ColorId: 7, ColorName: "رمادي" },
  { ColorId: 8, ColorName: "برتقالي" },
  { ColorId: 9, ColorName: "بنفسجي" },
  { ColorId: 10, ColorName: "وردي" },
  { ColorId: 11, ColorName: "بني" },
  { ColorId: 12, ColorName: "ذهبي" },
  { ColorId: 13, ColorName: "فضي" },
  { ColorId: 14, ColorName: "تركواز" },
  { ColorId: 15, ColorName: "نيلي" },
  { ColorId: 16, ColorName: "كحلي" },
  { ColorId: 17, ColorName: "عنابي" },
  { ColorId: 18, ColorName: "بيج" },
  { ColorId: 19, ColorName: "خردلي" },
  { ColorId: 20, ColorName: "فيروزي" },
  { ColorId: 21, ColorName: "زهري" },
  { ColorId: 22, ColorName: "أرجواني" },
  { ColorId: 23, ColorName: "لافندر" },
  { ColorId: 24, ColorName: "موف" },
  { ColorId: 25, ColorName: "ليموني" },
  { ColorId: 26, ColorName: "أخضر زيتي" },
  { ColorId: 27, ColorName: "أخضر فاتح" },
  { ColorId: 28, ColorName: "أزرق سماوي" },
  { ColorId: 29, ColorName: "أزرق ملكي" },
  { ColorId: 30, ColorName: "قرمزي" },
];
export const sizes = [
  { SizeId: 1, SizeName: "S" },
  { SizeId: 2, SizeName: "M" },
  { SizeId: 3, SizeName: "L" },
  { SizeId: 4, SizeName: "XL" },
  { SizeId: 5, SizeName: "XXL" },
  { SizeId: 6, SizeName: "XXX" },
  { SizeId: 7, SizeName: "XXXL" },
  { SizeId: 8, SizeName: "XXXX" },
  { SizeId: 9, SizeName: "A" },
  { SizeId: 10, SizeName: "B" },
  { SizeId: 11, SizeName: "C" },
  { SizeId: 12, SizeName: "D" },
  { SizeId: 13, SizeName: "E" },
  { SizeId: 14, SizeName: "F" },
  { SizeId: 15, SizeName: "22" },
  { SizeId: 16, SizeName: "23" },
  { SizeId: 17, SizeName: "24" },
  { SizeId: 18, SizeName: "25" },
  { SizeId: 19, SizeName: "26" },
  { SizeId: 20, SizeName: "27" },
  { SizeId: 21, SizeName: "28" },
  { SizeId: 22, SizeName: "29" },
  { SizeId: 23, SizeName: "30" },
  { SizeId: 24, SizeName: "31" },
  { SizeId: 25, SizeName: "32" },
  { SizeId: 26, SizeName: "33" },
  { SizeId: 27, SizeName: "34" },
  { SizeId: 28, SizeName: "35" },
  { SizeId: 29, SizeName: "36" },
  { SizeId: 30, SizeName: "37" },
  { SizeId: 31, SizeName: "38" },
  { SizeId: 32, SizeName: "39" },
  { SizeId: 33, SizeName: "40" },
  { SizeId: 34, SizeName: "41" },
  { SizeId: 35, SizeName: "42" },
  { SizeId: 36, SizeName: "43" },
  { SizeId: 37, SizeName: "44" },
];
