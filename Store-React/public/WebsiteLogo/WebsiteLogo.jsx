import React from "react";

const SouqAlBaladLogo = ({ width = 250, height = 100 }) => {
  return (
    <svg
      width={width}
      height={height}
      viewBox="0 0 500 200"
      xmlns="http://www.w3.org/2000/svg"
    >
      {/* خلفية شفافة */}
      <rect width="500" height="200" fill="none" />

      {/* تعريف التدرجات اللونية */}
      <defs>
        {/* تدرج لوني للهرم الأكبر */}
        <linearGradient
          id="pyramidGradient1"
          x1="0%"
          y1="0%"
          x2="100%"
          y2="100%"
        >
          <stop offset="0%" stopColor="#C2B280" />
          <stop offset="50%" stopColor="#A68A64" />
          <stop offset="100%" stopColor="#8B7355" />
        </linearGradient>
        {/* تدرج لوني للهرم الأوسط */}
        <linearGradient
          id="pyramidGradient2"
          x1="0%"
          y1="0%"
          x2="100%"
          y2="100%"
        >
          <stop offset="0%" stopColor="#B8A07A" />
          <stop offset="50%" stopColor="#A68A64" />
          <stop offset="100%" stopColor="#8B7355" />
        </linearGradient>
        {/* تدرج لوني للهرم الأصغر */}
        <linearGradient
          id="pyramidGradient3"
          x1="0%"
          y1="0%"
          x2="100%"
          y2="100%"
        >
          <stop offset="0%" stopColor="#A68A64" />
          <stop offset="50%" stopColor="#8B7355" />
          <stop offset="100%" stopColor="#6B5A4A" />
        </linearGradient>
      </defs>

      {/* رسم الأهرامات المصرية ثلاثية الأبعاد */}
      <g transform="translate(50,50)">
        {/* الهرم الأكبر (خوفو) */}
        <polygon
          points="0,100 100,20 200,100"
          fill="url(#pyramidGradient1)"
          stroke="#A68A64"
          strokeWidth="2"
        />
        {/* الجانب الأيسر للهرم الأكبر */}
        <polygon
          points="0,100 100,20 100,100"
          fill="#A68A64"
          stroke="#8B7355"
          strokeWidth="2"
        />
        {/* الجانب الأيمن للهرم الأكبر */}
        <polygon
          points="100,20 200,100 100,100"
          fill="#8B7355"
          stroke="#A68A64"
          strokeWidth="2"
        />

        {/* الهرم الأوسط (خفرع) */}
        <polygon
          points="220,100 290,40 360,100"
          fill="url(#pyramidGradient2)"
          stroke="#A68A64"
          strokeWidth="2"
        />
        {/* الجانب الأيسر للهرم الأوسط */}
        <polygon
          points="220,100 290,40 290,100"
          fill="#A68A64"
          stroke="#8B7355"
          strokeWidth="2"
        />
        {/* الجانب الأيمن للهرم الأوسط */}
        <polygon
          points="290,40 360,100 290,100"
          fill="#8B7355"
          stroke="#A68A64"
          strokeWidth="2"
        />

        {/* الهرم الأصغر (منقرع) */}
        <polygon
          points="380,100 420,60 460,100"
          fill="url(#pyramidGradient3)"
          stroke="#8B7355"
          strokeWidth="2"
        />
        {/* الجانب الأيسر للهرم الأصغر */}
        <polygon
          points="380,100 420,60 420,100"
          fill="#8B7355"
          stroke="#6B5A4A"
          strokeWidth="2"
        />
        {/* الجانب الأيمن للهرم الأصغر */}
        <polygon
          points="420,60 460,100 420,100"
          fill="#6B5A4A"
          stroke="#8B7355"
          strokeWidth="2"
        />
      </g>
    </svg>
  );
};

export default SouqAlBaladLogo;
