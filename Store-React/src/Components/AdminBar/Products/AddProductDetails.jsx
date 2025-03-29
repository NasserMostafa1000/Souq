import React, { useState, useRef, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import API_BASE_URL, { ServerPath } from "../../Constant";
import { colors, sizes } from "../../utils"; // تأكد من أن هذه الاستيرادات صحيحة

export default function AddProductDetails() {
  const navigate = useNavigate();
  const location = useLocation();

  // استلام productId من الصفحة السابقة
  const { productId } = location.state || {};
  if (!productId) {
    return <div>لا يوجد معرف للمنتج. الرجاء العودة والمحاولة مرة أخرى.</div>;
  }

  // حالات النموذج
  const [colorId, setColorId] = useState("");
  const [sizeId, setSizeId] = useState("");
  const [quantity, setQuantity] = useState(1);
  const [productImage, setProductImage] = useState(""); // سيُخزن عنوان URL للصورة بعد رفعها
  const [selectedFile, setSelectedFile] = useState(null); // لتخزين الملف المُختار
  const [message, setMessage] = useState("");
  const [messageType, setMessageType] = useState(""); // لتحديد نوع الرسالة (ناجحة أو فاشلة)
  const [detailsAdded, setDetailsAdded] = useState(false);
  const [isLoading, setIsLoading] = useState(false); // حالة تحميل جديدة
  const [usePreviousImage, setUsePreviousImage] = useState(false); // تحديد إذا كان سيتم استخدام الصورة السابقة

  // ref لحقل رفع الصورة لإعادة تعيينه
  const fileInputRef = useRef(null);
  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  // دالة رفع الصورة إلى الخادم
  const handleImageUpload = async (file) => {
    const token = sessionStorage.getItem("token");
    const formData = new FormData();
    formData.append("imageFile", file);
    try {
      const response = await fetch(
        `${API_BASE_URL}Product/UploadProductImage`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
          },
          body: formData,
        }
      );
      if (!response.ok) {
        throw new Error("فشل رفع الصورة");
      }
      const data = await response.json();
      return data.imageUrl; // تأكد من أن API يُعيد المفتاح الصحيح (imageUrl)
    } catch (error) {
      console.error("Error uploading image:", error);
      return "";
    }
  };

  const handleFileChange = (e) => {
    if (e.target.files && e.target.files[0]) {
      setSelectedFile(e.target.files[0]);
    }
  };

  // دالة إرسال تفاصيل المنتج
  const handleSubmit = async (e) => {
    e.preventDefault();

    setIsLoading(true); // تعيين حالة التحميل إلى true

    let uploadedImageUrl = productImage;
    if (selectedFile) {
      uploadedImageUrl = await handleImageUpload(selectedFile);
      if (!uploadedImageUrl) {
        setMessage("فشل رفع الصورة. الرجاء المحاولة مرة أخرى.");
        setMessageType("error");
        setIsLoading(false); // تعيين حالة التحميل إلى false بعد الانتهاء
        return;
      }
      setProductImage(uploadedImageUrl);
    }
    const productDetails = {
      productDetailsId: 0, // سيتم تعيينها على الخادم
      productId: productId,
      colorId: Number(colorId),
      sizeId: Number(sizeId),
      quantity: Number(quantity),
      productImage: uploadedImageUrl,
    };

    try {
      const token = sessionStorage.getItem("token");
      const response = await fetch(
        `${API_BASE_URL}Product/PostProductDetails`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify(productDetails),
        }
      );

      if (!response.ok) {
        throw new Error("فشل إضافة تفاصيل المنتج");
      }

      const data = await response.json();
      setMessage(`تم الاضافه بنجاح`);
      setMessageType("success");
      setDetailsAdded(true);
      // إعادة ضبط الحقول بعد الإضافة
      setColorId("");
      setSizeId("");
      setQuantity(1);
      setSelectedFile(null);
      if (fileInputRef.current) {
        fileInputRef.current.value = null;
      }
    } catch (error) {
      setMessage(`خطأ: ${error.message}`);
      setMessageType("error");
    } finally {
      setIsLoading(false); // تعيين حالة التحميل إلى false بعد الانتهاء
    }
  };

  // زر "أضف تفاصيل أخرى" يعيد المستخدم إلى نفس النموذج لإضافة تفاصيل جديدة لنفس المنتج
  const handleAddMore = () => {
    setDetailsAdded(false);
    setMessage("");
    setMessageType("");
    setSelectedFile(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = null;
    }
  };

  // زر "إنهاء" ينقل المستخدم إلى صفحة أخرى (مثلاً قائمة المنتجات)
  const handleFinish = () => {
    navigate("/"); // عدل المسار حسب متطلباتك
  };

  return (
    <div
      className="add-product-details-container"
      style={{ padding: "20px", direction: "rtl" }}
    >
      <h2 style={{ textAlign: "center", marginBottom: "1rem" }}>
        إضافة تفاصيل المنتج
      </h2>
      {message && (
        <p
          style={{
            textAlign: "center",
            color: messageType === "success" ? "green" : "red",
          }}
        >
          {message}
        </p>
      )}
      {isLoading && (
        <div
          style={{
            textAlign: "center",
            padding: "20px",
          }}
        >
          <div
            className="spinner"
            style={{
              border: "4px solid #f3f3f3" /* Light grey */,
              borderTop: "4px solid #3498db" /* Blue */,
              borderRadius: "50%",
              width: "50px",
              height: "50px",
              animation: "spin 2s linear infinite",
            }}
          ></div>
        </div>
      )}
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="colorId"
            style={{ display: "block", marginBottom: "5px", color: "white" }}
          >
            اختر اللون (اجباري)
          </label>
          <select
            id="colorId"
            name="colorId"
            value={colorId}
            onChange={(e) => setColorId(e.target.value)}
            required
            style={{ width: "100%", padding: "8px" }}
          >
            <option value="">اختر اللون</option>
            {colors.map((color) => (
              <option key={color.ColorId} value={color.ColorId}>
                {color.ColorName}
              </option>
            ))}
          </select>
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="sizeId"
            style={{ display: "block", marginBottom: "5px", color: "white" }}
          >
            اختر المقاس (اختياري)
          </label>
          <select
            id="sizeId"
            name="sizeId"
            value={sizeId}
            onChange={(e) => setSizeId(e.target.value)}
            style={{ width: "100%", padding: "8px" }}
          >
            <option value="">اختر المقاس</option>
            {sizes.map((size) => (
              <option key={size.SizeId} value={size.SizeId}>
                {size.SizeName}
              </option>
            ))}
          </select>
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="quantity"
            style={{ display: "block", marginBottom: "5px", color: "white" }}
          >
            أدخل الكميه (اجباري)
          </label>
          <input
            type="number"
            id="quantity"
            name="quantity"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            required
            style={{ width: "100%", padding: "8px" }}
            placeholder="أدخل الكمية"
          />
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="productImage"
            style={{ display: "block", marginBottom: "5px", color: "white" }}
          >
            رفع صورة المنتج:
          </label>
          <input
            type="file"
            id="productImage"
            name="productImage"
            accept="image/*"
            onChange={handleFileChange}
            ref={fileInputRef}
            style={{ width: "100%", padding: "8px" }}
          />
        </div>
        {productImage && (
          <div>
            <img
              src={ServerPath + productImage}
              alt="معاينة الصورة"
              style={{ width: "100px", height: "100px" }}
            />
          </div>
        )}

        <div
          style={{
            marginBottom: "1.5rem",
            display: "flex",
            alignItems: "unset",
            gap: "8px",
          }}
        >
          <label
            style={{
              color: "white",
              cursor: "pointer",
              textDecoration: "underline",
            }}
            htmlFor="usePreviousImage"
          >
            استخدم الصورة السابقة؟
          </label>
          <input
            type="checkbox"
            id="usePreviousImage"
            checked={usePreviousImage}
            onChange={(e) => setUsePreviousImage(e.target.checked)}
          />
        </div>

        {!detailsAdded && (
          <button
            type="submit"
            style={{ padding: "10px 20px", fontSize: "16px" }}
          >
            إضافة تفاصيل المنتج
          </button>
        )}
      </form>

      {detailsAdded && (
        <div style={{ marginTop: "1rem", textAlign: "center" }}>
          <button
            onClick={handleAddMore}
            style={{
              padding: "10px 20px",
              fontSize: "16px",
              marginRight: "1rem",
              backgroundColor: "orange",
              color: "white",
              border: "none",
              borderRadius: "5px",
              cursor: "pointer",
            }}
          >
            أضف تفاصيل أخرى
          </button>
          <button
            onClick={handleFinish}
            style={{
              padding: "10px 20px",
              fontSize: "16px",
              backgroundColor: "green",
              color: "white",
              border: "none",
              borderRadius: "5px",
              cursor: "pointer",
            }}
          >
            إنهاء
          </button>
        </div>
      )}
    </div>
  );
}
