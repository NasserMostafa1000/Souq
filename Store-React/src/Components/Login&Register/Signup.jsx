import { useEffect, useState } from "react";
import "../../Styles/Signup.css";
import API_BASE_URL from "../Constant";
import { Helmet } from "react-helmet"; // تأكد من تثبيت react-helmet
import WebSiteLogo from "../../../public/WebsiteLogo/WebsiteLogo.jsx";

export default function Signup() {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    password: "",
    confirmPassword: "",
  });

  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState("");

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };
  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);
  const isPasswordStrong = (password) => {
    const passwordRegex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$/;
    return passwordRegex.test(password);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (formData.password !== formData.confirmPassword) {
      setMessage("كلمات المرور غير متطابقة");
      setMessageType("error");
      return;
    }

    // التحقق من قوة كلمة المرور
    if (!isPasswordStrong(formData.password)) {
      setMessage(
        "كلمة المرور يجب أن تحتوي على: 8 أحرف على الأقل، حرف كبير، رقم، ورمز خاص"
      );
      setMessageType("error");
      return;
    }

    try {
      // إرسال البيانات إلى API
      const response = await fetch(`${API_BASE_URL}Clients/PostClient`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          firstName: formData.firstName,
          secondName: formData.lastName,
          email: formData.email,
          phoneNumber: formData.phone,
          password: formData.password,
        }),
      });
      const data = await response.json();
      if (response.ok) {
        setMessage("تم التسجيل بنجاح!");
        setMessageType("success");
      } else {
        setMessage(data.message);
        setMessageType("error");
      }
    } catch (error) {
      // في حال حدوث خطأ في الاتصال
      setMessage(error.message);
      setMessageType("error");
    }
  };

  return (
    <div className="signup-container">
      <Helmet>
        <title>انشاء حساب|سوق البلد</title>
        <meta
          name="description"
          content="انشاء حساب في سوق البلد للتمتع بتجربة تسوق مميزة."
        />
      </Helmet>
      <div>
        <WebSiteLogo width={200} height={100} />
      </div>
     <h1>إنشاء حساب</h1>
      {message && <div className={`message ${messageType}`}>{message}</div>}
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          name="firstName"
          placeholder="الاسم الأول"
          value={formData.firstName}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="lastName"
          placeholder="الاسم الأخير"
          value={formData.lastName}
          onChange={handleChange}
          required
        />
        <input
          type="email"
          name="email"
          placeholder="البريد الإلكتروني"
          value={formData.email}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="phone"
          placeholder="رقم الهاتف"
          value={formData.phone}
          onChange={handleChange}
          required
        />
        <input
          type="password"
          name="password"
          placeholder="كلمة المرور"
          value={formData.password}
          onChange={handleChange}
          required
        />
        <input
          type="password"
          name="confirmPassword"
          placeholder="تأكيد كلمة المرور"
          value={formData.confirmPassword}
          onChange={handleChange}
          required
        />
        <button type="submit">تسجيل</button>
      </form>
    </div>
  );
}
