import React, { useState, useEffect, useRef, useCallback } from "react";
import { Helmet } from "react-helmet";
import { useNavigate } from "react-router-dom";
import NavBar from "./Nav";
import ProductItem from "../Products/ProductItem.jsx";
import API_BASE_URL from "../Constant.js";
import CategoryItems from "./CategoryItems.jsx";
import "../../Styles/Home.css";
import { getRoleFromToken } from "../utils.js";
import ContactUs from "./ContactUs.jsx";
import WebSiteLogo from "../../../public/WebsiteLogo/WebsiteLogo.jsx";
export default function Home() {
  const [products, setProducts] = useState([]);
  const [clothesProducts, setClothesProducts] = useState([]);
  const [ProductsPage, setProductsPage] = useState(1);
  const [ClothesPage, setClothesPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  const [hasMoreClothes, setHasMoreClothes] = useState(true);
  const [loading, setLoading] = useState(true);
  const [loadingClothes, setLoadingClothes] = useState(false);
  const navigate = useNavigate();

  const productsRef = useRef(null);
  const clothesRef = useRef(null);

  const fetchProducts = async () => {
    if (!hasMore) return;

    try {
      const response = await fetch(
        `${API_BASE_URL}Product/GetDiscountProducts?page=${ProductsPage}&limit=5`
      );
      if (!response.ok) throw new Error("Network error");

      const data = await response.json();
      if (data.length === 0) setHasMore(false);
      else {
        setProducts((prev) => [...prev, ...data]);
        setProductsPage((prev) => prev + 1);
      }
    } catch (error) {
      console.error("Error fetching discount products:", error);
    } finally {
      setLoading(false);
    }
  };

  const fetchClothesProducts = async () => {
    if (!hasMoreClothes || loadingClothes) return;
    setLoadingClothes(true);

    try {
      const response = await fetch(
        `${API_BASE_URL}Product/GetProductsWhereInClothesCategory?page=${ClothesPage}&limit=5`
      );
      if (!response.ok) throw new Error("Network error");

      const data = await response.json();
      if (data.length === 0) setHasMoreClothes(false);
      else {
        setClothesProducts((prev) => [...prev, ...data]);
        setClothesPage((prev) => prev + 1);
      }
    } catch (error) {
      console.error("Error fetching clothes products:", error);
    } finally {
      setLoadingClothes(false);
    }
  };

  const handleScroll = useCallback((ref, fetchMore) => {
    if (!ref.current) return;
    const { scrollLeft, scrollWidth, clientWidth } = ref.current;
    const isRtl = getComputedStyle(ref.current).direction === "ltr";

    // Handle RTL scroll detection
    const isAtEnd = isRtl
      ? scrollLeft <= 50
      : scrollLeft + clientWidth >= scrollWidth - 10;

    if (isAtEnd) fetchMore();
  }, []);
  useEffect(() => {
    fetchProducts();
    fetchClothesProducts();
  }, []);

  const handleHorizontalScroll = useCallback((ref, fetchMore) => {
    if (!ref.current) return;
    const { scrollLeft, scrollWidth, clientWidth } = ref.current;
    if (scrollLeft + clientWidth >= scrollWidth - 10) {
      fetchMore();
    }
  }, []);

  useEffect(() => {
    const productsDiv = productsRef.current;
    const clothesDiv = clothesRef.current;

    const productsScrollHandler = () =>
      handleScroll(productsRef, fetchProducts);
    const clothesScrollHandler = () =>
      handleScroll(clothesRef, fetchClothesProducts);

    if (productsDiv)
      productsDiv.addEventListener("scroll", productsScrollHandler);
    if (clothesDiv) clothesDiv.addEventListener("scroll", clothesScrollHandler);

    return () => {
      if (productsDiv)
        productsDiv.removeEventListener("scroll", productsScrollHandler);
      if (clothesDiv)
        clothesDiv.removeEventListener("scroll", clothesScrollHandler);
    };
  }, [handleScroll, fetchProducts, fetchClothesProducts]);

  function handleProductClick(product) {
    navigate(`/productDetails/${product.productId}`, {
      state: { product },
      replace: true,
    });
  }

  if (loading) {
    return (
      <div
        style={{
          backgroundColor: "grey",
          height: "100vh",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <h2
          style={{
            fontFamily: "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif",
            fontSize: "36px",
            color: "black",
            margin: "0 0 20px",
          }}
        >
          سوق البلد يرحب بكم
        </h2>
        <WebSiteLogo width={200} height={100} />
      </div>
    );
  }

  return (
    <div>
      <Helmet>
        <title>الصفحه الرئيسيه - سوق البلد</title>
        <meta
          name="description"
          content="في سوق البلد يمكنك تسوق منتجات بأفضل جوده وأفضل ماركات عالميه يتحدث عنها البشر , مع خصومات تصل الي خمسون بالمائة"
        />
      </Helmet>
      <NavBar />
      <img
        src="/ProjectImages/Discounts.jpeg"
        alt="Discounts"
        style={{ width: "100%", height: "40vh" }}
      />

      <h1 className="discount-title">
        <span>%60</span> تسوق منتجات مع خصومات تصل إلى
      </h1>

      <div className="products-container" ref={productsRef}>
        {products.map((product) => (
          <div
            onClick={() => handleProductClick(product)}
            key={product.productId}
          >
            <ProductItem
              product={product}
              CurrentRole={getRoleFromToken(sessionStorage.getItem("token"))}
            />
          </div>
        ))}
      </div>

      <CategoryItems />
      <h1 className="discount-title">تسوق احدث موديلات الملابس</h1>

      <div className="products-container" ref={clothesRef}>
        {loadingClothes ? (
          <p style={{ textAlign: "center" }}>جارٍ تحميل منتجات الملابس...</p>
        ) : clothesProducts.length > 0 ? (
          clothesProducts.map((product) => (
            <div
              onClick={() => handleProductClick(product)}
              key={product.productId}
            >
              <ProductItem
                product={product}
                CurrentRole={getRoleFromToken(sessionStorage.getItem("token"))}
              />
            </div>
          ))
        ) : (
          <p style={{ textAlign: "center" }}>لا توجد منتجات للملابس حاليا.</p>
        )}
      </div>

      <ContactUs />
    </div>
  );
}
