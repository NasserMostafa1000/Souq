import React, { useEffect, useState } from "react";
import API_BASE_URL from "../Constant";
import "../../Styles/ForgotPassword.css";
import { Helmet } from "react-helmet"; // تأكد من تثبيت react-helmet

export default function ForgotPassword() {
  const [email, setEmail] = useState(""); // حالة لتخزين البريد الإلكتروني
  const [message, setMessage] = useState(""); // حالة لتخزين الرسائل
  const [messageType, setMessageType] = useState(""); // نوع الرسالة (نجاح أو خطأ)
  const [loading, setLoading] = useState(false); // حالة لتخزين حالة التحميل
  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  const RecetPassword = async (e) => {
    e.preventDefault();
    setLoading(true); // تفعيل التحميل

    try {
      // إرسال البريد الإلكتروني إلى الخادم
      const response = await fetch(`${API_BASE_URL}Users/ForgotPassword`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          userProviderIdentifier: email,
          authProvider: "Gmail",
        }),
      });

      const data = await response.json();

      if (response.ok) {
        setMessage(data.message);
        setMessageType("success");
      } else {
        setMessage(data.message);
        setMessageType("error");
      }
    } catch (error) {
      setMessage(error.message);
      setMessageType("error");
    } finally {
      setLoading(false); // إيقاف التحميل
    }
  };

  return (
    <div className="forgot-password-container">
      <Helmet>
        <title>استرجاع كلمه السر|سوق البلد</title>
        <meta
          name="description"
          content="انشاء حساب في سوق البلد للتمتع بتجربة تسوق مميزة."
        />
      </Helmet>
      <h2>استعادة كلمة المرور</h2>
      {message && <p className={`${messageType}`}>{message}</p>}{" "}
      {/* عرض الرسائل */}
      <form onSubmit={RecetPassword}>
        <div>
          <label htmlFor="email">البريد الإلكتروني</label>
          <input
            type="email"
            id="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="أدخل بريدك الإلكتروني"
            required
          />
        </div>

        <button type="submit" disabled={loading}>
          {loading ? (
            <div className="spinner"></div> // دائرة التحميل
          ) : (
            "إرسال رابط إعادة تعيين كلمة المرور"
          )}
        </button>
      </form>
      {loading && (
        <p className="loading-message">جاري المعالجة، يرجى الانتظار...</p>
      )}
    </div>
  );
}
