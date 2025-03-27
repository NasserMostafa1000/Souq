import React, { useEffect, useState } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { GoogleLogin } from "@react-oauth/google";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { Helmet } from "react-helmet";
import { Howl } from "howler"; // استيراد مكتبة Howler.js
import "../../Styles/Login.css";
import API_BASE_URL from "../Constant.js";
import WebSiteLogo from "../../../public/WebsiteLogo/WebsiteLogo.jsx";
import { getRoleFromToken } from "../../Components/utils.js"; // التأكد من أنك قد وضعت هذه الميثود في ملف utils

export default function Login() {
  const [Email, setEmail] = useState("");
  const [Password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState("");
  const navigate = useNavigate();
  const location = useLocation();
  const { path } = location.state || "/";

  useEffect(() => {
    window.scrollTo(100, 100);
  }, [message]);

  // الاتصال بـ SignalR بناءً على الدور
  // التأكد من أن الاتصال يتم مرة واحدة فقط بعد التوثيق

  const handleLogin = async ({
    email = null,
    password = null,
    token = null,
    authProvider,
  }) => {
    try {
      const res = await fetch(`${API_BASE_URL}Users/Login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: email,
          password: password,
          token: token,
          authProvider: authProvider,
        }),
      });
      const data = await res.json();
      if (res.ok) {
        sessionStorage.setItem("token", data.token);
        path ? navigate(`${path}`) : navigate("/");
        setMessage("تم تسجيل الدخول بنجاح!");
        OpenSignalConnection();
        setMessageType("success");
      } else {
        setMessage(data.message || "فشل تسجيل الدخول. الرجاء المحاولة مجدداً.");
        setMessageType("error");
      }
    } catch (error) {
      setMessage(error.message);
      setMessageType("error");
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    handleLogin({
      email: Email,
      password: Password,
      authProvider: "Online Store",
    });
  };

  // ✅ تسجيل الدخول باستخدام Google
  const handleGoogleLoginSuccess = (response) => {
    const token = response.credential;
    handleLogin({ token, authProvider: "Google" });
  };

  const handleGoogleLoginFailure = () => {
    setMessage("فشل تسجيل الدخول باستخدام Google.");
    setMessageType("error");
  };

  return (
    <div className="login-container">
      <Helmet>
        <title>تسجيل الدخول |سوق البلد</title>
        <meta
          name="description"
          content="تسجيل الدخول إلى سوق البلد للتمتع بتجربة تسوق مميزة."
        />
      </Helmet>
      <div>
        <WebSiteLogo width={200} height={100} />
      </div>
      {message && <p className={`message ${messageType}`}>{message}</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label
            htmlFor="email"
            style={{ display: "block", marginBottom: "5px" }}
          >
            البريد الإلكتروني
          </label>
          <input
            style={{ backgroundColor: "darkgray" }}
            type="email"
            id="email"
            name="email"
            value={Email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="أدخل بريدك الإلكتروني"
            required
            autoComplete="email" // السماح باللصق والنسخ
          />
        </div>

        <div style={{ marginTop: "2rem" }}>
          <label
            htmlFor="password"
            style={{ display: "block", marginBottom: "5px" }}
          >
            كلمة المرور
          </label>
          <input
            style={{ backgroundColor: "darkgray" }}
            type="password"
            id="password"
            name="password"
            value={Password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="أدخل كلمة المرور"
            required
            autoComplete="current-password"
          />
          <p>
            <Link to="/forgot-password">هل نسيت كلمة المرور؟</Link>
          </p>
        </div>

        <button type="submit" style={{ marginTop: "2rem" }}>
          تسجيل الدخول
        </button>

        <div className="login-links">
          <p>
            لا تملك حسابًا؟{" "}
            <Link to="/register" style={{ color: "blue" }}>
              سجل الآن
            </Link>
          </p>
        </div>
      </form>

      <h4>👇أو تسجيل بنقرة واحدة👇</h4>

      <div className="social-buttons-container">
        <GoogleLogin
          onSuccess={handleGoogleLoginSuccess}
          onError={handleGoogleLoginFailure}
          useOneTap
        />
      </div>
    </div>
  );
}
