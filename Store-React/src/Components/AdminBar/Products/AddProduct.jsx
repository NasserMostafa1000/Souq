import { useState, useEffect } from "react";
import API_BASE_URL from "../../Constant";
import { useNavigate } from "react-router-dom";

export default function AddProduct() {
  const [productName, setProductName] = useState("");
  const [productPrice, setProductPrice] = useState("");
  const [discountPercentage, setDiscountPercentage] = useState("");
  const [categoryName, setCategoryName] = useState("");
  const [moreDetails, setMoreDetails] = useState("");
  const [categories, setCategories] = useState([]);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    window.scrollTo(0, 0);
  }, [message]);

  useEffect(() => {
    const token = sessionStorage.getItem("token");
    fetch(`${API_BASE_URL}Product/GetCategoriesNames`, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    })
      .then((res) => {
        if (!res.ok) {
          throw new Error("فشل جلب أسماء التصنيفات");
        }
        return res.json();
      })
      .then((data) => setCategories(data))
      .catch((error) => console.error("Error fetching categories:", error));
  }, []);

  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true); // تشغيل التحميل

    const newProduct = {
      productId: 0,
      productName,
      productPrice: Number(productPrice),
      discountPercentage: Number(discountPercentage),
      categoryName,
      moreDetails,
    };

    try {
      const token = sessionStorage.getItem("token");
      const response = await fetch(`${API_BASE_URL}Product/PostProduct`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(newProduct),
      });

      if (!response.ok) {
        throw new Error("فشل في إضافة المنتج");
      }

      const data = await response.json();
      setMessage(`تمت إضافة المنتج بنجاح. رقم المنتج: ${data.productId}`);
      navigate("/admins/AddProductDetails", { state: { productId: data.id } });
    } catch (error) {
      setMessage(`خطأ: ${error.message}`);
    } finally {
      setLoading(false); // إيقاف التحميل
    }
  };

  const handleDiscountChange = (e) => {
    let value = parseFloat(e.target.value);
    if (isNaN(value)) {
      setDiscountPercentage("");
      return;
    }
    setDiscountPercentage(Math.max(0, Math.min(100, value)));
  };

  return (
    <div
      className="add-product-container"
      style={{ padding: "20px", direction: "rtl" }}
    >
      <h2 style={{ textAlign: "center", marginBottom: "1rem" }}>
        أضف منتج جديد
      </h2>
      {message && <p style={{ textAlign: "center" }}>{message}</p>}

      {loading && (
        <div style={{ textAlign: "center", marginBottom: "1rem" }}>
          <div className="spinner"></div>
          <p>جاري الإضافة...</p>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="productName"
            style={{ display: "block", marginBottom: "5px" }}
          >
            اسم المنتج:
          </label>
          <input
            type="text"
            id="productName"
            name="productName"
            value={productName}
            onChange={(e) => setProductName(e.target.value)}
            required
            disabled={loading}
            style={{ width: "100%", padding: "8px" }}
            placeholder="أدخل اسم المنتج"
          />
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="productPrice"
            style={{ display: "block", marginBottom: "5px" }}
          >
            سعر المنتج:
          </label>
          <input
            type="number"
            id="productPrice"
            name="productPrice"
            value={productPrice}
            onChange={(e) => setProductPrice(e.target.value)}
            required
            disabled={loading}
            style={{ width: "100%", padding: "8px" }}
            placeholder="أدخل سعر المنتج"
          />
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="discountPercentage"
            style={{ display: "block", marginBottom: "5px" }}
          >
            نسبة الخصم المئوية:
          </label>
          <input
            type="number"
            id="discountPercentage"
            name="discountPercentage"
            min="0"
            step="0.01"
            max="100"
            value={discountPercentage}
            onChange={handleDiscountChange}
            required
            disabled={loading}
            style={{ width: "100%", padding: "8px" }}
            placeholder="أدخل نسبة الخصم (0-100)"
          />
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="categoryName"
            style={{ display: "block", marginBottom: "5px" }}
          >
            اسم التصنيف:
          </label>
          <select
            id="categoryName"
            name="categoryName"
            value={categoryName}
            onChange={(e) => setCategoryName(e.target.value)}
            required
            disabled={loading}
            style={{ width: "100%", padding: "8px" }}
          >
            <option value="">اختر تصنيف</option>
            {categories.map((cat, index) => (
              <option key={index} value={cat}>
                {cat}
              </option>
            ))}
          </select>
        </div>

        <div style={{ marginBottom: "1rem" }}>
          <label
            htmlFor="moreDetails"
            style={{ display: "block", marginBottom: "5px" }}
          >
            تفاصيل إضافية:
          </label>
          <textarea
            id="moreDetails"
            name="moreDetails"
            value={moreDetails}
            onChange={(e) => setMoreDetails(e.target.value)}
            style={{ width: "100%", padding: "8px", minHeight: "100px" }}
            placeholder="أدخل تفاصيل إضافية للمنتج"
            disabled={loading}
          ></textarea>
        </div>

        <button
          type="submit"
          disabled={loading}
          style={{ padding: "10px 20px", fontSize: "16px" }}
        >
          {loading ? "جاري الإضافة..." : "إضافة المنتج"}
        </button>
      </form>

      <style>
        {`
          .spinner {
            border: 4px solid rgba(0, 0, 0, 0.1);
            border-left-color: #333;
            border-radius: 50%;
            width: 40px;
            height: 40px;
            animation: spin 1s linear infinite;
            margin: auto;
          }
          @keyframes spin {
            to {
              transform: rotate(360deg);
            }
          }
        `}
      </style>
    </div>
  );
}
